namespace AfetCadirKent.Entities.Ortak;

/// <summary>Tüm entity'lerin ortak tabanı: tamsayı birincil anahtar.</summary>
public abstract class VarlikTemel
{
    public int Id { get; set; }
}

/// <summary>Aktif/pasif durumu olan kayıtlar.</summary>
public interface IAktiflik
{
    bool Aktif { get; set; }
}

/// <summary>
/// Fiziksel olarak silinmesi yasak olan kayıtlar (kişi, aile, dağıtım, stok hareketi).
/// Bu kayıtlar yalnızca pasif yapılır; geçmiş korunur.
/// </summary>
public interface IFizikselSilinemez : IAktiflik
{
}
