namespace AfetCadirKent.Business.KullaniciYonetimi;

/// <summary>Yeni kullanıcı (ilk yönetici veya Koordinatörün eklediği görevli).</summary>
public sealed record YeniKullaniciBilgisi
{
    public required string Ad { get; init; }
    public required string Soyad { get; init; }
    public required string KullaniciAdi { get; init; }
    public required string Parola { get; init; }

    /// <summary>Ekranda ikinci kez girilen parola; eşleşmezse kayıt yapılmaz.</summary>
    public required string ParolaTekrar { get; init; }

    public string? Telefon { get; init; }
}

/// <summary>Kullanıcı listesindeki satır. Parola özeti dışarı verilmez.</summary>
public sealed record KullaniciOzeti
{
    public required int Id { get; init; }
    public required string Ad { get; init; }
    public required string Soyad { get; init; }
    public required string KullaniciAdi { get; init; }
    public string? Telefon { get; init; }
    public required RolTuru Rol { get; init; }
    public required bool Aktif { get; init; }
    public required DateTime OlusturmaZamani { get; init; }
    public DateTime? SonGirisZamani { get; init; }

    public string AdSoyad => $"{Ad} {Soyad}";
    public string RolAdi => Rol.Metin();
}
