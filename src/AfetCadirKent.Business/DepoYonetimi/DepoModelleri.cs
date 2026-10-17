namespace AfetCadirKent.Business.DepoYonetimi;

/// <summary>Depo ayarları (appsettings.json → Uygulama bölümü).</summary>
public sealed record DepoAyarlari
{
    /// <summary>SKT'sine bu kadar gün kalan partiler için uyarı verilir (K-048).</summary>
    public int SktUyariGunu { get; init; } = 7;
}

/// <summary>Ürün ekleme/güncelleme bilgisi (ürün ayarları dahil).</summary>
public sealed record UrunBilgisi
{
    public required string Ad { get; init; }
    public required UrunKategorisi Kategori { get; init; }
    public required Birim Birim { get; init; }
    public required HesaplamaTuru HesaplamaTuru { get; init; }
    public required decimal HakMiktari { get; init; }
    public required int TekrarAlmaGunu { get; init; }
    public required decimal KritikSeviye { get; init; }
}

/// <summary>Stok girişi: yeni parti açılır ve giriş hareketi yazılır.</summary>
public sealed record StokGirisiBilgisi
{
    public required int UrunId { get; init; }
    public required string PartiNo { get; init; }
    public required decimal Miktar { get; init; }
    public DateOnly? SonKullanmaTarihi { get; init; }
    public int? BagisciId { get; init; }
    public string? Aciklama { get; init; }
}

/// <summary>Stok durumu ekranındaki bir parti.</summary>
public sealed record PartiDurumu
{
    public required int Id { get; init; }
    public required int UrunId { get; init; }
    public required string UrunAdi { get; init; }
    public required string PartiNo { get; init; }
    public string? BagisciAdi { get; init; }
    public required DateTime GirisZamani { get; init; }
    public DateOnly? SonKullanmaTarihi { get; init; }
    public required decimal Miktar { get; init; }
    public required decimal AyrilanMiktar { get; init; }
    public required decimal KullanilabilirMiktar { get; init; }
    public required bool SktGecti { get; init; }
    public required bool SktYaklasiyor { get; init; }

    /// <summary>Dağıtımda önerilme sırası (FEFO, 1 = ilk); dağıtıma uygun değilse null.</summary>
    public int? FefoSirasi { get; init; }

    public bool Aktif { get; init; }
}

/// <summary>Ürün bazında stok özeti: fiziksel, ayrılmış (teslim bekleyen) ve kullanılabilir miktar ayrı görünür.</summary>
public sealed record UrunStokDurumu
{
    public required int UrunId { get; init; }
    public required string UrunAdi { get; init; }
    public required Birim Birim { get; init; }
    public required decimal FizikselMiktar { get; init; }

    /// <summary>Dağıtıma ayrılmış ama teslim edilmemiş miktar.</summary>
    public required decimal AyrilanMiktar { get; init; }

    public required decimal KullanilabilirMiktar { get; init; }

    /// <summary>SKT'si geçmiş, henüz fireye düşülmemiş miktar (kullanılabilire dahil değildir).</summary>
    public required decimal SktGecmisMiktar { get; init; }

    public required decimal KritikSeviye { get; init; }
    public bool KritikAltinda => KullanilabilirMiktar < KritikSeviye;
    public required IReadOnlyList<PartiDurumu> Partiler { get; init; }
}

public enum StokUyariTuru
{
    KritikSeviye,
    SktYaklasiyor,
    SktGecti,
}

/// <summary>Stok uyarısı (stok durumu ve ana ekran).</summary>
public sealed record StokUyarisi(StokUyariTuru Tur, int UrunId, string UrunAdi, int? PartiId, string Mesaj);

/// <summary>Stok hareketleri listesindeki satır.</summary>
public sealed record StokHareketSatiri
{
    public required int Id { get; init; }
    public required DateTime Zaman { get; init; }
    public required StokHareketTuru Tur { get; init; }
    public required string TurAdi { get; init; }
    public required string UrunAdi { get; init; }
    public required string PartiNo { get; init; }
    public required decimal Miktar { get; init; }

    /// <summary>İşaretli etki (+ giriş, − çıkış).</summary>
    public required decimal StokEtkisi { get; init; }

    public string? Aciklama { get; init; }
    public string? KullaniciAdi { get; init; }

    /// <summary>Dağıtım hareketlerinde aile numarası ve teslim alan.</summary>
    public string? AileNo { get; init; }
    public string? TeslimAlan { get; init; }

    public required bool Aktif { get; init; }
}

/// <summary>Stok hareketleri filtresi; boş alanlar filtrelenmez.</summary>
public sealed record StokHareketFiltresi
{
    public int? UrunId { get; init; }
    public StokHareketTuru? Tur { get; init; }
    public DateOnly? Baslangic { get; init; }
    public DateOnly? Bitis { get; init; }
    public bool PasifleriDeGoster { get; init; }
}
