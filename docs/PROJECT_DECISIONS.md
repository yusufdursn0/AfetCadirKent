# Proje Kararları

Önemli teknik kararlar, belirsizliklerin çözümü ve gerekçeleri. Her karar `K-xxx` numarasıyla anılır.

---

## A. Kaynaklar ve yapay zeka beyanı

**K-001 – Kaynak tutarlılığı.** `docs/Donem_Projesi_Foyu.pdf` ve `docs/Afet_Cadir_Kent_Proje_Son_Hali.pdf` okundu; CLAUDE.md özetiyle çelişki yok.
Föy kendi içinde bir tutarsızlık barındırıyor: Bölüm 5'te raporlama "opsiyonel", yapay zeka "opsiyonel – bonus" denirken Bölüm 2.5 ve rubrik yapay zekayı "zorunlu" sayıyor. **Karar:** Daha katı yorum uygulanır; raporlama ve yapay zeka zorunlu kabul edilir (şartname de ikisini istiyor).

**K-002 – Yapay zeka kullanım beyanı.** Bu projenin geliştirilmesinde kod yardımcısı olarak **Claude Code (Anthropic)** kullanılmıştır. Uygulama içi yapay zeka özelliği yerel **Ollama** modeliyle sağlanır. Beyan README'de de yer alır (Föy 8).

**K-003 – Otonom çalışma (2026-10-16, kullanıcı kararı).** Kullanıcı Claude Code'u auto modda çalıştırır; aşamalar arasında onay beklenmez (CLAUDE.md 1.2.1). Duraklanan noktalar: kullanıcının yapması gereken kurulumlar (DevExpress, Ollama), şartnameyi değiştiren kritik kararlar ve geri alınamaz işlemler (CLAUDE.md 6.2). Geliştirme veritabanı yalnızca Aşama 5'ten önce sormadan yeniden oluşturulabilir.

**K-004 – Geliştirmenin günlere yayılması (2026-10-16, kullanıcı kararı).** Föy 2.4 ve 8 gereği commit geçmişi gerçek süreci yansıtır: her oturumda bir iş paketi yapılır, commit'ler gerçek zamanında atılır; commit tarihleri değiştirilmez, hazır kod sonradan parça parça yüklenmez. Takvim `PROJECT_STATUS.md`'de. DevExpress 30 günlük denemesi sunumu kapsasın diye kurulum ~5 Kasım'a planlandı.

## B. Platform

**K-010 – .NET 10 (LTS).** Kurulu SDK'lar: 9.0.317 ve 10.0.402. .NET 10 güncel LTS sürümüdür. Entities/DataAccess/Business/Tests `net10.0`, UI `net10.0-windows`. DevExpress sürümü kurulumda .NET 10'u destekleyen en yeni sürüm olarak seçilecek (Aşama 5).

**K-011 – Kütüphaneler.** EF Core 10 (SqlServer, Design/Tools), BCrypt.Net-Next, Microsoft.Extensions.* (Configuration, DependencyInjection, Logging), Serilog (dosya log), xUnit. UI'da DevExpress WinForms + XtraReports.

**K-012 – Bağımlılık enjeksiyonu.** UI `Program.cs` içinde `Microsoft.Extensions.DependencyInjection` ile servisler kaydedilir. WinForms'ta uzun ömürlü form + kısa ömürlü veritabanı işlemi gerektiği için DbContext `IDbContextFactory<AfetDbContext>` ile her iş işlemi için ayrı oluşturulur.

