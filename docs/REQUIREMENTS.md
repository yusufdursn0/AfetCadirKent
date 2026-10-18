# Gereksinim Envanteri

Projenin ana kontrol listesi. Her aşamada güncellenir.

**Kaynak kısaltmaları:** `F` = Dönem Projesi Föyü (bölüm no), `Ş` = Proje Şartnamesi (modül no / bölüm adı), `C` = CLAUDE.md
**Öncelik:** `Y` yüksek (ana akış veya Föy zorunluluğu) · `O` orta · `D` düşük
**Durum:** `IMPLEMENTED` · `PARTIALLY IMPLEMENTED` · `NOT IMPLEMENTED` · `BLOCKED`
**Test:** `BT` birim testi · `VT` veritabanı (entegrasyon) testi · `MT` elle test (UI) · `KT` kabul testi · `DK` doküman kontrolü

---

## 1. Föy (ders şartnamesi) gereksinimleri

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-001 | C#, Visual Studio 2026, Windows Forms masaüstü uygulaması | F 2.1 | Y | — | NOT IMPLEMENTED | MT |
| REQ-002 | Arayüzde DevExpress kontrolleri (GridControl, form kontrolleri, rapor araçları); DevExpress karşılığı olan yerde standart WinForms kontrolü yok | F 2.2, Ş DevExpress | Y | DevExpress kurulumu | NOT IMPLEMENTED | MT, DK |
| REQ-003 | Veritabanı: SQL Server LocalDB (Access yok) | F 2.3 | Y | — | IMPLEMENTED | VT |
| REQ-004 | Veri erişimi EF Core; veritabanı migration ile tek komutla kurulur, demo verisi otomatik yüklenir | F 2.3, Ş GitHub | Y | REQ-003 | PARTIALLY IMPLEMENTED | VT, KT |
| REQ-005 | GitHub'da düzenli, küçük, anlamlı Türkçe commit geçmişi | F 2.4, F 8 | Y | — | PARTIALLY IMPLEMENTED | DK |
| REQ-006 | README: tanıtım, kurulum/çalıştırma, teknolojiler ve sürümleri, Ollama kurulumu ve `ollama pull`, ilk yönetici oluşturma, rol ekleme adımları | F 2.4, F 6.1, Ş GitHub | Y | — | PARTIALLY IMPLEMENTED | DK |
| REQ-007 | Gizli bilgi (bağlantı dizesi, parola, anahtar) repoda ve kodda yok; `appsettings.json` .gitignore'da, `appsettings.example.json` repoda | F 2.4, F 8 | Y | — | PARTIALLY IMPLEMENTED | DK |
| REQ-008 | Yapay zeka entegrasyonu (yerel LLM – Ollama) | F 2.5 | Y | M10 | NOT IMPLEMENTED | BT, KT |
| REQ-009 | OOP: anlamlı sınıf tasarımı, sınıflar ve nesneler | F 3 | Y | — | PARTIALLY IMPLEMENTED | DK |
| REQ-010 | OOP: kapsülleme (property, erişim belirleyici, doğrulama) | F 3, Ş OOP | Y | — | PARTIALLY IMPLEMENTED | BT, DK |
| REQ-011 | OOP: kalıtım ve çok biçimlilik | F 3, Ş OOP | Y | REQ-230, REQ-231 | PARTIALLY IMPLEMENTED | BT, DK |
| REQ-012 | OOP: interface ve soyut sınıf | F 3, Ş OOP | Y | REQ-060, REQ-130, REQ-232 | PARTIALLY IMPLEMENTED | DK |
| REQ-013 | OOP: koleksiyonlar (List, Dictionary) ve generic yapılar | F 3, Ş OOP | Y | REQ-232 | PARTIALLY IMPLEMENTED | DK |
| REQ-014 | İstisna yönetimi (try/catch), anlaşılır mesaj, teknik ayrıntı logda | F 3, Ş M14 | Y | — | PARTIALLY IMPLEMENTED | BT, MT |
| REQ-015 | Katmanlı yapı: UI → Business → DataAccess → Entities; UI DbContext kullanmaz | F 3, C 4.1 | Y | — | PARTIALLY IMPLEMENTED | DK |
| REQ-016 | Veritabanı üzerinde ekle / listele / güncelle / sil | F 5 | Y | REQ-180, REQ-181 | PARTIALLY IMPLEMENTED | VT, MT |
| REQ-017 | Birden fazla ilişkili tablo içeren düşünülmüş veritabanı tasarımı | F 5 | Y | REQ-190 | IMPLEMENTED | VT, DK |
| REQ-018 | Girdi doğrulama ve anlamlı hata/uyarı mesajları | F 5, Ş M14 | Y | — | PARTIALLY IMPLEMENTED | BT, MT |
| REQ-019 | Raporlama / dışa aktarma (PDF/Excel) | F 5, Ş M13 | Y | REQ-161 | NOT IMPLEMENTED | MT |
| REQ-020 | Yazılı rapor için malzeme: ER şeması, sınıf yapısı/UML, OOP eşlemesi, zorluklar ve çözümler | F 6.2 | O | — | NOT IMPLEMENTED | DK |
| REQ-021 | Sunum notları (mimari, veritabanı, OOP, yapay zeka; olası sorular) | F 6.3 | O | — | NOT IMPLEMENTED | DK |
| REQ-022 | Yapay zeka araçlarının (Claude Code, Ollama) kullanıldığı README ve PROJECT_DECISIONS'ta beyan edilir | F 8 | Y | — | PARTIALLY IMPLEMENTED | DK |

