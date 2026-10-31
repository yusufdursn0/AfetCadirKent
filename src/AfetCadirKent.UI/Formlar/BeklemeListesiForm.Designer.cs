namespace AfetCadirKent.UI.Formlar
{
    partial class BeklemeListesiForm
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
            this.btnListeyeEkle = new DevExpress.XtraEditors.SimpleButton();
            this.btnYenile = new DevExpress.XtraEditors.SimpleButton();
            this.gridBekleyenler = new DevExpress.XtraGrid.GridControl();
            this.viewBekleyenler = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colSira = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAileNo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAileAdi = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colKisi = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOncelik = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGerekceler = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEklenme = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSure = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAciklama = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnOner = new DevExpress.XtraEditors.SimpleButton();
            this.btnListedenCikar = new DevExpress.XtraEditors.SimpleButton();
            this.gridYerAcilan = new DevExpress.XtraGrid.GridControl();
            this.viewYerAcilan = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colYCadir = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colYBosYatak = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colYAile = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colYKisi = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colYOncelik = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colYEklenme = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnYerAcilanYerlestir = new DevExpress.XtraEditors.SimpleButton();
            this.oneriPaneli = new AfetCadirKent.UI.Formlar.YerlesimOneriPaneli();
            this.lciBilgi = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciListeyeEkle = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciYenile = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBekleyenler = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciOner = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciListedenCikar = new DevExpress.XtraLayout.LayoutControlItem();
            this.bosBekleyenDugmeler = new DevExpress.XtraLayout.EmptySpaceItem();
            this.grpBekleyenler = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciYerAcilan = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciYerAcilanYerlestir = new DevExpress.XtraLayout.LayoutControlItem();
            this.bosYerAcilan = new DevExpress.XtraLayout.EmptySpaceItem();
            this.grpYerAcilan = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciOneriPaneli = new DevExpress.XtraLayout.LayoutControlItem();
            this.grpOneri = new DevExpress.XtraLayout.LayoutControlGroup();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
            this.layoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridBekleyenler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewBekleyenler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridYerAcilan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewYerAcilan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBilgi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciListeyeEkle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYenile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBekleyenler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciListedenCikar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosBekleyenDugmeler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpBekleyenler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYerAcilan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYerAcilanYerlestir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosYerAcilan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpYerAcilan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOneriPaneli)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpOneri)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl
            // 
            this.layoutControl.Controls.Add(this.lblBilgi);
            this.layoutControl.Controls.Add(this.btnListeyeEkle);
            this.layoutControl.Controls.Add(this.btnYenile);
            this.layoutControl.Controls.Add(this.gridBekleyenler);
            this.layoutControl.Controls.Add(this.btnOner);
            this.layoutControl.Controls.Add(this.btnListedenCikar);
            this.layoutControl.Controls.Add(this.gridYerAcilan);
            this.layoutControl.Controls.Add(this.btnYerAcilanYerlestir);
            this.layoutControl.Controls.Add(this.oneriPaneli);
            this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl.Location = new System.Drawing.Point(0, 0);
            this.layoutControl.Name = "layoutControl";
            this.layoutControl.Root = this.Root;
            this.layoutControl.Size = new System.Drawing.Size(1240, 760);
            this.layoutControl.TabIndex = 0;
            // 
            // lblBilgi
            // 
            this.lblBilgi.Location = new System.Drawing.Point(12, 12);
            this.lblBilgi.Name = "lblBilgi";
            this.lblBilgi.AllowHtmlString = true;
            this.lblBilgi.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblBilgi.Text = "Yer bulunamayan aileler burada bekler. Bir çadırda yer açılınca sistem o çadıra sığan aileyi önerir; görevli onaylayınca aile yerleşir ve listeden çıkar.";
            this.lblBilgi.Size = new System.Drawing.Size(856, 22);
            this.lblBilgi.StyleController = this.layoutControl;
            this.lblBilgi.TabIndex = 1;
            // 
            // btnListeyeEkle
            // 
            this.btnListeyeEkle.Location = new System.Drawing.Point(872, 12);
            this.btnListeyeEkle.Name = "btnListeyeEkle";
            this.btnListeyeEkle.Text = "Aileyi listeye al";
            this.btnListeyeEkle.Size = new System.Drawing.Size(196, 22);
            this.btnListeyeEkle.StyleController = this.layoutControl;
            this.btnListeyeEkle.TabIndex = 2;
            this.btnListeyeEkle.Click += new System.EventHandler(this.btnListeyeEkle_Click);
            // 
            // btnYenile
            // 
            this.btnYenile.Location = new System.Drawing.Point(1072, 12);
            this.btnYenile.Name = "btnYenile";
            this.btnYenile.Text = "Yenile";
            this.btnYenile.Size = new System.Drawing.Size(156, 22);
            this.btnYenile.StyleController = this.layoutControl;
            this.btnYenile.TabIndex = 3;
            this.btnYenile.Click += new System.EventHandler(this.btnYenile_Click);
            // 
            // gridBekleyenler
            // 
            this.gridBekleyenler.Location = new System.Drawing.Point(24, 59);
            this.gridBekleyenler.MainView = this.viewBekleyenler;
            this.gridBekleyenler.Name = "gridBekleyenler";
            this.gridBekleyenler.Size = new System.Drawing.Size(568, 615);
            this.gridBekleyenler.TabIndex = 4;
            this.gridBekleyenler.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewBekleyenler});
            // 
            // viewBekleyenler
            // 
            this.viewBekleyenler.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colSira,
            this.colAileNo,
            this.colAileAdi,
            this.colKisi,
            this.colOncelik,
            this.colGerekceler,
            this.colEklenme,
            this.colSure,
            this.colAciklama});
            this.viewBekleyenler.GridControl = this.gridBekleyenler;
            this.viewBekleyenler.Name = "viewBekleyenler";
            this.viewBekleyenler.OptionsBehavior.Editable = false;
            this.viewBekleyenler.OptionsView.ShowGroupPanel = false;
            this.viewBekleyenler.OptionsView.ShowIndicator = false;
            this.viewBekleyenler.OptionsView.ColumnAutoWidth = false;
            this.viewBekleyenler.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.viewBekleyenler_FocusedRowChanged);
            this.viewBekleyenler.DoubleClick += new System.EventHandler(this.viewBekleyenler_DoubleClick);
            // 
            // colSira
            // 
            this.colSira.Caption = "Sıra";
            this.colSira.FieldName = "Sira";
            this.colSira.Name = "colSira";
            this.colSira.Visible = true;
            this.colSira.VisibleIndex = 0;
            this.colSira.Width = 36;
            // 
            // colAileNo
            // 
            this.colAileNo.Caption = "Aile no";
            this.colAileNo.FieldName = "AileNo";
            this.colAileNo.Name = "colAileNo";
            this.colAileNo.Visible = true;
            this.colAileNo.VisibleIndex = 1;
            this.colAileNo.Width = 70;
            // 
            // colAileAdi
            // 
            this.colAileAdi.Caption = "Aile";
            this.colAileAdi.FieldName = "AileAdi";
            this.colAileAdi.Name = "colAileAdi";
            this.colAileAdi.Visible = true;
            this.colAileAdi.VisibleIndex = 2;
            this.colAileAdi.Width = 110;
            // 
            // colKisi
            // 
            this.colKisi.Caption = "Kişi";
            this.colKisi.FieldName = "KisiSayisi";
            this.colKisi.Name = "colKisi";
            this.colKisi.Visible = true;
            this.colKisi.VisibleIndex = 3;
            this.colKisi.Width = 40;
            // 
            // colOncelik
            // 
            this.colOncelik.Caption = "Yardım önceliği";
            this.colOncelik.FieldName = "Oncelik";
            this.colOncelik.Name = "colOncelik";
            this.colOncelik.Visible = true;
            this.colOncelik.VisibleIndex = 4;
            this.colOncelik.Width = 80;
            // 
            // colGerekceler
            // 
            this.colGerekceler.Caption = "Gerekçeler";
            this.colGerekceler.FieldName = "Gerekceler";
            this.colGerekceler.Name = "colGerekceler";
            this.colGerekceler.Visible = true;
            this.colGerekceler.VisibleIndex = 5;
            this.colGerekceler.Width = 150;
            // 
            // colEklenme
            // 
            this.colEklenme.Caption = "Listeye alınma";
            this.colEklenme.FieldName = "EklenmeZamani";
            this.colEklenme.Name = "colEklenme";
            this.colEklenme.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";
            this.colEklenme.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colEklenme.Visible = true;
            this.colEklenme.VisibleIndex = 6;
            this.colEklenme.Width = 100;
            // 
            // colSure
            // 
            this.colSure.Caption = "Bekleme süresi";
            this.colSure.FieldName = "BeklemeSuresi";
            this.colSure.Name = "colSure";
            this.colSure.Visible = true;
            this.colSure.VisibleIndex = 7;
            this.colSure.Width = 80;
            // 
            // colAciklama
            // 
            this.colAciklama.Caption = "Açıklama";
            this.colAciklama.FieldName = "Aciklama";
            this.colAciklama.Name = "colAciklama";
            this.colAciklama.Visible = true;
            this.colAciklama.VisibleIndex = 8;
            this.colAciklama.Width = 120;
            // 
            // btnOner
            // 
            this.btnOner.Location = new System.Drawing.Point(24, 678);
            this.btnOner.Name = "btnOner";
            this.btnOner.Text = "Seçili aileye çadır öner";
            this.btnOner.Size = new System.Drawing.Size(240, 30);
            this.btnOner.StyleController = this.layoutControl;
            this.btnOner.TabIndex = 5;
            this.btnOner.Click += new System.EventHandler(this.btnOner_Click);
            // 
            // btnListedenCikar
            // 
            this.btnListedenCikar.Location = new System.Drawing.Point(268, 678);
            this.btnListedenCikar.Name = "btnListedenCikar";
            this.btnListedenCikar.Text = "Listeden çıkar";
            this.btnListedenCikar.Size = new System.Drawing.Size(180, 30);
            this.btnListedenCikar.StyleController = this.layoutControl;
            this.btnListedenCikar.TabIndex = 6;
            this.btnListedenCikar.Click += new System.EventHandler(this.btnListedenCikar_Click);
            // 
            // gridYerAcilan
            // 
            this.gridYerAcilan.Location = new System.Drawing.Point(628, 59);
            this.gridYerAcilan.MainView = this.viewYerAcilan;
            this.gridYerAcilan.Name = "gridYerAcilan";
            this.gridYerAcilan.Size = new System.Drawing.Size(588, 146);
            this.gridYerAcilan.TabIndex = 7;
            this.gridYerAcilan.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewYerAcilan});
            // 
            // viewYerAcilan
            // 
            this.viewYerAcilan.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colYCadir,
            this.colYBosYatak,
            this.colYAile,
            this.colYKisi,
            this.colYOncelik,
            this.colYEklenme});
            this.viewYerAcilan.GridControl = this.gridYerAcilan;
            this.viewYerAcilan.Name = "viewYerAcilan";
            this.viewYerAcilan.OptionsBehavior.Editable = false;
            this.viewYerAcilan.OptionsView.ShowGroupPanel = false;
            this.viewYerAcilan.OptionsView.ShowIndicator = false;
            this.viewYerAcilan.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.viewYerAcilan_FocusedRowChanged);
            // 
            // colYCadir
            // 
            this.colYCadir.Caption = "Çadır";
            this.colYCadir.FieldName = "Cadir";
            this.colYCadir.Name = "colYCadir";
            this.colYCadir.Visible = true;
            this.colYCadir.VisibleIndex = 0;
            this.colYCadir.Width = 60;
            // 
            // colYBosYatak
            // 
            this.colYBosYatak.Caption = "Boş yatak";
            this.colYBosYatak.FieldName = "BosYatak";
            this.colYBosYatak.Name = "colYBosYatak";
            this.colYBosYatak.Visible = true;
            this.colYBosYatak.VisibleIndex = 1;
            this.colYBosYatak.Width = 60;
            // 
            // colYAile
            // 
            this.colYAile.Caption = "Önerilen aile";
            this.colYAile.FieldName = "Aile";
            this.colYAile.Name = "colYAile";
            this.colYAile.Visible = true;
            this.colYAile.VisibleIndex = 2;
            this.colYAile.Width = 190;
            // 
            // colYKisi
            // 
            this.colYKisi.Caption = "Kişi";
            this.colYKisi.FieldName = "KisiSayisi";
            this.colYKisi.Name = "colYKisi";
            this.colYKisi.Visible = true;
            this.colYKisi.VisibleIndex = 3;
            this.colYKisi.Width = 40;
            // 
            // colYOncelik
            // 
            this.colYOncelik.Caption = "Yardım önceliği";
            this.colYOncelik.FieldName = "Oncelik";
            this.colYOncelik.Name = "colYOncelik";
            this.colYOncelik.Visible = true;
            this.colYOncelik.VisibleIndex = 4;
            this.colYOncelik.Width = 90;
            // 
            // colYEklenme
            // 
            this.colYEklenme.Caption = "Bekliyor (başlangıç)";
            this.colYEklenme.FieldName = "EklenmeZamani";
            this.colYEklenme.Name = "colYEklenme";
            this.colYEklenme.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";
            this.colYEklenme.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colYEklenme.Visible = true;
            this.colYEklenme.VisibleIndex = 5;
            this.colYEklenme.Width = 110;
            // 
            // btnYerAcilanYerlestir
            // 
            this.btnYerAcilanYerlestir.Location = new System.Drawing.Point(628, 209);
            this.btnYerAcilanYerlestir.Name = "btnYerAcilanYerlestir";
            this.btnYerAcilanYerlestir.Text = "Önerilen aileyi bu çadıra yerleştir";
            this.btnYerAcilanYerlestir.Size = new System.Drawing.Size(300, 30);
            this.btnYerAcilanYerlestir.StyleController = this.layoutControl;
            this.btnYerAcilanYerlestir.TabIndex = 8;
            this.btnYerAcilanYerlestir.Click += new System.EventHandler(this.btnYerAcilanYerlestir_Click);
            // 
            // oneriPaneli
            // 
            this.oneriPaneli.Location = new System.Drawing.Point(628, 280);
            this.oneriPaneli.Name = "oneriPaneli";
            this.oneriPaneli.Size = new System.Drawing.Size(588, 428);
            this.oneriPaneli.TabIndex = 9;
            // 
            // lciBilgi
            // 
            this.lciBilgi.Control = this.lblBilgi;
            this.lciBilgi.Location = new System.Drawing.Point(0, 0);
            this.lciBilgi.MaxSize = new System.Drawing.Size(0, 26);
            this.lciBilgi.MinSize = new System.Drawing.Size(200, 26);
            this.lciBilgi.Name = "lciBilgi";
            this.lciBilgi.Size = new System.Drawing.Size(860, 26);
            this.lciBilgi.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBilgi.TextVisible = false;
            // 
            // lciListeyeEkle
            // 
            this.lciListeyeEkle.Control = this.btnListeyeEkle;
            this.lciListeyeEkle.Location = new System.Drawing.Point(860, 0);
            this.lciListeyeEkle.MaxSize = new System.Drawing.Size(200, 26);
            this.lciListeyeEkle.MinSize = new System.Drawing.Size(80, 26);
            this.lciListeyeEkle.Name = "lciListeyeEkle";
            this.lciListeyeEkle.Size = new System.Drawing.Size(200, 26);
            this.lciListeyeEkle.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciListeyeEkle.TextVisible = false;
            // 
            // lciYenile
            // 
            this.lciYenile.Control = this.btnYenile;
            this.lciYenile.Location = new System.Drawing.Point(1060, 0);
            this.lciYenile.MaxSize = new System.Drawing.Size(160, 26);
            this.lciYenile.MinSize = new System.Drawing.Size(80, 26);
            this.lciYenile.Name = "lciYenile";
            this.lciYenile.Size = new System.Drawing.Size(160, 26);
            this.lciYenile.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciYenile.TextVisible = false;
            // 
            // lciBekleyenler
            // 
            this.lciBekleyenler.Control = this.gridBekleyenler;
            this.lciBekleyenler.Location = new System.Drawing.Point(0, 0);
            this.lciBekleyenler.MinSize = new System.Drawing.Size(300, 150);
            this.lciBekleyenler.Name = "lciBekleyenler";
            this.lciBekleyenler.Size = new System.Drawing.Size(572, 619);
            this.lciBekleyenler.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBekleyenler.TextVisible = false;
            // 
            // lciOner
            // 
            this.lciOner.Control = this.btnOner;
            this.lciOner.Location = new System.Drawing.Point(0, 619);
            this.lciOner.MaxSize = new System.Drawing.Size(0, 34);
            this.lciOner.MinSize = new System.Drawing.Size(80, 34);
            this.lciOner.Name = "lciOner";
            this.lciOner.Size = new System.Drawing.Size(244, 34);
            this.lciOner.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciOner.TextVisible = false;
            // 
            // lciListedenCikar
            // 
            this.lciListedenCikar.Control = this.btnListedenCikar;
            this.lciListedenCikar.Location = new System.Drawing.Point(244, 619);
            this.lciListedenCikar.MaxSize = new System.Drawing.Size(0, 34);
            this.lciListedenCikar.MinSize = new System.Drawing.Size(80, 34);
            this.lciListedenCikar.Name = "lciListedenCikar";
            this.lciListedenCikar.Size = new System.Drawing.Size(184, 34);
            this.lciListedenCikar.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciListedenCikar.TextVisible = false;
            // 
            // bosBekleyenDugmeler
            // 
            this.bosBekleyenDugmeler.Location = new System.Drawing.Point(428, 619);
            this.bosBekleyenDugmeler.Name = "bosBekleyenDugmeler";
            this.bosBekleyenDugmeler.Size = new System.Drawing.Size(144, 34);
            // 
            // grpBekleyenler
            // 
            this.grpBekleyenler.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBekleyenler,
            this.lciOner,
            this.lciListedenCikar,
            this.bosBekleyenDugmeler});
            this.grpBekleyenler.Location = new System.Drawing.Point(0, 26);
            this.grpBekleyenler.Name = "grpBekleyenler";
            this.grpBekleyenler.Size = new System.Drawing.Size(596, 714);
            this.grpBekleyenler.Text = "Bekleme listesi";
            // 
            // lciYerAcilan
            // 
            this.lciYerAcilan.Control = this.gridYerAcilan;
            this.lciYerAcilan.Location = new System.Drawing.Point(0, 0);
            this.lciYerAcilan.MinSize = new System.Drawing.Size(200, 90);
            this.lciYerAcilan.Name = "lciYerAcilan";
            this.lciYerAcilan.Size = new System.Drawing.Size(592, 150);
            this.lciYerAcilan.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciYerAcilan.TextVisible = false;
            // 
            // lciYerAcilanYerlestir
            // 
            this.lciYerAcilanYerlestir.Control = this.btnYerAcilanYerlestir;
            this.lciYerAcilanYerlestir.Location = new System.Drawing.Point(0, 150);
            this.lciYerAcilanYerlestir.MaxSize = new System.Drawing.Size(0, 34);
            this.lciYerAcilanYerlestir.MinSize = new System.Drawing.Size(80, 34);
            this.lciYerAcilanYerlestir.Name = "lciYerAcilanYerlestir";
            this.lciYerAcilanYerlestir.Size = new System.Drawing.Size(304, 34);
            this.lciYerAcilanYerlestir.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciYerAcilanYerlestir.TextVisible = false;
            // 
            // bosYerAcilan
            // 
            this.bosYerAcilan.Location = new System.Drawing.Point(304, 150);
            this.bosYerAcilan.Name = "bosYerAcilan";
            this.bosYerAcilan.Size = new System.Drawing.Size(288, 34);
            // 
            // grpYerAcilan
            // 
            this.grpYerAcilan.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciYerAcilan,
            this.lciYerAcilanYerlestir,
            this.bosYerAcilan});
            this.grpYerAcilan.Location = new System.Drawing.Point(596, 26);
            this.grpYerAcilan.Name = "grpYerAcilan";
            this.grpYerAcilan.Size = new System.Drawing.Size(624, 229);
            this.grpYerAcilan.Text = "Yer açılan çadırlar ve sistemin önerdiği aile";
            // 
            // lciOneriPaneli
            // 
            this.lciOneriPaneli.Control = this.oneriPaneli;
            this.lciOneriPaneli.Location = new System.Drawing.Point(0, 0);
            this.lciOneriPaneli.MinSize = new System.Drawing.Size(300, 250);
            this.lciOneriPaneli.Name = "lciOneriPaneli";
            this.lciOneriPaneli.Size = new System.Drawing.Size(600, 440);
            this.lciOneriPaneli.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciOneriPaneli.TextVisible = false;
            // 
            // grpOneri
            // 
            this.grpOneri.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciOneriPaneli});
            this.grpOneri.Location = new System.Drawing.Point(596, 255);
            this.grpOneri.Name = "grpOneri";
            this.grpOneri.Size = new System.Drawing.Size(624, 485);
            this.grpOneri.Text = "Seçili aile için çadır önerisi (bir veya iki çadır)";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBilgi,
            this.lciListeyeEkle,
            this.lciYenile,
            this.grpBekleyenler,
            this.grpYerAcilan,
            this.grpOneri});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1240, 760);
            this.Root.TextVisible = false;
            // 
            // BeklemeListesiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1240, 760);
            this.Controls.Add(this.layoutControl);
            this.MinimumSize = new System.Drawing.Size(1000, 680);
            this.Name = "BeklemeListesiForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bekleme listesi";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
            this.layoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridBekleyenler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewBekleyenler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridYerAcilan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewYerAcilan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBilgi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciListeyeEkle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYenile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBekleyenler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciListedenCikar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosBekleyenDugmeler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpBekleyenler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYerAcilan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciYerAcilanYerlestir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bosYerAcilan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpYerAcilan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOneriPaneli)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpOneri)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl;
        private DevExpress.XtraEditors.LabelControl lblBilgi;
        private DevExpress.XtraEditors.SimpleButton btnListeyeEkle;
        private DevExpress.XtraEditors.SimpleButton btnYenile;
        private DevExpress.XtraGrid.GridControl gridBekleyenler;
        private DevExpress.XtraGrid.Views.Grid.GridView viewBekleyenler;
        private DevExpress.XtraGrid.Columns.GridColumn colSira;
        private DevExpress.XtraGrid.Columns.GridColumn colAileNo;
        private DevExpress.XtraGrid.Columns.GridColumn colAileAdi;
        private DevExpress.XtraGrid.Columns.GridColumn colKisi;
        private DevExpress.XtraGrid.Columns.GridColumn colOncelik;
        private DevExpress.XtraGrid.Columns.GridColumn colGerekceler;
        private DevExpress.XtraGrid.Columns.GridColumn colEklenme;
        private DevExpress.XtraGrid.Columns.GridColumn colSure;
        private DevExpress.XtraGrid.Columns.GridColumn colAciklama;
        private DevExpress.XtraEditors.SimpleButton btnOner;
        private DevExpress.XtraEditors.SimpleButton btnListedenCikar;
        private DevExpress.XtraGrid.GridControl gridYerAcilan;
        private DevExpress.XtraGrid.Views.Grid.GridView viewYerAcilan;
        private DevExpress.XtraGrid.Columns.GridColumn colYCadir;
        private DevExpress.XtraGrid.Columns.GridColumn colYBosYatak;
        private DevExpress.XtraGrid.Columns.GridColumn colYAile;
        private DevExpress.XtraGrid.Columns.GridColumn colYKisi;
        private DevExpress.XtraGrid.Columns.GridColumn colYOncelik;
        private DevExpress.XtraGrid.Columns.GridColumn colYEklenme;
        private DevExpress.XtraEditors.SimpleButton btnYerAcilanYerlestir;
        private AfetCadirKent.UI.Formlar.YerlesimOneriPaneli oneriPaneli;
        private DevExpress.XtraLayout.LayoutControlItem lciBilgi;
        private DevExpress.XtraLayout.LayoutControlItem lciListeyeEkle;
        private DevExpress.XtraLayout.LayoutControlItem lciYenile;
        private DevExpress.XtraLayout.LayoutControlItem lciBekleyenler;
        private DevExpress.XtraLayout.LayoutControlItem lciOner;
        private DevExpress.XtraLayout.LayoutControlItem lciListedenCikar;
        private DevExpress.XtraLayout.EmptySpaceItem bosBekleyenDugmeler;
        private DevExpress.XtraLayout.LayoutControlGroup grpBekleyenler;
        private DevExpress.XtraLayout.LayoutControlItem lciYerAcilan;
        private DevExpress.XtraLayout.LayoutControlItem lciYerAcilanYerlestir;
        private DevExpress.XtraLayout.EmptySpaceItem bosYerAcilan;
        private DevExpress.XtraLayout.LayoutControlGroup grpYerAcilan;
        private DevExpress.XtraLayout.LayoutControlItem lciOneriPaneli;
        private DevExpress.XtraLayout.LayoutControlGroup grpOneri;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
    }
}
