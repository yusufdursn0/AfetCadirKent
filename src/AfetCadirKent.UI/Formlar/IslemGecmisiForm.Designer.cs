namespace AfetCadirKent.UI.Formlar
{
    partial class IslemGecmisiForm
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
            this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
            this.lblBilgi = new DevExpress.XtraEditors.LabelControl();
            this.dteBaslangic = new DevExpress.XtraEditors.DateEdit();
            this.dteBitis = new DevExpress.XtraEditors.DateEdit();
            this.cmbVarlik = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.cmbTur = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.txtKullanici = new DevExpress.XtraEditors.TextEdit();
            this.txtMetin = new DevExpress.XtraEditors.TextEdit();
            this.btnListele = new DevExpress.XtraEditors.SimpleButton();
            this.btnTemizle = new DevExpress.XtraEditors.SimpleButton();
            this.gridKayitlar = new DevExpress.XtraGrid.GridControl();
            this.viewKayitlar = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colZaman = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colKullanici = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTur = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colVarlik = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colVarlikId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAciklama = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lciBilgi = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBaslangic = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBitis = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciVarlik = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTur = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKullanici = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMetin = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciListele = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTemizle = new DevExpress.XtraLayout.LayoutControlItem();
            this.grpFiltre = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciKayitlar = new DevExpress.XtraLayout.LayoutControlItem();
            this.grpKayitlar = new DevExpress.XtraLayout.LayoutControlGroup();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
            this.layoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dteBaslangic.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteBaslangic.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteBitis.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteBitis.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbVarlik.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbTur.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKullanici.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMetin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridKayitlar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewKayitlar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBilgi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBaslangic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBitis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciVarlik)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTur)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKullanici)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMetin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciListele)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTemizle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpFiltre)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKayitlar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpKayitlar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl
            // 
            this.layoutControl.Controls.Add(this.lblBilgi);
            this.layoutControl.Controls.Add(this.dteBaslangic);
            this.layoutControl.Controls.Add(this.dteBitis);
            this.layoutControl.Controls.Add(this.cmbVarlik);
            this.layoutControl.Controls.Add(this.cmbTur);
            this.layoutControl.Controls.Add(this.txtKullanici);
            this.layoutControl.Controls.Add(this.txtMetin);
            this.layoutControl.Controls.Add(this.btnListele);
            this.layoutControl.Controls.Add(this.btnTemizle);
            this.layoutControl.Controls.Add(this.gridKayitlar);
            this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl.Location = new System.Drawing.Point(0, 0);
            this.layoutControl.Name = "layoutControl";
            this.layoutControl.Root = this.Root;
            this.layoutControl.Size = new System.Drawing.Size(1200, 760);
            this.layoutControl.TabIndex = 0;
            // 
            // lblBilgi
            // 
            this.lblBilgi.Location = new System.Drawing.Point(12, 12);
            this.lblBilgi.Name = "lblBilgi";
            this.lblBilgi.AllowHtmlString = true;
            this.lblBilgi.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblBilgi.Text = "Kim, ne zaman, neyi oluşturdu, değiştirdi veya iptal etti. Kayıtlar yalnızca okunur; en yeni önce, en fazla 1.000 satır gösterilir. Açıklamada aile/kişi numarası (ör. A-000012) veya talep numarası aranabilir.";
            this.lblBilgi.Size = new System.Drawing.Size(1176, 22);
            this.lblBilgi.StyleController = this.layoutControl;
            this.lblBilgi.TabIndex = 1;
            // 
            // dteBaslangic
            // 
            this.dteBaslangic.EditValue = null;
            this.dteBaslangic.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteBaslangic.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteBaslangic.Location = new System.Drawing.Point(108, 59);
            this.dteBaslangic.Name = "dteBaslangic";
            this.dteBaslangic.Size = new System.Drawing.Size(180, 22);
            this.dteBaslangic.StyleController = this.layoutControl;
            this.dteBaslangic.TabIndex = 2;
            // 
            // dteBitis
            // 
            this.dteBitis.EditValue = null;
            this.dteBitis.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteBitis.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteBitis.Location = new System.Drawing.Point(366, 59);
            this.dteBitis.Name = "dteBitis";
            this.dteBitis.Size = new System.Drawing.Size(180, 22);
            this.dteBitis.StyleController = this.layoutControl;
            this.dteBitis.TabIndex = 3;
            // 
            // cmbVarlik
            // 
            this.cmbVarlik.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbVarlik.Location = new System.Drawing.Point(624, 59);
            this.cmbVarlik.Name = "cmbVarlik";
            this.cmbVarlik.Size = new System.Drawing.Size(220, 22);
            this.cmbVarlik.StyleController = this.layoutControl;
            this.cmbVarlik.TabIndex = 4;
            // 
            // cmbTur
            // 
            this.cmbTur.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbTur.Location = new System.Drawing.Point(922, 59);
            this.cmbTur.Name = "cmbTur";
            this.cmbTur.Size = new System.Drawing.Size(254, 22);
            this.cmbTur.StyleController = this.layoutControl;
            this.cmbTur.TabIndex = 5;
            // 
            // txtKullanici
            // 
            this.txtKullanici.Location = new System.Drawing.Point(108, 85);
            this.txtKullanici.Name = "txtKullanici";
            this.txtKullanici.Properties.MaxLength = 50;
            this.txtKullanici.Size = new System.Drawing.Size(180, 22);
            this.txtKullanici.StyleController = this.layoutControl;
            this.txtKullanici.TabIndex = 6;
            // 
            // txtMetin
            // 
            this.txtMetin.Location = new System.Drawing.Point(366, 85);
            this.txtMetin.Name = "txtMetin";
            this.txtMetin.Properties.MaxLength = 100;
            this.txtMetin.Size = new System.Drawing.Size(478, 22);
            this.txtMetin.StyleController = this.layoutControl;
            this.txtMetin.TabIndex = 7;
            // 
            // btnListele
            // 
            this.btnListele.Location = new System.Drawing.Point(848, 85);
            this.btnListele.Name = "btnListele";
            this.btnListele.Text = "Listele";
            this.btnListele.Size = new System.Drawing.Size(160, 22);
            this.btnListele.StyleController = this.layoutControl;
            this.btnListele.TabIndex = 8;
            this.btnListele.Click += new System.EventHandler(this.btnListele_Click);
            // 
            // btnTemizle
            // 
            this.btnTemizle.Location = new System.Drawing.Point(1012, 85);
            this.btnTemizle.Name = "btnTemizle";
            this.btnTemizle.Text = "Filtreyi temizle";
            this.btnTemizle.Size = new System.Drawing.Size(164, 22);
            this.btnTemizle.StyleController = this.layoutControl;
            this.btnTemizle.TabIndex = 9;
            this.btnTemizle.Click += new System.EventHandler(this.btnTemizle_Click);
            // 
            // gridKayitlar
            // 
            this.gridKayitlar.Location = new System.Drawing.Point(24, 135);
            this.gridKayitlar.MainView = this.viewKayitlar;
            this.gridKayitlar.Name = "gridKayitlar";
            this.gridKayitlar.Size = new System.Drawing.Size(1152, 601);
            this.gridKayitlar.TabIndex = 10;
            this.gridKayitlar.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewKayitlar});
            // 
            // viewKayitlar
            // 
            this.viewKayitlar.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colZaman,
            this.colKullanici,
            this.colTur,
            this.colVarlik,
            this.colVarlikId,
            this.colAciklama});
            this.viewKayitlar.GridControl = this.gridKayitlar;
            this.viewKayitlar.Name = "viewKayitlar";
            this.viewKayitlar.OptionsBehavior.Editable = false;
            this.viewKayitlar.OptionsView.ShowGroupPanel = false;
            this.viewKayitlar.OptionsView.ShowIndicator = false;
            this.viewKayitlar.OptionsView.ColumnAutoWidth = false;
            // 
            // colZaman
            // 
            this.colZaman.Caption = "Zaman";
            this.colZaman.FieldName = "Zaman";
            this.colZaman.Name = "colZaman";
            this.colZaman.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";
            this.colZaman.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colZaman.Visible = true;
            this.colZaman.VisibleIndex = 0;
            this.colZaman.Width = 115;
            // 
            // colKullanici
            // 
            this.colKullanici.Caption = "Kullanıcı";
            this.colKullanici.FieldName = "KullaniciAdi";
            this.colKullanici.Name = "colKullanici";
            this.colKullanici.Visible = true;
            this.colKullanici.VisibleIndex = 1;
            this.colKullanici.Width = 100;
            // 
            // colTur
            // 
            this.colTur.Caption = "İşlem";
            this.colTur.FieldName = "Islem";
            this.colTur.Name = "colTur";
            this.colTur.Visible = true;
            this.colTur.VisibleIndex = 2;
            this.colTur.Width = 85;
            // 
            // colVarlik
            // 
            this.colVarlik.Caption = "Kayıt türü";
            this.colVarlik.FieldName = "KayitTuru";
            this.colVarlik.Name = "colVarlik";
            this.colVarlik.Visible = true;
            this.colVarlik.VisibleIndex = 3;
            this.colVarlik.Width = 120;
            // 
            // colVarlikId
            // 
            this.colVarlikId.Caption = "Kayıt no";
            this.colVarlikId.FieldName = "VarlikId";
            this.colVarlikId.Name = "colVarlikId";
            this.colVarlikId.Visible = true;
            this.colVarlikId.VisibleIndex = 4;
            this.colVarlikId.Width = 60;
            // 
            // colAciklama
            // 
            this.colAciklama.Caption = "Açıklama";
            this.colAciklama.FieldName = "Aciklama";
            this.colAciklama.Name = "colAciklama";
            this.colAciklama.Visible = true;
            this.colAciklama.VisibleIndex = 5;
            this.colAciklama.Width = 900;
            // 
            // lciBilgi
            // 
            this.lciBilgi.Control = this.lblBilgi;
            this.lciBilgi.Location = new System.Drawing.Point(0, 0);
            this.lciBilgi.MaxSize = new System.Drawing.Size(0, 26);
            this.lciBilgi.MinSize = new System.Drawing.Size(200, 26);
            this.lciBilgi.Name = "lciBilgi";
            this.lciBilgi.Size = new System.Drawing.Size(1180, 26);
            this.lciBilgi.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBilgi.TextVisible = false;
            // 
            // lciBaslangic
            // 
            this.lciBaslangic.Control = this.dteBaslangic;
            this.lciBaslangic.Location = new System.Drawing.Point(0, 0);
            this.lciBaslangic.MaxSize = new System.Drawing.Size(0, 26);
            this.lciBaslangic.MinSize = new System.Drawing.Size(80, 26);
            this.lciBaslangic.Name = "lciBaslangic";
            this.lciBaslangic.Size = new System.Drawing.Size(258, 26);
            this.lciBaslangic.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBaslangic.Text = "Başlangıç";
            this.lciBaslangic.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBaslangic.TextSize = new System.Drawing.Size(80, 13);
            // 
            // lciBitis
            // 
            this.lciBitis.Control = this.dteBitis;
            this.lciBitis.Location = new System.Drawing.Point(258, 0);
            this.lciBitis.MaxSize = new System.Drawing.Size(0, 26);
            this.lciBitis.MinSize = new System.Drawing.Size(80, 26);
            this.lciBitis.Name = "lciBitis";
            this.lciBitis.Size = new System.Drawing.Size(258, 26);
            this.lciBitis.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBitis.Text = "Bitiş";
            this.lciBitis.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBitis.TextSize = new System.Drawing.Size(70, 13);
            // 
            // lciVarlik
            // 
            this.lciVarlik.Control = this.cmbVarlik;
            this.lciVarlik.Location = new System.Drawing.Point(516, 0);
            this.lciVarlik.MaxSize = new System.Drawing.Size(0, 26);
            this.lciVarlik.MinSize = new System.Drawing.Size(80, 26);
            this.lciVarlik.Name = "lciVarlik";
            this.lciVarlik.Size = new System.Drawing.Size(300, 26);
            this.lciVarlik.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciVarlik.Text = "Kayıt türü";
            this.lciVarlik.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciVarlik.TextSize = new System.Drawing.Size(70, 13);
            // 
            // lciTur
            // 
            this.lciTur.Control = this.cmbTur;
            this.lciTur.Location = new System.Drawing.Point(816, 0);
            this.lciTur.MaxSize = new System.Drawing.Size(0, 26);
            this.lciTur.MinSize = new System.Drawing.Size(80, 26);
            this.lciTur.Name = "lciTur";
            this.lciTur.Size = new System.Drawing.Size(340, 26);
            this.lciTur.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciTur.Text = "İşlem";
            this.lciTur.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTur.TextSize = new System.Drawing.Size(70, 13);
            // 
            // lciKullanici
            // 
            this.lciKullanici.Control = this.txtKullanici;
            this.lciKullanici.Location = new System.Drawing.Point(0, 26);
            this.lciKullanici.MaxSize = new System.Drawing.Size(0, 26);
            this.lciKullanici.MinSize = new System.Drawing.Size(80, 26);
            this.lciKullanici.Name = "lciKullanici";
            this.lciKullanici.Size = new System.Drawing.Size(258, 26);
            this.lciKullanici.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKullanici.Text = "Kullanıcı adı";
            this.lciKullanici.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciKullanici.TextSize = new System.Drawing.Size(80, 13);
            // 
            // lciMetin
            // 
            this.lciMetin.Control = this.txtMetin;
            this.lciMetin.Location = new System.Drawing.Point(258, 26);
            this.lciMetin.MaxSize = new System.Drawing.Size(0, 26);
            this.lciMetin.MinSize = new System.Drawing.Size(80, 26);
            this.lciMetin.Name = "lciMetin";
            this.lciMetin.Size = new System.Drawing.Size(558, 26);
            this.lciMetin.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciMetin.Text = "Açıklamada";
            this.lciMetin.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMetin.TextSize = new System.Drawing.Size(70, 13);
            // 
            // lciListele
            // 
            this.lciListele.Control = this.btnListele;
            this.lciListele.Location = new System.Drawing.Point(816, 26);
            this.lciListele.MaxSize = new System.Drawing.Size(0, 26);
            this.lciListele.MinSize = new System.Drawing.Size(80, 26);
            this.lciListele.Name = "lciListele";
            this.lciListele.Size = new System.Drawing.Size(164, 26);
            this.lciListele.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciListele.TextVisible = false;
            // 
            // lciTemizle
            // 
            this.lciTemizle.Control = this.btnTemizle;
            this.lciTemizle.Location = new System.Drawing.Point(980, 26);
            this.lciTemizle.MaxSize = new System.Drawing.Size(0, 26);
            this.lciTemizle.MinSize = new System.Drawing.Size(80, 26);
            this.lciTemizle.Name = "lciTemizle";
            this.lciTemizle.Size = new System.Drawing.Size(176, 26);
            this.lciTemizle.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciTemizle.TextVisible = false;
            // 
            // grpFiltre
            // 
            this.grpFiltre.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBaslangic,
            this.lciBitis,
            this.lciVarlik,
            this.lciTur,
            this.lciKullanici,
            this.lciMetin,
            this.lciListele,
            this.lciTemizle});
            this.grpFiltre.Location = new System.Drawing.Point(0, 26);
            this.grpFiltre.Name = "grpFiltre";
            this.grpFiltre.Size = new System.Drawing.Size(1180, 97);
            this.grpFiltre.Text = "Filtre";
            // 
            // lciKayitlar
            // 
            this.lciKayitlar.Control = this.gridKayitlar;
            this.lciKayitlar.Location = new System.Drawing.Point(0, 0);
            this.lciKayitlar.MinSize = new System.Drawing.Size(300, 100);
            this.lciKayitlar.Name = "lciKayitlar";
            this.lciKayitlar.Size = new System.Drawing.Size(1156, 572);
            this.lciKayitlar.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKayitlar.TextVisible = false;
            // 
            // grpKayitlar
            // 
            this.grpKayitlar.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciKayitlar});
            this.grpKayitlar.Location = new System.Drawing.Point(0, 123);
            this.grpKayitlar.Name = "grpKayitlar";
            this.grpKayitlar.Size = new System.Drawing.Size(1180, 617);
            this.grpKayitlar.Text = "İşlem geçmişi";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBilgi,
            this.grpFiltre,
            this.grpKayitlar});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1200, 760);
            this.Root.TextVisible = false;
            // 
            // IslemGecmisiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 760);
            this.Controls.Add(this.layoutControl);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "IslemGecmisiForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "İşlem geçmişi";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
            this.layoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dteBaslangic.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteBaslangic.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteBitis.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteBitis.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbVarlik.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbTur.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKullanici.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMetin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridKayitlar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewKayitlar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBilgi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBaslangic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBitis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciVarlik)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTur)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKullanici)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMetin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciListele)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTemizle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpFiltre)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKayitlar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpKayitlar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl;
        private DevExpress.XtraEditors.LabelControl lblBilgi;
        private DevExpress.XtraEditors.DateEdit dteBaslangic;
        private DevExpress.XtraEditors.DateEdit dteBitis;
        private DevExpress.XtraEditors.ImageComboBoxEdit cmbVarlik;
        private DevExpress.XtraEditors.ImageComboBoxEdit cmbTur;
        private DevExpress.XtraEditors.TextEdit txtKullanici;
        private DevExpress.XtraEditors.TextEdit txtMetin;
        private DevExpress.XtraEditors.SimpleButton btnListele;
        private DevExpress.XtraEditors.SimpleButton btnTemizle;
        private DevExpress.XtraGrid.GridControl gridKayitlar;
        private DevExpress.XtraGrid.Views.Grid.GridView viewKayitlar;
        private DevExpress.XtraGrid.Columns.GridColumn colZaman;
        private DevExpress.XtraGrid.Columns.GridColumn colKullanici;
        private DevExpress.XtraGrid.Columns.GridColumn colTur;
        private DevExpress.XtraGrid.Columns.GridColumn colVarlik;
        private DevExpress.XtraGrid.Columns.GridColumn colVarlikId;
        private DevExpress.XtraGrid.Columns.GridColumn colAciklama;
        private DevExpress.XtraLayout.LayoutControlItem lciBilgi;
        private DevExpress.XtraLayout.LayoutControlItem lciBaslangic;
        private DevExpress.XtraLayout.LayoutControlItem lciBitis;
        private DevExpress.XtraLayout.LayoutControlItem lciVarlik;
        private DevExpress.XtraLayout.LayoutControlItem lciTur;
        private DevExpress.XtraLayout.LayoutControlItem lciKullanici;
        private DevExpress.XtraLayout.LayoutControlItem lciMetin;
        private DevExpress.XtraLayout.LayoutControlItem lciListele;
        private DevExpress.XtraLayout.LayoutControlItem lciTemizle;
        private DevExpress.XtraLayout.LayoutControlGroup grpFiltre;
        private DevExpress.XtraLayout.LayoutControlItem lciKayitlar;
        private DevExpress.XtraLayout.LayoutControlGroup grpKayitlar;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
    }
}
