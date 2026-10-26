using AfetCadirKent.Business.AnaEkran;
using AfetCadirKent.Business.CadirYonetimi;
using AfetCadirKent.Business.DagitimYonetimi;
using AfetCadirKent.Business.Denetim;
using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.Gerekceler;
using AfetCadirKent.Business.Kayit;
using AfetCadirKent.Business.KioskIslemleri;
using AfetCadirKent.Business.KullaniciYonetimi;
using AfetCadirKent.Business.TalepYonetimi;
using AfetCadirKent.Business.YapayZeka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AfetCadirKent.Business;

/// <summary>
/// İş katmanı servislerinin bağımlılık kayıtları. Masaüstü uygulamasında tek kullanıcı oturumu vardır; servisler
/// durumsuzdur ve her işlemde kısa ömürlü veri oturumu açar (K-063), bu yüzden tek nesne (singleton) olarak kaydedilir.
/// </summary>
public static class IsKatmaniKayitlari
{
    public static IServiceCollection IsKatmaniniEkle(this IServiceCollection servisler, OllamaAyarlari ollamaAyarlari,
        DepoAyarlari depoAyarlari)
    {
        ArgumentNullException.ThrowIfNull(ollamaAyarlari);
        ArgumentNullException.ThrowIfNull(depoAyarlari);

        servisler.AddSingleton(TimeProvider.System);
        servisler.AddSingleton<Oturum>();
        servisler.AddSingleton<IOturum>(sp => sp.GetRequiredService<Oturum>());
        servisler.AddSingleton(ollamaAyarlari);
        servisler.AddSingleton(depoAyarlari);
        servisler.AddSingleton(_ => GerekceDegerlendirici.Varsayilan());

        // Yapay zeka: farklı bir sağlayıcıya geçmek için yalnızca bu kayıt değişir (REQ-139).
        servisler.AddSingleton<IYapayZekaServisi>(sp => new OllamaServisi(
            new HttpClient(), sp.GetRequiredService<OllamaAyarlari>(), sp.GetService<ILogger<OllamaServisi>>()));
        servisler.AddSingleton<OtomatikSiniflandirici>();
        servisler.AddSingleton<ITalepSiniflandirmaKuyrugu>(sp => sp.GetRequiredService<OtomatikSiniflandirici>());

        servisler.AddSingleton<IUygulamaBaslatici, UygulamaBaslatici>();
        servisler.AddSingleton<IKurulumServisi, KurulumServisi>();
        servisler.AddSingleton<IOturumServisi, OturumServisi>();
        servisler.AddSingleton<IKullaniciServisi, KullaniciServisi>();
        servisler.AddSingleton<IKayitServisi, KayitServisi>();
        servisler.AddSingleton<IGerekceServisi, GerekceServisi>();
        servisler.AddSingleton<ICadirServisi, CadirServisi>();
        servisler.AddSingleton<IBeklemeListesiServisi, BeklemeListesiServisi>();
        servisler.AddSingleton<IUrunServisi, UrunServisi>();
        servisler.AddSingleton<IStokServisi, StokServisi>();
        servisler.AddSingleton<IDagitimServisi, DagitimServisi>();
        servisler.AddSingleton<ITalepServisi, TalepServisi>();
        servisler.AddSingleton<IKioskServisi, KioskServisi>();
        servisler.AddSingleton<IGunlukOzetServisi, GunlukOzetServisi>();
        servisler.AddSingleton<IAnaEkranServisi, AnaEkranServisi>();
        servisler.AddSingleton<IIslemGecmisiServisi, IslemGecmisiServisi>();
        return servisler;
    }
}
