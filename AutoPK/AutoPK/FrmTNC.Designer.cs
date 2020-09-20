namespace AutoPK
{
    partial class FrmTNC
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTNC));
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.BtnluuvitridaoTNC = new System.Windows.Forms.Button();
            this.Ckbtumuadungcu = new System.Windows.Forms.CheckBox();
            this.CkbPheTong = new System.Windows.Forms.CheckBox();
            this.Ckbtutranv = new System.Windows.Forms.CheckBox();
            this.Cobquancong = new System.Windows.Forms.ComboBox();
            this.TmrUpdateForm = new System.Windows.Forms.Timer(this.components);
            this.groupBox9.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.BtnluuvitridaoTNC);
            this.groupBox9.Controls.Add(this.Ckbtumuadungcu);
            this.groupBox9.Controls.Add(this.CkbPheTong);
            this.groupBox9.Controls.Add(this.Ckbtutranv);
            this.groupBox9.Controls.Add(this.Cobquancong);
            this.groupBox9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox9.ForeColor = System.Drawing.Color.Navy;
            this.groupBox9.Location = new System.Drawing.Point(5, 7);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(222, 97);
            this.groupBox9.TabIndex = 13;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Tài nguyên chiến";
            // 
            // BtnluuvitridaoTNC
            // 
            this.BtnluuvitridaoTNC.Location = new System.Drawing.Point(119, 14);
            this.BtnluuvitridaoTNC.Name = "BtnluuvitridaoTNC";
            this.BtnluuvitridaoTNC.Size = new System.Drawing.Size(97, 23);
            this.BtnluuvitridaoTNC.TabIndex = 11;
            this.BtnluuvitridaoTNC.Text = "Lưu vị trí đào";
            this.BtnluuvitridaoTNC.UseVisualStyleBackColor = true;
            this.BtnluuvitridaoTNC.Click += new System.EventHandler(this.BtnluuvitridaoTNC_Click);
            // 
            // Ckbtumuadungcu
            // 
            this.Ckbtumuadungcu.AutoSize = true;
            this.Ckbtumuadungcu.Location = new System.Drawing.Point(12, 68);
            this.Ckbtumuadungcu.Name = "Ckbtumuadungcu";
            this.Ckbtumuadungcu.Size = new System.Drawing.Size(115, 19);
            this.Ckbtumuadungcu.TabIndex = 9;
            this.Ckbtumuadungcu.Text = "Tự mua dụng cụ";
            this.Ckbtumuadungcu.UseVisualStyleBackColor = true;
            this.Ckbtumuadungcu.CheckedChanged += new System.EventHandler(this.Ckbtumuadungcu_CheckedChanged);
            // 
            // CkbPheTong
            // 
            this.CkbPheTong.AutoSize = true;
            this.CkbPheTong.Location = new System.Drawing.Point(12, 20);
            this.CkbPheTong.Name = "CkbPheTong";
            this.CkbPheTong.Size = new System.Drawing.Size(79, 19);
            this.CkbPheTong.TabIndex = 9;
            this.CkbPheTong.Text = "Phe Tống";
            this.CkbPheTong.UseVisualStyleBackColor = true;
            this.CkbPheTong.CheckedChanged += new System.EventHandler(this.CkbPheTong_CheckedChanged);
            // 
            // Ckbtutranv
            // 
            this.Ckbtutranv.AutoSize = true;
            this.Ckbtutranv.Location = new System.Drawing.Point(12, 44);
            this.Ckbtutranv.Name = "Ckbtutranv";
            this.Ckbtutranv.Size = new System.Drawing.Size(76, 19);
            this.Ckbtutranv.TabIndex = 9;
            this.Ckbtutranv.Text = "Tự trả NV";
            this.Ckbtutranv.UseVisualStyleBackColor = true;
            this.Ckbtutranv.CheckedChanged += new System.EventHandler(this.Ckbtutranv_CheckedChanged);
            // 
            // Cobquancong
            // 
            this.Cobquancong.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cobquancong.ForeColor = System.Drawing.Color.Blue;
            this.Cobquancong.FormattingEnabled = true;
            this.Cobquancong.Items.AddRange(new object[] {
            "Khong dung quan cong",
            "Quan cong chuong",
            "Quan cong dai",
            "Quan cong huy hoang",
            "Quan cong vinh du"});
            this.Cobquancong.Location = new System.Drawing.Point(119, 43);
            this.Cobquancong.Name = "Cobquancong";
            this.Cobquancong.Size = new System.Drawing.Size(97, 23);
            this.Cobquancong.TabIndex = 10;
            this.Cobquancong.SelectedIndexChanged += new System.EventHandler(this.Cobquancong_SelectedIndexChanged);
            this.Cobquancong.Click += new System.EventHandler(this.Cobquancong_Click);
            // 
            // TmrUpdateForm
            // 
            this.TmrUpdateForm.Enabled = true;
            this.TmrUpdateForm.Interval = 1000;
            this.TmrUpdateForm.Tick += new System.EventHandler(this.TmrUpdateForm_Tick);
            // 
            // FrmTNC
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(234, 108);
            this.Controls.Add(this.groupBox9);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ForeColor = System.Drawing.Color.Navy;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmTNC";
            this.Text = "FrmTNC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTNC_FormClosing);
            this.Load += new System.EventHandler(this.FrmTNC_Load);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Button BtnluuvitridaoTNC;
        private System.Windows.Forms.CheckBox Ckbtumuadungcu;
        private System.Windows.Forms.CheckBox CkbPheTong;
        private System.Windows.Forms.CheckBox Ckbtutranv;
        private System.Windows.Forms.ComboBox Cobquancong;
        private System.Windows.Forms.Timer TmrUpdateForm;
    }
}