namespace AutoPK
{
    partial class FrmAutoLogin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAutoLogin));
            this.TmrUpdatehWnd = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.Txbduongdan = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CobServer = new System.Windows.Forms.ComboBox();
            this.BtnHuy = new System.Windows.Forms.Button();
            this.BtnXoa = new System.Windows.Forms.Button();
            this.BtnSua = new System.Windows.Forms.Button();
            this.BtnThem = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.CobSTTNV = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Txbmatkhau = new System.Windows.Forms.TextBox();
            this.Txbtaikhoan = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxAcount = new System.Windows.Forms.CheckBox();
            this.BtnLuu = new System.Windows.Forms.Button();
            this.BtnDangNhap = new System.Windows.Forms.Button();
            this.Tmrchonnv = new System.Windows.Forms.Timer(this.components);
            this.lsvAcout = new AutoPK.ListViewsOther();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox6.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // TmrUpdatehWnd
            // 
            this.TmrUpdatehWnd.Enabled = true;
            this.TmrUpdatehWnd.Interval = 500;
            this.TmrUpdatehWnd.Tick += new System.EventHandler(this.TmrUpdatehWnd_Tick);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.button1.Location = new System.Drawing.Point(6, 20);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(44, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Tìm";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Btntim_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.Txbduongdan);
            this.groupBox6.Controls.Add(this.button1);
            this.groupBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox6.ForeColor = System.Drawing.Color.Navy;
            this.groupBox6.Location = new System.Drawing.Point(2, 2);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(230, 53);
            this.groupBox6.TabIndex = 13;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Tìm đường dẫn đến Võ Lâm 2:";
            // 
            // Txbduongdan
            // 
            this.Txbduongdan.ForeColor = System.Drawing.Color.Navy;
            this.Txbduongdan.Location = new System.Drawing.Point(56, 20);
            this.Txbduongdan.Name = "Txbduongdan";
            this.Txbduongdan.Size = new System.Drawing.Size(168, 21);
            this.Txbduongdan.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CobServer);
            this.groupBox1.Controls.Add(this.BtnHuy);
            this.groupBox1.Controls.Add(this.BtnXoa);
            this.groupBox1.Controls.Add(this.BtnSua);
            this.groupBox1.Controls.Add(this.BtnThem);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.CobSTTNV);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.Txbmatkhau);
            this.groupBox1.Controls.Add(this.Txbtaikhoan);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Navy;
            this.groupBox1.Location = new System.Drawing.Point(2, 61);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 194);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Nhập thông tin:";
            // 
            // CobServer
            // 
            this.CobServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CobServer.ForeColor = System.Drawing.Color.Navy;
            this.CobServer.FormattingEnabled = true;
            this.CobServer.Items.AddRange(new object[] {
            "Bạch Hổ",
            "Phục Hổ",
            "Quán Hổ",
            "Phi Hổ",
            "Tàng Long",
            "Thiên Long",
            "Linh Bảo Sơn",
            "Hoả Phụng",
            "Hàng Long"});
            this.CobServer.Location = new System.Drawing.Point(79, 103);
            this.CobServer.Name = "CobServer";
            this.CobServer.Size = new System.Drawing.Size(145, 23);
            this.CobServer.TabIndex = 3;
            this.CobServer.SelectedIndexChanged += new System.EventHandler(this.CobServer_SelectedIndexChanged);
            // 
            // BtnHuy
            // 
            this.BtnHuy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.BtnHuy.Location = new System.Drawing.Point(164, 163);
            this.BtnHuy.Name = "BtnHuy";
            this.BtnHuy.Size = new System.Drawing.Size(59, 23);
            this.BtnHuy.TabIndex = 7;
            this.BtnHuy.Text = "Huỷ";
            this.BtnHuy.UseVisualStyleBackColor = true;
            this.BtnHuy.Click += new System.EventHandler(this.BtnHuy_Click);
            // 
            // BtnXoa
            // 
            this.BtnXoa.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.BtnXoa.Location = new System.Drawing.Point(164, 134);
            this.BtnXoa.Name = "BtnXoa";
            this.BtnXoa.Size = new System.Drawing.Size(59, 23);
            this.BtnXoa.TabIndex = 6;
            this.BtnXoa.Text = "Xoá";
            this.BtnXoa.UseVisualStyleBackColor = true;
            this.BtnXoa.Click += new System.EventHandler(this.BtnXoa_Click);
            // 
            // BtnSua
            // 
            this.BtnSua.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.BtnSua.Location = new System.Drawing.Point(88, 134);
            this.BtnSua.Name = "BtnSua";
            this.BtnSua.Size = new System.Drawing.Size(59, 23);
            this.BtnSua.TabIndex = 5;
            this.BtnSua.Text = "Sửa";
            this.BtnSua.UseVisualStyleBackColor = true;
            this.BtnSua.Click += new System.EventHandler(this.BtnSua_Click);
            // 
            // BtnThem
            // 
            this.BtnThem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.BtnThem.Location = new System.Drawing.Point(12, 134);
            this.BtnThem.Name = "BtnThem";
            this.BtnThem.Size = new System.Drawing.Size(59, 23);
            this.BtnThem.TabIndex = 4;
            this.BtnThem.Text = "Thêm";
            this.BtnThem.UseVisualStyleBackColor = true;
            this.BtnThem.Click += new System.EventHandler(this.BtnThem_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "Server:";
            // 
            // CobSTTNV
            // 
            this.CobSTTNV.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CobSTTNV.ForeColor = System.Drawing.Color.Navy;
            this.CobSTTNV.FormattingEnabled = true;
            this.CobSTTNV.Items.AddRange(new object[] {
            "0",
            "1",
            "2"});
            this.CobSTTNV.Location = new System.Drawing.Point(79, 74);
            this.CobSTTNV.Name = "CobSTTNV";
            this.CobSTTNV.Size = new System.Drawing.Size(145, 23);
            this.CobSTTNV.TabIndex = 2;
            this.CobSTTNV.SelectedIndexChanged += new System.EventHandler(this.CobSTTNV_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "STT NV:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Mật khẩu:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Tài khoản:";
            // 
            // Txbmatkhau
            // 
            this.Txbmatkhau.ForeColor = System.Drawing.Color.Navy;
            this.Txbmatkhau.Location = new System.Drawing.Point(79, 47);
            this.Txbmatkhau.Name = "Txbmatkhau";
            this.Txbmatkhau.PasswordChar = '*';
            this.Txbmatkhau.Size = new System.Drawing.Size(145, 21);
            this.Txbmatkhau.TabIndex = 1;
            // 
            // Txbtaikhoan
            // 
            this.Txbtaikhoan.ForeColor = System.Drawing.Color.Navy;
            this.Txbtaikhoan.Location = new System.Drawing.Point(79, 20);
            this.Txbtaikhoan.Name = "Txbtaikhoan";
            this.Txbtaikhoan.Size = new System.Drawing.Size(145, 21);
            this.Txbtaikhoan.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxAcount);
            this.groupBox2.Controls.Add(this.lsvAcout);
            this.groupBox2.Controls.Add(this.BtnLuu);
            this.groupBox2.Controls.Add(this.BtnDangNhap);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Navy;
            this.groupBox2.Location = new System.Drawing.Point(2, 257);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(230, 271);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Danh sách:";
            // 
            // checkBoxAcount
            // 
            this.checkBoxAcount.AutoSize = true;
            this.checkBoxAcount.Location = new System.Drawing.Point(8, 27);
            this.checkBoxAcount.Name = "checkBoxAcount";
            this.checkBoxAcount.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAcount.TabIndex = 1;
            this.checkBoxAcount.UseVisualStyleBackColor = true;
            this.checkBoxAcount.CheckedChanged += new System.EventHandler(this.checkBoxAcount_CheckedChanged);
            // 
            // BtnLuu
            // 
            this.BtnLuu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.BtnLuu.Location = new System.Drawing.Point(147, 240);
            this.BtnLuu.Name = "BtnLuu";
            this.BtnLuu.Size = new System.Drawing.Size(80, 23);
            this.BtnLuu.TabIndex = 0;
            this.BtnLuu.Text = "Lưu";
            this.BtnLuu.UseVisualStyleBackColor = true;
            this.BtnLuu.Click += new System.EventHandler(this.BtnLuu_Click);
            // 
            // BtnDangNhap
            // 
            this.BtnDangNhap.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.BtnDangNhap.Location = new System.Drawing.Point(11, 240);
            this.BtnDangNhap.Name = "BtnDangNhap";
            this.BtnDangNhap.Size = new System.Drawing.Size(80, 23);
            this.BtnDangNhap.TabIndex = 3;
            this.BtnDangNhap.Text = "Đăng nhập";
            this.BtnDangNhap.UseVisualStyleBackColor = true;
            this.BtnDangNhap.Click += new System.EventHandler(this.BtnDangNhap_Click);
            // 
            // Tmrchonnv
            // 
            this.Tmrchonnv.Enabled = true;
            this.Tmrchonnv.Interval = 200;
            this.Tmrchonnv.Tick += new System.EventHandler(this.Tmrchonnv_Tick);
            // 
            // lsvAcout
            // 
            this.lsvAcout.AllowDrop = true;
            this.lsvAcout.BackColor = System.Drawing.SystemColors.Window;
            this.lsvAcout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lsvAcout.CheckBoxes = true;
            this.lsvAcout.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.lsvAcout.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.lsvAcout.ForeColor = System.Drawing.Color.Navy;
            this.lsvAcout.FullRowSelect = true;
            this.lsvAcout.GridLines = true;
            this.lsvAcout.Location = new System.Drawing.Point(4, 20);
            this.lsvAcout.MultiSelect = false;
            this.lsvAcout.Name = "lsvAcout";
            this.lsvAcout.Size = new System.Drawing.Size(223, 213);
            this.lsvAcout.TabIndex = 2;
            this.lsvAcout.UseCompatibleStateImageBehavior = false;
            this.lsvAcout.View = System.Windows.Forms.View.Details;
            this.lsvAcout.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lsvAcout_ItemCheck);
            this.lsvAcout.SelectedIndexChanged += new System.EventHandler(this.lsvAcout_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 20;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Mật khẩu";
            this.columnHeader3.Width = 70;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "STT Nhân vật";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Máy chủ";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Server";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Đường Dẫn";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Tag = "";
            this.columnHeader2.Text = "Tên TK";
            this.columnHeader2.Width = 100;
            // 
            // FrmAutoLogin
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(234, 533);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox6);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ForeColor = System.Drawing.Color.Navy;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAutoLogin";
            this.Text = "Auto Đăng nhập";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmAutoLogin_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmAutoLogin_FormClosed);
            this.Load += new System.EventHandler(this.FrmAutoLogin_Load);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer TmrUpdatehWnd;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox Txbduongdan;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox CobServer;
        private System.Windows.Forms.Button BtnHuy;
        private System.Windows.Forms.Button BtnXoa;
        private System.Windows.Forms.Button BtnSua;
        private System.Windows.Forms.Button BtnThem;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox CobSTTNV;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Txbmatkhau;
        private System.Windows.Forms.TextBox Txbtaikhoan;
        private System.Windows.Forms.GroupBox groupBox2;
        private ListViewsOther lsvAcout;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button BtnLuu;
        private System.Windows.Forms.Button BtnDangNhap;
        private System.Windows.Forms.CheckBox checkBoxAcount;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Timer Tmrchonnv;
    }
}