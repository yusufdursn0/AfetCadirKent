# CLAUDE.md — Afet Çadır Kent Yönetim Otomasyonu

> Bu dosya proje kök klasöründe durur ve Claude Code her oturumda otomatik olarak okur.
> Bu dosyadaki kurallar proje boyunca geçerlidir.

---

## 0. KAYNAKLAR VE ÖNCELİK SIRASI

Bu projenin iki bağlayıcı kaynağı vardır:

| Öncelik | Kaynak | Bu dosyadaki yeri | PDF hali (varsa) |
|---|---|---|---|
| 1 | **Hocanın Dönem Projesi Föyü** (ders şartnamesi) | Bölüm 2 | `docs/Donem_Projesi_Foyu.pdf` |
| 2 | **Proje Şartnamesi** (bu projenin onaylı tasarımı) | Bölüm 3 | `docs/Afet_Cadir_Kent_Proje_Son_Hali.pdf` |

- İki kaynak çelişirse **Föy kazanır.** Proje Şartnamesi, Föy'ü ihlal etmeden uygulanır.
- `docs/` klasöründe PDF'ler varsa onları da oku; bu dosyadaki özetle çelişki görürsen kullanıcıya bildir.
- Şartnamede **olmayan** bir özelliği "daha iyi olur" diye ekleme. Şartnamede **olan** bir özelliği atlama.

---

## 1. ROLÜN VE ÇALIŞMA BİÇİMİN

Sen bu projede yalnızca kod üreten bir araç değilsin. Kıdemli yazılım mimarı, geliştirici, test mühendisi, güvenlik mühendisi, teknik dokümantasyon uzmanı ve proje denetçisi olarak çalışırsın.

Döngün her aşamada şudur:

**ANLA → PLANLA → TASARLA → GELİŞTİR → TEST ET → HATALARI DÜZELT → TEKRAR KONTROL ET → ŞARTNAMEYLE KARŞILAŞTIR**

### 1.1 Analizden önce kod yazma
İlk oturumda kod yazmadan önce:
1. Bölüm 2 ve 3'ü (ve varsa `docs/` içindeki PDF'leri) tamamen analiz et.
2. `docs/REQUIREMENTS.md` içinde **gereksinim envanteri** oluştur (Bölüm 7.1).
3. Mevcut klasör yapısını incele (şu an sadece `.gitignore` var).
4. Belirsiz/çelişkili noktaları tespit et, kararlarını `docs/PROJECT_DECISIONS.md`'ye yaz.
5. `docs/ARCHITECTURE.md` ve `docs/PROJECT_STATUS.md`'yi oluştur.
6. Ardından Bölüm 5'teki aşama planıyla geliştirmeye başla.

### 1.2 Karar verme
- Klasör yapısı, sınıf/metot isimleri, kütüphane seçimi, kod ve test organizasyonu gibi **normal teknik kararları kendin ver**, onay isteme. Önemli olanları `PROJECT_DECISIONS.md`'ye yaz.
- Sadece **projenin amacını veya şartnamedeki bir gereksinimi değiştirecek** kritik kararlarda dur ve kullanıcıya sor.
- Şartnamede olmayan kritik bir bilgiyi gerçekmiş gibi uydurma. Standart bir varsayımla ilerleyebiliyorsan varsayımı belgeleyip ilerle.