**K-013 – Yapılandırma.** `appsettings.json` (git'e girmez) içinde: `ConnectionStrings:AfetDb`, `Ollama:Adres`, `Ollama:Model`, `Ollama:ZamanAsimiSaniye`, `Uygulama:SktUyariGunu`, `Uygulama:YedekKlasoru`. Repoda yalnızca `appsettings.example.json` bulunur. Dosya yoksa uygulama açılışta kurulum adımını gösteren mesajla kapanır.

**K-014 – Solution dosyası `.slnx`.** .NET 10 SDK ve Visual Studio 2026'nın varsayılan yeni solution biçimi kullanıldı (`AfetCadirKent.slnx`); CLAUDE.md'deki `.sln` adı yerine geçer, içerik ve proje yapısı aynıdır.

**K-015 – Güvenlik açığı olan dolaylı paket.** EF Core SqlServer sağlayıcısı dolaylı olarak `System.Security.Cryptography.Pkcs 9.0.11` getiriyordu (bilinen orta/yüksek açık, NU1902/NU1903). DataAccess projesine paketin güncel sürümü doğrudan eklenerek giderildi; derleme uyarısızdır.

**K-016 – Geçici başlangıç mesajları.** DevExpress kurulana kadar (Aşama 5) `Program.cs` ayar dosyası kontrolünde standart `MessageBox` kullanır; Aşama 5'te `XtraMessageBox` ile değiştirilecektir.

## C. Veri modeli

**K-020 – Kalıtım eşlemesi (Kisi → Afetzede, Gorevli, Gonullu).** `Kisi` soyut bir C# sınıfıdır ve EF modelinde ayrı tablo olarak eşlenmez; ortak alanları (ad, soyad, telefon, doğrulama) taşır. Her alt sınıf kendi tablosuna gider:
`Afetzede → Kisiler`, `Gorevli → Kullanicilar`, `Gonullu → Gonulluler`.
Gerekçe: Şartnamedeki tablo listesi (Kişiler, Kullanıcılar, Gönüllüler ayrı) korunur; ortak sorgu ihtiyacı yoktur, TPH'nin seyrek kolonlarından ve TPC'nin ortak sekans gerekliliğinden kaçınılır. Kalıtım ve kapsülleme kodda görünür kalır.

**K-021 – StokHareketi (soyut) → TPH.** `StokHareketleri` tek tablo, `HareketTuru` ayırıcı (discriminator). Alt sınıflar: `GirisHareketi`, `DagitimHareketi`, `IadeHareketi`, `HasarHareketi`, `SayimFarkiHareketi`. Her biri soyut `StokEtkisi` (miktarın stoğa işareti/etkisi) özelliğini kendi uygular → polimorfizm.

**K-022 – Ek tablolar.** Şartname "yaklaşık 25 tablo" der; listedeki 26 tabloya ek olarak:
- `StokAyirmalari` – bir dağıtım satırının FEFO ile birden fazla partiden ayırma yapabilmesi için (DagitimDetay ↔ Parti, miktar, durum).
- `KullaniciRolleri` yok; kullanıcının tek rolü vardır (`Kullanicilar.RolId`).
Toplam 27 tablo. Tablo isimleri Türkçe karaktersiz (`Cadirlar`, `KisiHareketleri`…).

**K-023 – Numaralar.** Aile numarası `A-000001`, kişi numarası `K-000001` biçiminde. Veritabanında Id'den hesaplanan **kalıcı (persisted) hesaplanmış kolon** olarak üretilir (`'A-' + RIGHT('000000' + CAST(Id AS varchar), 6)`), unique indekslidir; uygulama kodu numara üretmez, çakışma olamaz. Kiosk girişi aile numarasıyla yapılır.

**K-024 – Doğum tarihi.** Kimliksiz kişilerde tam tarih bilinmeyebilir. `DogumTarihi` zorunlu; tahmini ise `DogumTarihiTahmini = true` ve tarih 1 Ocak olarak girilir. Yaş hesapları bu tarihten yapılır.

**K-025 – TC kimlik.** Opsiyonel; girilirse 11 hane + resmî algoritma (10. ve 11. hane kontrolü, ilk hane 0 olamaz) ile doğrulanır. Dolu TC'ler için filtreli unique indeks (`WHERE TcKimlikNo IS NOT NULL`).

**K-026 – Yerleşim kişi bazındadır.** `Yerlesimler` (KisiId, CadirId, Baslangic, Bitis). Çadır doluluğu = bitişi olmayan yerleşim sayısı. Böylece "bir aile iki çadıra yerleşebilir" doğal olarak sağlanır; hastanedeki kişinin yerleşimi kapanmaz (yeri korunur).

**K-027 – Çadırda "sıcak çadır" bilgisi.** Gerekçe tablosundaki "Sıcak çadır" yerleşim etkisini karşılamak için `Cadirlar.Isitmali` alanı eklendi (şartname alanı tutmayı açıkça istemese de yerleşim etkisinin uygulanabilmesi için gerekli). "Büyük çadır" etkisi kapasiteyle (≥ 6) karşılanır. "Koordinatör takibi" ve "Görevli takibi" çadır seçimini etkilemez; ekranda işaret olarak gösterilir.

**K-028 – Silme / pasif yapma.** Pasif yapılabilen entity'ler `IPasifYapilabilir` (`Aktif` alanı) arayüzünü uygular; servisler bu kayıtları fiziksel olarak silmez. Silinebilir entity'lerde (Ürün, Çadır, Bölge, Kullanıcı, Gönüllü) önce kullanım kontrolü yapılır; kullanılmışsa `SilinemezException` → "Bu kayıt kullanıldığı için silinemez, pasif yapabilirsiniz." Bu entity'ler de `Aktif` alanı taşır. FK'lerde cascade delete kapalıdır (`Restrict`), yalnızca saf alt kayıtlarda (ör. GerekceUrunleri) cascade açıktır.

**K-029 – RowVersion.** `Cadirlar`, `Partiler`, `Aileler`, `Talepler`, `DagitimDetaylari` `RowVersion` (rowversion) taşır. Çakışmada `DbUpdateConcurrencyException` Business katmanında `EszamanlilikException`'a çevrilir: "Bu kayıt siz işlem yaparken başka bir kullanıcı tarafından değiştirildi. Liste yenilendi, lütfen tekrar deneyin."

## D. İş kuralları – belirsizliklerin çözümü

**K-040 – Gerekçe kuralları.** `IGerekceKurali` arayüzü: `Kod`, `Ad`, `DestekOnceligi`, `YerlesimEtkisi`, `OncelikliKategoriler`, `UygulanirMi(AileBaglami)`. 10 kural ayrı sınıflardır. Otomatik tespit:
- Bebek: yaşı ≤ 2 olan (0–2 yaş) aktif üye.
- 65 yaş üstü: yaşı ≥ 65 olan aktif üye ("65 yaş üstü" ifadesi 65'i de kapsayacak şekilde yorumlandı).
- 5 kişiden kalabalık: aktif üye sayısı > 5.
- Hamile / tekerlekli sandalye / kronik hasta / refakatsiz çocuk: kişi kaydındaki işaretler.
- Tek ebeveyn: aile kaydındaki işaret (görevli belirler).
- 3 gündür yardım almadı: aile ≥ 3 gündür kayıtlı ve son 3 günde hiç teslim almamış.
- Özel durumu yok: başka gerekçe yoksa.
Otomatik gerekçeler **saklanmaz, her okumada güncel veriden hesaplanır** (yaş, aile büyüklüğü ve "3 gündür yardım almadı" zamanla değiştiği için; aileden ayırmada iki aile için yeniden hesap kendiliğinden olur). `AileGerekceleri` tablosu yalnızca görevlinin elle yaptığı değişiklikleri tutar: `ElleEklendi` / `ElleKaldirildi`, açıklama zorunlu, zaman ve kullanıcıyla. Geçerli değişiklik (`Gecerli = true`) hesaplanan sonuca uygulanır; geri alınan değişiklik silinmez, `Gecerli = false` olur.

**K-041 – Gerekçe ↔ ürün.** Kurallar öncelikli malzemeleri **ürün kategorisi** olarak döndürür (Mama, Bez, Battaniye, Gıda, Hijyen, Giyim; "Tüm ürünler"). Seed sırasında `GerekceUrunleri` bu kategorilerdeki ürünlerle doldurulur; Ayarlar ekranındaki "gerekçe ayarları" bu eşleşmeleri düzenler ve dağıtım veritabanındaki eşleşmeyi kullanır. Destek önceliği ve yerleşim etkisi şartname tablosuna sabittir.

**K-042 – Destek önceliği = en yüksek gerekçe.** Yüksek > Orta > Normal.

**K-043 – Çadır öneri algoritması (`CadirOneriMotoru`).** Filtre: çadır ve bölgesi aktif, `Durum = Kullanilabilir`, boş yatağı var. Sıralama:
1. Yeterli boş yer (yerleşecek kişi sayısı ≤ boş yatak),
2. Yerleşim etkisi uyumu: ailenin gerekçelerinden gelen çadır etkilerinden (sıcak çadır → ısıtmalı, sağlık noktasına yakın, erişilebilir, büyük çadır → kapasite ≥ 6) karşılanan sayısı çok olan önce. "Koordinatör/görevli takibi" çadır seçmez.
3. En az boş yatak israfı (boş yatak − kişi sayısı),
4. Eşitlikte çadır kodu.
Tek çadır yetmiyorsa **aynı bölgede iki çadırlık** öneriler üretilir (boş yeri büyük olan önce doldurulur); iki çadırlı öneride bir etki ancak iki çadır da karşılıyorsa karşılanmış sayılır. En fazla 5 öneri döner. Hiç öneri yoksa "bekleme listesine alabilirsiniz" mesajı verilir. Sistem yalnızca önerir; yerleşim görevli onayıyla (`YerlestirAsync`) yapılır ve görevli öneri dışı bir çadır da seçebilir. Onay anında kapasite entity kuralıyla yeniden kontrol edilir, eşzamanlı yerleştirmede çadırın RowVersion'ı ikinci işlemi reddeder. Yerleştirmede ailenin **yerleşmemiş tüm üyeleri** yerleşir; atanan kişi sayısı bu sayıya eşit olmalıdır; iki çadırda üyeler kayıt sırasıyla dağıtılır (gerekirse görevli çadır değişikliğiyle düzeltir).

**K-044 – Bekleme listesi.** Görevli, çadıra yerleşmemiş üyesi olan bir aileyi listeye alabilir (bir aile aynı anda bir kez). Sıralama: destek önceliği yüksek olan önce, eşitlikte en uzun bekleyen (`BeklemeSiralamasi`). Yer açılan çadır için öneri: yerleşmemiş kişi sayısı boş yatağa sığan aileler bu sırayla; ilk sıradaki sistemin önerisidir. `YerAcilanCadirlarAsync` yeri olan her çadır için listedeki ilk uygun aileyi verir (bekleme listesi ekranındaki bildirim). Görevli onaylayınca aile yerleşir ve listeden çıkar; aile başka yoldan (çadır ekranından) yerleşse de listeden otomatik çıkar. Yerleştirmeden çıkarma açıklama ister.

**K-045a – Çadır ve bölge düzenleme kuralları.** Çadırda kişi varken çadır "Kapalı" veya pasif yapılamaz ("Bakımda" yapılabilir; bakımdaki çadıra yeni yerleşim yapılmaz). Kapasite doluluğun altına düşürülemez. Aynı bölgede aynı sıra-numara ve aynı kodlu bölge tekrar eklenemez. Çadır, yerleşim / kişi hareketi / talep kaydında geçmişse; bölge, çadırı veya vardiyası varsa silinemez ("Bu kayıt kullanıldığı için silinemez, pasif yapabilirsiniz.").

**K-045b – Çadır değişikliği.** Yalnızca şu an yerleşik kişiler taşınır; eski yerleşim kapanır, eski çadırın doluluğu düşer, yeni yerleşim açılır; `CadirDegisikligi` hareketi eski ve yeni çadırla yazılır; açıklama zorunlu.

**K-045c – Çadır planı.** `CadirPlaniAsync` her çadır için kod, kapasite, doluluk, durum, özellikler ve içindeki aileleri döndürür; renk `PlanDurumu`'ndan (Boş / Kısmen dolu / Dolu / Kullanılamaz) belirlenir. Refakatsiz çocuk ailelerinin adları Koordinatör dışındaki rollerde listelenmez (doluluk yine sayılır).

**K-045 – Hak hesabı.** Hak sayımında durumu **Burada** veya **Hastanede** olan aktif üyeler sayılır (hastanedeki kişinin hakkı ailede kalır). Bebek başı = 0–2 yaş üyeler. Aile başı = 1 × miktar. Hak 0 ise aile listede çıkmaz.

**K-046 – Tekrar alma süresi ve bekleyen hak.** Ailenin o üründe son teslim aldığı dönemin başlangıcı + tekrar alma süresi bugünden büyükse yeni hak doğmaz. Kısmi teslimatta kalan miktar `DagitimDetay` satırında **bekleyen hak** olarak durur; sonraki dağıtım listelerinde, süre dolmamış olsa bile bu bekleyen miktar listeye gelir. İstisna: görevli gerekçe yazarak süre dolmamış aileye hak tanıyabilir (`IstisnaGerekcesi`).

**K-047 – Ayırma ve gün sonu.** Liste onayında FEFO ile ayırma yapılır (`StokAyirmalari`, `Partiler.AyrilanMiktar` artar). Teslimde ayırma `Teslim Edildi` olur, parti miktarı düşer, `DagitimHareketi` oluşur. Arka plan servisi olmayan bir masaüstü uygulaması olduğundan "gün sonu" işlemi: (a) Dağıtım ekranındaki **Gün sonu** butonuyla, (b) uygulama açılışında ve dağıtım ekranı yüklenirken önceki günlerden kalan açık ayırmalar otomatik serbest bırakılır.

**K-048 – SKT.** SKT'si bugünden önce olan parti `Kullanilabilir` stoka dahil edilmez, FEFO seçiminde dışlanır; Stok durumu ekranındaki "SKT geçenleri fireye düş" işlemi `HasarHareketi` oluşturur. SKT yaklaşıyor uyarısı: `SktUyariGunu` (varsayılan 7) gün içinde.

**K-049 – Stok tahmini.** Günlük ortalama tüketim = son 7 gündeki dağıtım hareketleri toplamı ÷ verinin kapsadığı gün sayısı (en fazla 7). Dağıtım verisi yoksa "hesaplanamıyor". İkinci tahmin: günlük ihtiyaç = (mevcut kişi/aile/bebek sayısına göre toplam hak) ÷ tekrar alma süresi; yeterlilik = kullanılabilir stok ÷ günlük ihtiyaç. Ekranda hesabın dayandığı tarih aralığı gösterilir.

**K-050 – Talep yönlendirme ve atama.** Talep türü belirlenince hedef rol atanır (Sağlık → Sağlık personeli, Malzeme → Depo sorumlusu, Çadır/arıza → Saha görevlisi, Diğer → Koordinatör). Belirli bir kullanıcı seçilince durum `Atandı` olur. Geçişler: Yeni→Atandı, Yeni→İptal, Atandı→İşlemde, Atandı→İptal, İşlemde→Tamamlandı, İşlemde→İptal. Kapanışta sonuç zorunlu. Sağlık talebinde `YardimOnceligi = null` ve `SaglikPersoneliDegerlendirecek = true`.

**K-051 – Yapay zeka öncelik ayrımı.** Yapay zeka sağlık türü döndürürse, döndürdüğü öncelik Business katmanında **yok sayılır**. Prompt'ta tıbbi aciliyet değerlendirmesi yapmaması açıkça belirtilir. "Acil" kelimesi prompt çıktısında ve arayüzde kullanılmaz.

**K-052 – Kiosk girişi.** Kiosk modu, `Kiosk` rolündeki bir kullanıcı hesabıyla açılır (tam ekran). Afetzede aile numarası + ailedeki aktif bir üyenin doğum yılıyla giriş yapar. Kiosk yalnızca o ailenin taleplerini ve aktif duyuruları gösterir; çadır/iletişim bilgisi göstermez.

**K-053 – Gönüllü görev ataması.** `Vardiyalar` (GonulluId, Baslangic, Bitis, BolgeId, GorevTanimi, opsiyonel TalepId). Görev ataması vardiyaya görev tanımı yazılarak veya bir talebe bağlanarak yapılır. Gönüllüler giriş yapmaz.

**K-054 – Yakın bulma.** Başvuru: aranan kişinin adı, soyadı, yaklaşık yaşı, memleketi, başvuran bilgisi. Arama Türkçe normalize edilmiş ad/soyad üzerinde yapılır, yaş ±5 ve memleket ile daraltılır. Eşleşme bulunursa durum `Olası eşleşme`; görevli doğrulamayı yazılı açıklamayla yapar (fotoğraf benzerliği tek başına kabul edilmez). Sonuç ekranında çadır ve telefon bilgisi gösterilmez.

**K-055 – Refakatsiz çocuk görünürlüğü.** Kişi sorgularında Business katmanı, oturumdaki rol Koordinatör değilse `RefakatsizCocuk = true` kişileri sonuçtan çıkarır.

**K-056 – Mükerrer kontrol.** TC dolu ve veritabanında varsa kayıt engellenir. TC yoksa normalize ad + soyad eşit ve |yaş farkı| < 2 olan kişiler "olası eşleşme" olarak döner; görevli yine de kaydetmeye karar verebilir.

**K-057 – Yedekleme.** `BACKUP DATABASE … TO DISK` ve `RESTORE DATABASE … WITH REPLACE` (önce `SINGLE_USER WITH ROLLBACK IMMEDIATE`). Yalnızca Koordinatör. Yedek klasörü ayardan okunur.

**K-058 – Bağlantı kesintisi.** Kaydetme işlemleri try/catch ile sarılır; `SqlException`/bağlantı hatasında işlem transaction içinde olduğu için geri alınır ve kullanıcıya "İşlem kaydedilemedi, bağlantıyı kontrol edip tekrar deneyin" denir. Başarılı kayıtta "Kaydedildi" bildirimi verilir. Belirsiz durum (commit sırasında kopma) için kayıt yeniden okunarak kullanıcıya gerçek durum bildirilir.

**K-059 – Yetki matrisi.** `ReferansVerisi.YetkiMatrisi` (migration ile `Yetkiler` tablosuna yazılır, 68 kayıt):
| Rol | Modül: izinler |
|---|---|
| Koordinatör | Tüm modüller: görme, değiştirme, rapor |
| Kayıt görevlisi | Kayıt, Çadır: G/D/R · Talep, Yakın bulma: G/D |
| Depo sorumlusu | Depo, Dağıtım: G/D/R · Kayıt: G · Talep: G/D (malzeme talepleri depoya yönlenir) |
| Sağlık personeli | Sağlık bilgisi: G/D · Talep: G/D (yalnız sağlık talepleri, Business filtreler) · Kayıt: G |
| Saha görevlisi | Talep: G/D (yalnız kendine atananlar, Business filtreler) · Çadır: G |
| Kiosk | Kiosk: G/D |
Gerekçe: talepler "kayıt masasından" da gelir (kayıt görevlisi talep açar); yakın bulmayı "yetkili görevli" yapar (kayıt görevlisi + koordinatör); depo dağıtımda aileleri görmelidir.

**K-060 – Demo verisi ayrıntıları.** Uygulama açılışında veritabanı boşsa yüklenir (`VeritabaniHazirlayici`); tarihler yükleme anına göre hesaplanır. İçerik: 3 bölge (A: sağlık noktasına yakın, 1. sıra erişilebilir; B: 1. sıra ısıtmalı; C: genel, C2-05 bakımda), 30 çadır, 20 aile / 56 kişi (bebekli, hamile, tekerlekli sandalyeli, kronik hasta, yaşlı, tek ebeveyn, kalabalık, refakatsiz çocuk; bir kısmı kimliksiz), 10 ürün (şartnamedeki 6 + konserve, kuru gıda, çocuk/yetişkin giyim), 3 bağışçı, 14 parti (bebek maması: SKT 5 gün içinde ve kritik seviyenin altında; bir konserve partisinin SKT'si geçmiş), son 3 günün su / son 2 günün ekmek dağıtımları (stok tahmini için), 3 duyuru (biri süresi dolmuş). A bölgesinin 1. sırası ve B'nin ısıtmalı çadırları demo senaryosu (bebekli + hamile aile) için boş bırakılır. Demo TC numaraları algoritmaya uygun üretilmiş kurgusal numaralardır. Gerekçe–ürün eşleşmeleri ürün kategorilerine göre kurulur.

**K-061 – İstisna sınıflarının yeri.** `IsKuraliException` ve `DogrulamaException` entity'lerin kendi doğrulamalarında da kullanıldığı için Entities katmanındadır (`Entities.Ortak`). Business'a özgü istisnalar (`YetkisizIslemException`, `SilinemezException`, `EszamanlilikException`) Business katmanında `IsKuraliException`'dan türetilir.

**K-062 – Veritabanı düzeyinde ek korumalar.** İş kuralları Business'ta uygulanır; ayrıca son savunma hattı olarak CHECK kısıtları: çadır doluluğu 0…kapasite, parti ayrılan ≤ miktar, dağıtım teslim ≤ hak, sağlık talebinde yardım önceliği boş, stok hareketi miktarı > 0. Tekil indeksler: TC (dolu olanlar), kişi başına tek aktif yerleşim, aile başına tek aktif bekleme kaydı, bir dağıtım listesinde aile başına tek satır.

**K-063 – Veri oturumu.** Business servisleri her işlem için `IVeriOturumuFabrikasi.Ac()` ile kısa ömürlü bir `VeriOturumu` (unit of work) açar; aynı oturumdaki `Repo<T>()` çağrıları aynı DbContext'i paylaşır (repository'ler `Dictionary<Type, object>` içinde tutulur).

**K-064 – Kişi durum geçişleri.** İzin verilen geçişler ve kişi hareketleri:
| Eski → Yeni | Hareket | Çadırdaki yer |
|---|---|---|
| Burada → Hastanede | Hastaneye sevk | Korunur |
| Hastanede → Burada | Hastaneden dönüş | — |
| Burada/Hastanede → Ayrıldı | Ayrılış | Boşalır (yerleşim kapanır, doluluk −1) |
| Burada/Hastanede → Nakil | Nakil | Boşalır |
| Ayrıldı/Nakil → Burada | Geliş | Çadırsız döner; yerleşim çadır ekranından yapılır |
Diğer geçişler (ör. Ayrıldı → Hastanede) "önce 'Burada' kaydedin" mesajıyla reddedilir. Pasif kaydın durumu değiştirilemez.

**K-065 – Aileden ayırma kuralları.** Taşınacak kişilerin hepsi o aileye ait ve çadır kentte (Burada/Hastanede, aktif) olmalı; eski ailede en az bir kişi kalmalı; gerekçe (açıklama) zorunlu. Yeni aile yeni numara alır; `AileGecmisi` (Ayırma) ve her kişi için `AileDegisikligi` hareketi yazılır. Kişilerin çadır yerleşimi değişmez (ayrı çadıra geçiş çadır ekranından yapılır). Gerekçe ve haklar kayıtlı veriden hesaplandığı için iki aile için kendiliğinden yeniden hesaplanır. Elle yapılmış gerekçe değişiklikleri eski ailede kalır. Kaynak ailenin RowVersion'ı kontrol edilir; aynı aile eşzamanlı iki kez ayrılamaz.

**K-066 – Pasif yapma.** Kişi pasif yapılırken aktif yerleşimi kapanır ve çadırdaki yer boşalır (açıklama zorunlu, işlem geçmişine yazılır). Aile ancak çadır kentte üyesi kalmadıysa pasif yapılabilir; bekleme listesindeyse listeden çıkar. Kayıtlar fiziksel olarak silinmez.

**K-067 – Sağlık bilgisi görünürlüğü.** `SaglikNotu` yalnızca "Sağlık bilgisi" görme izni olan rollere (Koordinatör, Sağlık personeli) döner; diğer rollerde sorgu sonucunda boşaltılır. Kayıt sırasında sağlık notu girmek "Sağlık bilgisi" değiştirme izni ister. Hamile/kronik hasta/tekerlekli sandalye işaretleri gerekçe hesabı için gerektiğinden kayıt görevlisi tarafından da işaretlenebilir.

**K-068 – Refakatsiz çocuk görünürlüğü (uygulama).** Koordinatör dışındaki rollerde kişi listesi, aile ayrıntısı, kişi hareketleri ve mükerrer kontrolün olası eşleşmeleri refakatsiz çocukları içermez; tüm üyeleri refakatsiz çocuk olan aile "bulunamadı" olarak döner. Dağıtım listelerindeki gösterim P4'te ele alınacak.

**K-069 – Fotoğraf küçültme.** 300×300 küçültme ve kamera ile çekme arayüz katmanında (DevExpress CameraControl, Aşama 6) yapılır; Business, gelen `byte[]`'ı saklar. Gerekçe: görüntü işleme Windows'a özgü kütüphaneler gerektirir; Business platform bağımsız tutulur.

## E. Test

**K-070** – Saf iş kuralları (normalizasyon, gerekçe, öneri, FEFO, hak, tahmin, talep durum makinesi, yapay zeka cevap ayrıştırma) veritabanısız birim testlerle; servisler ayrı LocalDB test veritabanı `AfetCadirKent_Test` üzerinde entegrasyon testleriyle test edilir. Ollama testlerinde sahte `HttpMessageHandler` kullanılır; gerçek Ollama gerekmez.

**K-071 – Test veritabanı bağlantısı.** Test fixture'ı bağlantıyı `SqlConnectionStringBuilder` ile kurar: LocalDB, Windows kimlik doğrulaması, veritabanı `AfetCadirKent_Test`. Parola veya gizli bilgi içermez; sunucu `AFETCADIRKENT_TEST_SUNUCU` ortam değişkeniyle değiştirilebilir. Test oturumu başında bu veritabanı silinip yeniden kurulur; veri değiştiren testler transaction içinde çalışıp geri alınır.
