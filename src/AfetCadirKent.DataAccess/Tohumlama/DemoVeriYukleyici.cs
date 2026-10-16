using AfetCadirKent.DataAccess.Konfigurasyonlar;
using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.DataAccess.Tohumlama;

/// <summary>
/// Demo verisi (şartname "Demo"): 3 bölge, 30 çadır, 20 aile, 10 ürün, SKT'si yaklaşmış parti,
/// stok tahmini için son günlerin dağıtımları. Kullanıcı içermez.
/// Tarihler yükleme anına göre hesaplanır (SKT "yaklaşmış" kalsın diye).
/// </summary>
public static class DemoVeriYukleyici
{
    private sealed record UyeTanimi(
        string Ad, int Yas, Cinsiyet Cinsiyet, string? Yakinlik,
        bool Hamile = false, bool Tekerlekli = false, bool Kronik = false, bool Refakatsiz = false, bool Kimliksiz = false);

    private sealed record AileTanimi(string Soyad, string Memleket, bool TekEbeveyn, UyeTanimi[] Uyeler);

    private static readonly AileTanimi[] Aileler =
    [
        new("Yılmaz", "Kahramanmaraş", false, [new("Mehmet", 34, Cinsiyet.Erkek, "Baba"), new("Ayşe", 30, Cinsiyet.Kadin, "Anne", Hamile: true), new("Elif", 1, Cinsiyet.Kadin, "Çocuk", Kimliksiz: true)]),
        new("Kaya", "Hatay", false, [new("Hasan", 72, Cinsiyet.Erkek, "Baba", Tekerlekli: true), new("Fatma", 68, Cinsiyet.Kadin, "Anne")]),
        new("Demir", "Kahramanmaraş", false, [new("Ali", 40, Cinsiyet.Erkek, "Baba"), new("Zeynep", 38, Cinsiyet.Kadin, "Anne"), new("Can", 12, Cinsiyet.Erkek, "Çocuk"), new("Ece", 9, Cinsiyet.Kadin, "Çocuk"), new("Ahmet", 6, Cinsiyet.Erkek, "Çocuk"), new("Mert", 3, Cinsiyet.Erkek, "Çocuk")]),
        new("Çelik", "Adıyaman", true, [new("Emine", 29, Cinsiyet.Kadin, "Anne"), new("Yusuf", 0, Cinsiyet.Erkek, "Çocuk", Kimliksiz: true)]),
        new("Şahin", "Malatya", false, [new("Mustafa", 55, Cinsiyet.Erkek, "Baba", Kronik: true), new("Hatice", 52, Cinsiyet.Kadin, "Anne")]),
        new("Yıldız", "Hatay", false, [new("İbrahim", 45, Cinsiyet.Erkek, "Baba"), new("Gül", 43, Cinsiyet.Kadin, "Anne"), new("Burak", 17, Cinsiyet.Erkek, "Çocuk")]),
        new("Aydın", "Adıyaman", false, [new("Osman", 80, Cinsiyet.Erkek, "Baba"), new("Hacer", 77, Cinsiyet.Kadin, "Anne", Tekerlekli: true)]),
        new("Öztürk", "Kahramanmaraş", false, [new("Murat", 31, Cinsiyet.Erkek, "Baba"), new("Merve", 28, Cinsiyet.Kadin, "Anne"), new("Defne", 2, Cinsiyet.Kadin, "Çocuk")]),
        new("Arslan", "Malatya", false, [new("Kemal", 50, Cinsiyet.Erkek, "Baba"), new("Sevgi", 47, Cinsiyet.Kadin, "Anne"), new("Oğuz", 20, Cinsiyet.Erkek, "Çocuk"), new("Selin", 15, Cinsiyet.Kadin, "Çocuk")]),
        new("Doğan", "Hatay", false, [new("Hüseyin", 37, Cinsiyet.Erkek, "Baba"), new("Esra", 35, Cinsiyet.Kadin, "Anne", Hamile: true)]),
        new("Kılıç", "Kahramanmaraş", false, [new("Serkan", 42, Cinsiyet.Erkek, "Baba"), new("Dilek", 40, Cinsiyet.Kadin, "Anne"), new("Arda", 10, Cinsiyet.Erkek, "Çocuk"), new("Ada", 8, Cinsiyet.Kadin, "Çocuk"), new("Kerem", 5, Cinsiyet.Erkek, "Çocuk"), new("Duru", 1, Cinsiyet.Kadin, "Çocuk")]),
        new("Aslan", "Malatya", false, [new("Gökhan", 33, Cinsiyet.Erkek, null, Kimliksiz: true)]),
        new("Çetin", "Adıyaman", false, [new("Recep", 66, Cinsiyet.Erkek, "Baba"), new("Nuriye", 63, Cinsiyet.Kadin, "Anne", Kronik: true)]),
        new("Kara", "Hatay", true, [new("Selma", 36, Cinsiyet.Kadin, "Anne"), new("Eren", 11, Cinsiyet.Erkek, "Çocuk"), new("Yağmur", 7, Cinsiyet.Kadin, "Çocuk")]),
        new("Koç", "Kahramanmaraş", false, [new("Tolga", 28, Cinsiyet.Erkek, null, Tekerlekli: true)]),
        new("Kurt", "Malatya", false, [new("Cemal", 58, Cinsiyet.Erkek, "Baba"), new("Songül", 54, Cinsiyet.Kadin, "Anne"), new("Hakan", 25, Cinsiyet.Erkek, "Çocuk")]),
        new("Özdemir", "Adıyaman", false, [new("Ramazan", 61, Cinsiyet.Erkek, "Baba"), new("Ayten", 59, Cinsiyet.Kadin, "Anne"), new("Zehra", 85, Cinsiyet.Kadin, "Büyükanne")]),
        new("Şimşek", "Hatay", false, [new("Emre", 26, Cinsiyet.Erkek, "Baba"), new("Büşra", 24, Cinsiyet.Kadin, "Anne"), new("Alp", 0, Cinsiyet.Erkek, "Çocuk", Kimliksiz: true)]),
        new("Polat", "Kahramanmaraş", false, [new("Deniz", 14, Cinsiyet.Erkek, null, Refakatsiz: true, Kimliksiz: true)]),
        new("Erdoğan", "Malatya", false, [new("Volkan", 39, Cinsiyet.Erkek, "Baba"), new("Pınar", 37, Cinsiyet.Kadin, "Anne"), new("Sena", 13, Cinsiyet.Kadin, "Çocuk"), new("Berk", 4, Cinsiyet.Erkek, "Çocuk")]),
    ];