### 1.3 Her aşamada
1. Aşamanın gereksinimlerini (REQ ID'leriyle) belirle.
2. Etkilenecek dosyaları belirle.
3. Geliştir.
4. Tutarlılığı kontrol et (derleme, katmanlar arası uyum).
5. İlgili testleri çalıştır, başarısızları kök nedeniyle düzelt.
6. `REQUIREMENTS.md` ve `PROJECT_STATUS.md`'yi güncelle.
7. Git commit at (Bölüm 8).
8. Kendine sor: *"Bu aşamada şartnamenin istediği her şeyi gerçekten uyguladım mı?"* Cevap hayırsa sonraki aşamaya geçme.

### 1.4 "Tamamlandı" tanımı
**Çalışıyor + test edildi + gereksinimi karşılıyor + diğer modüllerle uyumlu = tamamlandı.**
Sadece kodun yazılmış olması tamamlandı anlamına gelmez.

---

## 2. FÖY GEREKSİNİMLERİ (BAĞLAYICI)

Kaynak: Nesne Tabanlı Programlama, 2. sınıf, Dönem Projesi Yönergesi. **Bireysel proje.**

### 2.1 Teknik zorunluluklar
| Konu | Şart |
|---|---|
| Geliştirme ortamı | Visual Studio 2026 ve **C#** |
| Uygulama türü | **Masaüstü – Windows Forms** |
| Arayüz | **DevExpress (zorunlu).** GridControl, form kontrolleri, rapor araçları kullanılacak. Standart WinForms kontrolleri tek başına yeterli sayılmaz. |
| Veritabanı | **Access HARİÇ.** (Seçilen: SQL Server LocalDB) |
| Veri erişimi | ADO.NET veya **Entity Framework Core (önerilen → seçilen)** |
| Sürüm kontrolü | **GitHub (zorunlu).** Düzenli ve anlamlı commit geçmişi; proje tek seferde yüklenmez. README zorunlu. |
| Gizli bilgiler | API anahtarı, bağlantı dizesi, parola **repoya yüklenmez**, `.gitignore` ile hariç tutulur. Kod içinde parola/anahtar olmaz. |
| Yapay zeka | **Zorunlu.** Seçenekler: bulut API, ML.NET, DevExpress AI, **yerel LLM (Ollama) → seçilen.** |

### 2.2 OOP beklentileri (kodda görünür olmalı)
- Sınıflar ve nesneler; anlamlı sınıf tasarımı
- Kapsülleme (property, erişim belirleyiciler, doğrulama)
- Kalıtım ve/veya çok biçimlilik
- Interface ve/veya soyut sınıflar
- Koleksiyonlar (List, Dictionary vb.) ve generic yapılar
- İstisna yönetimi (try/catch) ile hataların düzgün ele alınması
- Katmanlı yapı (arayüz — iş mantığı — veri erişimi ayrımı)

### 2.3 Asgari kapsam
- Veritabanı üzerinde ekle / listele / güncelle / sil (CRUD)
- Birden fazla ilişkili tablo içeren düşünülmüş veritabanı tasarımı
- DevExpress ile düzenli ve kullanışlı arayüz
- Girdi doğrulama ve kullanıcıya anlamlı hata/uyarı mesajları
- Raporlama / dışa aktarma
- Yapay zeka özelliği

### 2.4 Teslim edilecekler
- **GitHub reposu:** Çalışan kaynak kodun tamamı, anlamlı commit geçmişi, README (proje tanıtımı, kurulum/çalıştırma adımları, teknolojiler).
- **Yazılı rapor** (kullanıcı ayrıca yazacak; sen gerekli malzemeyi hazırlarsın): Kapak; özet ve amaç; teknolojiler; veritabanı tasarımı ve ER şeması; sınıf yapısı ve mimari (UML veya sınıf açıklamaları, hangi OOP kavramı nerede); ekran görüntüleri; karşılaşılan zorluklar ve çözümler; sonuç, repo bağlantısı ve kaynaklar.
- **Sunum / canlı demo:** Temel senaryoların canlı gösterimi; mimari, veritabanı ve OOP kararlarının kısa anlatımı; yapay zeka özelliğinin gösterimi.

### 2.5 Değerlendirme
İşlevsellik ve tamlık 25 · OOP ve kod kalitesi 20 · Veritabanı 15 · DevExpress arayüz 10 · GitHub 10 · Rapor 10 · Sunum 10 · Yapay zeka +10.

### 2.6 Akademik dürüstlük
- Commit geçmişi projenin süreç içinde geliştirildiğini yansıtmalı.
- Yapay zeka araçlarından (kod yardımı dahil) yararlanıldığı raporda belirtilmeli → Bu projede Claude Code kullanıldığını `docs/PROJECT_DECISIONS.md`'de ve README'de not et.

---

## 3. PROJE ŞARTNAMESİ

**Amaç:** Afet sonrası kurulan bir çadır kentte afetzedeleri, aileleri, çadırları, yardım malzemelerini, talepleri ve gönüllüleri tek bir masaüstü sistemden yönetmek.
**Ana akış:** Aile kaydı → Uygun çadır → İhtiyaç belirleme → Stoktan teslim → Takip
**Geliştirme önceliği:** Önce ana akış eksiksiz çalışır hale getirilir, sonra diğer modüllere geçilir.

### 3.1 Modüller

**M1. İlk kurulum ve giriş**
- Programın ilk açılışında hiç kullanıcı yoksa "İlk yönetici hesabını oluştur" ekranı çıkar. Kodda hiçbir parola bulunmaz.
- Parolalar hash'lenerek saklanır (BCrypt).
- Görme, değiştirme ve rapor alma ayrı izinlerdir.

**M2. Kayıt**
- Sistem her kişiye ve aileye benzersiz bir numara verir; kimliği olmayan kişi de kaydedilebilir.
- Mükerrer kontrol: TC aynıysa kayıt engellenir. TC yoksa ad-soyad aynı ve yaş farkı 2'den azsa olası eşleşmeler gösterilir. Karşılaştırmada Türkçe karakter farkları (ı/i, ş/s, ç/c, ğ/g, ö/o, ü/u) yok sayılır.
- Geliş, ayrılış, nakil ve çadır değişiklikleri geçmişiyle tutulur.
- Aileye sonradan gelen yakın eklenebilir; bir aile iki çadıra yerleşebilir.
- **Aileden ayırma:** Bir veya birkaç kişi mevcut aileden ayrılıp yeni aile oluşturabilir. Yeni aile kendi numarasını alır, eski aileyle bağlantısı geçmişte kalır, gerekçeler ve haklar iki aile için yeniden hesaplanır.
- **Aile fişi:** Kayıt bitince aile numarası, çadır bilgisi ve kiosk giriş bilgisini içeren küçük bir fiş yazdırılır (XtraReports).
- Refakatsiz çocuk kaydını sadece Koordinatör görür.
- Fotoğraf isteğe bağlıdır; kamerayla çekilir, 300×300'e küçültülüp veritabanında `varbinary` olarak saklanır.
- **Hastanede durumu:** Kişi hastaneye sevk edildiğinde çadırdaki yeri korunur, dağıtım hakları ailesinde kalır. Döndüğünde durumu "Burada" yapılır.

**M3. Gerekçeli öncelik (iki ayrı bilgi)**
Sistem ailenin özel durumlarından **gerekçeler** çıkarır. Her gerekçe iki bilgiyi besler:
- **A) Ailenin destek önceliği:** Yerleşim ve görevli takibi için tek seviye (Yüksek / Orta / Normal). Ailenin en yüksek gerekçesine göre belirlenir.
- **B) Malzemeye özel ihtiyaç:** Ürün bazında hangi malzemede öncelikli olduğu. Dağıtımda bu kullanılır.

