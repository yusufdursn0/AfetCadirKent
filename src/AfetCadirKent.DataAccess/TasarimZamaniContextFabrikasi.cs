using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AfetCadirKent.DataAccess;

/// <summary>
/// "dotnet ef" komutları için DbContext üretir. Bağlantı dizesi koda yazılmaz:
/// önce AFETCADIRKENT_DB ortam değişkenine, yoksa UI projesindeki appsettings.json dosyasına bakılır.
/// </summary>
public class TasarimZamaniContextFabrikasi : IDesignTimeDbContextFactory<AfetDbContext>
{
    public const string OrtamDegiskeni = "AFETCADIRKENT_DB";

    public AfetDbContext CreateDbContext(string[] args)
    {
        var baglanti = Environment.GetEnvironmentVariable(OrtamDegiskeni) ?? AyarDosyasindanOku()
            ?? throw new InvalidOperationException(
                $"Bağlantı dizesi bulunamadı. {OrtamDegiskeni} ortam değişkenini tanımlayın veya " +
                "src/AfetCadirKent.UI/appsettings.json dosyasını oluşturun (bkz. docs/SETUP.md).");

        var options = new DbContextOptionsBuilder<AfetDbContext>().UseSqlServer(baglanti).Options;
        return new AfetDbContext(options);
    }

    private static string? AyarDosyasindanOku()
    {
        string[] adaylar =
        [
            Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "AfetCadirKent.UI", "appsettings.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "src", "AfetCadirKent.UI", "appsettings.json"),
        ];

        var dosya = adaylar.FirstOrDefault(File.Exists);
        if (dosya is null)
            return null;

        using var belge = JsonDocument.Parse(File.ReadAllText(dosya));
        return belge.RootElement.TryGetProperty("ConnectionStrings", out var cs) && cs.TryGetProperty("AfetDb", out var db)
            ? db.GetString()
            : null;
    }
}
