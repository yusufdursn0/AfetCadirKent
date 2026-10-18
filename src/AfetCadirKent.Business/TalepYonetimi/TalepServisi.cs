using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.TalepYonetimi;

public interface ITalepServisi
{
    /// <summary>Kayıt masasından veya görevliden talep açar. Kiosk talepleri kiosk ekranından açılır.</summary>
    Task<TalepOzeti> OlusturAsync(TalepBilgisi bilgi, CancellationToken iptal = default);

    /// <summary>Görevli türü ve yardım önceliğini belirler; talep türüne göre yönlendirilir (sağlık talebi öncelik almaz).</summary>
    Task SiniflandirAsync(int talepId, TalepTuru tur, DestekOnceligi? yardimOnceligi, CancellationToken iptal = default);

    /// <summary>Talebi bir görevliye atar (Yeni → Atandı). Başkasına atamayı yalnızca Koordinatör yapar; diğerleri kendi rolündeki talebi üstlenebilir.</summary>
    Task AtaAsync(int talepId, int kullaniciId, CancellationToken iptal = default);

    /// <summary>Atandı → İşlemde (sorumlu görevli veya Koordinatör).</summary>
    Task IslemeAlAsync(int talepId, CancellationToken iptal = default);

    /// <summary>İşlemde → Tamamlandı. Sonuç zorunludur.</summary>
    Task TamamlaAsync(int talepId, string sonuc, CancellationToken iptal = default);

    /// <summary>Açık talebi iptal eder. Sonuç (iptal nedeni) zorunludur.</summary>
    Task IptalEtAsync(int talepId, string sonuc, CancellationToken iptal = default);

    /// <summary>Oturumdaki rolün görebildiği talepler (en yeni önce).</summary>
    Task<IReadOnlyList<TalepOzeti>> ListeleAsync(TalepFiltresi filtre, CancellationToken iptal = default);

    Task<TalepOzeti> GetirAsync(int talepId, CancellationToken iptal = default);

    /// <summary>Talebin atanabileceği aktif görevliler: hedef roldekiler ve Koordinatörler.</summary>
    Task<IReadOnlyList<AtanabilirGorevli>> AtanabilirGorevlilerAsync(int talepId, CancellationToken iptal = default);
}

public class TalepServisi(IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman) : ITalepServisi
{
    private bool Koordinator => oturum.Rol == RolTuru.Koordinator;

    // ---------------------------------------------------------------- Oluşturma ve sınıflandırma

    public async Task<TalepOzeti> OlusturAsync(TalepBilgisi bilgi, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Talep, Izin.Degistirme);
        ArgumentNullException.ThrowIfNull(bilgi);
        if (bilgi.Kaynak == TalepKaynagi.Kiosk)
            throw new IsKuraliException("Kiosk talepleri kiosk ekranından açılır.");

        await using var veri = veriFabrikasi.Ac();
        var talep = new Talep
        {
            Aciklama = bilgi.Aciklama, Kaynak = bilgi.Kaynak, AcilisZamani = zaman.Simdi(), OlusturanKullaniciId = oturum.KullaniciId,
        };