| Gerekçe | Destek önceliği | Yerleşim etkisi | Öncelikli malzemeler |
|---|---|---|---|
| 0–2 yaş bebek | Yüksek | Sıcak çadır | Mama, bez, battaniye |
| Hamile | Yüksek | Sağlık noktasına yakın | Gıda, hijyen |
| Tekerlekli sandalye | Yüksek | Erişilebilir çadır | — |
| Kronik hasta | Yüksek | Sağlık noktasına yakın | — |
| Refakatsiz çocuk | Yüksek | Koordinatör takibi | Gıda, giyim |
| 65 yaş üstü | Orta | Erişilebilir çadır | Battaniye |
| Tek ebeveyn | Orta | Görevli takibi | Hijyen |
| 5 kişiden kalabalık | Orta | Büyük çadır | Gıda |
| 3 gündür yardım almadı | Orta | — | Tüm ürünler |
| Özel durumu yok | Normal | — | — |

- Ekranda iki ayrı alan olarak gösterilir: "Destek önceliği: Yüksek (gerekçeler…)" ve "Malzeme ihtiyaçları: Mama ✔ · Bez ✔ …".
- Görevli gerekçe ekleyip çıkarabilir ama açıklama yazmak zorundadır; değişiklik kaydedilir.
- Ekranlarda "Acil" kelimesi kullanılmaz; etiket "Yardım önceliği"dir ve tıbbi değerlendirme değildir.

**M4. Çadır**
- Bölge, sıra, numara; kapasite ve doluluk; durum; erişilebilirlik ve sağlık noktasına yakınlık tutulur.
- Öneri sırası: kullanılabilir olması → yeterli yer → yerleşim etkisi (erişilebilirlik, sağlık noktasına yakınlık) → en az boş yatak israfı.
- Sistem önerir, görevli onaylar. Kapasite asla aşılmaz. Yer yoksa aile bekleme listesine alınır.
- **Bekleme listesi:** Bir çadırda yer açılınca sistem listede o çadıra uyan aileyi önerir (önce destek önceliği yüksek olan, eşitlikte en uzun bekleyen). Görevli onaylayınca aile yerleşir ve listeden çıkar.
- Renkli, tıklanabilir çadır planı (TileControl).

**M5. Depo**
- Stok, parti ve son kullanma tarihine (SKT) göre tutulur; SKT'si en yakın uygun parti önce önerilir.
- Birimler: adet, paket, kg, litre.
- Hareket türleri: giriş, dağıtım, iade, hasar, sayım farkı.
- Dağıtıma ayrılmış ama teslim edilmemiş miktar ayrıca görünür.
- **Ayırma işleyişi:** Dağıtım listesi onaylanınca miktar partiden "ayrıldı" olarak işaretlenir ve kullanılabilir stoktan düşer. Teslim edilince dağıtım hareketine dönüşür. Teslim iptal edilir ya da gün sonunda teslim edilmemişse ayırma kaldırılır, miktar kullanılabilir stoğa döner.
- SKT'si geçmiş parti dağıtıma çıkamaz, dağıtım ekranında seçilemez ve "hasar/fire" hareketiyle stoktan düşülür.

**M6. Dağıtım**

| Ürün | Hak | Tekrar alma | Kritik seviye |
|---|---|---|---|
| Su | Kişi başı 3 lt | 1 gün | 500 lt |
| Ekmek | Kişi başı 1 | 1 gün | 200 |
| Bebek maması | Bebek başı 1 kutu | 3 gün | 30 |
| Bebek bezi | Bebek başı 1 paket | 3 gün | 30 |
| Hijyen paketi | Aile başı 1 | 7 gün | 50 |
| Battaniye | Kişi başı 1 | 30 gün | 100 |

- Değerler ürün ayarlarından değiştirilebilir (hesaplama türü: kişi başı / aile başı / bebek başı; tekrar alma süresi; kritik seviye).
- Dağıtım listesi, o ürün için malzemeye özel ihtiyacı olan aileler önde olacak şekilde sıralanır.
- Gerekçe yazılarak istisna yapılabilir.
- Kısmi teslimatta kalan miktar bekleyen hak olarak kalır.
- Teslim eden, teslim alan, miktar ve zaman kaydedilir.

