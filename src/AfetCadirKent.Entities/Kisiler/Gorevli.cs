namespace AfetCadirKent.Entities.Kisiler;

/// <summary>Programa giriş yapan görevli (kullanıcı). Tablo: Kullanicilar.</summary>
public class Gorevli : Kisi, IAktiflik
{
    private string _kullaniciAdi = string.Empty;
    private string _parolaHash = string.Empty;

    public string KullaniciAdi
    {
        get => _kullaniciAdi;
        set
        {
            var temiz = Dogrula.ZorunluMetin(value, "Kullanıcı adı", 50);
            if (temiz.Length < 3 || temiz.Any(char.IsWhiteSpace))
                throw new DogrulamaException("Kullanıcı adı en az 3 karakter olmalı ve boşluk içermemelidir.");
            _kullaniciAdi = temiz;
        }
    }

    /// <summary>BCrypt ile üretilmiş parola özeti. Düz parola hiçbir zaman saklanmaz.</summary>
    public string ParolaHash => _parolaHash;

    public int RolId { get; set; }
    public Rol? Rol { get; set; }

    public bool Aktif { get; set; } = true;
    public DateTime OlusturmaZamani { get; set; }
    public DateTime? SonGirisZamani { get; set; }

    public override string KisiTuru => "Görevli";

    public void ParolaHashAta(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new DogrulamaException("Parola özeti boş olamaz.");
        _parolaHash = hash;
    }
}
