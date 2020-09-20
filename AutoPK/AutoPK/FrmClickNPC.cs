using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using AutoClient;

namespace AutoPK
{
    public partial class FrmClickNPC : Form
    {
        public FrmClickNPC()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        public AutoClientBS CurrentClient { set; get; }
        private bool update = true;

        //Thủ tục loadForm
        private void FrmClickNPC_Load(object sender, EventArgs e)
        {
            if (CurrentClient != null && update == true)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

                Ckbdoithoai1.Checked = CurrentClient.ckClickMenu1;
                Ckbdoithoai2.Checked = CurrentClient.ckClickMenu2;
                Ckbdoithoai3.Checked = CurrentClient.ckClickMenu3;
                Ckbdoithoai4.Checked = CurrentClient.ckClickMenu4;

                CobListNPC.Text = CurrentClient.ValueClickNPC;
                CobListMenu1.Text = CurrentClient.ValueClickMenu1;
                CobListMenu2.Text = CurrentClient.ValueClickMenu2;
                CobListMenu3.Text = CurrentClient.ValueClickMenu3;
                CobListMenu4.Text = CurrentClient.ValueClickMenu4;
                TxbGiancach.Text = CurrentClient.ValueGiancach.ToString();
            }
        }

        //Thủ tục click nút cập nhật
        private void BtnResetNPC_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                update = false;

                CurrentClient.RefreshEntityList();

                CobListNPC.DataSource = (from entity in CurrentClient.EntityList where entity.EntityType == NPCType.Item && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 500 orderby entity.EntityId select entity.EntityId.ToString() + "/" + entity.EntityNameNoMark.ToString()).ToList();
            }
        }

        //Thủ tục click nút cập nhật
        private void BtnResetMenu1_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                update = false;

                var line = CurrentClient.GetDialogLines();

                CobListMenu1.DataSource = line;
            }
        }

        //Thủ tục click nút cập nhật
        private void BtnResetMenu2_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                update = false;

                var line = CurrentClient.GetDialogLines();

                CobListMenu2.DataSource = line;

            }
        }

        //Thủ tục click nút cập nhật
        private void BtnResetMenu3_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                update = false;

                var line = CurrentClient.GetDialogLines();

                CobListMenu3.DataSource = line;
            }
        }

        //Thủ tục click nút cập nhật
        private void BtnResetMenu4_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                update = false;

                var line = CurrentClient.GetDialogLines();

                CobListMenu4.DataSource = line;
            }
        }

        //Thủ tục click nút cập nhật
        private void Ckbdoithoai1_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckClickMenu1 = Ckbdoithoai1.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.Check2", CurrentClient.ckClickMenu1.ToString());
            }
        }

        //Thủ tục click nút cập nhật
        private void Ckbdoithoai2_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckClickMenu2 = Ckbdoithoai2.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.Check3", CurrentClient.ckClickMenu2.ToString());
            }
        }

        //Thủ tục click nút cập nhật
        private void Ckbdoithoai3_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckClickMenu3 = Ckbdoithoai3.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.Check4", CurrentClient.ckClickMenu3.ToString());
            }
        }

        //Thủ tục click nút cập nhật
        private void Ckbdoithoai4_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckClickMenu4 = Ckbdoithoai4.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.Check5", CurrentClient.ckClickMenu4.ToString());
            }
        }

        //Thủ tục click nút cập nhật
        private void CobListNPC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ValueClickNPC = CobListNPC.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.ValueClickNPC", CurrentClient.ValueClickNPC.ToString());

            }
        }

        //Thủ tục lựa chọn giá trị trong combobox
        private void CobListMenu1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ValueClickMenu1 = CobListMenu1.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.ValueClickMenu1", CurrentClient.ValueClickMenu1.ToString());
            }
        }

        //Thủ tục lựa chọn giá trị trong combobox
        private void CobListMenu2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ValueClickMenu2 = CobListMenu2.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.ValueClickMenu2", CurrentClient.ValueClickMenu2.ToString());
            }
        }

        //Thủ tục lựa chọn giá trị trong combobox
        private void CobListMenu3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ValueClickMenu3 = CobListMenu3.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.ValueClickMenu3", CurrentClient.ValueClickMenu3.ToString());
            }
        }

        //Thủ tục lựa chọn giá trị trong combobox
        private void CobListMenu4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ValueClickMenu4 = CobListMenu4.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.ValueClickMenu4", CurrentClient.ValueClickMenu4.ToString());
            }
        }

        //Thủ tục thoát khỏi texbox
        private void TxbGiancach_Validated(object sender, EventArgs e)
        {
            if (TxbGiancach.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                TxbGiancach.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.ValueGiancach = Convert.ToInt32(TxbGiancach.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                    WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.ValueGiancach", CurrentClient.ValueGiancach.ToString());
                }
            }
        }

        //Thủ tục không cho gõ chữ vào textbox
        private void TxbGiancach_KeyPress(object sender, KeyPressEventArgs e)
        {
            update = false;

            if ("0123456789".IndexOf(e.KeyChar) != -1)
            {
                e.Handled = false;
            }
            else
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        //Timemer Update
        private void TmrUpdate_Tick(object sender, EventArgs e)
        {
            if(CurrentClient!=null && update == true)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

                Ckbdoithoai1.Checked = CurrentClient.ckClickMenu1;
                Ckbdoithoai2.Checked = CurrentClient.ckClickMenu2;
                Ckbdoithoai3.Checked = CurrentClient.ckClickMenu3;
                Ckbdoithoai4.Checked = CurrentClient.ckClickMenu4;

                CobListNPC.Text = CurrentClient.ValueClickNPC;
                CobListMenu1.Text = CurrentClient.ValueClickMenu1;
                CobListMenu2.Text = CurrentClient.ValueClickMenu2;
                CobListMenu3.Text = CurrentClient.ValueClickMenu3;
                CobListMenu4.Text = CurrentClient.ValueClickMenu4;
                TxbGiancach.Text = CurrentClient.ValueGiancach.ToString();
            }
        }

        // Thủ tục giải phóng bộ nhớ khi đóng Form
        private void FrmClickNPC_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
    }
}
