namespace AfetCadirKent.UI.Formlar
{
    partial class StokDurumuForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.tabControl = new DevExpress.XtraTab.XtraTabControl();
            this.tabStok = new DevExpress.XtraTab.XtraTabPage();
            this.tabHareketler = new DevExpress.XtraTab.XtraTabPage();
            this.tabUyarilar = new DevExpress.XtraTab.XtraTabPage();
            this.layoutStok = new DevExpress.XtraLayout.LayoutControl();
            this.lblStokOzet = new DevExpress.XtraEditors.LabelControl();
            this.btnYenile = new DevExpress.XtraEditors.SimpleButton();
            this.gridUrunStok = new DevExpress.XtraGrid.GridControl();
            this.viewUrunStok = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colUUrun = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUBirim = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUFiziksel = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUAyrilan = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUKullanilabilir = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUSktGecmis = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUKritik = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUDurum = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridPartiler = new DevExpress.XtraGrid.GridControl();
            this.viewPartiler = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colPSira = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPParti = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPBagisci = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPGiris = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPSkt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPMiktar = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPAyrilan = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPKullanilabilir = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPDurum = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbIslem = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.spnIslemMiktar = new DevExpress.XtraEditors.SpinEdit();
            this.txtIslemAciklama = new DevExpress.XtraEditors.TextEdit();
            this.btnIslemUygula = new DevExpress.XtraEditors.SimpleButton();
            this.lciStokOzet = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciYenile = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciUrunStok = new DevExpress.XtraLayout.LayoutControlItem();
            this.grpUrunStok = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciPartiler = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIslem = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIslemMiktar = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIslemAciklama = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIslemUygula = new DevExpress.XtraLayout.LayoutControlItem();
            this.grpPartiler = new DevExpress.XtraLayout.LayoutControlGroup();
            this.RootStok = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutHareket = new DevExpress.XtraLayout.LayoutControl();
            this.cmbHUrun = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.cmbHTur = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.dteHBaslangic = new DevExpress.XtraEditors.DateEdit();
            this.dteHBitis = new DevExpress.XtraEditors.DateEdit();
            this.chkHPasif = new DevExpress.XtraEditors.CheckEdit();
            this.btnListele = new DevExpress.XtraEditors.SimpleButton();
            this.gridHareketler = new DevExpress.XtraGrid.GridControl();
            this.viewHareketler = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colHZaman = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colHTur = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colHUrun = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colHParti = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colHMiktar = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colHEtki = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colHAile = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colHTeslimAlan = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colHAciklama = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colHKullanici = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colHKayit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnHareketPasif = new DevExpress.XtraEditors.SimpleButton();
            this.lciHUrun = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHTur = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHBaslangic = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHBitis = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHPasif = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciListele = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHareketler = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHareketPasif = new DevExpress.XtraLayout.LayoutControlItem();
            this.bosHareket = new DevExpress.XtraLayout.EmptySpaceItem();
            this.RootHareket = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutUyari = new DevExpress.XtraLayout.LayoutControl();
            this.gridUyarilar = new DevExpress.XtraGrid.GridControl();
            this.viewUyarilar = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colUyTur = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUyUrun = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUyMesaj = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnFireyeDus = new DevExpress.XtraEditors.SimpleButton();
            this.lblTahminBilgi = new DevExpress.XtraEditors.LabelControl();
            this.gridTahmin = new DevExpress.XtraGrid.GridControl();
            this.viewTahmin = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colTUrun = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTKullanilabilir = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTTuketimGunluk = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTTuketimGun = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTHakGunluk = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTHakGun = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTDonem = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTAciklama = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lciUyarilar = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciFireyeDus = new DevExpress.XtraLayout.LayoutControlItem();
            this.bosUyari = new DevExpress.XtraLayout.EmptySpaceItem();
            this.grpUyarilar = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciTahminBilgi = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTahmin = new DevExpress.XtraLayout.LayoutControlItem();
            this.grpTahmin = new DevExpress.XtraLayout.LayoutControlGroup();
            this.RootUyari = new DevExpress.XtraLayout.LayoutControlGroup();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabStok.SuspendLayout();
            this.tabHareketler.SuspendLayout();
            this.tabUyarilar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutStok)).BeginInit();
            this.layoutStok.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridUrunStok)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewUrunStok)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridPartiler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewPartiler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbIslem.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnIslemMiktar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIslemAciklama.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciStokOzet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYenile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciUrunStok)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpUrunStok)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPartiler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIslem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIslemMiktar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIslemAciklama)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIslemUygula)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpPartiler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootStok)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutHareket)).BeginInit();
            this.layoutHareket.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbHUrun.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbHTur.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteHBaslangic.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteHBaslangic.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteHBitis.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteHBitis.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkHPasif.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridHareketler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewHareketler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHUrun)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHTur)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHBaslangic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHBitis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHPasif)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciListele)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHareketler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHareketPasif)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosHareket)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootHareket)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutUyari)).BeginInit();
            this.layoutUyari.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridUyarilar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewUyarilar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridTahmin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewTahmin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciUyarilar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFireyeDus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosUyari)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpUyarilar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTahminBilgi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTahmin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpTahmin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootUyari)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedTabPage = this.tabStok;
            this.tabControl.Size = new System.Drawing.Size(1280, 760);
            this.tabControl.TabIndex = 0;
            this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabStok,
            this.tabHareketler,
            this.tabUyarilar});
            // 
            // tabStok
            // 
            this.tabStok.Controls.Add(this.layoutStok);
            this.tabStok.Name = "tabStok";
            this.tabStok.Size = new System.Drawing.Size(1278, 735);
            this.tabStok.Text = "Stok ve partiler";
            // 
            // tabHareketler
            // 
            this.tabHareketler.Controls.Add(this.layoutHareket);
            this.tabHareketler.Name = "tabHareketler";
            this.tabHareketler.Size = new System.Drawing.Size(1278, 735);
            this.tabHareketler.Text = "Stok hareketleri";
            // 
            // tabUyarilar
            // 
            this.tabUyarilar.Controls.Add(this.layoutUyari);
            this.tabUyarilar.Name = "tabUyarilar";
            this.tabUyarilar.Size = new System.Drawing.Size(1278, 735);
            this.tabUyarilar.Text = "Uyarılar ve stok tahmini";
            // 
            // layoutStok
            // 
            this.layoutStok.Controls.Add(this.lblStokOzet);
            this.layoutStok.Controls.Add(this.btnYenile);
            this.layoutStok.Controls.Add(this.gridUrunStok);
            this.layoutStok.Controls.Add(this.gridPartiler);
            this.layoutStok.Controls.Add(this.cmbIslem);
            this.layoutStok.Controls.Add(this.spnIslemMiktar);
            this.layoutStok.Controls.Add(this.txtIslemAciklama);
            this.layoutStok.Controls.Add(this.btnIslemUygula);
            this.layoutStok.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutStok.Location = new System.Drawing.Point(0, 0);
            this.layoutStok.Name = "layoutStok";
            this.layoutStok.Root = this.RootStok;
            this.layoutStok.Size = new System.Drawing.Size(1278, 735);
            this.layoutStok.TabIndex = 0;
            // 
            // lblStokOzet
            // 
            this.lblStokOzet.Location = new System.Drawing.Point(12, 12);
            this.lblStokOzet.Name = "lblStokOzet";
            this.lblStokOzet.AllowHtmlString = true;
            this.lblStokOzet.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblStokOzet.Text = "";
            this.lblStokOzet.Size = new System.Drawing.Size(1094, 22);
            this.lblStokOzet.StyleController = this.layoutStok;
            this.lblStokOzet.TabIndex = 1;
            // 
            // btnYenile
            // 
            this.btnYenile.Location = new System.Drawing.Point(1110, 12);
            this.btnYenile.Name = "btnYenile";
            this.btnYenile.Text = "Yenile";
            this.btnYenile.Size = new System.Drawing.Size(156, 22);
            this.btnYenile.StyleController = this.layoutStok;
            this.btnYenile.TabIndex = 2;
            this.btnYenile.Click += new System.EventHandler(this.btnYenile_Click);
            // 
            // gridUrunStok
            // 
            this.gridUrunStok.Location = new System.Drawing.Point(24, 59);
            this.gridUrunStok.MainView = this.viewUrunStok;
            this.gridUrunStok.Name = "gridUrunStok";
            this.gridUrunStok.Size = new System.Drawing.Size(1230, 251);
            this.gridUrunStok.TabIndex = 3;
            this.gridUrunStok.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewUrunStok});
            // 
            // viewUrunStok
            // 
            this.viewUrunStok.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colUUrun,
            this.colUBirim,
            this.colUFiziksel,
            this.colUAyrilan,
            this.colUKullanilabilir,
            this.colUSktGecmis,
            this.colUKritik,
            this.colUDurum});
            this.viewUrunStok.GridControl = this.gridUrunStok;
            this.viewUrunStok.Name = "viewUrunStok";
            this.viewUrunStok.OptionsBehavior.Editable = false;
            this.viewUrunStok.OptionsView.ShowGroupPanel = false;
            this.viewUrunStok.OptionsView.ShowIndicator = false;
            this.viewUrunStok.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.viewUrunStok_FocusedRowChanged);
            // 
            // colUUrun
            // 
            this.colUUrun.Caption = "Ürün";
            this.colUUrun.FieldName = "UrunAdi";
            this.colUUrun.Name = "colUUrun";
            this.colUUrun.Visible = true;
            this.colUUrun.VisibleIndex = 0;
            this.colUUrun.Width = 200;
            // 
            // colUBirim
            // 
            this.colUBirim.Caption = "Birim";
            this.colUBirim.FieldName = "Birim";
            this.colUBirim.Name = "colUBirim";
            this.colUBirim.Visible = true;
            this.colUBirim.VisibleIndex = 1;
            this.colUBirim.Width = 60;
            // 
            // colUFiziksel
            // 
            this.colUFiziksel.Caption = "Depodaki (fiziksel)";
            this.colUFiziksel.FieldName = "FizikselMiktar";
            this.colUFiziksel.Name = "colUFiziksel";
            this.colUFiziksel.DisplayFormat.FormatString = "#,##0.##";
            this.colUFiziksel.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colUFiziksel.Visible = true;
            this.colUFiziksel.VisibleIndex = 2;
            this.colUFiziksel.Width = 110;
            // 
            // colUAyrilan
            // 
            this.colUAyrilan.Caption = "Dağıtıma ayrılan (teslim bekliyor)";
            this.colUAyrilan.FieldName = "AyrilanMiktar";
            this.colUAyrilan.Name = "colUAyrilan";
            this.colUAyrilan.DisplayFormat.FormatString = "#,##0.##";
            this.colUAyrilan.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colUAyrilan.Visible = true;
            this.colUAyrilan.VisibleIndex = 3;
            this.colUAyrilan.Width = 170;
            // 
            // colUKullanilabilir
            // 
            this.colUKullanilabilir.Caption = "Kullanılabilir";
            this.colUKullanilabilir.FieldName = "KullanilabilirMiktar";
            this.colUKullanilabilir.Name = "colUKullanilabilir";
            this.colUKullanilabilir.DisplayFormat.FormatString = "#,##0.##";
            this.colUKullanilabilir.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colUKullanilabilir.Visible = true;
            this.colUKullanilabilir.VisibleIndex = 4;
            this.colUKullanilabilir.Width = 100;
            // 
            // colUSktGecmis
            // 
            this.colUSktGecmis.Caption = "SKT geçmiş";
            this.colUSktGecmis.FieldName = "SktGecmisMiktar";
            this.colUSktGecmis.Name = "colUSktGecmis";
            this.colUSktGecmis.DisplayFormat.FormatString = "#,##0.##";
            this.colUSktGecmis.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colUSktGecmis.Visible = true;
            this.colUSktGecmis.VisibleIndex = 5;
            this.colUSktGecmis.Width = 90;
            // 
            // colUKritik
            // 
            this.colUKritik.Caption = "Kritik seviye";
            this.colUKritik.FieldName = "KritikSeviye";
            this.colUKritik.Name = "colUKritik";
            this.colUKritik.DisplayFormat.FormatString = "#,##0.##";
            this.colUKritik.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colUKritik.Visible = true;
            this.colUKritik.VisibleIndex = 6;
            this.colUKritik.Width = 90;
            // 
            // colUDurum
            // 
            this.colUDurum.Caption = "Durum";
            this.colUDurum.FieldName = "Durum";
            this.colUDurum.Name = "colUDurum";
            this.colUDurum.Visible = true;
            this.colUDurum.VisibleIndex = 7;
            this.colUDurum.Width = 160;
            // 
            // gridPartiler
            // 
            this.gridPartiler.Location = new System.Drawing.Point(24, 356);
            this.gridPartiler.MainView = this.viewPartiler;
            this.gridPartiler.Name = "gridPartiler";
            this.gridPartiler.Size = new System.Drawing.Size(1230, 318);
            this.gridPartiler.TabIndex = 4;
            this.gridPartiler.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewPartiler});
            // 
            // viewPartiler
            // 
            this.viewPartiler.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colPSira,
            this.colPParti,
            this.colPBagisci,
            this.colPGiris,
            this.colPSkt,
            this.colPMiktar,
            this.colPAyrilan,
            this.colPKullanilabilir,
            this.colPDurum});
            this.viewPartiler.GridControl = this.gridPartiler;
            this.viewPartiler.Name = "viewPartiler";
            this.viewPartiler.OptionsBehavior.Editable = false;
            this.viewPartiler.OptionsView.ShowGroupPanel = false;
            this.viewPartiler.OptionsView.ShowIndicator = false;
            this.viewPartiler.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.viewPartiler_FocusedRowChanged);
            // 
            // colPSira
            // 
            this.colPSira.Caption = "Dağıtım sırası";
            this.colPSira.FieldName = "FefoSirasi";
            this.colPSira.Name = "colPSira";
            this.colPSira.Visible = true;
            this.colPSira.VisibleIndex = 0;
            this.colPSira.Width = 90;
            // 
            // colPParti
            // 
            this.colPParti.Caption = "Parti";
            this.colPParti.FieldName = "PartiNo";
            this.colPParti.Name = "colPParti";
            this.colPParti.Visible = true;
            this.colPParti.VisibleIndex = 1;
            this.colPParti.Width = 110;
            // 
            // colPBagisci
            // 
            this.colPBagisci.Caption = "Bağışçı";
            this.colPBagisci.FieldName = "BagisciAdi";
            this.colPBagisci.Name = "colPBagisci";
            this.colPBagisci.Visible = true;
            this.colPBagisci.VisibleIndex = 2;
            this.colPBagisci.Width = 150;
            // 
            // colPGiris
            // 
            this.colPGiris.Caption = "Giriş";
            this.colPGiris.FieldName = "GirisZamani";
            this.colPGiris.Name = "colPGiris";
            this.colPGiris.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";
            this.colPGiris.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colPGiris.Visible = true;
            this.colPGiris.VisibleIndex = 3;
            this.colPGiris.Width = 110;
            // 
            // colPSkt
            // 
            this.colPSkt.Caption = "SKT";
            this.colPSkt.FieldName = "Skt";
            this.colPSkt.Name = "colPSkt";
            this.colPSkt.DisplayFormat.FormatString = "dd.MM.yyyy";
            this.colPSkt.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colPSkt.Visible = true;
            this.colPSkt.VisibleIndex = 4;
            this.colPSkt.Width = 80;
            // 
            // colPMiktar
            // 
            this.colPMiktar.Caption = "Depodaki";
            this.colPMiktar.FieldName = "Miktar";
            this.colPMiktar.Name = "colPMiktar";
            this.colPMiktar.DisplayFormat.FormatString = "#,##0.##";
            this.colPMiktar.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colPMiktar.Visible = true;
            this.colPMiktar.VisibleIndex = 5;
            this.colPMiktar.Width = 80;
            // 
            // colPAyrilan
            // 
            this.colPAyrilan.Caption = "Ayrılan";
            this.colPAyrilan.FieldName = "AyrilanMiktar";
            this.colPAyrilan.Name = "colPAyrilan";
            this.colPAyrilan.DisplayFormat.FormatString = "#,##0.##";
            this.colPAyrilan.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colPAyrilan.Visible = true;
            this.colPAyrilan.VisibleIndex = 6;
            this.colPAyrilan.Width = 80;
            // 
            // colPKullanilabilir
            // 
            this.colPKullanilabilir.Caption = "Kullanılabilir";
            this.colPKullanilabilir.FieldName = "KullanilabilirMiktar";
            this.colPKullanilabilir.Name = "colPKullanilabilir";
            this.colPKullanilabilir.DisplayFormat.FormatString = "#,##0.##";
            this.colPKullanilabilir.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colPKullanilabilir.Visible = true;
            this.colPKullanilabilir.VisibleIndex = 7;
            this.colPKullanilabilir.Width = 90;
            // 
            // colPDurum
            // 
            this.colPDurum.Caption = "Durum";
            this.colPDurum.FieldName = "Durum";
            this.colPDurum.Name = "colPDurum";
            this.colPDurum.Visible = true;
            this.colPDurum.VisibleIndex = 8;
            this.colPDurum.Width = 160;
            // 
            // cmbIslem
            // 
            this.cmbIslem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbIslem.Location = new System.Drawing.Point(64, 678);
            this.cmbIslem.Name = "cmbIslem";
            this.cmbIslem.Size = new System.Drawing.Size(236, 22);
            this.cmbIslem.StyleController = this.layoutStok;
            this.cmbIslem.TabIndex = 5;
            this.cmbIslem.EditValueChanged += new System.EventHandler(this.cmbIslem_EditValueChanged);
            // 
            // spnIslemMiktar
            // 
            this.spnIslemMiktar.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spnIslemMiktar.Location = new System.Drawing.Point(394, 678);
            this.spnIslemMiktar.Name = "spnIslemMiktar";
            this.spnIslemMiktar.EditValue = new decimal(new int[] {0, 0, 0, 0});
            this.spnIslemMiktar.Properties.MaxValue = new decimal(new int[] {10000000, 0, 0, 0});
            this.spnIslemMiktar.Properties.MinValue = new decimal(new int[] {0, 0, 0, 0});
            this.spnIslemMiktar.Size = new System.Drawing.Size(156, 22);
            this.spnIslemMiktar.StyleController = this.layoutStok;
            this.spnIslemMiktar.TabIndex = 6;
            // 
            // txtIslemAciklama
            // 
            this.txtIslemAciklama.Location = new System.Drawing.Point(614, 678);
            this.txtIslemAciklama.Name = "txtIslemAciklama";
            this.txtIslemAciklama.Properties.MaxLength = 500;
            this.txtIslemAciklama.Size = new System.Drawing.Size(440, 22);
            this.txtIslemAciklama.StyleController = this.layoutStok;
            this.txtIslemAciklama.TabIndex = 7;
            // 
            // btnIslemUygula
            // 
            this.btnIslemUygula.Location = new System.Drawing.Point(1058, 678);
            this.btnIslemUygula.Name = "btnIslemUygula";
            this.btnIslemUygula.Text = "Seçili partiye uygula";
            this.btnIslemUygula.Size = new System.Drawing.Size(196, 22);
            this.btnIslemUygula.StyleController = this.layoutStok;
            this.btnIslemUygula.TabIndex = 8;
            this.btnIslemUygula.Click += new System.EventHandler(this.btnIslemUygula_Click);
            // 
            // lciStokOzet
            // 
            this.lciStokOzet.Control = this.lblStokOzet;
            this.lciStokOzet.Location = new System.Drawing.Point(0, 0);
            this.lciStokOzet.MaxSize = new System.Drawing.Size(0, 26);
            this.lciStokOzet.MinSize = new System.Drawing.Size(200, 26);
            this.lciStokOzet.Name = "lciStokOzet";
            this.lciStokOzet.Size = new System.Drawing.Size(1098, 26);
            this.lciStokOzet.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciStokOzet.TextVisible = false;
            // 
            // lciYenile
            // 
            this.lciYenile.Control = this.btnYenile;
            this.lciYenile.Location = new System.Drawing.Point(1098, 0);
            this.lciYenile.MaxSize = new System.Drawing.Size(160, 26);
            this.lciYenile.MinSize = new System.Drawing.Size(80, 26);
            this.lciYenile.Name = "lciYenile";
            this.lciYenile.Size = new System.Drawing.Size(160, 26);
            this.lciYenile.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciYenile.TextVisible = false;
            // 
            // lciUrunStok
            // 
            this.lciUrunStok.Control = this.gridUrunStok;
            this.lciUrunStok.Location = new System.Drawing.Point(0, 0);
            this.lciUrunStok.MinSize = new System.Drawing.Size(300, 100);
            this.lciUrunStok.Name = "lciUrunStok";
            this.lciUrunStok.Size = new System.Drawing.Size(1234, 255);
            this.lciUrunStok.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciUrunStok.TextVisible = false;
            // 
            // grpUrunStok
            // 
            this.grpUrunStok.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciUrunStok});
            this.grpUrunStok.Location = new System.Drawing.Point(0, 26);
            this.grpUrunStok.Name = "grpUrunStok";
            this.grpUrunStok.Size = new System.Drawing.Size(1258, 300);
            this.grpUrunStok.Text = "Ürün bazında stok (kritik seviyenin altındakiler kırmızı)";
            // 
            // lciPartiler
            // 
            this.lciPartiler.Control = this.gridPartiler;
            this.lciPartiler.Location = new System.Drawing.Point(0, 0);
            this.lciPartiler.MinSize = new System.Drawing.Size(300, 100);
            this.lciPartiler.Name = "lciPartiler";
            this.lciPartiler.Size = new System.Drawing.Size(1234, 318);
            this.lciPartiler.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciPartiler.TextVisible = false;
            // 
            // lciIslem
            // 
            this.lciIslem.Control = this.cmbIslem;
            this.lciIslem.Location = new System.Drawing.Point(0, 318);
            this.lciIslem.MaxSize = new System.Drawing.Size(0, 26);
            this.lciIslem.MinSize = new System.Drawing.Size(80, 26);
            this.lciIslem.Name = "lciIslem";
            this.lciIslem.Size = new System.Drawing.Size(300, 26);
            this.lciIslem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciIslem.Text = "İşlem";
            this.lciIslem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIslem.TextSize = new System.Drawing.Size(40, 13);
            // 
            // lciIslemMiktar
            // 
            this.lciIslemMiktar.Control = this.spnIslemMiktar;
            this.lciIslemMiktar.Location = new System.Drawing.Point(300, 318);
            this.lciIslemMiktar.MaxSize = new System.Drawing.Size(0, 26);
            this.lciIslemMiktar.MinSize = new System.Drawing.Size(80, 26);
            this.lciIslemMiktar.Name = "lciIslemMiktar";
            this.lciIslemMiktar.Size = new System.Drawing.Size(250, 26);
            this.lciIslemMiktar.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciIslemMiktar.Text = "Miktar";
            this.lciIslemMiktar.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIslemMiktar.TextSize = new System.Drawing.Size(90, 13);
            // 
            // lciIslemAciklama
            // 
            this.lciIslemAciklama.Control = this.txtIslemAciklama;
            this.lciIslemAciklama.Location = new System.Drawing.Point(550, 318);
            this.lciIslemAciklama.MaxSize = new System.Drawing.Size(0, 26);
            this.lciIslemAciklama.MinSize = new System.Drawing.Size(80, 26);
            this.lciIslemAciklama.Name = "lciIslemAciklama";
            this.lciIslemAciklama.Size = new System.Drawing.Size(504, 26);
            this.lciIslemAciklama.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciIslemAciklama.Text = "Açıklama";
            this.lciIslemAciklama.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIslemAciklama.TextSize = new System.Drawing.Size(60, 13);
            // 
            // lciIslemUygula
            // 
            this.lciIslemUygula.Control = this.btnIslemUygula;
            this.lciIslemUygula.Location = new System.Drawing.Point(1054, 318);
            this.lciIslemUygula.MaxSize = new System.Drawing.Size(0, 26);
            this.lciIslemUygula.MinSize = new System.Drawing.Size(80, 26);
            this.lciIslemUygula.Name = "lciIslemUygula";
            this.lciIslemUygula.Size = new System.Drawing.Size(180, 26);
            this.lciIslemUygula.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciIslemUygula.TextVisible = false;
            // 
            // grpPartiler
            // 
            this.grpPartiler.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciPartiler,
            this.lciIslem,
            this.lciIslemMiktar,
            this.lciIslemAciklama,
            this.lciIslemUygula});
            this.grpPartiler.Location = new System.Drawing.Point(0, 326);
            this.grpPartiler.Name = "grpPartiler";
            this.grpPartiler.Size = new System.Drawing.Size(1258, 389);
            this.grpPartiler.Text = "Partiler";
            // 
            // RootStok
            // 
            this.RootStok.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.RootStok.GroupBordersVisible = false;
            this.RootStok.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciStokOzet,
            this.lciYenile,
            this.grpUrunStok,
            this.grpPartiler});
            this.RootStok.Name = "RootStok";
            this.RootStok.Size = new System.Drawing.Size(1278, 735);
            this.RootStok.TextVisible = false;
            // 
            // layoutHareket
            // 
            this.layoutHareket.Controls.Add(this.cmbHUrun);
            this.layoutHareket.Controls.Add(this.cmbHTur);
            this.layoutHareket.Controls.Add(this.dteHBaslangic);
            this.layoutHareket.Controls.Add(this.dteHBitis);
            this.layoutHareket.Controls.Add(this.chkHPasif);
            this.layoutHareket.Controls.Add(this.btnListele);
            this.layoutHareket.Controls.Add(this.gridHareketler);
            this.layoutHareket.Controls.Add(this.btnHareketPasif);
            this.layoutHareket.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutHareket.Location = new System.Drawing.Point(0, 0);
            this.layoutHareket.Name = "layoutHareket";
            this.layoutHareket.Root = this.RootHareket;
            this.layoutHareket.Size = new System.Drawing.Size(1278, 735);
            this.layoutHareket.TabIndex = 0;
            // 
            // cmbHUrun
            // 
            this.cmbHUrun.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbHUrun.Location = new System.Drawing.Point(52, 12);
            this.cmbHUrun.Name = "cmbHUrun";
            this.cmbHUrun.Size = new System.Drawing.Size(256, 22);
            this.cmbHUrun.StyleController = this.layoutHareket;
            this.cmbHUrun.TabIndex = 1;
            // 
            // cmbHTur
            // 
            this.cmbHTur.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbHTur.Location = new System.Drawing.Point(342, 12);
            this.cmbHTur.Name = "cmbHTur";
            this.cmbHTur.Size = new System.Drawing.Size(206, 22);
            this.cmbHTur.StyleController = this.layoutHareket;
            this.cmbHTur.TabIndex = 2;
            // 
            // dteHBaslangic
            // 
            this.dteHBaslangic.EditValue = null;
            this.dteHBaslangic.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteHBaslangic.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteHBaslangic.Location = new System.Drawing.Point(612, 12);
            this.dteHBaslangic.Name = "dteHBaslangic";
            this.dteHBaslangic.Size = new System.Drawing.Size(136, 22);
            this.dteHBaslangic.StyleController = this.layoutHareket;
            this.dteHBaslangic.TabIndex = 3;
            // 
            // dteHBitis
            // 
            this.dteHBitis.EditValue = null;
            this.dteHBitis.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteHBitis.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteHBitis.Location = new System.Drawing.Point(792, 12);
            this.dteHBitis.Name = "dteHBitis";
            this.dteHBitis.Size = new System.Drawing.Size(136, 22);
            this.dteHBitis.StyleController = this.layoutHareket;
            this.dteHBitis.TabIndex = 4;
            // 
            // chkHPasif
            // 
            this.chkHPasif.Location = new System.Drawing.Point(932, 12);
            this.chkHPasif.Name = "chkHPasif";
            this.chkHPasif.Properties.Caption = "Geri alınanları da göster";
            this.chkHPasif.Size = new System.Drawing.Size(166, 22);
            this.chkHPasif.StyleController = this.layoutHareket;
            this.chkHPasif.TabIndex = 5;
            // 
            // btnListele
            // 
            this.btnListele.Location = new System.Drawing.Point(1102, 12);
            this.btnListele.Name = "btnListele";
            this.btnListele.Text = "Listele";
            this.btnListele.Size = new System.Drawing.Size(164, 22);
            this.btnListele.StyleController = this.layoutHareket;
            this.btnListele.TabIndex = 6;
            this.btnListele.Click += new System.EventHandler(this.btnListele_Click);
            // 
            // gridHareketler
            // 
            this.gridHareketler.Location = new System.Drawing.Point(12, 38);
            this.gridHareketler.MainView = this.viewHareketler;
            this.gridHareketler.Name = "gridHareketler";
            this.gridHareketler.Size = new System.Drawing.Size(1254, 651);
            this.gridHareketler.TabIndex = 7;
            this.gridHareketler.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewHareketler});
            // 
            // viewHareketler
            // 
            this.viewHareketler.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colHZaman,
            this.colHTur,
            this.colHUrun,
            this.colHParti,
            this.colHMiktar,
            this.colHEtki,
            this.colHAile,
            this.colHTeslimAlan,
            this.colHAciklama,
            this.colHKullanici,
            this.colHKayit});
            this.viewHareketler.GridControl = this.gridHareketler;
            this.viewHareketler.Name = "viewHareketler";
            this.viewHareketler.OptionsBehavior.Editable = false;
            this.viewHareketler.OptionsView.ShowGroupPanel = false;
            this.viewHareketler.OptionsView.ShowIndicator = false;
            this.viewHareketler.OptionsView.ColumnAutoWidth = false;
            this.viewHareketler.OptionsView.ShowAutoFilterRow = true;
            this.viewHareketler.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.viewHareketler_FocusedRowChanged);
            // 
            // colHZaman
            // 
            this.colHZaman.Caption = "Zaman";
            this.colHZaman.FieldName = "Zaman";
            this.colHZaman.Name = "colHZaman";
            this.colHZaman.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";
            this.colHZaman.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colHZaman.Visible = true;
            this.colHZaman.VisibleIndex = 0;
            this.colHZaman.Width = 110;
            // 
            // colHTur
            // 
            this.colHTur.Caption = "Tür";
            this.colHTur.FieldName = "TurAdi";
            this.colHTur.Name = "colHTur";
            this.colHTur.Visible = true;
            this.colHTur.VisibleIndex = 1;
            this.colHTur.Width = 120;
            // 
            // colHUrun
            // 
            this.colHUrun.Caption = "Ürün";
            this.colHUrun.FieldName = "UrunAdi";
            this.colHUrun.Name = "colHUrun";
            this.colHUrun.Visible = true;
            this.colHUrun.VisibleIndex = 2;
            this.colHUrun.Width = 130;
            // 
            // colHParti
            // 
            this.colHParti.Caption = "Parti";
            this.colHParti.FieldName = "PartiNo";
            this.colHParti.Name = "colHParti";
            this.colHParti.Visible = true;
            this.colHParti.VisibleIndex = 3;
            this.colHParti.Width = 90;
            // 
            // colHMiktar
            // 
            this.colHMiktar.Caption = "Miktar";
            this.colHMiktar.FieldName = "Miktar";
            this.colHMiktar.Name = "colHMiktar";
            this.colHMiktar.DisplayFormat.FormatString = "#,##0.##";
            this.colHMiktar.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colHMiktar.Visible = true;
            this.colHMiktar.VisibleIndex = 4;
            this.colHMiktar.Width = 70;
            // 
            // colHEtki
            // 
            this.colHEtki.Caption = "Stok etkisi";
            this.colHEtki.FieldName = "StokEtkisi";
            this.colHEtki.Name = "colHEtki";
            this.colHEtki.DisplayFormat.FormatString = "+#,##0.##;-#,##0.##;0";
            this.colHEtki.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colHEtki.Visible = true;
            this.colHEtki.VisibleIndex = 5;
            this.colHEtki.Width = 80;
            // 
            // colHAile
            // 
            this.colHAile.Caption = "Aile";
            this.colHAile.FieldName = "AileNo";
            this.colHAile.Name = "colHAile";
            this.colHAile.Visible = true;
            this.colHAile.VisibleIndex = 6;
            this.colHAile.Width = 80;
            // 
            // colHTeslimAlan
            // 
            this.colHTeslimAlan.Caption = "Teslim alan";
            this.colHTeslimAlan.FieldName = "TeslimAlan";
            this.colHTeslimAlan.Name = "colHTeslimAlan";
            this.colHTeslimAlan.Visible = true;
            this.colHTeslimAlan.VisibleIndex = 7;
            this.colHTeslimAlan.Width = 120;
            // 
            // colHAciklama
            // 
            this.colHAciklama.Caption = "Açıklama";
            this.colHAciklama.FieldName = "Aciklama";
            this.colHAciklama.Name = "colHAciklama";
            this.colHAciklama.Visible = true;
            this.colHAciklama.VisibleIndex = 8;
            this.colHAciklama.Width = 220;
            // 
            // colHKullanici
            // 
            this.colHKullanici.Caption = "Kaydeden";
            this.colHKullanici.FieldName = "KullaniciAdi";
            this.colHKullanici.Name = "colHKullanici";
            this.colHKullanici.Visible = true;
            this.colHKullanici.VisibleIndex = 9;
            this.colHKullanici.Width = 90;
            // 
            // colHKayit
            // 
            this.colHKayit.Caption = "Kayıt";
            this.colHKayit.FieldName = "KayitDurumu";
            this.colHKayit.Name = "colHKayit";
            this.colHKayit.Visible = true;
            this.colHKayit.VisibleIndex = 10;
            this.colHKayit.Width = 70;
            // 
            // btnHareketPasif
            // 
            this.btnHareketPasif.Location = new System.Drawing.Point(12, 693);
            this.btnHareketPasif.Name = "btnHareketPasif";
            this.btnHareketPasif.Text = "Seçili hareketi geri al (pasif yap)";
            this.btnHareketPasif.Size = new System.Drawing.Size(300, 30);
            this.btnHareketPasif.StyleController = this.layoutHareket;
            this.btnHareketPasif.TabIndex = 8;
            this.btnHareketPasif.Click += new System.EventHandler(this.btnHareketPasif_Click);
            // 
            // lciHUrun
            // 
            this.lciHUrun.Control = this.cmbHUrun;
            this.lciHUrun.Location = new System.Drawing.Point(0, 0);
            this.lciHUrun.MaxSize = new System.Drawing.Size(0, 26);
            this.lciHUrun.MinSize = new System.Drawing.Size(80, 26);
            this.lciHUrun.Name = "lciHUrun";
            this.lciHUrun.Size = new System.Drawing.Size(300, 26);
            this.lciHUrun.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciHUrun.Text = "Ürün";
            this.lciHUrun.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciHUrun.TextSize = new System.Drawing.Size(36, 13);
            // 
            // lciHTur
            // 
            this.lciHTur.Control = this.cmbHTur;
            this.lciHTur.Location = new System.Drawing.Point(300, 0);
            this.lciHTur.MaxSize = new System.Drawing.Size(0, 26);
            this.lciHTur.MinSize = new System.Drawing.Size(80, 26);
            this.lciHTur.Name = "lciHTur";
            this.lciHTur.Size = new System.Drawing.Size(240, 26);
            this.lciHTur.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciHTur.Text = "Tür";
            this.lciHTur.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciHTur.TextSize = new System.Drawing.Size(30, 13);
            // 
            // lciHBaslangic
            // 
            this.lciHBaslangic.Control = this.dteHBaslangic;
            this.lciHBaslangic.Location = new System.Drawing.Point(540, 0);
            this.lciHBaslangic.MaxSize = new System.Drawing.Size(0, 26);
            this.lciHBaslangic.MinSize = new System.Drawing.Size(80, 26);
            this.lciHBaslangic.Name = "lciHBaslangic";
            this.lciHBaslangic.Size = new System.Drawing.Size(200, 26);
            this.lciHBaslangic.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciHBaslangic.Text = "Başlangıç";
            this.lciHBaslangic.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciHBaslangic.TextSize = new System.Drawing.Size(60, 13);
            // 
            // lciHBitis
            // 
            this.lciHBitis.Control = this.dteHBitis;
            this.lciHBitis.Location = new System.Drawing.Point(740, 0);
            this.lciHBitis.MaxSize = new System.Drawing.Size(0, 26);
            this.lciHBitis.MinSize = new System.Drawing.Size(80, 26);
            this.lciHBitis.Name = "lciHBitis";
            this.lciHBitis.Size = new System.Drawing.Size(180, 26);
            this.lciHBitis.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciHBitis.Text = "Bitiş";
            this.lciHBitis.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciHBitis.TextSize = new System.Drawing.Size(40, 13);
            // 
            // lciHPasif
            // 
            this.lciHPasif.Control = this.chkHPasif;
            this.lciHPasif.Location = new System.Drawing.Point(920, 0);
            this.lciHPasif.MaxSize = new System.Drawing.Size(170, 26);
            this.lciHPasif.MinSize = new System.Drawing.Size(80, 26);
            this.lciHPasif.Name = "lciHPasif";
            this.lciHPasif.Size = new System.Drawing.Size(170, 26);
            this.lciHPasif.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciHPasif.TextVisible = false;
            // 
            // lciListele
            // 
            this.lciListele.Control = this.btnListele;
            this.lciListele.Location = new System.Drawing.Point(1090, 0);
            this.lciListele.MaxSize = new System.Drawing.Size(168, 26);
            this.lciListele.MinSize = new System.Drawing.Size(80, 26);
            this.lciListele.Name = "lciListele";
            this.lciListele.Size = new System.Drawing.Size(168, 26);
            this.lciListele.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciListele.TextVisible = false;
            // 
            // lciHareketler
            // 
            this.lciHareketler.Control = this.gridHareketler;
            this.lciHareketler.Location = new System.Drawing.Point(0, 26);
            this.lciHareketler.MinSize = new System.Drawing.Size(300, 120);
            this.lciHareketler.Name = "lciHareketler";
            this.lciHareketler.Size = new System.Drawing.Size(1258, 655);
            this.lciHareketler.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciHareketler.TextVisible = false;
            // 
            // lciHareketPasif
            // 
            this.lciHareketPasif.Control = this.btnHareketPasif;
            this.lciHareketPasif.Location = new System.Drawing.Point(0, 681);
            this.lciHareketPasif.MaxSize = new System.Drawing.Size(0, 34);
            this.lciHareketPasif.MinSize = new System.Drawing.Size(80, 34);
            this.lciHareketPasif.Name = "lciHareketPasif";
            this.lciHareketPasif.Size = new System.Drawing.Size(304, 34);
            this.lciHareketPasif.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciHareketPasif.TextVisible = false;
            // 
            // bosHareket
            // 
            this.bosHareket.Location = new System.Drawing.Point(304, 681);
            this.bosHareket.Name = "bosHareket";
            this.bosHareket.Size = new System.Drawing.Size(954, 34);
            // 
            // RootHareket
            // 
            this.RootHareket.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.RootHareket.GroupBordersVisible = false;
            this.RootHareket.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciHUrun,
            this.lciHTur,
            this.lciHBaslangic,
            this.lciHBitis,
            this.lciHPasif,
            this.lciListele,
            this.lciHareketler,
            this.lciHareketPasif,
            this.bosHareket});
            this.RootHareket.Name = "RootHareket";
            this.RootHareket.Size = new System.Drawing.Size(1278, 735);
            this.RootHareket.TextVisible = false;
            // 
            // layoutUyari
            // 
            this.layoutUyari.Controls.Add(this.gridUyarilar);
            this.layoutUyari.Controls.Add(this.btnFireyeDus);
            this.layoutUyari.Controls.Add(this.lblTahminBilgi);
            this.layoutUyari.Controls.Add(this.gridTahmin);
            this.layoutUyari.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutUyari.Location = new System.Drawing.Point(0, 0);
            this.layoutUyari.Name = "layoutUyari";
            this.layoutUyari.Root = this.RootUyari;
            this.layoutUyari.Size = new System.Drawing.Size(1278, 735);
            this.layoutUyari.TabIndex = 0;
            // 
            // gridUyarilar
            // 
            this.gridUyarilar.Location = new System.Drawing.Point(24, 45);
            this.gridUyarilar.MainView = this.viewUyarilar;
            this.gridUyarilar.Name = "gridUyarilar";
            this.gridUyarilar.Size = new System.Drawing.Size(1230, 247);
            this.gridUyarilar.TabIndex = 1;
            this.gridUyarilar.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewUyarilar});
            // 
            // viewUyarilar
            // 
            this.viewUyarilar.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colUyTur,
            this.colUyUrun,
            this.colUyMesaj});
            this.viewUyarilar.GridControl = this.gridUyarilar;
            this.viewUyarilar.Name = "viewUyarilar";
            this.viewUyarilar.OptionsBehavior.Editable = false;
            this.viewUyarilar.OptionsView.ShowGroupPanel = false;
            this.viewUyarilar.OptionsView.ShowIndicator = false;
            // 
            // colUyTur
            // 
            this.colUyTur.Caption = "Uyarı";
            this.colUyTur.FieldName = "Tur";
            this.colUyTur.Name = "colUyTur";
            this.colUyTur.Visible = true;
            this.colUyTur.VisibleIndex = 0;
            this.colUyTur.Width = 110;
            // 
            // colUyUrun
            // 
            this.colUyUrun.Caption = "Ürün";
            this.colUyUrun.FieldName = "UrunAdi";
            this.colUyUrun.Name = "colUyUrun";
            this.colUyUrun.Visible = true;
            this.colUyUrun.VisibleIndex = 1;
            this.colUyUrun.Width = 160;
            // 
            // colUyMesaj
            // 
            this.colUyMesaj.Caption = "Açıklama";
            this.colUyMesaj.FieldName = "Mesaj";
            this.colUyMesaj.Name = "colUyMesaj";
            this.colUyMesaj.Visible = true;
            this.colUyMesaj.VisibleIndex = 2;
            this.colUyMesaj.Width = 900;
            // 
            // btnFireyeDus
            // 
            this.btnFireyeDus.Location = new System.Drawing.Point(24, 296);
            this.btnFireyeDus.Name = "btnFireyeDus";
            this.btnFireyeDus.Text = "SKT'si geçen partileri hasar/fire olarak düş";
            this.btnFireyeDus.Size = new System.Drawing.Size(316, 30);
            this.btnFireyeDus.StyleController = this.layoutUyari;
            this.btnFireyeDus.TabIndex = 2;
            this.btnFireyeDus.Click += new System.EventHandler(this.btnFireyeDus_Click);
            // 
            // lblTahminBilgi
            // 
            this.lblTahminBilgi.Location = new System.Drawing.Point(24, 375);
            this.lblTahminBilgi.Name = "lblTahminBilgi";
            this.lblTahminBilgi.AllowHtmlString = true;
            this.lblTahminBilgi.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTahminBilgi.Text = "";
            this.lblTahminBilgi.Size = new System.Drawing.Size(1230, 36);
            this.lblTahminBilgi.StyleController = this.layoutUyari;
            this.lblTahminBilgi.TabIndex = 3;
            // 
            // gridTahmin
            // 
            this.gridTahmin.Location = new System.Drawing.Point(24, 415);
            this.gridTahmin.MainView = this.viewTahmin;
            this.gridTahmin.Name = "gridTahmin";
            this.gridTahmin.Size = new System.Drawing.Size(1230, 296);
            this.gridTahmin.TabIndex = 4;
            this.gridTahmin.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewTahmin});
            // 
            // viewTahmin
            // 
            this.viewTahmin.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colTUrun,
            this.colTKullanilabilir,
            this.colTTuketimGunluk,
            this.colTTuketimGun,
            this.colTHakGunluk,
            this.colTHakGun,
            this.colTDonem,
            this.colTAciklama});
            this.viewTahmin.GridControl = this.gridTahmin;
            this.viewTahmin.Name = "viewTahmin";
            this.viewTahmin.OptionsBehavior.Editable = false;
            this.viewTahmin.OptionsView.ShowGroupPanel = false;
            this.viewTahmin.OptionsView.ShowIndicator = false;
            this.viewTahmin.OptionsView.ColumnAutoWidth = false;
            // 
            // colTUrun
            // 
            this.colTUrun.Caption = "Ürün";
            this.colTUrun.FieldName = "Urun";
            this.colTUrun.Name = "colTUrun";
            this.colTUrun.Visible = true;
            this.colTUrun.VisibleIndex = 0;
            this.colTUrun.Width = 150;
            // 
            // colTKullanilabilir
            // 
            this.colTKullanilabilir.Caption = "Kullanılabilir";
            this.colTKullanilabilir.FieldName = "Kullanilabilir";
            this.colTKullanilabilir.Name = "colTKullanilabilir";
            this.colTKullanilabilir.Visible = true;
            this.colTKullanilabilir.VisibleIndex = 1;
            this.colTKullanilabilir.Width = 100;
            // 
            // colTTuketimGunluk
            // 
            this.colTTuketimGunluk.Caption = "Günlük tüketim";
            this.colTTuketimGunluk.FieldName = "TuketimGunluk";
            this.colTTuketimGunluk.Name = "colTTuketimGunluk";
            this.colTTuketimGunluk.Visible = true;
            this.colTTuketimGunluk.VisibleIndex = 2;
            this.colTTuketimGunluk.Width = 100;
            // 
            // colTTuketimGun
            // 
            this.colTTuketimGun.Caption = "Yeter (tüketime göre)";
            this.colTTuketimGun.FieldName = "TuketimeGore";
            this.colTTuketimGun.Name = "colTTuketimGun";
            this.colTTuketimGun.Visible = true;
            this.colTTuketimGun.VisibleIndex = 3;
            this.colTTuketimGun.Width = 130;
            // 
            // colTHakGunluk
            // 
            this.colTHakGunluk.Caption = "Günlük hak (kişi sayısına göre)";
            this.colTHakGunluk.FieldName = "HakGunluk";
            this.colTHakGunluk.Name = "colTHakGunluk";
            this.colTHakGunluk.Visible = true;
            this.colTHakGunluk.VisibleIndex = 4;
            this.colTHakGunluk.Width = 160;
            // 
            // colTHakGun
            // 
            this.colTHakGun.Caption = "Yeter (hakka göre)";
            this.colTHakGun.FieldName = "HakaGore";
            this.colTHakGun.Name = "colTHakGun";
            this.colTHakGun.Visible = true;
            this.colTHakGun.VisibleIndex = 5;
            this.colTHakGun.Width = 120;
            // 
            // colTDonem
            // 
            this.colTDonem.Caption = "Hesabın dayandığı dönem";
            this.colTDonem.FieldName = "Donem";
            this.colTDonem.Name = "colTDonem";
            this.colTDonem.Visible = true;
            this.colTDonem.VisibleIndex = 6;
            this.colTDonem.Width = 220;
            // 
            // colTAciklama
            // 
            this.colTAciklama.Caption = "Açıklama";
            this.colTAciklama.FieldName = "Aciklama";
            this.colTAciklama.Name = "colTAciklama";
            this.colTAciklama.Visible = true;
            this.colTAciklama.VisibleIndex = 7;
            this.colTAciklama.Width = 400;
            // 
            // lciUyarilar
            // 
            this.lciUyarilar.Control = this.gridUyarilar;
            this.lciUyarilar.Location = new System.Drawing.Point(0, 0);
            this.lciUyarilar.MinSize = new System.Drawing.Size(300, 100);
            this.lciUyarilar.Name = "lciUyarilar";
            this.lciUyarilar.Size = new System.Drawing.Size(1234, 251);
            this.lciUyarilar.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciUyarilar.TextVisible = false;
            // 
            // lciFireyeDus
            // 
            this.lciFireyeDus.Control = this.btnFireyeDus;
            this.lciFireyeDus.Location = new System.Drawing.Point(0, 251);
            this.lciFireyeDus.MaxSize = new System.Drawing.Size(0, 34);
            this.lciFireyeDus.MinSize = new System.Drawing.Size(80, 34);
            this.lciFireyeDus.Name = "lciFireyeDus";
            this.lciFireyeDus.Size = new System.Drawing.Size(320, 34);
            this.lciFireyeDus.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciFireyeDus.TextVisible = false;
            // 
            // bosUyari
            // 
            this.bosUyari.Location = new System.Drawing.Point(320, 251);
            this.bosUyari.Name = "bosUyari";
            this.bosUyari.Size = new System.Drawing.Size(914, 34);
            // 
            // grpUyarilar
            // 
            this.grpUyarilar.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciUyarilar,
            this.lciFireyeDus,
            this.bosUyari});
            this.grpUyarilar.Location = new System.Drawing.Point(0, 0);
            this.grpUyarilar.Name = "grpUyarilar";
            this.grpUyarilar.Size = new System.Drawing.Size(1258, 330);
            this.grpUyarilar.Text = "Stok uyarıları: kritik seviye, SKT'si yaklaşan ve geçen partiler";
            // 
            // lciTahminBilgi
            // 
            this.lciTahminBilgi.Control = this.lblTahminBilgi;
            this.lciTahminBilgi.Location = new System.Drawing.Point(0, 0);
            this.lciTahminBilgi.MaxSize = new System.Drawing.Size(0, 40);
            this.lciTahminBilgi.MinSize = new System.Drawing.Size(100, 40);
            this.lciTahminBilgi.Name = "lciTahminBilgi";
            this.lciTahminBilgi.Size = new System.Drawing.Size(1234, 40);
            this.lciTahminBilgi.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciTahminBilgi.TextVisible = false;
            // 
            // lciTahmin
            // 
            this.lciTahmin.Control = this.gridTahmin;
            this.lciTahmin.Location = new System.Drawing.Point(0, 40);
            this.lciTahmin.MinSize = new System.Drawing.Size(300, 100);
            this.lciTahmin.Name = "lciTahmin";
            this.lciTahmin.Size = new System.Drawing.Size(1234, 300);
            this.lciTahmin.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciTahmin.TextVisible = false;
            // 
            // grpTahmin
            // 
            this.grpTahmin.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciTahminBilgi,
            this.lciTahmin});
            this.grpTahmin.Location = new System.Drawing.Point(0, 330);
            this.grpTahmin.Name = "grpTahmin";
            this.grpTahmin.Size = new System.Drawing.Size(1258, 385);
            this.grpTahmin.Text = "Stok tahmini: mevcut stok kaç gün yeter?";
            // 
            // RootUyari
            // 
            this.RootUyari.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.RootUyari.GroupBordersVisible = false;
            this.RootUyari.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.grpUyarilar,
            this.grpTahmin});
            this.RootUyari.Name = "RootUyari";
            this.RootUyari.Size = new System.Drawing.Size(1278, 735);
            this.RootUyari.TextVisible = false;
            // 
            // StokDurumuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 760);
            this.Controls.Add(this.tabControl);
            this.MinimumSize = new System.Drawing.Size(1100, 700);
            this.Name = "StokDurumuForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stok durumu";
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabStok.ResumeLayout(false);
            this.tabHareketler.ResumeLayout(false);
            this.tabUyarilar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutStok)).EndInit();
            this.layoutStok.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridUrunStok)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewUrunStok)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridPartiler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewPartiler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbIslem.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnIslemMiktar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIslemAciklama.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciStokOzet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYenile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciUrunStok)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpUrunStok)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPartiler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIslem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIslemMiktar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIslemAciklama)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIslemUygula)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpPartiler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootStok)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutHareket)).EndInit();
            this.layoutHareket.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbHUrun.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbHTur.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteHBaslangic.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteHBaslangic.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteHBitis.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteHBitis.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkHPasif.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridHareketler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewHareketler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHUrun)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHTur)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHBaslangic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHBitis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHPasif)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciListele)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHareketler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHareketPasif)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosHareket)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootHareket)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutUyari)).EndInit();
            this.layoutUyari.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridUyarilar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewUyarilar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridTahmin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewTahmin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciUyarilar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFireyeDus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosUyari)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpUyarilar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTahminBilgi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTahmin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpTahmin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootUyari)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl tabControl;
        private DevExpress.XtraTab.XtraTabPage tabStok;
        private DevExpress.XtraTab.XtraTabPage tabHareketler;
        private DevExpress.XtraTab.XtraTabPage tabUyarilar;
        private DevExpress.XtraLayout.LayoutControl layoutStok;
        private DevExpress.XtraEditors.LabelControl lblStokOzet;
        private DevExpress.XtraEditors.SimpleButton btnYenile;
        private DevExpress.XtraGrid.GridControl gridUrunStok;
        private DevExpress.XtraGrid.Views.Grid.GridView viewUrunStok;
        private DevExpress.XtraGrid.Columns.GridColumn colUUrun;
        private DevExpress.XtraGrid.Columns.GridColumn colUBirim;
        private DevExpress.XtraGrid.Columns.GridColumn colUFiziksel;
        private DevExpress.XtraGrid.Columns.GridColumn colUAyrilan;
        private DevExpress.XtraGrid.Columns.GridColumn colUKullanilabilir;
        private DevExpress.XtraGrid.Columns.GridColumn colUSktGecmis;
        private DevExpress.XtraGrid.Columns.GridColumn colUKritik;
        private DevExpress.XtraGrid.Columns.GridColumn colUDurum;
        private DevExpress.XtraGrid.GridControl gridPartiler;
        private DevExpress.XtraGrid.Views.Grid.GridView viewPartiler;
        private DevExpress.XtraGrid.Columns.GridColumn colPSira;
        private DevExpress.XtraGrid.Columns.GridColumn colPParti;
        private DevExpress.XtraGrid.Columns.GridColumn colPBagisci;
        private DevExpress.XtraGrid.Columns.GridColumn colPGiris;
        private DevExpress.XtraGrid.Columns.GridColumn colPSkt;
        private DevExpress.XtraGrid.Columns.GridColumn colPMiktar;
        private DevExpress.XtraGrid.Columns.GridColumn colPAyrilan;
        private DevExpress.XtraGrid.Columns.GridColumn colPKullanilabilir;
        private DevExpress.XtraGrid.Columns.GridColumn colPDurum;
        private DevExpress.XtraEditors.ImageComboBoxEdit cmbIslem;
        private DevExpress.XtraEditors.SpinEdit spnIslemMiktar;
        private DevExpress.XtraEditors.TextEdit txtIslemAciklama;
        private DevExpress.XtraEditors.SimpleButton btnIslemUygula;
        private DevExpress.XtraLayout.LayoutControlItem lciStokOzet;
        private DevExpress.XtraLayout.LayoutControlItem lciYenile;
        private DevExpress.XtraLayout.LayoutControlItem lciUrunStok;
        private DevExpress.XtraLayout.LayoutControlGroup grpUrunStok;
        private DevExpress.XtraLayout.LayoutControlItem lciPartiler;
        private DevExpress.XtraLayout.LayoutControlItem lciIslem;
        private DevExpress.XtraLayout.LayoutControlItem lciIslemMiktar;
        private DevExpress.XtraLayout.LayoutControlItem lciIslemAciklama;
        private DevExpress.XtraLayout.LayoutControlItem lciIslemUygula;
        private DevExpress.XtraLayout.LayoutControlGroup grpPartiler;
        private DevExpress.XtraLayout.LayoutControlGroup RootStok;
        private DevExpress.XtraLayout.LayoutControl layoutHareket;
        private DevExpress.XtraEditors.ImageComboBoxEdit cmbHUrun;
        private DevExpress.XtraEditors.ImageComboBoxEdit cmbHTur;
        private DevExpress.XtraEditors.DateEdit dteHBaslangic;
        private DevExpress.XtraEditors.DateEdit dteHBitis;
        private DevExpress.XtraEditors.CheckEdit chkHPasif;
        private DevExpress.XtraEditors.SimpleButton btnListele;
        private DevExpress.XtraGrid.GridControl gridHareketler;
        private DevExpress.XtraGrid.Views.Grid.GridView viewHareketler;
        private DevExpress.XtraGrid.Columns.GridColumn colHZaman;
        private DevExpress.XtraGrid.Columns.GridColumn colHTur;
        private DevExpress.XtraGrid.Columns.GridColumn colHUrun;
        private DevExpress.XtraGrid.Columns.GridColumn colHParti;
        private DevExpress.XtraGrid.Columns.GridColumn colHMiktar;
        private DevExpress.XtraGrid.Columns.GridColumn colHEtki;
        private DevExpress.XtraGrid.Columns.GridColumn colHAile;
        private DevExpress.XtraGrid.Columns.GridColumn colHTeslimAlan;
        private DevExpress.XtraGrid.Columns.GridColumn colHAciklama;
        private DevExpress.XtraGrid.Columns.GridColumn colHKullanici;
        private DevExpress.XtraGrid.Columns.GridColumn colHKayit;
        private DevExpress.XtraEditors.SimpleButton btnHareketPasif;
        private DevExpress.XtraLayout.LayoutControlItem lciHUrun;
        private DevExpress.XtraLayout.LayoutControlItem lciHTur;
        private DevExpress.XtraLayout.LayoutControlItem lciHBaslangic;
        private DevExpress.XtraLayout.LayoutControlItem lciHBitis;
        private DevExpress.XtraLayout.LayoutControlItem lciHPasif;
        private DevExpress.XtraLayout.LayoutControlItem lciListele;
        private DevExpress.XtraLayout.LayoutControlItem lciHareketler;
        private DevExpress.XtraLayout.LayoutControlItem lciHareketPasif;
        private DevExpress.XtraLayout.EmptySpaceItem bosHareket;
        private DevExpress.XtraLayout.LayoutControlGroup RootHareket;
        private DevExpress.XtraLayout.LayoutControl layoutUyari;
        private DevExpress.XtraGrid.GridControl gridUyarilar;
        private DevExpress.XtraGrid.Views.Grid.GridView viewUyarilar;
        private DevExpress.XtraGrid.Columns.GridColumn colUyTur;
        private DevExpress.XtraGrid.Columns.GridColumn colUyUrun;
        private DevExpress.XtraGrid.Columns.GridColumn colUyMesaj;
        private DevExpress.XtraEditors.SimpleButton btnFireyeDus;
        private DevExpress.XtraEditors.LabelControl lblTahminBilgi;
        private DevExpress.XtraGrid.GridControl gridTahmin;
        private DevExpress.XtraGrid.Views.Grid.GridView viewTahmin;
        private DevExpress.XtraGrid.Columns.GridColumn colTUrun;
        private DevExpress.XtraGrid.Columns.GridColumn colTKullanilabilir;
        private DevExpress.XtraGrid.Columns.GridColumn colTTuketimGunluk;
        private DevExpress.XtraGrid.Columns.GridColumn colTTuketimGun;
        private DevExpress.XtraGrid.Columns.GridColumn colTHakGunluk;
        private DevExpress.XtraGrid.Columns.GridColumn colTHakGun;
        private DevExpress.XtraGrid.Columns.GridColumn colTDonem;
        private DevExpress.XtraGrid.Columns.GridColumn colTAciklama;
        private DevExpress.XtraLayout.LayoutControlItem lciUyarilar;
        private DevExpress.XtraLayout.LayoutControlItem lciFireyeDus;
        private DevExpress.XtraLayout.EmptySpaceItem bosUyari;
        private DevExpress.XtraLayout.LayoutControlGroup grpUyarilar;
        private DevExpress.XtraLayout.LayoutControlItem lciTahminBilgi;
        private DevExpress.XtraLayout.LayoutControlItem lciTahmin;
        private DevExpress.XtraLayout.LayoutControlGroup grpTahmin;
        private DevExpress.XtraLayout.LayoutControlGroup RootUyari;
    }
}
