# Test

## Strateji
| Tür | Kapsam | Veritabanı |
|---|---|---|
| Birim testi | Entity kuralları (doğrulama, kapasite, parti/ayırma, stok hareketi polimorfizmi, TC, Türkçe normalizasyon); Aşama 3–4'te iş kuralları ve yapay zeka cevap ayrıştırma | Yok |
| Veritabanı (entegrasyon) testi | Demo verisi, numaralar, kısıtlar (TC tekilliği, kapasite, sağlık önceliği), RowVersion çakışması, repository silme kuralları | `AfetCadirKent_Test` (LocalDB) |
| Kabul testi | CLAUDE.md 9.2 senaryosu, elle | Geliştirme veritabanı |

- Veritabanı testleri `[Collection("Veritabani")]` ile sırayla çalışır. Test oturumu başında `AfetCadirKent_Test` silinir, migration ve demo verisiyle yeniden kurulur. Geliştirme veritabanına (`AfetCadirKent`) dokunulmaz.
- Veri değiştiren testler transaction içinde çalışır ve geri alınır; transaction kullanamayan eşzamanlılık testi kendi oluşturduğu kayıtları `finally` içinde temizler.

## Çalıştırma
```
dotnet test
```
Gereksinim: SQL Server LocalDB (`sqllocaldb info` → `MSSQLLocalDB`). Farklı sunucu için `AFETCADIRKENT_TEST_SUNUCU` ortam değişkeni.

## Durum
| Tarih | Toplam | Başarılı | Başarısız |
|---|---|---|---|
| 2026-10-16 (Aşama 2) | 52 | 52 | 0 |
| 2026-10-16 (Aşama 3a / P2) | 103 | 103 | 0 |
| 2026-10-16 (Aşama 3b / P3) | 124 | 124 | 0 |
| 2026-10-17 (Aşama 3c / P4) | 156 | 156 | 0 |

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
| Kısmi teslimat (bekleyen hak, devretme) | `DepoKuralTestleri.KismiTeslim_…`, `DagitimServisiTestleri` |
| Dağıtım listesi sıralaması (malzeme ihtiyacı önde) | `DagitimServisiTestleri.MalzemeIhtiyaciOlanAileOnde_…` |
| Stok tahmini ve "hesaplanamıyor" | `DepoKuralTestleri.Tahmin_*`, `DepoServisTestleri.Tahmin_…` |
| Stok hareketleri (iade, hasar, sayım farkı, pasif yapma / geri alma) | `DepoKuralTestleri.HareketGeriAlma_…`, `DepoServisTestleri` |
| Stokta eşzamanlılık (RowVersion) | `DepoServisTestleri.AyniStokEsZamanliIkiKisiyeAyrilamaz_…` |
| Sağlık talebinin öncelik almaması (DB kısıtı) | `VeritabaniKisitTestleri.SaglikTalebineYardimOnceligiVerilemez` |
| Henüz yok (P5–P6) | Talep durum geçişleri ve zorunlu sonuç, sağlık talebinin öncelik almaması (servis), kurulum/giriş, yapay zeka |

Çadır servis testleri her test için benzersiz kodlu ayrı bir test bölgesi ve çadırlar oluşturur.

Servis testleri kendi oluşturdukları ailelerle (benzersiz soyad) çalışır; demo verisi testleri yalnızca demo ailelerini sayar.

Depo ve dağıtım testleri kendi ürünlerini (`TestUrun…` önekli, benzersiz ad) ve partilerini oluşturur; demo verisi testleri bu ürünleri ve testlerin eklediği kullanıcıları saymaz. Tekrar alma süresi testleri `SabitZaman` ile ileri tarihli liste hazırlar ve test sonunda o tarihte gün sonu yaparak listeleri kapatır. Stok tutarlılığı testi yalnızca aktif hareketleri toplar (pasif yapılan hareketin etkisi geri alınmıştır).
