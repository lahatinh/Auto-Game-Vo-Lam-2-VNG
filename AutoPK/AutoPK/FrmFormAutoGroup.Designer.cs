namespace AutoPK
{
    partial class FrmFormAutoGroup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFormAutoGroup));
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.Btngiaitan = new System.Windows.Forms.Button();
            this.Btnlapto = new System.Windows.Forms.Button();
            this.Ckbchapnhanvaodoi = new System.Windows.Forms.CheckBox();
            this.Ckbnhantatcaloimoi = new System.Windows.Forms.CheckBox();
            this.Ckbnhaptatca = new System.Windows.Forms.CheckBox();
            this.Ckbmoitatca = new System.Windows.Forms.CheckBox();
            this.TmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.groupBox11.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.Btngiaitan);
            this.groupBox11.Controls.Add(this.Btnlapto);
            this.groupBox11.Controls.Add(this.Ckbchapnhanvaodoi);
            this.groupBox11.Controls.Add(this.Ckbnhantatcaloimoi);
            this.groupBox11.Controls.Add(this.Ckbnhaptatca);
            this.groupBox11.Controls.Add(this.Ckbmoitatca);
            this.groupBox11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox11.Location = new System.Drawing.Point(4, 7);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(224, 126);
            this.groupBox11.TabIndex = 4;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Tổ đội";
            // 
            // Btngiaitan
            // 
            this.Btngiaitan.Location = new System.Drawing.Point(143, 50);
            this.Btngiaitan.Name = "Btngiaitan";
            this.Btngiaitan.Size = new System.Drawing.Size(75, 23);
            this.Btngiaitan.TabIndex = 3;
            this.Btngiaitan.Text = "Giải tán";
            this.Btngiaitan.UseVisualStyleBackColor = true;
            this.Btngiaitan.Click += new System.EventHandler(this.Btngiaitan_Click);
            // 
            // Btnlapto
            // 
            this.Btnlapto.Location = new System.Drawing.Point(143, 21);
            this.Btnlapto.Name = "Btnlapto";
            this.Btnlapto.Size = new System.Drawing.Size(75, 23);
            this.Btnlapto.TabIndex = 3;
            this.Btnlapto.Text = "Lập tổ đội";
            this.Btnlapto.UseVisualStyleBackColor = true;
            this.Btnlapto.Click += new System.EventHandler(this.Btnlapto_Click);
            // 
            // Ckbchapnhanvaodoi
            // 
            this.Ckbchapnhanvaodoi.AutoSize = true;
            this.Ckbchapnhanvaodoi.Location = new System.Drawing.Point(8, 99);
            this.Ckbchapnhanvaodoi.Name = "Ckbchapnhanvaodoi";
            this.Ckbchapnhanvaodoi.Size = new System.Drawing.Size(182, 19);
            this.Ckbchapnhanvaodoi.TabIndex = 0;
            this.Ckbchapnhanvaodoi.Text = "Đồng ý tất cả lời xin nhập đội";
            this.Ckbchapnhanvaodoi.UseVisualStyleBackColor = true;
            this.Ckbchapnhanvaodoi.CheckedChanged += new System.EventHandler(this.Ckbchapnhanvaodoi_CheckedChanged);
            this.Ckbchapnhanvaodoi.Click += new System.EventHandler(this.Ckbchapnhanvaodoi_Click);
            // 
            // Ckbnhantatcaloimoi
            // 
            this.Ckbnhantatcaloimoi.AutoSize = true;
            this.Ckbnhantatcaloimoi.Location = new System.Drawing.Point(8, 74);
            this.Ckbnhantatcaloimoi.Name = "Ckbnhantatcaloimoi";
            this.Ckbnhantatcaloimoi.Size = new System.Drawing.Size(128, 19);
            this.Ckbnhantatcaloimoi.TabIndex = 0;
            this.Ckbnhantatcaloimoi.Text = "Nhận tất cả lời mời";
            this.Ckbnhantatcaloimoi.UseVisualStyleBackColor = true;
            this.Ckbnhantatcaloimoi.CheckedChanged += new System.EventHandler(this.Ckbnhantatcaloimoi_CheckedChanged);
            this.Ckbnhantatcaloimoi.Click += new System.EventHandler(this.Ckbnhantatcaloimoi_Click);
            // 
            // Ckbnhaptatca
            // 
            this.Ckbnhaptatca.AutoSize = true;
            this.Ckbnhaptatca.Location = new System.Drawing.Point(8, 49);
            this.Ckbnhaptatca.Name = "Ckbnhaptatca";
            this.Ckbnhaptatca.Size = new System.Drawing.Size(108, 19);
            this.Ckbnhaptatca.TabIndex = 2;
            this.Ckbnhaptatca.Text = "Xin nhập tổ đội";
            this.Ckbnhaptatca.UseVisualStyleBackColor = true;
            this.Ckbnhaptatca.CheckedChanged += new System.EventHandler(this.Ckbnhaptatca_CheckedChanged);
            this.Ckbnhaptatca.Click += new System.EventHandler(this.Ckbnhaptatca_Click);
            // 
            // Ckbmoitatca
            // 
            this.Ckbmoitatca.AutoSize = true;
            this.Ckbmoitatca.Location = new System.Drawing.Point(8, 24);
            this.Ckbmoitatca.Name = "Ckbmoitatca";
            this.Ckbmoitatca.Size = new System.Drawing.Size(79, 19);
            this.Ckbmoitatca.TabIndex = 1;
            this.Ckbmoitatca.Text = "Mời tất cả";
            this.Ckbmoitatca.UseVisualStyleBackColor = true;
            this.Ckbmoitatca.CheckedChanged += new System.EventHandler(this.Ckbmoitatca_CheckedChanged);
            this.Ckbmoitatca.Click += new System.EventHandler(this.Ckbmoitatca_Click);
            // 
            // TmrUpdate
            // 
            this.TmrUpdate.Enabled = true;
            this.TmrUpdate.Interval = 1000;
            this.TmrUpdate.Tick += new System.EventHandler(this.TmrUpdate_Tick);
            // 
            // FrmFormAutoGroup
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(234, 138);
            this.Controls.Add(this.groupBox11);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ForeColor = System.Drawing.Color.Navy;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmFormAutoGroup";
            this.Text = "FrmFormAutoGroup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmFormAutoGroup_FormClosing);
            this.Load += new System.EventHandler(this.FrmFormAutoGroup_Load);
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Button Btngiaitan;
        private System.Windows.Forms.Button Btnlapto;
        private System.Windows.Forms.CheckBox Ckbchapnhanvaodoi;
        private System.Windows.Forms.CheckBox Ckbnhantatcaloimoi;
        private System.Windows.Forms.CheckBox Ckbnhaptatca;
        private System.Windows.Forms.CheckBox Ckbmoitatca;
        private System.Windows.Forms.Timer TmrUpdate;
    }
}