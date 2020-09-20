namespace AutoPK
{
    partial class FromAutoBuff
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FromAutoBuff));
            this.CobListPlayer = new System.Windows.Forms.ComboBox();
            this.Ckbtheosau = new System.Windows.Forms.CheckBox();
            this.btnCapnhatPlayer = new System.Windows.Forms.Button();
            this.cbbuffdanhsach = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lstbuff = new System.Windows.Forms.ListBox();
            this.btnxoa = new System.Windows.Forms.Button();
            this.btnthem = new System.Windows.Forms.Button();
            this.btncapnhat = new System.Windows.Forms.Button();
            this.cbbuff = new System.Windows.Forms.ComboBox();
            this.TmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // CobListPlayer
            // 
            this.CobListPlayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CobListPlayer.FormattingEnabled = true;
            this.CobListPlayer.Location = new System.Drawing.Point(89, 325);
            this.CobListPlayer.Name = "CobListPlayer";
            this.CobListPlayer.Size = new System.Drawing.Size(132, 23);
            this.CobListPlayer.TabIndex = 27;
            this.CobListPlayer.SelectedIndexChanged += new System.EventHandler(this.CobListPlayer_SelectedIndexChanged);
            // 
            // Ckbtheosau
            // 
            this.Ckbtheosau.AutoSize = true;
            this.Ckbtheosau.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Ckbtheosau.Location = new System.Drawing.Point(5, 327);
            this.Ckbtheosau.Name = "Ckbtheosau";
            this.Ckbtheosau.Size = new System.Drawing.Size(77, 19);
            this.Ckbtheosau.TabIndex = 25;
            this.Ckbtheosau.Text = "Theo sau";
            this.Ckbtheosau.UseVisualStyleBackColor = true;
            this.Ckbtheosau.CheckedChanged += new System.EventHandler(this.Ckbtheosau_CheckedChanged);
            // 
            // btnCapnhatPlayer
            // 
            this.btnCapnhatPlayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCapnhatPlayer.ForeColor = System.Drawing.Color.Navy;
            this.btnCapnhatPlayer.Location = new System.Drawing.Point(150, 355);
            this.btnCapnhatPlayer.Name = "btnCapnhatPlayer";
            this.btnCapnhatPlayer.Size = new System.Drawing.Size(71, 23);
            this.btnCapnhatPlayer.TabIndex = 23;
            this.btnCapnhatPlayer.Text = "Cập nhật";
            this.btnCapnhatPlayer.UseVisualStyleBackColor = true;
            this.btnCapnhatPlayer.Click += new System.EventHandler(this.btnCapnhatPlayer_Click);
            // 
            // cbbuffdanhsach
            // 
            this.cbbuffdanhsach.AutoSize = true;
            this.cbbuffdanhsach.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbuffdanhsach.Location = new System.Drawing.Point(5, 301);
            this.cbbuffdanhsach.Name = "cbbuffdanhsach";
            this.cbbuffdanhsach.Size = new System.Drawing.Size(134, 19);
            this.cbbuffdanhsach.TabIndex = 26;
            this.cbbuffdanhsach.Text = "Buff theo danh sách";
            this.cbbuffdanhsach.UseVisualStyleBackColor = true;
            this.cbbuffdanhsach.CheckedChanged += new System.EventHandler(this.cbbuffdanhsach_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lstbuff);
            this.groupBox3.Controls.Add(this.btnxoa);
            this.groupBox3.Controls.Add(this.btnthem);
            this.groupBox3.Controls.Add(this.btncapnhat);
            this.groupBox3.Controls.Add(this.cbbuff);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.Navy;
            this.groupBox3.Location = new System.Drawing.Point(5, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(222, 292);
            this.groupBox3.TabIndex = 24;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Đội";
            // 
            // lstbuff
            // 
            this.lstbuff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstbuff.ForeColor = System.Drawing.Color.Navy;
            this.lstbuff.FormattingEnabled = true;
            this.lstbuff.ItemHeight = 15;
            this.lstbuff.Location = new System.Drawing.Point(6, 87);
            this.lstbuff.Name = "lstbuff";
            this.lstbuff.Size = new System.Drawing.Size(210, 199);
            this.lstbuff.TabIndex = 6;
            this.lstbuff.SelectedIndexChanged += new System.EventHandler(this.lstbuff_SelectedIndexChanged_1);
            // 
            // btnxoa
            // 
            this.btnxoa.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnxoa.Location = new System.Drawing.Point(156, 54);
            this.btnxoa.Name = "btnxoa";
            this.btnxoa.Size = new System.Drawing.Size(60, 23);
            this.btnxoa.TabIndex = 5;
            this.btnxoa.Text = "Xóa";
            this.btnxoa.UseVisualStyleBackColor = true;
            this.btnxoa.Click += new System.EventHandler(this.btnxoa_Click);
            // 
            // btnthem
            // 
            this.btnthem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnthem.ForeColor = System.Drawing.Color.Navy;
            this.btnthem.Location = new System.Drawing.Point(81, 54);
            this.btnthem.Name = "btnthem";
            this.btnthem.Size = new System.Drawing.Size(60, 23);
            this.btnthem.TabIndex = 4;
            this.btnthem.Text = "Thêm";
            this.btnthem.UseVisualStyleBackColor = true;
            this.btnthem.Click += new System.EventHandler(this.btnthem_Click);
            // 
            // btncapnhat
            // 
            this.btncapnhat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btncapnhat.ForeColor = System.Drawing.Color.Navy;
            this.btncapnhat.Location = new System.Drawing.Point(6, 19);
            this.btncapnhat.Name = "btncapnhat";
            this.btncapnhat.Size = new System.Drawing.Size(71, 23);
            this.btncapnhat.TabIndex = 2;
            this.btncapnhat.Text = "Cập nhật";
            this.btncapnhat.UseVisualStyleBackColor = true;
            this.btncapnhat.Click += new System.EventHandler(this.btncapnhat_Click);
            // 
            // cbbuff
            // 
            this.cbbuff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbuff.ForeColor = System.Drawing.Color.Blue;
            this.cbbuff.FormattingEnabled = true;
            this.cbbuff.Location = new System.Drawing.Point(84, 19);
            this.cbbuff.Name = "cbbuff";
            this.cbbuff.Size = new System.Drawing.Size(132, 23);
            this.cbbuff.TabIndex = 1;
            // 
            // TmrUpdate
            // 
            this.TmrUpdate.Enabled = true;
            this.TmrUpdate.Interval = 1000;
            this.TmrUpdate.Tick += new System.EventHandler(this.TmrUpdate_Tick);
            // 
            // FromAutoBuff
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(234, 385);
            this.Controls.Add(this.CobListPlayer);
            this.Controls.Add(this.Ckbtheosau);
            this.Controls.Add(this.btnCapnhatPlayer);
            this.Controls.Add(this.cbbuffdanhsach);
            this.Controls.Add(this.groupBox3);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ForeColor = System.Drawing.Color.Navy;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FromAutoBuff";
            this.Text = "FromAutoBuff";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FromAutoBuff_FormClosing);
            this.Load += new System.EventHandler(this.FromAutoBuff_Load);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CobListPlayer;
        private System.Windows.Forms.CheckBox Ckbtheosau;
        private System.Windows.Forms.Button btnCapnhatPlayer;
        private System.Windows.Forms.CheckBox cbbuffdanhsach;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnxoa;
        private System.Windows.Forms.Button btnthem;
        private System.Windows.Forms.Button btncapnhat;
        private System.Windows.Forms.ComboBox cbbuff;
        private System.Windows.Forms.ListBox lstbuff;
        private System.Windows.Forms.Timer TmrUpdate;
    }
}