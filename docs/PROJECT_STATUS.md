# Proje Durumu

_Son güncelleme: 2026-10-17_

## Aşama özeti

| Aşama | İçerik | Durum |
|---|---|---|
| 0 | Şartname analizi, gereksinim envanteri, kararlar, mimari | ✅ Tamamlandı |
| 1 | Solution iskeleti, projeler, `appsettings.example.json`, README taslağı | ✅ Tamamlandı |
| 2 | Entity'ler, DbContext, migration, seed | ✅ Tamamlandı (P1) |
| 3 | Business servisleri ve kurallar + birim testleri | ⏳ Devam ediyor: 3a ✅ (P2), 3b ✅ (P3), 3c ✅ (P4), 3d sırada (P5) |
| 4 | Ollama yapay zeka servisi + testleri | Bekliyor |
| ⏸ | DevExpress kurulumu ve Ollama modeli (kullanıcı) | Bekliyor |
| 5–10 | UI, ana akış, diğer formlar, altyapı, test, final denetim | Bekliyor |

## Sıradaki adım
**P6 – Aşama 4:** Yapay zeka servisi: `IYapayZekaServisi` interface'i + `OllamaServisi` (`HttpClient`, adres ve model `appsettings.json`'dan, JSON çıktı modu), talep sınıflandırma (tür, yardım önceliği, gerekçe önerisi → `Talep.YzOnerilenTur/YzOnerilenOncelik/YzGerekce`, `SiniflandirmaDurumu.YapayZekaOnerdi`; son karar görevlide), sağlık türünde önerilen önceliğin yok sayılması (K-051), zaman aşımı / bağlantı hatası / okunamayan cevapta talebin "Sınıflandırılmamış" kalması, koordinatör için günlük özet; sahte `HttpMessageHandler` ile testler (K-070). İlgili REQ'ler: M10 (REQ-130…). Talep servisine YZ önerisini kaydetme ve öneriyi kabul etme yolu eklenecek.

## Takvim (teslim: Aralık başı)

Kural: oturum başına bir iş paketi (CLAUDE.md 1.2.1). Tarihler hedeftir; gerekirse kayar.

| # | Hedef tarih | İş paketi |
|---|---|---|
| P1 | 16 Eki | Aşama 2: entity'ler, DbContext, migration, demo verisi, veritabanı testleri |
| P2 | ~17–18 Eki | Aşama 3a: Türkçe normalizasyon, mükerrer kontrol, kayıt servisi, aileden ayırma, gerekçe kuralları |
| P3 | ~19–20 Eki | Aşama 3b: çadır önerisi, yerleşim, bekleme listesi |
| P4 | ~21–22 Eki | Aşama 3c: depo, FEFO, ayırma/teslim/iptal, hak hesabı, kısmi teslimat, stok tahmini |
| P5 | ~23–24 Eki | Aşama 3d: talep iş akışı, yetki kontrolü, işlem geçmişi, kurulum/giriş servisi |
| P6 | ~27–28 Eki | Aşama 4: Ollama servisi (interface, JSON, hata → "Sınıflandırılmamış") |
| — | ~29 Eki–4 Kas | Kullanıcı: Ollama kurulumu ve model denemesi; rapor malzemesi (ER, UML), sunum notları taslağı |
| ⏸ | **~5 Kas** | **Kullanıcı: DevExpress kurulumu** (30 günlük deneme ~5 Ara'ya kadar sürer → sunum dahil kapsanır) |
| P7 | ~5–7 Kas | Aşama 5: ilk kurulum, giriş, ana ekran, rol bazlı menü |
| P8–P11 | ~8–18 Kas | Aşama 6: ana akış formları (kayıt, kişi listesi, çadır planı, bekleme, ürün, stok, dağıtım, talepler) |
| P12–P14 | ~19–24 Kas | Aşama 7: yakın bulma, gönüllüler, kiosk, raporlar, ayarlar, işlem geçmişi, aile fişi |
| P15 | ~25–26 Kas | Aşama 8: eşzamanlılık, yedek/geri yükleme, bağlantı kesintisi |
| P16 | ~27–28 Kas | Aşama 9: kabul testi, hata düzeltme; ekran görüntüleri |
| P17 | ~29–30 Kas | Aşama 10: final denetim, dokümanlar, sunum notları |

## Tamamlananlar
- **Aşama 0:** Föy ve Şartname PDF'leri okundu, CLAUDE.md ile karşılaştırıldı (çelişki yok; Föy içindeki "opsiyonel/zorunlu" tutarsızlığı K-001'de çözüldü). `REQUIREMENTS.md` (142 gereksinim), `PROJECT_DECISIONS.md`, `ARCHITECTURE.md`.
- **Aşama 1:**
  - `AfetCadirKent.slnx` + 5 proje (Entities, DataAccess, Business, UI, Tests); referans yönü UI → Business → DataAccess → Entities.
  - Paketler: EF Core 10.0.13 (SqlServer, Design), BCrypt.Net-Next 4.2.0, Microsoft.Extensions (Configuration.Json, DI), Serilog.Sinks.File, xUnit.
  - `.gitignore`: `appsettings.json` ve `*.bak` eklendi; `appsettings.example.json` repoda.
  - UI `Program.cs`: Serilog dosya logu, `appsettings.json` yoksa kurulum mesajı.
  - `README.md` taslağı ve `docs/SETUP.md`.
  - `dotnet build`: 0 uyarı, 0 hata.
