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
