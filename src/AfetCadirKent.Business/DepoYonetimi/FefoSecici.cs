namespace AfetCadirKent.Business.DepoYonetimi;

/// <summary>FEFO seçiminde bir partiden alınacak miktar.</summary>
public sealed record PartiPayi(Parti Parti, decimal Miktar);

/// <summary>
/// FEFO (ilk son kullanma tarihi gelen ilk çıkar) parti seçimi. SKT'si geçmiş ve pasif partiler dışlanır;
/// SKT'si olmayan partiler en sona kalır, eşitlikte önce giren parti önce çıkar. Veritabanına bağlı değildir.
/// </summary>
public static class FefoSecici
{
    /// <summary>Dağıtıma çıkabilecek partiler, önerilme sırasıyla.</summary>
    public static IReadOnlyList<Parti> UygunPartiler(IEnumerable<Parti> partiler, DateOnly bugun) =>
        partiler
            .Where(p => p.Aktif && p.KullanilabilirMiktar(bugun) > 0)
            .OrderBy(p => p.SonKullanmaTarihi is null)
            .ThenBy(p => p.SonKullanmaTarihi)
            .ThenBy(p => p.GirisZamani)
            .ThenBy(p => p.Id)
            .ToList();

    /// <summary>İstenen miktarı SKT'si en yakın partilerden başlayarak böler.</summary>
    /// <exception cref="IsKuraliException">Kullanılabilir stok istenen miktarı karşılamıyorsa.</exception>
    public static IReadOnlyList<PartiPayi> Sec(IEnumerable<Parti> partiler, decimal miktar, DateOnly bugun)
    {
        Dogrula.Pozitif(miktar, "İstenen miktar");
        var uygun = UygunPartiler(partiler, bugun);
        var toplam = uygun.Sum(p => p.KullanilabilirMiktar(bugun));
        if (toplam < miktar)
            throw new IsKuraliException(
                $"Yetersiz stok. Kullanılabilir: {toplam:0.##}, gereken: {miktar:0.##}. SKT'si geçmiş partiler dağıtıma çıkamaz.");

        var paylar = new List<PartiPayi>();
        var kalan = miktar;
        foreach (var parti in uygun)
        {
            var pay = Math.Min(kalan, parti.KullanilabilirMiktar(bugun));
            paylar.Add(new PartiPayi(parti, pay));
            kalan -= pay;
            if (kalan == 0)
                break;
        }

        return paylar;
    }
}
