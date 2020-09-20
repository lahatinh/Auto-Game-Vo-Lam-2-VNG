namespace AutoPK
{
    partial class Frmphuchoi
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frmphuchoi));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CobTheLuc = new System.Windows.Forms.ComboBox();
            this.CobLakCtr = new System.Windows.Forms.ComboBox();
            this.CobCuuChuyen = new System.Windows.Forms.ComboBox();
            this.CobMana = new System.Windows.Forms.ComboBox();
            this.CobSinhNoi = new System.Windows.Forms.ComboBox();
            this.CobMau = new System.Windows.Forms.ComboBox();
            this.txbDelay = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txbThelucUse = new System.Windows.Forms.TextBox();
            this.txbLakCTUse = new System.Windows.Forms.TextBox();
            this.Ckbanbanhngo = new System.Windows.Forms.CheckBox();
            this.txbCCuse = new System.Windows.Forms.TextBox();
            this.CkbAnLakCtr = new System.Windows.Forms.CheckBox();
            this.CkbAnmauCC = new System.Windows.Forms.CheckBox();
            this.CkbBommana = new System.Windows.Forms.CheckBox();
            this.Ckbansinhnoi = new System.Windows.Forms.CheckBox();
            this.CkbBommau = new System.Windows.Forms.CheckBox();
            this.txbManaUse = new System.Windows.Forms.TextBox();
            this.txbSinhnoiUse = new System.Windows.Forms.TextBox();
            this.txbHPuse = new System.Windows.Forms.TextBox();
            this.TmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CobTheLuc);
            this.groupBox1.Controls.Add(this.CobLakCtr);
            this.groupBox1.Controls.Add(this.CobCuuChuyen);
            this.groupBox1.Controls.Add(this.CobMana);
            this.groupBox1.Controls.Add(this.CobSinhNoi);
            this.groupBox1.Controls.Add(this.CobMau);
            this.groupBox1.Controls.Add(this.txbDelay);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.txbThelucUse);
            this.groupBox1.Controls.Add(this.txbLakCTUse);
            this.groupBox1.Controls.Add(this.Ckbanbanhngo);
            this.groupBox1.Controls.Add(this.txbCCuse);
            this.groupBox1.Controls.Add(this.CkbAnLakCtr);
            this.groupBox1.Controls.Add(this.CkbAnmauCC);
            this.groupBox1.Controls.Add(this.CkbBommana);
            this.groupBox1.Controls.Add(this.Ckbansinhnoi);
            this.groupBox1.Controls.Add(this.CkbBommau);
            this.groupBox1.Controls.Add(this.txbManaUse);
            this.groupBox1.Controls.Add(this.txbSinhnoiUse);
            this.groupBox1.Controls.Add(this.txbHPuse);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.groupBox1.Location = new System.Drawing.Point(2, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 223);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Phục hồi";
            // 
            // CobTheLuc
            // 
            this.CobTheLuc.FormattingEnabled = true;
            this.CobTheLuc.Items.AddRange(new object[] {
            "Banh Ngo"});
            this.CobTheLuc.Location = new System.Drawing.Point(74, 156);
            this.CobTheLuc.Name = "CobTheLuc";
            this.CobTheLuc.Size = new System.Drawing.Size(95, 23);
            this.CobTheLuc.TabIndex = 21;
            this.CobTheLuc.SelectedIndexChanged += new System.EventHandler(this.CobTheLuc_SelectedIndexChanged);
            this.CobTheLuc.Click += new System.EventHandler(this.CobTheLuc_Click);
            // 
            // CobLakCtr
            // 
            this.CobLakCtr.FormattingEnabled = true;
            this.CobLakCtr.Items.AddRange(new object[] {
            "Than Trao don",
            "Vo Cuc Tien don"});
            this.CobLakCtr.Location = new System.Drawing.Point(74, 128);
            this.CobLakCtr.Name = "CobLakCtr";
            this.CobLakCtr.Size = new System.Drawing.Size(95, 23);
            this.CobLakCtr.TabIndex = 21;
            this.CobLakCtr.SelectedIndexChanged += new System.EventHandler(this.CobLakCtr_SelectedIndexChanged);
            this.CobLakCtr.Click += new System.EventHandler(this.CobLakCtr_Click);
            // 
            // CobCuuChuyen
            // 
            this.CobCuuChuyen.FormattingEnabled = true;
            this.CobCuuChuyen.Items.AddRange(new object[] {
            "Tieu Dao Cuu Chuyen Don",
            "Cuu chuyen hoi hon don"});
            this.CobCuuChuyen.Location = new System.Drawing.Point(74, 100);
            this.CobCuuChuyen.Name = "CobCuuChuyen";
            this.CobCuuChuyen.Size = new System.Drawing.Size(95, 23);
            this.CobCuuChuyen.TabIndex = 21;
            this.CobCuuChuyen.SelectedIndexChanged += new System.EventHandler(this.CobCuuChuyen_SelectedIndexChanged);
            this.CobCuuChuyen.Click += new System.EventHandler(this.CobCuuChuyen_Click);
            // 
            // CobMana
            // 
            this.CobMana.FormattingEnabled = true;
            this.CobMana.Items.AddRange(new object[] {
            "Dai Hoan don",
            "Nhat Nguyen Phuc Thuy Don",
            "Van Vat Quy Nguyen Don",
            "Dao Tri Dai Hoan Don",
            "Tieu Dao Dai Hoan Don",
            "Tieu Dao Phuc Thuy Don",
            "Tieu Dao Quy Nguyen Don"});
            this.CobMana.Location = new System.Drawing.Point(74, 72);
            this.CobMana.Name = "CobMana";
            this.CobMana.Size = new System.Drawing.Size(95, 23);
            this.CobMana.TabIndex = 21;
            this.CobMana.SelectedIndexChanged += new System.EventHandler(this.CobMana_SelectedIndexChanged);
            this.CobMana.Click += new System.EventHandler(this.CobMana_Click);
            // 
            // CobSinhNoi
            // 
            this.CobSinhNoi.FormattingEnabled = true;
            this.CobSinhNoi.Items.AddRange(new object[] {
            "Ngoc Linh tan",
            "Ngu Hoa Ngoc Lo Hoan",
            "Sinh Sinh Hoa Tan",
            "Tuyet Sam Phan Khi Hoan",
            "Tieu Dao Ngoc Linh Tan",
            "Tieu Dao Ngoc Lo Hoan",
            "Tieu Dao Tao Hoa Tan"});
            this.CobSinhNoi.Location = new System.Drawing.Point(74, 44);
            this.CobSinhNoi.Name = "CobSinhNoi";
            this.CobSinhNoi.Size = new System.Drawing.Size(95, 23);
            this.CobSinhNoi.TabIndex = 21;
            this.CobSinhNoi.SelectedIndexChanged += new System.EventHandler(this.CobSinhNoi_SelectedIndexChanged);
            this.CobSinhNoi.Click += new System.EventHandler(this.CobSinhNoi_Click);
            // 
            // CobMau
            // 
            this.CobMau.FormattingEnabled = true;
            this.CobMau.Items.AddRange(new object[] {
            "Bach Van tan",
            "Thien Huong Cam Tuc",
            "Hac Ngoc Doan Tuc Cao",
            "Linh Chi Tuc Menh Hoan",
            "Tieu Dao Bach Van Tan",
            "Tieu Dao Van Cam Tuc",
            "Tieu Dao Doan Tuc Cao"});
            this.CobMau.Location = new System.Drawing.Point(74, 16);
            this.CobMau.Name = "CobMau";
            this.CobMau.Size = new System.Drawing.Size(95, 23);
            this.CobMau.TabIndex = 21;
            this.CobMau.SelectedIndexChanged += new System.EventHandler(this.CobMau_SelectedIndexChanged);
            this.CobMau.Click += new System.EventHandler(this.CobMau_Click);
            // 
            // txbDelay
            // 
            this.txbDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.txbDelay.Location = new System.Drawing.Point(175, 186);
            this.txbDelay.Name = "txbDelay";
            this.txbDelay.Size = new System.Drawing.Size(49, 21);
            this.txbDelay.TabIndex = 4;
            this.txbDelay.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txbDelay_KeyPress);
            this.txbDelay.Validated += new System.EventHandler(this.txbDelay_Validated);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label19.Location = new System.Drawing.Point(-1, 193);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(65, 15);
            this.label19.TabIndex = 0;
            this.label19.Text = "Giãn cách:";
            // 
            // txbThelucUse
            // 
            this.txbThelucUse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.txbThelucUse.Location = new System.Drawing.Point(175, 158);
            this.txbThelucUse.Name = "txbThelucUse";
            this.txbThelucUse.Size = new System.Drawing.Size(49, 21);
            this.txbThelucUse.TabIndex = 3;
            this.txbThelucUse.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txbThelucUse_KeyPress);
            this.txbThelucUse.Validated += new System.EventHandler(this.txbThelucUse_Validated);
            // 
            // txbLakCTUse
            // 
            this.txbLakCTUse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.txbLakCTUse.Location = new System.Drawing.Point(175, 130);
            this.txbLakCTUse.Name = "txbLakCTUse";
            this.txbLakCTUse.Size = new System.Drawing.Size(49, 21);
            this.txbLakCTUse.TabIndex = 3;
            this.txbLakCTUse.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txbLakCTUse_KeyPress);
            this.txbLakCTUse.Validated += new System.EventHandler(this.txbLakCTUse_Validated);
            // 
            // Ckbanbanhngo
            // 
            this.Ckbanbanhngo.AutoSize = true;
            this.Ckbanbanhngo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.Ckbanbanhngo.ForeColor = System.Drawing.Color.Navy;
            this.Ckbanbanhngo.Location = new System.Drawing.Point(2, 160);
            this.Ckbanbanhngo.Name = "Ckbanbanhngo";
            this.Ckbanbanhngo.Size = new System.Drawing.Size(69, 19);
            this.Ckbanbanhngo.TabIndex = 20;
            this.Ckbanbanhngo.Text = "Thể lực:";
            this.Ckbanbanhngo.UseVisualStyleBackColor = true;
            this.Ckbanbanhngo.CheckedChanged += new System.EventHandler(this.Ckbanbanhngo_CheckedChanged);
            // 
            // txbCCuse
            // 
            this.txbCCuse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.txbCCuse.Location = new System.Drawing.Point(175, 102);
            this.txbCCuse.Name = "txbCCuse";
            this.txbCCuse.Size = new System.Drawing.Size(49, 21);
            this.txbCCuse.TabIndex = 3;
            this.txbCCuse.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txbCCuse_KeyPress);
            this.txbCCuse.Validated += new System.EventHandler(this.txbCCuse_Validated);
            // 
            // CkbAnLakCtr
            // 
            this.CkbAnLakCtr.AutoSize = true;
            this.CkbAnLakCtr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.CkbAnLakCtr.ForeColor = System.Drawing.Color.Navy;
            this.CkbAnLakCtr.Location = new System.Drawing.Point(2, 132);
            this.CkbAnLakCtr.Name = "CkbAnLakCtr";
            this.CkbAnLakCtr.Size = new System.Drawing.Size(71, 19);
            this.CkbAnLakCtr.TabIndex = 20;
            this.CkbAnLakCtr.Text = "Lak CTr:";
            this.CkbAnLakCtr.UseVisualStyleBackColor = true;
            this.CkbAnLakCtr.CheckedChanged += new System.EventHandler(this.CkbAnLakCtr_CheckedChanged);
            // 
            // CkbAnmauCC
            // 
            this.CkbAnmauCC.AutoSize = true;
            this.CkbAnmauCC.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.CkbAnmauCC.ForeColor = System.Drawing.Color.Navy;
            this.CkbAnmauCC.Location = new System.Drawing.Point(2, 104);
            this.CkbAnmauCC.Name = "CkbAnmauCC";
            this.CkbAnmauCC.Size = new System.Drawing.Size(62, 19);
            this.CkbAnmauCC.TabIndex = 20;
            this.CkbAnmauCC.Text = "Ăn CC:";
            this.CkbAnmauCC.UseVisualStyleBackColor = true;
            this.CkbAnmauCC.CheckedChanged += new System.EventHandler(this.CkbAnmauCC_CheckedChanged);
            // 
            // CkbBommana
            // 
            this.CkbBommana.AutoSize = true;
            this.CkbBommana.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.CkbBommana.ForeColor = System.Drawing.Color.Navy;
            this.CkbBommana.Location = new System.Drawing.Point(3, 76);
            this.CkbBommana.Name = "CkbBommana";
            this.CkbBommana.Size = new System.Drawing.Size(58, 19);
            this.CkbBommana.TabIndex = 20;
            this.CkbBommana.Text = "Mana";
            this.CkbBommana.UseVisualStyleBackColor = true;
            this.CkbBommana.CheckedChanged += new System.EventHandler(this.CkbBommana_CheckedChanged);
            // 
            // Ckbansinhnoi
            // 
            this.Ckbansinhnoi.AutoSize = true;
            this.Ckbansinhnoi.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.Ckbansinhnoi.ForeColor = System.Drawing.Color.Navy;
            this.Ckbansinhnoi.Location = new System.Drawing.Point(3, 49);
            this.Ckbansinhnoi.Name = "Ckbansinhnoi";
            this.Ckbansinhnoi.Size = new System.Drawing.Size(68, 19);
            this.Ckbansinhnoi.TabIndex = 20;
            this.Ckbansinhnoi.Text = "HP Nội:";
            this.Ckbansinhnoi.UseVisualStyleBackColor = true;
            this.Ckbansinhnoi.CheckedChanged += new System.EventHandler(this.Ckbansinhnoi_CheckedChanged);
            // 
            // CkbBommau
            // 
            this.CkbBommau.AutoSize = true;
            this.CkbBommau.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.CkbBommau.ForeColor = System.Drawing.Color.Navy;
            this.CkbBommau.Location = new System.Drawing.Point(3, 21);
            this.CkbBommau.Name = "CkbBommau";
            this.CkbBommau.Size = new System.Drawing.Size(42, 19);
            this.CkbBommau.TabIndex = 20;
            this.CkbBommau.Text = "Hp";
            this.CkbBommau.UseVisualStyleBackColor = true;
            this.CkbBommau.CheckedChanged += new System.EventHandler(this.CkbBommau_CheckedChanged);
            // 
            // txbManaUse
            // 
            this.txbManaUse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.txbManaUse.Location = new System.Drawing.Point(175, 74);
            this.txbManaUse.Name = "txbManaUse";
            this.txbManaUse.Size = new System.Drawing.Size(49, 21);
            this.txbManaUse.TabIndex = 2;
            this.txbManaUse.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txbManaUse_KeyPress);
            this.txbManaUse.Validated += new System.EventHandler(this.txbManaUse_Validated);
            // 
            // txbSinhnoiUse
            // 
            this.txbSinhnoiUse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.txbSinhnoiUse.Location = new System.Drawing.Point(175, 46);
            this.txbSinhnoiUse.Name = "txbSinhnoiUse";
            this.txbSinhnoiUse.Size = new System.Drawing.Size(49, 21);
            this.txbSinhnoiUse.TabIndex = 2;
            this.txbSinhnoiUse.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txbSinhnoiUse_KeyPress);
            this.txbSinhnoiUse.Validated += new System.EventHandler(this.txbSinhnoiUse_Validated);
            // 
            // txbHPuse
            // 
            this.txbHPuse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.txbHPuse.Location = new System.Drawing.Point(175, 18);
            this.txbHPuse.Name = "txbHPuse";
            this.txbHPuse.Size = new System.Drawing.Size(49, 21);
            this.txbHPuse.TabIndex = 2;
            this.txbHPuse.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txbHPuse_KeyPress);
            this.txbHPuse.Validated += new System.EventHandler(this.txbHPuse_Validated);
            // 
            // TmrUpdate
            // 
            this.TmrUpdate.Enabled = true;
            this.TmrUpdate.Interval = 1000;
            this.TmrUpdate.Tick += new System.EventHandler(this.TmrUpdate_Tick);
            // 
            // Frmphuchoi
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(234, 232);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ForeColor = System.Drawing.Color.Navy;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frmphuchoi";
            this.Text = "Frmphuchoi";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frmphuchoi_FormClosing);
            this.Load += new System.EventHandler(this.Frmphuchoi_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox CobTheLuc;
        private System.Windows.Forms.ComboBox CobLakCtr;
        private System.Windows.Forms.ComboBox CobCuuChuyen;
        private System.Windows.Forms.ComboBox CobMana;
        private System.Windows.Forms.ComboBox CobSinhNoi;
        private System.Windows.Forms.ComboBox CobMau;
        private System.Windows.Forms.TextBox txbDelay;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txbThelucUse;
        private System.Windows.Forms.TextBox txbLakCTUse;
        private System.Windows.Forms.CheckBox Ckbanbanhngo;
        private System.Windows.Forms.TextBox txbCCuse;
        private System.Windows.Forms.CheckBox CkbAnLakCtr;
        private System.Windows.Forms.CheckBox CkbAnmauCC;
        private System.Windows.Forms.CheckBox CkbBommana;
        private System.Windows.Forms.CheckBox Ckbansinhnoi;
        private System.Windows.Forms.CheckBox CkbBommau;
        private System.Windows.Forms.TextBox txbManaUse;
        private System.Windows.Forms.TextBox txbSinhnoiUse;
        private System.Windows.Forms.TextBox txbHPuse;
        private System.Windows.Forms.Timer TmrUpdate;
    }
}