    public static async Task YukleAsync(AfetDbContext db, DateTime simdi, CancellationToken iptal = default)
    {
        var bugun = DateOnly.FromDateTime(simdi);

        await using var islem = await db.Database.BeginTransactionAsync(iptal);

        var cadirlar = BolgeVeCadirlariEkle(db);
        var urunler = UrunleriEkle(db);
        await db.SaveChangesAsync(iptal);

        GerekceUrunleriniEkle(db, urunler);
        var partiler = BagisciVePartileriEkle(db, urunler, simdi, bugun);
        var aileler = AileleriEkle(db, simdi, bugun);
        await db.SaveChangesAsync(iptal);

        Yerlestir(db, aileler, cadirlar, simdi);
        GecmisDagitimlariEkle(db, aileler, urunler, partiler, simdi, bugun);
        DuyurulariEkle(db, simdi);
        await db.SaveChangesAsync(iptal);

        await islem.CommitAsync(iptal);
    }

    private static List<Cadir> BolgeVeCadirlariEkle(AfetDbContext db)
    {
        var a = new Bolge { Kod = "A", Ad = "A Bölgesi", Aciklama = "Sağlık noktasının yanı; erişilebilir çadırlar" };
        var b = new Bolge { Kod = "B", Ad = "B Bölgesi", Aciklama = "Isıtmalı çadırlar" };
        var c = new Bolge { Kod = "C", Ad = "C Bölgesi", Aciklama = "Genel yerleşim" };
        db.Bolgeler.AddRange(a, b, c);

        int[] kapasiteler = [4, 4, 6, 6, 8, 4, 6, 6, 8, 10];
        var cadirlar = new List<Cadir>();
        foreach (var bolge in new[] { a, b, c })
        {
            for (var i = 0; i < 10; i++)
            {
                var sira = i / 5 + 1;
                var numara = i % 5 + 1;
                cadirlar.Add(new Cadir
                {
                    Bolge = bolge,
                    Sira = sira,
                    Numara = numara,
                    Kapasite = kapasiteler[i],
                    SaglikNoktasinaYakin = bolge == a,
                    Erisilebilir = (bolge == a && sira == 1) || (bolge == b && sira == 2 && numara <= 2),
                    Isitmali = bolge == b && sira == 1,
                    Durum = bolge == c && sira == 2 && numara == 5 ? CadirDurumu.Bakimda : CadirDurumu.Kullanilabilir,
                });
            }
        }

        db.Cadirlar.AddRange(cadirlar);
        return cadirlar;
    }

