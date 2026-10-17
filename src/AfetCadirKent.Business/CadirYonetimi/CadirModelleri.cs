namespace AfetCadirKent.Business.CadirYonetimi;

/// <summary>Çadır ekleme/güncelleme bilgisi.</summary>
public sealed record CadirBilgisi
{
    public required int BolgeId { get; init; }
    public required int Sira { get; init; }
    public required int Numara { get; init; }
    public required int Kapasite { get; init; }
    public CadirDurumu Durum { get; init; } = CadirDurumu.Kullanilabilir;
    public bool Erisilebilir { get; init; }
    public bool SaglikNoktasinaYakin { get; init; }
    public bool Isitmali { get; init; }
}

/// <summary>Çadır planında çadırın rengini belirleyen durum.</summary>
public enum PlanDurumu
{
    Bos,
    KismenDolu,
    Dolu,
    Kullanilamaz,
}

/// <summary>Renkli çadır planındaki bir çadır.</summary>
public sealed record CadirPlanOgesi
{
    public required int Id { get; init; }
    public required string Kod { get; init; }
    public required int BolgeId { get; init; }
    public required string BolgeKodu { get; init; }
    public required string BolgeAdi { get; init; }
    public required int Sira { get; init; }
    public required int Numara { get; init; }
    public required int Kapasite { get; init; }
    public required int Doluluk { get; init; }
    public required CadirDurumu Durum { get; init; }
    public required bool Aktif { get; init; }
    public bool Erisilebilir { get; init; }
    public bool SaglikNoktasinaYakin { get; init; }
    public bool Isitmali { get; init; }

    /// <summary>Çadırdaki aileler (ör. "A-000001 Yılmaz Ailesi").</summary>
    public IReadOnlyList<string> Aileler { get; init; } = [];

    public int BosYatak => Kapasite - Doluluk;

    public PlanDurumu PlanDurumu =>
        !Aktif || Durum != CadirDurumu.Kullanilabilir ? PlanDurumu.Kullanilamaz
        : Doluluk == 0 ? PlanDurumu.Bos
        : Doluluk >= Kapasite ? PlanDurumu.Dolu
        : PlanDurumu.KismenDolu;
}

/// <summary>Bir aile için çadır önerisi sonucu.</summary>
public sealed record YerlesimOnerisiSonucu
{
    public required int AileId { get; init; }
    public required string AileNo { get; init; }
    public required string AileAdi { get; init; }
    public required int YerlesecekKisiSayisi { get; init; }
    public required DestekOnceligi DestekOnceligi { get; init; }
    public required IReadOnlyList<YerlesimEtkisi> IstenenEtkiler { get; init; }
    public required IReadOnlyList<CadirOnerisi> Oneriler { get; init; }

    /// <summary>Uygun çadır yoksa aile bekleme listesine alınmalıdır.</summary>
    public bool BeklemeListesiOnerilir => YerlesecekKisiSayisi > 0 && Oneriler.Count == 0;

    public string Mesaj =>
        YerlesecekKisiSayisi == 0 ? "Ailenin tüm üyeleri bir çadıra yerleşmiş."
        : BeklemeListesiOnerilir ? $"{YerlesecekKisiSayisi} kişi için uygun çadır yok. Aileyi bekleme listesine alabilirsiniz."
        : $"{YerlesecekKisiSayisi} kişi için {Oneriler.Count} öneri bulundu. Görevli onayıyla yerleştirilir.";
}

/// <summary>Bekleme listesindeki aile.</summary>
public sealed record BekleyenAile
{
    public required int KayitId { get; init; }
    public required int AileId { get; init; }
    public required string AileNo { get; init; }
    public required string AileAdi { get; init; }
    public required int KisiSayisi { get; init; }
    public required DestekOnceligi DestekOnceligi { get; init; }
    public required string Gerekceler { get; init; }
    public required DateTime EklenmeZamani { get; init; }
    public string? Aciklama { get; init; }
}

/// <summary>Yeri olan bir çadır ve ona uyan, listede ilk sıradaki aile.</summary>
public sealed record YerAcilanCadirOnerisi(CadirPlanOgesi Cadir, BekleyenAile OnerilenAile);

/// <summary>Bekleme listesi sıralama kuralı (şartname M4).</summary>
public static class BeklemeSiralamasi
{
    /// <summary>Önce destek önceliği yüksek olan, eşitlikte en uzun bekleyen.</summary>
    public static IEnumerable<BekleyenAile> Sirala(IEnumerable<BekleyenAile> liste) =>
        liste.OrderByDescending(b => b.DestekOnceligi).ThenBy(b => b.EklenmeZamani).ThenBy(b => b.KayitId);

    /// <summary>Boş yatağa sığan aileler, öneri sırasıyla.</summary>
    public static IEnumerable<BekleyenAile> CadiraUyanlar(IEnumerable<BekleyenAile> liste, int bosYatak) =>
        Sirala(liste.Where(b => b.KisiSayisi > 0 && b.KisiSayisi <= bosYatak));
}