**M7. Stok tahmini**
- Yeterlilik süresi = kullanılabilir stok ÷ günlük ortalama tüketim. Kişi başı tüketime göre ikinci bir tahmin yapılır.
- Hesabın dayandığı dönem gösterilir; veri yoksa "hesaplanamıyor" yazar.
- SKT'si yaklaşan ürünler için de uyarı verilir.

**M8. Yakın bulma**
- Aramayı sadece yetkili görevli yapar. Sonuçlar yaş ve memleketle daraltılır; Türkçe karakter farkları yok sayılır.
- Başvuru durumları: Açık → Olası eşleşme → Doğrulandı → Kapandı.
- Fotoğraf tek başına doğrulama sayılmaz; çadır ve iletişim bilgisi otomatik paylaşılmaz.

**M9. Talep ve görev takibi**
- Talepler kayıt masasından, görevliden veya kiosktan gelir.
- Durumlar: Yeni → Atandı → İşlemde → Tamamlandı / İptal.
- Yönlendirme: Sağlık → sağlık personeli, Malzeme → depo, Çadır/arıza → saha görevlisi, Diğer → koordinatör.
- Her talepte: aile/çadır, açıklama, tür, sorumlu görevli, durum, açılış ve kapanış zamanı, sonuç.
- **Sonuç zorunludur:** Talep "Tamamlandı" veya "İptal" yapılırken sonuç yazılmadan kapatılamaz.

**M10. Yapay zeka (Ollama, yerel)**
- **Talep sınıflandırma:** Tür, yardım önceliği ve gerekçe önerir; cevap JSON formatında döner (Ollama JSON çıktı modu).
- **Tıbbi ayrım:** Yapay zeka ve sistem tıbbi aciliyet belirlemez. Sağlık talepleri öncelik verilmeden doğrudan sağlık personeline yönlendirilir ve "Sağlık personeli değerlendirecek" olarak işaretlenir.
- **Günlük özet:** Koordinatör için durum raporu yazar.
- Ollama çalışmıyorsa veya cevap okunamazsa talep "Sınıflandırılmamış" kalır, görevli elle devam eder. Son karar her zaman görevlidedir.
- Program Ollama'ya `HttpClient` ile `http://localhost:11434` üzerinden bağlanır. Adres ve model adı ayar dosyasındadır. İnternet gerekmez; veriler bilgisayar dışına çıkmaz.
- Model: Türkçe'yi iyi anlayan orta boyutlu bir model (örn. `qwen2.5:7b` veya `gemma3:4b`); denenerek seçilir.
- Yapay zeka servisi bir **interface** ile tasarlanır (farklı servise geçiş tek sınıf eklemekle yapılabilmeli).

**M11. Kiosk**
- Tam ekran moddur; giriş aile numarası ve doğum yılıyla yapılır.
- Talep bırakılır, talep durumu ve duyurular görülür.
- Duyuruları Koordinatör Ayarlar ekranından ekler (başlık, metin, başlangıç ve bitiş tarihi). Süresi dolan duyuru kioskta görünmez.

**M12. Gönüllüler**
- Kayıt, uzmanlık alanı, vardiya planı ve görev ataması.
- Gönüllüleri Koordinatör yönetir; gönüllüler programa giriş yapmaz.

**M13. Ana ekran ve raporlar**
- Ana ekran: toplam kişi, boş yatak, bekleme listesi, kritik stoklar, SKT'si yaklaşanlar, açık talepler, yapay zeka özeti.
- Raporlar: dağıtım tutanağı, stok, nüfus ve talep raporları (PDF/Excel).

**M14. Teknik altyapı**
- İşlem geçmişi: kim oluşturdu, değiştirdi, iptal etti.
- `RowVersion` ile aynı çadır veya stok aynı anda iki kişiye verilemez (iyimser eşzamanlılık; çakışmada kullanıcıya anlaşılır mesaj).
- Program içinden yedek alma ve geri yükleme.
- Bağlantı kesilirse işlemin kaydedilip kaydedilmediği açıkça gösterilir.
- Girdi doğrulama, anlamlı hata mesajları, try/catch.

### 3.2 Ekle / Listele / Güncelle / Sil kuralları
- **Silinebilir:** Ürün, çadır, bölge, kullanıcı, gönüllü. Kayıt kullanılmışsa: "Bu kayıt kullanıldığı için silinemez, pasif yapabilirsiniz."
- **Pasif yapılır (silinmez):** Kişi, aile, dağıtım, stok hareketi. Geçmiş korunur.

### 3.3 Roller
| Rol | Yetki |
|---|---|
| Koordinatör | Her şey ve rapor; gönüllü yönetimi ve duyurular |
| Kayıt görevlisi | Kayıt ve çadır |
| Depo sorumlusu | Depo, dağıtım ve rapor |
| Sağlık personeli | Sağlık bilgileri ve sağlık talepleri |
| Saha görevlisi | Kendine atanan talepler |
| Kiosk | Kendi ailesi ve talep oluşturma |

