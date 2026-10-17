# Afet Çadır Kent Yönetim Otomasyonu

Afet sonrası kurulan bir çadır kentte afetzedeleri, aileleri, çadırları, yardım malzemelerini, talepleri ve gönüllüleri tek bir masaüstü uygulamasından yönetmek için geliştirilen Windows Forms uygulaması.

**Ana akış:** Aile kaydı → Uygun çadır → İhtiyaç belirleme → Stoktan teslim → Takip

> Nesne Tabanlı Programlama (2. sınıf) dönem projesidir. Proje geliştirme aşamasındadır; güncel durum için [docs/PROJECT_STATUS.md](docs/PROJECT_STATUS.md).

## Özellikler ve durum

Geliştirme aşamalara bölünmüştür; güncel ayrıntı için [PROJECT_STATUS.md](docs/PROJECT_STATUS.md). **Ekranlar henüz yoktur** (DevExpress kurulumundan sonra, Aşama 5'ten itibaren gelecek); aşağıdaki "hazır" özellikler iş katmanında çalışır ve otomatik testlerle doğrulanır.

| Özellik | Durum |
|---|---|
| Aile ve kişi kaydı, mükerrer kayıt kontrolü (Türkçe karakter farkları yok sayılır), aileden ayırma, hastane durumu | ✅ Hazır (iş katmanı) |
| Gerekçeli öncelik: ailenin **destek önceliği** ve **malzemeye özel ihtiyaçları** ayrı ayrı | ✅ Hazır (iş katmanı) |
| Çadır önerisi, kapasite kontrolü, bekleme listesi, çadır planı verisi | ✅ Hazır (iş katmanı) |
| Parti ve son kullanma tarihli depo (en yakın SKT önce), dağıtıma ayırma, kısmi teslimat, gün sonu | ✅ Hazır (iş katmanı) |
| Stok tahmini, kritik seviye ve SKT uyarıları | ✅ Hazır (iş katmanı) |
| Talep ve görev takibi, ilk yönetici kurulumu ve giriş | ⏳ Planlandı (Aşama 3d) |
| Yerel yapay zeka (Ollama) ile talep sınıflandırma ve günlük özet | ⏳ Planlandı (Aşama 4) |
| DevExpress ekranları, renkli çadır planı, aile fişi, kiosk, gönüllü/vardiya, yakın bulma, raporlar (PDF/Excel), işlem geçmişi ekranı, yedekleme | ⏳ Planlandı (Aşama 5–8) |

## Kullanılan teknolojiler

| Alan | Teknoloji |
|---|---|
| Geliştirme ortamı | Visual Studio 2026, C# |
| Platform | .NET 10 (LTS), Windows Forms |
| Arayüz | DevExpress WinForms (XtraForm, Ribbon, GridControl, LayoutControl, TileControl, XtraReports …) |
| Veritabanı | SQL Server LocalDB |
| Veri erişimi | Entity Framework Core 10 (migration + seed) |
| Yapay zeka | Ollama (yerel model, `HttpClient` ile `http://localhost:11434`) |
| Parola güvenliği | BCrypt.Net-Next |
| Loglama | Serilog (dosya) |
| Test | xUnit |

## Proje yapısı

```
src/AfetCadirKent.Entities     Entity sınıfları ve enum'lar
src/AfetCadirKent.DataAccess   EF Core DbContext, migration'lar, seed, generic repository
src/AfetCadirKent.Business     İş kuralları, servisler, yapay zeka servisi
src/AfetCadirKent.UI           DevExpress WinForms arayüzü
tests/AfetCadirKent.Tests      xUnit testleri
docs/                          Gereksinimler, mimari, kararlar, kurulum
```

## Kurulum (özet)

Ayrıntılı adımlar: [docs/SETUP.md](docs/SETUP.md)

1. Visual Studio 2026 (".NET masaüstü geliştirme" iş yükü) ve .NET 10 SDK kurulu olmalı. SQL Server LocalDB Visual Studio ile gelir.
2. DevExpress WinForms bileşenlerini kurun (devexpress.com/products/try – 30 günlük deneme).
3. Ayar dosyasını oluşturun:
   `src/AfetCadirKent.UI/appsettings.example.json` → aynı klasöre `appsettings.json` olarak kopyalayın ve gerekirse düzenleyin. Bu dosya `.gitignore` ile repoya yüklenmez.
4. Ollama'yı kurun (ollama.com) ve modeli indirin:
   ```
   ollama pull qwen2.5:7b
   ```
5. Veritabanını oluşturun (SQL Server LocalDB):
   ```
   dotnet tool restore
   dotnet ef database update --project src/AfetCadirKent.DataAccess
   ```
   Bağlantı dizesi `appsettings.json` dosyasından (veya `AFETCADIRKENT_DB` ortam değişkeninden) okunur. Aşama 5'ten itibaren uygulama açılışta migration'ları kendisi uygulayacak ve veritabanı boşsa demo verisini yükleyecektir; o zaman bu adım gerekmez.
6. Çözümü (`AfetCadirKent.slnx`) Visual Studio'da açıp `AfetCadirKent.UI` projesini başlatın. *Şu an arayüz olmadığı için uygulama yalnızca ayar dosyasını kontrol edip kapanır.*

## İlk açılış

> Aşama 5'te gelecek.

Demo verisi kullanıcı içermez ve kodda hiçbir parola bulunmaz. Uygulama ilk açıldığında **"İlk yönetici hesabını oluştur"** ekranı çıkar. Yönetici (Koordinatör) giriş yaptıktan sonra Ayarlar ekranından diğer rolleri (kayıt görevlisi, depo sorumlusu, sağlık personeli, saha görevlisi, kiosk) ekler.

## Testleri çalıştırma

```
dotnet test
```
Testler ayrı bir LocalDB test veritabanı (`AfetCadirKent_Test`) kullanır ve her çalıştırmada onu yeniden kurar; geliştirme veritabanına dokunmaz. Ayrıntı: [docs/TESTING.md](docs/TESTING.md).

## Yapay zeka kullanım beyanı

- **Uygulama özelliği:** Talep sınıflandırma ve günlük özet, bilgisayarda yerel çalışan bir Ollama modeliyle yapılır; veriler bilgisayar dışına çıkmaz. Yapay zeka tıbbi aciliyet belirlemez; son karar her zaman görevlidedir.
- **Geliştirme:** Bu projenin geliştirilmesinde kod yardımcısı olarak **Claude Code (Anthropic)** kullanılmıştır.

## Dokümanlar

[Gereksinimler](docs/REQUIREMENTS.md) · [Mimari](docs/ARCHITECTURE.md) · [Kararlar](docs/PROJECT_DECISIONS.md) · [Durum](docs/PROJECT_STATUS.md) · [Kurulum](docs/SETUP.md) · [Test](docs/TESTING.md)
