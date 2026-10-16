namespace AfetCadirKent.Entities.Depo;

/// <summary>
/// Stok partisi. Fiziksel miktar ve dağıtıma ayrılmış miktar ayrı tutulur:
/// kullanılabilir = miktar − ayrılan (SKT'si geçmişse 0).
/// Miktarlar yalnızca metotlar üzerinden değişir (kapsülleme).
/// </summary>
public class Parti : VarlikTemel, IAktiflik
{
    private string _partiNo = string.Empty;

    public int UrunId { get; set; }
    public Urun? Urun { get; set; }

    public int? BagisciId { get; set; }
    public Bagisci? Bagisci { get; set; }

    public string PartiNo
    {
        get => _partiNo;
        set => _partiNo = Dogrula.ZorunluMetin(value, "Parti numarası", 50);
    }

    /// <summary>Depodaki fiziksel miktar (ayrılan dahil).</summary>
    public decimal Miktar { get; private set; }

    /// <summary>Dağıtıma ayrılmış ama henüz teslim edilmemiş miktar.</summary>
    public decimal AyrilanMiktar { get; private set; }

    public DateOnly? SonKullanmaTarihi { get; set; }
    public DateTime GirisZamani { get; set; }
    public bool Aktif { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];

    public List<StokHareketi> Hareketler { get; set; } = [];

    public bool SktGectiMi(DateOnly bugun) => SonKullanmaTarihi is { } skt && skt < bugun;

    public bool SktYaklasiyorMu(DateOnly bugun, int gun) =>
        SonKullanmaTarihi is { } skt && skt >= bugun && skt <= bugun.AddDays(gun);

    public decimal KullanilabilirMiktar(DateOnly bugun) => SktGectiMi(bugun) ? 0 : Miktar - AyrilanMiktar;

    public void MiktarEkle(decimal miktar)
    {
        Dogrula.Pozitif(miktar, "Miktar");
        Miktar += miktar;
    }

    /// <summary>Ayrılmamış stoktan düşer (hasar/fire, eksik sayım).</summary>
    public void MiktarDus(decimal miktar)
    {
        Dogrula.Pozitif(miktar, "Miktar");
        if (miktar > Miktar - AyrilanMiktar)
            throw new IsKuraliException(
                $"{PartiNo} partisinde düşülebilecek miktar {Miktar - AyrilanMiktar}; {miktar} düşülemez.");
        Miktar -= miktar;
    }

    /// <summary>Dağıtım listesi onaylandığında miktarı "ayrıldı" olarak işaretler.</summary>
    public void Ayir(decimal miktar, DateOnly bugun)
    {
        Dogrula.Pozitif(miktar, "Ayrılacak miktar");
        if (SktGectiMi(bugun))
            throw new IsKuraliException($"{PartiNo} partisinin son kullanma tarihi geçmiş; dağıtıma çıkamaz.");
        if (miktar > KullanilabilirMiktar(bugun))
            throw new IsKuraliException(
                $"{PartiNo} partisinde yeterli stok yok. Kullanılabilir: {KullanilabilirMiktar(bugun)}, istenen: {miktar}.");
        AyrilanMiktar += miktar;
    }

    /// <summary>Teslim iptali veya gün sonu: ayrılan miktar kullanılabilir stoğa döner.</summary>
    public void AyirmaKaldir(decimal miktar)
    {
        Dogrula.Pozitif(miktar, "Miktar");
        if (miktar > AyrilanMiktar)
            throw new IsKuraliException($"{PartiNo} partisinde ayrılmış miktar {AyrilanMiktar}; {miktar} geri alınamaz.");
        AyrilanMiktar -= miktar;
    }

    /// <summary>Ayrılmış miktarın teslim edilmesi: hem ayrılandan hem fiziksel stoktan düşer.</summary>
    public void AyrilaniTeslimEt(decimal miktar)
    {
        AyirmaKaldir(miktar);
        Miktar -= miktar;
    }
}
