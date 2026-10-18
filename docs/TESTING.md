# Test

## Strateji
| Tür | Kapsam | Veritabanı |
|---|---|---|
| Birim testi | Entity kuralları (doğrulama, kapasite, parti/ayırma, stok hareketi polimorfizmi, TC, Türkçe normalizasyon) ve saf iş kuralları (mükerrer kontrol, gerekçe, çadır önerisi, bekleme sırası, FEFO, hak, devretme, stok tahmini); Aşama 4'te yapay zeka cevap ayrıştırma | Yok |
| Veritabanı (entegrasyon) testi | Demo verisi, numaralar, kısıtlar (TC tekilliği, kapasite, sağlık önceliği, tek açık dağıtım listesi), RowVersion çakışması, repository silme kuralları, Business servisleri (kayıt, gerekçe, çadır, bekleme, ürün, stok, dağıtım) | `AfetCadirKent_Test` (LocalDB) |
| Kabul testi | CLAUDE.md 9.2 senaryosu, elle | Geliştirme veritabanı |

- Veritabanı testleri `[Collection("Veritabani")]` ile sırayla çalışır. Test oturumu başında `AfetCadirKent_Test` silinir, migration ve demo verisiyle yeniden kurulur. Geliştirme veritabanına (`AfetCadirKent`) dokunulmaz.
- Kısıt testleri (`VeritabaniKisitTestleri`) transaction içinde çalışır ve geri alınır; çadır eşzamanlılık testi kendi kayıtlarını `finally` içinde temizler.
- **Servis testleri verilerini test veritabanında kalıcı bırakır** (servisler kendi transaction'larını yönetir). Bu yüzden her test kendi verisini benzersiz adlarla oluşturur ve yalnızca kendi kayıtları üzerinde doğrulama yapar; demo verisi testleri demo kayıtlarını ayrıca süzer (bkz. aşağıdaki notlar). Test sırasına bağımlı doğrulama yazılmaz.

## Çalıştırma
```
dotnet test
```
Gereksinim: SQL Server LocalDB (`sqllocaldb info` → `MSSQLLocalDB`). Farklı sunucu için `AFETCADIRKENT_TEST_SUNUCU` ortam değişkeni.

## Kapsam
```
dotnet test --settings tests/kapsam.runsettings --collect:"XPlat Code Coverage"
```
Sonuç `tests/AfetCadirKent.Tests/TestResults/<guid>/coverage.cobertura.xml` dosyasına yazılır (git'e girmez). Ölçüme Entities, DataAccess ve Business katmanları girer; EF Core'un ürettiği migration kodu ve test projesi hariçtir. UI katmanı henüz ekran içermediği için ölçülmez. Async metotların gövdesi derleyicinin ürettiği sınıflarda bulunduğundan `CompilerGenerated` dışlanmaz; aynı kaynak satırı bir kez sayılır.

**2026-10-17 (170 test):**

| Katman | Satır | Dal (branch) |
|---|---|---|
| **Toplam** | **%91,2** (3120/3421) | **%76,7** (719/937) |
| Entities | %82,1 | %79,2 |
| DataAccess | %95,5 | %87,5 |
| Business | %92,5 | %74,8 |
| ↳ Gerekçeler | %98,7 | %88,5 |
| ↳ Depo yönetimi | %95,2 | %77,2 |
| ↳ Dağıtım yönetimi | %93,1 | %71,8 |
| ↳ Çadır yönetimi | %91,9 | %66,9 |
| ↳ Kayıt | %87,6 | %79,6 |
| ↳ Ortak | %85,0 | %71,0 |

En düşük kapsamlı dosyalar ve nedeni:
- `Talep.cs` (%16), `Gonullu.cs` (%44), `Sistem.cs` (%55: duyuru, vardiya): servisleri sonraki aşamalarda (P5, Aşama 7) yazılacak; şimdilik yalnızca demo verisi ve kısıt testleri dokunuyor.
- `TasarimZamaniContextFabrikasi.cs` (%0): yalnızca `dotnet ef` komutlarında çalışır, uygulama ve testler kullanmaz.
- `ServisYardimcilari.cs` (%77): ekrandaki Türkçe metin karşılıklarının (`Gosterim`) bir kısmı henüz hiçbir akışta gösterilmiyor.

## Durum
| Tarih | Toplam | Başarılı | Başarısız |
|---|---|---|---|
| 2026-10-16 (Aşama 2) | 52 | 52 | 0 |
| 2026-10-16 (Aşama 3a / P2) | 103 | 103 | 0 |
| 2026-10-16 (Aşama 3b / P3) | 124 | 124 | 0 |
| 2026-10-17 (Aşama 3c / P4) | 156 | 156 | 0 |
| 2026-10-17 (denetim düzeltmeleri, kritik) | 165 | 165 | 0 |
| 2026-10-17 (denetim düzeltmeleri, önemli) | 170 | 170 | 0 |
| 2026-10-18 (Aşama 3d / P5) | 207 | 207 | 0 |

## Kapsanan kurallar (C 9.1 listesine göre)
| Kural | Test sınıfı |
|---|---|
| Mükerrer kayıt ve Türkçe normalizasyon | `MukerrerKontrolTestleri`, `TurkceMetinTestleri`, `KayitServisiTestleri` |
| Gerekçe → destek önceliği ve malzeme ihtiyacı | `GerekceKuralTestleri`, `KayitServisiTestleri.DemoAilesiIcinIkiAyriBilgiGosterilir` |
| Yetki kontrolleri | `OturumTestleri`, `KayitServisiTestleri.YetkisizRolKayitYapamaz`, refakatsiz çocuk ve sağlık notu testleri |
| Silme / pasife alma | `RepositoryTestleri`, `KayitServisiTestleri.KisiPasifYapilir…` |
| Çadır kapasitesi (entity + DB + eşzamanlı yerleştirme) | `CadirTestleri`, `VeritabaniKisitTestleri`, `CadirServisiTestleri.AyniCadiraEsZamanliYerlestirmedeYalnizcaBiriBasarili` |
| Çadır öneri sırası | `CadirOneriMotoruTestleri`, `CadirServisiTestleri` |
| Bekleme listesi sıralaması ve önerisi | `BeklemeSiralamasiTestleri`, `BeklemeListesiServisiTestleri` |
| Parti, ayırma, SKT | `PartiVeStokHareketiTestleri`, `DepoKuralTestleri` |
| FEFO parti seçimi ve SKT'si geçmiş partinin dışlanması | `DepoKuralTestleri.Fefo_*`, `DepoServisTestleri.SktGecenParti_…`, `DagitimServisiTestleri.Onay_FefoIle…` |
| Ayırma / teslim / iptal stok hesapları | `DepoKuralTestleri.Ayirma_*`, `DagitimServisiTestleri.MalzemeIhtiyaciOlanAileOnde_…` (onay → ayrılan, teslim → fiziksel düşer, teslim iptali ve gün sonu → stoğa döner), `YetersizStoktaOnayYapilmaz_…` |
| Hak hesabı (kişi / aile / bebek başı; hastanedeki üye) | `DepoKuralTestleri.Hak_*`, `DagitimServisiTestleri.HastanedekiUyeninHakkiAiledeKalir_…` |
| Tekrar alma süresi ve istisna | `DepoKuralTestleri.TekrarAlmaSuresi_…`, `DagitimServisiTestleri.TekrarAlmaSuresi_KismiTeslimDevreder_IstisnaGerekceyleYapilir` |
| Kısmi teslimat (bekleyen hak, devretme) | `DepoKuralTestleri.KismiTeslim_…`, `DepoKuralTestleri.Devretme_TeslimOnceDevredeneSayilir` (6 durum), `DepoKuralTestleri.HakAta_…`, `DagitimServisiTestleri.DevredenHak_AileGelmeseDeKaybolmaz_TeslimiTekrarAlmaSuresiniKaydirmaz` |
| Yeni üründe gerekçe ↔ ürün eşleşmesi (kategori) | `DepoServisTestleri.YeniUrundeGerekceEslesmeleriKategoridenKurulur_…`, `DagitimServisiTestleri.MalzemeIhtiyaciOlanAileOnde_…` (elle eşleşme olmadan) |
| Refakatsiz çocuk gizliliği (tüm servisler) | `RefakatsizGizlilikTestleri`, `KayitServisiTestleri.RefakatsizCocuguYalnizcaKoordinatorGorur`, `DagitimServisiTestleri.RefakatsizCocukAilesi_…` |
| Metin uzunlukları ve veritabanı uzunluk hatası | `KayitServisiTestleri.UzunMetinler_…`, `KayitServisiTestleri.VeritabaniUzunlukHatasiAnlasilirMesajaCevrilir` |
| Ürün başına tek açık liste (yarış durumu) | `DagitimServisiTestleri.BirUrundeIkinciAcikListeVeritabanindaEngellenir` |
| Dağıtım listesi sıralaması (malzeme ihtiyacı önde) | `DagitimServisiTestleri.MalzemeIhtiyaciOlanAileOnde_…` |
| Stok tahmini ve "hesaplanamıyor" | `DepoKuralTestleri.Tahmin_*`, `DepoServisTestleri.Tahmin_…` |
| Stok hareketleri (iade, hasar, sayım farkı, pasif yapma / geri alma) | `DepoKuralTestleri.HareketGeriAlma_…`, `DepoServisTestleri` |
| Stokta eşzamanlılık (RowVersion) | `DepoServisTestleri.AyniStokEsZamanliIkiKisiyeAyrilamaz_…` |
| Sağlık talebinin öncelik almaması (DB kısıtı) | `VeritabaniKisitTestleri.SaglikTalebineYardimOnceligiVerilemez` |
| Talep durum geçişleri ve zorunlu sonuç | `TalepVeParolaKuralTestleri.Akis_…`, `GecersizGecisler_Reddedilir`, `Iptal_…`, `TalepServisiTestleri.Senaryo4_…`, `Iptal_AcanGorevliYeniTalebiniIptalEder_…` |
| Talep yönlendirme ve atama | `TalepVeParolaKuralTestleri.Yonlendirme_TureGoreHedefRol`, `YenidenSiniflandirma_…`, `TalepServisiTestleri.Atama_…` |
| Sağlık talebinin öncelik almaması (servis) | `TalepVeParolaKuralTestleri.SaglikTalebi_…`, `TalepServisiTestleri.Senaryo5_CocukAtesleniyor_…` |
| Talep görünürlüğü (saha görevlisi yalnız kendine atanan, refakatsiz) | `TalepServisiTestleri.Senaryo4_…`, `RefakatsizCocukAilesininTalebi_YalnizKoordinatoreGorunur` |
| Kiosk girişi, talebi, duyurular | `KioskServisiTestleri` |
| İlk kurulum, giriş, parola (BCrypt) | `KurulumTestleri`, `KullaniciVeOturumTestleri`, `TalepVeParolaKuralTestleri.ParolaKurali_…`, `Bcrypt_…` |
| Kullanıcı silme / pasif, son Koordinatör | `KullaniciVeOturumTestleri.Silme_…`, `AcikTalebiOlanGorevliPasifYapilamaz_…`, `KurulumTestleri` |
| İşlem geçmişi listeleme | `KullaniciVeOturumTestleri.IslemGecmisi_YalnizKoordinatorGorur_Filtrelenir` |
| Henüz yok (P6) | Yapay zeka (Ollama) servisi |

Çadır servis testleri her test için benzersiz kodlu ayrı bir test bölgesi ve çadırlar oluşturur.

Servis testleri kendi oluşturdukları ailelerle (benzersiz soyad) çalışır; demo verisi testleri yalnızca demo ailelerini sayar.

Depo ve dağıtım testleri kendi ürünlerini (`TestUrun…` önekli, benzersiz ad) ve partilerini oluşturur; demo verisi testleri bu ürünleri ve testlerin eklediği kullanıcıları saymaz. Tekrar alma süresi testleri `SabitZaman` ile ileri tarihli liste hazırlar ve test sonunda o tarihte gün sonu yaparak listeleri kapatır. Stok tutarlılığı testi yalnızca aktif hareketleri toplar (pasif yapılan hareketin etkisi geri alınmıştır).
