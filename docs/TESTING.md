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
