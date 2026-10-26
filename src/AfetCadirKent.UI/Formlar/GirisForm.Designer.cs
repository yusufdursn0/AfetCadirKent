namespace AfetCadirKent.UI.Formlar
{
    partial class GirisForm
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
            this.txtKullaniciAdi = new DevExpress.XtraEditors.TextEdit();
            this.txtParola = new DevExpress.XtraEditors.TextEdit();
            this.btnGiris = new DevExpress.XtraEditors.SimpleButton();
            this.btnKapat = new DevExpress.XtraEditors.SimpleButton();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciBaslik = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKullaniciAdi = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciParola = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceOrta = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceAlt = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciGiris = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKapat = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKullaniciAdi.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtParola.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBaslik)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKullaniciAdi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciParola)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceOrta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceAlt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGiris)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKapat)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.lblBaslik);
            this.layoutControl1.Controls.Add(this.txtKullaniciAdi);
            this.layoutControl1.Controls.Add(this.txtParola);
            this.layoutControl1.Controls.Add(this.btnGiris);
            this.layoutControl1.Controls.Add(this.btnKapat);
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
            this.lblBaslik.Size = new System.Drawing.Size(396, 46);
            this.lblBaslik.StyleController = this.layoutControl1;
            this.lblBaslik.TabIndex = 0;
            this.lblBaslik.Text = "<size=+6><b>Afet Çadır Kent</b></size><br>Kullanıcı adınız ve parolanızla giriş yapın.";
            //
            // txtKullaniciAdi
            //
            this.txtKullaniciAdi.Location = new System.Drawing.Point(95, 62);
            this.txtKullaniciAdi.Name = "txtKullaniciAdi";
            this.txtKullaniciAdi.Properties.MaxLength = 50;
            this.txtKullaniciAdi.Size = new System.Drawing.Size(313, 20);
            this.txtKullaniciAdi.StyleController = this.layoutControl1;
            this.txtKullaniciAdi.TabIndex = 1;
            //
            // txtParola
            //
            this.txtParola.Location = new System.Drawing.Point(95, 86);
            this.txtParola.Name = "txtParola";
            this.txtParola.Properties.MaxLength = 64;
            this.txtParola.Properties.UseSystemPasswordChar = true;
            this.txtParola.Size = new System.Drawing.Size(313, 20);
            this.txtParola.StyleController = this.layoutControl1;
            this.txtParola.TabIndex = 2;
            //
            // btnGiris
            //
            this.btnGiris.Location = new System.Drawing.Point(212, 204);
            this.btnGiris.Name = "btnGiris";
            this.btnGiris.Size = new System.Drawing.Size(96, 34);
            this.btnGiris.StyleController = this.layoutControl1;
            this.btnGiris.TabIndex = 3;
            this.btnGiris.Text = "Giriş";
            this.btnGiris.Click += new System.EventHandler(this.btnGiris_Click);
            //
            // btnKapat
            //
            this.btnKapat.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnKapat.Location = new System.Drawing.Point(312, 204);
            this.btnKapat.Name = "btnKapat";
            this.btnKapat.Size = new System.Drawing.Size(96, 34);
            this.btnKapat.StyleController = this.layoutControl1;
            this.btnKapat.TabIndex = 4;
            this.btnKapat.Text = "Kapat";
            //
            // Root
            //
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBaslik,
            this.lciKullaniciAdi,
            this.lciParola,
            this.emptySpaceOrta,
            this.emptySpaceAlt,
            this.lciGiris,
            this.lciKapat});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(420, 250);
            this.Root.TextVisible = false;
            //
            // lciBaslik
            //
            this.lciBaslik.Control = this.lblBaslik;
            this.lciBaslik.Location = new System.Drawing.Point(0, 0);
            this.lciBaslik.Name = "lciBaslik";
            this.lciBaslik.Size = new System.Drawing.Size(400, 50);
            this.lciBaslik.TextVisible = false;
            //
            // lciKullaniciAdi
            //
            this.lciKullaniciAdi.Control = this.txtKullaniciAdi;
            this.lciKullaniciAdi.Location = new System.Drawing.Point(0, 50);
            this.lciKullaniciAdi.Name = "lciKullaniciAdi";
            this.lciKullaniciAdi.Size = new System.Drawing.Size(400, 24);
            this.lciKullaniciAdi.Text = "Kullanıcı adı";
            this.lciKullaniciAdi.TextSize = new System.Drawing.Size(71, 13);
            //
            // lciParola
            //
            this.lciParola.Control = this.txtParola;
            this.lciParola.Location = new System.Drawing.Point(0, 74);
            this.lciParola.Name = "lciParola";
            this.lciParola.Size = new System.Drawing.Size(400, 24);
            this.lciParola.Text = "Parola";
            this.lciParola.TextSize = new System.Drawing.Size(71, 13);
            //
            // emptySpaceOrta
            //
            this.emptySpaceOrta.Location = new System.Drawing.Point(0, 98);
            this.emptySpaceOrta.Name = "emptySpaceOrta";
            this.emptySpaceOrta.Size = new System.Drawing.Size(400, 94);
            //
            // emptySpaceAlt
            //
            this.emptySpaceAlt.Location = new System.Drawing.Point(0, 192);
            this.emptySpaceAlt.Name = "emptySpaceAlt";
            this.emptySpaceAlt.Size = new System.Drawing.Size(200, 38);
            //
            // lciGiris
            //
            this.lciGiris.Control = this.btnGiris;
            this.lciGiris.Location = new System.Drawing.Point(200, 192);
            this.lciGiris.Name = "lciGiris";
            this.lciGiris.Size = new System.Drawing.Size(100, 38);
            this.lciGiris.TextVisible = false;
            //
            // lciKapat
            //
            this.lciKapat.Control = this.btnKapat;
            this.lciKapat.Location = new System.Drawing.Point(300, 192);
            this.lciKapat.Name = "lciKapat";
            this.lciKapat.Size = new System.Drawing.Size(100, 38);
            this.lciKapat.TextVisible = false;
            //
            // GirisForm
            //
            this.AcceptButton = this.btnGiris;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnKapat;
            this.ClientSize = new System.Drawing.Size(420, 250);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GirisForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Afet Çadır Kent – Giriş";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKullaniciAdi.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtParola.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBaslik)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKullaniciAdi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciParola)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceOrta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceAlt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGiris)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKapat)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.LabelControl lblBaslik;
        private DevExpress.XtraEditors.TextEdit txtKullaniciAdi;
        private DevExpress.XtraEditors.TextEdit txtParola;
        private DevExpress.XtraEditors.SimpleButton btnGiris;
        private DevExpress.XtraEditors.SimpleButton btnKapat;
        private DevExpress.XtraLayout.LayoutControlItem lciBaslik;
        private DevExpress.XtraLayout.LayoutControlItem lciKullaniciAdi;
        private DevExpress.XtraLayout.LayoutControlItem lciParola;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceOrta;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceAlt;
        private DevExpress.XtraLayout.LayoutControlItem lciGiris;
        private DevExpress.XtraLayout.LayoutControlItem lciKapat;
    }
}
