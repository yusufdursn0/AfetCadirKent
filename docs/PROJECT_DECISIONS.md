# Proje Kararları

Önemli teknik kararlar, belirsizliklerin çözümü ve gerekçeleri. Her karar `K-xxx` numarasıyla anılır.

---

## A. Kaynaklar ve yapay zeka beyanı

**K-001 – Kaynak tutarlılığı.** `docs/Donem_Projesi_Foyu.pdf` ve `docs/Afet_Cadir_Kent_Proje_Son_Hali.pdf` okundu; CLAUDE.md özetiyle çelişki yok.
Föy kendi içinde bir tutarsızlık barındırıyor: Bölüm 5'te raporlama "opsiyonel", yapay zeka "opsiyonel – bonus" denirken Bölüm 2.5 ve rubrik yapay zekayı "zorunlu" sayıyor. **Karar:** Daha katı yorum uygulanır; raporlama ve yapay zeka zorunlu kabul edilir (şartname de ikisini istiyor).

**K-002 – Yapay zeka kullanım beyanı.** Bu projenin geliştirilmesinde kod yardımcısı olarak **Claude Code (Anthropic)** kullanılmıştır. Uygulama içi yapay zeka özelliği yerel **Ollama** modeliyle sağlanır. Beyan README'de de yer alır (Föy 8).

## B. Platform

**K-010 – .NET 10 (LTS).** Kurulu SDK'lar: 9.0.317 ve 10.0.402. .NET 10 güncel LTS sürümüdür. Entities/DataAccess/Business/Tests `net10.0`, UI `net10.0-windows`. DevExpress sürümü kurulumda .NET 10'u destekleyen en yeni sürüm olarak seçilecek (Aşama 5).

**K-011 – Kütüphaneler.** EF Core 10 (SqlServer, Design/Tools), BCrypt.Net-Next, Microsoft.Extensions.* (Configuration, DependencyInjection, Logging), Serilog (dosya log), xUnit. UI'da DevExpress WinForms + XtraReports.

**K-012 – Bağımlılık enjeksiyonu.** UI `Program.cs` içinde `Microsoft.Extensions.DependencyInjection` ile servisler kaydedilir. WinForms'ta uzun ömürlü form + kısa ömürlü veritabanı işlemi gerektiği için DbContext `IDbContextFactory<AfetDbContext>` ile her iş işlemi için ayrı oluşturulur.

**K-013 – Yapılandırma.** `appsettings.json` (git'e girmez) içinde: `ConnectionStrings:AfetDb`, `Ollama:Adres`, `Ollama:Model`, `Ollama:ZamanAsimiSaniye`, `Uygulama:SktUyariGunu`, `Uygulama:YedekKlasoru`. Repoda yalnızca `appsettings.example.json` bulunur. Dosya yoksa uygulama açılışta kurulum adımını gösteren mesajla kapanır.

## C. Veri modeli

**K-020 – Kalıtım eşlemesi (Kisi → Afetzede, Gorevli, Gonullu).** `Kisi` soyut bir C# sınıfıdır ve EF modelinde ayrı tablo olarak eşlenmez; ortak alanları (ad, soyad, telefon, doğrulama) taşır. Her alt sınıf kendi tablosuna gider:
`Afetzede → Kisiler`, `Gorevli → Kullanicilar`, `Gonullu → Gonulluler`.
Gerekçe: Şartnamedeki tablo listesi (Kişiler, Kullanıcılar, Gönüllüler ayrı) korunur; ortak sorgu ihtiyacı yoktur, TPH'nin seyrek kolonlarından ve TPC'nin ortak sekans gerekliliğinden kaçınılır. Kalıtım ve kapsülleme kodda görünür kalır.

**K-021 – StokHareketi (soyut) → TPH.** `StokHareketleri` tek tablo, `HareketTuru` ayırıcı (discriminator). Alt sınıflar: `GirisHareketi`, `DagitimHareketi`, `IadeHareketi`, `HasarHareketi`, `SayimFarkiHareketi`. Her biri soyut `StokEtkisi` (miktarın stoğa işareti/etkisi) özelliğini kendi uygular → polimorfizm.

**K-022 – Ek tablolar.** Şartname "yaklaşık 25 tablo" der; listedeki 26 tabloya ek olarak:
- `StokAyirmalari` – bir dağıtım satırının FEFO ile birden fazla partiden ayırma yapabilmesi için (DagitimDetay ↔ Parti, miktar, durum).
- `KullaniciRolleri` yok; kullanıcının tek rolü vardır (`Kullanicilar.RolId`).
Toplam 27 tablo. Tablo isimleri Türkçe karaktersiz (`Cadirlar`, `KisiHareketleri`…).

**K-023 – Numaralar.** Aile numarası `A-000001`, kişi numarası `K-000001` biçiminde, kimlik (identity) değerinden türetilir ve unique indekslidir. Kiosk girişi aile numarasıyla yapılır.

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
Görevli elle gerekçe ekleyip çıkarabilir (`AileGerekceleri.Kaynak = Elle`, açıklama zorunlu). Elle çıkarılan otomatik gerekçe yeniden hesaplamada geri gelmez (`Kaldirildi` işaretiyle saklanır).

**K-041 – Gerekçe ↔ ürün.** Kurallar öncelikli malzemeleri **ürün kategorisi** olarak döndürür (Mama, Bez, Battaniye, Gıda, Hijyen, Giyim; "Tüm ürünler"). Seed sırasında `GerekceUrunleri` bu kategorilerdeki ürünlerle doldurulur; Ayarlar ekranındaki "gerekçe ayarları" bu eşleşmeleri düzenler ve dağıtım veritabanındaki eşleşmeyi kullanır. Destek önceliği ve yerleşim etkisi şartname tablosuna sabittir.

**K-042 – Destek önceliği = en yüksek gerekçe.** Yüksek > Orta > Normal.

**K-043 – Çadır öneri algoritması.** Filtre: `Durum = Kullanilabilir` ve `Aktif`. Sıralama:
1. Yeterli boş yer (yerleşecek kişi sayısı ≤ boş yatak),
2. Yerleşim etkisi uyumu puanı (ailenin istediği her etki için çadır sağlıyorsa +1),
3. En az boş yatak israfı (boş yatak − kişi sayısı en küçük),
4. Eşitlikte bölge/sıra/numara.
Tek çadır yetmiyorsa aynı bölgede iki çadırlık en az israflı kombinasyon önerilir (aile iki çadıra yerleşebilir). Hiçbiri yoksa "bekleme listesine al" önerilir. Onay anında kapasite yeniden kontrol edilir ve RowVersion ile korunur.

**K-044 – Bekleme listesi önerisi.** Yer açılan çadır için, listedeki aktif aileler arasında aile kişi sayısı boş yere sığanlar; destek önceliği yüksek olan önce, eşitlikte `EklenmeZamani` en eski olan.

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

## E. Test

**K-070** – Saf iş kuralları (normalizasyon, gerekçe, öneri, FEFO, hak, tahmin, talep durum makinesi, yapay zeka cevap ayrıştırma) veritabanısız birim testlerle; servisler ayrı LocalDB test veritabanı `AfetCadirKent_Test` üzerinde entegrasyon testleriyle test edilir. Ollama testlerinde sahte `HttpMessageHandler` kullanılır; gerçek Ollama gerekmez.