## 2. M1 – İlk kurulum, giriş ve yetki

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-030 | Hiç kullanıcı yoksa ilk açılışta "İlk yönetici hesabını oluştur" ekranı | Ş M1 | Y | — | PARTIALLY IMPLEMENTED | VT, KT |
| REQ-031 | Kodda hiçbir parola bulunmaz; seed kullanıcı içermez | Ş M1, Ş Demo | Y | — | IMPLEMENTED | DK |
| REQ-032 | Parolalar BCrypt ile hash'lenerek saklanır | Ş M1 | Y | — | IMPLEMENTED | BT |
| REQ-033 | Kullanıcı adı ve parolayla giriş; pasif kullanıcı giremez | Ş Ekran 2 | Y | REQ-032 | IMPLEMENTED | BT, KT |
| REQ-034 | Görme, değiştirme ve rapor alma ayrı izinlerdir (Yetkiler tablosu) | Ş M1 | Y | — | IMPLEMENTED | BT |
| REQ-035 | Altı rol ve yetki matrisi (Koordinatör, Kayıt görevlisi, Depo sorumlusu, Sağlık personeli, Saha görevlisi, Kiosk) | Ş Roller | Y | REQ-034 | IMPLEMENTED | BT |
| REQ-036 | Yetkisiz ekran/butonlar gizlenir **ve** Business katmanında kontrol edilir | C 4.5 | Y | REQ-035 | PARTIALLY IMPLEMENTED | BT, KT |

