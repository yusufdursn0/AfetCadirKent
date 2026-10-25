namespace AfetCadirKent.Business.YapayZeka;

/// <summary>
/// Yapay zeka servisi sözleşmesi (REQ-130). Uygulama yalnızca bu interface'i bilir; farklı bir servise geçmek
/// (ör. bulut API) bu interface'i uygulayan tek bir sınıf eklemekle yapılır. Varsayılan uygulama: <see cref="OllamaServisi"/>.
/// Servis yalnızca öneri yapar, hiçbir kaydı değiştirmez; son karar görevlidedir (REQ-136).
/// Hata durumunda istisna fırlatmaz, başarısız <see cref="YzSonucu{T}"/> döner (kullanıcı iptali hariç).
/// </summary>
public interface IYapayZekaServisi
{
    /// <summary>Talep metninden tür, yardım önceliği ve gerekçe önerir. Sağlık önerisinde öncelik daima boştur (K-051).</summary>
    Task<YzSonucu<TalepSiniflandirmaOnerisi>> TalepSiniflandirAsync(string talepMetni, CancellationToken iptal = default);

    /// <summary>Koordinatör için günlük durum özeti yazar. Modele yalnızca toplu sayılar gider, kişisel bilgi gitmez.</summary>
    Task<YzSonucu<string>> GunlukOzetYazAsync(GunlukDurum durum, CancellationToken iptal = default);
}

public enum YzHataTuru
{
    /// <summary>Servise bağlanılamadı (Ollama çalışmıyor).</summary>
    Ulasilamadi,
    ZamanAsimi,
    ModelBulunamadi,
    SunucuHatasi,

    /// <summary>Cevap geldi ama beklenen biçimde değil.</summary>
    CevapOkunamadi,
}

/// <summary>Yapay zeka çağrısının sonucu: başarılıysa değer, değilse hata türü ve kullanıcıya gösterilecek mesaj.</summary>
public sealed class YzSonucu<T> where T : class
{
    private YzSonucu(T? deger, YzHataTuru? hata, string? hataMesaji)
    {
        Deger = deger;
        Hata = hata;
        HataMesaji = hataMesaji;
    }

    public T? Deger { get; }
    public YzHataTuru? Hata { get; }
    public string? HataMesaji { get; }
    public bool Basarili => Deger is not null;

    public static YzSonucu<T> Basari(T deger) => new(deger ?? throw new ArgumentNullException(nameof(deger)), null, null);

    public static YzSonucu<T> Basarisiz(YzHataTuru hata, string mesaj) => new(null, hata, mesaj);
}

/// <summary>Yapay zekanın talep için önerisi. Tıbbi değerlendirme değildir.</summary>
public sealed record TalepSiniflandirmaOnerisi(TalepTuru Tur, DestekOnceligi? YardimOnceligi, string? Gerekce);

/// <summary>Günlük özetin dayandığı toplu durum bilgisi (ana ekran göstergeleriyle aynı kaynaklar).</summary>
public sealed record GunlukDurum
{
    public required DateOnly Tarih { get; init; }
    public required int BuradakiKisi { get; init; }
    public required int HastanedekiKisi { get; init; }
    public required int AileSayisi { get; init; }

    /// <summary>Kullanılabilir çadırların toplam kapasitesi ve boş yatağı.</summary>
    public required int ToplamKapasite { get; init; }

    public required int BosYatak { get; init; }

    /// <summary>Bakımda veya kapalı çadır sayısı.</summary>
    public required int KullanilamayanCadir { get; init; }

    public required int BekleyenAile { get; init; }

    /// <summary>Kritik seviyenin altındaki ürünlerin uyarı metinleri.</summary>
    public required IReadOnlyList<string> KritikStoklar { get; init; }

    public required IReadOnlyList<string> SktYaklasanlar { get; init; }
    public required IReadOnlyList<string> SktGecenler { get; init; }

    /// <summary>Açık taleplerin türe göre sayısı (anahtar: ekrandaki tür adı, sınıflandırılmamışlar dahil).</summary>
    public required IReadOnlyDictionary<string, int> AcikTalepler { get; init; }

    public required int BugunAcilanTalep { get; init; }
    public required int BugunKapananTalep { get; init; }
    public required int BugunkuTeslimSayisi { get; init; }

    public int AcikTalepToplami => AcikTalepler.Values.Sum();
}

/// <summary>Günlük özet ekranının sonucu: durum her zaman vardır; yapay zeka metni servis çalışıyorsa vardır.</summary>
public sealed record GunlukOzet(GunlukDurum Durum, string? Metin, string? HataMesaji)
{
    public bool YapayZekaYazdi => Metin is not null;
}
