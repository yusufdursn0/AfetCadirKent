namespace AfetCadirKent.UI.Formlar
{
    partial class CadirPlaniForm
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
            this.tabPlan = new DevExpress.XtraTab.XtraTabPage();
            this.tabTanimlar = new DevExpress.XtraTab.XtraTabPage();
            this.layoutPlan = new DevExpress.XtraLayout.LayoutControl();
            this.cmbBolge = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.btnYenile = new DevExpress.XtraEditors.SimpleButton();
            this.lblLejant = new DevExpress.XtraEditors.LabelControl();
            this.lblPlanOzet = new DevExpress.XtraEditors.LabelControl();
            this.tileCadirlar = new DevExpress.XtraEditors.TileControl();
            this.lblCadirOzet = new DevExpress.XtraEditors.LabelControl();
            this.gridCadirdakiler = new DevExpress.XtraGrid.GridControl();
            this.viewCadirdakiler = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colCKisiNo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCAdSoyad = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCYas = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCDurum = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCAile = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCBaslangic = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnTasi = new DevExpress.XtraEditors.SimpleButton();
            this.btnBekleyeniYerlestir = new DevExpress.XtraEditors.SimpleButton();
            this.lkAile = new DevExpress.XtraEditors.LookUpEdit();
            this.btnOner = new DevExpress.XtraEditors.SimpleButton();
            this.oneriPaneli = new AfetCadirKent.UI.Formlar.YerlesimOneriPaneli();
            this.lciBolge = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciYenile = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciLejant = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPlanOzet = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPlan = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCadirOzet = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCadirdakiler = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTasi = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBekleyeniYerlestir = new DevExpress.XtraLayout.LayoutControlItem();
            this.bosCadirDugmeler = new DevExpress.XtraLayout.EmptySpaceItem();
            this.grpCadir = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciAile = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciOner = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciOneriPaneli = new DevExpress.XtraLayout.LayoutControlItem();
            this.grpYerlestir = new DevExpress.XtraLayout.LayoutControlGroup();
            this.tabSag = new DevExpress.XtraLayout.TabbedControlGroup();
            this.RootPlan = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutTanim = new DevExpress.XtraLayout.LayoutControl();
            this.gridBolgeler = new DevExpress.XtraGrid.GridControl();
            this.viewBolgeler = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colBKod = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBAd = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBAciklama = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBCadirSayisi = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBKayitDurumu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtBolgeKod = new DevExpress.XtraEditors.TextEdit();
            this.txtBolgeAd = new DevExpress.XtraEditors.TextEdit();
            this.txtBolgeAciklama = new DevExpress.XtraEditors.TextEdit();
            this.btnBolgeYeni = new DevExpress.XtraEditors.SimpleButton();
            this.btnBolgeKaydet = new DevExpress.XtraEditors.SimpleButton();
            this.btnBolgeSil = new DevExpress.XtraEditors.SimpleButton();
            this.btnBolgeAktiflik = new DevExpress.XtraEditors.SimpleButton();
            this.gridCadirTanim = new DevExpress.XtraGrid.GridControl();
            this.viewCadirTanim = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colTKod = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTBolge = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTKapasite = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTDoluluk = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTDurum = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTErisilebilir = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTSaglik = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTIsitmali = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTKayitDurumu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbCadirBolge = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.spnSira = new DevExpress.XtraEditors.SpinEdit();
            this.spnNumara = new DevExpress.XtraEditors.SpinEdit();
            this.spnKapasite = new DevExpress.XtraEditors.SpinEdit();
            this.cmbCadirDurum = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.chkErisilebilir = new DevExpress.XtraEditors.CheckEdit();
            this.chkSaglik = new DevExpress.XtraEditors.CheckEdit();
            this.chkIsitmali = new DevExpress.XtraEditors.CheckEdit();
            this.btnCadirYeni = new DevExpress.XtraEditors.SimpleButton();
            this.btnCadirKaydet = new DevExpress.XtraEditors.SimpleButton();
            this.btnCadirSil = new DevExpress.XtraEditors.SimpleButton();
            this.btnCadirAktiflik = new DevExpress.XtraEditors.SimpleButton();
            this.lciBolgeler = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBolgeKod = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBolgeAd = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBolgeAciklama = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBolgeYeni = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBolgeKaydet = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBolgeSil = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBolgeAktiflik = new DevExpress.XtraLayout.LayoutControlItem();
            this.grpBolgeDuzen = new DevExpress.XtraLayout.LayoutControlGroup();
            this.bosBolge = new DevExpress.XtraLayout.EmptySpaceItem();
            this.grpBolgeler = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciCadirTanim = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCadirBolge = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSira = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciNumara = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKapasite = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCadirDurum = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciErisilebilir = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSaglik = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIsitmali = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCadirYeni = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCadirKaydet = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCadirSil = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCadirAktiflik = new DevExpress.XtraLayout.LayoutControlItem();
            this.bosCadirDuzen = new DevExpress.XtraLayout.EmptySpaceItem();
            this.grpCadirDuzen = new DevExpress.XtraLayout.LayoutControlGroup();
            this.bosCadir = new DevExpress.XtraLayout.EmptySpaceItem();
            this.grpCadirlar = new DevExpress.XtraLayout.LayoutControlGroup();
            this.RootTanim = new DevExpress.XtraLayout.LayoutControlGroup();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabPlan.SuspendLayout();
            this.tabTanimlar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPlan)).BeginInit();
            this.layoutPlan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbBolge.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCadirdakiler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewCadirdakiler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lkAile.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYenile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciLejant)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPlanOzet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPlan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirOzet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirdakiler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTasi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBekleyeniYerlestir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosCadirDugmeler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpCadir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOneriPaneli)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpYerlestir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabSag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootPlan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTanim)).BeginInit();
            this.layoutTanim.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridBolgeler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewBolgeler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBolgeKod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBolgeAd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBolgeAciklama.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCadirTanim)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewCadirTanim)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCadirBolge.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnSira.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnNumara.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnKapasite.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCadirDurum.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkErisilebilir.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSaglik.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsitmali.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolgeler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolgeKod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolgeAd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolgeAciklama)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolgeYeni)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolgeKaydet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolgeSil)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolgeAktiflik)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpBolgeDuzen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosBolge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpBolgeler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirTanim)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirBolge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSira)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNumara)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKapasite)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirDurum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciErisilebilir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSaglik)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIsitmali)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirYeni)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirKaydet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirSil)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirAktiflik)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosCadirDuzen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpCadirDuzen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosCadir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpCadirlar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootTanim)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedTabPage = this.tabPlan;
            this.tabControl.Size = new System.Drawing.Size(1280, 760);
            this.tabControl.TabIndex = 0;
            this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabPlan,
            this.tabTanimlar});
            // 
            // tabPlan
            // 
            this.tabPlan.Controls.Add(this.layoutPlan);
            this.tabPlan.Name = "tabPlan";
            this.tabPlan.Size = new System.Drawing.Size(1278, 735);
            this.tabPlan.Text = "Çadır planı ve yerleşim";
            // 
            // tabTanimlar
            // 
            this.tabTanimlar.Controls.Add(this.layoutTanim);
            this.tabTanimlar.Name = "tabTanimlar";
            this.tabTanimlar.Size = new System.Drawing.Size(1278, 735);
            this.tabTanimlar.Text = "Çadır ve bölge tanımları";
            // 
            // layoutPlan
            // 
            this.layoutPlan.Controls.Add(this.cmbBolge);
            this.layoutPlan.Controls.Add(this.btnYenile);
            this.layoutPlan.Controls.Add(this.lblLejant);
            this.layoutPlan.Controls.Add(this.lblPlanOzet);
            this.layoutPlan.Controls.Add(this.tileCadirlar);
            this.layoutPlan.Controls.Add(this.lblCadirOzet);
            this.layoutPlan.Controls.Add(this.gridCadirdakiler);
            this.layoutPlan.Controls.Add(this.btnTasi);
            this.layoutPlan.Controls.Add(this.btnBekleyeniYerlestir);
            this.layoutPlan.Controls.Add(this.lkAile);
            this.layoutPlan.Controls.Add(this.btnOner);
            this.layoutPlan.Controls.Add(this.oneriPaneli);
            this.layoutPlan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutPlan.Location = new System.Drawing.Point(0, 0);
            this.layoutPlan.Name = "layoutPlan";
            this.layoutPlan.Root = this.RootPlan;
            this.layoutPlan.Size = new System.Drawing.Size(1278, 735);
            this.layoutPlan.TabIndex = 0;
            // 
            // cmbBolge
            // 
            this.cmbBolge.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbBolge.Location = new System.Drawing.Point(56, 12);
            this.cmbBolge.Name = "cmbBolge";
            this.cmbBolge.Size = new System.Drawing.Size(244, 22);
            this.cmbBolge.StyleController = this.layoutPlan;
            this.cmbBolge.TabIndex = 1;
            this.cmbBolge.EditValueChanged += new System.EventHandler(this.cmbBolge_EditValueChanged);
            // 
            // btnYenile
            // 
            this.btnYenile.Location = new System.Drawing.Point(304, 12);
            this.btnYenile.Name = "btnYenile";
            this.btnYenile.Text = "Yenile";
            this.btnYenile.Size = new System.Drawing.Size(106, 22);
            this.btnYenile.StyleController = this.layoutPlan;
            this.btnYenile.TabIndex = 2;
            this.btnYenile.Click += new System.EventHandler(this.btnYenile_Click);
            // 
            // lblLejant
            // 
            this.lblLejant.Location = new System.Drawing.Point(414, 12);
            this.lblLejant.Name = "lblLejant";
            this.lblLejant.AllowHtmlString = true;
            this.lblLejant.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblLejant.Size = new System.Drawing.Size(852, 22);
            this.lblLejant.StyleController = this.layoutPlan;
            this.lblLejant.TabIndex = 3;
            // 
            // lblPlanOzet
            // 
            this.lblPlanOzet.Location = new System.Drawing.Point(12, 38);
            this.lblPlanOzet.Name = "lblPlanOzet";
            this.lblPlanOzet.AllowHtmlString = true;
            this.lblPlanOzet.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPlanOzet.Size = new System.Drawing.Size(636, 16);
            this.lblPlanOzet.StyleController = this.layoutPlan;
            this.lblPlanOzet.TabIndex = 4;
            // 
            // tileCadirlar
            // 
            this.tileCadirlar.AllowDrag = false;
            this.tileCadirlar.AllowSelectedItem = true;
            this.tileCadirlar.AllowSelectedItemBorder = true;
            this.tileCadirlar.IndentBetweenItems = 6;
            this.tileCadirlar.ItemSize = 88;
            this.tileCadirlar.Location = new System.Drawing.Point(12, 58);
            this.tileCadirlar.Name = "tileCadirlar";
            this.tileCadirlar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tileCadirlar.Padding = new System.Windows.Forms.Padding(8);
            this.tileCadirlar.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(71)))), ((int)(((byte)(161)))));
            this.tileCadirlar.ShowGroupText = true;
            this.tileCadirlar.Size = new System.Drawing.Size(636, 665);
            this.tileCadirlar.TabIndex = 5;
            this.tileCadirlar.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileCadirlar_ItemClick);
            // 
            // lblCadirOzet
            // 
            this.lblCadirOzet.Location = new System.Drawing.Point(666, 69);
            this.lblCadirOzet.Name = "lblCadirOzet";
            this.lblCadirOzet.AllowHtmlString = true;
            this.lblCadirOzet.Appearance.Options.UseTextOptions = true;
            this.lblCadirOzet.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblCadirOzet.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblCadirOzet.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCadirOzet.Size = new System.Drawing.Size(590, 80);
            this.lblCadirOzet.StyleController = this.layoutPlan;
            this.lblCadirOzet.TabIndex = 6;
            // 
            // gridCadirdakiler
            // 
            this.gridCadirdakiler.Location = new System.Drawing.Point(666, 169);
            this.gridCadirdakiler.MainView = this.viewCadirdakiler;
            this.gridCadirdakiler.Name = "gridCadirdakiler";
            this.gridCadirdakiler.Size = new System.Drawing.Size(590, 500);
            this.gridCadirdakiler.TabIndex = 7;
            this.gridCadirdakiler.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewCadirdakiler});
            // 
            // viewCadirdakiler
            // 
            this.viewCadirdakiler.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colCKisiNo,
            this.colCAdSoyad,
            this.colCYas,
            this.colCDurum,
            this.colCAile,
            this.colCBaslangic});
            this.viewCadirdakiler.GridControl = this.gridCadirdakiler;
            this.viewCadirdakiler.Name = "viewCadirdakiler";
            this.viewCadirdakiler.OptionsBehavior.Editable = false;
            this.viewCadirdakiler.OptionsView.ShowGroupPanel = false;
            this.viewCadirdakiler.OptionsView.ShowIndicator = false;
            this.viewCadirdakiler.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            // 
            // colCKisiNo
            // 
            this.colCKisiNo.Caption = "Kişi no";
            this.colCKisiNo.FieldName = "KisiNo";
            this.colCKisiNo.Name = "colCKisiNo";
            this.colCKisiNo.Visible = true;
            this.colCKisiNo.VisibleIndex = 0;
            this.colCKisiNo.Width = 80;
            // 
            // colCAdSoyad
            // 
            this.colCAdSoyad.Caption = "Ad soyad";
            this.colCAdSoyad.FieldName = "AdSoyad";
            this.colCAdSoyad.Name = "colCAdSoyad";
            this.colCAdSoyad.Visible = true;
            this.colCAdSoyad.VisibleIndex = 1;
            this.colCAdSoyad.Width = 150;
            // 
            // colCYas
            // 
            this.colCYas.Caption = "Yaş";
            this.colCYas.FieldName = "Yas";
            this.colCYas.Name = "colCYas";
            this.colCYas.Visible = true;
            this.colCYas.VisibleIndex = 2;
            this.colCYas.Width = 45;
            // 
            // colCDurum
            // 
            this.colCDurum.Caption = "Durum";
            this.colCDurum.FieldName = "Durum";
            this.colCDurum.Name = "colCDurum";
            this.colCDurum.Visible = true;
            this.colCDurum.VisibleIndex = 3;
            this.colCDurum.Width = 80;
            // 
            // colCAile
            // 
            this.colCAile.Caption = "Aile";
            this.colCAile.FieldName = "Aile";
            this.colCAile.Name = "colCAile";
            this.colCAile.Visible = true;
            this.colCAile.VisibleIndex = 4;
            this.colCAile.Width = 160;
            // 
            // colCBaslangic
            // 
            this.colCBaslangic.Caption = "Yerleşim";
            this.colCBaslangic.FieldName = "Baslangic";
            this.colCBaslangic.Name = "colCBaslangic";
            this.colCBaslangic.DisplayFormat.FormatString = "dd.MM.yyyy";
            this.colCBaslangic.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colCBaslangic.Visible = true;
            this.colCBaslangic.VisibleIndex = 5;
            this.colCBaslangic.Width = 90;
            // 
            // btnTasi
            // 
            this.btnTasi.Location = new System.Drawing.Point(666, 269);
            this.btnTasi.Name = "btnTasi";
            this.btnTasi.Text = "Seçilenleri başka çadıra taşı";
            this.btnTasi.Size = new System.Drawing.Size(226, 30);
            this.btnTasi.StyleController = this.layoutPlan;
            this.btnTasi.TabIndex = 8;
            this.btnTasi.Click += new System.EventHandler(this.btnTasi_Click);
            // 
            // btnBekleyeniYerlestir
            // 
            this.btnBekleyeniYerlestir.Location = new System.Drawing.Point(896, 269);
            this.btnBekleyeniYerlestir.Name = "btnBekleyeniYerlestir";
            this.btnBekleyeniYerlestir.Text = "Önerilen bekleyen aileyi yerleştir";
            this.btnBekleyeniYerlestir.Size = new System.Drawing.Size(256, 30);
            this.btnBekleyeniYerlestir.StyleController = this.layoutPlan;
            this.btnBekleyeniYerlestir.TabIndex = 9;
            this.btnBekleyeniYerlestir.Click += new System.EventHandler(this.btnBekleyeniYerlestir_Click);
            // 
            // lkAile
            // 
            this.lkAile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lkAile.Location = new System.Drawing.Point(790, 355);
            this.lkAile.Name = "lkAile";
            this.lkAile.Properties.DisplayMember = "Metin";
            this.lkAile.Properties.ValueMember = "AileId";
            this.lkAile.Properties.NullText = "Aile seçin";
            this.lkAile.Properties.ShowHeader = false;
            this.lkAile.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Metin", "Aile")});
            this.lkAile.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoFilter;
            this.lkAile.Size = new System.Drawing.Size(346, 22);
            this.lkAile.StyleController = this.layoutPlan;
            this.lkAile.TabIndex = 10;
            this.lkAile.EditValueChanged += new System.EventHandler(this.lkAile_EditValueChanged);
            // 
            // btnOner
            // 
            this.btnOner.Location = new System.Drawing.Point(1140, 355);
            this.btnOner.Name = "btnOner";
            this.btnOner.Text = "Çadır öner";
            this.btnOner.Size = new System.Drawing.Size(116, 22);
            this.btnOner.StyleController = this.layoutPlan;
            this.btnOner.TabIndex = 11;
            this.btnOner.Click += new System.EventHandler(this.btnOner_Click);
            // 
            // oneriPaneli
            // 
            this.oneriPaneli.Location = new System.Drawing.Point(666, 130);
            this.oneriPaneli.Name = "oneriPaneli";
            this.oneriPaneli.Size = new System.Drawing.Size(590, 580);
            this.oneriPaneli.TabIndex = 12;
            // 
            // lciBolge
            // 
            this.lciBolge.Control = this.cmbBolge;
            this.lciBolge.Location = new System.Drawing.Point(0, 0);
            this.lciBolge.MaxSize = new System.Drawing.Size(0, 26);
            this.lciBolge.MinSize = new System.Drawing.Size(80, 26);
            this.lciBolge.Name = "lciBolge";
            this.lciBolge.Size = new System.Drawing.Size(292, 26);
            this.lciBolge.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBolge.Text = "Bölge";
            this.lciBolge.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBolge.TextSize = new System.Drawing.Size(40, 13);
            // 
            // lciYenile
            // 
            this.lciYenile.Control = this.btnYenile;
            this.lciYenile.Location = new System.Drawing.Point(292, 0);
            this.lciYenile.MaxSize = new System.Drawing.Size(110, 26);
            this.lciYenile.MinSize = new System.Drawing.Size(80, 26);
            this.lciYenile.Name = "lciYenile";
            this.lciYenile.Size = new System.Drawing.Size(110, 26);
            this.lciYenile.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciYenile.TextVisible = false;
            // 
            // lciLejant
            // 
            this.lciLejant.Control = this.lblLejant;
            this.lciLejant.Location = new System.Drawing.Point(402, 0);
            this.lciLejant.MaxSize = new System.Drawing.Size(0, 26);
            this.lciLejant.MinSize = new System.Drawing.Size(200, 26);
            this.lciLejant.Name = "lciLejant";
            this.lciLejant.Size = new System.Drawing.Size(856, 26);
            this.lciLejant.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciLejant.TextVisible = false;
            // 
            // lciPlanOzet
            // 
            this.lciPlanOzet.Control = this.lblPlanOzet;
            this.lciPlanOzet.Location = new System.Drawing.Point(0, 26);
            this.lciPlanOzet.MaxSize = new System.Drawing.Size(0, 20);
            this.lciPlanOzet.MinSize = new System.Drawing.Size(100, 20);
            this.lciPlanOzet.Name = "lciPlanOzet";
            this.lciPlanOzet.Size = new System.Drawing.Size(640, 20);
            this.lciPlanOzet.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciPlanOzet.TextVisible = false;
            // 
            // lciPlan
            // 
            this.lciPlan.Control = this.tileCadirlar;
            this.lciPlan.Location = new System.Drawing.Point(0, 46);
            this.lciPlan.MinSize = new System.Drawing.Size(300, 200);
            this.lciPlan.Name = "lciPlan";
            this.lciPlan.Size = new System.Drawing.Size(640, 667);
            this.lciPlan.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciPlan.TextVisible = false;
            // 
            // lciCadirOzet
            // 
            this.lciCadirOzet.Control = this.lblCadirOzet;
            this.lciCadirOzet.Location = new System.Drawing.Point(0, 0);
            this.lciCadirOzet.MaxSize = new System.Drawing.Size(0, 72);
            this.lciCadirOzet.MinSize = new System.Drawing.Size(200, 72);
            this.lciCadirOzet.Name = "lciCadirOzet";
            this.lciCadirOzet.Size = new System.Drawing.Size(594, 72);
            this.lciCadirOzet.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCadirOzet.TextVisible = false;
            // 
            // lciCadirdakiler
            // 
            this.lciCadirdakiler.Control = this.gridCadirdakiler;
            this.lciCadirdakiler.Location = new System.Drawing.Point(0, 72);
            this.lciCadirdakiler.MinSize = new System.Drawing.Size(200, 80);
            this.lciCadirdakiler.Name = "lciCadirdakiler";
            this.lciCadirdakiler.Size = new System.Drawing.Size(594, 529);
            this.lciCadirdakiler.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCadirdakiler.Text = "Çadırdaki kişiler (taşımak için seçin)";
            this.lciCadirdakiler.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciCadirdakiler.TextSize = new System.Drawing.Size(100, 13);
            // 
            // lciTasi
            // 
            this.lciTasi.Control = this.btnTasi;
            this.lciTasi.Location = new System.Drawing.Point(0, 601);
            this.lciTasi.MaxSize = new System.Drawing.Size(0, 34);
            this.lciTasi.MinSize = new System.Drawing.Size(80, 34);
            this.lciTasi.Name = "lciTasi";
            this.lciTasi.Size = new System.Drawing.Size(230, 34);
            this.lciTasi.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciTasi.TextVisible = false;
            // 
            // lciBekleyeniYerlestir
            // 
            this.lciBekleyeniYerlestir.Control = this.btnBekleyeniYerlestir;
            this.lciBekleyeniYerlestir.Location = new System.Drawing.Point(230, 601);
            this.lciBekleyeniYerlestir.MaxSize = new System.Drawing.Size(0, 34);
            this.lciBekleyeniYerlestir.MinSize = new System.Drawing.Size(80, 34);
            this.lciBekleyeniYerlestir.Name = "lciBekleyeniYerlestir";
            this.lciBekleyeniYerlestir.Size = new System.Drawing.Size(260, 34);
            this.lciBekleyeniYerlestir.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBekleyeniYerlestir.TextVisible = false;
            // 
            // bosCadirDugmeler
            // 
            this.bosCadirDugmeler.Location = new System.Drawing.Point(490, 601);
            this.bosCadirDugmeler.Name = "bosCadirDugmeler";
            this.bosCadirDugmeler.Size = new System.Drawing.Size(104, 34);
            // 
            // grpCadir
            // 
            this.grpCadir.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciCadirOzet,
            this.lciCadirdakiler,
            this.lciTasi,
            this.lciBekleyeniYerlestir,
            this.bosCadirDugmeler});
            this.grpCadir.Location = new System.Drawing.Point(0, 0);
            this.grpCadir.Name = "grpCadir";
            this.grpCadir.Size = new System.Drawing.Size(594, 635);
            this.grpCadir.Text = "Seçili çadır";
            // 
            // lciAile
            // 
            this.lciAile.Control = this.lkAile;
            this.lciAile.Location = new System.Drawing.Point(0, 0);
            this.lciAile.MaxSize = new System.Drawing.Size(0, 26);
            this.lciAile.MinSize = new System.Drawing.Size(200, 26);
            this.lciAile.Name = "lciAile";
            this.lciAile.Size = new System.Drawing.Size(474, 26);
            this.lciAile.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciAile.Text = "Çadır bekleyen aile";
            this.lciAile.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciAile.TextSize = new System.Drawing.Size(110, 13);
            // 
            // lciOner
            // 
            this.lciOner.Control = this.btnOner;
            this.lciOner.Location = new System.Drawing.Point(474, 0);
            this.lciOner.MaxSize = new System.Drawing.Size(120, 26);
            this.lciOner.MinSize = new System.Drawing.Size(80, 26);
            this.lciOner.Name = "lciOner";
            this.lciOner.Size = new System.Drawing.Size(120, 26);
            this.lciOner.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciOner.TextVisible = false;
            // 
            // lciOneriPaneli
            // 
            this.lciOneriPaneli.Control = this.oneriPaneli;
            this.lciOneriPaneli.Location = new System.Drawing.Point(0, 26);
            this.lciOneriPaneli.MinSize = new System.Drawing.Size(300, 300);
            this.lciOneriPaneli.Name = "lciOneriPaneli";
            this.lciOneriPaneli.Size = new System.Drawing.Size(594, 609);
            this.lciOneriPaneli.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciOneriPaneli.TextVisible = false;
            // 
            // grpYerlestir
            // 
            this.grpYerlestir.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciAile,
            this.lciOner,
            this.lciOneriPaneli});
            this.grpYerlestir.Location = new System.Drawing.Point(0, 0);
            this.grpYerlestir.Name = "grpYerlestir";
            this.grpYerlestir.Size = new System.Drawing.Size(594, 635);
            this.grpYerlestir.Text = "Aile yerleştir (sistem önerir, görevli onaylar)";
            // 
            // tabSag
            // 
            this.tabSag.Location = new System.Drawing.Point(640, 26);
            this.tabSag.Name = "tabSag";
            this.tabSag.SelectedTabPage = this.grpCadir;
            this.tabSag.SelectedTabPageIndex = 0;
            this.tabSag.Size = new System.Drawing.Size(618, 687);
            this.tabSag.TabPages.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.grpCadir,
            this.grpYerlestir});
            // 
            // RootPlan
            // 
            this.RootPlan.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.RootPlan.GroupBordersVisible = false;
            this.RootPlan.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBolge,
            this.lciYenile,
            this.lciLejant,
            this.lciPlanOzet,
            this.lciPlan,
            this.tabSag});
            this.RootPlan.Name = "RootPlan";
            this.RootPlan.Size = new System.Drawing.Size(1278, 735);
            this.RootPlan.TextVisible = false;
            // 
            // layoutTanim
            // 
            this.layoutTanim.Controls.Add(this.gridBolgeler);
            this.layoutTanim.Controls.Add(this.txtBolgeKod);
            this.layoutTanim.Controls.Add(this.txtBolgeAd);
            this.layoutTanim.Controls.Add(this.txtBolgeAciklama);
            this.layoutTanim.Controls.Add(this.btnBolgeYeni);
            this.layoutTanim.Controls.Add(this.btnBolgeKaydet);
            this.layoutTanim.Controls.Add(this.btnBolgeSil);
            this.layoutTanim.Controls.Add(this.btnBolgeAktiflik);
            this.layoutTanim.Controls.Add(this.gridCadirTanim);
            this.layoutTanim.Controls.Add(this.cmbCadirBolge);
            this.layoutTanim.Controls.Add(this.spnSira);
            this.layoutTanim.Controls.Add(this.spnNumara);
            this.layoutTanim.Controls.Add(this.spnKapasite);
            this.layoutTanim.Controls.Add(this.cmbCadirDurum);
            this.layoutTanim.Controls.Add(this.chkErisilebilir);
            this.layoutTanim.Controls.Add(this.chkSaglik);
            this.layoutTanim.Controls.Add(this.chkIsitmali);
            this.layoutTanim.Controls.Add(this.btnCadirYeni);
            this.layoutTanim.Controls.Add(this.btnCadirKaydet);
            this.layoutTanim.Controls.Add(this.btnCadirSil);
            this.layoutTanim.Controls.Add(this.btnCadirAktiflik);
            this.layoutTanim.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutTanim.Location = new System.Drawing.Point(0, 0);
            this.layoutTanim.Name = "layoutTanim";
            this.layoutTanim.Root = this.RootTanim;
            this.layoutTanim.Size = new System.Drawing.Size(1278, 735);
            this.layoutTanim.TabIndex = 0;
            // 
            // gridBolgeler
            // 
            this.gridBolgeler.Location = new System.Drawing.Point(24, 45);
            this.gridBolgeler.MainView = this.viewBolgeler;
            this.gridBolgeler.Name = "gridBolgeler";
            this.gridBolgeler.Size = new System.Drawing.Size(392, 366);
            this.gridBolgeler.TabIndex = 1;
            this.gridBolgeler.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewBolgeler});
            // 
            // viewBolgeler
            // 
            this.viewBolgeler.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colBKod,
            this.colBAd,
            this.colBAciklama,
            this.colBCadirSayisi,
            this.colBKayitDurumu});
            this.viewBolgeler.GridControl = this.gridBolgeler;
            this.viewBolgeler.Name = "viewBolgeler";
            this.viewBolgeler.OptionsBehavior.Editable = false;
            this.viewBolgeler.OptionsView.ShowGroupPanel = false;
            this.viewBolgeler.OptionsView.ShowIndicator = false;
            this.viewBolgeler.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.viewBolgeler_FocusedRowChanged);
            // 
            // colBKod
            // 
            this.colBKod.Caption = "Kod";
            this.colBKod.FieldName = "Kod";
            this.colBKod.Name = "colBKod";
            this.colBKod.Visible = true;
            this.colBKod.VisibleIndex = 0;
            this.colBKod.Width = 45;
            // 
            // colBAd
            // 
            this.colBAd.Caption = "Ad";
            this.colBAd.FieldName = "Ad";
            this.colBAd.Name = "colBAd";
            this.colBAd.Visible = true;
            this.colBAd.VisibleIndex = 1;
            this.colBAd.Width = 130;
            // 
            // colBAciklama
            // 
            this.colBAciklama.Caption = "Açıklama";
            this.colBAciklama.FieldName = "Aciklama";
            this.colBAciklama.Name = "colBAciklama";
            this.colBAciklama.Visible = true;
            this.colBAciklama.VisibleIndex = 2;
            this.colBAciklama.Width = 110;
            // 
            // colBCadirSayisi
            // 
            this.colBCadirSayisi.Caption = "Çadır";
            this.colBCadirSayisi.FieldName = "CadirSayisi";
            this.colBCadirSayisi.Name = "colBCadirSayisi";
            this.colBCadirSayisi.Visible = true;
            this.colBCadirSayisi.VisibleIndex = 3;
            this.colBCadirSayisi.Width = 50;
            // 
            // colBKayitDurumu
            // 
            this.colBKayitDurumu.Caption = "Kayıt";
            this.colBKayitDurumu.FieldName = "KayitDurumu";
            this.colBKayitDurumu.Name = "colBKayitDurumu";
            this.colBKayitDurumu.Visible = true;
            this.colBKayitDurumu.VisibleIndex = 4;
            this.colBKayitDurumu.Width = 55;
            // 
            // txtBolgeKod
            // 
            this.txtBolgeKod.Location = new System.Drawing.Point(106, 456);
            this.txtBolgeKod.Name = "txtBolgeKod";
            this.txtBolgeKod.Properties.MaxLength = 5;
            this.txtBolgeKod.Size = new System.Drawing.Size(298, 22);
            this.txtBolgeKod.StyleController = this.layoutTanim;
            this.txtBolgeKod.TabIndex = 2;
            // 
            // txtBolgeAd
            // 
            this.txtBolgeAd.Location = new System.Drawing.Point(106, 482);
            this.txtBolgeAd.Name = "txtBolgeAd";
            this.txtBolgeAd.Properties.MaxLength = 100;
            this.txtBolgeAd.Size = new System.Drawing.Size(298, 22);
            this.txtBolgeAd.StyleController = this.layoutTanim;
            this.txtBolgeAd.TabIndex = 3;
            // 
            // txtBolgeAciklama
            // 
            this.txtBolgeAciklama.Location = new System.Drawing.Point(106, 508);
            this.txtBolgeAciklama.Name = "txtBolgeAciklama";
            this.txtBolgeAciklama.Properties.MaxLength = 500;
            this.txtBolgeAciklama.Size = new System.Drawing.Size(298, 22);
            this.txtBolgeAciklama.StyleController = this.layoutTanim;
            this.txtBolgeAciklama.TabIndex = 4;
            // 
            // btnBolgeYeni
            // 
            this.btnBolgeYeni.Location = new System.Drawing.Point(36, 534);
            this.btnBolgeYeni.Name = "btnBolgeYeni";
            this.btnBolgeYeni.Text = "Yeni";
            this.btnBolgeYeni.Size = new System.Drawing.Size(90, 30);
            this.btnBolgeYeni.StyleController = this.layoutTanim;
            this.btnBolgeYeni.TabIndex = 5;
            this.btnBolgeYeni.Click += new System.EventHandler(this.btnBolgeYeni_Click);
            // 
            // btnBolgeKaydet
            // 
            this.btnBolgeKaydet.Location = new System.Drawing.Point(130, 534);
            this.btnBolgeKaydet.Name = "btnBolgeKaydet";
            this.btnBolgeKaydet.Text = "Kaydet";
            this.btnBolgeKaydet.Size = new System.Drawing.Size(90, 30);
            this.btnBolgeKaydet.StyleController = this.layoutTanim;
            this.btnBolgeKaydet.TabIndex = 6;
            this.btnBolgeKaydet.Click += new System.EventHandler(this.btnBolgeKaydet_Click);
            // 
            // btnBolgeSil
            // 
            this.btnBolgeSil.Location = new System.Drawing.Point(224, 534);
            this.btnBolgeSil.Name = "btnBolgeSil";
            this.btnBolgeSil.Text = "Sil";
            this.btnBolgeSil.Size = new System.Drawing.Size(90, 30);
            this.btnBolgeSil.StyleController = this.layoutTanim;
            this.btnBolgeSil.TabIndex = 7;
            this.btnBolgeSil.Click += new System.EventHandler(this.btnBolgeSil_Click);
            // 
            // btnBolgeAktiflik
            // 
            this.btnBolgeAktiflik.Location = new System.Drawing.Point(318, 534);
            this.btnBolgeAktiflik.Name = "btnBolgeAktiflik";
            this.btnBolgeAktiflik.Text = "Pasif yap";
            this.btnBolgeAktiflik.Size = new System.Drawing.Size(86, 30);
            this.btnBolgeAktiflik.StyleController = this.layoutTanim;
            this.btnBolgeAktiflik.TabIndex = 8;
            this.btnBolgeAktiflik.Click += new System.EventHandler(this.btnBolgeAktiflik_Click);
            // 
            // gridCadirTanim
            // 
            this.gridCadirTanim.Location = new System.Drawing.Point(464, 45);
            this.gridCadirTanim.MainView = this.viewCadirTanim;
            this.gridCadirTanim.Name = "gridCadirTanim";
            this.gridCadirTanim.Size = new System.Drawing.Size(770, 366);
            this.gridCadirTanim.TabIndex = 9;
            this.gridCadirTanim.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewCadirTanim});
            // 
            // viewCadirTanim
            // 
            this.viewCadirTanim.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colTKod,
            this.colTBolge,
            this.colTKapasite,
            this.colTDoluluk,
            this.colTDurum,
            this.colTErisilebilir,
            this.colTSaglik,
            this.colTIsitmali,
            this.colTKayitDurumu});
            this.viewCadirTanim.GridControl = this.gridCadirTanim;
            this.viewCadirTanim.Name = "viewCadirTanim";
            this.viewCadirTanim.OptionsBehavior.Editable = false;
            this.viewCadirTanim.OptionsView.ShowGroupPanel = false;
            this.viewCadirTanim.OptionsView.ShowIndicator = false;
            this.viewCadirTanim.OptionsView.ShowAutoFilterRow = true;
            this.viewCadirTanim.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.viewCadirTanim_FocusedRowChanged);
            // 
            // colTKod
            // 
            this.colTKod.Caption = "Çadır";
            this.colTKod.FieldName = "Kod";
            this.colTKod.Name = "colTKod";
            this.colTKod.Visible = true;
            this.colTKod.VisibleIndex = 0;
            this.colTKod.Width = 60;
            // 
            // colTBolge
            // 
            this.colTBolge.Caption = "Bölge";
            this.colTBolge.FieldName = "Bolge";
            this.colTBolge.Name = "colTBolge";
            this.colTBolge.Visible = true;
            this.colTBolge.VisibleIndex = 1;
            this.colTBolge.Width = 140;
            // 
            // colTKapasite
            // 
            this.colTKapasite.Caption = "Kapasite";
            this.colTKapasite.FieldName = "Kapasite";
            this.colTKapasite.Name = "colTKapasite";
            this.colTKapasite.Visible = true;
            this.colTKapasite.VisibleIndex = 2;
            this.colTKapasite.Width = 60;
            // 
            // colTDoluluk
            // 
            this.colTDoluluk.Caption = "Dolu";
            this.colTDoluluk.FieldName = "Doluluk";
            this.colTDoluluk.Name = "colTDoluluk";
            this.colTDoluluk.Visible = true;
            this.colTDoluluk.VisibleIndex = 3;
            this.colTDoluluk.Width = 50;
            // 
            // colTDurum
            // 
            this.colTDurum.Caption = "Durum";
            this.colTDurum.FieldName = "Durum";
            this.colTDurum.Name = "colTDurum";
            this.colTDurum.Visible = true;
            this.colTDurum.VisibleIndex = 4;
            this.colTDurum.Width = 90;
            // 
            // colTErisilebilir
            // 
            this.colTErisilebilir.Caption = "Erişilebilir";
            this.colTErisilebilir.FieldName = "Erisilebilir";
            this.colTErisilebilir.Name = "colTErisilebilir";
            this.colTErisilebilir.Visible = true;
            this.colTErisilebilir.VisibleIndex = 5;
            this.colTErisilebilir.Width = 70;
            // 
            // colTSaglik
            // 
            this.colTSaglik.Caption = "Sağlık noktasına yakın";
            this.colTSaglik.FieldName = "SaglikNoktasinaYakin";
            this.colTSaglik.Name = "colTSaglik";
            this.colTSaglik.Visible = true;
            this.colTSaglik.VisibleIndex = 6;
            this.colTSaglik.Width = 120;
            // 
            // colTIsitmali
            // 
            this.colTIsitmali.Caption = "Isıtmalı";
            this.colTIsitmali.FieldName = "Isitmali";
            this.colTIsitmali.Name = "colTIsitmali";
            this.colTIsitmali.Visible = true;
            this.colTIsitmali.VisibleIndex = 7;
            this.colTIsitmali.Width = 60;
            // 
            // colTKayitDurumu
            // 
            this.colTKayitDurumu.Caption = "Kayıt";
            this.colTKayitDurumu.FieldName = "KayitDurumu";
            this.colTKayitDurumu.Name = "colTKayitDurumu";
            this.colTKayitDurumu.Visible = true;
            this.colTKayitDurumu.VisibleIndex = 8;
            this.colTKayitDurumu.Width = 60;
            // 
            // cmbCadirBolge
            // 
            this.cmbCadirBolge.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbCadirBolge.Location = new System.Drawing.Point(536, 456);
            this.cmbCadirBolge.Name = "cmbCadirBolge";
            this.cmbCadirBolge.Size = new System.Drawing.Size(224, 22);
            this.cmbCadirBolge.StyleController = this.layoutTanim;
            this.cmbCadirBolge.TabIndex = 10;
            // 
            // spnSira
            // 
            this.spnSira.Location = new System.Drawing.Point(834, 456);
            this.spnSira.Name = "spnSira";
            this.spnSira.Properties.IsFloatValue = false;
            this.spnSira.Properties.MaxValue = new decimal(new int[] {99, 0, 0, 0});
            this.spnSira.Properties.MinValue = new decimal(new int[] {1, 0, 0, 0});
            this.spnSira.EditValue = new decimal(new int[] {1, 0, 0, 0});
            this.spnSira.Size = new System.Drawing.Size(80, 22);
            this.spnSira.StyleController = this.layoutTanim;
            this.spnSira.TabIndex = 11;
            // 
            // spnNumara
            // 
            this.spnNumara.Location = new System.Drawing.Point(988, 456);
            this.spnNumara.Name = "spnNumara";
            this.spnNumara.Properties.IsFloatValue = false;
            this.spnNumara.Properties.MaxValue = new decimal(new int[] {99, 0, 0, 0});
            this.spnNumara.Properties.MinValue = new decimal(new int[] {1, 0, 0, 0});
            this.spnNumara.EditValue = new decimal(new int[] {1, 0, 0, 0});
            this.spnNumara.Size = new System.Drawing.Size(80, 22);
            this.spnNumara.StyleController = this.layoutTanim;
            this.spnNumara.TabIndex = 12;
            // 
            // spnKapasite
            // 
            this.spnKapasite.Location = new System.Drawing.Point(1142, 456);
            this.spnKapasite.Name = "spnKapasite";
            this.spnKapasite.Properties.IsFloatValue = false;
            this.spnKapasite.Properties.MaxValue = new decimal(new int[] {50, 0, 0, 0});
            this.spnKapasite.Properties.MinValue = new decimal(new int[] {1, 0, 0, 0});
            this.spnKapasite.EditValue = new decimal(new int[] {1, 0, 0, 0});
            this.spnKapasite.Size = new System.Drawing.Size(80, 22);
            this.spnKapasite.StyleController = this.layoutTanim;
            this.spnKapasite.TabIndex = 13;
            // 
            // cmbCadirDurum
            // 
            this.cmbCadirDurum.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbCadirDurum.Location = new System.Drawing.Point(536, 482);
            this.cmbCadirDurum.Name = "cmbCadirDurum";
            this.cmbCadirDurum.Size = new System.Drawing.Size(224, 22);
            this.cmbCadirDurum.StyleController = this.layoutTanim;
            this.cmbCadirDurum.TabIndex = 14;
            // 
            // chkErisilebilir
            // 
            this.chkErisilebilir.Location = new System.Drawing.Point(764, 482);
            this.chkErisilebilir.Name = "chkErisilebilir";
            this.chkErisilebilir.Properties.Caption = "Erişilebilir";
            this.chkErisilebilir.Size = new System.Drawing.Size(146, 22);
            this.chkErisilebilir.StyleController = this.layoutTanim;
            this.chkErisilebilir.TabIndex = 15;
            // 
            // chkSaglik
            // 
            this.chkSaglik.Location = new System.Drawing.Point(914, 482);
            this.chkSaglik.Name = "chkSaglik";
            this.chkSaglik.Properties.Caption = "Sağlık noktasına yakın";
            this.chkSaglik.Size = new System.Drawing.Size(186, 22);
            this.chkSaglik.StyleController = this.layoutTanim;
            this.chkSaglik.TabIndex = 16;
            // 
            // chkIsitmali
            // 
            this.chkIsitmali.Location = new System.Drawing.Point(1104, 482);
            this.chkIsitmali.Name = "chkIsitmali";
            this.chkIsitmali.Properties.Caption = "Isıtmalı";
            this.chkIsitmali.Size = new System.Drawing.Size(118, 22);
            this.chkIsitmali.StyleController = this.layoutTanim;
            this.chkIsitmali.TabIndex = 17;
            // 
            // btnCadirYeni
            // 
            this.btnCadirYeni.Location = new System.Drawing.Point(476, 508);
            this.btnCadirYeni.Name = "btnCadirYeni";
            this.btnCadirYeni.Text = "Yeni";
            this.btnCadirYeni.Size = new System.Drawing.Size(140, 30);
            this.btnCadirYeni.StyleController = this.layoutTanim;
            this.btnCadirYeni.TabIndex = 18;
            this.btnCadirYeni.Click += new System.EventHandler(this.btnCadirYeni_Click);
            // 
            // btnCadirKaydet
            // 
            this.btnCadirKaydet.Location = new System.Drawing.Point(620, 508);
            this.btnCadirKaydet.Name = "btnCadirKaydet";
            this.btnCadirKaydet.Text = "Kaydet";
            this.btnCadirKaydet.Size = new System.Drawing.Size(140, 30);
            this.btnCadirKaydet.StyleController = this.layoutTanim;
            this.btnCadirKaydet.TabIndex = 19;
            this.btnCadirKaydet.Click += new System.EventHandler(this.btnCadirKaydet_Click);
            // 
            // btnCadirSil
            // 
            this.btnCadirSil.Location = new System.Drawing.Point(764, 508);
            this.btnCadirSil.Name = "btnCadirSil";
            this.btnCadirSil.Text = "Sil";
            this.btnCadirSil.Size = new System.Drawing.Size(140, 30);
            this.btnCadirSil.StyleController = this.layoutTanim;
            this.btnCadirSil.TabIndex = 20;
            this.btnCadirSil.Click += new System.EventHandler(this.btnCadirSil_Click);
            // 
            // btnCadirAktiflik
            // 
            this.btnCadirAktiflik.Location = new System.Drawing.Point(908, 508);
            this.btnCadirAktiflik.Name = "btnCadirAktiflik";
            this.btnCadirAktiflik.Text = "Pasif yap";
            this.btnCadirAktiflik.Size = new System.Drawing.Size(140, 30);
            this.btnCadirAktiflik.StyleController = this.layoutTanim;
            this.btnCadirAktiflik.TabIndex = 21;
            this.btnCadirAktiflik.Click += new System.EventHandler(this.btnCadirAktiflik_Click);
            // 
            // lciBolgeler
            // 
            this.lciBolgeler.Control = this.gridBolgeler;
            this.lciBolgeler.Location = new System.Drawing.Point(0, 0);
            this.lciBolgeler.MinSize = new System.Drawing.Size(200, 120);
            this.lciBolgeler.Name = "lciBolgeler";
            this.lciBolgeler.Size = new System.Drawing.Size(396, 370);
            this.lciBolgeler.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBolgeler.TextVisible = false;
            // 
            // lciBolgeKod
            // 
            this.lciBolgeKod.Control = this.txtBolgeKod;
            this.lciBolgeKod.Location = new System.Drawing.Point(0, 0);
            this.lciBolgeKod.MaxSize = new System.Drawing.Size(0, 26);
            this.lciBolgeKod.MinSize = new System.Drawing.Size(80, 26);
            this.lciBolgeKod.Name = "lciBolgeKod";
            this.lciBolgeKod.Size = new System.Drawing.Size(372, 26);
            this.lciBolgeKod.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBolgeKod.Text = "Kod";
            this.lciBolgeKod.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBolgeKod.TextSize = new System.Drawing.Size(66, 13);
            // 
            // lciBolgeAd
            // 
            this.lciBolgeAd.Control = this.txtBolgeAd;
            this.lciBolgeAd.Location = new System.Drawing.Point(0, 26);
            this.lciBolgeAd.MaxSize = new System.Drawing.Size(0, 26);
            this.lciBolgeAd.MinSize = new System.Drawing.Size(80, 26);
            this.lciBolgeAd.Name = "lciBolgeAd";
            this.lciBolgeAd.Size = new System.Drawing.Size(372, 26);
            this.lciBolgeAd.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBolgeAd.Text = "Ad";
            this.lciBolgeAd.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBolgeAd.TextSize = new System.Drawing.Size(66, 13);
            // 
            // lciBolgeAciklama
            // 
            this.lciBolgeAciklama.Control = this.txtBolgeAciklama;
            this.lciBolgeAciklama.Location = new System.Drawing.Point(0, 52);
            this.lciBolgeAciklama.MaxSize = new System.Drawing.Size(0, 26);
            this.lciBolgeAciklama.MinSize = new System.Drawing.Size(80, 26);
            this.lciBolgeAciklama.Name = "lciBolgeAciklama";
            this.lciBolgeAciklama.Size = new System.Drawing.Size(372, 26);
            this.lciBolgeAciklama.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBolgeAciklama.Text = "Açıklama";
            this.lciBolgeAciklama.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBolgeAciklama.TextSize = new System.Drawing.Size(66, 13);
            // 
            // lciBolgeYeni
            // 
            this.lciBolgeYeni.Control = this.btnBolgeYeni;
            this.lciBolgeYeni.Location = new System.Drawing.Point(0, 78);
            this.lciBolgeYeni.MaxSize = new System.Drawing.Size(0, 34);
            this.lciBolgeYeni.MinSize = new System.Drawing.Size(80, 34);
            this.lciBolgeYeni.Name = "lciBolgeYeni";
            this.lciBolgeYeni.Size = new System.Drawing.Size(94, 34);
            this.lciBolgeYeni.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBolgeYeni.TextVisible = false;
            // 
            // lciBolgeKaydet
            // 
            this.lciBolgeKaydet.Control = this.btnBolgeKaydet;
            this.lciBolgeKaydet.Location = new System.Drawing.Point(94, 78);
            this.lciBolgeKaydet.MaxSize = new System.Drawing.Size(0, 34);
            this.lciBolgeKaydet.MinSize = new System.Drawing.Size(80, 34);
            this.lciBolgeKaydet.Name = "lciBolgeKaydet";
            this.lciBolgeKaydet.Size = new System.Drawing.Size(94, 34);
            this.lciBolgeKaydet.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBolgeKaydet.TextVisible = false;
            // 
            // lciBolgeSil
            // 
            this.lciBolgeSil.Control = this.btnBolgeSil;
            this.lciBolgeSil.Location = new System.Drawing.Point(188, 78);
            this.lciBolgeSil.MaxSize = new System.Drawing.Size(0, 34);
            this.lciBolgeSil.MinSize = new System.Drawing.Size(80, 34);
            this.lciBolgeSil.Name = "lciBolgeSil";
            this.lciBolgeSil.Size = new System.Drawing.Size(94, 34);
            this.lciBolgeSil.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBolgeSil.TextVisible = false;
            // 
            // lciBolgeAktiflik
            // 
            this.lciBolgeAktiflik.Control = this.btnBolgeAktiflik;
            this.lciBolgeAktiflik.Location = new System.Drawing.Point(282, 78);
            this.lciBolgeAktiflik.MaxSize = new System.Drawing.Size(0, 34);
            this.lciBolgeAktiflik.MinSize = new System.Drawing.Size(80, 34);
            this.lciBolgeAktiflik.Name = "lciBolgeAktiflik";
            this.lciBolgeAktiflik.Size = new System.Drawing.Size(90, 34);
            this.lciBolgeAktiflik.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBolgeAktiflik.TextVisible = false;
            // 
            // grpBolgeDuzen
            // 
            this.grpBolgeDuzen.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBolgeKod,
            this.lciBolgeAd,
            this.lciBolgeAciklama,
            this.lciBolgeYeni,
            this.lciBolgeKaydet,
            this.lciBolgeSil,
            this.lciBolgeAktiflik});
            this.grpBolgeDuzen.Location = new System.Drawing.Point(0, 370);
            this.grpBolgeDuzen.Name = "grpBolgeDuzen";
            this.grpBolgeDuzen.Size = new System.Drawing.Size(396, 155);
            this.grpBolgeDuzen.Text = "Yeni bölge";
            // 
            // bosBolge
            // 
            this.bosBolge.Location = new System.Drawing.Point(0, 525);
            this.bosBolge.Name = "bosBolge";
            this.bosBolge.Size = new System.Drawing.Size(396, 143);
            // 
            // grpBolgeler
            // 
            this.grpBolgeler.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBolgeler,
            this.grpBolgeDuzen,
            this.bosBolge});
            this.grpBolgeler.Location = new System.Drawing.Point(0, 0);
            this.grpBolgeler.Name = "grpBolgeler";
            this.grpBolgeler.Size = new System.Drawing.Size(420, 713);
            this.grpBolgeler.Text = "Bölgeler";
            // 
            // lciCadirTanim
            // 
            this.lciCadirTanim.Control = this.gridCadirTanim;
            this.lciCadirTanim.Location = new System.Drawing.Point(0, 0);
            this.lciCadirTanim.MinSize = new System.Drawing.Size(300, 120);
            this.lciCadirTanim.Name = "lciCadirTanim";
            this.lciCadirTanim.Size = new System.Drawing.Size(774, 370);
            this.lciCadirTanim.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCadirTanim.TextVisible = false;
            // 
            // lciCadirBolge
            // 
            this.lciCadirBolge.Control = this.cmbCadirBolge;
            this.lciCadirBolge.Location = new System.Drawing.Point(0, 0);
            this.lciCadirBolge.MaxSize = new System.Drawing.Size(0, 26);
            this.lciCadirBolge.MinSize = new System.Drawing.Size(80, 26);
            this.lciCadirBolge.Name = "lciCadirBolge";
            this.lciCadirBolge.Size = new System.Drawing.Size(296, 26);
            this.lciCadirBolge.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCadirBolge.Text = "Bölge";
            this.lciCadirBolge.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCadirBolge.TextSize = new System.Drawing.Size(60, 13);
            // 
            // lciSira
            // 
            this.lciSira.Control = this.spnSira;
            this.lciSira.Location = new System.Drawing.Point(296, 0);
            this.lciSira.MaxSize = new System.Drawing.Size(0, 26);
            this.lciSira.MinSize = new System.Drawing.Size(80, 26);
            this.lciSira.Name = "lciSira";
            this.lciSira.Size = new System.Drawing.Size(154, 26);
            this.lciSira.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciSira.Text = "Sıra";
            this.lciSira.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSira.TextSize = new System.Drawing.Size(60, 13);
            // 
            // lciNumara
            // 
            this.lciNumara.Control = this.spnNumara;
            this.lciNumara.Location = new System.Drawing.Point(450, 0);
            this.lciNumara.MaxSize = new System.Drawing.Size(0, 26);
            this.lciNumara.MinSize = new System.Drawing.Size(80, 26);
            this.lciNumara.Name = "lciNumara";
            this.lciNumara.Size = new System.Drawing.Size(154, 26);
            this.lciNumara.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciNumara.Text = "Numara";
            this.lciNumara.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciNumara.TextSize = new System.Drawing.Size(60, 13);
            // 
            // lciKapasite
            // 
            this.lciKapasite.Control = this.spnKapasite;
            this.lciKapasite.Location = new System.Drawing.Point(604, 0);
            this.lciKapasite.MaxSize = new System.Drawing.Size(0, 26);
            this.lciKapasite.MinSize = new System.Drawing.Size(80, 26);
            this.lciKapasite.Name = "lciKapasite";
            this.lciKapasite.Size = new System.Drawing.Size(146, 26);
            this.lciKapasite.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKapasite.Text = "Kapasite";
            this.lciKapasite.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciKapasite.TextSize = new System.Drawing.Size(60, 13);
            // 
            // lciCadirDurum
            // 
            this.lciCadirDurum.Control = this.cmbCadirDurum;
            this.lciCadirDurum.Location = new System.Drawing.Point(0, 26);
            this.lciCadirDurum.MaxSize = new System.Drawing.Size(0, 26);
            this.lciCadirDurum.MinSize = new System.Drawing.Size(80, 26);
            this.lciCadirDurum.Name = "lciCadirDurum";
            this.lciCadirDurum.Size = new System.Drawing.Size(296, 26);
            this.lciCadirDurum.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCadirDurum.Text = "Durum";
            this.lciCadirDurum.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCadirDurum.TextSize = new System.Drawing.Size(60, 13);
            // 
            // lciErisilebilir
            // 
            this.lciErisilebilir.Control = this.chkErisilebilir;
            this.lciErisilebilir.Location = new System.Drawing.Point(296, 26);
            this.lciErisilebilir.MaxSize = new System.Drawing.Size(0, 26);
            this.lciErisilebilir.MinSize = new System.Drawing.Size(80, 26);
            this.lciErisilebilir.Name = "lciErisilebilir";
            this.lciErisilebilir.Size = new System.Drawing.Size(150, 26);
            this.lciErisilebilir.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciErisilebilir.TextVisible = false;
            // 
            // lciSaglik
            // 
            this.lciSaglik.Control = this.chkSaglik;
            this.lciSaglik.Location = new System.Drawing.Point(446, 26);
            this.lciSaglik.MaxSize = new System.Drawing.Size(0, 26);
            this.lciSaglik.MinSize = new System.Drawing.Size(80, 26);
            this.lciSaglik.Name = "lciSaglik";
            this.lciSaglik.Size = new System.Drawing.Size(190, 26);
            this.lciSaglik.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciSaglik.TextVisible = false;
            // 
            // lciIsitmali
            // 
            this.lciIsitmali.Control = this.chkIsitmali;
            this.lciIsitmali.Location = new System.Drawing.Point(636, 26);
            this.lciIsitmali.MaxSize = new System.Drawing.Size(0, 26);
            this.lciIsitmali.MinSize = new System.Drawing.Size(80, 26);
            this.lciIsitmali.Name = "lciIsitmali";
            this.lciIsitmali.Size = new System.Drawing.Size(114, 26);
            this.lciIsitmali.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciIsitmali.TextVisible = false;
            // 
            // lciCadirYeni
            // 
            this.lciCadirYeni.Control = this.btnCadirYeni;
            this.lciCadirYeni.Location = new System.Drawing.Point(0, 52);
            this.lciCadirYeni.MaxSize = new System.Drawing.Size(0, 34);
            this.lciCadirYeni.MinSize = new System.Drawing.Size(80, 34);
            this.lciCadirYeni.Name = "lciCadirYeni";
            this.lciCadirYeni.Size = new System.Drawing.Size(144, 34);
            this.lciCadirYeni.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCadirYeni.TextVisible = false;
            // 
            // lciCadirKaydet
            // 
            this.lciCadirKaydet.Control = this.btnCadirKaydet;
            this.lciCadirKaydet.Location = new System.Drawing.Point(144, 52);
            this.lciCadirKaydet.MaxSize = new System.Drawing.Size(0, 34);
            this.lciCadirKaydet.MinSize = new System.Drawing.Size(80, 34);
            this.lciCadirKaydet.Name = "lciCadirKaydet";
            this.lciCadirKaydet.Size = new System.Drawing.Size(144, 34);
            this.lciCadirKaydet.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCadirKaydet.TextVisible = false;
            // 
            // lciCadirSil
            // 
            this.lciCadirSil.Control = this.btnCadirSil;
            this.lciCadirSil.Location = new System.Drawing.Point(288, 52);
            this.lciCadirSil.MaxSize = new System.Drawing.Size(0, 34);
            this.lciCadirSil.MinSize = new System.Drawing.Size(80, 34);
            this.lciCadirSil.Name = "lciCadirSil";
            this.lciCadirSil.Size = new System.Drawing.Size(144, 34);
            this.lciCadirSil.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCadirSil.TextVisible = false;
            // 
            // lciCadirAktiflik
            // 
            this.lciCadirAktiflik.Control = this.btnCadirAktiflik;
            this.lciCadirAktiflik.Location = new System.Drawing.Point(432, 52);
            this.lciCadirAktiflik.MaxSize = new System.Drawing.Size(0, 34);
            this.lciCadirAktiflik.MinSize = new System.Drawing.Size(80, 34);
            this.lciCadirAktiflik.Name = "lciCadirAktiflik";
            this.lciCadirAktiflik.Size = new System.Drawing.Size(144, 34);
            this.lciCadirAktiflik.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCadirAktiflik.TextVisible = false;
            // 
            // bosCadirDuzen
            // 
            this.bosCadirDuzen.Location = new System.Drawing.Point(576, 52);
            this.bosCadirDuzen.Name = "bosCadirDuzen";
            this.bosCadirDuzen.Size = new System.Drawing.Size(174, 34);
            // 
            // grpCadirDuzen
            // 
            this.grpCadirDuzen.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciCadirBolge,
            this.lciSira,
            this.lciNumara,
            this.lciKapasite,
            this.lciCadirDurum,
            this.lciErisilebilir,
            this.lciSaglik,
            this.lciIsitmali,
            this.lciCadirYeni,
            this.lciCadirKaydet,
            this.lciCadirSil,
            this.lciCadirAktiflik,
            this.bosCadirDuzen});
            this.grpCadirDuzen.Location = new System.Drawing.Point(0, 370);
            this.grpCadirDuzen.Name = "grpCadirDuzen";
            this.grpCadirDuzen.Size = new System.Drawing.Size(774, 131);
            this.grpCadirDuzen.Text = "Yeni çadır";
            // 
            // bosCadir
            // 
            this.bosCadir.Location = new System.Drawing.Point(0, 501);
            this.bosCadir.Name = "bosCadir";
            this.bosCadir.Size = new System.Drawing.Size(774, 167);
            // 
            // grpCadirlar
            // 
            this.grpCadirlar.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciCadirTanim,
            this.grpCadirDuzen,
            this.bosCadir});
            this.grpCadirlar.Location = new System.Drawing.Point(420, 0);
            this.grpCadirlar.Name = "grpCadirlar";
            this.grpCadirlar.Size = new System.Drawing.Size(838, 713);
            this.grpCadirlar.Text = "Çadırlar (kapasite doluluğun altına düşürülemez; dolu çadır kapatılamaz)";
            // 
            // RootTanim
            // 
            this.RootTanim.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.RootTanim.GroupBordersVisible = false;
            this.RootTanim.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.grpBolgeler,
            this.grpCadirlar});
            this.RootTanim.Name = "RootTanim";
            this.RootTanim.Size = new System.Drawing.Size(1278, 735);
            this.RootTanim.TextVisible = false;
            // 
            // CadirPlaniForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 760);
            this.Controls.Add(this.tabControl);
            this.MinimumSize = new System.Drawing.Size(1100, 700);
            this.Name = "CadirPlaniForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Çadır planı";
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabPlan.ResumeLayout(false);
            this.tabTanimlar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutPlan)).EndInit();
            this.layoutPlan.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbBolge.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCadirdakiler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewCadirdakiler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lkAile.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYenile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciLejant)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPlanOzet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPlan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirOzet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirdakiler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTasi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBekleyeniYerlestir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosCadirDugmeler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpCadir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOneriPaneli)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpYerlestir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabSag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootPlan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTanim)).EndInit();
            this.layoutTanim.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridBolgeler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewBolgeler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBolgeKod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBolgeAd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBolgeAciklama.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCadirTanim)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewCadirTanim)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCadirBolge.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnSira.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnNumara.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnKapasite.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCadirDurum.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkErisilebilir.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSaglik.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsitmali.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolgeler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolgeKod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolgeAd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolgeAciklama)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolgeYeni)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolgeKaydet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolgeSil)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBolgeAktiflik)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpBolgeDuzen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosBolge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpBolgeler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirTanim)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirBolge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSira)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNumara)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKapasite)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirDurum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciErisilebilir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSaglik)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIsitmali)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirYeni)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirKaydet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirSil)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCadirAktiflik)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosCadirDuzen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpCadirDuzen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosCadir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpCadirlar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootTanim)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl tabControl;
        private DevExpress.XtraTab.XtraTabPage tabPlan;
        private DevExpress.XtraTab.XtraTabPage tabTanimlar;
        private DevExpress.XtraLayout.LayoutControl layoutPlan;
        private DevExpress.XtraEditors.ImageComboBoxEdit cmbBolge;
        private DevExpress.XtraEditors.SimpleButton btnYenile;
        private DevExpress.XtraEditors.LabelControl lblLejant;
        private DevExpress.XtraEditors.LabelControl lblPlanOzet;
        private DevExpress.XtraEditors.TileControl tileCadirlar;
        private DevExpress.XtraEditors.LabelControl lblCadirOzet;
        private DevExpress.XtraGrid.GridControl gridCadirdakiler;
        private DevExpress.XtraGrid.Views.Grid.GridView viewCadirdakiler;
        private DevExpress.XtraGrid.Columns.GridColumn colCKisiNo;
        private DevExpress.XtraGrid.Columns.GridColumn colCAdSoyad;
        private DevExpress.XtraGrid.Columns.GridColumn colCYas;
        private DevExpress.XtraGrid.Columns.GridColumn colCDurum;
        private DevExpress.XtraGrid.Columns.GridColumn colCAile;
        private DevExpress.XtraGrid.Columns.GridColumn colCBaslangic;
        private DevExpress.XtraEditors.SimpleButton btnTasi;
        private DevExpress.XtraEditors.SimpleButton btnBekleyeniYerlestir;
        private DevExpress.XtraEditors.LookUpEdit lkAile;
        private DevExpress.XtraEditors.SimpleButton btnOner;
        private AfetCadirKent.UI.Formlar.YerlesimOneriPaneli oneriPaneli;
        private DevExpress.XtraLayout.LayoutControlItem lciBolge;
        private DevExpress.XtraLayout.LayoutControlItem lciYenile;
        private DevExpress.XtraLayout.LayoutControlItem lciLejant;
        private DevExpress.XtraLayout.LayoutControlItem lciPlanOzet;
        private DevExpress.XtraLayout.LayoutControlItem lciPlan;
        private DevExpress.XtraLayout.LayoutControlItem lciCadirOzet;
        private DevExpress.XtraLayout.LayoutControlItem lciCadirdakiler;
        private DevExpress.XtraLayout.LayoutControlItem lciTasi;
        private DevExpress.XtraLayout.LayoutControlItem lciBekleyeniYerlestir;
        private DevExpress.XtraLayout.EmptySpaceItem bosCadirDugmeler;
        private DevExpress.XtraLayout.LayoutControlGroup grpCadir;
        private DevExpress.XtraLayout.LayoutControlItem lciAile;
        private DevExpress.XtraLayout.LayoutControlItem lciOner;
        private DevExpress.XtraLayout.LayoutControlItem lciOneriPaneli;
        private DevExpress.XtraLayout.LayoutControlGroup grpYerlestir;
        private DevExpress.XtraLayout.TabbedControlGroup tabSag;
        private DevExpress.XtraLayout.LayoutControlGroup RootPlan;
        private DevExpress.XtraLayout.LayoutControl layoutTanim;
        private DevExpress.XtraGrid.GridControl gridBolgeler;
        private DevExpress.XtraGrid.Views.Grid.GridView viewBolgeler;
        private DevExpress.XtraGrid.Columns.GridColumn colBKod;
        private DevExpress.XtraGrid.Columns.GridColumn colBAd;
        private DevExpress.XtraGrid.Columns.GridColumn colBAciklama;
        private DevExpress.XtraGrid.Columns.GridColumn colBCadirSayisi;
        private DevExpress.XtraGrid.Columns.GridColumn colBKayitDurumu;
        private DevExpress.XtraEditors.TextEdit txtBolgeKod;
        private DevExpress.XtraEditors.TextEdit txtBolgeAd;
        private DevExpress.XtraEditors.TextEdit txtBolgeAciklama;
        private DevExpress.XtraEditors.SimpleButton btnBolgeYeni;
        private DevExpress.XtraEditors.SimpleButton btnBolgeKaydet;
        private DevExpress.XtraEditors.SimpleButton btnBolgeSil;
        private DevExpress.XtraEditors.SimpleButton btnBolgeAktiflik;
        private DevExpress.XtraGrid.GridControl gridCadirTanim;
        private DevExpress.XtraGrid.Views.Grid.GridView viewCadirTanim;
        private DevExpress.XtraGrid.Columns.GridColumn colTKod;
        private DevExpress.XtraGrid.Columns.GridColumn colTBolge;
        private DevExpress.XtraGrid.Columns.GridColumn colTKapasite;
        private DevExpress.XtraGrid.Columns.GridColumn colTDoluluk;
        private DevExpress.XtraGrid.Columns.GridColumn colTDurum;
        private DevExpress.XtraGrid.Columns.GridColumn colTErisilebilir;
        private DevExpress.XtraGrid.Columns.GridColumn colTSaglik;
        private DevExpress.XtraGrid.Columns.GridColumn colTIsitmali;
        private DevExpress.XtraGrid.Columns.GridColumn colTKayitDurumu;
        private DevExpress.XtraEditors.ImageComboBoxEdit cmbCadirBolge;
        private DevExpress.XtraEditors.SpinEdit spnSira;
        private DevExpress.XtraEditors.SpinEdit spnNumara;
        private DevExpress.XtraEditors.SpinEdit spnKapasite;
        private DevExpress.XtraEditors.ImageComboBoxEdit cmbCadirDurum;
        private DevExpress.XtraEditors.CheckEdit chkErisilebilir;
        private DevExpress.XtraEditors.CheckEdit chkSaglik;
        private DevExpress.XtraEditors.CheckEdit chkIsitmali;
        private DevExpress.XtraEditors.SimpleButton btnCadirYeni;
        private DevExpress.XtraEditors.SimpleButton btnCadirKaydet;
        private DevExpress.XtraEditors.SimpleButton btnCadirSil;
        private DevExpress.XtraEditors.SimpleButton btnCadirAktiflik;
        private DevExpress.XtraLayout.LayoutControlItem lciBolgeler;
        private DevExpress.XtraLayout.LayoutControlItem lciBolgeKod;
        private DevExpress.XtraLayout.LayoutControlItem lciBolgeAd;
        private DevExpress.XtraLayout.LayoutControlItem lciBolgeAciklama;
        private DevExpress.XtraLayout.LayoutControlItem lciBolgeYeni;
        private DevExpress.XtraLayout.LayoutControlItem lciBolgeKaydet;
        private DevExpress.XtraLayout.LayoutControlItem lciBolgeSil;
        private DevExpress.XtraLayout.LayoutControlItem lciBolgeAktiflik;
        private DevExpress.XtraLayout.LayoutControlGroup grpBolgeDuzen;
        private DevExpress.XtraLayout.EmptySpaceItem bosBolge;
        private DevExpress.XtraLayout.LayoutControlGroup grpBolgeler;
        private DevExpress.XtraLayout.LayoutControlItem lciCadirTanim;
        private DevExpress.XtraLayout.LayoutControlItem lciCadirBolge;
        private DevExpress.XtraLayout.LayoutControlItem lciSira;
        private DevExpress.XtraLayout.LayoutControlItem lciNumara;
        private DevExpress.XtraLayout.LayoutControlItem lciKapasite;
        private DevExpress.XtraLayout.LayoutControlItem lciCadirDurum;
        private DevExpress.XtraLayout.LayoutControlItem lciErisilebilir;
        private DevExpress.XtraLayout.LayoutControlItem lciSaglik;
        private DevExpress.XtraLayout.LayoutControlItem lciIsitmali;
        private DevExpress.XtraLayout.LayoutControlItem lciCadirYeni;
        private DevExpress.XtraLayout.LayoutControlItem lciCadirKaydet;
        private DevExpress.XtraLayout.LayoutControlItem lciCadirSil;
        private DevExpress.XtraLayout.LayoutControlItem lciCadirAktiflik;
        private DevExpress.XtraLayout.EmptySpaceItem bosCadirDuzen;
        private DevExpress.XtraLayout.LayoutControlGroup grpCadirDuzen;
        private DevExpress.XtraLayout.EmptySpaceItem bosCadir;
        private DevExpress.XtraLayout.LayoutControlGroup grpCadirlar;
        private DevExpress.XtraLayout.LayoutControlGroup RootTanim;
    }
}
