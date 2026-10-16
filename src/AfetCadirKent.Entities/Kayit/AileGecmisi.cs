namespace AfetCadirKent.Entities.Kayit;

/// <summary>Aileden ayırma / birleşme kayıtları. Eski aileyle bağlantı burada geçmiş olarak kalır.</summary>
public class AileGecmisi : VarlikTemel
{
    public AileGecmisiTuru Tur { get; set; }

    public int KaynakAileId { get; set; }
    public Aile? KaynakAile { get; set; }

    public int HedefAileId { get; set; }
    public Aile? HedefAile { get; set; }

    public DateTime Zaman { get; set; }
    public string Aciklama { get; set; } = string.Empty;

    /// <summary>Taşınan kişilerin numaraları (ör. "K-000012, K-000013").</summary>
    public string TasinanKisiler { get; set; } = string.Empty;

    public int? KullaniciId { get; set; }
    public Gorevli? Kullanici { get; set; }
}
