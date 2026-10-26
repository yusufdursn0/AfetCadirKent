using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;

namespace AfetCadirKent.UI.Altyapi;

public static class GridYardimcisi
{
    /// <summary>Satır yokken grid'in ortasında açıklayıcı metin gösterir (CLAUDE.md 4.5 "Empty state").</summary>
    public static void BosMetinGoster(GridView gorunum, string metin)
    {
        gorunum.CustomDrawEmptyForeground += (_, e) =>
        {
            if (gorunum.RowCount > 0)
                return;
            e.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
            e.Appearance.TextOptions.VAlignment = VertAlignment.Center;
            e.Appearance.DrawString(e.Cache, metin, e.Bounds);
        };
    }
}
