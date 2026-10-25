using Microsoft.EntityFrameworkCore;
using AfetCadirKent.Business.TalepYonetimi;

namespace AfetCadirKent.Business.KioskIslemleri;

/// <summary>Kioskta giriş yapmış aile. Çadır ve iletişim bilgisi içermez (K-052).</summary>
public sealed record KioskAilesi(int AileId, string AileNo, string AileAdi);

/// <summary>Kioskta ailenin gördüğü talep satırı. Sorumlu görevlinin adı gösterilmez.</summary>
public sealed record KioskTalebi(int Id, string Aciklama, TalepTuru? Tur, TalepDurumu Durum, DateTime AcilisZamani,
    DateTime? KapanisZamani, string? Sonuc)
{
    public string TurMetni => Tur.Metin();
    public string DurumMetni => Durum.Metin();
}

public sealed record KioskDuyurusu(int Id, string Baslik, string Metin, DateTime BaslangicTarihi, DateTime BitisTarihi);

public interface IKioskServisi
{
    /// <summary>Kioskta o an giriş yapmış aile; yoksa null.</summary>
    KioskAilesi? Aile { get; }

    /// <summary>Aile numarası ve ailedeki çadır kentte bulunan bir üyenin doğum yılıyla giriş (şartname M11).</summary>
    Task<KioskAilesi> AileGirisiAsync(string aileNo, int dogumYili, CancellationToken iptal = default);

    /// <summary>Aile oturumunu kapatır; kiosk, sıradaki aile için giriş ekranına döner.</summary>
    void AileCikisi();

    /// <summary>
    /// Ailenin talebi "Kiosk" kaynağıyla, sınıflandırılmamış olarak açılır ve arka planda yapay zeka sınıflandırmasına
    /// sıraya alınır (K-080); son kararı görevli verir.
    /// </summary>
    Task<KioskTalebi> TalepBirakAsync(string aciklama, CancellationToken iptal = default);

    /// <summary>Ailenin talepleri ve durumları (en yeni önce).</summary>
    Task<IReadOnlyList<KioskTalebi>> TaleplerimAsync(CancellationToken iptal = default);

    /// <summary>Yayında olan duyurular; süresi dolan veya henüz başlamayan duyuru görünmez.</summary>
    Task<IReadOnlyList<KioskDuyurusu>> DuyurulariGetirAsync(CancellationToken iptal = default);
}

