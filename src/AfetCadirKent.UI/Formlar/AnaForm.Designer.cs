namespace AfetCadirKent.UI.Formlar
{
    partial class AnaForm
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
            this.components = new System.ComponentModel.Container();
            this.ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.bbiYenile = new DevExpress.XtraBars.BarButtonItem();
            this.bbiGunlukOzet = new DevExpress.XtraBars.BarButtonItem();
            this.bbiParolaDegistir = new DevExpress.XtraBars.BarButtonItem();
            this.bbiOturumuKapat = new DevExpress.XtraBars.BarButtonItem();
            this.bsiKullanici = new DevExpress.XtraBars.BarStaticItem();
            this.bsiSonGuncelleme = new DevExpress.XtraBars.BarStaticItem();
            this.ribbonPageAnaSayfa = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.rpgGostergeler = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.rpgYapayZeka = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.rpgOturum = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
            this.lblKisi = new DevExpress.XtraEditors.LabelControl();
            this.lblYatak = new DevExpress.XtraEditors.LabelControl();
            this.lblBekleme = new DevExpress.XtraEditors.LabelControl();
            this.lblTalep = new DevExpress.XtraEditors.LabelControl();
            this.gaugeDoluluk = new DevExpress.XtraGauges.Win.GaugeControl();
            this.chartTalepler = new DevExpress.XtraCharts.ChartControl();
            this.gridUyarilar = new DevExpress.XtraGrid.GridControl();
            this.gridViewUyarilar = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colTur = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUrun = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMesaj = new DevExpress.XtraGrid.Columns.GridColumn();
            this.memoOzet = new DevExpress.XtraEditors.MemoEdit();
            this.lblBos = new DevExpress.XtraEditors.LabelControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciKisi = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciYatak = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBekleme = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTalep = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDoluluk = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTalepGrafigi = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciUyarilar = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciOzet = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBos = new DevExpress.XtraLayout.LayoutControlItem();
            this.alertControl = new DevExpress.XtraBars.Alerter.AlertControl(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
            this.layoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartTalepler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridUyarilar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewUyarilar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoOzet.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKisi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYatak)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBekleme)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTalep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDoluluk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTalepGrafigi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciUyarilar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOzet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBos)).BeginInit();
            this.SuspendLayout();
            //
            // ribbonControl
            //
            this.ribbonControl.ExpandCollapseItem.Id = 0;
            this.ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl.ExpandCollapseItem,
            this.bbiYenile,
            this.bbiGunlukOzet,
            this.bbiParolaDegistir,
            this.bbiOturumuKapat,
            this.bsiKullanici,
            this.bsiSonGuncelleme});
            this.ribbonControl.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl.MaxItemId = 7;
            this.ribbonControl.Name = "ribbonControl";
            this.ribbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPageAnaSayfa});
            this.ribbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonControl.Size = new System.Drawing.Size(1200, 158);
            this.ribbonControl.StatusBar = this.ribbonStatusBar;
            this.ribbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            //
            // bbiYenile
            //
            this.bbiYenile.Caption = "Göstergeleri yenile";
            this.bbiYenile.Id = 1;
            this.bbiYenile.Name = "bbiYenile";
            this.bbiYenile.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.bbiYenile.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiYenile_ItemClick);
            //
            // bbiGunlukOzet
            //
            this.bbiGunlukOzet.Caption = "Günlük özet oluştur";
            this.bbiGunlukOzet.Id = 2;
            this.bbiGunlukOzet.Name = "bbiGunlukOzet";
            this.bbiGunlukOzet.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.bbiGunlukOzet.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiGunlukOzet_ItemClick);
            //
            // bbiParolaDegistir
            //
            this.bbiParolaDegistir.Caption = "Parola değiştir";
            this.bbiParolaDegistir.Id = 3;
            this.bbiParolaDegistir.Name = "bbiParolaDegistir";
            this.bbiParolaDegistir.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.bbiParolaDegistir.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiParolaDegistir_ItemClick);
            //
            // bbiOturumuKapat
            //
            this.bbiOturumuKapat.Caption = "Oturumu kapat";
            this.bbiOturumuKapat.Id = 4;
            this.bbiOturumuKapat.Name = "bbiOturumuKapat";
            this.bbiOturumuKapat.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.bbiOturumuKapat.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiOturumuKapat_ItemClick);
            //
            // bsiKullanici
            //
            this.bsiKullanici.Id = 5;
            this.bsiKullanici.Name = "bsiKullanici";
            //
            // bsiSonGuncelleme
            //
            this.bsiSonGuncelleme.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.bsiSonGuncelleme.Id = 6;
            this.bsiSonGuncelleme.Name = "bsiSonGuncelleme";
            //
            // ribbonPageAnaSayfa
            //
            this.ribbonPageAnaSayfa.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.rpgGostergeler,
            this.rpgYapayZeka,
            this.rpgOturum});
            this.ribbonPageAnaSayfa.Name = "ribbonPageAnaSayfa";
            this.ribbonPageAnaSayfa.Text = "Ana sayfa";
            //
            // rpgGostergeler
            //
            this.rpgGostergeler.ItemLinks.Add(this.bbiYenile);
            this.rpgGostergeler.Name = "rpgGostergeler";
            this.rpgGostergeler.Text = "Göstergeler";
            //
            // rpgYapayZeka
            //
            this.rpgYapayZeka.ItemLinks.Add(this.bbiGunlukOzet);
            this.rpgYapayZeka.Name = "rpgYapayZeka";
            this.rpgYapayZeka.Text = "Yapay zeka";
            //
            // rpgOturum
            //
            this.rpgOturum.ItemLinks.Add(this.bbiParolaDegistir);
            this.rpgOturum.ItemLinks.Add(this.bbiOturumuKapat);
            this.rpgOturum.Name = "rpgOturum";
            this.rpgOturum.Text = "Oturum";
            //
            // ribbonStatusBar
            //
            this.ribbonStatusBar.ItemLinks.Add(this.bsiKullanici);
            this.ribbonStatusBar.ItemLinks.Add(this.bsiSonGuncelleme);
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 766);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbonControl;
            this.ribbonStatusBar.Size = new System.Drawing.Size(1200, 24);
            //
            // layoutControl
            //
            this.layoutControl.Controls.Add(this.lblKisi);
            this.layoutControl.Controls.Add(this.lblYatak);
            this.layoutControl.Controls.Add(this.lblBekleme);
            this.layoutControl.Controls.Add(this.lblTalep);
            this.layoutControl.Controls.Add(this.gaugeDoluluk);
            this.layoutControl.Controls.Add(this.chartTalepler);
            this.layoutControl.Controls.Add(this.gridUyarilar);
            this.layoutControl.Controls.Add(this.memoOzet);
            this.layoutControl.Controls.Add(this.lblBos);
            this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl.Location = new System.Drawing.Point(0, 158);
            this.layoutControl.Name = "layoutControl";
            this.layoutControl.Root = this.Root;
            this.layoutControl.Size = new System.Drawing.Size(1200, 608);
            this.layoutControl.TabIndex = 0;
            //
            // lblKisi
            //
            this.lblKisi.AllowHtmlString = true;
            this.lblKisi.Appearance.Options.UseTextOptions = true;
            this.lblKisi.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblKisi.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblKisi.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblKisi.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblKisi.Location = new System.Drawing.Point(12, 31);
            this.lblKisi.Name = "lblKisi";
            this.lblKisi.Size = new System.Drawing.Size(291, 73);
            this.lblKisi.StyleController = this.layoutControl;
            this.lblKisi.TabIndex = 1;
            //
            // lblYatak
            //
            this.lblYatak.AllowHtmlString = true;
            this.lblYatak.Appearance.Options.UseTextOptions = true;
            this.lblYatak.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblYatak.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblYatak.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblYatak.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblYatak.Location = new System.Drawing.Point(307, 31);
            this.lblYatak.Name = "lblYatak";
            this.lblYatak.Size = new System.Drawing.Size(291, 73);
            this.lblYatak.StyleController = this.layoutControl;
            this.lblYatak.TabIndex = 2;
            //
            // lblBekleme
            //
            this.lblBekleme.AllowHtmlString = true;
            this.lblBekleme.Appearance.Options.UseTextOptions = true;
            this.lblBekleme.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblBekleme.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblBekleme.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblBekleme.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblBekleme.Location = new System.Drawing.Point(602, 31);
            this.lblBekleme.Name = "lblBekleme";
            this.lblBekleme.Size = new System.Drawing.Size(291, 73);
            this.lblBekleme.StyleController = this.layoutControl;
            this.lblBekleme.TabIndex = 3;
            //
            // lblTalep
            //
            this.lblTalep.AllowHtmlString = true;
            this.lblTalep.Appearance.Options.UseTextOptions = true;
            this.lblTalep.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblTalep.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblTalep.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblTalep.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTalep.Location = new System.Drawing.Point(897, 31);
            this.lblTalep.Name = "lblTalep";
            this.lblTalep.Size = new System.Drawing.Size(291, 73);
            this.lblTalep.StyleController = this.layoutControl;
            this.lblTalep.TabIndex = 4;
            //
            // gaugeDoluluk
            //
            this.gaugeDoluluk.Location = new System.Drawing.Point(12, 127);
            this.gaugeDoluluk.Name = "gaugeDoluluk";
            this.gaugeDoluluk.Size = new System.Drawing.Size(376, 167);
            this.gaugeDoluluk.TabIndex = 5;
            //
            // chartTalepler
            //
            this.chartTalepler.EmptyChartText.Text = "Açık talep yok.";
            this.chartTalepler.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;
            this.chartTalepler.Location = new System.Drawing.Point(392, 127);
            this.chartTalepler.Name = "chartTalepler";
            this.chartTalepler.Size = new System.Drawing.Size(796, 167);
            this.chartTalepler.TabIndex = 6;
            //
            // gridUyarilar
            //
            this.gridUyarilar.Location = new System.Drawing.Point(12, 317);
            this.gridUyarilar.MainView = this.gridViewUyarilar;
            this.gridUyarilar.MenuManager = this.ribbonControl;
            this.gridUyarilar.Name = "gridUyarilar";
            this.gridUyarilar.Size = new System.Drawing.Size(1176, 167);
            this.gridUyarilar.TabIndex = 7;
            this.gridUyarilar.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewUyarilar});
            //
            // gridViewUyarilar
            //
            this.gridViewUyarilar.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colTur,
            this.colUrun,
            this.colMesaj});
            this.gridViewUyarilar.GridControl = this.gridUyarilar;
            this.gridViewUyarilar.Name = "gridViewUyarilar";
            this.gridViewUyarilar.OptionsBehavior.Editable = false;
            this.gridViewUyarilar.OptionsView.ShowGroupPanel = false;
            this.gridViewUyarilar.OptionsView.ShowIndicator = false;
            //
            // colTur
            //
            this.colTur.Caption = "Uyarı";
            this.colTur.FieldName = "TurMetni";
            this.colTur.Name = "colTur";
            this.colTur.Visible = true;
            this.colTur.VisibleIndex = 0;
            this.colTur.Width = 130;
            //
            // colUrun
            //
            this.colUrun.Caption = "Ürün";
            this.colUrun.FieldName = "UrunAdi";
            this.colUrun.Name = "colUrun";
            this.colUrun.Visible = true;
            this.colUrun.VisibleIndex = 1;
            this.colUrun.Width = 180;
            //
            // colMesaj
            //
            this.colMesaj.Caption = "Açıklama";
            this.colMesaj.FieldName = "Mesaj";
            this.colMesaj.Name = "colMesaj";
            this.colMesaj.Visible = true;
            this.colMesaj.VisibleIndex = 2;
            this.colMesaj.Width = 600;
            //
            // memoOzet
            //
            this.memoOzet.Location = new System.Drawing.Point(12, 507);
            this.memoOzet.MenuManager = this.ribbonControl;
            this.memoOzet.Name = "memoOzet";
            this.memoOzet.Properties.NullValuePrompt = "Koordinatör için günlük durum raporu: Ribbon\'daki \"Günlük özet oluştur\" düğmesine basın.";
            this.memoOzet.Properties.ReadOnly = true;
            this.memoOzet.Size = new System.Drawing.Size(1176, 59);
            this.memoOzet.StyleController = this.layoutControl;
            this.memoOzet.TabIndex = 8;
            //
            // lblBos
            //
            this.lblBos.Appearance.Options.UseTextOptions = true;
            this.lblBos.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblBos.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblBos.Location = new System.Drawing.Point(12, 570);
            this.lblBos.Name = "lblBos";
            this.lblBos.Size = new System.Drawing.Size(1176, 26);
            this.lblBos.StyleController = this.layoutControl;
            this.lblBos.TabIndex = 9;
            this.lblBos.Text = "Bu hesabın yetkileri kapsamında ana ekranda gösterilecek bir gösterge yok.";
            //
            // Root
            //
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciKisi,
            this.lciYatak,
            this.lciBekleme,
            this.lciTalep,
            this.lciDoluluk,
            this.lciTalepGrafigi,
            this.lciUyarilar,
            this.lciOzet,
            this.lciBos});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1200, 608);
            this.Root.TextVisible = false;
            //
            // lciKisi
            //
            this.lciKisi.Control = this.lblKisi;
            this.lciKisi.Location = new System.Drawing.Point(0, 0);
            this.lciKisi.Name = "lciKisi";
            this.lciKisi.MaxSize = new System.Drawing.Size(0, 100);
            this.lciKisi.MinSize = new System.Drawing.Size(120, 70);
            this.lciKisi.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKisi.Size = new System.Drawing.Size(295, 96);
            this.lciKisi.Text = "Çadır kentteki kişi";
            this.lciKisi.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciKisi.TextSize = new System.Drawing.Size(120, 13);
            //
            // lciYatak
            //
            this.lciYatak.Control = this.lblYatak;
            this.lciYatak.Location = new System.Drawing.Point(295, 0);
            this.lciYatak.Name = "lciYatak";
            this.lciYatak.MaxSize = new System.Drawing.Size(0, 100);
            this.lciYatak.MinSize = new System.Drawing.Size(120, 70);
            this.lciYatak.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciYatak.Size = new System.Drawing.Size(295, 96);
            this.lciYatak.Text = "Boş yatak";
            this.lciYatak.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciYatak.TextSize = new System.Drawing.Size(120, 13);
            //
            // lciBekleme
            //
            this.lciBekleme.Control = this.lblBekleme;
            this.lciBekleme.Location = new System.Drawing.Point(590, 0);
            this.lciBekleme.Name = "lciBekleme";
            this.lciBekleme.MaxSize = new System.Drawing.Size(0, 100);
            this.lciBekleme.MinSize = new System.Drawing.Size(120, 70);
            this.lciBekleme.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBekleme.Size = new System.Drawing.Size(295, 96);
            this.lciBekleme.Text = "Bekleme listesi";
            this.lciBekleme.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciBekleme.TextSize = new System.Drawing.Size(120, 13);
            //
            // lciTalep
            //
            this.lciTalep.Control = this.lblTalep;
            this.lciTalep.Location = new System.Drawing.Point(885, 0);
            this.lciTalep.Name = "lciTalep";
            this.lciTalep.MaxSize = new System.Drawing.Size(0, 100);
            this.lciTalep.MinSize = new System.Drawing.Size(120, 70);
            this.lciTalep.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciTalep.Size = new System.Drawing.Size(295, 96);
            this.lciTalep.Text = "Açık talepler";
            this.lciTalep.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciTalep.TextSize = new System.Drawing.Size(120, 13);
            //
            // lciDoluluk
            //
            this.lciDoluluk.Control = this.gaugeDoluluk;
            this.lciDoluluk.Location = new System.Drawing.Point(0, 96);
            this.lciDoluluk.Name = "lciDoluluk";
            this.lciDoluluk.MaxSize = new System.Drawing.Size(0, 0);
            this.lciDoluluk.MinSize = new System.Drawing.Size(150, 130);
            this.lciDoluluk.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDoluluk.Size = new System.Drawing.Size(380, 190);
            this.lciDoluluk.Text = "Yatak doluluğu";
            this.lciDoluluk.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciDoluluk.TextSize = new System.Drawing.Size(120, 13);
            //
            // lciTalepGrafigi
            //
            this.lciTalepGrafigi.Control = this.chartTalepler;
            this.lciTalepGrafigi.Location = new System.Drawing.Point(380, 96);
            this.lciTalepGrafigi.Name = "lciTalepGrafigi";
            this.lciTalepGrafigi.MaxSize = new System.Drawing.Size(0, 0);
            this.lciTalepGrafigi.MinSize = new System.Drawing.Size(200, 130);
            this.lciTalepGrafigi.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciTalepGrafigi.Size = new System.Drawing.Size(800, 190);
            this.lciTalepGrafigi.Text = "Türe göre açık talepler";
            this.lciTalepGrafigi.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciTalepGrafigi.TextSize = new System.Drawing.Size(120, 13);
            //
            // lciUyarilar
            //
            this.lciUyarilar.Control = this.gridUyarilar;
            this.lciUyarilar.Location = new System.Drawing.Point(0, 286);
            this.lciUyarilar.Name = "lciUyarilar";
            this.lciUyarilar.MaxSize = new System.Drawing.Size(0, 0);
            this.lciUyarilar.MinSize = new System.Drawing.Size(200, 110);
            this.lciUyarilar.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciUyarilar.Size = new System.Drawing.Size(1180, 190);
            this.lciUyarilar.Text = "Stok uyarıları (kritik seviye ve son kullanma tarihi)";
            this.lciUyarilar.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciUyarilar.TextSize = new System.Drawing.Size(120, 13);
            //
            // lciOzet
            //
            this.lciOzet.Control = this.memoOzet;
            this.lciOzet.Location = new System.Drawing.Point(0, 476);
            this.lciOzet.Name = "lciOzet";
            this.lciOzet.MaxSize = new System.Drawing.Size(0, 140);
            this.lciOzet.MinSize = new System.Drawing.Size(200, 60);
            this.lciOzet.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciOzet.Size = new System.Drawing.Size(1180, 82);
            this.lciOzet.Text = "Yapay zeka günlük özeti";
            this.lciOzet.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciOzet.TextSize = new System.Drawing.Size(120, 13);
            //
            // lciBos
            //
            this.lciBos.Control = this.lblBos;
            this.lciBos.Location = new System.Drawing.Point(0, 558);
            this.lciBos.Name = "lciBos";
            this.lciBos.MaxSize = new System.Drawing.Size(0, 30);
            this.lciBos.MinSize = new System.Drawing.Size(100, 24);
            this.lciBos.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBos.Size = new System.Drawing.Size(1180, 30);
            this.lciBos.TextVisible = false;
            this.lciBos.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //
            // AnaForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 790);
            this.Controls.Add(this.layoutControl);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbonControl);
            this.Name = "AnaForm";
            this.Ribbon = this.ribbonControl;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "Afet Çadır Kent Yönetim Otomasyonu";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
            this.layoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartTalepler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridUyarilar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewUyarilar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoOzet.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKisi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYatak)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBekleme)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTalep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDoluluk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTalepGrafigi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciUyarilar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOzet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPageAnaSayfa;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgGostergeler;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgYapayZeka;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgOturum;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.BarButtonItem bbiYenile;
        private DevExpress.XtraBars.BarButtonItem bbiGunlukOzet;
        private DevExpress.XtraBars.BarButtonItem bbiParolaDegistir;
        private DevExpress.XtraBars.BarButtonItem bbiOturumuKapat;
        private DevExpress.XtraBars.BarStaticItem bsiKullanici;
        private DevExpress.XtraBars.BarStaticItem bsiSonGuncelleme;
        private DevExpress.XtraLayout.LayoutControl layoutControl;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.LabelControl lblKisi;
        private DevExpress.XtraEditors.LabelControl lblYatak;
        private DevExpress.XtraEditors.LabelControl lblBekleme;
        private DevExpress.XtraEditors.LabelControl lblTalep;
        private DevExpress.XtraGauges.Win.GaugeControl gaugeDoluluk;
        private DevExpress.XtraCharts.ChartControl chartTalepler;
        private DevExpress.XtraGrid.GridControl gridUyarilar;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewUyarilar;
        private DevExpress.XtraGrid.Columns.GridColumn colTur;
        private DevExpress.XtraGrid.Columns.GridColumn colUrun;
        private DevExpress.XtraGrid.Columns.GridColumn colMesaj;
        private DevExpress.XtraEditors.MemoEdit memoOzet;
        private DevExpress.XtraEditors.LabelControl lblBos;
        private DevExpress.XtraLayout.LayoutControlItem lciKisi;
        private DevExpress.XtraLayout.LayoutControlItem lciYatak;
        private DevExpress.XtraLayout.LayoutControlItem lciBekleme;
        private DevExpress.XtraLayout.LayoutControlItem lciTalep;
        private DevExpress.XtraLayout.LayoutControlItem lciDoluluk;
        private DevExpress.XtraLayout.LayoutControlItem lciTalepGrafigi;
        private DevExpress.XtraLayout.LayoutControlItem lciUyarilar;
        private DevExpress.XtraLayout.LayoutControlItem lciOzet;
        private DevExpress.XtraLayout.LayoutControlItem lciBos;
        private DevExpress.XtraBars.Alerter.AlertControl alertControl;
    }
}
