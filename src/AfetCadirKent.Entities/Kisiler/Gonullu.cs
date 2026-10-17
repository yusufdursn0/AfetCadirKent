namespace AfetCadirKent.Entities.Kisiler;

/// <summary>Gönüllü. Programa giriş yapmaz; kayıtlarını Koordinatör tutar. Tablo: Gonulluler.</summary>
public class Gonullu : Kisi, IAktiflik
{
    private string _uzmanlikAlani = string.Empty;
    private string? _notlar;

    public string UzmanlikAlani
    {
        get => _uzmanlikAlani;
        set => _uzmanlikAlani = Dogrula.ZorunluMetin(value, "Uzmanlık alanı", 100);
    }

    public string? Notlar
    {
        get => _notlar;
        set => _notlar = Dogrula.IstegeBagliMetin(value, "Notlar", 1000);
    }
    public bool Aktif { get; set; } = true;
    public DateTime KayitZamani { get; set; }

    public List<Vardiya> Vardiyalar { get; set; } = [];

    public override string KisiTuru => "Gönüllü";
}