    private static Dictionary<string, Urun> UrunleriEkle(AfetDbContext db)
    {
        Urun U(string ad, UrunKategorisi k, Birim b, HesaplamaTuru h, decimal hak, int tekrar, decimal kritik) =>
            new() { Ad = ad, Kategori = k, Birim = b, HesaplamaTuru = h, HakMiktari = hak, TekrarAlmaGunu = tekrar, KritikSeviye = kritik };

        var urunler = new[]
        {
            // Şartname M6 tablosundaki varsayılanlar
            U("Su", UrunKategorisi.Su, Birim.Litre, HesaplamaTuru.KisiBasi, 3, 1, 500),
            U("Ekmek", UrunKategorisi.Gida, Birim.Adet, HesaplamaTuru.KisiBasi, 1, 1, 200),
            U("Bebek maması", UrunKategorisi.Mama, Birim.Adet, HesaplamaTuru.BebekBasi, 1, 3, 30),
            U("Bebek bezi", UrunKategorisi.Bez, Birim.Paket, HesaplamaTuru.BebekBasi, 1, 3, 30),
            U("Hijyen paketi", UrunKategorisi.Hijyen, Birim.Paket, HesaplamaTuru.AileBasi, 1, 7, 50),
            U("Battaniye", UrunKategorisi.Battaniye, Birim.Adet, HesaplamaTuru.KisiBasi, 1, 30, 100),
            // Ek demo ürünleri
            U("Konserve gıda", UrunKategorisi.Gida, Birim.Adet, HesaplamaTuru.KisiBasi, 2, 3, 100),
            U("Kuru gıda kolisi", UrunKategorisi.Gida, Birim.Paket, HesaplamaTuru.AileBasi, 1, 7, 20),
            U("Çocuk giyim seti", UrunKategorisi.Giyim, Birim.Paket, HesaplamaTuru.KisiBasi, 1, 30, 20),
            U("Yetişkin giyim seti", UrunKategorisi.Giyim, Birim.Paket, HesaplamaTuru.KisiBasi, 1, 30, 20),
        };

        db.Urunler.AddRange(urunler);
        return urunler.ToDictionary(u => u.Ad);
    }

    private static void GerekceUrunleriniEkle(AfetDbContext db, Dictionary<string, Urun> urunler)
    {
        foreach (var (kod, kategoriler) in ReferansVerisi.OncelikliKategoriler)
            foreach (var urun in urunler.Values.Where(u => kategoriler.Contains(u.Kategori)))
                db.GerekceUrunleri.Add(new GerekceUrun { GerekceId = (int)kod, UrunId = urun.Id });
    }