- **Aşama 2 (P1, 2026-10-16):**
  - 27 tablo: entity'ler (soyut `Kisi` → `Afetzede`/`Gorevli`/`Gonullu`; soyut `StokHareketi` → 5 tür, TPH), Fluent konfigürasyonlar, CHECK kısıtları, filtreli tekil indeksler, RowVersion.
  - Migration `IlkOlusturma`; roller (6), yetki matrisi (68), gerekçeler (10) migration ile gelir.
  - `DemoVeriYukleyici` + `VeritabaniHazirlayici`: uygulama açılışında migration + boşsa demo verisi (K-060).
  - Generic `IRepository<T>` / `Repository<T>`, `VeriOturumu` (unit of work), tasarım zamanı fabrikası (bağlantı dizesi ortam değişkeni veya yerel `appsettings.json`).
  - Geliştirme veritabanı `AfetCadirKent` LocalDB'de oluşturuldu (şema + referans verisi; demo verisi ilk uygulama açılışında yüklenecek).
  - 52 test (birim + `AfetCadirKent_Test` üzerinde veritabanı testleri), tamamı başarılı.
- **Aşama 3a (P2, 2026-10-16):**
  - Business altyapısı: `Oturum` (rol izinleri, `IzinGerekli`), Business istisnaları, işlem geçmişi yazıcısı, eşzamanlılık/tekillik hatalarını anlaşılır mesaja çeviren `GuvenliKaydetAsync`, `TimeProvider` ile test edilebilir zaman.
  - Gerekçe kuralları: `IGerekceKurali` + 10 kural sınıfı (polimorfizm), `GerekceDegerlendirici` (en yüksek gerekçe → destek önceliği, elle ekleme/kaldırma), `GerekceServisi` (toplu değerlendirme, ürün bazlı malzeme ihtiyacı, açıklamalı elle değişiklik + geri alma + geçmiş).
  - Kayıt: `MukerrerKontrol` (TC engeli, ad-soyad + yaş farkı < 2, Türkçe karakter farkı yok sayılır), `KayitServisi` (aile/kişi kaydı, sonradan yakın ekleme, güncelleme, sağlık bilgisi, durum geçişleri ve hareketler, aileden ayırma, pasif yapma, refakatsiz çocuk ve sağlık notu görünürlüğü, listeleme/arama).
  - 103 test, tamamı başarılı; derleme 0 uyarı.