### 3.4 Durumlar
- **Kişi:** Burada / Ayrıldı / Nakil / Hastanede
- **Çadır:** Kullanılabilir / Bakımda / Kapalı
- **Talep:** Yeni / Atandı / İşlemde / Tamamlandı / İptal
- **Yakın bulma başvurusu:** Açık / Olası eşleşme / Doğrulandı / Kapandı

### 3.5 Veritabanı (yaklaşık 25 tablo)
Kullanıcılar, Roller, Yetkiler, Aileler, Kişiler, KişiHareketleri, AileGeçmişi, Gerekçeler, GerekçeÜrünleri, AileGerekçeleri, Bölgeler, Çadırlar, Yerleşimler, BeklemeListesi, Ürünler, Partiler, StokHareketleri, Dağıtımlar, DağıtımDetay, Talepler, YakınBulmaBaşvuruları, Gönüllüler, İşlemGeçmişi, Bağışçılar, Duyurular, Vardiyalar.
Her entity için alanlar, tipler, PK/FK, unique alanlar, indeksler, doğrulama ve cascade davranışı tasarlanır. Kod ve tablo isimlerinde Türkçe karakter kullanma (örn. `Cadirlar`, `KisiHareketleri`); ekranda Türkçe karakterli göster.

### 3.6 OOP eşlemesi (rapor ve sunum için korunmalı)
- **Sınıflar:** Kisi, Aile, Cadir, Urun, Parti, Dagitim, Talep …
- **Kalıtım:** Kisi → Afetzede, Gorevli, Gonullu
- **Soyut sınıf:** Kisi, StokHareketi
- **Interface ve polimorfizm:** Gerekçe kuralları (her kural kendi destek önceliğini, yerleşim etkisini ve öncelikli malzemelerini döndürür), yapay zeka servisi, generic Repository
- **Kapsülleme:** Property'ler ve doğrulama
- **Koleksiyonlar ve generic:** List, Dictionary, generic Repository

### 3.7 DevExpress
- Arayüzde **sadece DevExpress kontrolleri:** XtraForm, TextEdit, SimpleButton, LookUpEdit, DateEdit, GridControl …
- Ayrıca: RibbonControl, LayoutControl, TileControl (çadır planı ve kiosk), ChartControl, GaugeControl, SchedulerControl, AlertControl, XtraReports, CameraControl ve PictureEdit (fotoğraf), XtraTabControl (sekmeli bölümler).

### 3.8 Ekranlar (18 form)
| # | Form | İçerik |
|---|---|---|
| 1 | İlk kurulum | İlk açılışta yönetici hesabı oluşturma |
| 2 | Giriş | Kullanıcı adı ve parolayla giriş |
| 3 | Ana ekran | Ribbon menü ve özet göstergeler |
| 4 | Aile ve kişi kayıt | Aile, kişi, gerekçeler, fotoğraf, aileden ayırma, aile fişi |
| 5 | Kişi listesi ve arama | Filtreleme, durum değiştirme |
| 6 | Çadır planı | Renkli tıklanabilir plan; öneri ve onay |
| 7 | Bekleme listesi | Yer bekleyen aileler, yer açılınca öneri |
| 8 | Ürün tanımları | Ürün, birim, hak, tekrar alma süresi, kritik seviye |
| 9 | Stok girişi | Parti, miktar, SKT, bağışçı (bağışçı kaydı da buradan) |
| 10 | Stok durumu | Partiler, hareketler, tahmin ve uyarılar |
| 11 | Dağıtım | Liste, teslim, kısmi teslimat |
| 12 | Talepler | Giriş, yapay zeka sınıflandırma, atama, takip, sonuç |
| 13 | Yakın bulma | Arama ve başvurular |
| 14 | Gönüllüler | Kayıt, vardiya takvimi, görev ataması |
| 15 | Kiosk | Tam ekran afetzede ekranı |
| 16 | Raporlar | Dağıtım tutanağı, stok, nüfus, talep raporları |
| 17 | Ayarlar ve kullanıcılar | Kullanıcılar, roller, gerekçe ayarları, yedekleme, duyurular |
| 18 | İşlem geçmişi | Kim, ne zaman, neyi değiştirdi |

### 3.9 Demo verisi ve senaryo
- **Veri (seed):** 3 bölge, 30 çadır, 20 aile (bebekli, engelli, yaşlı olanlar dahil), 10 ürün, SKT'si yaklaşmış bir parti. **Demo verisi kullanıcı içermez**; ilk açılışta yönetici oluşturulur, diğer rolleri yönetici ekler.
- **Senaryo:** (1) Bebekli ve hamile bir aile kaydedilir; destek önceliği ve malzeme ihtiyaçları ayrı görünür. (2) Sistem çadır önerir, görevli onaylar. (3) Mama dağıtımında aile öne çıkar, kısmi teslimat yapılır. (4) "Çadır su alıyor" talebi girilir, yapay zeka sınıflandırır, sonuç yazılarak kapatılır. (5) "Çocuk ateşleniyor" talebi doğrudan sağlık personeline gider. (6) Bir kişi aileden ayrılıp yeni aile olur. (7) Stok uyarısı ve günlük özet gösterilir.

---

## 4. TEKNİK MİMARİ (BAŞLANGIÇ KARARLARI)

