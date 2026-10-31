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
        var kisiDagilimi = KisiDagilimi(atamalar, uyeler);

        var cadirIdleri = atamalar.Select(a => a.CadirId).ToList();
        var cadirlar = await veri.Repo<Cadir>().Sorgu().Include(c => c.Bolge)
            .Where(c => cadirIdleri.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, iptal);

        var kullaniciId = oturum.KullaniciId;
        var ozet = new List<string>();
        for (var i = 0; i < atamalar.Count; i++)
        {
            var atama = atamalar[i];
            if (!cadirlar.TryGetValue(atama.CadirId, out var cadir))
                throw new KayitBulunamadiException("Çadır");

            cadir.Yerlestir(atama.KisiSayisi); // kullanılabilirlik ve kapasite kontrolü
            foreach (var kisi in kisiDagilimi[i])
            {
                veri.Repo<Yerlesim>().Ekle(new Yerlesim { KisiId = kisi.Id, Cadir = cadir, Baslangic = simdi, KullaniciId = kullaniciId });
                veri.Repo<KisiHareketi>().Ekle(new KisiHareketi
                {
                    KisiId = kisi.Id, Tur = KisiHareketTuru.CadirDegisikligi, YeniCadir = cadir, Zaman = simdi,
                    KullaniciId = kullaniciId, Aciklama = aciklama,
                });
            }

            ozet.Add($"{cadir.Kod} ({atama.KisiSayisi} kişi)");
        }

        // Aile bekleme listesindeyse listeden çıkar.
        var bekleme = await veri.Repo<BeklemeListesiKaydi>().ListeleAsync(b => b.AileId == aile.Id && b.CikisZamani == null, iptal);
        foreach (var kayit in bekleme)
            kayit.CikisZamani = simdi;

        veri.IslemKaydet(oturum, simdi, IslemTuru.Olusturma, nameof(Yerlesim), aile.Id,
            $"{aile.AileNo} {aile.AileAdi} yerleştirildi: {string.Join(" + ", ozet)}.{(bekleme.Count > 0 ? " Bekleme listesinden çıkarıldı." : "")}");
    }

    /// <summary>
    /// Her atamaya yerleşecek kişiler. Kişi seçilmemişse üyeler kayıt sırasıyla dağıtılır; seçildiyse ya her atamada seçim
    /// olmalı ya da hiçbirinde olmamalı, seçilen kişiler yerleşmemiş üyelerin tamamını tam bir kez kapsamalıdır.
    /// </summary>
    private static List<List<Afetzede>> KisiDagilimi(IReadOnlyList<CadirAtamasi> atamalar, List<Afetzede> uyeler)
    {
        if (atamalar.All(a => a.KisiIdler is null))
        {
            var sira = 0;
            return atamalar.Select(a =>
            {
                var parca = uyeler.Skip(sira).Take(a.KisiSayisi).ToList();
                sira += a.KisiSayisi;
                return parca;
            }).ToList();
        }

        if (atamalar.Any(a => a.KisiIdler is null))
            throw new DogrulamaException("Kişi seçimi yapıldıysa her çadır için yerleşecek kişiler seçilmelidir.");

        var uyeSozlugu = uyeler.ToDictionary(u => u.Id);
        var secilenler = atamalar.SelectMany(a => a.KisiIdler!).ToList();
        if (secilenler.Distinct().Count() != secilenler.Count)
            throw new DogrulamaException("Bir kişi birden fazla çadıra seçilmiş.");
        if (secilenler.Any(id => !uyeSozlugu.ContainsKey(id)))
            throw new IsKuraliException("Seçilen kişilerden biri bu ailenin çadıra yerleşmemiş üyesi değil. Listeyi yenileyip tekrar deneyin.");
        if (secilenler.Count != uyeler.Count)
            throw new IsKuraliException($"Yerleşecek {uyeler.Count} kişinin hepsi bir çadıra seçilmelidir.");
        if (atamalar.FirstOrDefault(a => a.KisiIdler!.Count != a.KisiSayisi) is { } hatali)
            throw new IsKuraliException($"{hatali.CadirKodu} çadırı için {hatali.KisiSayisi} kişi seçilmeli; {hatali.KisiIdler!.Count} kişi seçildi.");

        return atamalar.Select(a => a.KisiIdler!.Select(id => uyeSozlugu[id]).ToList()).ToList();
    }
}