- **Aşama 3b (P3, 2026-10-16; commit 2026-10-17):**
  - `CadirOneriMotoru`: kullanılabilir → yeterli yer → yerleşim etkisi → en az israf; tek çadır yetmezse aynı bölgede iki çadır; öneri özeti metni.
  - `CadirServisi`: bölge ve çadır ekle/listele/güncelle/sil/pasif (kullanılmışsa silinemez, dolu çadır kapatılamaz), çadır planı (renk durumu, çadırdaki aileler), öneri, görevli onayıyla yerleştirme (aile iki çadıra yerleşebilir), çadır değişikliği.
  - `BeklemeListesiServisi`: ekleme, öncelik + bekleme süresine göre sıralı liste, yer açılan çadır için öneri, onayla yerleştirme, listeden çıkarma; doğrudan yerleşen aile listeden otomatik çıkar.
  - 124 test (eşzamanlı yerleştirme dahil), tamamı başarılı; derleme 0 uyarı.
- **Aşama 3c (P4, 2026-10-17):**
  - Saf kurallar: `FefoSecici` (SKT'si en yakın uygun parti önce, SKT geçmiş/pasif dışlanır, partilere bölme, yetersiz stok mesajı), `HakHesaplayici` (kişi/aile/bebek başı, hastanedeki üye dahil, tekrar alma süresi), `StokTahminMotoru` (tüketime ve hakka göre iki tahmin, dönem, "hesaplanamıyor").
  - Entity: `StokAyirma.TeslimEt/SerbestBirak`; `StokHareketi.PartidenGeriAl` (her tür kendi etkisini geri alır; dağıtım geri alınamaz).
  - `UrunServisi`: ürün ve ürün ayarları ekle/listele/güncelle/sil/pasif (kullanılmışsa silinemez), bağışçı kaydı.
  - `StokServisi`: stok girişi (parti, SKT, bağışçı), iade, hasar, sayım farkı, hatalı hareketi pasif yapma, SKT geçenleri fireye düşme, stok durumu (fiziksel / ayrılan / kullanılabilir ayrı), hareket listesi, kritik seviye ve SKT uyarıları, stok tahmini.
  - `DagitimServisi`: liste hazırlama (hakkı doğanlar + devreden bekleyen hak, malzeme ihtiyacı olanlar önde), taslakta aile ekleme (süre dolmamışsa gerekçeli istisna) ve satır çıkarma, onayda FEFO ayırma, teslim (teslim eden/alan/miktar/zaman, kısmi teslimat), teslim iptali, liste iptali, gün sonu ve önceki günleri kapatma; refakatsiz çocuk ailesi Koordinatör dışında gizli (K-068).
  - 156 test, tamamı başarılı; derleme 0 uyarı.
- **Denetim düzeltmeleri (2026-10-17, kritik maddeler):**
  - Devreden hak modeli: `DagitimDetaylari.DevredenMiktar` (migration `DevredenHakVeTekAcikListe`); ödenmemiş devreden artık kaybolmuyor, devredenin teslimi tekrar alma süresini kaydırmıyor (K-046).
  - Ürün başına tek açık dağıtım listesi için filtreli tekil indeks; eşzamanlı ikinci listeye anlaşılır mesaj (K-049a, K-062).
  - Dokümanlar kodla eşitlendi: ARCHITECTURE'daki olmayan tip adları, K-028, TESTING'deki transaction açıklaması.
  - 165 test, tamamı başarılı. Geliştirme veritabanına migration uygulandı (veri kaybı yok).
- **Denetim düzeltmeleri (2026-10-17, önemli maddeler 4, 5, 8, 9):**
  - Yeni üründe ve kategori değişiminde gerekçe ↔ ürün eşleşmeleri kendiliğinden kuruluyor; öncelikli kategorilerin tek kaynağı `ReferansVerisi` (K-041).
  - Uzun metin hataları: sistemin birleştirdiği açıklamalar sütuna sığdırılıyor, aile/gönüllü notları doğrulanıyor, veritabanı uzunluk hatası anlaşılır mesaja çevriliyor.
  - Refakatsiz çocuk görünürlüğü `RefakatsizGizlilik` yardımcısında toplandı; gerekçe, bekleme listesi, aile güncelleme/kişi ekleme ve aileden ayırmadaki açıklar kapatıldı (K-068).
  - README (hazır/planlanan ayrımı, veritabanı adımı) ve SETUP (dotnet-ef, ortam değişkenleri, test veritabanı) güncellendi.
  - 170 test, tamamı başarılı.
- **Denetim düzeltmeleri (2026-10-17, iyi olur maddeleri 12, 14):**
  - Tekrarlanan kod toplandı: tek transaction'da "kaydet + işlem geçmişine yaz" yardımcısı (`KaydetVeGecmiseYazAsync`, 11 kopya kaldırıldı), `IOturum.KullaniciId`, tek yaş hesabı (`Afetzede.YasHesapla`). Davranış değişmedi, 170 test geçiyor.
  - Test kapsamı ölçüldü (`tests/kapsam.runsettings`): satır %91,2, dal %76,7 (ayrıntı `docs/TESTING.md` → Kapsam).
- **Aşama 3d (P5, 2026-10-18):**
  - `Talep` durum makinesi entity'de (K-050): sınıflandırma ve yönlendirme, atama, işleme alma, tamamlama/iptal (sonuç zorunlu); sağlık talebine öncelik verilmez, "Sağlık personeli değerlendirecek" işaretlenir.
  - `TalepServisi`: kayıt masası/görevli talebi (çadır tek çadırdaysa kendiliğinden), sınıflandırma, atama (yalnız hedef rol veya Koordinatör), iş akışı, rol bazlı görünürlük ve filtreli liste, atanabilir görevliler (K-072).
  - `KioskServisi`: aile no + doğum yılıyla aile girişi (art arda hatalı girişte bekleme), kiosk talebi, ailenin talepleri, yayındaki duyurular (K-073).
  - `KurulumServisi` (ilk Koordinatör, yalnız hiç kullanıcı yokken), `OturumServisi` (BCrypt giriş, pasif hesap, giriş/çıkış kaydı, parola değiştirme), `KullaniciServisi` (ekle, güncelle/rol, parola yenile, pasif, kullanılmamışsa sil), `ParolaGuvenligi` (K-074).
  - `IslemGecmisiServisi`: tarih, kullanıcı, varlık, işlem türü ve metinle filtreli işlem geçmişi.
  - 207 test, tamamı başarılı; derleme 0 uyarı.
- **Ertelenen denetim maddeleri:** 10 (yerleşimde kişi seçimi, bekleme listesinden iki çadıra) → Aşama 6; 11 (büyük veri için sayfalama/son satır sorgusu) → gerekirse; 13 (demo verisi ayarı) → Aşama 5.
- **Açık kararlar (kullanıcı):** sağlık bilgisi (hamile/kronik hasta) iznini kim kullanır; pasif kişi/aileyi yeniden aktif yapma eklensin mi (denetim maddeleri 6 ve 7).

## Ortam
- .NET SDK 10.0.402 (ve 9.0.317) → hedef .NET 10.
- SQL Server LocalDB (`MSSQLLocalDB`) mevcut.
- DevExpress: **kurulu değil** (Aşama 4 sonrası kurulacak).
- Ollama: henüz kontrol edilmedi (Aşama 4 sonrası).

## Bilinen problemler
- UI şu an yalnızca ayar kontrolü yapıp kapanır; formlar Aşama 5'ten itibaren eklenecek (DevExpress gerekli).
- Başlangıç mesajı geçici olarak standart `MessageBox` (K-016).

## Test durumu
- 207 / 207 başarılı (bkz. `docs/TESTING.md`).

## Kalan gereksinimler
- IMPLEMENTED 64 · PARTIALLY 31 · NOT IMPLEMENTED 47 (bkz. REQUIREMENTS.md).
