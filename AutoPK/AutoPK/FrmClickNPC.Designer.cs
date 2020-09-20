namespace AutoPK
{
    partial class FrmClickNPC
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
            System.Windows.Forms.Timer TmrUpdate;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmClickNPC));
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.TxbGiancach = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.BtnResetMenu4 = new System.Windows.Forms.Button();
            this.BtnResetMenu3 = new System.Windows.Forms.Button();
            this.BtnResetMenu2 = new System.Windows.Forms.Button();
            this.BtnResetMenu1 = new System.Windows.Forms.Button();
            this.BtnResetNPC = new System.Windows.Forms.Button();
            this.CobListNPC = new System.Windows.Forms.ComboBox();
            this.CobListMenu4 = new System.Windows.Forms.ComboBox();
            this.CobListMenu3 = new System.Windows.Forms.ComboBox();
            this.Ckbdoithoai1 = new System.Windows.Forms.CheckBox();
            this.Ckbdoithoai4 = new System.Windows.Forms.CheckBox();
            this.Ckbdoithoai2 = new System.Windows.Forms.CheckBox();
            this.CobListMenu2 = new System.Windows.Forms.ComboBox();
            this.CobListMenu1 = new System.Windows.Forms.ComboBox();
            this.Ckbdoithoai3 = new System.Windows.Forms.CheckBox();
            TmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // TmrUpdate
            // 
            TmrUpdate.Enabled = true;
            TmrUpdate.Interval = 1500;
            TmrUpdate.Tick += new System.EventHandler(this.TmrUpdate_Tick);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label16);
            this.groupBox6.Controls.Add(this.TxbGiancach);
            this.groupBox6.Controls.Add(this.label1);
            this.groupBox6.Controls.Add(this.label17);
            this.groupBox6.Controls.Add(this.BtnResetMenu4);
            this.groupBox6.Controls.Add(this.BtnResetMenu3);
            this.groupBox6.Controls.Add(this.BtnResetMenu2);
            this.groupBox6.Controls.Add(this.BtnResetMenu1);
            this.groupBox6.Controls.Add(this.BtnResetNPC);
            this.groupBox6.Controls.Add(this.CobListNPC);
            this.groupBox6.Controls.Add(this.CobListMenu4);
            this.groupBox6.Controls.Add(this.CobListMenu3);
            this.groupBox6.Controls.Add(this.Ckbdoithoai1);
            this.groupBox6.Controls.Add(this.Ckbdoithoai4);
            this.groupBox6.Controls.Add(this.Ckbdoithoai2);
            this.groupBox6.Controls.Add(this.CobListMenu2);
            this.groupBox6.Controls.Add(this.CobListMenu1);
            this.groupBox6.Controls.Add(this.Ckbdoithoai3);
            this.groupBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox6.ForeColor = System.Drawing.Color.Navy;
            this.groupBox6.Location = new System.Drawing.Point(5, 5);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(222, 197);
            this.groupBox6.TabIndex = 12;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Click NPC";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label16.Location = new System.Drawing.Point(193, 169);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(24, 15);
            this.label16.TabIndex = 12;
            this.label16.Text = "ms";
            // 
            // TxbGiancach
            // 
            this.TxbGiancach.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.TxbGiancach.ForeColor = System.Drawing.Color.Navy;
            this.TxbGiancach.Location = new System.Drawing.Point(99, 166);
            this.TxbGiancach.Name = "TxbGiancach";
            this.TxbGiancach.Size = new System.Drawing.Size(93, 21);
            this.TxbGiancach.TabIndex = 13;
            this.TxbGiancach.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxbGiancach_KeyPress);
            this.TxbGiancach.Validated += new System.EventHandler(this.TxbGiancach_Validated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label1.Location = new System.Drawing.Point(5, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 13;
            this.label1.Text = "Chọn NPC:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label17.Location = new System.Drawing.Point(5, 168);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(65, 15);
            this.label17.TabIndex = 13;
            this.label17.Text = "Giãn cách:";
            // 
            // BtnResetMenu4
            // 
            this.BtnResetMenu4.ForeColor = System.Drawing.Color.Navy;
            this.BtnResetMenu4.Location = new System.Drawing.Point(99, 137);
            this.BtnResetMenu4.Name = "BtnResetMenu4";
            this.BtnResetMenu4.Size = new System.Drawing.Size(22, 23);
            this.BtnResetMenu4.TabIndex = 12;
            this.BtnResetMenu4.Text = "R";
            this.BtnResetMenu4.UseVisualStyleBackColor = true;
            this.BtnResetMenu4.Click += new System.EventHandler(this.BtnResetMenu4_Click);
            // 
            // BtnResetMenu3
            // 
            this.BtnResetMenu3.ForeColor = System.Drawing.Color.Navy;
            this.BtnResetMenu3.Location = new System.Drawing.Point(99, 108);
            this.BtnResetMenu3.Name = "BtnResetMenu3";
            this.BtnResetMenu3.Size = new System.Drawing.Size(22, 23);
            this.BtnResetMenu3.TabIndex = 12;
            this.BtnResetMenu3.Text = "R";
            this.BtnResetMenu3.UseVisualStyleBackColor = true;
            this.BtnResetMenu3.Click += new System.EventHandler(this.BtnResetMenu3_Click);
            // 
            // BtnResetMenu2
            // 
            this.BtnResetMenu2.ForeColor = System.Drawing.Color.Navy;
            this.BtnResetMenu2.Location = new System.Drawing.Point(99, 79);
            this.BtnResetMenu2.Name = "BtnResetMenu2";
            this.BtnResetMenu2.Size = new System.Drawing.Size(22, 23);
            this.BtnResetMenu2.TabIndex = 12;
            this.BtnResetMenu2.Text = "R";
            this.BtnResetMenu2.UseVisualStyleBackColor = true;
            this.BtnResetMenu2.Click += new System.EventHandler(this.BtnResetMenu2_Click);
            // 
            // BtnResetMenu1
            // 
            this.BtnResetMenu1.ForeColor = System.Drawing.Color.Navy;
            this.BtnResetMenu1.Location = new System.Drawing.Point(99, 49);
            this.BtnResetMenu1.Name = "BtnResetMenu1";
            this.BtnResetMenu1.Size = new System.Drawing.Size(22, 23);
            this.BtnResetMenu1.TabIndex = 12;
            this.BtnResetMenu1.Text = "R";
            this.BtnResetMenu1.UseVisualStyleBackColor = true;
            this.BtnResetMenu1.Click += new System.EventHandler(this.BtnResetMenu1_Click);
            // 
            // BtnResetNPC
            // 
            this.BtnResetNPC.ForeColor = System.Drawing.Color.Navy;
            this.BtnResetNPC.Location = new System.Drawing.Point(99, 21);
            this.BtnResetNPC.Name = "BtnResetNPC";
            this.BtnResetNPC.Size = new System.Drawing.Size(22, 23);
            this.BtnResetNPC.TabIndex = 12;
            this.BtnResetNPC.Text = "R";
            this.BtnResetNPC.UseVisualStyleBackColor = true;
            this.BtnResetNPC.Click += new System.EventHandler(this.BtnResetNPC_Click);
            // 
            // CobListNPC
            // 
            this.CobListNPC.ForeColor = System.Drawing.Color.Navy;
            this.CobListNPC.FormattingEnabled = true;
            this.CobListNPC.Location = new System.Drawing.Point(127, 21);
            this.CobListNPC.Name = "CobListNPC";
            this.CobListNPC.Size = new System.Drawing.Size(88, 23);
            this.CobListNPC.TabIndex = 10;
            this.CobListNPC.SelectedIndexChanged += new System.EventHandler(this.CobListNPC_SelectedIndexChanged);
            // 
            // CobListMenu4
            // 
            this.CobListMenu4.ForeColor = System.Drawing.Color.Navy;
            this.CobListMenu4.FormattingEnabled = true;
            this.CobListMenu4.Location = new System.Drawing.Point(127, 137);
            this.CobListMenu4.Name = "CobListMenu4";
            this.CobListMenu4.Size = new System.Drawing.Size(88, 23);
            this.CobListMenu4.TabIndex = 10;
            this.CobListMenu4.SelectedIndexChanged += new System.EventHandler(this.CobListMenu4_SelectedIndexChanged);
            // 
            // CobListMenu3
            // 
            this.CobListMenu3.ForeColor = System.Drawing.Color.Navy;
            this.CobListMenu3.FormattingEnabled = true;
            this.CobListMenu3.Location = new System.Drawing.Point(127, 108);
            this.CobListMenu3.Name = "CobListMenu3";
            this.CobListMenu3.Size = new System.Drawing.Size(88, 23);
            this.CobListMenu3.TabIndex = 10;
            this.CobListMenu3.SelectedIndexChanged += new System.EventHandler(this.CobListMenu3_SelectedIndexChanged);
            // 
            // Ckbdoithoai1
            // 
            this.Ckbdoithoai1.AutoSize = true;
            this.Ckbdoithoai1.Location = new System.Drawing.Point(6, 54);
            this.Ckbdoithoai1.Name = "Ckbdoithoai1";
            this.Ckbdoithoai1.Size = new System.Drawing.Size(88, 19);
            this.Ckbdoithoai1.TabIndex = 9;
            this.Ckbdoithoai1.Text = "Đối thoại 1:";
            this.Ckbdoithoai1.UseVisualStyleBackColor = true;
            this.Ckbdoithoai1.CheckedChanged += new System.EventHandler(this.Ckbdoithoai1_CheckedChanged);
            // 
            // Ckbdoithoai4
            // 
            this.Ckbdoithoai4.AutoSize = true;
            this.Ckbdoithoai4.Location = new System.Drawing.Point(6, 140);
            this.Ckbdoithoai4.Name = "Ckbdoithoai4";
            this.Ckbdoithoai4.Size = new System.Drawing.Size(88, 19);
            this.Ckbdoithoai4.TabIndex = 9;
            this.Ckbdoithoai4.Text = "Đối thoại 4:";
            this.Ckbdoithoai4.UseVisualStyleBackColor = true;
            this.Ckbdoithoai4.CheckedChanged += new System.EventHandler(this.Ckbdoithoai4_CheckedChanged);
            // 
            // Ckbdoithoai2
            // 
            this.Ckbdoithoai2.AutoSize = true;
            this.Ckbdoithoai2.Location = new System.Drawing.Point(6, 82);
            this.Ckbdoithoai2.Name = "Ckbdoithoai2";
            this.Ckbdoithoai2.Size = new System.Drawing.Size(88, 19);
            this.Ckbdoithoai2.TabIndex = 9;
            this.Ckbdoithoai2.Text = "Đối thoại 2:";
            this.Ckbdoithoai2.UseVisualStyleBackColor = true;
            this.Ckbdoithoai2.CheckedChanged += new System.EventHandler(this.Ckbdoithoai2_CheckedChanged);
            // 
            // CobListMenu2
            // 
            this.CobListMenu2.ForeColor = System.Drawing.Color.Navy;
            this.CobListMenu2.FormattingEnabled = true;
            this.CobListMenu2.Location = new System.Drawing.Point(127, 79);
            this.CobListMenu2.Name = "CobListMenu2";
            this.CobListMenu2.Size = new System.Drawing.Size(88, 23);
            this.CobListMenu2.TabIndex = 10;
            this.CobListMenu2.SelectedIndexChanged += new System.EventHandler(this.CobListMenu2_SelectedIndexChanged);
            // 
            // CobListMenu1
            // 
            this.CobListMenu1.ForeColor = System.Drawing.Color.Navy;
            this.CobListMenu1.FormattingEnabled = true;
            this.CobListMenu1.Location = new System.Drawing.Point(127, 50);
            this.CobListMenu1.Name = "CobListMenu1";
            this.CobListMenu1.Size = new System.Drawing.Size(88, 23);
            this.CobListMenu1.TabIndex = 10;
            this.CobListMenu1.SelectedIndexChanged += new System.EventHandler(this.CobListMenu1_SelectedIndexChanged);
            // 
            // Ckbdoithoai3
            // 
            this.Ckbdoithoai3.AutoSize = true;
            this.Ckbdoithoai3.Location = new System.Drawing.Point(6, 111);
            this.Ckbdoithoai3.Name = "Ckbdoithoai3";
            this.Ckbdoithoai3.Size = new System.Drawing.Size(88, 19);
            this.Ckbdoithoai3.TabIndex = 9;
            this.Ckbdoithoai3.Text = "Đối thoại 3:";
            this.Ckbdoithoai3.UseVisualStyleBackColor = true;
            this.Ckbdoithoai3.CheckedChanged += new System.EventHandler(this.Ckbdoithoai3_CheckedChanged);
            // 
            // FrmClickNPC
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(234, 208);
            this.Controls.Add(this.groupBox6);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ForeColor = System.Drawing.Color.Navy;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmClickNPC";
            this.Text = "FrmClickNPC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmClickNPC_FormClosing);
            this.Load += new System.EventHandler(this.FrmClickNPC_Load);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox TxbGiancach;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button BtnResetMenu4;
        private System.Windows.Forms.Button BtnResetMenu3;
        private System.Windows.Forms.Button BtnResetMenu2;
        private System.Windows.Forms.Button BtnResetMenu1;
        private System.Windows.Forms.Button BtnResetNPC;
        private System.Windows.Forms.ComboBox CobListNPC;
        private System.Windows.Forms.ComboBox CobListMenu4;
        private System.Windows.Forms.ComboBox CobListMenu3;
        private System.Windows.Forms.CheckBox Ckbdoithoai1;
        private System.Windows.Forms.CheckBox Ckbdoithoai4;
        private System.Windows.Forms.CheckBox Ckbdoithoai2;
        private System.Windows.Forms.ComboBox CobListMenu2;
        private System.Windows.Forms.ComboBox CobListMenu1;
        private System.Windows.Forms.CheckBox Ckbdoithoai3;
    }
}