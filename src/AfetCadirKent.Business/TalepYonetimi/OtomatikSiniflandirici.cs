using System.Threading.Channels;
using AfetCadirKent.Business.YapayZeka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AfetCadirKent.Business.TalepYonetimi;

/// <summary>Türü belirlenmeden açılan talepleri yapay zeka sınıflandırmasına sıraya alır (K-080).</summary>
public interface ITalepSiniflandirmaKuyrugu
{
    /// <summary>Talebi sıraya alır ve beklemeden döner; talebi açan kullanıcı (ör. kiosk) yapay zekayı beklemez.</summary>
    void Ekle(int talepId);
}

/// <summary>
/// Kayıt masası, görevli veya kiosktan türü belirlenmeden açılan talepleri arka planda, sırayla yapay zekaya gönderir (K-080).
/// Böylece sağlık talebi kimse buton beklemeden sağlık personeline yönlenir (K-077). İşlem sistem adına yapılır
/// (işlem geçmişinde "sistem"). Yapay zeka çalışmıyorsa talep "Sınıflandırılmamış" kalır; görevli elle sınıflandırır veya
/// talep ekranından yeniden öneri ister. Hatalar loglanır, arka plan döngüsünü durdurmaz.
/// </summary>
public sealed class OtomatikSiniflandirici(IVeriOturumuFabrikasi veriFabrikasi, TimeProvider zaman, IYapayZekaServisi yapayZeka,
    ILogger<OtomatikSiniflandirici>? log = null) : ITalepSiniflandirmaKuyrugu
{
    /// <summary>Giriş yapılmamış oturum: işlem geçmişine kullanıcı olarak "sistem" yazılır.</summary>
    private static readonly IOturum SistemOturumu = new Oturum();

    private readonly Channel<int> _kanal = Channel.CreateUnbounded<int>(new UnboundedChannelOptions { SingleReader = true });
    private readonly ILogger _log = log ?? NullLogger<OtomatikSiniflandirici>.Instance;

    public void Ekle(int talepId) => _kanal.Writer.TryWrite(talepId);

    /// <summary>Uygulama açılışında: daha önce sınıflandırılamamış açık talepleri sıraya alır (ör. Ollama kapalıyken açılanlar).</summary>
    public async Task<int> BekleyenleriEkleAsync(CancellationToken iptal = default)
    {
        await using var veri = veriFabrikasi.Ac();
        var idler = await veri.Repo<Talep>().Sorgu().AsNoTracking()
            .Where(t => t.Tur == null && t.SiniflandirmaDurumu == SiniflandirmaDurumu.Siniflandirilmamis
                        && t.Durum != TalepDurumu.Tamamlandi && t.Durum != TalepDurumu.Iptal)
            .OrderBy(t => t.Id).Select(t => t.Id).ToListAsync(iptal);
        foreach (var id in idler)
            Ekle(id);
        return idler.Count;
    }

    /// <summary>Sıradaki talepleri uygulama kapanana (iptal edilene) kadar işler. UI açılışta bir kez başlatır.</summary>
    public async Task CalistirAsync(CancellationToken iptal)
    {
        await foreach (var talepId in _kanal.Reader.ReadAllAsync(iptal))
            await IsleAsync(talepId, iptal);
    }

    /// <summary>
    /// Tek talebi işler; öneri kaydedildiyse true. Talep bu arada kapanmış veya sınıflandırılmışsa dokunmaz.
    /// Kullanıcı iptali dışında istisna fırlatmaz.
    /// </summary>
    public async Task<bool> IsleAsync(int talepId, CancellationToken iptal = default)
    {
        try
        {
            await using var veri = veriFabrikasi.Ac();
            var talep = await veri.Repo<Talep>().Sorgu().FirstOrDefaultAsync(t => t.Id == talepId, iptal);
            if (talep is null || !talep.AcikMi || talep.Tur is not null || talep.SiniflandirmaDurumu != SiniflandirmaDurumu.Siniflandirilmamis)
                return false;

            var sonuc = await yapayZeka.TalepSiniflandirAsync(talep.Aciklama, iptal);
            if (sonuc.Deger is not { } oneri)
            {
                _log.LogInformation("Talep #{TalepId} otomatik sınıflandırılamadı, görevli elle devam edecek: {Hata}", talepId, sonuc.HataMesaji);
                return false;
            }

            await YzOnerisiKaydedici.KaydetAsync(veri, SistemOturumu, zaman, talep, oneri, iptal);
            return true;
        }
        catch (OperationCanceledException) when (iptal.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Ör. görevli talebi aynı anda sınıflandırdı (eşzamanlılık) veya veritabanına ulaşılamadı.
            _log.LogWarning(ex, "Talep #{TalepId} otomatik sınıflandırması kaydedilemedi", talepId);
            return false;
        }
    }
}

/// <summary>Yapay zeka önerisini talebe yazıp işlem geçmişiyle kaydeder (elle istenen ve otomatik öneri aynı yoldan geçer).</summary>
internal static class YzOnerisiKaydedici
{
    public static async Task KaydetAsync(IVeriOturumu veri, IOturum oturum, TimeProvider zaman, Talep talep,
        TalepSiniflandirmaOnerisi oneri, CancellationToken iptal)
    {
        talep.YapayZekaOnerisiKaydet(oneri.Tur, oneri.YardimOnceligi, oneri.Gerekce);
        var oncelik = talep.SaglikPersoneliDegerlendirecek ? "Sağlık personeli değerlendirecek; doğrudan sağlık personeline yönlendirildi"
            : $"yardım önceliği {(talep.YzOnerilenOncelik is { } o ? o.Metin() : "—")}; görevli onayı bekleniyor";
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Guncelleme, nameof(Talep), talep.Id,
            $"Talep #{talep.Id} için yapay zeka önerisi: {oneri.Tur.Metin()} ({oncelik}).");
        await veri.GuvenliKaydetAsync(iptal);
    }
}