## 3. M2 – Kayıt

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-040 | Her kişiye ve aileye benzersiz numara | Ş M2 | Y | — | IMPLEMENTED | VT |
| REQ-041 | Kimliği (TC) olmayan kişi kaydedilebilir | Ş M2 | Y | — | IMPLEMENTED | BT |
| REQ-042 | TC doğrulama (11 hane + algoritma); aynı TC ile ikinci kayıt engellenir | Ş M2, C 9.2 | Y | — | IMPLEMENTED | BT, VT |
| REQ-043 | TC yoksa: ad-soyad aynı ve yaş farkı 2'den az ise olası eşleşmeler gösterilir | Ş M2 | Y | REQ-044 | IMPLEMENTED | BT |
| REQ-044 | Türkçe karakter farkları (ı/i, ş/s, ç/c, ğ/g, ö/o, ü/u) karşılaştırmada yok sayılır | Ş M2, M8 | Y | — | IMPLEMENTED | BT |
| REQ-045 | Geliş, ayrılış, nakil ve çadır değişiklikleri geçmişiyle tutulur (KisiHareketleri) | Ş M2 | Y | — | IMPLEMENTED | VT |
| REQ-046 | Aileye sonradan gelen yakın eklenebilir | Ş M2 | Y | — | IMPLEMENTED | VT |
| REQ-047 | Bir aile iki çadıra yerleşebilir | Ş M2 | O | REQ-073 | IMPLEMENTED | VT |
| REQ-048 | Aileden ayırma: seçilen kişiler yeni aile olur, yeni numara, AileGecmisi kaydı, gerekçe ve haklar iki aile için yeniden hesaplanır | Ş M2 | Y | REQ-060 | IMPLEMENTED | VT, KT |
| REQ-049 | Aile fişi (aile no, çadır, kiosk giriş bilgisi) XtraReports ile yazdırılır | Ş M2 | O | DevExpress | NOT IMPLEMENTED | MT |
| REQ-050 | Refakatsiz çocuk kaydını yalnızca Koordinatör görür | Ş M2 | Y | REQ-036 | IMPLEMENTED | BT |
| REQ-051 | İsteğe bağlı fotoğraf: kamerayla çekilir, 300×300'e küçültülür, `varbinary` saklanır | Ş M2 | O | DevExpress | PARTIALLY IMPLEMENTED | BT, MT |
| REQ-052 | Hastanede durumu: çadırdaki yer korunur, dağıtım hakkı ailede kalır, dönüşte "Burada" | Ş M2 | Y | — | IMPLEMENTED | BT |
| REQ-053 | Kişi durumları: Burada / Ayrıldı / Nakil / Hastanede | Ş Durumlar | Y | — | IMPLEMENTED | BT |

## 4. M3 – Gerekçeli öncelik

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-060 | 10 gerekçe kuralı interface + polimorfizmle; her kural destek önceliğini, yerleşim etkisini, öncelikli malzemelerini döndürür | Ş M3, Ş OOP | Y | — | IMPLEMENTED | BT |
| REQ-061 | Ailenin destek önceliği (Yüksek/Orta/Normal) en yüksek gerekçeye göre | Ş M3 | Y | REQ-060 | IMPLEMENTED | BT |
| REQ-062 | Malzemeye özel ihtiyaç ürün bazında hesaplanır | Ş M3 | Y | REQ-060 | IMPLEMENTED | BT |
| REQ-063 | Ekranda iki ayrı alan: "Destek önceliği: … (gerekçeler)" ve "Malzeme ihtiyaçları: Mama ✔ …" | Ş M3 | Y | DevExpress | PARTIALLY IMPLEMENTED | MT |
| REQ-064 | Görevli gerekçe ekleyip çıkarabilir; açıklama zorunlu; değişiklik kaydedilir | Ş M3 | Y | REQ-170 | IMPLEMENTED | BT |
| REQ-065 | "Acil" kelimesi hiçbir ekranda yok; etiket "Yardım önceliği"; tıbbi değerlendirme değildir | Ş M3 | Y | — | PARTIALLY IMPLEMENTED | DK, MT |
| REQ-066 | Gerekçe ayarları (Ayarlar ekranı): gerekçe–ürün eşleşmeleri düzenlenebilir | Ş Ekran 17 | O | REQ-060 | PARTIALLY IMPLEMENTED | VT |

## 5. M4 – Çadır ve bekleme listesi

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-070 | Çadır: bölge, sıra, numara, kapasite, doluluk, durum, erişilebilirlik, sağlık noktasına yakınlık | Ş M4 | Y | — | IMPLEMENTED | VT |
| REQ-071 | Çadır durumları: Kullanılabilir / Bakımda / Kapalı | Ş Durumlar | Y | — | IMPLEMENTED | BT |
| REQ-072 | Öneri sırası: kullanılabilir → yeterli yer → yerleşim etkisi → en az boş yatak israfı | Ş M4 | Y | REQ-060 | IMPLEMENTED | BT |
| REQ-073 | Sistem önerir, görevli onaylar | Ş M4 | Y | REQ-072 | IMPLEMENTED | VT, KT |
| REQ-074 | Kapasite asla aşılmaz (eşzamanlı işlemlerde de) | Ş M4, M14 | Y | REQ-171 | IMPLEMENTED | BT, VT |
| REQ-075 | Yer yoksa aile bekleme listesine alınır | Ş M4 | Y | — | IMPLEMENTED | VT |
| REQ-076 | Yer açılınca o çadıra uyan aile önerilir: önce destek önceliği, eşitlikte en uzun bekleyen | Ş M4 | Y | REQ-061 | IMPLEMENTED | BT |
| REQ-077 | Görevli onaylayınca aile yerleşir ve listeden çıkar | Ş M4 | Y | REQ-076 | IMPLEMENTED | VT |
| REQ-078 | Renkli, tıklanabilir çadır planı (TileControl) | Ş M4, Ş DevExpress | Y | DevExpress | PARTIALLY IMPLEMENTED | MT |