    private static Dictionary<string, Parti> BagisciVePartileriEkle(
        AfetDbContext db, Dictionary<string, Urun> urunler, DateTime simdi, DateOnly bugun)
    {
        var kizilay = new Bagisci { Ad = "Türk Kızılay", Aciklama = "Kurumsal bağış" };
        var afad = new Bagisci { Ad = "AFAD", Aciklama = "Kurumsal bağış" };
        var esnaf = new Bagisci { Ad = "Yerel Esnaf Derneği", Telefon = "0344 000 00 00" };
        db.Bagiscilar.AddRange(kizilay, afad, esnaf);

        var partiler = new Dictionary<string, Parti>();

        void P(string anahtar, string urunAdi, Bagisci bagisci, decimal miktar, int? sktGun, int girisGunOnce)
        {
            var parti = new Parti
            {
                Urun = urunler[urunAdi],
                Bagisci = bagisci,
                PartiNo = $"P-{bugun.AddDays(-girisGunOnce):yyMMdd}-{partiler.Count + 1:00}",
                SonKullanmaTarihi = sktGun is { } g ? bugun.AddDays(g) : null,
                GirisZamani = simdi.AddDays(-girisGunOnce),
            };
            var giris = new GirisHareketi { Parti = parti, Miktar = miktar, Zaman = parti.GirisZamani, Aciklama = "Demo verisi: ilk giriş" };
            giris.PartiyeUygula(parti);
            db.Partiler.Add(parti);
            db.StokHareketleri.Add(giris);
            partiler[anahtar] = parti;
        }

        P("su1", "Su", afad, 1400, 180, 5);
        P("su2", "Su", kizilay, 600, 240, 1);
        P("ekmek", "Ekmek", esnaf, 400, 2, 3);
        P("mama", "Bebek maması", kizilay, 25, 5, 4);        // SKT'si yaklaşmış ve kritik seviyenin altında
        P("bez1", "Bebek bezi", kizilay, 20, 300, 4);
        P("bez2", "Bebek bezi", afad, 15, 120, 2);
        P("hijyen", "Hijyen paketi", afad, 60, 365, 5);
        P("battaniye", "Battaniye", kizilay, 150, null, 5);
        P("konserve1", "Konserve gıda", afad, 200, 400, 5);
        P("konserve2", "Konserve gıda", esnaf, 40, 20, 3);
        P("konserveSktGecmis", "Konserve gıda", esnaf, 10, -2, 10); // SKT'si geçmiş: dağıtıma çıkamaz
        P("kuruGida", "Kuru gıda kolisi", kizilay, 30, 90, 5);
        P("cocukGiyim", "Çocuk giyim seti", esnaf, 15, null, 5);
        P("yetiskinGiyim", "Yetişkin giyim seti", esnaf, 25, null, 5);

        return partiler;
    }

    private static List<Aile> AileleriEkle(AfetDbContext db, DateTime simdi, DateOnly bugun)
    {
        var aileler = new List<Aile>();
        var tcSayaci = 0;
        for (var i = 0; i < Aileler.Length; i++)
        {
            var tanim = Aileler[i];
            var kayitZamani = simdi.AddDays(-6).AddHours(i);
            var aile = new Aile
            {
                AileAdi = $"{tanim.Soyad} Ailesi",
                TekEbeveyn = tanim.TekEbeveyn,
                KayitZamani = kayitZamani,
            };

            foreach (var uye in tanim.Uyeler)
            {
                var kisi = new Afetzede
                {
                    Ad = uye.Ad,
                    Soyad = tanim.Soyad,
                    DogumTarihi = bugun.AddYears(-uye.Yas).AddDays(-(i * 11 % 300) - 1),
                    Cinsiyet = uye.Cinsiyet,
                    Memleket = tanim.Memleket,
                    Yakinlik = uye.Yakinlik,
                    Hamile = uye.Hamile,
                    TekerlekliSandalye = uye.Tekerlekli,
                    KronikHasta = uye.Kronik,
                    RefakatsizCocuk = uye.Refakatsiz,
                    KayitZamani = kayitZamani,
                    // Demo amaçlı, algoritmaya uygun üretilmiş kimlik numaraları.
                    TcKimlikNo = uye.Kimliksiz ? null : TcKimlik.KontrolHaneleriyleTamamla($"{900000001 + tcSayaci++ * 7919}"),
                };
                kisi.Hareketler.Add(new KisiHareketi { Tur = KisiHareketTuru.Gelis, Zaman = kayitZamani, Aciklama = "Çadır kente geliş" });
                aile.Uyeler.Add(kisi);
            }

            aileler.Add(aile);
        }

        db.Aileler.AddRange(aileler);
        return aileler;
    }

