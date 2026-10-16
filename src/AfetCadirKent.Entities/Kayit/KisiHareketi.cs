namespace AfetCadirKent.Entities.Kayit;

/// <summary>Kişinin geliş, ayrılış, nakil, çadır değişikliği, hastane ve aile değişikliği geçmişi.</summary>
public class KisiHareketi : VarlikTemel
{
    public int KisiId { get; set; }
    public Afetzede? Kisi { get; set; }

    public KisiHareketTuru Tur { get; set; }
    public DateTime Zaman { get; set; }
    public string? Aciklama { get; set; }

    public int? EskiCadirId { get; set; }
    public Cadir? EskiCadir { get; set; }
    public int? YeniCadirId { get; set; }
    public Cadir? YeniCadir { get; set; }

    public int? EskiAileId { get; set; }
    public Aile? EskiAile { get; set; }
    public int? YeniAileId { get; set; }
    public Aile? YeniAile { get; set; }

    public int? KullaniciId { get; set; }
    public Gorevli? Kullanici { get; set; }
}