## 6. M5 – Depo

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-080 | Stok parti ve SKT'ye göre tutulur; SKT'si en yakın uygun parti önce önerilir (FEFO) | Ş M5 | Y | — | IMPLEMENTED | BT |
| REQ-081 | Birimler: adet, paket, kg, litre | Ş M5 | Y | — | IMPLEMENTED | BT |
| REQ-082 | Hareket türleri: giriş, dağıtım, iade, hasar, sayım farkı | Ş M5 | Y | REQ-231 | IMPLEMENTED | BT |
| REQ-083 | Dağıtıma ayrılmış ama teslim edilmemiş miktar ayrıca görünür | Ş M5 | Y | REQ-084 | PARTIALLY IMPLEMENTED | BT, MT |
| REQ-084 | Ayırma: liste onayı → ayrıldı (kullanılabilirden düşer); teslim → dağıtım hareketi; iptal/gün sonu → ayırma kalkar | Ş M5 | Y | — | IMPLEMENTED | BT, VT |
| REQ-085 | SKT'si geçmiş parti dağıtıma çıkamaz, seçilemez, "hasar/fire" hareketiyle düşülür | Ş M5 | Y | REQ-080 | IMPLEMENTED | BT |
| REQ-086 | Stok girişinde bağışçı seçilir / yeni bağışçı kaydedilir | Ş Ekran 9 | O | — | PARTIALLY IMPLEMENTED | VT |

## 7. M6 – Dağıtım

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-090 | Ürün ayarları: hesaplama türü (kişi/aile/bebek başı), hak miktarı, tekrar alma süresi, kritik seviye; tablodaki varsayılanlar | Ş M6 | Y | — | IMPLEMENTED | BT |
| REQ-091 | Hak hesabı kişi başı / aile başı / bebek başı | Ş M6 | Y | REQ-090 | IMPLEMENTED | BT |
| REQ-092 | Tekrar alma süresi dolmadan yeni hak verilmez | Ş M6 | Y | REQ-090 | IMPLEMENTED | BT |
| REQ-093 | Dağıtım listesi o ürüne özel ihtiyacı olan aileler önde olacak şekilde sıralanır | Ş M6 | Y | REQ-062 | IMPLEMENTED | BT |
| REQ-094 | Gerekçe yazılarak istisna yapılabilir | Ş M6 | O | — | IMPLEMENTED | BT |
| REQ-095 | Kısmi teslimatta kalan miktar bekleyen hak olarak kalır | Ş M6 | Y | — | IMPLEMENTED | BT |
| REQ-096 | Teslim eden, teslim alan, miktar ve zaman kaydedilir | Ş M6 | Y | — | IMPLEMENTED | VT |

## 8. M7 – Stok tahmini

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-100 | Yeterlilik süresi = kullanılabilir stok ÷ günlük ortalama tüketim | Ş M7 | O | REQ-084 | IMPLEMENTED | BT |
| REQ-101 | Kişi başı tüketime göre ikinci tahmin | Ş M7 | O | REQ-091 | IMPLEMENTED | BT |
| REQ-102 | Hesabın dayandığı dönem gösterilir; veri yoksa "hesaplanamıyor" | Ş M7 | O | — | IMPLEMENTED | BT |
| REQ-103 | SKT'si yaklaşan ürünler için uyarı | Ş M7 | O | — | IMPLEMENTED | BT |
| REQ-104 | Kritik seviyenin altındaki ürünler için uyarı | Ş M6, M13 | O | REQ-090 | IMPLEMENTED | BT |

