namespace AfetCadirKent.UI.Formlar
{
    partial class RaporlarForm
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
            this.rdgRapor = new DevExpress.XtraEditors.RadioGroup();
            this.cmbDagitim = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.dteBaslangic = new DevExpress.XtraEditors.DateEdit();
            this.dteBitis = new DevExpress.XtraEditors.DateEdit();
            this.lblRaporNot = new DevExpress.XtraEditors.LabelControl();
            this.btnOlustur = new DevExpress.XtraEditors.SimpleButton();
            this.btnPdf = new DevExpress.XtraEditors.SimpleButton();
            this.btnExcel = new DevExpress.XtraEditors.SimpleButton();
            this.btnYazdir = new DevExpress.XtraEditors.SimpleButton();
            this.viewer = new DevExpress.XtraPrinting.Preview.DocumentViewer();
            this.lciBilgi = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRapor = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDagitim = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBaslangic = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBitis = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRaporNot = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciOlustur = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPdf = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciExcel = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciYazdir = new DevExpress.XtraLayout.LayoutControlItem();
            this.bosSecim = new DevExpress.XtraLayout.EmptySpaceItem();
            this.grpSecim = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciViewer = new DevExpress.XtraLayout.LayoutControlItem();
            this.grpOnizleme = new DevExpress.XtraLayout.LayoutControlGroup();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
            this.layoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rdgRapor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDagitim.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteBaslangic.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteBaslangic.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteBitis.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteBitis.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBilgi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRapor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDagitim)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBaslangic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBitis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRaporNot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOlustur)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPdf)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExcel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYazdir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosSecim)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpSecim)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciViewer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpOnizleme)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl
            // 
            this.layoutControl.Controls.Add(this.lblBilgi);
            this.layoutControl.Controls.Add(this.rdgRapor);
            this.layoutControl.Controls.Add(this.cmbDagitim);
            this.layoutControl.Controls.Add(this.dteBaslangic);
            this.layoutControl.Controls.Add(this.dteBitis);
            this.layoutControl.Controls.Add(this.lblRaporNot);
            this.layoutControl.Controls.Add(this.btnOlustur);
            this.layoutControl.Controls.Add(this.btnPdf);
            this.layoutControl.Controls.Add(this.btnExcel);
            this.layoutControl.Controls.Add(this.btnYazdir);
            this.layoutControl.Controls.Add(this.viewer);
            this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl.Location = new System.Drawing.Point(0, 0);
            this.layoutControl.Name = "layoutControl";
            this.layoutControl.Root = this.Root;
            this.layoutControl.Size = new System.Drawing.Size(1300, 800);
            this.layoutControl.TabIndex = 0;
            // 
            // lblBilgi
            // 
            this.lblBilgi.Location = new System.Drawing.Point(12, 12);
            this.lblBilgi.Name = "lblBilgi";
            this.lblBilgi.AllowHtmlString = true;
            this.lblBilgi.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblBilgi.Text = "Raporu seçip “Raporu oluştur”a basın; önizleme sağda açılır. Rapor PDF veya Excel olarak kaydedilebilir ya da yazdırılabilir. Görünen raporlar yetkinize göredir.";
            this.lblBilgi.Size = new System.Drawing.Size(1276, 22);
            this.lblBilgi.StyleController = this.layoutControl;
            this.lblBilgi.TabIndex = 1;
            // 
            // rdgRapor
            // 
            this.rdgRapor.Location = new System.Drawing.Point(24, 59);
            this.rdgRapor.Name = "rdgRapor";
            this.rdgRapor.Size = new System.Drawing.Size(296, 140);
            this.rdgRapor.StyleController = this.layoutControl;
            this.rdgRapor.TabIndex = 2;
            this.rdgRapor.SelectedIndexChanged += new System.EventHandler(this.rdgRapor_SelectedIndexChanged);
            // 
            // cmbDagitim
            // 
            this.cmbDagitim.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbDagitim.Location = new System.Drawing.Point(24, 223);
            this.cmbDagitim.Name = "cmbDagitim";
            this.cmbDagitim.Size = new System.Drawing.Size(296, 22);
            this.cmbDagitim.StyleController = this.layoutControl;
            this.cmbDagitim.TabIndex = 3;
            // 
            // dteBaslangic
            // 
            this.dteBaslangic.EditValue = null;
            this.dteBaslangic.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteBaslangic.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteBaslangic.Location = new System.Drawing.Point(90, 249);
            this.dteBaslangic.Name = "dteBaslangic";
            this.dteBaslangic.Size = new System.Drawing.Size(230, 22);
            this.dteBaslangic.StyleController = this.layoutControl;
            this.dteBaslangic.TabIndex = 4;
            // 
            // dteBitis
            // 
            this.dteBitis.EditValue = null;
            this.dteBitis.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteBitis.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteBitis.Location = new System.Drawing.Point(90, 275);
            this.dteBitis.Name = "dteBitis";
            this.dteBitis.Size = new System.Drawing.Size(230, 22);
            this.dteBitis.StyleController = this.layoutControl;
            this.dteBitis.TabIndex = 5;
            // 
            // lblRaporNot
            // 
            this.lblRaporNot.Location = new System.Drawing.Point(24, 301);
            this.lblRaporNot.Name = "lblRaporNot";
            this.lblRaporNot.AllowHtmlString = true;
            this.lblRaporNot.Appearance.Options.UseTextOptions = true;
            this.lblRaporNot.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblRaporNot.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblRaporNot.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblRaporNot.Text = "";
            this.lblRaporNot.Size = new System.Drawing.Size(296, 120);
            this.lblRaporNot.StyleController = this.layoutControl;
            this.lblRaporNot.TabIndex = 6;
            // 
            // btnOlustur
            // 
            this.btnOlustur.Location = new System.Drawing.Point(24, 425);
            this.btnOlustur.Name = "btnOlustur";
            this.btnOlustur.Text = "Raporu oluştur";
            this.btnOlustur.Size = new System.Drawing.Size(296, 34);
            this.btnOlustur.StyleController = this.layoutControl;
            this.btnOlustur.TabIndex = 7;
            this.btnOlustur.Click += new System.EventHandler(this.btnOlustur_Click);
            // 
            // btnPdf
            // 
            this.btnPdf.Location = new System.Drawing.Point(24, 463);
            this.btnPdf.Name = "btnPdf";
            this.btnPdf.Text = "PDF olarak kaydet";
            this.btnPdf.Size = new System.Drawing.Size(296, 30);
            this.btnPdf.StyleController = this.layoutControl;
            this.btnPdf.TabIndex = 8;
            this.btnPdf.Click += new System.EventHandler(this.btnPdf_Click);
            // 
            // btnExcel
            // 
            this.btnExcel.Location = new System.Drawing.Point(24, 497);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Text = "Excel olarak kaydet";
            this.btnExcel.Size = new System.Drawing.Size(296, 30);
            this.btnExcel.StyleController = this.layoutControl;
            this.btnExcel.TabIndex = 9;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // btnYazdir
            // 
            this.btnYazdir.Location = new System.Drawing.Point(24, 531);
            this.btnYazdir.Name = "btnYazdir";
            this.btnYazdir.Text = "Yazdır";
            this.btnYazdir.Size = new System.Drawing.Size(296, 30);
            this.btnYazdir.StyleController = this.layoutControl;
            this.btnYazdir.TabIndex = 10;
            this.btnYazdir.Click += new System.EventHandler(this.btnYazdir_Click);
            // 
            // viewer
            // 
            this.viewer.IsMetric = true;
            this.viewer.Location = new System.Drawing.Point(356, 59);
            this.viewer.Name = "viewer";
            this.viewer.Size = new System.Drawing.Size(920, 717);
            this.viewer.TabIndex = 11;
            // 
            // lciBilgi
            // 
            this.lciBilgi.Control = this.lblBilgi;
            this.lciBilgi.Location = new System.Drawing.Point(0, 0);
            this.lciBilgi.MaxSize = new System.Drawing.Size(0, 26);
            this.lciBilgi.MinSize = new System.Drawing.Size(200, 26);
            this.lciBilgi.Name = "lciBilgi";
            this.lciBilgi.Size = new System.Drawing.Size(1280, 26);
            this.lciBilgi.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBilgi.TextVisible = false;
            // 
            // lciRapor
            // 
            this.lciRapor.Control = this.rdgRapor;
            this.lciRapor.Location = new System.Drawing.Point(0, 0);
            this.lciRapor.MaxSize = new System.Drawing.Size(0, 140);
            this.lciRapor.MinSize = new System.Drawing.Size(100, 140);
            this.lciRapor.Name = "lciRapor";
            this.lciRapor.Size = new System.Drawing.Size(296, 140);
            this.lciRapor.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciRapor.TextVisible = false;
            // 
            // lciDagitim
            // 
            this.lciDagitim.Control = this.cmbDagitim;
            this.lciDagitim.Location = new System.Drawing.Point(0, 140);
            this.lciDagitim.MaxSize = new System.Drawing.Size(0, 46);
            this.lciDagitim.MinSize = new System.Drawing.Size(80, 46);
            this.lciDagitim.Name = "lciDagitim";
            this.lciDagitim.Size = new System.Drawing.Size(296, 46);
            this.lciDagitim.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDagitim.Text = "Dağıtım listesi";
            this.lciDagitim.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciDagitim.TextSize = new System.Drawing.Size(100, 13);
            // 
            // lciBaslangic
            // 
            this.lciBaslangic.Control = this.dteBaslangic;
            this.lciBaslangic.Location = new System.Drawing.Point(0, 186);
            this.lciBaslangic.MaxSize = new System.Drawing.Size(0, 26);
            this.lciBaslangic.MinSize = new System.Drawing.Size(80, 26);
            this.lciBaslangic.Name = "lciBaslangic";
            this.lciBaslangic.Size = new System.Drawing.Size(296, 26);
            this.lciBaslangic.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBaslangic.Text = "Başlangıç";
            this.lciBaslangic.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBaslangic.TextSize = new System.Drawing.Size(60, 13);
            // 
            // lciBitis
            // 
            this.lciBitis.Control = this.dteBitis;
            this.lciBitis.Location = new System.Drawing.Point(0, 212);
            this.lciBitis.MaxSize = new System.Drawing.Size(0, 26);
            this.lciBitis.MinSize = new System.Drawing.Size(80, 26);
            this.lciBitis.Name = "lciBitis";
            this.lciBitis.Size = new System.Drawing.Size(296, 26);
            this.lciBitis.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBitis.Text = "Bitiş";
            this.lciBitis.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBitis.TextSize = new System.Drawing.Size(60, 13);
            // 
            // lciRaporNot
            // 
            this.lciRaporNot.Control = this.lblRaporNot;
            this.lciRaporNot.Location = new System.Drawing.Point(0, 238);
            this.lciRaporNot.MinSize = new System.Drawing.Size(80, 60);
            this.lciRaporNot.Name = "lciRaporNot";
            this.lciRaporNot.Size = new System.Drawing.Size(296, 120);
            this.lciRaporNot.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciRaporNot.TextVisible = false;
            // 
            // lciOlustur
            // 
            this.lciOlustur.Control = this.btnOlustur;
            this.lciOlustur.Location = new System.Drawing.Point(0, 358);
            this.lciOlustur.MaxSize = new System.Drawing.Size(0, 38);
            this.lciOlustur.MinSize = new System.Drawing.Size(80, 38);
            this.lciOlustur.Name = "lciOlustur";
            this.lciOlustur.Size = new System.Drawing.Size(296, 38);
            this.lciOlustur.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciOlustur.TextVisible = false;
            // 
            // lciPdf
            // 
            this.lciPdf.Control = this.btnPdf;
            this.lciPdf.Location = new System.Drawing.Point(0, 396);
            this.lciPdf.MaxSize = new System.Drawing.Size(0, 34);
            this.lciPdf.MinSize = new System.Drawing.Size(80, 34);
            this.lciPdf.Name = "lciPdf";
            this.lciPdf.Size = new System.Drawing.Size(296, 34);
            this.lciPdf.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciPdf.TextVisible = false;
            // 
            // lciExcel
            // 
            this.lciExcel.Control = this.btnExcel;
            this.lciExcel.Location = new System.Drawing.Point(0, 430);
            this.lciExcel.MaxSize = new System.Drawing.Size(0, 34);
            this.lciExcel.MinSize = new System.Drawing.Size(80, 34);
            this.lciExcel.Name = "lciExcel";
            this.lciExcel.Size = new System.Drawing.Size(296, 34);
            this.lciExcel.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciExcel.TextVisible = false;
            // 
            // lciYazdir
            // 
            this.lciYazdir.Control = this.btnYazdir;
            this.lciYazdir.Location = new System.Drawing.Point(0, 464);
            this.lciYazdir.MaxSize = new System.Drawing.Size(0, 34);
            this.lciYazdir.MinSize = new System.Drawing.Size(80, 34);
            this.lciYazdir.Name = "lciYazdir";
            this.lciYazdir.Size = new System.Drawing.Size(296, 34);
            this.lciYazdir.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciYazdir.TextVisible = false;
            // 
            // bosSecim
            // 
            this.bosSecim.Location = new System.Drawing.Point(0, 498);
            this.bosSecim.Name = "bosSecim";
            this.bosSecim.Size = new System.Drawing.Size(296, 231);
            // 
            // grpSecim
            // 
            this.grpSecim.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciRapor,
            this.lciDagitim,
            this.lciBaslangic,
            this.lciBitis,
            this.lciRaporNot,
            this.lciOlustur,
            this.lciPdf,
            this.lciExcel,
            this.lciYazdir,
            this.bosSecim});
            this.grpSecim.Location = new System.Drawing.Point(0, 26);
            this.grpSecim.Name = "grpSecim";
            this.grpSecim.Size = new System.Drawing.Size(320, 774);
            this.grpSecim.Text = "Rapor";
            // 
            // lciViewer
            // 
            this.lciViewer.Control = this.viewer;
            this.lciViewer.Location = new System.Drawing.Point(0, 0);
            this.lciViewer.MinSize = new System.Drawing.Size(300, 200);
            this.lciViewer.Name = "lciViewer";
            this.lciViewer.Size = new System.Drawing.Size(936, 729);
            this.lciViewer.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciViewer.TextVisible = false;
            // 
            // grpOnizleme
            // 
            this.grpOnizleme.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciViewer});
            this.grpOnizleme.Location = new System.Drawing.Point(320, 26);
            this.grpOnizleme.Name = "grpOnizleme";
            this.grpOnizleme.Size = new System.Drawing.Size(960, 774);
            this.grpOnizleme.Text = "Önizleme";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBilgi,
            this.grpSecim,
            this.grpOnizleme});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1300, 800);
            this.Root.TextVisible = false;
            // 
            // RaporlarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 800);
            this.Controls.Add(this.layoutControl);
            this.MinimumSize = new System.Drawing.Size(1000, 640);
            this.Name = "RaporlarForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Raporlar";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
            this.layoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rdgRapor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDagitim.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteBaslangic.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteBaslangic.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteBitis.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteBitis.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBilgi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRapor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDagitim)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBaslangic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBitis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRaporNot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOlustur)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPdf)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExcel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYazdir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosSecim)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpSecim)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciViewer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpOnizleme)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl;
        private DevExpress.XtraEditors.LabelControl lblBilgi;
        private DevExpress.XtraEditors.RadioGroup rdgRapor;
        private DevExpress.XtraEditors.ImageComboBoxEdit cmbDagitim;
        private DevExpress.XtraEditors.DateEdit dteBaslangic;
        private DevExpress.XtraEditors.DateEdit dteBitis;
        private DevExpress.XtraEditors.LabelControl lblRaporNot;
        private DevExpress.XtraEditors.SimpleButton btnOlustur;
        private DevExpress.XtraEditors.SimpleButton btnPdf;
        private DevExpress.XtraEditors.SimpleButton btnExcel;
        private DevExpress.XtraEditors.SimpleButton btnYazdir;
        private DevExpress.XtraPrinting.Preview.DocumentViewer viewer;
        private DevExpress.XtraLayout.LayoutControlItem lciBilgi;
        private DevExpress.XtraLayout.LayoutControlItem lciRapor;
        private DevExpress.XtraLayout.LayoutControlItem lciDagitim;
        private DevExpress.XtraLayout.LayoutControlItem lciBaslangic;
        private DevExpress.XtraLayout.LayoutControlItem lciBitis;
        private DevExpress.XtraLayout.LayoutControlItem lciRaporNot;
        private DevExpress.XtraLayout.LayoutControlItem lciOlustur;
        private DevExpress.XtraLayout.LayoutControlItem lciPdf;
        private DevExpress.XtraLayout.LayoutControlItem lciExcel;
        private DevExpress.XtraLayout.LayoutControlItem lciYazdir;
        private DevExpress.XtraLayout.EmptySpaceItem bosSecim;
        private DevExpress.XtraLayout.LayoutControlGroup grpSecim;
        private DevExpress.XtraLayout.LayoutControlItem lciViewer;
        private DevExpress.XtraLayout.LayoutControlGroup grpOnizleme;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
    }
}
