namespace AfetCadirKent.Entities.Cadirlar;

/// <summary>
/// Kişinin bir çadıra yerleşimi. Bitişi boş olan kayıt aktif yerleşimdir; kişi başına en fazla bir tane olur.
/// Hastanedeki kişinin yerleşimi kapanmaz (yeri korunur).
/// </summary>
public class Yerlesim : VarlikTemel
{
    public int KisiId { get; set; }
    public Afetzede? Kisi { get; set; }

    public int CadirId { get; set; }
    public Cadir? Cadir { get; set; }

    public DateTime Baslangic { get; set; }
    public DateTime? Bitis { get; set; }

    public int? KullaniciId { get; set; }
    public Gorevli? Kullanici { get; set; }

    public bool AktifMi => Bitis is null;
}

/// <summary>Yer bulunamayan ailelerin bekleme kaydı. Tablo: BeklemeListesi.</summary>
public class BeklemeListesiKaydi : VarlikTemel
{
    public int AileId { get; set; }
    public Aile? Aile { get; set; }

    public DateTime EklenmeZamani { get; set; }

    /// <summary>Listeden çıkış zamanı (yerleşti veya iptal). Boşsa aile hâlâ bekliyor.</summary>
    public DateTime? CikisZamani { get; set; }

    public string? Aciklama { get; set; }

    public int? KullaniciId { get; set; }
    public Gorevli? Kullanici { get; set; }

    public bool BekliyorMu => CikisZamani is null;
}