## 9. M8 – Yakın bulma

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-110 | Aramayı yalnızca yetkili görevli yapar | Ş M8 | O | REQ-036 | NOT IMPLEMENTED | BT |
| REQ-111 | Sonuçlar yaş ve memleketle daraltılır; Türkçe karakter farkları yok sayılır | Ş M8 | O | REQ-044 | NOT IMPLEMENTED | BT |
| REQ-112 | Başvuru durumları: Açık → Olası eşleşme → Doğrulandı → Kapandı | Ş M8 | O | — | NOT IMPLEMENTED | BT |
| REQ-113 | Fotoğraf tek başına doğrulama sayılmaz; çadır ve iletişim bilgisi otomatik paylaşılmaz | Ş M8 | O | — | NOT IMPLEMENTED | BT |

## 10. M9 – Talep ve görev takibi

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-120 | Talep kaynakları: kayıt masası, görevli, kiosk | Ş M9 | Y | — | IMPLEMENTED | VT |
| REQ-121 | Durumlar ve geçişler: Yeni → Atandı → İşlemde → Tamamlandı / İptal | Ş M9 | Y | — | IMPLEMENTED | BT |
| REQ-122 | Yönlendirme: Sağlık → sağlık personeli, Malzeme → depo, Çadır/arıza → saha görevlisi, Diğer → koordinatör | Ş M9 | Y | — | IMPLEMENTED | BT |
| REQ-123 | Talep alanları: aile/çadır, açıklama, tür, sorumlu görevli, durum, açılış/kapanış zamanı, sonuç | Ş M9 | Y | — | IMPLEMENTED | VT |
| REQ-124 | "Tamamlandı" veya "İptal" yapılırken sonuç yazılması zorunlu | Ş M9 | Y | REQ-121 | IMPLEMENTED | BT |
| REQ-125 | Saha görevlisi yalnızca kendine atanan talepleri görür | Ş Roller | Y | REQ-036 | IMPLEMENTED | BT |

## 11. M10 – Yapay zeka (Ollama)

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-130 | Yapay zeka servisi interface ile tasarlanır; yeni servis tek sınıf eklemekle eklenir | Ş M10 | Y | — | NOT IMPLEMENTED | DK, BT |
| REQ-131 | `HttpClient` ile `http://localhost:11434`; JSON çıktı modu; adres ve model adı ayar dosyasında; internet gerekmez | Ş M10 | Y | — | NOT IMPLEMENTED | BT |
| REQ-132 | Talep sınıflandırma: tür, yardım önceliği ve gerekçe önerisi (JSON) | Ş M10 | Y | REQ-131 | NOT IMPLEMENTED | BT, KT |
| REQ-133 | Sağlık talepleri öncelik almaz, doğrudan sağlık personeline gider, "Sağlık personeli değerlendirecek" işaretlenir | Ş M10 | Y | REQ-122 | PARTIALLY IMPLEMENTED | BT, KT |
| REQ-134 | Ollama çalışmıyor / zaman aşımı / cevap okunamıyorsa talep "Sınıflandırılmamış" kalır, görevli elle devam eder | Ş M10 | Y | — | NOT IMPLEMENTED | BT, KT |
| REQ-135 | Koordinatör için günlük durum özeti | Ş M10 | Y | REQ-131 | NOT IMPLEMENTED | BT, KT |
| REQ-136 | Son karar görevlide: yapay zeka yalnızca öneri yapar, görevli onaylar/değiştirir | Ş M10 | Y | — | NOT IMPLEMENTED | MT |
| REQ-137 | Türkçe'yi iyi anlayan orta boyutlu model denenerek seçilir | Ş M10 | O | Ollama kurulumu | NOT IMPLEMENTED | MT |

## 12. M11 – Kiosk

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-140 | Tam ekran kiosk modu (TileControl) | Ş M11 | O | DevExpress | NOT IMPLEMENTED | MT |
| REQ-141 | Giriş aile numarası ve doğum yılıyla | Ş M11 | O | — | IMPLEMENTED | BT |
| REQ-142 | Kioskta talep bırakılır, talep durumu ve duyurular görülür; yalnızca kendi ailesi | Ş M11, Roller | O | REQ-120 | PARTIALLY IMPLEMENTED | BT, MT |
| REQ-143 | Duyuruları Koordinatör ekler (başlık, metin, başlangıç, bitiş); süresi dolan görünmez | Ş M11 | O | — | PARTIALLY IMPLEMENTED | BT |

