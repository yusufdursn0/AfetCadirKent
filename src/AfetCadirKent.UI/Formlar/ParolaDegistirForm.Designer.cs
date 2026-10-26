namespace AfetCadirKent.UI.Formlar
{
    partial class ParolaDegistirForm
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.lblBaslik = new DevExpress.XtraEditors.LabelControl();
            this.txtMevcutParola = new DevExpress.XtraEditors.TextEdit();
            this.txtYeniParola = new DevExpress.XtraEditors.TextEdit();
            this.txtYeniParolaTekrar = new DevExpress.XtraEditors.TextEdit();
            this.lblParolaKurali = new DevExpress.XtraEditors.LabelControl();
            this.btnKaydet = new DevExpress.XtraEditors.SimpleButton();
            this.btnVazgec = new DevExpress.XtraEditors.SimpleButton();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciBaslik = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMevcutParola = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciYeniParola = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciYeniParolaTekrar = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciParolaKurali = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceOrta = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceAlt = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciKaydet = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciVazgec = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMevcutParola.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtYeniParola.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtYeniParolaTekrar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBaslik)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMevcutParola)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYeniParola)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYeniParolaTekrar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciParolaKurali)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceOrta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceAlt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKaydet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciVazgec)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.lblBaslik);
            this.layoutControl1.Controls.Add(this.txtMevcutParola);
            this.layoutControl1.Controls.Add(this.txtYeniParola);
            this.layoutControl1.Controls.Add(this.txtYeniParolaTekrar);
            this.layoutControl1.Controls.Add(this.lblParolaKurali);
            this.layoutControl1.Controls.Add(this.btnKaydet);
            this.layoutControl1.Controls.Add(this.btnVazgec);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(420, 250);
            this.layoutControl1.TabIndex = 0;
            //
            // lblBaslik
            //
            this.lblBaslik.AllowHtmlString = true;
            this.lblBaslik.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblBaslik.Location = new System.Drawing.Point(12, 12);
            this.lblBaslik.Name = "lblBaslik";
            this.lblBaslik.Size = new System.Drawing.Size(396, 36);
            this.lblBaslik.StyleController = this.layoutControl1;
            this.lblBaslik.TabIndex = 0;
            this.lblBaslik.Text = "<size=+4><b>Parola değiştir</b></size><br>Önce mevcut parolanızı, sonra yeni parolanızı iki kez girin.";
            //
            // txtMevcutParola
            //
            this.txtMevcutParola.Location = new System.Drawing.Point(119, 52);
            this.txtMevcutParola.Name = "txtMevcutParola";
            this.txtMevcutParola.Properties.MaxLength = 64;
            this.txtMevcutParola.Properties.UseSystemPasswordChar = true;
            this.txtMevcutParola.Size = new System.Drawing.Size(289, 20);
            this.txtMevcutParola.StyleController = this.layoutControl1;
            this.txtMevcutParola.TabIndex = 1;
            //
            // txtYeniParola
            //
            this.txtYeniParola.Location = new System.Drawing.Point(119, 76);
            this.txtYeniParola.Name = "txtYeniParola";
            this.txtYeniParola.Properties.MaxLength = 64;
            this.txtYeniParola.Properties.UseSystemPasswordChar = true;
            this.txtYeniParola.Size = new System.Drawing.Size(289, 20);
            this.txtYeniParola.StyleController = this.layoutControl1;
            this.txtYeniParola.TabIndex = 2;
            //
            // txtYeniParolaTekrar
            //
            this.txtYeniParolaTekrar.Location = new System.Drawing.Point(119, 100);
            this.txtYeniParolaTekrar.Name = "txtYeniParolaTekrar";
            this.txtYeniParolaTekrar.Properties.MaxLength = 64;
            this.txtYeniParolaTekrar.Properties.UseSystemPasswordChar = true;
            this.txtYeniParolaTekrar.Size = new System.Drawing.Size(289, 20);
            this.txtYeniParolaTekrar.StyleController = this.layoutControl1;
            this.txtYeniParolaTekrar.TabIndex = 3;
            //
            // lblParolaKurali
            //
            this.lblParolaKurali.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblParolaKurali.Location = new System.Drawing.Point(12, 124);
            this.lblParolaKurali.Name = "lblParolaKurali";
            this.lblParolaKurali.Size = new System.Drawing.Size(396, 40);
            this.lblParolaKurali.StyleController = this.layoutControl1;
            this.lblParolaKurali.TabIndex = 4;
            //
            // btnKaydet
            //
            this.btnKaydet.Location = new System.Drawing.Point(212, 204);
            this.btnKaydet.Name = "btnKaydet";
            this.btnKaydet.Size = new System.Drawing.Size(96, 34);
            this.btnKaydet.StyleController = this.layoutControl1;
            this.btnKaydet.TabIndex = 5;
            this.btnKaydet.Text = "Kaydet";
            this.btnKaydet.Click += new System.EventHandler(this.btnKaydet_Click);
            //
            // btnVazgec
            //
            this.btnVazgec.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnVazgec.Location = new System.Drawing.Point(312, 204);
            this.btnVazgec.Name = "btnVazgec";
            this.btnVazgec.Size = new System.Drawing.Size(96, 34);
            this.btnVazgec.StyleController = this.layoutControl1;
            this.btnVazgec.TabIndex = 6;
            this.btnVazgec.Text = "Vazgeç";
            //
            // Root
            //
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBaslik,
            this.lciMevcutParola,
            this.lciYeniParola,
            this.lciYeniParolaTekrar,
            this.lciParolaKurali,
            this.emptySpaceOrta,
            this.emptySpaceAlt,
            this.lciKaydet,
            this.lciVazgec});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(420, 250);
            this.Root.TextVisible = false;
            //
            // lciBaslik
            //
            this.lciBaslik.Control = this.lblBaslik;
            this.lciBaslik.Location = new System.Drawing.Point(0, 0);
            this.lciBaslik.Name = "lciBaslik";
            this.lciBaslik.Size = new System.Drawing.Size(400, 40);
            this.lciBaslik.TextVisible = false;
            //
            // lciMevcutParola
            //
            this.lciMevcutParola.Control = this.txtMevcutParola;
            this.lciMevcutParola.Location = new System.Drawing.Point(0, 40);
            this.lciMevcutParola.Name = "lciMevcutParola";
            this.lciMevcutParola.Size = new System.Drawing.Size(400, 24);
            this.lciMevcutParola.Text = "Mevcut parola";
            this.lciMevcutParola.TextSize = new System.Drawing.Size(95, 13);
            //
            // lciYeniParola
            //
            this.lciYeniParola.Control = this.txtYeniParola;
            this.lciYeniParola.Location = new System.Drawing.Point(0, 64);
            this.lciYeniParola.Name = "lciYeniParola";
            this.lciYeniParola.Size = new System.Drawing.Size(400, 24);
            this.lciYeniParola.Text = "Yeni parola";
            this.lciYeniParola.TextSize = new System.Drawing.Size(95, 13);
            //
            // lciYeniParolaTekrar
            //
            this.lciYeniParolaTekrar.Control = this.txtYeniParolaTekrar;
            this.lciYeniParolaTekrar.Location = new System.Drawing.Point(0, 88);
            this.lciYeniParolaTekrar.Name = "lciYeniParolaTekrar";
            this.lciYeniParolaTekrar.Size = new System.Drawing.Size(400, 24);
            this.lciYeniParolaTekrar.Text = "Yeni parola (tekrar)";
            this.lciYeniParolaTekrar.TextSize = new System.Drawing.Size(95, 13);
            //
            // lciParolaKurali
            //
            this.lciParolaKurali.Control = this.lblParolaKurali;
            this.lciParolaKurali.Location = new System.Drawing.Point(0, 112);
            this.lciParolaKurali.Name = "lciParolaKurali";
            this.lciParolaKurali.Size = new System.Drawing.Size(400, 44);
            this.lciParolaKurali.TextVisible = false;
            //
            // emptySpaceOrta
            //
            this.emptySpaceOrta.Location = new System.Drawing.Point(0, 156);
            this.emptySpaceOrta.Name = "emptySpaceOrta";
            this.emptySpaceOrta.Size = new System.Drawing.Size(400, 36);
            //
            // emptySpaceAlt
            //
            this.emptySpaceAlt.Location = new System.Drawing.Point(0, 192);
            this.emptySpaceAlt.Name = "emptySpaceAlt";
            this.emptySpaceAlt.Size = new System.Drawing.Size(200, 38);
            //
            // lciKaydet
            //
            this.lciKaydet.Control = this.btnKaydet;
            this.lciKaydet.Location = new System.Drawing.Point(200, 192);
            this.lciKaydet.Name = "lciKaydet";
            this.lciKaydet.Size = new System.Drawing.Size(100, 38);
            this.lciKaydet.TextVisible = false;
            //
            // lciVazgec
            //
            this.lciVazgec.Control = this.btnVazgec;
            this.lciVazgec.Location = new System.Drawing.Point(300, 192);
            this.lciVazgec.Name = "lciVazgec";
            this.lciVazgec.Size = new System.Drawing.Size(100, 38);
            this.lciVazgec.TextVisible = false;
            //
            // ParolaDegistirForm
            //
            this.AcceptButton = this.btnKaydet;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnVazgec;
            this.ClientSize = new System.Drawing.Size(420, 250);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ParolaDegistirForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Parola değiştir";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtMevcutParola.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtYeniParola.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtYeniParolaTekrar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBaslik)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMevcutParola)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYeniParola)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYeniParolaTekrar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciParolaKurali)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceOrta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceAlt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKaydet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciVazgec)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.LabelControl lblBaslik;
        private DevExpress.XtraEditors.TextEdit txtMevcutParola;
        private DevExpress.XtraEditors.TextEdit txtYeniParola;
        private DevExpress.XtraEditors.TextEdit txtYeniParolaTekrar;
        private DevExpress.XtraEditors.LabelControl lblParolaKurali;
        private DevExpress.XtraEditors.SimpleButton btnKaydet;
        private DevExpress.XtraEditors.SimpleButton btnVazgec;
        private DevExpress.XtraLayout.LayoutControlItem lciBaslik;
        private DevExpress.XtraLayout.LayoutControlItem lciMevcutParola;
        private DevExpress.XtraLayout.LayoutControlItem lciYeniParola;
        private DevExpress.XtraLayout.LayoutControlItem lciYeniParolaTekrar;
        private DevExpress.XtraLayout.LayoutControlItem lciParolaKurali;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceOrta;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceAlt;
        private DevExpress.XtraLayout.LayoutControlItem lciKaydet;
        private DevExpress.XtraLayout.LayoutControlItem lciVazgec;
    }
}