    /// <summary>
    /// Aileleri sırayla, aileyi tek başına alabilecek ilk boş çadıra yerleştirir.
    /// A bölgesinin 1. sırası ve B bölgesinin ısıtmalı çadırları demo senaryosu için boş bırakılır.
    /// Yer bulunamayan aile bekleme listesine alınır.
    /// </summary>
    private static void Yerlestir(AfetDbContext db, List<Aile> aileler, List<Cadir> cadirlar, DateTime simdi)
    {
        var aday = cadirlar
            .Where(c => c.KullanilabilirMi && !(c.Bolge!.Kod == "A" && c.Sira == 1) && !c.Isitmali)
            .OrderBy(c => c.Bolge!.Kod == "A" ? 2 : c.Bolge!.Kod == "B" ? 1 : 0)
            .ThenBy(c => c.Sira).ThenBy(c => c.Numara)
            .ToList();

        foreach (var aile in aileler)
        {
            var cadir = aday.FirstOrDefault(c => c.Doluluk == 0 && c.Kapasite >= aile.Uyeler.Count);
            if (cadir is null)
            {
                db.BeklemeListesi.Add(new BeklemeListesiKaydi { Aile = aile, EklenmeZamani = aile.KayitZamani, Aciklama = "Uygun çadır bulunamadı" });
                continue;
            }

            cadir.Yerlestir(aile.Uyeler.Count);
            foreach (var kisi in aile.Uyeler)
            {
                db.Yerlesimler.Add(new Yerlesim { Kisi = kisi, Cadir = cadir, Baslangic = aile.KayitZamani });
                kisi.Hareketler.Add(new KisiHareketi
                {
                    Kisi = kisi, Tur = KisiHareketTuru.CadirDegisikligi, YeniCadir = cadir,
                    Zaman = aile.KayitZamani.AddMinutes(30), Aciklama = "İlk yerleşim",
                });
            }
        }
    }

    /// <summary>Stok tahmini için son 3 günün su ve ekmek dağıtımları (tamamı teslim edilmiş, kapanmış listeler).</summary>
    private static void GecmisDagitimlariEkle(
        AfetDbContext db, List<Aile> aileler, Dictionary<string, Urun> urunler,
        Dictionary<string, Parti> partiler, DateTime simdi, DateOnly bugun)
    {
        foreach (var (urunAdi, partiAnahtari, gunler) in new[] { ("Su", "su1", 3), ("Ekmek", "ekmek", 2) })
        {
            var urun = urunler[urunAdi];
            var parti = partiler[partiAnahtari];
            for (var gun = gunler; gun >= 1; gun--)
            {
                var zaman = simdi.Date.AddDays(-gun).AddHours(10);
                var dagitim = new Dagitim
                {
                    Urun = urun, Tarih = bugun.AddDays(-gun), Durum = DagitimDurumu.Kapandi,
                    OlusturmaZamani = zaman.AddHours(-1), OnayZamani = zaman.AddMinutes(-30), Aciklama = "Demo verisi",
                };

                foreach (var aile in aileler)
                {
                    var hak = urun.HakMiktari * aile.Uyeler.Count;
                    var detay = new DagitimDetay { Aile = aile, HakMiktari = hak };
                    parti.Ayir(hak, bugun.AddDays(-gun));
                    var ayirma = new StokAyirma { Parti = parti, Miktar = hak, OlusturmaZamani = zaman.AddMinutes(-30) };

                    var teslim = new DagitimHareketi
                    {
                        Parti = parti, DagitimDetay = detay, Miktar = hak, Zaman = zaman,
                        TeslimAlanKisi = aile.Uyeler[0], Aciklama = "Demo verisi: teslim",
                    };
                    teslim.PartiyeUygula(parti);
                    detay.TeslimKaydet(hak);
                    ayirma.TeslimEdilenMiktar = hak;
                    ayirma.Durum = AyirmaDurumu.Kapandi;
                    ayirma.KapanisZamani = zaman;

                    detay.Ayirmalar.Add(ayirma);
                    dagitim.Detaylar.Add(detay);
                    db.StokHareketleri.Add(teslim);
                }

                db.Dagitimlar.Add(dagitim);
            }
        }
    }

    private static void DuyurulariEkle(AfetDbContext db, DateTime simdi)
    {
        db.Duyurular.AddRange(
            new Duyuru
            {
                Baslik = "Sağlık taraması",
                Metin = "Yarın 10:00–16:00 arasında A bölgesindeki sağlık noktasında genel sağlık taraması yapılacaktır.",
                BaslangicTarihi = simdi.Date, BitisTarihi = simdi.Date.AddDays(2).AddHours(23),
            },
            new Duyuru
            {
                Baslik = "Su dağıtım saati",
                Metin = "Su dağıtımı her gün 09:00–11:00 saatleri arasında depo önünde yapılmaktadır.",
                BaslangicTarihi = simdi.Date.AddDays(-5), BitisTarihi = simdi.Date.AddDays(10),
            },
            new Duyuru
            {
                Baslik = "Geçmiş duyuru",
                Metin = "Bu duyurunun süresi dolmuştur; kioskta görünmemelidir.",
                BaslangicTarihi = simdi.Date.AddDays(-10), BitisTarihi = simdi.Date.AddDays(-3),
            });
    }
}