## 13. M12 – Gönüllüler

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-150 | Gönüllü kaydı ve uzmanlık alanı | Ş M12 | O | — | NOT IMPLEMENTED | VT |
| REQ-151 | Vardiya planı (SchedulerControl takvimi) | Ş M12 | O | DevExpress | NOT IMPLEMENTED | VT, MT |
| REQ-152 | Görev ataması | Ş M12 | O | — | NOT IMPLEMENTED | VT |
| REQ-153 | Gönüllüleri yalnızca Koordinatör yönetir; gönüllüler giriş yapmaz | Ş M12 | O | REQ-036 | NOT IMPLEMENTED | BT |

## 14. M13 – Ana ekran ve raporlar

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-160 | Ana ekran: toplam kişi, boş yatak, bekleme listesi, kritik stoklar, SKT'si yaklaşanlar, açık talepler, yapay zeka özeti | Ş M13 | Y | — | NOT IMPLEMENTED | BT, MT |
| REQ-161 | Raporlar: dağıtım tutanağı, stok, nüfus, talep (PDF/Excel) | Ş M13 | Y | DevExpress | NOT IMPLEMENTED | MT |

## 15. M14 – Teknik altyapı

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-170 | İşlem geçmişi: kim oluşturdu, değiştirdi, iptal etti (ne zaman, neyi) | Ş M14 | Y | — | IMPLEMENTED | VT |
| REQ-171 | `RowVersion` ile iyimser eşzamanlılık: aynı çadır/stok iki kişiye verilemez; anlaşılır mesaj | Ş M14 | Y | — | PARTIALLY IMPLEMENTED | VT |
| REQ-172 | Program içinden yedek alma ve geri yükleme | Ş M14 | O | — | NOT IMPLEMENTED | VT, MT |
| REQ-173 | Bağlantı kesilirse işlemin kaydedilip kaydedilmediği açıkça gösterilir | Ş M14 | O | — | NOT IMPLEMENTED | MT |
| REQ-174 | Beklenmeyen hatalar `logs/` klasörüne loglanır | C 4.2 | O | — | NOT IMPLEMENTED | MT |
| REQ-175 | `appsettings.json` yoksa anlaşılır kurulum mesajı | C 4.3 | O | — | PARTIALLY IMPLEMENTED | MT |

## 16. Ekle / Listele / Güncelle / Sil kuralları

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-180 | Silinebilir: ürün, çadır, bölge, kullanıcı, gönüllü; kullanılmışsa "Bu kayıt kullanıldığı için silinemez, pasif yapabilirsiniz." | Ş CRUD | Y | — | PARTIALLY IMPLEMENTED | VT |
| REQ-181 | Pasif yapılır, fiziksel silinmez: kişi, aile, dağıtım, stok hareketi | Ş CRUD | Y | — | IMPLEMENTED | VT |

## 17. Veritabanı ve OOP eşlemesi

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-190 | ~25 tablo (Ş listesi); Türkçe karaktersiz isimler; PK/FK, unique, indeks, cascade tasarımı | Ş Veritabanı | Y | — | IMPLEMENTED | VT, DK |
| REQ-230 | Kalıtım: soyut `Kisi` → `Afetzede`, `Gorevli`, `Gonullu` | Ş OOP | Y | — | IMPLEMENTED | DK |
| REQ-231 | Soyut `StokHareketi` ve türleri (polimorfik stok etkisi) | Ş OOP | Y | — | IMPLEMENTED | BT |
| REQ-232 | Generic Repository (`IRepository<T>`) | Ş OOP | Y | — | IMPLEMENTED | VT |

## 18. Ekranlar (18 form)