Bu kararlar başlangıç noktasıdır; değiştirmen gerekirse gerekçesiyle `PROJECT_DECISIONS.md`'ye yaz.

### 4.1 Solution yapısı
```
AfetCadirKent.sln
├── src/
│   ├── AfetCadirKent.Entities     → Entity sınıfları, enum'lar (bağımlılığı yok)
│   ├── AfetCadirKent.DataAccess   → EF Core DbContext, konfigürasyonlar, migration'lar, seed, generic Repository
│   ├── AfetCadirKent.Business     → İş kuralları, servisler, gerekçe kuralları, yapay zeka servisi
│   └── AfetCadirKent.UI           → DevExpress WinForms formları (sadece arayüz; iş kuralı içermez)
├── tests/
│   └── AfetCadirKent.Tests        → xUnit testleri (Business ve DataAccess)
└── docs/                          → Dokümantasyon ve PDF'ler
```
Bağımlılık yönü: UI → Business → DataAccess → Entities. UI doğrudan DbContext kullanmaz.

### 4.2 Platform
- Hedef: Visual Studio 2026 ile gelen güncel **LTS .NET** sürümü, `-windows` hedefiyle (UI projesi). `dotnet --list-sdks` ile kurulu SDK'yı kontrol et. DevExpress'in desteklediği en yeni sürümü seç.
- EF Core: SQL Server sağlayıcısı, **LocalDB** (`(localdb)\MSSQLLocalDB`), migration'lar ve seed verisi.
- Parola hash: BCrypt (örn. `BCrypt.Net-Next`).
- Loglama: Beklenmeyen hatalar dosyaya loglanır (`logs/`, `.gitignore`'da); kullanıcıya teknik olmayan mesaj gösterilir.

### 4.3 Yapılandırma ve gizli bilgiler
- `appsettings.json` (bağlantı dizesi, Ollama adresi, model adı) **repoya yüklenmez**, `.gitignore`'a eklenir.
- Repoda **`appsettings.example.json`** bulunur (gerçek değer yok, açıklayıcı örnek).
- Program `appsettings.json` bulamazsa anlaşılır bir mesajla kurulum adımını gösterir.
- Kodda hiçbir parola, anahtar veya bağlantı dizesi bulunmaz.

### 4.4 HTTP API yok
Bu bir masaüstü uygulamasıdır; ayrı bir backend/HTTP API **yoktur**. UI, Business servislerini doğrudan çağırır. Tek dış entegrasyon yerel Ollama'dır. Bu nedenle `API.md` oluşturulmaz; servis sözleşmeleri ve Ollama entegrasyonu `ARCHITECTURE.md`'de anlatılır.

### 4.5 Masaüstü karşılıkları (web kavramları yerine)
| Genel kavram | Bu projedeki karşılığı |
|---|---|
| Loading state | Uzun işlemlerde bekleme göstergesi (DevExpress SplashScreenManager/WaitForm), arayüz donmaz (async) |
| Empty state | Boş gridlerde açıklayıcı metin |
| Error state | XtraMessageBox ile anlaşılır mesaj; teknik ayrıntı sadece logda |
| Success state | Kısa onay mesajı veya AlertControl bildirimi |
| Responsive | Formlar yeniden boyutlandırılabilir, LayoutControl ile düzgün yerleşim |
| Protected routes | Rol/izin kontrolü: yetkisiz ekran ve butonlar gizlenir **ve** Business katmanında da kontrol edilir |

### 4.6 Form tasarımı ve Designer dosyaları
- Formlar Visual Studio tasarımcısında açılabilir kalmalıdır: `InitializeComponent()` desenine uy, `.Designer.cs` içine iş mantığı koyma.
- Designer dosyalarını elle düzenlerken tasarımcının üretebileceği biçimde yaz; tasarımcıyı bozacak kod (lambda, döngü, koşul) ekleme.

---

## 5. AŞAMA PLANI

> ⚠️ **DevExpress henüz kurulu değil.** 30 günlük deneme süresi kurulumla başlar. Bu yüzden **arayüz gerektirmeyen aşamalar (0–4) önce** yapılır. Aşama 4 bitince **dur** ve kullanıcıdan DevExpress'i kurmasını iste.

| Aşama | İçerik | Çıktı |
|---|---|---|
| **0** | Şartname analizi, gereksinim envanteri, belirsizlikler, mimari | `docs/REQUIREMENTS.md`, `PROJECT_DECISIONS.md`, `ARCHITECTURE.md`, `PROJECT_STATUS.md` |
| **1** | Solution iskeleti, projeler, referanslar, `.gitignore` kontrolü, `appsettings.example.json`, README taslağı | Derlenen boş solution |
| **2** | Entity'ler, DbContext, konfigürasyonlar, ilk migration, seed verisi | LocalDB'de oluşan veritabanı |
| **3** | Business servisleri ve kurallar: kayıt ve mükerrer kontrol, Türkçe normalizasyon, aileden ayırma, gerekçe kuralları, çadır önerisi, bekleme listesi, parti/FEFO, ayırma işleyişi, dağıtım hakları ve kısmi teslimat, stok tahmini, talep iş akışı, yetki kontrolü, işlem geçmişi | Birim testleri geçen iş katmanı |
| **4** | Ollama servisi (interface + uygulama), JSON ayrıştırma, zaman aşımı ve hata durumunda "Sınıflandırılmamış" | Testleri geçen yapay zeka servisi |
| ⏸ | **DUR:** Kullanıcıdan DevExpress kurulumunu ve Ollama modelini indirmesini iste | — |
| **5** | UI temeli: ilk kurulum, giriş, ana ekran (Ribbon), rol bazlı menü | Giriş yapılabilen uygulama |
| **6** | Ana akış formları: 4, 5, 6, 7, 8, 9, 10, 11, 12 | Uçtan uca ana akış |
| **7** | Diğer formlar: 13–18, kiosk, raporlar (XtraReports), aile fişi | Tüm ekranlar |
| **8** | Teknik altyapı: RowVersion çakışma yönetimi, yedek/geri yükleme, bağlantı kesintisi davranışı | Dayanıklı uygulama |
| **9** | Test, kabul testi (Bölüm 9), hata düzeltme | Tüm testler geçer |
| **10** | Dokümantasyon tamamlama, kod denetimi, `FINAL_AUDIT.md` | Teslime hazır proje |

---

## 6. KESİN KURALLAR

### 6.1 Yasaklar
- ❌ Gerçek implementasyon yerine **TODO, FIXME, "sonra eklenecek", boş metot, sahte veri ile bırakılmış özellik, çalışmayan buton, sadece görsel özellik.** Şartnamedeki her özellik gerçekten çalışmalı. Dış bağımlılık yüzünden yapılamayan bir şey varsa açıkça belgele.
- ❌ Kodda parola, anahtar, bağlantı dizesi. `appsettings.json`'ı commit etme.
- ❌ Arayüzde standart WinForms kontrolü (DevExpress karşılığı varken).
- ❌ UI katmanında iş kuralı veya doğrudan DbContext kullanımı.
- ❌ Şartnamede olmayan özellik eklemek; şartnamedeki bir özelliği atlamak.
- ❌ Yapay zekanın tıbbi aciliyet belirlemesi.
- ❌ Pasif yapılması gereken kayıtları (kişi, aile, dağıtım, stok hareketi) fiziksel olarak silmek.
- ❌ Hata mesajını susturmak için geçici çözüm; testleri geçirmek için testi bozmak.
- ❌ Olmayan problemleri raporda varmış gibi göstermek.

### 6.2 Kullanıcıya sormadan YAPMA
- `git push --force`, `git reset --hard`, geçmişi yeniden yazan git komutları
- Veritabanını silmek veya sıfırlamak (geliştirme veritabanı dahil)
- Proje klasörü dışındaki dosyalara dokunmak
- Sistem ayarı değiştirmek veya bilgisayara program kurmak (DevExpress, Ollama modeli vb. kurulumları kullanıcı yapar)

### 6.3 Kod kalitesi
Çalışabilir, okunabilir, modüler, güvenli, test edilebilir, tekrarsız ve mimariyle uyumlu kod. Gereksiz soyutlama yapma; kısa kod için kaliteden de ödün verme. Async kullanımında UI thread'ini dondurma.

### 6.4 Bağlam ve kullanım verimliliği
Kullanıcının kullanım limiti var. Aynı dosyaları gereksiz yere tekrar okuma; büyük dosyalarda sadece gereken bölümü incele; alınmış kararları tekrar üretme; uzun açıklama yerine işi yap.

---

## 7. DOKÜMANTASYON VE DURUM TAKİBİ

### 7.1 `docs/REQUIREMENTS.md` — gereksinim envanteri
Her gereksinim için: **ID** (REQ-001…), gereksinim, kaynak (Föy bölümü veya Şartname modülü), öncelik, bağımlılıklar, durum, test yöntemi.
Durum değerleri: `IMPLEMENTED` · `PARTIALLY IMPLEMENTED` · `NOT IMPLEMENTED` · `BLOCKED`.
Bu liste projenin ana kontrol listesidir; her aşamada güncellenir.

### 7.2 Diğer dosyalar
| Dosya | İçerik |
|---|---|
| `README.md` | Proje tanıtımı, ekran görüntüleri (sonradan), teknolojiler, kurulum ve çalıştırma adımları (LocalDB, `appsettings.json` oluşturma, migration, Ollama kurulumu ve `ollama pull <model>`, DevExpress gereksinimi), ilk açılışta yönetici oluşturma, yapay zeka kullanım beyanı |
| `docs/SETUP.md` | Ayrıntılı kurulum |
| `docs/ARCHITECTURE.md` | Katmanlar, sınıf yapısı, OOP kavramlarının nerede kullanıldığı, servis sözleşmeleri, Ollama entegrasyonu, ER şeması açıklaması |
| `docs/PROJECT_DECISIONS.md` | Alınan önemli kararlar ve gerekçeleri, belirsizliklerin çözümü |
| `docs/TESTING.md` | Test stratejisi, nasıl çalıştırılır, kapsam |
| `docs/PROJECT_STATUS.md` | Tamamlanan / devam eden / bekleyen işler, bilinen problemler, test durumu, kalan gereksinimler |
| `docs/SUNUM_NOTLARI.md` | Kullanıcının sunumda anlatacağı teknik özet ve hocanın sorabileceği sorulara sade cevaplar (mimari, veritabanı, OOP kararları, yapay zeka) |
| `docs/FINAL_AUDIT.md` | Bölüm 10'daki final denetim raporu |

Dokümantasyon gerçek kodla aynı olmalı; kod değişince ilgili doküman da güncellenir.

---

## 8. GIT KURALLARI

- Repo: `https://github.com/yusufdursn0/AfetCadirKent` (dal: `main`).
- Her tamamlanan anlamlı adımdan sonra **küçük ve anlamlı Türkçe commit** at (örn. "Çadır öneri kuralı ve testleri eklendi"). Büyük toplu commit atma.
- Her aşamanın sonunda `git push` yap.
- Commit öncesi kontrol: `appsettings.json`, `logs/`, `bin/`, `obj/`, `.vs/` ve kişisel dosyalar commit'e girmemeli.

---

## 9. TEST VE KABUL

### 9.1 Otomatik testler (xUnit)
En az şu kuralların birim testleri olmalı: mükerrer kayıt ve Türkçe normalizasyon, gerekçe → destek önceliği ve malzeme ihtiyacı, çadır öneri sırası ve kapasite aşılmaması, bekleme listesi sıralaması, FEFO parti seçimi ve SKT'si geçmiş partinin dışlanması, ayırma/teslim/iptal stok hesapları, hak hesabı (kişi/aile/bebek başı), tekrar alma süresi ve istisna, kısmi teslimat, stok tahmini ve "hesaplanamıyor" durumu, talep durum geçişleri ve zorunlu sonuç, sağlık talebinin öncelik almaması, yapay zeka cevabı okunamazsa "Sınıflandırılmamış", yetki kontrolleri, silme/pasife alma kuralları.
Veritabanı testleri için ayrı bir LocalDB test veritabanı kullan; geliştirme veritabanına dokunma.

### 9.2 Kabul testi (final)
Gerçek kullanıcı gibi: uygulamayı başlat → ilk yöneticiyi oluştur → giriş yap → diğer rolleri oluştur → Bölüm 3.9'daki 7 adımlı senaryoyu uygula → hatalı girişleri dene (boş alan, hatalı TC, dolu çadır, yetersiz stok, SKT'si geçmiş parti) → yetkisiz işlemleri dene (her rol için) → verilerin doğru kaydedildiğini kontrol et → Ollama kapalıyken talep gir → raporları al → çıkış yap.

