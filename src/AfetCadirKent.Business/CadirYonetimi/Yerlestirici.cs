using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.CadirYonetimi;

/// <summary>
/// Çadır servisi ve bekleme listesi servisinin ortak yerleştirme işlemi (aynı veri oturumu içinde çalışır).
/// Kapasite <see cref="Cadir.Yerlestir"/> ile, eşzamanlılık çadırın RowVersion'ı ile korunur.
/// </summary>
internal static class Yerlestirici
{
    /// <summary>Ailenin çadır kentte olup henüz aktif yerleşimi olmayan üyeleri (kayıt sırasıyla).</summary>
    public static Task<List<Afetzede>> YerlesmemisUyelerAsync(IVeriOturumu veri, int aileId, CancellationToken iptal) =>
        veri.Repo<Afetzede>().Sorgu()
            .Where(k => k.AileId == aileId && k.Aktif
                        && (k.Durum == KisiDurumu.Burada || k.Durum == KisiDurumu.Hastanede)
                        && !k.Yerlesimler.Any(y => y.Bitis == null))
            .OrderBy(k => k.Id)
            .ToListAsync(iptal);

    public static async Task YerlestirAsync(IVeriOturumu veri, IOturum oturum, Aile aile, IReadOnlyList<CadirAtamasi> atamalar,
        DateTime simdi, string aciklama, CancellationToken iptal)
    {
        if (atamalar is null || atamalar.Count == 0)
            throw new DogrulamaException("En az bir çadır seçilmelidir.");
        if (atamalar.Select(a => a.CadirId).Distinct().Count() != atamalar.Count)
            throw new DogrulamaException("Aynı çadır birden fazla kez seçilmiş.");

        var uyeler = await YerlesmemisUyelerAsync(veri, aile.Id, iptal);
        if (uyeler.Count == 0)
            throw new IsKuraliException($"{aile.AileNo} ailesinin çadıra yerleşmemiş üyesi yok.");
        var atanan = atamalar.Sum(a => a.KisiSayisi);
        if (atanan != uyeler.Count)
            throw new IsKuraliException($"Çadırlara atanan kişi sayısı ({atanan}) yerleşecek kişi sayısıyla ({uyeler.Count}) aynı olmalıdır.");

        var cadirIdleri = atamalar.Select(a => a.CadirId).ToList();
        var cadirlar = await veri.Repo<Cadir>().Sorgu().Include(c => c.Bolge)
            .Where(c => cadirIdleri.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, iptal);

        var kullaniciId = oturum.KullaniciId;
        var sira = 0;
        var ozet = new List<string>();
        foreach (var atama in atamalar)
        {
            if (!cadirlar.TryGetValue(atama.CadirId, out var cadir))
                throw new KayitBulunamadiException("Çadır");

            cadir.Yerlestir(atama.KisiSayisi); // kullanılabilirlik ve kapasite kontrolü
            foreach (var kisi in uyeler.Skip(sira).Take(atama.KisiSayisi))
            {
                veri.Repo<Yerlesim>().Ekle(new Yerlesim { KisiId = kisi.Id, Cadir = cadir, Baslangic = simdi, KullaniciId = kullaniciId });
                veri.Repo<KisiHareketi>().Ekle(new KisiHareketi
                {
                    KisiId = kisi.Id, Tur = KisiHareketTuru.CadirDegisikligi, YeniCadir = cadir, Zaman = simdi,
                    KullaniciId = kullaniciId, Aciklama = aciklama,
                });
            }

            sira += atama.KisiSayisi;
            ozet.Add($"{cadir.Kod} ({atama.KisiSayisi} kişi)");
        }

        // Aile bekleme listesindeyse listeden çıkar.
        var bekleme = await veri.Repo<BeklemeListesiKaydi>().ListeleAsync(b => b.AileId == aile.Id && b.CikisZamani == null, iptal);
        foreach (var kayit in bekleme)
            kayit.CikisZamani = simdi;

        veri.IslemKaydet(oturum, simdi, IslemTuru.Olusturma, nameof(Yerlesim), aile.Id,
            $"{aile.AileNo} {aile.AileAdi} yerleştirildi: {string.Join(" + ", ozet)}.{(bekleme.Count > 0 ? " Bekleme listesinden çıkarıldı." : "")}");
    }
}
