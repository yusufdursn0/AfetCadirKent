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
| Parti, ayırma, SKT | `PartiVeStokHareketiTestleri` |
| Sağlık talebinin öncelik almaması (DB kısıtı) | `VeritabaniKisitTestleri.SaglikTalebineYardimOnceligiVerilemez` |
| Henüz yok (P4–P6) | FEFO seçimi, ayırma/teslim/iptal servisi, hak hesabı, tekrar alma, kısmi teslimat servisi, stok tahmini, talep geçişleri, yapay zeka |

Çadır servis testleri her test için benzersiz kodlu ayrı bir test bölgesi ve çadırlar oluşturur.

Servis testleri kendi oluşturdukları ailelerle (benzersiz soyad) çalışır; demo verisi testleri yalnızca demo ailelerini sayar.
