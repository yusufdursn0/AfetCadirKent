# Kurulum

## 1. Gereksinimler
- Windows 10/11
- Visual Studio 2026 – ".NET masaüstü geliştirme" ve "Veri depolama ve işleme" iş yükleri (SQL Server LocalDB bununla gelir)
- .NET 10 SDK (`dotnet --list-sdks` ile kontrol edin)
- DevExpress WinForms (deneme veya lisanslı sürüm)
- Ollama (yapay zeka özellikleri için; yoksa uygulama çalışır, talepler "Sınıflandırılmamış" kalır)

## 2. LocalDB kontrolü
```
sqllocaldb info
```
Çıktıda `MSSQLLocalDB` görünmelidir. Görünmüyorsa: `sqllocaldb create MSSQLLocalDB`.

## 3. Ayar dosyası
`src/AfetCadirKent.UI/appsettings.example.json` dosyasını aynı klasöre `appsettings.json` adıyla kopyalayın.

| Ayar | Açıklama |
|---|---|
| `ConnectionStrings:AfetDb` | Veritabanı bağlantısı. Varsayılan LocalDB, Windows kimlik doğrulaması (parola yok). |
| `Ollama:Adres` | Ollama adresi, varsayılan `http://localhost:11434` |
| `Ollama:Model` | Kullanılacak model adı (ör. `qwen2.5:7b`) |
| `Ollama:ZamanAsimiSaniye` | Yapay zeka cevabı için bekleme süresi |
| `Uygulama:SktUyariGunu` | Son kullanma tarihi kaç gün kala uyarı verilsin |
| `Uygulama:YedekKlasoru` | Yedeklerin alınacağı klasör |

`appsettings.json` repoya yüklenmez (`.gitignore`).

## 4. Ollama
1. ollama.com adresinden Ollama'yı kurun.
2. Modeli indirin: `ollama pull qwen2.5:7b`
3. Çalıştığını kontrol edin: tarayıcıda `http://localhost:11434` → "Ollama is running".

## 5. Derleme ve çalıştırma
```
dotnet build AfetCadirKent.slnx
```
Visual Studio'da `AfetCadirKent.UI` başlangıç projesi olarak seçilip çalıştırılır.