---

## 10. FİNAL DENETİM

"Bitti" demeden önce:
1. Föy'ü ve Şartname'yi baştan sona tekrar kontrol et; `REQUIREMENTS.md` ile karşılaştır.
2. Kodun tamamını denetle: kullanılmayan kod, tekrar, hatalı referans, eksik doğrulama, güvenlik açığı, gizli bilgi, null hataları, kenar durumlar, eşzamanlılık, kullanıcı deneyimi sorunları. Gerçek problemleri düzelt.
3. `docs/FINAL_AUDIT.md` oluştur:
   - **REQUIREMENTS:** toplam / tamamlanan / kısmen / tamamlanmayan
   - **TESTS:** toplam / başarılı / başarısız
   - **SECURITY:** bulunan / düzeltilen problemler
   - **PERFORMANCE:** kontroller ve sonuç
   - **DOCUMENTATION:** durum
   - **REMAINING ISSUES**
   - **FINAL VERDICT:** `READY FOR DELIVERY` veya `NOT READY FOR DELIVERY` (kritik gereksinimlerin tamamı karşılanmadıkça READY denemez)

---

## 11. KULLANICIYLA İLETİŞİM

- Kullanıcıyla **Türkçe** konuş. Kullanıcı kod yazmıyor; açıklamaları sade tut.
- Her aşama sonunda kısa bir özet ver: **ne yapıldı, nasıl denenir (hangi ekran/komut), sırada ne var.**
- Kullanıcının yapması gereken bir şey varsa (DevExpress kurulumu, Ollama modeli indirme, Visual Studio'da bir ayar) adım adım ve net söyle, sonra bekle.
- Uzun teknik açıklamaları dokümana yaz; sohbette kısa tut.

---

## 12. İLK OTURUMDA YAPILACAKLAR

1. Bu dosyayı ve varsa `docs/` içindeki PDF'leri tamamen oku.
2. Kapsamı çıkar, gereksinim envanterini oluştur (`docs/REQUIREMENTS.md`).
3. Mevcut proje yapısını incele.
4. Eksik/çelişkili noktaları belirle, kararlarını `PROJECT_DECISIONS.md`'ye yaz.
5. Teknik mimariyi `ARCHITECTURE.md`'ye yaz.
6. `PROJECT_STATUS.md`'yi oluştur.
7. Kullanıcıya kısa bir özet ver, ardından Aşama 1'den başlayarak geliştirmeye geç.
