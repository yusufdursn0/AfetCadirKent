namespace AfetCadirKent.Business.Ortak;

/// <summary>Oturumdaki kullanıcının bu işlem için izni yok.</summary>
public class YetkisizIslemException(string mesaj = "Bu işlem için yetkiniz yok.") : IsKuraliException(mesaj);

/// <summary>Kullanılmış bir kayıt silinmek istendi (şartname: "Ekle / Listele / Güncelle / Sil").</summary>
public class SilinemezException() : IsKuraliException("Bu kayıt kullanıldığı için silinemez, pasif yapabilirsiniz.");

/// <summary>İyimser eşzamanlılık çakışması (RowVersion).</summary>
public class EszamanlilikException() : IsKuraliException(
    "Bu kayıt siz işlem yaparken başka bir kullanıcı tarafından değiştirildi. Liste yenilendi, lütfen tekrar deneyin.");

/// <summary>Aranan kayıt bulunamadı (silinmiş/pasif ya da kullanıcının görme yetkisi dışında).</summary>
public class KayitBulunamadiException(string kayit) : IsKuraliException($"{kayit} bulunamadı.");
