using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.KullaniciYonetimi;

public interface IKurulumServisi
{
    /// <summary>Hiç kullanıcı yoksa true: uygulama "İlk yönetici hesabını oluştur" ekranını açar.</summary>
    Task<bool> IlkKurulumGerekliMiAsync(CancellationToken iptal = default);

    /// <summary>İlk Koordinatör hesabını oluşturur. Yalnızca hiç kullanıcı yokken çalışır (oturum gerekmez).</summary>
    Task<KullaniciOzeti> IlkYoneticiOlusturAsync(YeniKullaniciBilgisi bilgi, CancellationToken iptal = default);
}

/// <summary>İlk kurulum (şartname M1). Kodda ve demo verisinde hiçbir kullanıcı/parola yoktur.</summary>
public class KurulumServisi(IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman) : IKurulumServisi
{
    public async Task<bool> IlkKurulumGerekliMiAsync(CancellationToken iptal = default)
    {
        await using var veri = veriFabrikasi.Ac();
        return !await veri.Repo<Gorevli>().VarMiAsync(_ => true, iptal);
    }

    public async Task<KullaniciOzeti> IlkYoneticiOlusturAsync(YeniKullaniciBilgisi bilgi, CancellationToken iptal = default)
    {
        await using var veri = veriFabrikasi.Ac();
        if (await veri.Repo<Gorevli>().VarMiAsync(_ => true, iptal))
            throw new IsKuraliException("İlk kurulum zaten yapılmış. Kullanıcı adınız ve parolanızla giriş yapın.");

        var simdi = zaman.Simdi();
        var yonetici = KullaniciOlusturucu.Olustur(bilgi, RolTuru.Koordinator, simdi);

        await using var islem = await veri.IslemBaslatAsync(iptal);
        veri.Repo<Gorevli>().Ekle(yonetici);
        await veri.GuvenliKaydetAsync(iptal);

        // İki kurulum ekranı aynı anda kaydederse ikisi de geri alınır; ilk yönetici tek kalır.
        if (await veri.Repo<Gorevli>().Sorgu().CountAsync(iptal) != 1)
            throw new IsKuraliException("İlk kurulum aynı anda başka bir yerden yapılıyor. Uygulamayı yeniden açıp giriş yapmayı deneyin.");

        veri.IslemKaydet(oturum, simdi, IslemTuru.Olusturma, nameof(Gorevli), yonetici.Id,
            $"İlk kurulum: Koordinatör hesabı oluşturuldu ({yonetici.KullaniciAdi}).");
        await veri.GuvenliKaydetAsync(iptal);
        await islem.CommitAsync(iptal);
        return KullaniciOlusturucu.Ozet(yonetici);
    }
}

/// <summary>Kurulum ve kullanıcı servisinin ortak kullanıcı oluşturma ve gösterim kodu.</summary>
internal static class KullaniciOlusturucu
{
    public static Gorevli Olustur(YeniKullaniciBilgisi bilgi, RolTuru rol, DateTime simdi)
    {
        ArgumentNullException.ThrowIfNull(bilgi);
        var gorevli = new Gorevli
        {
            Ad = bilgi.Ad, Soyad = bilgi.Soyad, KullaniciAdi = bilgi.KullaniciAdi, Telefon = bilgi.Telefon,
            RolId = (int)rol, OlusturmaZamani = simdi,
        };
        ParolaAta(gorevli, bilgi.Parola, bilgi.ParolaTekrar);
        return gorevli;
    }

    public static void ParolaAta(Gorevli gorevli, string parola, string parolaTekrar)
    {
        if (parola != parolaTekrar)
            throw new DogrulamaException("Parola ve parola tekrarı aynı değil.");
        ParolaGuvenligi.KuralaUymali(parola, gorevli.KullaniciAdi);
        gorevli.ParolaHashAta(ParolaGuvenligi.Ozetle(parola));
    }

    public static KullaniciOzeti Ozet(Gorevli g) => new()
    {
        Id = g.Id, Ad = g.Ad, Soyad = g.Soyad, KullaniciAdi = g.KullaniciAdi, Telefon = g.Telefon, Rol = (RolTuru)g.RolId,
        Aktif = g.Aktif, OlusturmaZamani = g.OlusturmaZamani, SonGirisZamani = g.SonGirisZamani,
    };
}