        if (bilgi.AileId is { } aileId)
        {
            var aile = await veri.Repo<Aile>().Sorgu().Include(a => a.Uyeler).AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == aileId && a.Aktif, iptal) ?? throw new KayitBulunamadiException("Aile");
            oturum.AileGorunurOlmali(aile);
            talep.AileId = aileId;
        }

        talep.CadirId = bilgi.CadirId ?? await TekCadirAsync(veri, bilgi.AileId, iptal);
        if (bilgi.CadirId is { } cadirId && !await veri.Repo<Cadir>().VarMiAsync(c => c.Id == cadirId, iptal))
            throw new KayitBulunamadiException("Çadır");

        if (bilgi.Tur is { } tur)
            talep.Siniflandir(tur, bilgi.YardimOnceligi, SiniflandirmaDurumu.GorevliBelirledi);
        else if (bilgi.YardimOnceligi is not null)
            throw new DogrulamaException("Yardım önceliği vermek için önce talep türünü seçin.");

        veri.Repo<Talep>().Ekle(talep);
        await veri.KaydetVeGecmiseYazAsync(oturum, talep.AcilisZamani, IslemTuru.Olusturma, nameof(Talep), () => talep.Id,
            () => $"Talep #{talep.Id} açıldı ({talep.Kaynak.Metin()}, {talep.Tur.Metin()}"
                  + (talep.HedefRol is { } r ? $" → {r.Metin()}" : "") + ").", iptal);
        return await OzetOkuAsync(veri, talep.Id, iptal);
    }

    /// <summary>Ailenin çadır kentteki tüm üyeleri tek çadırdaysa o çadır (talebin "aile/çadır" bilgisi için).</summary>
    internal static async Task<int?> TekCadirAsync(IVeriOturumu veri, int? aileId, CancellationToken iptal)
    {
        if (aileId is null)
            return null;
        var cadirlar = await veri.Repo<Yerlesim>().Sorgu()
            .Where(y => y.Bitis == null && y.Kisi!.AileId == aileId)
            .Select(y => y.CadirId).Distinct().ToListAsync(iptal);
        return cadirlar.Count == 1 ? cadirlar[0] : null;
    }

    public async Task SiniflandirAsync(int talepId, TalepTuru tur, DestekOnceligi? yardimOnceligi, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Talep, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var talep = await GorunurTalepAsync(veri, talepId, iptal);

        var onceki = talep.Tur.Metin();
        var atanmisti = talep.SorumluKullaniciId is not null;
        talep.Siniflandir(tur, yardimOnceligi, SiniflandirmaDurumu.GorevliBelirledi);
        var oncelik = talep.SaglikPersoneliDegerlendirecek ? "Sağlık personeli değerlendirecek"
            : $"yardım önceliği {(talep.YardimOnceligi is { } o ? o.Metin() : "—")}";
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Guncelleme, nameof(Talep), talepId,
            $"Talep #{talepId} sınıflandırıldı: {onceki} → {tur.Metin()} ({oncelik}), {talep.HedefRol!.Value.Metin()} yönlendirildi"
            + (atanmisti && talep.SorumluKullaniciId is null ? "; önceki atama kaldırıldı." : "."));
        await veri.GuvenliKaydetAsync(iptal);
    }

    // ---------------------------------------------------------------- İş akışı

    public async Task AtaAsync(int talepId, int kullaniciId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Talep, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var talep = await GorunurTalepAsync(veri, talepId, iptal);
        if (talep.Tur is null)
            throw new IsKuraliException("Talep atanmadan önce türü belirlenmelidir.");

        if (!Koordinator && (kullaniciId != oturum.KullaniciId || oturum.Rol != talep.HedefRol))
            throw new YetkisizIslemException("Talebi başka bir görevliye yalnızca Koordinatör atayabilir; kendi rolünüzdeki talebi üstlenebilirsiniz.");

        var gorevli = await veri.Repo<Gorevli>().Sorgu().AsNoTracking().FirstOrDefaultAsync(g => g.Id == kullaniciId && g.Aktif, iptal)
            ?? throw new KayitBulunamadiException("Görevli");
        var rol = (RolTuru)gorevli.RolId;
        if (rol != talep.HedefRol && rol != RolTuru.Koordinator)
            throw new IsKuraliException($"{talep.Tur.Metin()} talepleri {talep.HedefRol!.Value.Metin()} veya Koordinatöre atanabilir; {gorevli.AdSoyad} {rol.Metin()}.");

        talep.Ata(kullaniciId);
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Guncelleme, nameof(Talep), talepId,
            $"Talep #{talepId} {gorevli.AdSoyad} ({gorevli.KullaniciAdi}) görevlisine atandı.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task IslemeAlAsync(int talepId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Talep, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var talep = await GorunurTalepAsync(veri, talepId, iptal);
        SorumluOlmali(talep);

        talep.IslemeAl();
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Guncelleme, nameof(Talep), talepId, $"Talep #{talepId} işleme alındı.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task TamamlaAsync(int talepId, string sonuc, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Talep, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var talep = await GorunurTalepAsync(veri, talepId, iptal);
        SorumluOlmali(talep);

        var simdi = zaman.Simdi();
        talep.Tamamla(sonuc, simdi);
        veri.IslemKaydet(oturum, simdi, IslemTuru.Guncelleme, nameof(Talep), talepId, $"Talep #{talepId} tamamlandı. Sonuç: {talep.Sonuc}");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task IptalEtAsync(int talepId, string sonuc, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Talep, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var talep = await GorunurTalepAsync(veri, talepId, iptal);
        var acan = talep.Durum == TalepDurumu.Yeni && talep.OlusturanKullaniciId is { } o && o == oturum.KullaniciId;
        if (!acan)
            SorumluOlmali(talep);

        var simdi = zaman.Simdi();
        talep.IptalEt(sonuc, simdi);
        veri.IslemKaydet(oturum, simdi, IslemTuru.Iptal, nameof(Talep), talepId, $"Talep #{talepId} iptal edildi. Sonuç: {talep.Sonuc}");
        await veri.GuvenliKaydetAsync(iptal);
    }

    /// <summary>Talebi ilerletmeyi/kapatmayı sorumlu görevli veya Koordinatör yapar.</summary>
    private void SorumluOlmali(Talep talep)
    {
        if (!Koordinator && (talep.SorumluKullaniciId is null || talep.SorumluKullaniciId != oturum.KullaniciId))
            throw new YetkisizIslemException("Bu talebi yalnızca sorumlu görevli veya Koordinatör ilerletebilir.");
    }

    // ---------------------------------------------------------------- Sorgular

    public async Task<IReadOnlyList<TalepOzeti>> ListeleAsync(TalepFiltresi filtre, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Talep, Izin.Gorme);
        ArgumentNullException.ThrowIfNull(filtre);
        await using var veri = veriFabrikasi.Ac();

        var sorgu = GorunurTalepler(veri);
        if (filtre.Durum is { } durum)
            sorgu = sorgu.Where(t => t.Durum == durum);
        if (filtre.Tur is { } tur)
            sorgu = sorgu.Where(t => t.Tur == tur);
        if (filtre.SadeceAcik)
            sorgu = sorgu.Where(t => t.Durum != TalepDurumu.Tamamlandi && t.Durum != TalepDurumu.Iptal);
        if (filtre.SadeceSiniflandirilmamis)
            sorgu = sorgu.Where(t => t.Tur == null);
        if (filtre.AileId is { } aileId)
            sorgu = sorgu.Where(t => t.AileId == aileId);
        if (filtre.SadeceBanaAtanan)
        {
            var uid = oturum.KullaniciId;
            sorgu = sorgu.Where(t => uid != null && t.SorumluKullaniciId == uid);
        }

        return await Ozetler(sorgu.OrderByDescending(t => t.AcilisZamani).ThenByDescending(t => t.Id)).ToListAsync(iptal);
    }

    public async Task<TalepOzeti> GetirAsync(int talepId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Talep, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        return await Ozetler(GorunurTalepler(veri).Where(t => t.Id == talepId)).FirstOrDefaultAsync(iptal)
            ?? throw new KayitBulunamadiException("Talep");
    }

    public async Task<IReadOnlyList<AtanabilirGorevli>> AtanabilirGorevlilerAsync(int talepId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Talep, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        var talep = await GorunurTalepAsync(veri, talepId, iptal);
        if (talep.HedefRol is not { } hedef)
            return [];

        int[] roller = [(int)hedef, (int)RolTuru.Koordinator];
        return await veri.Repo<Gorevli>().Sorgu().AsNoTracking()
            .Where(g => g.Aktif && roller.Contains(g.RolId))
            .OrderBy(g => g.RolId == (int)RolTuru.Koordinator).ThenBy(g => g.Ad).ThenBy(g => g.Soyad)
            .Select(g => new AtanabilirGorevli(g.Id, g.Ad + " " + g.Soyad, g.KullaniciAdi, (RolTuru)g.RolId))
            .ToListAsync(iptal);
    }

    private Task<TalepOzeti> OzetOkuAsync(IVeriOturumu veri, int talepId, CancellationToken iptal) =>
        Ozetler(veri.Repo<Talep>().Sorgu().Where(t => t.Id == talepId)).SingleAsync(iptal);

    private async Task<Talep> GorunurTalepAsync(IVeriOturumu veri, int talepId, CancellationToken iptal) =>
        await GorunurTalepler(veri).FirstOrDefaultAsync(t => t.Id == talepId, iptal) ?? throw new KayitBulunamadiException("Talep");

    /// <summary>
    /// Rol bazlı görünürlük (K-072): Koordinatör hepsini; saha görevlisi yalnızca kendine atananları;
    /// sağlık personeli sağlık taleplerini, depo sorumlusu malzeme taleplerini; kayıt görevlisi sınıflandırılmamış talepleri görür.
    /// Herkes kendi açtığı ve kendine atanan talepleri görür. Refakatsiz çocuk ailesinin talepleri yalnızca Koordinatöre görünür.
    /// </summary>
    private IQueryable<Talep> GorunurTalepler(IVeriOturumu veri)
    {
        var sorgu = veri.Repo<Talep>().Sorgu();
        if (Koordinator)
            return sorgu;

        var uid = oturum.KullaniciId;
        var rol = oturum.Rol;
        sorgu = sorgu.Where(t => t.AileId == null || !(t.Aile!.Uyeler.Any() && t.Aile.Uyeler.All(u => u.RefakatsizCocuk)));
        return rol switch
        {
            RolTuru.SahaGorevlisi => sorgu.Where(t => uid != null && t.SorumluKullaniciId == uid),
            RolTuru.KayitGorevlisi => sorgu.Where(t => t.Tur == null
                || (uid != null && (t.OlusturanKullaniciId == uid || t.SorumluKullaniciId == uid))),
            _ => sorgu.Where(t => t.HedefRol == rol
                || (uid != null && (t.OlusturanKullaniciId == uid || t.SorumluKullaniciId == uid))),
        };
    }

    private static IQueryable<TalepOzeti> Ozetler(IQueryable<Talep> sorgu) =>
        sorgu.Select(t => new TalepOzeti
        {
            Id = t.Id, Aciklama = t.Aciklama, Tur = t.Tur, Kaynak = t.Kaynak, Durum = t.Durum, YardimOnceligi = t.YardimOnceligi,
            SaglikPersoneliDegerlendirecek = t.SaglikPersoneliDegerlendirecek, HedefRol = t.HedefRol,
            SiniflandirmaDurumu = t.SiniflandirmaDurumu,
            AileId = t.AileId, AileNo = t.Aile == null ? null : t.Aile.AileNo, AileAdi = t.Aile == null ? null : t.Aile.AileAdi,
            CadirId = t.CadirId,
            CadirKodu = t.Cadir == null ? null
                : t.Cadir.Bolge!.Kod + t.Cadir.Sira.ToString() + "-" + (t.Cadir.Numara < 10 ? "0" : "") + t.Cadir.Numara.ToString(),
            SorumluKullaniciId = t.SorumluKullaniciId,
            SorumluAdi = t.SorumluKullanici == null ? null : t.SorumluKullanici.Ad + " " + t.SorumluKullanici.Soyad,
            OlusturanAdi = t.OlusturanKullanici == null ? null : t.OlusturanKullanici.Ad + " " + t.OlusturanKullanici.Soyad,
            AcilisZamani = t.AcilisZamani, KapanisZamani = t.KapanisZamani, Sonuc = t.Sonuc,
            YzOnerilenTur = t.YzOnerilenTur, YzOnerilenOncelik = t.YzOnerilenOncelik, YzGerekce = t.YzGerekce,
        });
}
