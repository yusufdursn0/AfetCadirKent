namespace AfetCadirKent.UI.Formlar
{
    partial class IlkKurulumForm
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
            this.txtAd = new DevExpress.XtraEditors.TextEdit();
            this.txtSoyad = new DevExpress.XtraEditors.TextEdit();
            this.txtTelefon = new DevExpress.XtraEditors.TextEdit();
            this.txtKullaniciAdi = new DevExpress.XtraEditors.TextEdit();
            this.txtParola = new DevExpress.XtraEditors.TextEdit();
            this.txtParolaTekrar = new DevExpress.XtraEditors.TextEdit();
            this.lblParolaKurali = new DevExpress.XtraEditors.LabelControl();
            this.btnOlustur = new DevExpress.XtraEditors.SimpleButton();
            this.btnVazgec = new DevExpress.XtraEditors.SimpleButton();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciBaslik = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciAd = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSoyad = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTelefon = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKullaniciAdi = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciParola = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciParolaTekrar = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciParolaKurali = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceOrta = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceAlt = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciOlustur = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciVazgec = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSoyad.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTelefon.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKullaniciAdi.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtParola.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtParolaTekrar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBaslik)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSoyad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTelefon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKullaniciAdi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciParola)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciParolaTekrar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciParolaKurali)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceOrta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceAlt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOlustur)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciVazgec)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.lblBaslik);
            this.layoutControl1.Controls.Add(this.txtAd);
            this.layoutControl1.Controls.Add(this.txtSoyad);
            this.layoutControl1.Controls.Add(this.txtTelefon);
            this.layoutControl1.Controls.Add(this.txtKullaniciAdi);
            this.layoutControl1.Controls.Add(this.txtParola);
            this.layoutControl1.Controls.Add(this.txtParolaTekrar);
            this.layoutControl1.Controls.Add(this.lblParolaKurali);
            this.layoutControl1.Controls.Add(this.btnOlustur);
            this.layoutControl1.Controls.Add(this.btnVazgec);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(460, 420);
            this.layoutControl1.TabIndex = 0;
            //
            // lblBaslik
            //
            this.lblBaslik.AllowHtmlString = true;
            this.lblBaslik.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblBaslik.Location = new System.Drawing.Point(12, 12);
            this.lblBaslik.Name = "lblBaslik";
            this.lblBaslik.Size = new System.Drawing.Size(436, 60);
            this.lblBaslik.StyleController = this.layoutControl1;
            this.lblBaslik.TabIndex = 0;
            this.lblBaslik.Text = "<size=+5><b>İlk yönetici hesabını oluştur</b></size><br>Sistemde henüz kullanıcı yok. Bu hesap <b>Koordinatör</b> yetkisiyle oluşturulur; diğer görevlileri daha sonra bu hesapla eklersiniz.";
            //
            // txtAd
            //
            this.txtAd.Location = new System.Drawing.Point(107, 76);
            this.txtAd.Name = "txtAd";
            this.txtAd.Properties.MaxLength = 50;
            this.txtAd.Size = new System.Drawing.Size(341, 20);
            this.txtAd.StyleController = this.layoutControl1;
            this.txtAd.TabIndex = 1;
            //
            // txtSoyad
            //
            this.txtSoyad.Location = new System.Drawing.Point(107, 100);
            this.txtSoyad.Name = "txtSoyad";
            this.txtSoyad.Properties.MaxLength = 50;
            this.txtSoyad.Size = new System.Drawing.Size(341, 20);
            this.txtSoyad.StyleController = this.layoutControl1;
            this.txtSoyad.TabIndex = 2;
            //
            // txtTelefon
            //
            this.txtTelefon.Location = new System.Drawing.Point(107, 124);
            this.txtTelefon.Name = "txtTelefon";
            this.txtTelefon.Properties.MaxLength = 20;
            this.txtTelefon.Properties.NullValuePrompt = "İsteğe bağlı";
            this.txtTelefon.Size = new System.Drawing.Size(341, 20);
            this.txtTelefon.StyleController = this.layoutControl1;
            this.txtTelefon.TabIndex = 3;
            //
            // txtKullaniciAdi
            //
            this.txtKullaniciAdi.Location = new System.Drawing.Point(107, 148);
            this.txtKullaniciAdi.Name = "txtKullaniciAdi";
            this.txtKullaniciAdi.Properties.MaxLength = 50;
            this.txtKullaniciAdi.Size = new System.Drawing.Size(341, 20);
            this.txtKullaniciAdi.StyleController = this.layoutControl1;
            this.txtKullaniciAdi.TabIndex = 4;
            //
            // txtParola
            //
            this.txtParola.Location = new System.Drawing.Point(107, 172);
            this.txtParola.Name = "txtParola";
            this.txtParola.Properties.MaxLength = 64;
            this.txtParola.Properties.UseSystemPasswordChar = true;
            this.txtParola.Size = new System.Drawing.Size(341, 20);
            this.txtParola.StyleController = this.layoutControl1;
            this.txtParola.TabIndex = 5;
            //
            // txtParolaTekrar
            //
            this.txtParolaTekrar.Location = new System.Drawing.Point(107, 196);
            this.txtParolaTekrar.Name = "txtParolaTekrar";
            this.txtParolaTekrar.Properties.MaxLength = 64;
            this.txtParolaTekrar.Properties.UseSystemPasswordChar = true;
            this.txtParolaTekrar.Size = new System.Drawing.Size(341, 20);
            this.txtParolaTekrar.StyleController = this.layoutControl1;
            this.txtParolaTekrar.TabIndex = 6;
            //
            // lblParolaKurali
            //
            this.lblParolaKurali.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblParolaKurali.Location = new System.Drawing.Point(12, 220);
            this.lblParolaKurali.Name = "lblParolaKurali";
            this.lblParolaKurali.Size = new System.Drawing.Size(436, 40);
            this.lblParolaKurali.StyleController = this.layoutControl1;
            this.lblParolaKurali.TabIndex = 7;
            //
            // btnOlustur
            //
            this.btnOlustur.Location = new System.Drawing.Point(232, 374);
            this.btnOlustur.Name = "btnOlustur";
            this.btnOlustur.Size = new System.Drawing.Size(116, 34);
            this.btnOlustur.StyleController = this.layoutControl1;
            this.btnOlustur.TabIndex = 8;
            this.btnOlustur.Text = "Hesabı oluştur";
            this.btnOlustur.Click += new System.EventHandler(this.btnOlustur_Click);
            //
            // btnVazgec
            //
            this.btnVazgec.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnVazgec.Location = new System.Drawing.Point(352, 374);
            this.btnVazgec.Name = "btnVazgec";
            this.btnVazgec.Size = new System.Drawing.Size(96, 34);
            this.btnVazgec.StyleController = this.layoutControl1;
            this.btnVazgec.TabIndex = 9;
            this.btnVazgec.Text = "Kapat";
            //
            // Root
            //
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBaslik,
            this.lciAd,
            this.lciSoyad,
            this.lciTelefon,
            this.lciKullaniciAdi,
            this.lciParola,
            this.lciParolaTekrar,
            this.lciParolaKurali,
            this.emptySpaceOrta,
            this.emptySpaceAlt,
            this.lciOlustur,
            this.lciVazgec});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(460, 420);
            this.Root.TextVisible = false;
            //
            // lciBaslik
            //
            this.lciBaslik.Control = this.lblBaslik;
            this.lciBaslik.Location = new System.Drawing.Point(0, 0);
            this.lciBaslik.Name = "lciBaslik";
            this.lciBaslik.Size = new System.Drawing.Size(440, 64);
            this.lciBaslik.TextVisible = false;
            //
            // lciAd
            //
            this.lciAd.Control = this.txtAd;
            this.lciAd.Location = new System.Drawing.Point(0, 64);
            this.lciAd.Name = "lciAd";
            this.lciAd.Size = new System.Drawing.Size(440, 24);
            this.lciAd.Text = "Ad";
            this.lciAd.TextSize = new System.Drawing.Size(83, 13);
            //
            // lciSoyad
            //
            this.lciSoyad.Control = this.txtSoyad;
            this.lciSoyad.Location = new System.Drawing.Point(0, 88);
            this.lciSoyad.Name = "lciSoyad";
            this.lciSoyad.Size = new System.Drawing.Size(440, 24);
            this.lciSoyad.Text = "Soyad";
            this.lciSoyad.TextSize = new System.Drawing.Size(83, 13);
            //
            // lciTelefon
            //
            this.lciTelefon.Control = this.txtTelefon;
            this.lciTelefon.Location = new System.Drawing.Point(0, 112);
            this.lciTelefon.Name = "lciTelefon";
            this.lciTelefon.Size = new System.Drawing.Size(440, 24);
            this.lciTelefon.Text = "Telefon";
            this.lciTelefon.TextSize = new System.Drawing.Size(83, 13);
            //
            // lciKullaniciAdi
            //
            this.lciKullaniciAdi.Control = this.txtKullaniciAdi;
            this.lciKullaniciAdi.Location = new System.Drawing.Point(0, 136);
            this.lciKullaniciAdi.Name = "lciKullaniciAdi";
            this.lciKullaniciAdi.Size = new System.Drawing.Size(440, 24);
            this.lciKullaniciAdi.Text = "Kullanıcı adı";
            this.lciKullaniciAdi.TextSize = new System.Drawing.Size(83, 13);
            //
            // lciParola
            //
            this.lciParola.Control = this.txtParola;
            this.lciParola.Location = new System.Drawing.Point(0, 160);
            this.lciParola.Name = "lciParola";
            this.lciParola.Size = new System.Drawing.Size(440, 24);
            this.lciParola.Text = "Parola";
            this.lciParola.TextSize = new System.Drawing.Size(83, 13);
            //
            // lciParolaTekrar
            //
            this.lciParolaTekrar.Control = this.txtParolaTekrar;
            this.lciParolaTekrar.Location = new System.Drawing.Point(0, 184);
            this.lciParolaTekrar.Name = "lciParolaTekrar";
            this.lciParolaTekrar.Size = new System.Drawing.Size(440, 24);
            this.lciParolaTekrar.Text = "Parola (tekrar)";
            this.lciParolaTekrar.TextSize = new System.Drawing.Size(83, 13);
            //
            // lciParolaKurali
            //
            this.lciParolaKurali.Control = this.lblParolaKurali;
            this.lciParolaKurali.Location = new System.Drawing.Point(0, 208);
            this.lciParolaKurali.Name = "lciParolaKurali";
            this.lciParolaKurali.Size = new System.Drawing.Size(440, 44);
            this.lciParolaKurali.TextVisible = false;
            //
            // emptySpaceOrta
            //
            this.emptySpaceOrta.Location = new System.Drawing.Point(0, 252);
            this.emptySpaceOrta.Name = "emptySpaceOrta";
            this.emptySpaceOrta.Size = new System.Drawing.Size(440, 110);
            //
            // emptySpaceAlt
            //
            this.emptySpaceAlt.Location = new System.Drawing.Point(0, 362);
            this.emptySpaceAlt.Name = "emptySpaceAlt";
            this.emptySpaceAlt.Size = new System.Drawing.Size(220, 38);
            //
            // lciOlustur
            //
            this.lciOlustur.Control = this.btnOlustur;
            this.lciOlustur.Location = new System.Drawing.Point(220, 362);
            this.lciOlustur.Name = "lciOlustur";
            this.lciOlustur.Size = new System.Drawing.Size(120, 38);
            this.lciOlustur.TextVisible = false;
            //
            // lciVazgec
            //
            this.lciVazgec.Control = this.btnVazgec;
            this.lciVazgec.Location = new System.Drawing.Point(340, 362);
            this.lciVazgec.Name = "lciVazgec";
            this.lciVazgec.Size = new System.Drawing.Size(100, 38);
            this.lciVazgec.TextVisible = false;
            //
            // IlkKurulumForm
            //
            this.AcceptButton = this.btnOlustur;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnVazgec;
            this.ClientSize = new System.Drawing.Size(460, 420);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IlkKurulumForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Afet Çadır Kent – İlk kurulum";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtAd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSoyad.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTelefon.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKullaniciAdi.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtParola.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtParolaTekrar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBaslik)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSoyad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTelefon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKullaniciAdi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciParola)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciParolaTekrar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciParolaKurali)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceOrta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceAlt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOlustur)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciVazgec)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.LabelControl lblBaslik;
        private DevExpress.XtraEditors.TextEdit txtAd;
        private DevExpress.XtraEditors.TextEdit txtSoyad;
        private DevExpress.XtraEditors.TextEdit txtTelefon;
        private DevExpress.XtraEditors.TextEdit txtKullaniciAdi;
        private DevExpress.XtraEditors.TextEdit txtParola;
        private DevExpress.XtraEditors.TextEdit txtParolaTekrar;
        private DevExpress.XtraEditors.LabelControl lblParolaKurali;
        private DevExpress.XtraEditors.SimpleButton btnOlustur;
        private DevExpress.XtraEditors.SimpleButton btnVazgec;
        private DevExpress.XtraLayout.LayoutControlItem lciBaslik;
        private DevExpress.XtraLayout.LayoutControlItem lciAd;
        private DevExpress.XtraLayout.LayoutControlItem lciSoyad;
        private DevExpress.XtraLayout.LayoutControlItem lciTelefon;
        private DevExpress.XtraLayout.LayoutControlItem lciKullaniciAdi;
        private DevExpress.XtraLayout.LayoutControlItem lciParola;
        private DevExpress.XtraLayout.LayoutControlItem lciParolaTekrar;
        private DevExpress.XtraLayout.LayoutControlItem lciParolaKurali;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceOrta;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceAlt;
        private DevExpress.XtraLayout.LayoutControlItem lciOlustur;
        private DevExpress.XtraLayout.LayoutControlItem lciVazgec;
    }
}
