using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Localization;

namespace AfetCadirKent.UI.Altyapi;

/// <summary>
/// DevExpress'in hazır Türkçe kaynak paketi kurulu olmadığından ekranda görünen ortak metinler burada Türkçeleştirilir.
/// Listede olmayan metinler DevExpress'in varsayılanıyla gösterilir.
/// </summary>
public static class TurkceYerellestirme
{
    public static void Uygula()
    {
        Localizer.Active = new EditorMetinleri();
        GridLocalizer.Active = new GridMetinleri();
    }

    private sealed class EditorMetinleri : Localizer
    {
        public override string GetLocalizedString(StringId id) => id switch
        {
            StringId.XtraMessageBoxOkButtonText => "Tamam",
            StringId.XtraMessageBoxCancelButtonText => "Vazgeç",
            StringId.XtraMessageBoxYesButtonText => "Evet",
            StringId.XtraMessageBoxNoButtonText => "Hayır",
            StringId.XtraMessageBoxAbortButtonText => "Durdur",
            StringId.XtraMessageBoxRetryButtonText => "Yeniden dene",
            StringId.XtraMessageBoxIgnoreButtonText => "Yok say",
            StringId.OK => "Tamam",
            StringId.Cancel => "Vazgeç",
            StringId.TextEditMenuCopy => "Kopyala",
            StringId.TextEditMenuCut => "Kes",
            StringId.TextEditMenuPaste => "Yapıştır",
            StringId.TextEditMenuDelete => "Sil",
            StringId.TextEditMenuSelectAll => "Tümünü seç",
            StringId.TextEditMenuUndo => "Geri al",
            StringId.DateEditToday => "Bugün",
            StringId.DateEditClear => "Temizle",
            _ => base.GetLocalizedString(id),
        };
    }

    private sealed class GridMetinleri : GridLocalizer
    {
        public override string GetLocalizedString(GridStringId id) => id switch
        {
            GridStringId.GridGroupPanelText => "Gruplamak için bir sütun başlığını buraya sürükleyin",
            GridStringId.FindControlFindButton => "Bul",
            GridStringId.FindControlClearButton => "Temizle",
            GridStringId.FindNullPrompt => "Aranacak metni yazın…",
            GridStringId.MenuColumnSortAscending => "Artan sırala",
            GridStringId.MenuColumnSortDescending => "Azalan sırala",
            GridStringId.MenuColumnClearSorting => "Sıralamayı kaldır",
            GridStringId.MenuColumnGroup => "Bu sütuna göre grupla",
            GridStringId.MenuColumnUnGroup => "Gruplamayı kaldır",
            GridStringId.MenuColumnBestFit => "En uygun genişlik",
            GridStringId.MenuColumnBestFitAllColumns => "Tüm sütunlarda en uygun genişlik",
            GridStringId.MenuColumnFindFilterShow => "Arama panelini göster",
            GridStringId.MenuColumnFindFilterHide => "Arama panelini gizle",
            GridStringId.MenuColumnAutoFilterRowShow => "Filtre satırını göster",
            GridStringId.MenuColumnAutoFilterRowHide => "Filtre satırını gizle",
            GridStringId.MenuColumnClearFilter => "Filtreyi kaldır",
            GridStringId.MenuColumnFilterEditor => "Filtre düzenleyici…",
            GridStringId.MenuColumnColumnCustomization => "Sütun seçici",
            GridStringId.MenuColumnRemoveColumn => "Sütunu gizle",
            _ => base.GetLocalizedString(id),
        };
    }
}
