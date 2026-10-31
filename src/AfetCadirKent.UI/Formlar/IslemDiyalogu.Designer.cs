namespace AfetCadirKent.UI.Formlar
{
    partial class IslemDiyalogu
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
            this.lciMesaj = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblMesaj = new DevExpress.XtraEditors.LabelControl();
            this.lciSecim = new DevExpress.XtraLayout.LayoutControlItem();
            this.cmbSecim = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.lciMetin = new DevExpress.XtraLayout.LayoutControlItem();
            this.txtMetin = new DevExpress.XtraEditors.TextEdit();
            this.lciAciklama = new DevExpress.XtraLayout.LayoutControlItem();
            this.memoAciklama = new DevExpress.XtraEditors.MemoEdit();
            this.bosOrta = new DevExpress.XtraLayout.EmptySpaceItem();
            this.bosAlt = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciTamam = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnTamam = new DevExpress.XtraEditors.SimpleButton();
            this.lciVazgec = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnVazgec = new DevExpress.XtraEditors.SimpleButton();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
            this.layoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lciMesaj)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSecim)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSecim.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMetin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMetin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAciklama)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoAciklama.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosOrta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosAlt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTamam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciVazgec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl
            // 
            this.layoutControl.Controls.Add(this.lblMesaj);
            this.layoutControl.Controls.Add(this.cmbSecim);
            this.layoutControl.Controls.Add(this.txtMetin);
            this.layoutControl.Controls.Add(this.memoAciklama);
            this.layoutControl.Controls.Add(this.btnTamam);
            this.layoutControl.Controls.Add(this.btnVazgec);
            this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl.Location = new System.Drawing.Point(0, 0);
            this.layoutControl.Name = "layoutControl";
            this.layoutControl.Root = this.Root;
            this.layoutControl.Size = new System.Drawing.Size(440, 300);
            this.layoutControl.TabIndex = 0;
            // 
            // lblMesaj
            // 
            this.lblMesaj.Location = new System.Drawing.Point(12, 12);
            this.lblMesaj.Name = "lblMesaj";
            this.lblMesaj.AllowHtmlString = true;
            this.lblMesaj.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblMesaj.Appearance.Options.UseTextOptions = true;
            this.lblMesaj.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblMesaj.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblMesaj.Size = new System.Drawing.Size(416, 46);
            this.lblMesaj.StyleController = this.layoutControl;
            this.lblMesaj.TabIndex = 1;
            // 
            // cmbSecim
            // 
            this.cmbSecim.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbSecim.Location = new System.Drawing.Point(116, 62);
            this.cmbSecim.Name = "cmbSecim";
            this.cmbSecim.Size = new System.Drawing.Size(312, 20);
            this.cmbSecim.StyleController = this.layoutControl;
            this.cmbSecim.TabIndex = 2;
            // 
            // txtMetin
            // 
            this.txtMetin.Location = new System.Drawing.Point(116, 86);
            this.txtMetin.Name = "txtMetin";
            this.txtMetin.Properties.MaxLength = 100;
            this.txtMetin.Size = new System.Drawing.Size(312, 20);
            this.txtMetin.StyleController = this.layoutControl;
            this.txtMetin.TabIndex = 3;
            // 
            // memoAciklama
            // 
            this.memoAciklama.Location = new System.Drawing.Point(12, 127);
            this.memoAciklama.Name = "memoAciklama";
            this.memoAciklama.Properties.MaxLength = 500;
            this.memoAciklama.Size = new System.Drawing.Size(416, 99);
            this.memoAciklama.StyleController = this.layoutControl;
            this.memoAciklama.TabIndex = 4;
            // 
            // btnTamam
            // 
            this.btnTamam.Location = new System.Drawing.Point(232, 254);
            this.btnTamam.Name = "btnTamam";
            this.btnTamam.Text = "Tamam";
            this.btnTamam.Click += new System.EventHandler(this.btnTamam_Click);
            this.btnTamam.Size = new System.Drawing.Size(96, 34);
            this.btnTamam.StyleController = this.layoutControl;
            this.btnTamam.TabIndex = 5;
            // 
            // btnVazgec
            // 
            this.btnVazgec.Location = new System.Drawing.Point(332, 254);
            this.btnVazgec.Name = "btnVazgec";
            this.btnVazgec.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnVazgec.Text = "Vazgeç";
            this.btnVazgec.Size = new System.Drawing.Size(96, 34);
            this.btnVazgec.StyleController = this.layoutControl;
            this.btnVazgec.TabIndex = 6;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciMesaj,
            this.lciSecim,
            this.lciMetin,
            this.lciAciklama,
            this.bosOrta,
            this.bosAlt,
            this.lciTamam,
            this.lciVazgec});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(440, 300);
            this.Root.TextVisible = false;
            // 
            // lciMesaj
            // 
            this.lciMesaj.Control = this.lblMesaj;
            this.lciMesaj.Location = new System.Drawing.Point(0, 0);
            this.lciMesaj.Name = "lciMesaj";
            this.lciMesaj.Size = new System.Drawing.Size(420, 50);
            this.lciMesaj.TextVisible = false;
            // 
            // lciSecim
            // 
            this.lciSecim.Control = this.cmbSecim;
            this.lciSecim.Location = new System.Drawing.Point(0, 50);
            this.lciSecim.Name = "lciSecim";
            this.lciSecim.Size = new System.Drawing.Size(420, 24);
            this.lciSecim.Text = "Seçim";
            this.lciSecim.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSecim.TextSize = new System.Drawing.Size(100, 13);
            // 
            // lciMetin
            // 
            this.lciMetin.Control = this.txtMetin;
            this.lciMetin.Location = new System.Drawing.Point(0, 74);
            this.lciMetin.Name = "lciMetin";
            this.lciMetin.Size = new System.Drawing.Size(420, 24);
            this.lciMetin.Text = "Metin";
            this.lciMetin.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMetin.TextSize = new System.Drawing.Size(100, 13);
            // 
            // lciAciklama
            // 
            this.lciAciklama.Control = this.memoAciklama;
            this.lciAciklama.Location = new System.Drawing.Point(0, 98);
            this.lciAciklama.Name = "lciAciklama";
            this.lciAciklama.Size = new System.Drawing.Size(420, 120);
            this.lciAciklama.Text = "Açıklama";
            this.lciAciklama.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciAciklama.TextSize = new System.Drawing.Size(100, 13);
            // 
            // bosOrta
            // 
            this.bosOrta.Location = new System.Drawing.Point(0, 218);
            this.bosOrta.Name = "bosOrta";
            this.bosOrta.Size = new System.Drawing.Size(420, 24);
            // 
            // bosAlt
            // 
            this.bosAlt.Location = new System.Drawing.Point(0, 242);
            this.bosAlt.Name = "bosAlt";
            this.bosAlt.Size = new System.Drawing.Size(220, 38);
            // 
            // lciTamam
            // 
            this.lciTamam.Control = this.btnTamam;
            this.lciTamam.Location = new System.Drawing.Point(220, 242);
            this.lciTamam.Name = "lciTamam";
            this.lciTamam.Size = new System.Drawing.Size(100, 38);
            this.lciTamam.TextVisible = false;
            // 
            // lciVazgec
            // 
            this.lciVazgec.Control = this.btnVazgec;
            this.lciVazgec.Location = new System.Drawing.Point(320, 242);
            this.lciVazgec.Name = "lciVazgec";
            this.lciVazgec.Size = new System.Drawing.Size(100, 38);
            this.lciVazgec.TextVisible = false;
            // 
            // IslemDiyalogu
            // 
            this.AcceptButton = this.btnTamam;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnVazgec;
            this.ClientSize = new System.Drawing.Size(440, 300);
            this.Controls.Add(this.layoutControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Name = "IslemDiyalogu";
            this.Text = "İşlem";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
            this.layoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lciMesaj)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSecim)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSecim.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMetin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMetin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAciklama)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoAciklama.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosOrta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosAlt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTamam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciVazgec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lciMesaj;
        private DevExpress.XtraEditors.LabelControl lblMesaj;
        private DevExpress.XtraLayout.LayoutControlItem lciSecim;
        private DevExpress.XtraEditors.ImageComboBoxEdit cmbSecim;
        private DevExpress.XtraLayout.LayoutControlItem lciMetin;
        private DevExpress.XtraEditors.TextEdit txtMetin;
        private DevExpress.XtraLayout.LayoutControlItem lciAciklama;
        private DevExpress.XtraEditors.MemoEdit memoAciklama;
        private DevExpress.XtraLayout.EmptySpaceItem bosOrta;
        private DevExpress.XtraLayout.EmptySpaceItem bosAlt;
        private DevExpress.XtraLayout.LayoutControlItem lciTamam;
        private DevExpress.XtraEditors.SimpleButton btnTamam;
        private DevExpress.XtraLayout.LayoutControlItem lciVazgec;
        private DevExpress.XtraEditors.SimpleButton btnVazgec;
    }
}
