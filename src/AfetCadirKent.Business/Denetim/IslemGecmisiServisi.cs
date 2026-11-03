using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.Denetim;

/// <summary>İşlem geçmişi filtresi; boş alanlar filtrelenmez.</summary>
public sealed record IslemGecmisiFiltresi
{
    public DateOnly? Baslangic { get; init; }
    public DateOnly? Bitis { get; init; }
    public string? KullaniciAdi { get; init; }
    public string? VarlikAdi { get; init; }
    public int? VarlikId { get; init; }
    public IslemTuru? IslemTuru { get; init; }

    /// <summary>Açıklamada geçen metin (ör. aile numarası "A-000012").</summary>
    public string? Metin { get; init; }

    public int EnFazlaKayit { get; init; } = 1000;
}

public interface IIslemGecmisiServisi
{
    /// <summary>Kim, ne zaman, neyi oluşturdu/değiştirdi/iptal etti (en yeni önce).</summary>
    Task<IReadOnlyList<IslemKaydi>> ListeleAsync(IslemGecmisiFiltresi filtre, CancellationToken iptal = default);
}

/// <summary>İşlem geçmişi ekranının veri kaynağı (şartname M14). Kayıtlar servislerce yazılır; burada yalnızca okunur.</summary>
public class IslemGecmisiServisi(IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum) : IIslemGecmisiServisi
{
    public async Task<IReadOnlyList<IslemKaydi>> ListeleAsync(IslemGecmisiFiltresi filtre, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.IslemGecmisi, Izin.Gorme);
        ArgumentNullException.ThrowIfNull(filtre);
        if (filtre.EnFazlaKayit is < 1 or > 10_000)
            throw new DogrulamaException("Kayıt sayısı 1 ile 10.000 arasında olmalıdır.");

        await using var veri = veriFabrikasi.Ac();
        var sorgu = veri.Repo<IslemKaydi>().Sorgu().AsNoTracking();
        if (filtre.Baslangic is { } bas)
            sorgu = sorgu.Where(i => i.Zaman >= bas.ToDateTime(TimeOnly.MinValue));
        if (filtre.Bitis is { } bit)
            sorgu = sorgu.Where(i => i.Zaman < bit.AddDays(1).ToDateTime(TimeOnly.MinValue));
        if (!string.IsNullOrWhiteSpace(filtre.KullaniciAdi))
            sorgu = sorgu.Where(i => i.KullaniciAdi == filtre.KullaniciAdi.Trim());
        if (!string.IsNullOrWhiteSpace(filtre.VarlikAdi))
            sorgu = sorgu.Where(i => i.VarlikAdi == filtre.VarlikAdi);
        if (filtre.VarlikId is { } varlikId)
            sorgu = sorgu.Where(i => i.VarlikId == varlikId);
        if (filtre.IslemTuru is { } tur)
            sorgu = sorgu.Where(i => i.IslemTuru == tur);
        if (!string.IsNullOrWhiteSpace(filtre.Metin))
            sorgu = sorgu.Where(i => i.Aciklama.Contains(filtre.Metin.Trim()));

        return await sorgu.OrderByDescending(i => i.Zaman).ThenByDescending(i => i.Id).Take(filtre.EnFazlaKayit).ToListAsync(iptal);
    }
}

/// <summary>İşlem geçmişindeki kayıt türlerinin (varlık adları) ekranda gösterilen Türkçe adları.</summary>
public static class KayitTurleri
{
    public static readonly IReadOnlyDictionary<string, string> Adlar = new Dictionary<string, string>
    {
        [nameof(Aile)] = "Aile",
        [nameof(Afetzede)] = "Kişi",
        [nameof(Cadir)] = "Çadır",
        [nameof(Bolge)] = "Bölge",
        [nameof(Yerlesim)] = "Yerleşim",
        [nameof(BeklemeListesiKaydi)] = "Bekleme listesi",
        [nameof(Urun)] = "Ürün",
        [nameof(Parti)] = "Parti (stok girişi)",
        [nameof(StokHareketi)] = "Stok hareketi",
        [nameof(Bagisci)] = "Bağışçı",
        [nameof(Dagitim)] = "Dağıtım listesi",
        [nameof(DagitimDetay)] = "Dağıtım satırı",
        [nameof(Talep)] = "Talep",
        [nameof(YakinBulmaBasvurusu)] = "Yakın bulma",
        [nameof(Gorevli)] = "Kullanıcı",
        [nameof(Gonullu)] = "Gönüllü",
        [nameof(Vardiya)] = "Vardiya",
        [nameof(Duyuru)] = "Duyuru",
        [nameof(Gerekce)] = "Gerekçe ayarı",
    };

    /// <summary>Türkçe ad; listede olmayan tür için varlık adının kendisi.</summary>
    public static string Metin(string varlikAdi) => Adlar.GetValueOrDefault(varlikAdi, varlikAdi);
}
