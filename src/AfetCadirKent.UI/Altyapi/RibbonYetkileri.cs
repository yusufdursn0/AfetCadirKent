using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Enumlar;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;

namespace AfetCadirKent.UI.Altyapi;

/// <summary>
/// Rol bazlı menü: her Ribbon öğesi bir izin kuralıyla eşlenir; kuralı sağlamayan öğe gizlenir, öğesi kalmayan grup ve
/// sayfa da gizlenir. Bu yalnızca görünümdür; Business her işlemde izni ayrıca kontrol eder (REQ-036).
/// </summary>
public sealed class RibbonYetkileri
{
    private readonly Dictionary<BarItem, Func<IOturum, bool>> _kurallar = [];

    /// <summary>Öğe, oturumdaki rolün bu modülde bu izni varsa görünür.</summary>
    public RibbonYetkileri Izin(BarItem oge, Modul modul, Izin izin) => Kural(oge, o => o.IzinVarMi(modul, izin));

    /// <summary>Öğe yalnızca belirtilen rollerde görünür (ör. günlük özet yalnızca Koordinatör).</summary>
    public RibbonYetkileri Rol(BarItem oge, params RolTuru[] roller) => Kural(oge, o => o.Rol is { } r && roller.Contains(r));

    public RibbonYetkileri Kural(BarItem oge, Func<IOturum, bool> kural)
    {
        _kurallar[oge] = kural;
        return this;
    }

    public void Uygula(RibbonControl ribbon, IOturum oturum)
    {
        foreach (var (oge, kural) in _kurallar)
            oge.Visibility = kural(oturum) ? BarItemVisibility.Always : BarItemVisibility.Never;

        foreach (RibbonPage sayfa in ribbon.Pages)
        {
            foreach (RibbonPageGroup grup in sayfa.Groups)
                grup.Visible = grup.ItemLinks.Cast<BarItemLink>().Any(l => l.Item.Visibility != BarItemVisibility.Never);
            sayfa.Visible = sayfa.Groups.Cast<RibbonPageGroup>().Any(g => g.Visible);
        }
    }
}
