namespace AutoPK
{
    partial class FrmTNT
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTNT));
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.CkbmoruongTKL = new System.Windows.Forms.CheckBox();
            this.CkblattheTKL = new System.Windows.Forms.CheckBox();
            this.CkbvuotaiTKL = new System.Windows.Forms.CheckBox();
            this.Ckbacctreo = new System.Windows.Forms.CheckBox();
            this.Ckbchukey = new System.Windows.Forms.CheckBox();
            this.TmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.groupBox10.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.CkbmoruongTKL);
            this.groupBox10.Controls.Add(this.CkblattheTKL);
            this.groupBox10.Controls.Add(this.CkbvuotaiTKL);
            this.groupBox10.Controls.Add(this.Ckbacctreo);
            this.groupBox10.Controls.Add(this.Ckbchukey);
            this.groupBox10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox10.ForeColor = System.Drawing.Color.Navy;
            this.groupBox10.Location = new System.Drawing.Point(6, 7);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(222, 96);
            this.groupBox10.TabIndex = 13;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Thái nhất tháp";
            // 
            // CkbmoruongTKL
            // 
            this.CkbmoruongTKL.AutoSize = true;
            this.CkbmoruongTKL.Location = new System.Drawing.Point(119, 21);
            this.CkbmoruongTKL.Name = "CkbmoruongTKL";
            this.CkbmoruongTKL.Size = new System.Drawing.Size(104, 19);
            this.CkbmoruongTKL.TabIndex = 9;
            this.CkbmoruongTKL.Text = "Mở rương TKL";
            this.CkbmoruongTKL.UseVisualStyleBackColor = true;
            this.CkbmoruongTKL.CheckedChanged += new System.EventHandler(this.CkbmoruongTKL_CheckedChanged);
            // 
            // CkblattheTKL
            // 
            this.CkblattheTKL.AutoSize = true;
            this.CkblattheTKL.Location = new System.Drawing.Point(119, 73);
            this.CkblattheTKL.Name = "CkblattheTKL";
            this.CkblattheTKL.Size = new System.Drawing.Size(88, 19);
            this.CkblattheTKL.TabIndex = 9;
            this.CkblattheTKL.Text = "Lật thẻ TKL";
            this.CkblattheTKL.UseVisualStyleBackColor = true;
            this.CkblattheTKL.CheckedChanged += new System.EventHandler(this.CkblattheTKL_CheckedChanged);
            // 
            // CkbvuotaiTKL
            // 
            this.CkbvuotaiTKL.AutoSize = true;
            this.CkbvuotaiTKL.Location = new System.Drawing.Point(119, 47);
            this.CkbvuotaiTKL.Name = "CkbvuotaiTKL";
            this.CkbvuotaiTKL.Size = new System.Drawing.Size(88, 19);
            this.CkbvuotaiTKL.TabIndex = 9;
            this.CkbvuotaiTKL.Text = "Vượt ải TKL";
            this.CkbvuotaiTKL.UseVisualStyleBackColor = true;
            this.CkbvuotaiTKL.CheckedChanged += new System.EventHandler(this.CkbvuotaiTKL_CheckedChanged);
            // 
            // Ckbacctreo
            // 
            this.Ckbacctreo.AutoSize = true;
            this.Ckbacctreo.Location = new System.Drawing.Point(6, 47);
            this.Ckbacctreo.Name = "Ckbacctreo";
            this.Ckbacctreo.Size = new System.Drawing.Size(69, 19);
            this.Ckbacctreo.TabIndex = 9;
            this.Ckbacctreo.Text = "Acc treo";
            this.Ckbacctreo.UseVisualStyleBackColor = true;
            this.Ckbacctreo.CheckedChanged += new System.EventHandler(this.Ckbacctreo_CheckedChanged);
            // 
            // Ckbchukey
            // 
            this.Ckbchukey.AutoSize = true;
            this.Ckbchukey.Location = new System.Drawing.Point(6, 21);
            this.Ckbchukey.Name = "Ckbchukey";
            this.Ckbchukey.Size = new System.Drawing.Size(71, 19);
            this.Ckbchukey.TabIndex = 9;
            this.Ckbchukey.Text = "Chủ Key";
            this.Ckbchukey.UseVisualStyleBackColor = true;
            this.Ckbchukey.CheckedChanged += new System.EventHandler(this.Ckbchukey_CheckedChanged);
            // 
            // TmrUpdate
            // 
            this.TmrUpdate.Enabled = true;
            this.TmrUpdate.Interval = 1000;
            this.TmrUpdate.Tick += new System.EventHandler(this.TmrUpdate_Tick);
            // 
            // FrmTNT
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(234, 107);
            this.Controls.Add(this.groupBox10);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ForeColor = System.Drawing.Color.Navy;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmTNT";
            this.Text = "FrmTNT";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTNT_FormClosing);
            this.Load += new System.EventHandler(this.FrmTNT_Load);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.CheckBox CkbmoruongTKL;
        private System.Windows.Forms.CheckBox CkblattheTKL;
        private System.Windows.Forms.CheckBox CkbvuotaiTKL;
        private System.Windows.Forms.CheckBox Ckbacctreo;
        private System.Windows.Forms.CheckBox Ckbchukey;
        private System.Windows.Forms.Timer TmrUpdate;
    }
}