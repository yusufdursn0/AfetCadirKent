# Afet Çadır Kent Yönetim Otomasyonu

Afet sonrası kurulan bir çadır kentte afetzedeleri, aileleri, çadırları, yardım malzemelerini, talepleri ve gönüllüleri tek bir masaüstü uygulamasından yönetmek için geliştirilen Windows Forms uygulaması.

**Ana akış:** Aile kaydı → Uygun çadır → İhtiyaç belirleme → Stoktan teslim → Takip

> Nesne Tabanlı Programlama (2. sınıf) dönem projesidir. Proje geliştirme aşamasındadır; güncel durum için [docs/PROJECT_STATUS.md](docs/PROJECT_STATUS.md).

## Öne çıkan özellikler

- Aile ve kişi kaydı, mükerrer kayıt kontrolü (Türkçe karakter farkları yok sayılır), aileden ayırma, aile fişi
- Gerekçeli öncelik: ailenin **destek önceliği** ve **malzemeye özel ihtiyaçları** ayrı ayrı hesaplanır
- Çadır önerisi, kapasite kontrolü, bekleme listesi, renkli çadır planı
- Parti ve son kullanma tarihli depo (en yakın SKT önce), dağıtıma ayırma, kısmi teslimat
- Stok tahmini, kritik seviye ve SKT uyarıları
- Talep ve görev takibi; yerel yapay zeka (Ollama) ile talep sınıflandırma ve günlük özet
- Kiosk ekranı, gönüllü ve vardiya yönetimi, yakın bulma, raporlar (PDF/Excel), işlem geçmişi, yedekleme

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
5. Çözümü (`AfetCadirKent.slnx`) Visual Studio'da açıp `AfetCadirKent.UI` projesini başlatın.

## İlk açılış

Demo verisi kullanıcı içermez ve kodda hiçbir parola bulunmaz. Uygulama ilk açıldığında **"İlk yönetici hesabını oluştur"** ekranı çıkar. Yönetici (Koordinatör) giriş yaptıktan sonra Ayarlar ekranından diğer rolleri (kayıt görevlisi, depo sorumlusu, sağlık personeli, saha görevlisi, kiosk) ekler.

## Testleri çalıştırma

```
dotnet test
```

## Yapay zeka kullanım beyanı

- **Uygulama özelliği:** Talep sınıflandırma ve günlük özet, bilgisayarda yerel çalışan bir Ollama modeliyle yapılır; veriler bilgisayar dışına çıkmaz. Yapay zeka tıbbi aciliyet belirlemez; son karar her zaman görevlidedir.
- **Geliştirme:** Bu projenin geliştirilmesinde kod yardımcısı olarak **Claude Code (Anthropic)** kullanılmıştır.

## Dokümanlar

[Gereksinimler](docs/REQUIREMENTS.md) · [Mimari](docs/ARCHITECTURE.md) · [Kararlar](docs/PROJECT_DECISIONS.md) · [Durum](docs/PROJECT_STATUS.md)
