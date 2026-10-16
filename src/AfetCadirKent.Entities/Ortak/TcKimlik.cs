namespace AfetCadirKent.Entities.Ortak;

/// <summary>T.C. kimlik numarası doğrulaması (11 hane, ilk hane 0 değil, 10. ve 11. hane kontrolleri).</summary>
public static class TcKimlik
{
    public static bool GecerliMi(string? tc)
    {
        if (tc is null || tc.Length != 11 || !tc.All(char.IsDigit) || tc[0] == '0')
            return false;

        var h = tc.Select(c => c - '0').ToArray();
        var tekToplam = h[0] + h[2] + h[4] + h[6] + h[8];
        var ciftToplam = h[1] + h[3] + h[5] + h[7];
        var onuncu = ((tekToplam * 7 - ciftToplam) % 10 + 10) % 10;
        if (h[9] != onuncu)
            return false;

        var onbirinci = h.Take(10).Sum() % 10;
        return h[10] == onbirinci;
    }

    /// <summary>İlk 9 haneden kontrol haneleri hesaplanmış geçerli bir numara üretir (demo verisi ve testler için).</summary>
    public static string KontrolHaneleriyleTamamla(string ilkDokuzHane)
    {
        if (ilkDokuzHane.Length != 9 || !ilkDokuzHane.All(char.IsDigit) || ilkDokuzHane[0] == '0')
            throw new ArgumentException("İlk 9 hane, 0 ile başlamayan 9 rakam olmalıdır.", nameof(ilkDokuzHane));

        var h = ilkDokuzHane.Select(c => c - '0').ToList();
        var onuncu = (((h[0] + h[2] + h[4] + h[6] + h[8]) * 7 - (h[1] + h[3] + h[5] + h[7])) % 10 + 10) % 10;
        h.Add(onuncu);
        h.Add(h.Sum() % 10);
        return string.Concat(h);
    }
}
