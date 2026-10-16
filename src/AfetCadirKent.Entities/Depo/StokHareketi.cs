namespace AfetCadirKent.Entities.Depo;

/// <summary>
/// Soyut stok hareketi. Her hareket türü stoğa etkisini ve partiye nasıl uygulanacağını kendisi belirler
/// (çok biçimlilik). Tablo: StokHareketleri (TPH, ayırıcı: HareketTuru). Fiziksel olarak silinmez.
/// </summary>
public abstract class StokHareketi : VarlikTemel, IFizikselSilinemez
{
    private decimal _miktar;

    public int PartiId { get; set; }
    public Parti? Parti { get; set; }

    /// <summary>Her zaman pozitif; yönü <see cref="StokEtkisi"/> belirler.</summary>
    public decimal Miktar
    {
        get => _miktar;
        set => _miktar = Dogrula.Pozitif(value, "Hareket miktarı");
    }

    public DateTime Zaman { get; set; }
    public string? Aciklama { get; set; }

    public int? KullaniciId { get; set; }
    public Gorevli? Kullanici { get; set; }

    public bool Aktif { get; set; } = true;

    public abstract StokHareketTuru Tur { get; }

    /// <summary>Stok miktarına işaretli etkisi (+ giriş, − çıkış).</summary>
    public abstract decimal StokEtkisi { get; }

    public abstract string TurAdi { get; }

    /// <summary>Hareketi partinin miktarlarına uygular.</summary>
    public abstract void PartiyeUygula(Parti parti);
}

public class GirisHareketi : StokHareketi
{
    public override StokHareketTuru Tur => StokHareketTuru.Giris;
    public override decimal StokEtkisi => Miktar;
    public override string TurAdi => "Giriş";
    public override void PartiyeUygula(Parti parti) => parti.MiktarEkle(Miktar);
}

/// <summary>Teslim edilen dağıtım. Teslim eden kullanıcı <see cref="StokHareketi.KullaniciId"/>, teslim alan kişi ayrıca tutulur.</summary>
public class DagitimHareketi : StokHareketi
{
    public int DagitimDetayId { get; set; }
    public DagitimDetay? DagitimDetay { get; set; }

    public int? TeslimAlanKisiId { get; set; }
    public Afetzede? TeslimAlanKisi { get; set; }

    public override StokHareketTuru Tur => StokHareketTuru.Dagitim;
    public override decimal StokEtkisi => -Miktar;
    public override string TurAdi => "Dağıtım";

    /// <summary>Dağıtım daima önceden ayrılmış miktardan yapılır.</summary>
    public override void PartiyeUygula(Parti parti) => parti.AyrilaniTeslimEt(Miktar);
}

public class IadeHareketi : StokHareketi
{
    public override StokHareketTuru Tur => StokHareketTuru.Iade;
    public override decimal StokEtkisi => Miktar;
    public override string TurAdi => "İade";
    public override void PartiyeUygula(Parti parti) => parti.MiktarEkle(Miktar);
}

/// <summary>Hasar / fire (SKT'si geçen partiler de bu hareketle düşülür).</summary>
public class HasarHareketi : StokHareketi
{
    public override StokHareketTuru Tur => StokHareketTuru.Hasar;
    public override decimal StokEtkisi => -Miktar;
    public override string TurAdi => "Hasar/Fire";
    public override void PartiyeUygula(Parti parti) => parti.MiktarDus(Miktar);
}

public class SayimFarkiHareketi : StokHareketi
{
    /// <summary>True: sayımda fazla çıktı (stok artar); false: eksik çıktı (stok azalır).</summary>
    public bool FazlaMi { get; set; }

    public override StokHareketTuru Tur => StokHareketTuru.SayimFarki;
    public override decimal StokEtkisi => FazlaMi ? Miktar : -Miktar;
    public override string TurAdi => FazlaMi ? "Sayım farkı (fazla)" : "Sayım farkı (eksik)";

    public override void PartiyeUygula(Parti parti)
    {
        if (FazlaMi)
            parti.MiktarEkle(Miktar);
        else
            parti.MiktarDus(Miktar);
    }
}
