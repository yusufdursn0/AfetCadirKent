namespace AfetCadirKent.UI.Formlar
{
    partial class AyarlarForm
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
            this.tabKullanicilar = new DevExpress.XtraTab.XtraTabPage();
            this.tabRoller = new DevExpress.XtraTab.XtraTabPage();
            this.tabGerekceler = new DevExpress.XtraTab.XtraTabPage();
            this.tabDuyurular = new DevExpress.XtraTab.XtraTabPage();
            this.layoutKullanici = new DevExpress.XtraLayout.LayoutControl();
            this.chkKPasifler = new DevExpress.XtraEditors.CheckEdit();
            this.gridKullanicilar = new DevExpress.XtraGrid.GridControl();
            this.viewKullanicilar = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colKAd = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colKKullanici = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colKRol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colKTelefon = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colKDurum = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colKSonGiris = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtKAd = new DevExpress.XtraEditors.TextEdit();
            this.txtKSoyad = new DevExpress.XtraEditors.TextEdit();
            this.txtKTelefon = new DevExpress.XtraEditors.TextEdit();
            this.txtKKullaniciAdi = new DevExpress.XtraEditors.TextEdit();
            this.cmbKRol = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.txtKParola = new DevExpress.XtraEditors.TextEdit();
            this.txtKParolaTekrar = new DevExpress.XtraEditors.TextEdit();
            this.lblKParolaNot = new DevExpress.XtraEditors.LabelControl();
            this.btnKYeni = new DevExpress.XtraEditors.SimpleButton();
            this.btnKKaydet = new DevExpress.XtraEditors.SimpleButton();
            this.btnKParola = new DevExpress.XtraEditors.SimpleButton();
            this.btnKAktiflik = new DevExpress.XtraEditors.SimpleButton();
            this.btnKSil = new DevExpress.XtraEditors.SimpleButton();
            this.lciKPasifler = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKullanicilar = new DevExpress.XtraLayout.LayoutControlItem();
            this.grpKullanicilar = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciKAd = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKSoyad = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKTelefon = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKKullaniciAdi = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKRol = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKParola = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKParolaTekrar = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKParolaNot = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKYeni = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKKaydet = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKParolaSifirla = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKAktiflik = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKSil = new DevExpress.XtraLayout.LayoutControlItem();
            this.bosKSil = new DevExpress.XtraLayout.EmptySpaceItem();
            this.bosKAlt = new DevExpress.XtraLayout.EmptySpaceItem();
            this.grpKullanici = new DevExpress.XtraLayout.LayoutControlGroup();
            this.RootKullanici = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutRol = new DevExpress.XtraLayout.LayoutControl();
            this.lblRolBilgi = new DevExpress.XtraEditors.LabelControl();
            this.gridYetkiler = new DevExpress.XtraGrid.GridControl();
            this.viewYetkiler = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colYRol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colYModul = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colYGorme = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colYDegistirme = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colYRapor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lciRolBilgi = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciYetkiler = new DevExpress.XtraLayout.LayoutControlItem();
            this.RootRol = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutGerekce = new DevExpress.XtraLayout.LayoutControl();
            this.lblGerekceBilgi = new DevExpress.XtraEditors.LabelControl();
            this.gridGerekceler = new DevExpress.XtraGrid.GridControl();
            this.viewGerekceler = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colGGerekce = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGOncelik = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGEtki = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGUrunler = new DevExpress.XtraGrid.Columns.GridColumn();
            this.chklUrunler = new DevExpress.XtraEditors.CheckedListBoxControl();
            this.txtGAciklama = new DevExpress.XtraEditors.TextEdit();
            this.btnGKaydet = new DevExpress.XtraEditors.SimpleButton();
            this.lciGerekceBilgi = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGerekceler = new DevExpress.XtraLayout.LayoutControlItem();
            this.grpGerekceler = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciUrunler = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGAciklama = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGKaydet = new DevExpress.XtraLayout.LayoutControlItem();
            this.bosGerekce = new DevExpress.XtraLayout.EmptySpaceItem();
            this.grpEslesme = new DevExpress.XtraLayout.LayoutControlGroup();
            this.RootGerekce = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutDuyuru = new DevExpress.XtraLayout.LayoutControl();
            this.chkDPasifler = new DevExpress.XtraEditors.CheckEdit();
            this.gridDuyurular = new DevExpress.XtraGrid.GridControl();
            this.viewDuyurular = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colDBaslik = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDBaslangic = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDBitis = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDDurum = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDEkleyen = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtDBaslik = new DevExpress.XtraEditors.TextEdit();
            this.memoDMetin = new DevExpress.XtraEditors.MemoEdit();
            this.dteDBaslangic = new DevExpress.XtraEditors.DateEdit();
            this.dteDBitis = new DevExpress.XtraEditors.DateEdit();
            this.btnDYeni = new DevExpress.XtraEditors.SimpleButton();
            this.btnDKaydet = new DevExpress.XtraEditors.SimpleButton();
            this.btnDAktiflik = new DevExpress.XtraEditors.SimpleButton();
            this.lciDPasifler = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDuyurular = new DevExpress.XtraLayout.LayoutControlItem();
            this.grpDuyurular = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciDBaslik = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDMetin = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDBaslangic = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDBitis = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDYeni = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDKaydet = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDAktiflik = new DevExpress.XtraLayout.LayoutControlItem();
            this.bosDAktiflik = new DevExpress.XtraLayout.EmptySpaceItem();
            this.bosDAlt = new DevExpress.XtraLayout.EmptySpaceItem();
            this.grpDuyuru = new DevExpress.XtraLayout.LayoutControlGroup();
            this.RootDuyuru = new DevExpress.XtraLayout.LayoutControlGroup();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabKullanicilar.SuspendLayout();
            this.tabRoller.SuspendLayout();
            this.tabGerekceler.SuspendLayout();
            this.tabDuyurular.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutKullanici)).BeginInit();
            this.layoutKullanici.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkKPasifler.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridKullanicilar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewKullanicilar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKAd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKSoyad.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKTelefon.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKKullaniciAdi.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbKRol.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKParola.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKParolaTekrar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKPasifler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKullanicilar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpKullanicilar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKAd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKSoyad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKTelefon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKKullaniciAdi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKRol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKParola)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKParolaTekrar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKParolaNot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKYeni)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKKaydet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKParolaSifirla)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKAktiflik)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKSil)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosKSil)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosKAlt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpKullanici)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootKullanici)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutRol)).BeginInit();
            this.layoutRol.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridYetkiler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewYetkiler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRolBilgi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYetkiler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootRol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutGerekce)).BeginInit();
            this.layoutGerekce.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridGerekceler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewGerekceler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chklUrunler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGAciklama.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGerekceBilgi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGerekceler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpGerekceler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciUrunler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGAciklama)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGKaydet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosGerekce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpEslesme)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootGerekce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDuyuru)).BeginInit();
            this.layoutDuyuru.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkDPasifler.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDuyurular)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDuyurular)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDBaslik.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoDMetin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDBaslangic.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDBaslangic.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDBitis.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDBitis.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDPasifler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDuyurular)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpDuyurular)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDBaslik)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDMetin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDBaslangic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDBitis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDYeni)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDKaydet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDAktiflik)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosDAktiflik)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosDAlt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpDuyuru)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootDuyuru)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedTabPage = this.tabKullanicilar;
            this.tabControl.Size = new System.Drawing.Size(1240, 760);
            this.tabControl.TabIndex = 0;
            this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabKullanicilar,
            this.tabRoller,
            this.tabGerekceler,
            this.tabDuyurular});
            // 
            // tabKullanicilar
            // 
            this.tabKullanicilar.Controls.Add(this.layoutKullanici);
            this.tabKullanicilar.Name = "tabKullanicilar";
            this.tabKullanicilar.Size = new System.Drawing.Size(1238, 735);
            this.tabKullanicilar.Text = "Kullanıcılar";
            // 
            // tabRoller
            // 
            this.tabRoller.Controls.Add(this.layoutRol);
            this.tabRoller.Name = "tabRoller";
            this.tabRoller.Size = new System.Drawing.Size(1238, 735);
            this.tabRoller.Text = "Roller ve yetkiler";
            // 
            // tabGerekceler
            // 
            this.tabGerekceler.Controls.Add(this.layoutGerekce);
            this.tabGerekceler.Name = "tabGerekceler";
            this.tabGerekceler.Size = new System.Drawing.Size(1238, 735);
            this.tabGerekceler.Text = "Gerekçe ayarları";
            // 
            // tabDuyurular
            // 
            this.tabDuyurular.Controls.Add(this.layoutDuyuru);
            this.tabDuyurular.Name = "tabDuyurular";
            this.tabDuyurular.Size = new System.Drawing.Size(1238, 735);
            this.tabDuyurular.Text = "Duyurular";
            // 
            // layoutKullanici
            // 
            this.layoutKullanici.Controls.Add(this.chkKPasifler);
            this.layoutKullanici.Controls.Add(this.gridKullanicilar);
            this.layoutKullanici.Controls.Add(this.txtKAd);
            this.layoutKullanici.Controls.Add(this.txtKSoyad);
            this.layoutKullanici.Controls.Add(this.txtKTelefon);
            this.layoutKullanici.Controls.Add(this.txtKKullaniciAdi);
            this.layoutKullanici.Controls.Add(this.cmbKRol);
            this.layoutKullanici.Controls.Add(this.txtKParola);
            this.layoutKullanici.Controls.Add(this.txtKParolaTekrar);
            this.layoutKullanici.Controls.Add(this.lblKParolaNot);
            this.layoutKullanici.Controls.Add(this.btnKYeni);
            this.layoutKullanici.Controls.Add(this.btnKKaydet);
            this.layoutKullanici.Controls.Add(this.btnKParola);
            this.layoutKullanici.Controls.Add(this.btnKAktiflik);
            this.layoutKullanici.Controls.Add(this.btnKSil);
            this.layoutKullanici.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutKullanici.Location = new System.Drawing.Point(0, 0);
            this.layoutKullanici.Name = "layoutKullanici";
            this.layoutKullanici.Root = this.RootKullanici;
            this.layoutKullanici.Size = new System.Drawing.Size(1238, 735);
            this.layoutKullanici.TabIndex = 0;
            // 
            // chkKPasifler
            // 
            this.chkKPasifler.Location = new System.Drawing.Point(24, 45);
            this.chkKPasifler.Name = "chkKPasifler";
            this.chkKPasifler.Properties.Caption = "Pasif kullanıcıları da göster";
            this.chkKPasifler.Size = new System.Drawing.Size(760, 22);
            this.chkKPasifler.StyleController = this.layoutKullanici;
            this.chkKPasifler.TabIndex = 1;
            this.chkKPasifler.CheckedChanged += new System.EventHandler(this.chkKPasifler_CheckedChanged);
            // 
            // gridKullanicilar
            // 
            this.gridKullanicilar.Location = new System.Drawing.Point(24, 71);
            this.gridKullanicilar.MainView = this.viewKullanicilar;
            this.gridKullanicilar.Name = "gridKullanicilar";
            this.gridKullanicilar.Size = new System.Drawing.Size(760, 640);
            this.gridKullanicilar.TabIndex = 2;
            this.gridKullanicilar.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewKullanicilar});
            // 
            // viewKullanicilar
            // 
            this.viewKullanicilar.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colKAd,
            this.colKKullanici,
            this.colKRol,
            this.colKTelefon,
            this.colKDurum,
            this.colKSonGiris});
            this.viewKullanicilar.GridControl = this.gridKullanicilar;
            this.viewKullanicilar.Name = "viewKullanicilar";
            this.viewKullanicilar.OptionsBehavior.Editable = false;
            this.viewKullanicilar.OptionsView.ShowGroupPanel = false;
            this.viewKullanicilar.OptionsView.ShowIndicator = false;
            this.viewKullanicilar.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.viewKullanicilar_FocusedRowChanged);
            // 
            // colKAd
            // 
            this.colKAd.Caption = "Ad soyad";
            this.colKAd.FieldName = "AdSoyad";
            this.colKAd.Name = "colKAd";
            this.colKAd.Visible = true;
            this.colKAd.VisibleIndex = 0;
            this.colKAd.Width = 150;
            // 
            // colKKullanici
            // 
            this.colKKullanici.Caption = "Kullanıcı adı";
            this.colKKullanici.FieldName = "KullaniciAdi";
            this.colKKullanici.Name = "colKKullanici";
            this.colKKullanici.Visible = true;
            this.colKKullanici.VisibleIndex = 1;
            this.colKKullanici.Width = 110;
            // 
            // colKRol
            // 
            this.colKRol.Caption = "Rol";
            this.colKRol.FieldName = "RolAdi";
            this.colKRol.Name = "colKRol";
            this.colKRol.Visible = true;
            this.colKRol.VisibleIndex = 2;
            this.colKRol.Width = 120;
            // 
            // colKTelefon
            // 
            this.colKTelefon.Caption = "Telefon";
            this.colKTelefon.FieldName = "Telefon";
            this.colKTelefon.Name = "colKTelefon";
            this.colKTelefon.Visible = true;
            this.colKTelefon.VisibleIndex = 3;
            this.colKTelefon.Width = 100;
            // 
            // colKDurum
            // 
            this.colKDurum.Caption = "Durum";
            this.colKDurum.FieldName = "Durum";
            this.colKDurum.Name = "colKDurum";
            this.colKDurum.Visible = true;
            this.colKDurum.VisibleIndex = 4;
            this.colKDurum.Width = 60;
            // 
            // colKSonGiris
            // 
            this.colKSonGiris.Caption = "Son giriş";
            this.colKSonGiris.FieldName = "SonGirisZamani";
            this.colKSonGiris.Name = "colKSonGiris";
            this.colKSonGiris.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";
            this.colKSonGiris.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colKSonGiris.Visible = true;
            this.colKSonGiris.VisibleIndex = 5;
            this.colKSonGiris.Width = 110;
            // 
            // txtKAd
            // 
            this.txtKAd.Location = new System.Drawing.Point(920, 45);
            this.txtKAd.Name = "txtKAd";
            this.txtKAd.Properties.MaxLength = 50;
            this.txtKAd.Size = new System.Drawing.Size(294, 22);
            this.txtKAd.StyleController = this.layoutKullanici;
            this.txtKAd.TabIndex = 3;
            // 
            // txtKSoyad
            // 
            this.txtKSoyad.Location = new System.Drawing.Point(920, 71);
            this.txtKSoyad.Name = "txtKSoyad";
            this.txtKSoyad.Properties.MaxLength = 50;
            this.txtKSoyad.Size = new System.Drawing.Size(294, 22);
            this.txtKSoyad.StyleController = this.layoutKullanici;
            this.txtKSoyad.TabIndex = 4;
            // 
            // txtKTelefon
            // 
            this.txtKTelefon.Location = new System.Drawing.Point(920, 97);
            this.txtKTelefon.Name = "txtKTelefon";
            this.txtKTelefon.Properties.MaxLength = 20;
            this.txtKTelefon.Size = new System.Drawing.Size(294, 22);
            this.txtKTelefon.StyleController = this.layoutKullanici;
            this.txtKTelefon.TabIndex = 5;
            // 
            // txtKKullaniciAdi
            // 
            this.txtKKullaniciAdi.Location = new System.Drawing.Point(920, 123);
            this.txtKKullaniciAdi.Name = "txtKKullaniciAdi";
            this.txtKKullaniciAdi.Properties.MaxLength = 50;
            this.txtKKullaniciAdi.Size = new System.Drawing.Size(294, 22);
            this.txtKKullaniciAdi.StyleController = this.layoutKullanici;
            this.txtKKullaniciAdi.TabIndex = 6;
            // 
            // cmbKRol
            // 
            this.cmbKRol.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbKRol.Location = new System.Drawing.Point(920, 149);
            this.cmbKRol.Name = "cmbKRol";
            this.cmbKRol.Size = new System.Drawing.Size(294, 22);
            this.cmbKRol.StyleController = this.layoutKullanici;
            this.cmbKRol.TabIndex = 7;
            // 
            // txtKParola
            // 
            this.txtKParola.Location = new System.Drawing.Point(920, 175);
            this.txtKParola.Name = "txtKParola";
            this.txtKParola.Properties.MaxLength = 64;
            this.txtKParola.Size = new System.Drawing.Size(294, 22);
            this.txtKParola.StyleController = this.layoutKullanici;
            this.txtKParola.TabIndex = 8;
            // 
            // txtKParolaTekrar
            // 
            this.txtKParolaTekrar.Location = new System.Drawing.Point(920, 201);
            this.txtKParolaTekrar.Name = "txtKParolaTekrar";
            this.txtKParolaTekrar.Properties.MaxLength = 64;
            this.txtKParolaTekrar.Size = new System.Drawing.Size(294, 22);
            this.txtKParolaTekrar.StyleController = this.layoutKullanici;
            this.txtKParolaTekrar.TabIndex = 9;
            // 
            // lblKParolaNot
            // 
            this.lblKParolaNot.Location = new System.Drawing.Point(808, 227);
            this.lblKParolaNot.Name = "lblKParolaNot";
            this.lblKParolaNot.AllowHtmlString = true;
            this.lblKParolaNot.Appearance.Options.UseTextOptions = true;
            this.lblKParolaNot.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblKParolaNot.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblKParolaNot.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblKParolaNot.Text = "Parola en az 8, en fazla 64 karakter olmalı; en az bir harf ve bir rakam içermeli, kullanıcı adıyla aynı olmamalı. Kullanıcı adı sonradan değiştirilemez.";
            this.lblKParolaNot.Size = new System.Drawing.Size(406, 50);
            this.lblKParolaNot.StyleController = this.layoutKullanici;
            this.lblKParolaNot.TabIndex = 10;
            // 
            // btnKYeni
            // 
            this.btnKYeni.Location = new System.Drawing.Point(808, 281);
            this.btnKYeni.Name = "btnKYeni";
            this.btnKYeni.Text = "Yeni kullanıcı";
            this.btnKYeni.Size = new System.Drawing.Size(200, 30);
            this.btnKYeni.StyleController = this.layoutKullanici;
            this.btnKYeni.TabIndex = 11;
            this.btnKYeni.Click += new System.EventHandler(this.btnKYeni_Click);
            // 
            // btnKKaydet
            // 
            this.btnKKaydet.Location = new System.Drawing.Point(1012, 281);
            this.btnKKaydet.Name = "btnKKaydet";
            this.btnKKaydet.Text = "Kaydet";
            this.btnKKaydet.Size = new System.Drawing.Size(202, 30);
            this.btnKKaydet.StyleController = this.layoutKullanici;
            this.btnKKaydet.TabIndex = 12;
            this.btnKKaydet.Click += new System.EventHandler(this.btnKKaydet_Click);
            // 
            // btnKParola
            // 
            this.btnKParola.Location = new System.Drawing.Point(808, 315);
            this.btnKParola.Name = "btnKParola";
            this.btnKParola.Text = "Parolayı sıfırla";
            this.btnKParola.Size = new System.Drawing.Size(200, 30);
            this.btnKParola.StyleController = this.layoutKullanici;
            this.btnKParola.TabIndex = 13;
            this.btnKParola.Click += new System.EventHandler(this.btnKParola_Click);
            // 
            // btnKAktiflik
            // 
            this.btnKAktiflik.Location = new System.Drawing.Point(1012, 315);
            this.btnKAktiflik.Name = "btnKAktiflik";
            this.btnKAktiflik.Text = "Pasif yap";
            this.btnKAktiflik.Size = new System.Drawing.Size(202, 30);
            this.btnKAktiflik.StyleController = this.layoutKullanici;
            this.btnKAktiflik.TabIndex = 14;
            this.btnKAktiflik.Click += new System.EventHandler(this.btnKAktiflik_Click);
            // 
            // btnKSil
            // 
            this.btnKSil.Location = new System.Drawing.Point(808, 349);
            this.btnKSil.Name = "btnKSil";
            this.btnKSil.Text = "Sil";
            this.btnKSil.Size = new System.Drawing.Size(200, 30);
            this.btnKSil.StyleController = this.layoutKullanici;
            this.btnKSil.TabIndex = 15;
            this.btnKSil.Click += new System.EventHandler(this.btnKSil_Click);
            // 
            // lciKPasifler
            // 
            this.lciKPasifler.Control = this.chkKPasifler;
            this.lciKPasifler.Location = new System.Drawing.Point(0, 0);
            this.lciKPasifler.MaxSize = new System.Drawing.Size(0, 26);
            this.lciKPasifler.MinSize = new System.Drawing.Size(80, 26);
            this.lciKPasifler.Name = "lciKPasifler";
            this.lciKPasifler.Size = new System.Drawing.Size(764, 26);
            this.lciKPasifler.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKPasifler.TextVisible = false;
            // 
            // lciKullanicilar
            // 
            this.lciKullanicilar.Control = this.gridKullanicilar;
            this.lciKullanicilar.Location = new System.Drawing.Point(0, 26);
            this.lciKullanicilar.MinSize = new System.Drawing.Size(300, 100);
            this.lciKullanicilar.Name = "lciKullanicilar";
            this.lciKullanicilar.Size = new System.Drawing.Size(764, 649);
            this.lciKullanicilar.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKullanicilar.TextVisible = false;
            // 
            // grpKullanicilar
            // 
            this.grpKullanicilar.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciKPasifler,
            this.lciKullanicilar});
            this.grpKullanicilar.Location = new System.Drawing.Point(0, 0);
            this.grpKullanicilar.Name = "grpKullanicilar";
            this.grpKullanicilar.Size = new System.Drawing.Size(788, 715);
            this.grpKullanicilar.Text = "Kullanıcılar";
            // 
            // lciKAd
            // 
            this.lciKAd.Control = this.txtKAd;
            this.lciKAd.Location = new System.Drawing.Point(0, 0);
            this.lciKAd.MaxSize = new System.Drawing.Size(0, 26);
            this.lciKAd.MinSize = new System.Drawing.Size(80, 26);
            this.lciKAd.Name = "lciKAd";
            this.lciKAd.Size = new System.Drawing.Size(406, 26);
            this.lciKAd.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKAd.Text = "Ad";
            this.lciKAd.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciKAd.TextSize = new System.Drawing.Size(100, 13);
            // 
            // lciKSoyad
            // 
            this.lciKSoyad.Control = this.txtKSoyad;
            this.lciKSoyad.Location = new System.Drawing.Point(0, 26);
            this.lciKSoyad.MaxSize = new System.Drawing.Size(0, 26);
            this.lciKSoyad.MinSize = new System.Drawing.Size(80, 26);
            this.lciKSoyad.Name = "lciKSoyad";
            this.lciKSoyad.Size = new System.Drawing.Size(406, 26);
            this.lciKSoyad.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKSoyad.Text = "Soyad";
            this.lciKSoyad.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciKSoyad.TextSize = new System.Drawing.Size(100, 13);
            // 
            // lciKTelefon
            // 
            this.lciKTelefon.Control = this.txtKTelefon;
            this.lciKTelefon.Location = new System.Drawing.Point(0, 52);
            this.lciKTelefon.MaxSize = new System.Drawing.Size(0, 26);
            this.lciKTelefon.MinSize = new System.Drawing.Size(80, 26);
            this.lciKTelefon.Name = "lciKTelefon";
            this.lciKTelefon.Size = new System.Drawing.Size(406, 26);
            this.lciKTelefon.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKTelefon.Text = "Telefon";
            this.lciKTelefon.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciKTelefon.TextSize = new System.Drawing.Size(100, 13);
            // 
            // lciKKullaniciAdi
            // 
            this.lciKKullaniciAdi.Control = this.txtKKullaniciAdi;
            this.lciKKullaniciAdi.Location = new System.Drawing.Point(0, 78);
            this.lciKKullaniciAdi.MaxSize = new System.Drawing.Size(0, 26);
            this.lciKKullaniciAdi.MinSize = new System.Drawing.Size(80, 26);
            this.lciKKullaniciAdi.Name = "lciKKullaniciAdi";
            this.lciKKullaniciAdi.Size = new System.Drawing.Size(406, 26);
            this.lciKKullaniciAdi.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKKullaniciAdi.Text = "Kullanıcı adı";
            this.lciKKullaniciAdi.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciKKullaniciAdi.TextSize = new System.Drawing.Size(100, 13);
            // 
            // lciKRol
            // 
            this.lciKRol.Control = this.cmbKRol;
            this.lciKRol.Location = new System.Drawing.Point(0, 104);
            this.lciKRol.MaxSize = new System.Drawing.Size(0, 26);
            this.lciKRol.MinSize = new System.Drawing.Size(80, 26);
            this.lciKRol.Name = "lciKRol";
            this.lciKRol.Size = new System.Drawing.Size(406, 26);
            this.lciKRol.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKRol.Text = "Rol";
            this.lciKRol.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciKRol.TextSize = new System.Drawing.Size(100, 13);
            // 
            // lciKParola
            // 
            this.lciKParola.Control = this.txtKParola;
            this.lciKParola.Location = new System.Drawing.Point(0, 130);
            this.lciKParola.MaxSize = new System.Drawing.Size(0, 26);
            this.lciKParola.MinSize = new System.Drawing.Size(80, 26);
            this.lciKParola.Name = "lciKParola";
            this.lciKParola.Size = new System.Drawing.Size(406, 26);
            this.lciKParola.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKParola.Text = "Parola";
            this.lciKParola.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciKParola.TextSize = new System.Drawing.Size(100, 13);
            // 
            // lciKParolaTekrar
            // 
            this.lciKParolaTekrar.Control = this.txtKParolaTekrar;
            this.lciKParolaTekrar.Location = new System.Drawing.Point(0, 156);
            this.lciKParolaTekrar.MaxSize = new System.Drawing.Size(0, 26);
            this.lciKParolaTekrar.MinSize = new System.Drawing.Size(80, 26);
            this.lciKParolaTekrar.Name = "lciKParolaTekrar";
            this.lciKParolaTekrar.Size = new System.Drawing.Size(406, 26);
            this.lciKParolaTekrar.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKParolaTekrar.Text = "Parola (tekrar)";
            this.lciKParolaTekrar.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciKParolaTekrar.TextSize = new System.Drawing.Size(100, 13);
            // 
            // lciKParolaNot
            // 
            this.lciKParolaNot.Control = this.lblKParolaNot;
            this.lciKParolaNot.Location = new System.Drawing.Point(0, 182);
            this.lciKParolaNot.MaxSize = new System.Drawing.Size(0, 54);
            this.lciKParolaNot.MinSize = new System.Drawing.Size(100, 54);
            this.lciKParolaNot.Name = "lciKParolaNot";
            this.lciKParolaNot.Size = new System.Drawing.Size(406, 54);
            this.lciKParolaNot.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKParolaNot.TextVisible = false;
            // 
            // lciKYeni
            // 
            this.lciKYeni.Control = this.btnKYeni;
            this.lciKYeni.Location = new System.Drawing.Point(0, 236);
            this.lciKYeni.MaxSize = new System.Drawing.Size(0, 34);
            this.lciKYeni.MinSize = new System.Drawing.Size(80, 34);
            this.lciKYeni.Name = "lciKYeni";
            this.lciKYeni.Size = new System.Drawing.Size(203, 34);
            this.lciKYeni.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKYeni.TextVisible = false;
            // 
            // lciKKaydet
            // 
            this.lciKKaydet.Control = this.btnKKaydet;
            this.lciKKaydet.Location = new System.Drawing.Point(203, 236);
            this.lciKKaydet.MaxSize = new System.Drawing.Size(0, 34);
            this.lciKKaydet.MinSize = new System.Drawing.Size(80, 34);
            this.lciKKaydet.Name = "lciKKaydet";
            this.lciKKaydet.Size = new System.Drawing.Size(203, 34);
            this.lciKKaydet.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKKaydet.TextVisible = false;
            // 
            // lciKParolaSifirla
            // 
            this.lciKParolaSifirla.Control = this.btnKParola;
            this.lciKParolaSifirla.Location = new System.Drawing.Point(0, 270);
            this.lciKParolaSifirla.MaxSize = new System.Drawing.Size(0, 34);
            this.lciKParolaSifirla.MinSize = new System.Drawing.Size(80, 34);
            this.lciKParolaSifirla.Name = "lciKParolaSifirla";
            this.lciKParolaSifirla.Size = new System.Drawing.Size(203, 34);
            this.lciKParolaSifirla.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKParolaSifirla.TextVisible = false;
            // 
            // lciKAktiflik
            // 
            this.lciKAktiflik.Control = this.btnKAktiflik;
            this.lciKAktiflik.Location = new System.Drawing.Point(203, 270);
            this.lciKAktiflik.MaxSize = new System.Drawing.Size(0, 34);
            this.lciKAktiflik.MinSize = new System.Drawing.Size(80, 34);
            this.lciKAktiflik.Name = "lciKAktiflik";
            this.lciKAktiflik.Size = new System.Drawing.Size(203, 34);
            this.lciKAktiflik.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKAktiflik.TextVisible = false;
            // 
            // lciKSil
            // 
            this.lciKSil.Control = this.btnKSil;
            this.lciKSil.Location = new System.Drawing.Point(0, 304);
            this.lciKSil.MaxSize = new System.Drawing.Size(0, 34);
            this.lciKSil.MinSize = new System.Drawing.Size(80, 34);
            this.lciKSil.Name = "lciKSil";
            this.lciKSil.Size = new System.Drawing.Size(203, 34);
            this.lciKSil.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKSil.TextVisible = false;
            // 
            // bosKSil
            // 
            this.bosKSil.Location = new System.Drawing.Point(203, 304);
            this.bosKSil.Name = "bosKSil";
            this.bosKSil.Size = new System.Drawing.Size(203, 34);
            // 
            // bosKAlt
            // 
            this.bosKAlt.Location = new System.Drawing.Point(0, 338);
            this.bosKAlt.Name = "bosKAlt";
            this.bosKAlt.Size = new System.Drawing.Size(406, 337);
            // 
            // grpKullanici
            // 
            this.grpKullanici.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciKAd,
            this.lciKSoyad,
            this.lciKTelefon,
            this.lciKKullaniciAdi,
            this.lciKRol,
            this.lciKParola,
            this.lciKParolaTekrar,
            this.lciKParolaNot,
            this.lciKYeni,
            this.lciKKaydet,
            this.lciKParolaSifirla,
            this.lciKAktiflik,
            this.lciKSil,
            this.bosKSil,
            this.bosKAlt});
            this.grpKullanici.Location = new System.Drawing.Point(788, 0);
            this.grpKullanici.Name = "grpKullanici";
            this.grpKullanici.Size = new System.Drawing.Size(430, 715);
            this.grpKullanici.Text = "Yeni kullanıcı";
            // 
            // RootKullanici
            // 
            this.RootKullanici.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.RootKullanici.GroupBordersVisible = false;
            this.RootKullanici.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.grpKullanicilar,
            this.grpKullanici});
            this.RootKullanici.Name = "RootKullanici";
            this.RootKullanici.Size = new System.Drawing.Size(1238, 735);
            this.RootKullanici.TextVisible = false;
            // 
            // layoutRol
            // 
            this.layoutRol.Controls.Add(this.lblRolBilgi);
            this.layoutRol.Controls.Add(this.gridYetkiler);
            this.layoutRol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutRol.Location = new System.Drawing.Point(0, 0);
            this.layoutRol.Name = "layoutRol";
            this.layoutRol.Root = this.RootRol;
            this.layoutRol.Size = new System.Drawing.Size(1238, 735);
            this.layoutRol.TabIndex = 0;
            // 
            // lblRolBilgi
            // 
            this.lblRolBilgi.Location = new System.Drawing.Point(12, 12);
            this.lblRolBilgi.Name = "lblRolBilgi";
            this.lblRolBilgi.AllowHtmlString = true;
            this.lblRolBilgi.Appearance.Options.UseTextOptions = true;
            this.lblRolBilgi.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblRolBilgi.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblRolBilgi.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblRolBilgi.Text = "Rollerin izinleri şartnamedeki roller tablosuna göre sabittir (K-059) ve yalnızca görüntülenir. Görme: ekranı ve kayıtları görme; Değiştirme: ekleme, güncelleme, durum değiştirme; Rapor: rapor alma ve dışa aktarma.";
            this.lblRolBilgi.Size = new System.Drawing.Size(1214, 36);
            this.lblRolBilgi.StyleController = this.layoutRol;
            this.lblRolBilgi.TabIndex = 1;
            // 
            // gridYetkiler
            // 
            this.gridYetkiler.Location = new System.Drawing.Point(12, 52);
            this.gridYetkiler.MainView = this.viewYetkiler;
            this.gridYetkiler.Name = "gridYetkiler";
            this.gridYetkiler.Size = new System.Drawing.Size(1214, 671);
            this.gridYetkiler.TabIndex = 2;
            this.gridYetkiler.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewYetkiler});
            // 
            // viewYetkiler
            // 
            this.viewYetkiler.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colYRol,
            this.colYModul,
            this.colYGorme,
            this.colYDegistirme,
            this.colYRapor});
            this.viewYetkiler.GridControl = this.gridYetkiler;
            this.viewYetkiler.Name = "viewYetkiler";
            this.viewYetkiler.OptionsBehavior.Editable = false;
            this.viewYetkiler.OptionsView.ShowGroupPanel = false;
            this.viewYetkiler.OptionsView.ShowIndicator = false;
            // 
            // colYRol
            // 
            this.colYRol.Caption = "Rol";
            this.colYRol.FieldName = "Rol";
            this.colYRol.Name = "colYRol";
            this.colYRol.Visible = true;
            this.colYRol.VisibleIndex = 0;
            this.colYRol.Width = 150;
            // 
            // colYModul
            // 
            this.colYModul.Caption = "Modül";
            this.colYModul.FieldName = "Modul";
            this.colYModul.Name = "colYModul";
            this.colYModul.Visible = true;
            this.colYModul.VisibleIndex = 1;
            this.colYModul.Width = 150;
            // 
            // colYGorme
            // 
            this.colYGorme.Caption = "Görme";
            this.colYGorme.FieldName = "Gorme";
            this.colYGorme.Name = "colYGorme";
            this.colYGorme.Visible = true;
            this.colYGorme.VisibleIndex = 2;
            this.colYGorme.Width = 70;
            // 
            // colYDegistirme
            // 
            this.colYDegistirme.Caption = "Değiştirme";
            this.colYDegistirme.FieldName = "Degistirme";
            this.colYDegistirme.Name = "colYDegistirme";
            this.colYDegistirme.Visible = true;
            this.colYDegistirme.VisibleIndex = 3;
            this.colYDegistirme.Width = 70;
            // 
            // colYRapor
            // 
            this.colYRapor.Caption = "Rapor";
            this.colYRapor.FieldName = "Rapor";
            this.colYRapor.Name = "colYRapor";
            this.colYRapor.Visible = true;
            this.colYRapor.VisibleIndex = 4;
            this.colYRapor.Width = 70;
            // 
            // lciRolBilgi
            // 
            this.lciRolBilgi.Control = this.lblRolBilgi;
            this.lciRolBilgi.Location = new System.Drawing.Point(0, 0);
            this.lciRolBilgi.MaxSize = new System.Drawing.Size(0, 40);
            this.lciRolBilgi.MinSize = new System.Drawing.Size(100, 40);
            this.lciRolBilgi.Name = "lciRolBilgi";
            this.lciRolBilgi.Size = new System.Drawing.Size(1218, 40);
            this.lciRolBilgi.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciRolBilgi.TextVisible = false;
            // 
            // lciYetkiler
            // 
            this.lciYetkiler.Control = this.gridYetkiler;
            this.lciYetkiler.Location = new System.Drawing.Point(0, 40);
            this.lciYetkiler.MinSize = new System.Drawing.Size(300, 100);
            this.lciYetkiler.Name = "lciYetkiler";
            this.lciYetkiler.Size = new System.Drawing.Size(1218, 675);
            this.lciYetkiler.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciYetkiler.TextVisible = false;
            // 
            // RootRol
            // 
            this.RootRol.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.RootRol.GroupBordersVisible = false;
            this.RootRol.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciRolBilgi,
            this.lciYetkiler});
            this.RootRol.Name = "RootRol";
            this.RootRol.Size = new System.Drawing.Size(1238, 735);
            this.RootRol.TextVisible = false;
            // 
            // layoutGerekce
            // 
            this.layoutGerekce.Controls.Add(this.lblGerekceBilgi);
            this.layoutGerekce.Controls.Add(this.gridGerekceler);
            this.layoutGerekce.Controls.Add(this.chklUrunler);
            this.layoutGerekce.Controls.Add(this.txtGAciklama);
            this.layoutGerekce.Controls.Add(this.btnGKaydet);
            this.layoutGerekce.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutGerekce.Location = new System.Drawing.Point(0, 0);
            this.layoutGerekce.Name = "layoutGerekce";
            this.layoutGerekce.Root = this.RootGerekce;
            this.layoutGerekce.Size = new System.Drawing.Size(1238, 735);
            this.layoutGerekce.TabIndex = 0;
            // 
            // lblGerekceBilgi
            // 
            this.lblGerekceBilgi.Location = new System.Drawing.Point(12, 12);
            this.lblGerekceBilgi.Name = "lblGerekceBilgi";
            this.lblGerekceBilgi.AllowHtmlString = true;
            this.lblGerekceBilgi.Appearance.Options.UseTextOptions = true;
            this.lblGerekceBilgi.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblGerekceBilgi.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblGerekceBilgi.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGerekceBilgi.Text = "Gerekçelerin destek önceliği ve yerleşim etkisi şartname tablosuna sabittir. Düzenlenebilen, gerekçenin hangi ürünlerde öncelik verdiğidir: dağıtım listesinde bu ürünlerde gerekçesi olan aileler önde sıralanır. Not: bir ürünün kategorisi değişirse eşleşmeleri kategoriye göre yeniden kurulur.";
            this.lblGerekceBilgi.Size = new System.Drawing.Size(1214, 36);
            this.lblGerekceBilgi.StyleController = this.layoutGerekce;
            this.lblGerekceBilgi.TabIndex = 1;
            // 
            // gridGerekceler
            // 
            this.gridGerekceler.Location = new System.Drawing.Point(24, 81);
            this.gridGerekceler.MainView = this.viewGerekceler;
            this.gridGerekceler.Name = "gridGerekceler";
            this.gridGerekceler.Size = new System.Drawing.Size(760, 630);
            this.gridGerekceler.TabIndex = 2;
            this.gridGerekceler.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewGerekceler});
            // 
            // viewGerekceler
            // 
            this.viewGerekceler.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colGGerekce,
            this.colGOncelik,
            this.colGEtki,
            this.colGUrunler});
            this.viewGerekceler.GridControl = this.gridGerekceler;
            this.viewGerekceler.Name = "viewGerekceler";
            this.viewGerekceler.OptionsBehavior.Editable = false;
            this.viewGerekceler.OptionsView.ShowGroupPanel = false;
            this.viewGerekceler.OptionsView.ShowIndicator = false;
            this.viewGerekceler.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.viewGerekceler_FocusedRowChanged);
            // 
            // colGGerekce
            // 
            this.colGGerekce.Caption = "Gerekçe";
            this.colGGerekce.FieldName = "Ad";
            this.colGGerekce.Name = "colGGerekce";
            this.colGGerekce.Visible = true;
            this.colGGerekce.VisibleIndex = 0;
            this.colGGerekce.Width = 160;
            // 
            // colGOncelik
            // 
            this.colGOncelik.Caption = "Destek önceliği";
            this.colGOncelik.FieldName = "Oncelik";
            this.colGOncelik.Name = "colGOncelik";
            this.colGOncelik.Visible = true;
            this.colGOncelik.VisibleIndex = 1;
            this.colGOncelik.Width = 90;
            // 
            // colGEtki
            // 
            this.colGEtki.Caption = "Yerleşim etkisi";
            this.colGEtki.FieldName = "Etki";
            this.colGEtki.Name = "colGEtki";
            this.colGEtki.Visible = true;
            this.colGEtki.VisibleIndex = 2;
            this.colGEtki.Width = 130;
            // 
            // colGUrunler
            // 
            this.colGUrunler.Caption = "Öncelikli ürünler";
            this.colGUrunler.FieldName = "Urunler";
            this.colGUrunler.Name = "colGUrunler";
            this.colGUrunler.Visible = true;
            this.colGUrunler.VisibleIndex = 3;
            this.colGUrunler.Width = 300;
            // 
            // chklUrunler
            // 
            this.chklUrunler.CheckOnClick = true;
            this.chklUrunler.Location = new System.Drawing.Point(820, 81);
            this.chklUrunler.Name = "chklUrunler";
            this.chklUrunler.Size = new System.Drawing.Size(394, 520);
            this.chklUrunler.TabIndex = 3;
            // 
            // txtGAciklama
            // 
            this.txtGAciklama.Location = new System.Drawing.Point(820, 621);
            this.txtGAciklama.Name = "txtGAciklama";
            this.txtGAciklama.Properties.MaxLength = 500;
            this.txtGAciklama.Size = new System.Drawing.Size(394, 22);
            this.txtGAciklama.StyleController = this.layoutGerekce;
            this.txtGAciklama.TabIndex = 4;
            // 
            // btnGKaydet
            // 
            this.btnGKaydet.Location = new System.Drawing.Point(808, 647);
            this.btnGKaydet.Name = "btnGKaydet";
            this.btnGKaydet.Text = "Öncelikli ürünleri kaydet";
            this.btnGKaydet.Size = new System.Drawing.Size(406, 30);
            this.btnGKaydet.StyleController = this.layoutGerekce;
            this.btnGKaydet.TabIndex = 5;
            this.btnGKaydet.Click += new System.EventHandler(this.btnGKaydet_Click);
            // 
            // lciGerekceBilgi
            // 
            this.lciGerekceBilgi.Control = this.lblGerekceBilgi;
            this.lciGerekceBilgi.Location = new System.Drawing.Point(0, 0);
            this.lciGerekceBilgi.MaxSize = new System.Drawing.Size(0, 40);
            this.lciGerekceBilgi.MinSize = new System.Drawing.Size(100, 40);
            this.lciGerekceBilgi.Name = "lciGerekceBilgi";
            this.lciGerekceBilgi.Size = new System.Drawing.Size(1218, 40);
            this.lciGerekceBilgi.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciGerekceBilgi.TextVisible = false;
            // 
            // lciGerekceler
            // 
            this.lciGerekceler.Control = this.gridGerekceler;
            this.lciGerekceler.Location = new System.Drawing.Point(0, 0);
            this.lciGerekceler.MinSize = new System.Drawing.Size(300, 100);
            this.lciGerekceler.Name = "lciGerekceler";
            this.lciGerekceler.Size = new System.Drawing.Size(764, 630);
            this.lciGerekceler.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciGerekceler.TextVisible = false;
            // 
            // grpGerekceler
            // 
            this.grpGerekceler.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciGerekceler});
            this.grpGerekceler.Location = new System.Drawing.Point(0, 40);
            this.grpGerekceler.Name = "grpGerekceler";
            this.grpGerekceler.Size = new System.Drawing.Size(788, 675);
            this.grpGerekceler.Text = "Gerekçeler";
            // 
            // lciUrunler
            // 
            this.lciUrunler.Control = this.chklUrunler;
            this.lciUrunler.Location = new System.Drawing.Point(0, 0);
            this.lciUrunler.MinSize = new System.Drawing.Size(100, 100);
            this.lciUrunler.Name = "lciUrunler";
            this.lciUrunler.Size = new System.Drawing.Size(406, 524);
            this.lciUrunler.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciUrunler.Text = "Öncelikli ürünler";
            this.lciUrunler.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciUrunler.TextSize = new System.Drawing.Size(100, 13);
            // 
            // lciGAciklama
            // 
            this.lciGAciklama.Control = this.txtGAciklama;
            this.lciGAciklama.Location = new System.Drawing.Point(0, 524);
            this.lciGAciklama.MaxSize = new System.Drawing.Size(0, 46);
            this.lciGAciklama.MinSize = new System.Drawing.Size(100, 46);
            this.lciGAciklama.Name = "lciGAciklama";
            this.lciGAciklama.Size = new System.Drawing.Size(406, 46);
            this.lciGAciklama.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciGAciklama.Text = "Değişikliğin açıklaması (zorunlu)";
            this.lciGAciklama.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciGAciklama.TextSize = new System.Drawing.Size(100, 13);
            // 
            // lciGKaydet
            // 
            this.lciGKaydet.Control = this.btnGKaydet;
            this.lciGKaydet.Location = new System.Drawing.Point(0, 570);
            this.lciGKaydet.MaxSize = new System.Drawing.Size(0, 34);
            this.lciGKaydet.MinSize = new System.Drawing.Size(80, 34);
            this.lciGKaydet.Name = "lciGKaydet";
            this.lciGKaydet.Size = new System.Drawing.Size(406, 34);
            this.lciGKaydet.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciGKaydet.TextVisible = false;
            // 
            // bosGerekce
            // 
            this.bosGerekce.Location = new System.Drawing.Point(0, 604);
            this.bosGerekce.Name = "bosGerekce";
            this.bosGerekce.Size = new System.Drawing.Size(406, 26);
            // 
            // grpEslesme
            // 
            this.grpEslesme.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciUrunler,
            this.lciGAciklama,
            this.lciGKaydet,
            this.bosGerekce});
            this.grpEslesme.Location = new System.Drawing.Point(788, 40);
            this.grpEslesme.Name = "grpEslesme";
            this.grpEslesme.Size = new System.Drawing.Size(430, 675);
            this.grpEslesme.Text = "Seçili gerekçe";
            // 
            // RootGerekce
            // 
            this.RootGerekce.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.RootGerekce.GroupBordersVisible = false;
            this.RootGerekce.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciGerekceBilgi,
            this.grpGerekceler,
            this.grpEslesme});
            this.RootGerekce.Name = "RootGerekce";
            this.RootGerekce.Size = new System.Drawing.Size(1238, 735);
            this.RootGerekce.TextVisible = false;
            // 
            // layoutDuyuru
            // 
            this.layoutDuyuru.Controls.Add(this.chkDPasifler);
            this.layoutDuyuru.Controls.Add(this.gridDuyurular);
            this.layoutDuyuru.Controls.Add(this.txtDBaslik);
            this.layoutDuyuru.Controls.Add(this.memoDMetin);
            this.layoutDuyuru.Controls.Add(this.dteDBaslangic);
            this.layoutDuyuru.Controls.Add(this.dteDBitis);
            this.layoutDuyuru.Controls.Add(this.btnDYeni);
            this.layoutDuyuru.Controls.Add(this.btnDKaydet);
            this.layoutDuyuru.Controls.Add(this.btnDAktiflik);
            this.layoutDuyuru.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutDuyuru.Location = new System.Drawing.Point(0, 0);
            this.layoutDuyuru.Name = "layoutDuyuru";
            this.layoutDuyuru.Root = this.RootDuyuru;
            this.layoutDuyuru.Size = new System.Drawing.Size(1238, 735);
            this.layoutDuyuru.TabIndex = 0;
            // 
            // chkDPasifler
            // 
            this.chkDPasifler.Location = new System.Drawing.Point(24, 45);
            this.chkDPasifler.Name = "chkDPasifler";
            this.chkDPasifler.Properties.Caption = "Pasif duyuruları da göster";
            this.chkDPasifler.Size = new System.Drawing.Size(760, 22);
            this.chkDPasifler.StyleController = this.layoutDuyuru;
            this.chkDPasifler.TabIndex = 1;
            this.chkDPasifler.CheckedChanged += new System.EventHandler(this.chkDPasifler_CheckedChanged);
            // 
            // gridDuyurular
            // 
            this.gridDuyurular.Location = new System.Drawing.Point(24, 71);
            this.gridDuyurular.MainView = this.viewDuyurular;
            this.gridDuyurular.Name = "gridDuyurular";
            this.gridDuyurular.Size = new System.Drawing.Size(760, 640);
            this.gridDuyurular.TabIndex = 2;
            this.gridDuyurular.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewDuyurular});
            // 
            // viewDuyurular
            // 
            this.viewDuyurular.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colDBaslik,
            this.colDBaslangic,
            this.colDBitis,
            this.colDDurum,
            this.colDEkleyen});
            this.viewDuyurular.GridControl = this.gridDuyurular;
            this.viewDuyurular.Name = "viewDuyurular";
            this.viewDuyurular.OptionsBehavior.Editable = false;
            this.viewDuyurular.OptionsView.ShowGroupPanel = false;
            this.viewDuyurular.OptionsView.ShowIndicator = false;
            this.viewDuyurular.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.viewDuyurular_FocusedRowChanged);
            // 
            // colDBaslik
            // 
            this.colDBaslik.Caption = "Başlık";
            this.colDBaslik.FieldName = "Baslik";
            this.colDBaslik.Name = "colDBaslik";
            this.colDBaslik.Visible = true;
            this.colDBaslik.VisibleIndex = 0;
            this.colDBaslik.Width = 220;
            // 
            // colDBaslangic
            // 
            this.colDBaslangic.Caption = "Başlangıç";
            this.colDBaslangic.FieldName = "BaslangicTarihi";
            this.colDBaslangic.Name = "colDBaslangic";
            this.colDBaslangic.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";
            this.colDBaslangic.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colDBaslangic.Visible = true;
            this.colDBaslangic.VisibleIndex = 1;
            this.colDBaslangic.Width = 110;
            // 
            // colDBitis
            // 
            this.colDBitis.Caption = "Bitiş";
            this.colDBitis.FieldName = "BitisTarihi";
            this.colDBitis.Name = "colDBitis";
            this.colDBitis.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";
            this.colDBitis.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colDBitis.Visible = true;
            this.colDBitis.VisibleIndex = 2;
            this.colDBitis.Width = 110;
            // 
            // colDDurum
            // 
            this.colDDurum.Caption = "Durum";
            this.colDDurum.FieldName = "YayinDurumu";
            this.colDDurum.Name = "colDDurum";
            this.colDDurum.Visible = true;
            this.colDDurum.VisibleIndex = 3;
            this.colDDurum.Width = 100;
            // 
            // colDEkleyen
            // 
            this.colDEkleyen.Caption = "Ekleyen";
            this.colDEkleyen.FieldName = "Olusturan";
            this.colDEkleyen.Name = "colDEkleyen";
            this.colDEkleyen.Visible = true;
            this.colDEkleyen.VisibleIndex = 4;
            this.colDEkleyen.Width = 90;
            // 
            // txtDBaslik
            // 
            this.txtDBaslik.Location = new System.Drawing.Point(900, 45);
            this.txtDBaslik.Name = "txtDBaslik";
            this.txtDBaslik.Properties.MaxLength = 150;
            this.txtDBaslik.Size = new System.Drawing.Size(314, 22);
            this.txtDBaslik.StyleController = this.layoutDuyuru;
            this.txtDBaslik.TabIndex = 3;
            // 
            // memoDMetin
            // 
            this.memoDMetin.Location = new System.Drawing.Point(900, 71);
            this.memoDMetin.Name = "memoDMetin";
            this.memoDMetin.Properties.MaxLength = 2000;
            this.memoDMetin.Size = new System.Drawing.Size(314, 200);
            this.memoDMetin.StyleController = this.layoutDuyuru;
            this.memoDMetin.TabIndex = 4;
            // 
            // dteDBaslangic
            // 
            this.dteDBaslangic.EditValue = null;
            this.dteDBaslangic.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteDBaslangic.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteDBaslangic.Location = new System.Drawing.Point(900, 275);
            this.dteDBaslangic.Name = "dteDBaslangic";
            this.dteDBaslangic.Size = new System.Drawing.Size(314, 22);
            this.dteDBaslangic.StyleController = this.layoutDuyuru;
            this.dteDBaslangic.TabIndex = 5;
            // 
            // dteDBitis
            // 
            this.dteDBitis.EditValue = null;
            this.dteDBitis.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteDBitis.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteDBitis.Location = new System.Drawing.Point(900, 301);
            this.dteDBitis.Name = "dteDBitis";
            this.dteDBitis.Size = new System.Drawing.Size(314, 22);
            this.dteDBitis.StyleController = this.layoutDuyuru;
            this.dteDBitis.TabIndex = 6;
            // 
            // btnDYeni
            // 
            this.btnDYeni.Location = new System.Drawing.Point(808, 327);
            this.btnDYeni.Name = "btnDYeni";
            this.btnDYeni.Text = "Yeni duyuru";
            this.btnDYeni.Size = new System.Drawing.Size(200, 30);
            this.btnDYeni.StyleController = this.layoutDuyuru;
            this.btnDYeni.TabIndex = 7;
            this.btnDYeni.Click += new System.EventHandler(this.btnDYeni_Click);
            // 
            // btnDKaydet
            // 
            this.btnDKaydet.Location = new System.Drawing.Point(1012, 327);
            this.btnDKaydet.Name = "btnDKaydet";
            this.btnDKaydet.Text = "Kaydet";
            this.btnDKaydet.Size = new System.Drawing.Size(202, 30);
            this.btnDKaydet.StyleController = this.layoutDuyuru;
            this.btnDKaydet.TabIndex = 8;
            this.btnDKaydet.Click += new System.EventHandler(this.btnDKaydet_Click);
            // 
            // btnDAktiflik
            // 
            this.btnDAktiflik.Location = new System.Drawing.Point(808, 361);
            this.btnDAktiflik.Name = "btnDAktiflik";
            this.btnDAktiflik.Text = "Pasif yap";
            this.btnDAktiflik.Size = new System.Drawing.Size(200, 30);
            this.btnDAktiflik.StyleController = this.layoutDuyuru;
            this.btnDAktiflik.TabIndex = 9;
            this.btnDAktiflik.Click += new System.EventHandler(this.btnDAktiflik_Click);
            // 
            // lciDPasifler
            // 
            this.lciDPasifler.Control = this.chkDPasifler;
            this.lciDPasifler.Location = new System.Drawing.Point(0, 0);
            this.lciDPasifler.MaxSize = new System.Drawing.Size(0, 26);
            this.lciDPasifler.MinSize = new System.Drawing.Size(80, 26);
            this.lciDPasifler.Name = "lciDPasifler";
            this.lciDPasifler.Size = new System.Drawing.Size(764, 26);
            this.lciDPasifler.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDPasifler.TextVisible = false;
            // 
            // lciDuyurular
            // 
            this.lciDuyurular.Control = this.gridDuyurular;
            this.lciDuyurular.Location = new System.Drawing.Point(0, 26);
            this.lciDuyurular.MinSize = new System.Drawing.Size(300, 100);
            this.lciDuyurular.Name = "lciDuyurular";
            this.lciDuyurular.Size = new System.Drawing.Size(764, 649);
            this.lciDuyurular.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDuyurular.TextVisible = false;
            // 
            // grpDuyurular
            // 
            this.grpDuyurular.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciDPasifler,
            this.lciDuyurular});
            this.grpDuyurular.Location = new System.Drawing.Point(0, 0);
            this.grpDuyurular.Name = "grpDuyurular";
            this.grpDuyurular.Size = new System.Drawing.Size(788, 715);
            this.grpDuyurular.Text = "Duyurular (kioskta yayındakiler görünür)";
            // 
            // lciDBaslik
            // 
            this.lciDBaslik.Control = this.txtDBaslik;
            this.lciDBaslik.Location = new System.Drawing.Point(0, 0);
            this.lciDBaslik.MaxSize = new System.Drawing.Size(0, 26);
            this.lciDBaslik.MinSize = new System.Drawing.Size(80, 26);
            this.lciDBaslik.Name = "lciDBaslik";
            this.lciDBaslik.Size = new System.Drawing.Size(406, 26);
            this.lciDBaslik.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDBaslik.Text = "Başlık";
            this.lciDBaslik.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDBaslik.TextSize = new System.Drawing.Size(80, 13);
            // 
            // lciDMetin
            // 
            this.lciDMetin.Control = this.memoDMetin;
            this.lciDMetin.Location = new System.Drawing.Point(0, 26);
            this.lciDMetin.MinSize = new System.Drawing.Size(100, 80);
            this.lciDMetin.Name = "lciDMetin";
            this.lciDMetin.Size = new System.Drawing.Size(406, 204);
            this.lciDMetin.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDMetin.Text = "Metin";
            this.lciDMetin.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDMetin.TextSize = new System.Drawing.Size(80, 13);
            // 
            // lciDBaslangic
            // 
            this.lciDBaslangic.Control = this.dteDBaslangic;
            this.lciDBaslangic.Location = new System.Drawing.Point(0, 230);
            this.lciDBaslangic.MaxSize = new System.Drawing.Size(0, 26);
            this.lciDBaslangic.MinSize = new System.Drawing.Size(80, 26);
            this.lciDBaslangic.Name = "lciDBaslangic";
            this.lciDBaslangic.Size = new System.Drawing.Size(406, 26);
            this.lciDBaslangic.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDBaslangic.Text = "Başlangıç";
            this.lciDBaslangic.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDBaslangic.TextSize = new System.Drawing.Size(80, 13);
            // 
            // lciDBitis
            // 
            this.lciDBitis.Control = this.dteDBitis;
            this.lciDBitis.Location = new System.Drawing.Point(0, 256);
            this.lciDBitis.MaxSize = new System.Drawing.Size(0, 26);
            this.lciDBitis.MinSize = new System.Drawing.Size(80, 26);
            this.lciDBitis.Name = "lciDBitis";
            this.lciDBitis.Size = new System.Drawing.Size(406, 26);
            this.lciDBitis.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDBitis.Text = "Bitiş";
            this.lciDBitis.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDBitis.TextSize = new System.Drawing.Size(80, 13);
            // 
            // lciDYeni
            // 
            this.lciDYeni.Control = this.btnDYeni;
            this.lciDYeni.Location = new System.Drawing.Point(0, 282);
            this.lciDYeni.MaxSize = new System.Drawing.Size(0, 34);
            this.lciDYeni.MinSize = new System.Drawing.Size(80, 34);
            this.lciDYeni.Name = "lciDYeni";
            this.lciDYeni.Size = new System.Drawing.Size(203, 34);
            this.lciDYeni.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDYeni.TextVisible = false;
            // 
            // lciDKaydet
            // 
            this.lciDKaydet.Control = this.btnDKaydet;
            this.lciDKaydet.Location = new System.Drawing.Point(203, 282);
            this.lciDKaydet.MaxSize = new System.Drawing.Size(0, 34);
            this.lciDKaydet.MinSize = new System.Drawing.Size(80, 34);
            this.lciDKaydet.Name = "lciDKaydet";
            this.lciDKaydet.Size = new System.Drawing.Size(203, 34);
            this.lciDKaydet.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDKaydet.TextVisible = false;
            // 
            // lciDAktiflik
            // 
            this.lciDAktiflik.Control = this.btnDAktiflik;
            this.lciDAktiflik.Location = new System.Drawing.Point(0, 316);
            this.lciDAktiflik.MaxSize = new System.Drawing.Size(0, 34);
            this.lciDAktiflik.MinSize = new System.Drawing.Size(80, 34);
            this.lciDAktiflik.Name = "lciDAktiflik";
            this.lciDAktiflik.Size = new System.Drawing.Size(203, 34);
            this.lciDAktiflik.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDAktiflik.TextVisible = false;
            // 
            // bosDAktiflik
            // 
            this.bosDAktiflik.Location = new System.Drawing.Point(203, 316);
            this.bosDAktiflik.Name = "bosDAktiflik";
            this.bosDAktiflik.Size = new System.Drawing.Size(203, 34);
            // 
            // bosDAlt
            // 
            this.bosDAlt.Location = new System.Drawing.Point(0, 350);
            this.bosDAlt.Name = "bosDAlt";
            this.bosDAlt.Size = new System.Drawing.Size(406, 325);
            // 
            // grpDuyuru
            // 
            this.grpDuyuru.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciDBaslik,
            this.lciDMetin,
            this.lciDBaslangic,
            this.lciDBitis,
            this.lciDYeni,
            this.lciDKaydet,
            this.lciDAktiflik,
            this.bosDAktiflik,
            this.bosDAlt});
            this.grpDuyuru.Location = new System.Drawing.Point(788, 0);
            this.grpDuyuru.Name = "grpDuyuru";
            this.grpDuyuru.Size = new System.Drawing.Size(430, 715);
            this.grpDuyuru.Text = "Yeni duyuru";
            // 
            // RootDuyuru
            // 
            this.RootDuyuru.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.RootDuyuru.GroupBordersVisible = false;
            this.RootDuyuru.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.grpDuyurular,
            this.grpDuyuru});
            this.RootDuyuru.Name = "RootDuyuru";
            this.RootDuyuru.Size = new System.Drawing.Size(1238, 735);
            this.RootDuyuru.TextVisible = false;
            // 
            // AyarlarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1240, 760);
            this.Controls.Add(this.tabControl);
            this.MinimumSize = new System.Drawing.Size(1050, 650);
            this.Name = "AyarlarForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ayarlar ve kullanıcılar";
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabKullanicilar.ResumeLayout(false);
            this.tabRoller.ResumeLayout(false);
            this.tabGerekceler.ResumeLayout(false);
            this.tabDuyurular.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutKullanici)).EndInit();
            this.layoutKullanici.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkKPasifler.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridKullanicilar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewKullanicilar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKAd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKSoyad.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKTelefon.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKKullaniciAdi.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbKRol.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKParola.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKParolaTekrar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKPasifler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKullanicilar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpKullanicilar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKAd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKSoyad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKTelefon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKKullaniciAdi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKRol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKParola)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKParolaTekrar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKParolaNot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKYeni)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKKaydet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKParolaSifirla)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKAktiflik)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKSil)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosKSil)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosKAlt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpKullanici)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootKullanici)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutRol)).EndInit();
            this.layoutRol.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridYetkiler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewYetkiler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRolBilgi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYetkiler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootRol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutGerekce)).EndInit();
            this.layoutGerekce.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridGerekceler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewGerekceler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chklUrunler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGAciklama.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGerekceBilgi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGerekceler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpGerekceler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciUrunler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGAciklama)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGKaydet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosGerekce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpEslesme)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootGerekce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDuyuru)).EndInit();
            this.layoutDuyuru.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkDPasifler.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDuyurular)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDuyurular)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDBaslik.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoDMetin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDBaslangic.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDBaslangic.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDBitis.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDBitis.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDPasifler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDuyurular)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpDuyurular)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDBaslik)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDMetin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDBaslangic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDBitis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDYeni)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDKaydet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDAktiflik)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosDAktiflik)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosDAlt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpDuyuru)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RootDuyuru)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl tabControl;
        private DevExpress.XtraTab.XtraTabPage tabKullanicilar;
        private DevExpress.XtraTab.XtraTabPage tabRoller;
        private DevExpress.XtraTab.XtraTabPage tabGerekceler;
        private DevExpress.XtraTab.XtraTabPage tabDuyurular;
        private DevExpress.XtraLayout.LayoutControl layoutKullanici;
        private DevExpress.XtraEditors.CheckEdit chkKPasifler;
        private DevExpress.XtraGrid.GridControl gridKullanicilar;
        private DevExpress.XtraGrid.Views.Grid.GridView viewKullanicilar;
        private DevExpress.XtraGrid.Columns.GridColumn colKAd;
        private DevExpress.XtraGrid.Columns.GridColumn colKKullanici;
        private DevExpress.XtraGrid.Columns.GridColumn colKRol;
        private DevExpress.XtraGrid.Columns.GridColumn colKTelefon;
        private DevExpress.XtraGrid.Columns.GridColumn colKDurum;
        private DevExpress.XtraGrid.Columns.GridColumn colKSonGiris;
        private DevExpress.XtraEditors.TextEdit txtKAd;
        private DevExpress.XtraEditors.TextEdit txtKSoyad;
        private DevExpress.XtraEditors.TextEdit txtKTelefon;
        private DevExpress.XtraEditors.TextEdit txtKKullaniciAdi;
        private DevExpress.XtraEditors.ImageComboBoxEdit cmbKRol;
        private DevExpress.XtraEditors.TextEdit txtKParola;
        private DevExpress.XtraEditors.TextEdit txtKParolaTekrar;
        private DevExpress.XtraEditors.LabelControl lblKParolaNot;
        private DevExpress.XtraEditors.SimpleButton btnKYeni;
        private DevExpress.XtraEditors.SimpleButton btnKKaydet;
        private DevExpress.XtraEditors.SimpleButton btnKParola;
        private DevExpress.XtraEditors.SimpleButton btnKAktiflik;
        private DevExpress.XtraEditors.SimpleButton btnKSil;
        private DevExpress.XtraLayout.LayoutControlItem lciKPasifler;
        private DevExpress.XtraLayout.LayoutControlItem lciKullanicilar;
        private DevExpress.XtraLayout.LayoutControlGroup grpKullanicilar;
        private DevExpress.XtraLayout.LayoutControlItem lciKAd;
        private DevExpress.XtraLayout.LayoutControlItem lciKSoyad;
        private DevExpress.XtraLayout.LayoutControlItem lciKTelefon;
        private DevExpress.XtraLayout.LayoutControlItem lciKKullaniciAdi;
        private DevExpress.XtraLayout.LayoutControlItem lciKRol;
        private DevExpress.XtraLayout.LayoutControlItem lciKParola;
        private DevExpress.XtraLayout.LayoutControlItem lciKParolaTekrar;
        private DevExpress.XtraLayout.LayoutControlItem lciKParolaNot;
        private DevExpress.XtraLayout.LayoutControlItem lciKYeni;
        private DevExpress.XtraLayout.LayoutControlItem lciKKaydet;
        private DevExpress.XtraLayout.LayoutControlItem lciKParolaSifirla;
        private DevExpress.XtraLayout.LayoutControlItem lciKAktiflik;
        private DevExpress.XtraLayout.LayoutControlItem lciKSil;
        private DevExpress.XtraLayout.EmptySpaceItem bosKSil;
        private DevExpress.XtraLayout.EmptySpaceItem bosKAlt;
        private DevExpress.XtraLayout.LayoutControlGroup grpKullanici;
        private DevExpress.XtraLayout.LayoutControlGroup RootKullanici;
        private DevExpress.XtraLayout.LayoutControl layoutRol;
        private DevExpress.XtraEditors.LabelControl lblRolBilgi;
        private DevExpress.XtraGrid.GridControl gridYetkiler;
        private DevExpress.XtraGrid.Views.Grid.GridView viewYetkiler;
        private DevExpress.XtraGrid.Columns.GridColumn colYRol;
        private DevExpress.XtraGrid.Columns.GridColumn colYModul;
        private DevExpress.XtraGrid.Columns.GridColumn colYGorme;
        private DevExpress.XtraGrid.Columns.GridColumn colYDegistirme;
        private DevExpress.XtraGrid.Columns.GridColumn colYRapor;
        private DevExpress.XtraLayout.LayoutControlItem lciRolBilgi;
        private DevExpress.XtraLayout.LayoutControlItem lciYetkiler;
        private DevExpress.XtraLayout.LayoutControlGroup RootRol;
        private DevExpress.XtraLayout.LayoutControl layoutGerekce;
        private DevExpress.XtraEditors.LabelControl lblGerekceBilgi;
        private DevExpress.XtraGrid.GridControl gridGerekceler;
        private DevExpress.XtraGrid.Views.Grid.GridView viewGerekceler;
        private DevExpress.XtraGrid.Columns.GridColumn colGGerekce;
        private DevExpress.XtraGrid.Columns.GridColumn colGOncelik;
        private DevExpress.XtraGrid.Columns.GridColumn colGEtki;
        private DevExpress.XtraGrid.Columns.GridColumn colGUrunler;
        private DevExpress.XtraEditors.CheckedListBoxControl chklUrunler;
        private DevExpress.XtraEditors.TextEdit txtGAciklama;
        private DevExpress.XtraEditors.SimpleButton btnGKaydet;
        private DevExpress.XtraLayout.LayoutControlItem lciGerekceBilgi;
        private DevExpress.XtraLayout.LayoutControlItem lciGerekceler;
        private DevExpress.XtraLayout.LayoutControlGroup grpGerekceler;
        private DevExpress.XtraLayout.LayoutControlItem lciUrunler;
        private DevExpress.XtraLayout.LayoutControlItem lciGAciklama;
        private DevExpress.XtraLayout.LayoutControlItem lciGKaydet;
        private DevExpress.XtraLayout.EmptySpaceItem bosGerekce;
        private DevExpress.XtraLayout.LayoutControlGroup grpEslesme;
        private DevExpress.XtraLayout.LayoutControlGroup RootGerekce;
        private DevExpress.XtraLayout.LayoutControl layoutDuyuru;
        private DevExpress.XtraEditors.CheckEdit chkDPasifler;
        private DevExpress.XtraGrid.GridControl gridDuyurular;
        private DevExpress.XtraGrid.Views.Grid.GridView viewDuyurular;
        private DevExpress.XtraGrid.Columns.GridColumn colDBaslik;
        private DevExpress.XtraGrid.Columns.GridColumn colDBaslangic;
        private DevExpress.XtraGrid.Columns.GridColumn colDBitis;
        private DevExpress.XtraGrid.Columns.GridColumn colDDurum;
        private DevExpress.XtraGrid.Columns.GridColumn colDEkleyen;
        private DevExpress.XtraEditors.TextEdit txtDBaslik;
        private DevExpress.XtraEditors.MemoEdit memoDMetin;
        private DevExpress.XtraEditors.DateEdit dteDBaslangic;
        private DevExpress.XtraEditors.DateEdit dteDBitis;
        private DevExpress.XtraEditors.SimpleButton btnDYeni;
        private DevExpress.XtraEditors.SimpleButton btnDKaydet;
        private DevExpress.XtraEditors.SimpleButton btnDAktiflik;
        private DevExpress.XtraLayout.LayoutControlItem lciDPasifler;
        private DevExpress.XtraLayout.LayoutControlItem lciDuyurular;
        private DevExpress.XtraLayout.LayoutControlGroup grpDuyurular;
        private DevExpress.XtraLayout.LayoutControlItem lciDBaslik;
        private DevExpress.XtraLayout.LayoutControlItem lciDMetin;
        private DevExpress.XtraLayout.LayoutControlItem lciDBaslangic;
        private DevExpress.XtraLayout.LayoutControlItem lciDBitis;
        private DevExpress.XtraLayout.LayoutControlItem lciDYeni;
        private DevExpress.XtraLayout.LayoutControlItem lciDKaydet;
        private DevExpress.XtraLayout.LayoutControlItem lciDAktiflik;
        private DevExpress.XtraLayout.EmptySpaceItem bosDAktiflik;
        private DevExpress.XtraLayout.EmptySpaceItem bosDAlt;
        private DevExpress.XtraLayout.LayoutControlGroup grpDuyuru;
        private DevExpress.XtraLayout.LayoutControlGroup RootDuyuru;
    }
}
