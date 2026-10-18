using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.KullaniciYonetimi;

public interface IOturumServisi
{
    /// <summary>
    /// Kullanıcı adı ve parolayla giriş; başarılıysa oturum rolün yetkileriyle açılır.
    /// Hatalı kullanıcı adı ile hatalı parola aynı mesajı verir; pasif kullanıcı giremez.
    /// </summary>
    Task<KullaniciOzeti> GirisYapAsync(string kullaniciAdi, string parola, CancellationToken iptal = default);

    Task CikisYapAsync(CancellationToken iptal = default);

    /// <summary>Oturumdaki kullanıcının kendi parolasını değiştirmesi (eski parola doğrulanır).</summary>
    Task ParolaDegistirAsync(string eskiParola, string yeniParola, string yeniParolaTekrar, CancellationToken iptal = default);
}

public class OturumServisi(IVeriOturumuFabrikasi veriFabrikasi, Oturum oturum, TimeProvider zaman) : IOturumServisi
{
    private const string HataliGiris = "Kullanıcı adı veya parola hatalı.";

    public async Task<KullaniciOzeti> GirisYapAsync(string kullaniciAdi, string parola, CancellationToken iptal = default)
    {
        var ad = kullaniciAdi?.Trim();
        if (string.IsNullOrEmpty(ad) || string.IsNullOrEmpty(parola))
            throw new DogrulamaException("Kullanıcı adı ve parolayı girin.");
        if (oturum.AcikMi)
            oturum.Kapat();

        await using var veri = veriFabrikasi.Ac();
        var simdi = zaman.Simdi();
        var gorevli = await veri.Repo<Gorevli>().Sorgu().FirstOrDefaultAsync(g => g.KullaniciAdi == ad, iptal);

        if (gorevli is null)
        {
            ParolaGuvenligi.SahteDogrula(parola);
            await BasarisizGirisAsync(veri, simdi, ad, iptal);
            throw new IsKuraliException(HataliGiris);
        }

        if (!ParolaGuvenligi.Dogrula(parola, gorevli.ParolaHash))
        {
            await BasarisizGirisAsync(veri, simdi, ad, iptal);
            throw new IsKuraliException(HataliGiris);
        }

        if (!gorevli.Aktif)
        {
            await BasarisizGirisAsync(veri, simdi, $"{ad} (pasif hesap)", iptal);
            throw new IsKuraliException("Hesabınız pasif. Koordinatöre başvurun.");
        }

        var yetkiler = await veri.Repo<Yetki>().Sorgu().AsNoTracking().Where(y => y.RolId == gorevli.RolId).ToListAsync(iptal);
        gorevli.SonGirisZamani = simdi;
        oturum.Ac(gorevli, (RolTuru)gorevli.RolId, yetkiler);
        veri.IslemKaydet(oturum, simdi, IslemTuru.Giris, nameof(Gorevli), gorevli.Id, $"{gorevli.KullaniciAdi} giriş yaptı.");
        try
        {
            await veri.GuvenliKaydetAsync(iptal);
        }
        catch
        {
            oturum.Kapat(); // giriş kaydedilemediyse oturum açık kalmaz
            throw;
        }

        return KullaniciOlusturucu.Ozet(gorevli);
    }

    private Task BasarisizGirisAsync(IVeriOturumu veri, DateTime simdi, string denenenAd, CancellationToken iptal)
    {
        veri.IslemKaydet(oturum, simdi, IslemTuru.Giris, nameof(Gorevli), null, $"Başarısız giriş denemesi: '{denenenAd.Kisalt(60)}'.");
        return veri.GuvenliKaydetAsync(iptal);
    }

    public async Task CikisYapAsync(CancellationToken iptal = default)
    {
        if (!oturum.AcikMi)
            return;
        await using (var veri = veriFabrikasi.Ac())
        {
            veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Diger, nameof(Gorevli), oturum.KullaniciId,
                $"{oturum.Kullanici!.KullaniciAdi} çıkış yaptı.");
            await veri.GuvenliKaydetAsync(iptal);
        }

        oturum.Kapat();
    }

    public async Task ParolaDegistirAsync(string eskiParola, string yeniParola, string yeniParolaTekrar, CancellationToken iptal = default)
    {
        if (oturum.KullaniciId is not { } id)
            throw new YetkisizIslemException("Bu işlem için giriş yapmalısınız.");

        await using var veri = veriFabrikasi.Ac();
        var gorevli = await veri.Repo<Gorevli>().BulAsync(id, iptal) ?? throw new KayitBulunamadiException("Kullanıcı");
        if (!ParolaGuvenligi.Dogrula(eskiParola ?? "", gorevli.ParolaHash))
            throw new IsKuraliException("Mevcut parola hatalı.");
        if (eskiParola == yeniParola)
            throw new DogrulamaException("Yeni parola eskisinden farklı olmalıdır.");

        KullaniciOlusturucu.ParolaAta(gorevli, yeniParola, yeniParolaTekrar);
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Guncelleme, nameof(Gorevli), id, $"{gorevli.KullaniciAdi} parolasını değiştirdi.");
        await veri.GuvenliKaydetAsync(iptal);
    }
}