| ID | Form | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-200 | 1. İlk kurulum | Ş Ekranlar | Y | REQ-030 | NOT IMPLEMENTED | KT |
| REQ-201 | 2. Giriş | Ş Ekranlar | Y | REQ-033 | NOT IMPLEMENTED | KT |
| REQ-202 | 3. Ana ekran (Ribbon, göstergeler) | Ş Ekranlar | Y | REQ-160 | NOT IMPLEMENTED | KT |
| REQ-203 | 4. Aile ve kişi kayıt (gerekçeler, fotoğraf, ayırma, fiş) | Ş Ekranlar | Y | M2, M3 | NOT IMPLEMENTED | KT |
| REQ-204 | 5. Kişi listesi ve arama (filtre, durum değiştirme) | Ş Ekranlar | Y | M2 | NOT IMPLEMENTED | KT |
| REQ-205 | 6. Çadır planı (öneri ve onay) | Ş Ekranlar | Y | M4 | NOT IMPLEMENTED | KT |
| REQ-206 | 7. Bekleme listesi | Ş Ekranlar | Y | M4 | NOT IMPLEMENTED | KT |
| REQ-207 | 8. Ürün tanımları | Ş Ekranlar | Y | M6 | NOT IMPLEMENTED | KT |
| REQ-208 | 9. Stok girişi (bağışçı dahil) | Ş Ekranlar | Y | M5 | NOT IMPLEMENTED | KT |
| REQ-209 | 10. Stok durumu (partiler, hareketler, tahmin, uyarılar) | Ş Ekranlar | Y | M5, M7 | NOT IMPLEMENTED | KT |
| REQ-210 | 11. Dağıtım (liste, teslim, kısmi teslimat) | Ş Ekranlar | Y | M6 | NOT IMPLEMENTED | KT |
| REQ-211 | 12. Talepler (giriş, YZ sınıflandırma, atama, takip, sonuç) | Ş Ekranlar | Y | M9, M10 | NOT IMPLEMENTED | KT |
| REQ-212 | 13. Yakın bulma | Ş Ekranlar | O | M8 | NOT IMPLEMENTED | KT |
| REQ-213 | 14. Gönüllüler (vardiya takvimi, görev ataması) | Ş Ekranlar | O | M12 | NOT IMPLEMENTED | KT |
| REQ-214 | 15. Kiosk | Ş Ekranlar | O | M11 | NOT IMPLEMENTED | KT |
| REQ-215 | 16. Raporlar | Ş Ekranlar | Y | M13 | NOT IMPLEMENTED | KT |
| REQ-216 | 17. Ayarlar ve kullanıcılar (roller, gerekçe ayarları, yedekleme, duyurular) | Ş Ekranlar | Y | M1, M11, M14 | NOT IMPLEMENTED | KT |
| REQ-217 | 18. İşlem geçmişi | Ş Ekranlar | O | REQ-170 | NOT IMPLEMENTED | KT |

## 19. Demo, test ve dokümantasyon

| ID | Gereksinim | Kaynak | Ön. | Bağımlılık | Durum | Test |
|---|---|---|---|---|---|---|
| REQ-220 | Seed: 3 bölge, 30 çadır, 20 aile (bebekli, engelli, yaşlı dahil), 10 ürün, SKT'si yaklaşmış bir parti | Ş Demo | Y | REQ-004 | IMPLEMENTED | VT |
| REQ-221 | Seed kullanıcı içermez | Ş Demo | Y | — | IMPLEMENTED | VT |
| REQ-222 | 7 adımlı demo senaryosu uçtan uca çalışır | Ş Demo | Y | Tümü | NOT IMPLEMENTED | KT |
| REQ-240 | xUnit birim testleri (C 9.1 listesindeki tüm kurallar) | C 9.1 | Y | — | PARTIALLY IMPLEMENTED | BT |
| REQ-241 | Veritabanı testleri ayrı LocalDB test veritabanında | C 9.1 | Y | — | IMPLEMENTED | VT |
| REQ-250 | Dokümanlar: README, SETUP, ARCHITECTURE, PROJECT_DECISIONS, TESTING, PROJECT_STATUS, SUNUM_NOTLARI, FINAL_AUDIT | C 7 | Y | — | PARTIALLY IMPLEMENTED | DK |

---

**Özet:** Toplam 142 gereksinim · IMPLEMENTED 64 · PARTIALLY 31 · NOT IMPLEMENTED 47 · BLOCKED 0