/// <summary>
/// Kiosk ekranının iş kuralları. Kiosk, "Kiosk" rolündeki kullanıcı hesabıyla açılır; afetzede ayrıca aile girişi yapar
/// ve yalnızca kendi ailesinin taleplerini görür (K-052). Art arda hatalı girişte kısa süre bekletilir (K-073).
/// </summary>
public class KioskServisi(IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman,
    ITalepSiniflandirmaKuyrugu siniflandirmaKuyrugu) : IKioskServisi
{
    public const int HataliGirisSiniri = 5;
    public static readonly TimeSpan BeklemeSuresi = TimeSpan.FromSeconds(30);

    private const string HataliGiris = "Aile numarası veya doğum yılı hatalı. Bilgilerinizi kontrol edin ya da kayıt masasına başvurun.";

    private int _hataliGirisSayisi;
    private DateTime? _beklemeBitisi;

    public KioskAilesi? Aile { get; private set; }

    public async Task<KioskAilesi> AileGirisiAsync(string aileNo, int dogumYili, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kiosk, Izin.Gorme);
        Aile = null;
        var simdi = zaman.Simdi();
        if (_beklemeBitisi is { } bitis && simdi < bitis)
            throw new IsKuraliException($"Çok sayıda hatalı giriş yapıldı. {Math.Ceiling((bitis - simdi).TotalSeconds)} saniye sonra tekrar deneyin.");

        var no = aileNo?.Trim().ToUpperInvariant();
        if (string.IsNullOrEmpty(no))
            throw new DogrulamaException("Aile numaranızı girin.");
        if (dogumYili < 1900 || dogumYili > simdi.Year)
            throw new DogrulamaException("Doğum yılını dört haneli olarak girin (ör. 1985).");

        await using var veri = veriFabrikasi.Ac();
        var aile = await veri.Repo<Aile>().Sorgu().AsNoTracking()
            .Where(a => a.AileNo == no && a.Aktif && a.Uyeler.Any(u => u.Aktif
                && (u.Durum == KisiDurumu.Burada || u.Durum == KisiDurumu.Hastanede) && u.DogumTarihi.Year == dogumYili))
            .Select(a => new KioskAilesi(a.Id, a.AileNo, a.AileAdi))
            .FirstOrDefaultAsync(iptal);

        if (aile is null)
        {
            if (++_hataliGirisSayisi >= HataliGirisSiniri)
            {
                _hataliGirisSayisi = 0;
                _beklemeBitisi = simdi + BeklemeSuresi;
            }

            throw new IsKuraliException(HataliGiris);
        }

        _hataliGirisSayisi = 0;
        _beklemeBitisi = null;
        Aile = aile;
        return aile;
    }

    public void AileCikisi() => Aile = null;

    public async Task<KioskTalebi> TalepBirakAsync(string aciklama, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kiosk, Izin.Degistirme);
        var aile = AileGirisiGerekli();

        await using var veri = veriFabrikasi.Ac();
        if (!await veri.Repo<Aile>().VarMiAsync(a => a.Id == aile.AileId && a.Aktif, iptal))
        {
            Aile = null;
            throw new IsKuraliException("Aile kaydınız artık aktif değil. Kayıt masasına başvurun.");
        }

        var talep = new Talep
        {
            Aciklama = aciklama, Kaynak = TalepKaynagi.Kiosk, AileId = aile.AileId, AcilisZamani = zaman.Simdi(),
            OlusturanKullaniciId = oturum.KullaniciId,
            CadirId = await TalepServisi.TekCadirAsync(veri, aile.AileId, iptal),
        };
        veri.Repo<Talep>().Ekle(talep);
        await veri.KaydetVeGecmiseYazAsync(oturum, talep.AcilisZamani, IslemTuru.Olusturma, nameof(Talep), () => talep.Id,
            () => $"Talep #{talep.Id} kiosktan açıldı ({aile.AileNo}).", iptal);
        siniflandirmaKuyrugu.Ekle(talep.Id); // sağlık talebi görevli beklemeden sağlık personeline yönlensin (K-080)
        return Satir(talep);
    }

    public async Task<IReadOnlyList<KioskTalebi>> TaleplerimAsync(CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kiosk, Izin.Gorme);
        var aile = AileGirisiGerekli();
        await using var veri = veriFabrikasi.Ac();
        return await veri.Repo<Talep>().Sorgu().AsNoTracking()
            .Where(t => t.AileId == aile.AileId)
            .OrderByDescending(t => t.AcilisZamani).ThenByDescending(t => t.Id)
            .Select(t => new KioskTalebi(t.Id, t.Aciklama, t.Tur, t.Durum, t.AcilisZamani, t.KapanisZamani, t.Sonuc))
            .ToListAsync(iptal);
    }

    public async Task<IReadOnlyList<KioskDuyurusu>> DuyurulariGetirAsync(CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kiosk, Izin.Gorme);
        var simdi = zaman.Simdi();
        await using var veri = veriFabrikasi.Ac();
        return await veri.Repo<Duyuru>().Sorgu().AsNoTracking()
            .Where(d => d.Aktif && d.BaslangicTarihi <= simdi && simdi <= d.BitisTarihi)
            .OrderByDescending(d => d.BaslangicTarihi).ThenByDescending(d => d.Id)
            .Select(d => new KioskDuyurusu(d.Id, d.Baslik, d.Metin, d.BaslangicTarihi, d.BitisTarihi))
            .ToListAsync(iptal);
    }

    private KioskAilesi AileGirisiGerekli() =>
        Aile ?? throw new IsKuraliException("Önce aile numaranız ve doğum yılınızla giriş yapın.");

    private static KioskTalebi Satir(Talep t) => new(t.Id, t.Aciklama, t.Tur, t.Durum, t.AcilisZamani, t.KapanisZamani, t.Sonuc);
}
