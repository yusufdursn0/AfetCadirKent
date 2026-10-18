using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.KullaniciYonetimi;

public interface IKullaniciServisi
{
    Task<IReadOnlyList<KullaniciOzeti>> ListeleAsync(bool pasifleriDeGoster = false, CancellationToken iptal = default);

    Task<IReadOnlyList<Rol>> RolleriListeleAsync(CancellationToken iptal = default);

    /// <summary>Koordinatör yeni görevli (veya kiosk hesabı) ekler.</summary>
    Task<KullaniciOzeti> EkleAsync(YeniKullaniciBilgisi bilgi, RolTuru rol, CancellationToken iptal = default);

    Task GuncelleAsync(int id, string ad, string soyad, string? telefon, RolTuru rol, CancellationToken iptal = default);

    /// <summary>Koordinatör, parolasını unutan görevliye yeni parola verir.</summary>
    Task ParolaSifirlaAsync(int id, string yeniParola, string yeniParolaTekrar, CancellationToken iptal = default);

    Task AktiflikDegistirAsync(int id, bool aktif, CancellationToken iptal = default);

    /// <summary>Hiç kullanılmamış (giriş yapmamış, işlem yapmamış, talep almamış) kullanıcı silinir; aksi halde pasif yapılır.</summary>
    Task SilAsync(int id, CancellationToken iptal = default);
}

public class KullaniciServisi(IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman) : IKullaniciServisi
{
    public async Task<IReadOnlyList<KullaniciOzeti>> ListeleAsync(bool pasifleriDeGoster = false, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.KullaniciYonetimi, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        var liste = await veri.Repo<Gorevli>().Sorgu().AsNoTracking()
            .Where(g => pasifleriDeGoster || g.Aktif)
            .OrderBy(g => g.RolId).ThenBy(g => g.Ad).ThenBy(g => g.Soyad)
            .ToListAsync(iptal);
        return liste.Select(KullaniciOlusturucu.Ozet).ToList();
    }

    public async Task<IReadOnlyList<Rol>> RolleriListeleAsync(CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.KullaniciYonetimi, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        return await veri.Repo<Rol>().Sorgu().AsNoTracking().OrderBy(r => r.Id).ToListAsync(iptal);
    }

    public async Task<KullaniciOzeti> EkleAsync(YeniKullaniciBilgisi bilgi, RolTuru rol, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.KullaniciYonetimi, Izin.Degistirme);
        RolGecerliOlmali(rol);
        var simdi = zaman.Simdi();
        var gorevli = KullaniciOlusturucu.Olustur(bilgi, rol, simdi);

        await using var veri = veriFabrikasi.Ac();
        if (await veri.Repo<Gorevli>().VarMiAsync(g => g.KullaniciAdi == gorevli.KullaniciAdi, iptal))
            throw new IsKuraliException($"'{gorevli.KullaniciAdi}' kullanıcı adı kullanılıyor. Başka bir kullanıcı adı seçin.");

