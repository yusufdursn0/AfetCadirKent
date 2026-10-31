namespace AfetCadirKent.UI.Formlar
{
    partial class YerlesimOneriPaneli
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

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
            this.lblBilgi = new DevExpress.XtraEditors.LabelControl();
            this.gridOneriler = new DevExpress.XtraGrid.GridControl();
            this.viewOneriler = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colOSira = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOCadirlar = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOEtkiler = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOBosYatak = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridKisiler = new DevExpress.XtraGrid.GridControl();
            this.viewKisiler = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colKKisiNo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colKAdSoyad = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colKYas = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colKDurum = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colKCadir = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoCadir = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.btnOnayla = new DevExpress.XtraEditors.SimpleButton();
            this.btnBeklemeyeAl = new DevExpress.XtraEditors.SimpleButton();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciBilgi = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciOneriler = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKisiler = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciOnayla = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBeklemeyeAl = new DevExpress.XtraLayout.LayoutControlItem();
            this.bosDugmeler = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
            this.layoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridOneriler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewOneriler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridKisiler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewKisiler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCadir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBilgi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOneriler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKisiler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOnayla)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBeklemeyeAl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosDugmeler)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl
            //
            this.layoutControl.Controls.Add(this.lblBilgi);
            this.layoutControl.Controls.Add(this.gridOneriler);
            this.layoutControl.Controls.Add(this.gridKisiler);
            this.layoutControl.Controls.Add(this.btnOnayla);
            this.layoutControl.Controls.Add(this.btnBeklemeyeAl);
            this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl.Location = new System.Drawing.Point(0, 0);
            this.layoutControl.Name = "layoutControl";
            this.layoutControl.Root = this.Root;
            this.layoutControl.Size = new System.Drawing.Size(640, 430);
            this.layoutControl.TabIndex = 0;
            //
            // lblBilgi
            //
            this.lblBilgi.AllowHtmlString = true;
            this.lblBilgi.Appearance.Options.UseTextOptions = true;
            this.lblBilgi.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblBilgi.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblBilgi.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblBilgi.Location = new System.Drawing.Point(12, 12);
            this.lblBilgi.Name = "lblBilgi";
            this.lblBilgi.Size = new System.Drawing.Size(616, 66);
            this.lblBilgi.StyleController = this.layoutControl;
            this.lblBilgi.TabIndex = 0;
            //
            // gridOneriler
            //
            this.gridOneriler.Location = new System.Drawing.Point(12, 98);
            this.gridOneriler.MainView = this.viewOneriler;
            this.gridOneriler.Name = "gridOneriler";
            this.gridOneriler.Size = new System.Drawing.Size(616, 120);
            this.gridOneriler.TabIndex = 1;
            this.gridOneriler.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewOneriler});
            //
            // viewOneriler
            //
            this.viewOneriler.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colOSira,
            this.colOCadirlar,
            this.colOEtkiler,
            this.colOBosYatak});
            this.viewOneriler.GridControl = this.gridOneriler;
            this.viewOneriler.Name = "viewOneriler";
            this.viewOneriler.OptionsBehavior.Editable = false;
            this.viewOneriler.OptionsCustomization.AllowSort = false;
            this.viewOneriler.OptionsView.ShowGroupPanel = false;
            this.viewOneriler.OptionsView.ShowIndicator = false;
            this.viewOneriler.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.viewOneriler_FocusedRowChanged);
            //
            // colOSira
            //
            this.colOSira.Caption = "Sıra";
            this.colOSira.FieldName = "Sira";
            this.colOSira.Name = "colOSira";
            this.colOSira.Visible = true;
            this.colOSira.VisibleIndex = 0;
            this.colOSira.Width = 40;
            //
            // colOCadirlar
            //
            this.colOCadirlar.Caption = "Çadır(lar)";
            this.colOCadirlar.FieldName = "Cadirlar";
            this.colOCadirlar.Name = "colOCadirlar";
            this.colOCadirlar.Visible = true;
            this.colOCadirlar.VisibleIndex = 1;
            this.colOCadirlar.Width = 190;
            //
            // colOEtkiler
            //
            this.colOEtkiler.Caption = "Yerleşim ihtiyacı (✔ karşılanıyor, ✘ karşılanmıyor)";
            this.colOEtkiler.FieldName = "Etkiler";
            this.colOEtkiler.Name = "colOEtkiler";
            this.colOEtkiler.Visible = true;
            this.colOEtkiler.VisibleIndex = 2;
            this.colOEtkiler.Width = 290;
            //
            // colOBosYatak
            //
            this.colOBosYatak.Caption = "Boş kalacak yatak";
            this.colOBosYatak.FieldName = "BosYatak";
            this.colOBosYatak.Name = "colOBosYatak";
            this.colOBosYatak.Visible = true;
            this.colOBosYatak.VisibleIndex = 3;
            this.colOBosYatak.Width = 96;
            //
            // gridKisiler
            //
            this.gridKisiler.Location = new System.Drawing.Point(12, 238);
            this.gridKisiler.MainView = this.viewKisiler;
            this.gridKisiler.Name = "gridKisiler";
            this.gridKisiler.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoCadir});
            this.gridKisiler.Size = new System.Drawing.Size(616, 142);
            this.gridKisiler.TabIndex = 2;
            this.gridKisiler.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewKisiler});
            //
            // viewKisiler
            //
            this.viewKisiler.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colKKisiNo,
            this.colKAdSoyad,
            this.colKYas,
            this.colKDurum,
            this.colKCadir});
            this.viewKisiler.GridControl = this.gridKisiler;
            this.viewKisiler.Name = "viewKisiler";
            this.viewKisiler.OptionsView.ShowGroupPanel = false;
            this.viewKisiler.OptionsView.ShowIndicator = false;
            this.viewKisiler.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.viewKisiler_CellValueChanged);
            //
            // colKKisiNo
            //
            this.colKKisiNo.Caption = "Kişi no";
            this.colKKisiNo.FieldName = "KisiNo";
            this.colKKisiNo.Name = "colKKisiNo";
            this.colKKisiNo.OptionsColumn.AllowEdit = false;
            this.colKKisiNo.Visible = true;
            this.colKKisiNo.VisibleIndex = 0;
            this.colKKisiNo.Width = 80;
            //
            // colKAdSoyad
            //
            this.colKAdSoyad.Caption = "Ad soyad";
            this.colKAdSoyad.FieldName = "AdSoyad";
            this.colKAdSoyad.Name = "colKAdSoyad";
            this.colKAdSoyad.OptionsColumn.AllowEdit = false;
            this.colKAdSoyad.Visible = true;
            this.colKAdSoyad.VisibleIndex = 1;
            this.colKAdSoyad.Width = 200;
            //
            // colKYas
            //
            this.colKYas.Caption = "Yaş";
            this.colKYas.FieldName = "Yas";
            this.colKYas.Name = "colKYas";
            this.colKYas.OptionsColumn.AllowEdit = false;
            this.colKYas.Visible = true;
            this.colKYas.VisibleIndex = 2;
            this.colKYas.Width = 50;
            //
            // colKDurum
            //
            this.colKDurum.Caption = "Durum";
            this.colKDurum.FieldName = "Durum";
            this.colKDurum.Name = "colKDurum";
            this.colKDurum.OptionsColumn.AllowEdit = false;
            this.colKDurum.Visible = true;
            this.colKDurum.VisibleIndex = 3;
            this.colKDurum.Width = 90;
            //
            // colKCadir
            //
            this.colKCadir.Caption = "Çadır";
            this.colKCadir.ColumnEdit = this.repoCadir;
            this.colKCadir.FieldName = "CadirId";
            this.colKCadir.Name = "colKCadir";
            this.colKCadir.Visible = true;
            this.colKCadir.VisibleIndex = 4;
            this.colKCadir.Width = 110;
            //
            // repoCadir
            //
            this.repoCadir.AutoHeight = false;
            this.repoCadir.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoCadir.Name = "repoCadir";
            //
            // btnOnayla
            //
            this.btnOnayla.Location = new System.Drawing.Point(12, 384);
            this.btnOnayla.Name = "btnOnayla";
            this.btnOnayla.Size = new System.Drawing.Size(236, 30);
            this.btnOnayla.StyleController = this.layoutControl;
            this.btnOnayla.TabIndex = 3;
            this.btnOnayla.Text = "Seçili öneriyi onayla ve yerleştir";
            this.btnOnayla.Click += new System.EventHandler(this.btnOnayla_Click);
            //
            // btnBeklemeyeAl
            //
            this.btnBeklemeyeAl.Location = new System.Drawing.Point(252, 384);
            this.btnBeklemeyeAl.Name = "btnBeklemeyeAl";
            this.btnBeklemeyeAl.Size = new System.Drawing.Size(186, 30);
            this.btnBeklemeyeAl.StyleController = this.layoutControl;
            this.btnBeklemeyeAl.TabIndex = 4;
            this.btnBeklemeyeAl.Text = "Bekleme listesine al";
            this.btnBeklemeyeAl.Click += new System.EventHandler(this.btnBeklemeyeAl_Click);
            //
            // Root
            //
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBilgi,
            this.lciOneriler,
            this.lciKisiler,
            this.lciOnayla,
            this.lciBeklemeyeAl,
            this.bosDugmeler});
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(640, 430);
            this.Root.TextVisible = false;
            //
            // lciBilgi
            //
            this.lciBilgi.Control = this.lblBilgi;
            this.lciBilgi.Location = new System.Drawing.Point(0, 0);
            this.lciBilgi.MaxSize = new System.Drawing.Size(0, 58);
            this.lciBilgi.MinSize = new System.Drawing.Size(200, 58);
            this.lciBilgi.Name = "lciBilgi";
            this.lciBilgi.Size = new System.Drawing.Size(640, 58);
            this.lciBilgi.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBilgi.TextVisible = false;
            //
            // lciOneriler
            //
            this.lciOneriler.Control = this.gridOneriler;
            this.lciOneriler.Location = new System.Drawing.Point(0, 70);
            this.lciOneriler.MinSize = new System.Drawing.Size(200, 80);
            this.lciOneriler.Name = "lciOneriler";
            this.lciOneriler.Size = new System.Drawing.Size(620, 140);
            this.lciOneriler.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciOneriler.Text = "Sistemin çadır önerileri (en uygun öneri üstte; görevli seçip onaylar)";
            this.lciOneriler.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciOneriler.TextSize = new System.Drawing.Size(100, 13);
            //
            // lciKisiler
            //
            this.lciKisiler.Control = this.gridKisiler;
            this.lciKisiler.Location = new System.Drawing.Point(0, 210);
            this.lciKisiler.MinSize = new System.Drawing.Size(200, 80);
            this.lciKisiler.Name = "lciKisiler";
            this.lciKisiler.Size = new System.Drawing.Size(620, 162);
            this.lciKisiler.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKisiler.Text = "Yerleşecek kişiler";
            this.lciKisiler.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciKisiler.TextSize = new System.Drawing.Size(100, 13);
            //
            // lciOnayla
            //
            this.lciOnayla.Control = this.btnOnayla;
            this.lciOnayla.Location = new System.Drawing.Point(0, 372);
            this.lciOnayla.MaxSize = new System.Drawing.Size(0, 34);
            this.lciOnayla.MinSize = new System.Drawing.Size(120, 34);
            this.lciOnayla.Name = "lciOnayla";
            this.lciOnayla.Size = new System.Drawing.Size(240, 34);
            this.lciOnayla.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciOnayla.TextVisible = false;
            //
            // lciBeklemeyeAl
            //
            this.lciBeklemeyeAl.Control = this.btnBeklemeyeAl;
            this.lciBeklemeyeAl.Location = new System.Drawing.Point(240, 372);
            this.lciBeklemeyeAl.MaxSize = new System.Drawing.Size(0, 34);
            this.lciBeklemeyeAl.MinSize = new System.Drawing.Size(120, 34);
            this.lciBeklemeyeAl.Name = "lciBeklemeyeAl";
            this.lciBeklemeyeAl.Size = new System.Drawing.Size(190, 34);
            this.lciBeklemeyeAl.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBeklemeyeAl.TextVisible = false;
            //
            // bosDugmeler
            //
            this.bosDugmeler.Location = new System.Drawing.Point(430, 372);
            this.bosDugmeler.Name = "bosDugmeler";
            this.bosDugmeler.Size = new System.Drawing.Size(190, 34);
            //
            // YerlesimOneriPaneli
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl);
            this.Name = "YerlesimOneriPaneli";
            this.Size = new System.Drawing.Size(640, 430);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
            this.layoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridOneriler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewOneriler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridKisiler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewKisiler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCadir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBilgi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOneriler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKisiler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOnayla)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBeklemeyeAl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosDugmeler)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.LabelControl lblBilgi;
        private DevExpress.XtraLayout.LayoutControlItem lciBilgi;
        private DevExpress.XtraGrid.GridControl gridOneriler;
        private DevExpress.XtraGrid.Views.Grid.GridView viewOneriler;
        private DevExpress.XtraGrid.Columns.GridColumn colOSira;
        private DevExpress.XtraGrid.Columns.GridColumn colOCadirlar;
        private DevExpress.XtraGrid.Columns.GridColumn colOEtkiler;
        private DevExpress.XtraGrid.Columns.GridColumn colOBosYatak;
        private DevExpress.XtraLayout.LayoutControlItem lciOneriler;
        private DevExpress.XtraGrid.GridControl gridKisiler;
        private DevExpress.XtraGrid.Views.Grid.GridView viewKisiler;
        private DevExpress.XtraGrid.Columns.GridColumn colKKisiNo;
        private DevExpress.XtraGrid.Columns.GridColumn colKAdSoyad;
        private DevExpress.XtraGrid.Columns.GridColumn colKYas;
        private DevExpress.XtraGrid.Columns.GridColumn colKDurum;
        private DevExpress.XtraGrid.Columns.GridColumn colKCadir;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox repoCadir;
        private DevExpress.XtraLayout.LayoutControlItem lciKisiler;
        private DevExpress.XtraEditors.SimpleButton btnOnayla;
        private DevExpress.XtraLayout.LayoutControlItem lciOnayla;
        private DevExpress.XtraEditors.SimpleButton btnBeklemeyeAl;
        private DevExpress.XtraLayout.LayoutControlItem lciBeklemeyeAl;
        private DevExpress.XtraLayout.EmptySpaceItem bosDugmeler;
    }
}
