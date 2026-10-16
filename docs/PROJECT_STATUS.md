# Proje Durumu

_Son güncelleme: 2026-10-16_

## Aşama özeti

| Aşama | İçerik | Durum |
|---|---|---|
| 0 | Şartname analizi, gereksinim envanteri, kararlar, mimari | ✅ Tamamlandı |
| 1 | Solution iskeleti, projeler, `appsettings.example.json`, README taslağı | ✅ Tamamlandı |
| 2 | Entity'ler, DbContext, migration, seed | ⏳ Sırada |
| 3 | Business servisleri ve kurallar + birim testleri | Bekliyor |
| 4 | Ollama yapay zeka servisi + testleri | Bekliyor |
| ⏸ | DevExpress kurulumu ve Ollama modeli (kullanıcı) | Bekliyor |
| 5–10 | UI, ana akış, diğer formlar, altyapı, test, final denetim | Bekliyor |

## Sıradaki adım
**P1 – Aşama 2 (devam ediyor):** Entity'ler ✅ → DbContext ve konfigürasyonlar → ilk migration → demo verisi → veritabanı testleri. Bitince dur; P2 yeni oturumda.

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

## Ortam
- .NET SDK 10.0.402 (ve 9.0.317) → hedef .NET 10.
- SQL Server LocalDB (`MSSQLLocalDB`) mevcut.
- DevExpress: **kurulu değil** (Aşama 4 sonrası kurulacak).
- Ollama: henüz kontrol edilmedi (Aşama 4 sonrası).

## Bilinen problemler
- UI şu an yalnızca ayar kontrolü yapıp kapanır; formlar Aşama 5'ten itibaren eklenecek (DevExpress gerekli).
- Başlangıç mesajı geçici olarak standart `MessageBox` (K-016).

## Test durumu
- Test projesi hazır, henüz test yok (testler Aşama 2–4'te).

## Kalan gereksinimler
- 135 NOT IMPLEMENTED, 7 PARTIALLY IMPLEMENTED (bkz. REQUIREMENTS.md).