        veri.Repo<Gorevli>().Ekle(gorevli);
        await veri.KaydetVeGecmiseYazAsync(oturum, simdi, IslemTuru.Olusturma, nameof(Gorevli), () => gorevli.Id,
            () => $"Kullanıcı eklendi: {gorevli.KullaniciAdi} ({gorevli.AdSoyad}), rol {rol.Metin()}.", iptal);
        return KullaniciOlusturucu.Ozet(gorevli);
    }

    public async Task GuncelleAsync(int id, string ad, string soyad, string? telefon, RolTuru rol, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.KullaniciYonetimi, Izin.Degistirme);
        RolGecerliOlmali(rol);
        await using var veri = veriFabrikasi.Ac();
        var gorevli = await veri.Repo<Gorevli>().BulAsync(id, iptal) ?? throw new KayitBulunamadiException("Kullanıcı");

        var eskiRol = (RolTuru)gorevli.RolId;
        if (eskiRol != rol)
        {
            await SonKoordinatorKalmaliAsync(veri, gorevli, iptal);
            await AcikTalebiOlmamaliAsync(veri, gorevli, rol, iptal);
        }

        gorevli.Ad = ad;
        gorevli.Soyad = soyad;
        gorevli.Telefon = telefon;
        gorevli.RolId = (int)rol;
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Guncelleme, nameof(Gorevli), id,
            $"Kullanıcı güncellendi: {gorevli.KullaniciAdi} ({gorevli.AdSoyad})"
            + (eskiRol != rol ? $", rol {eskiRol.Metin()} → {rol.Metin()}." : "."));
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task ParolaSifirlaAsync(int id, string yeniParola, string yeniParolaTekrar, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.KullaniciYonetimi, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var gorevli = await veri.Repo<Gorevli>().BulAsync(id, iptal) ?? throw new KayitBulunamadiException("Kullanıcı");

        KullaniciOlusturucu.ParolaAta(gorevli, yeniParola, yeniParolaTekrar);
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Guncelleme, nameof(Gorevli), id,
            $"{gorevli.KullaniciAdi} kullanıcısının parolası Koordinatör tarafından yenilendi.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task AktiflikDegistirAsync(int id, bool aktif, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.KullaniciYonetimi, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var gorevli = await veri.Repo<Gorevli>().BulAsync(id, iptal) ?? throw new KayitBulunamadiException("Kullanıcı");
        if (!aktif)
        {
            KendisiOlmamali(id, "pasif yapamazsınız");
            await SonKoordinatorKalmaliAsync(veri, gorevli, iptal);
            await AcikTalebiOlmamaliAsync(veri, gorevli, null, iptal);
        }

        gorevli.Aktif = aktif;
        veri.IslemKaydet(oturum, zaman.Simdi(), aktif ? IslemTuru.Guncelleme : IslemTuru.PasifYapma, nameof(Gorevli), id,
            $"Kullanıcı {gorevli.KullaniciAdi} {(aktif ? "aktif" : "pasif")} yapıldı.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task SilAsync(int id, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.KullaniciYonetimi, Izin.Degistirme);
        KendisiOlmamali(id, "silemezsiniz");
        await using var veri = veriFabrikasi.Ac();
        var gorevli = await veri.Repo<Gorevli>().BulAsync(id, iptal) ?? throw new KayitBulunamadiException("Kullanıcı");
        await SonKoordinatorKalmaliAsync(veri, gorevli, iptal);

        // Giriş yapan veya işlem yapan kullanıcının adı işlem geçmişinde ve kayıtlarda geçer: silinmez, pasif yapılır.
        var kullanildi = await veri.Repo<IslemKaydi>().VarMiAsync(i => i.KullaniciId == id, iptal)
            || await veri.Repo<Talep>().VarMiAsync(t => t.SorumluKullaniciId == id || t.OlusturanKullaniciId == id, iptal);
        if (kullanildi)
            throw new SilinemezException();

        veri.Repo<Gorevli>().Sil(gorevli);
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Silme, nameof(Gorevli), null,
            $"Kullanıcı silindi: {gorevli.KullaniciAdi} ({gorevli.AdSoyad}).");
        try
        {
            await veri.GuvenliKaydetAsync(iptal);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
        {
            throw new SilinemezException(); // başka bir tablodan başvuru var (son savunma: FK kısıtı)
        }
    }

    private static void RolGecerliOlmali(RolTuru rol)
    {
        if (!Enum.IsDefined(rol))
            throw new DogrulamaException("Geçerli bir rol seçin.");
    }

    private void KendisiOlmamali(int id, string islem)
    {
        if (oturum.KullaniciId == id)
            throw new IsKuraliException($"Kendi hesabınızı {islem}.");
    }

    /// <summary>Son aktif Koordinatör pasif yapılamaz, silinemez, rolü değiştirilemez (yönetici kalmaz).</summary>
    private static async Task SonKoordinatorKalmaliAsync(IVeriOturumu veri, Gorevli gorevli, CancellationToken iptal)
    {
        if (gorevli.RolId != (int)RolTuru.Koordinator || !gorevli.Aktif)
            return;
        if (!await veri.Repo<Gorevli>().VarMiAsync(g => g.Id != gorevli.Id && g.Aktif && g.RolId == (int)RolTuru.Koordinator, iptal))
            throw new IsKuraliException("Sistemde en az bir aktif Koordinatör kalmalıdır. Önce başka bir Koordinatör ekleyin.");
    }

    /// <summary>Görevliye atanmış açık talepler, yeni rolü bu talepleri alamayacaksa önce başkasına atanmalıdır.</summary>
    private static async Task AcikTalebiOlmamaliAsync(IVeriOturumu veri, Gorevli gorevli, RolTuru? yeniRol, CancellationToken iptal)
    {
        if (yeniRol == RolTuru.Koordinator)
            return; // Koordinatör her türlü talebi üstlenebilir
        var sayi = await veri.Repo<Talep>().Sorgu().CountAsync(t => t.SorumluKullaniciId == gorevli.Id
            && t.Durum != TalepDurumu.Tamamlandi && t.Durum != TalepDurumu.Iptal
            && (yeniRol == null || t.HedefRol != yeniRol), iptal);
        if (sayi > 0)
            throw new IsKuraliException($"{gorevli.AdSoyad} kullanıcısına atanmış {sayi} açık talep var. Önce talepleri başka bir görevliye atayın.");
    }
}
