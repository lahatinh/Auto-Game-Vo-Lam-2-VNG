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
    public partial class FrmComboTLQ : Form
    {
        public FrmComboTLQ()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        public AutoClientBS CurrentClient {set; get; }
        private bool update = true;

        // Thủ tục Load Form
        private void FrmComboTLQ_Load(object sender, EventArgs e)
        {
            if (CurrentClient != null && update == true)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

                CkbkngatVAC.Checked = CurrentClient.cksutkngatvac;
                CkbxdameVAC.Checked = CurrentClient.ckxdamevac;
                CkbngatVAC.Checked = CurrentClient.cksutngatvac;
                CkbVACDT.Checked = CurrentClient.ckcombodt;
                Ckbvaccdt.Checked = CurrentClient.ckvaccdt;
                CkbĐT1o.Checked = CurrentClient.ckdt1o;

                // Lấy giá trị vào Project
                txbtimeVAC.Text = CurrentClient.timevac.ToString();
                txbtimeDT.Text = CurrentClient.timedt.ToString();
                txbtimecdt.Text = CurrentClient.timecdt.ToString();
                txbtimerunVAC.Text = CurrentClient.timerun.ToString();

                // Keyboard
                cbphimdung.Text = CurrentClient.phimdung.ToString();
                cbphimXVAC.Text = CurrentClient.phimvac.ToString();
                cbphimVACDT.Text = CurrentClient.phimvacdt.ToString();
                cbvaccdt.Text = CurrentClient.phimvaccdt.ToString();
                cbphimDT1o.Text = CurrentClient.phimdt1o.ToString();
            }
        }

        // Thủ tục Lưu CheckBox
        private void CkbxdameVAC_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckxdamevac = CkbxdameVAC.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckComboTLQ", "CheckComboTLQ.Check3", CurrentClient.ckxdamevac.ToString());
            }
        }

        // Thủ tục Lưu CheckBox
        private void CkbngatVAC_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.cksutngatvac = CkbngatVAC.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckComboTLQ", "CheckComboTLQ.Check4", CurrentClient.cksutngatvac.ToString());
            }
        }

        // Thủ tục Lưu CheckBox
        private void CkbVACDT_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckcombodt = CkbVACDT.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckComboTLQ", "CheckComboTLQ.Check5", CurrentClient.ckcombodt.ToString());
            }
        }

        // Thủ tục Lưu CheckBox
        private void Ckbvaccdt_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckvaccdt = Ckbvaccdt.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckComboTLQ", "CheckComboTLQ.Check6", CurrentClient.ckvaccdt.ToString());
            }
        }

        // Thủ tục Lưu CheckBox
        private void CkbĐT1o_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckdt1o = CkbĐT1o.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckComboTLQ", "CheckComboTLQ.Check7", CurrentClient.ckdt1o.ToString());
            }
        }

        // Thủ tục Lưu CheckBox
        private void CkbkngatVAC_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.cksutkngatvac = CkbkngatVAC.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckComboTLQ", "CheckComboTLQ.Check2", CurrentClient.cksutkngatvac.ToString());
            }
        }

        // Thủ tục Click chuột vào Checkbox
        private void CkbxdameVAC_Click(object sender, EventArgs e)
        {
            CkbngatVAC.Checked = false;
            CkbkngatVAC.Checked = false;
        }

        // Thủ tục Click chuột vào Checkbox
        private void CkbngatVAC_Click(object sender, EventArgs e)
        {
            CkbxdameVAC.Checked = false;
            CkbkngatVAC.Checked = false;
        }

        // Thủ tục Click chuột vào Checkbox
        private void CkbkngatVAC_Click(object sender, EventArgs e)
        {
            CkbxdameVAC.Checked = false;
            CkbngatVAC.Checked = false;
        }

        // Thủ tục thoát khỏi textbox
        private void txbtimeVAC_Validated(object sender, EventArgs e)
        {
            if (txbtimeVAC.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbtimeVAC.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.timevac = Convert.ToInt32(txbtimeVAC.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                    WinAPI.Ghifile(text, "ComboTLQ", "timevac", CurrentClient.timevac.ToString());
                }
            }
        }

        // Thủ tục thoát khỏi textbox
        private void txbtimeDT_Validated(object sender, EventArgs e)
        {
            if (txbtimeDT.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbtimeDT.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {

                    CurrentClient.timedt = Convert.ToInt32(txbtimeDT.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                    WinAPI.Ghifile(text, "ComboTLQ", "timedt", CurrentClient.timedt.ToString());
                }
            }
        }

        // Thủ tục thoát khỏi textbox
        private void txbtimecdt_Validated(object sender, EventArgs e)
        {
            if (txbtimecdt.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbtimecdt.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {

                    CurrentClient.timecdt = Convert.ToInt32(txbtimecdt.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                    WinAPI.Ghifile(text, "ComboTLQ", "timecdt", CurrentClient.timecdt.ToString());
                }
            }
        }

        // Thủ tục thoát khỏi textbox
        private void txbtimerunVAC_Validated(object sender, EventArgs e)
        {
            if (txbtimecdt.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbtimecdt.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.timerun = Convert.ToInt32(txbtimerunVAC.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                    WinAPI.Ghifile(text, "ComboTLQ", "timerun", CurrentClient.timerun.ToString());
                }
            }
        }

        // Thủ tục gõ phím trên Textbox
        private void txbtimeVAC_KeyPress(object sender, KeyPressEventArgs e)
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

        // Thủ tục gõ phím trên Textbox
        private void txbtimeDT_KeyPress(object sender, KeyPressEventArgs e)
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

        // Thủ tục gõ phím trên Textbox
        private void txbtimecdt_KeyPress(object sender, KeyPressEventArgs e)
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

        // Thủ tục gõ phím trên Textbox
        private void txbtimerunVAC_KeyPress(object sender, KeyPressEventArgs e)
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

        // Thủ tục lựa chọn phím Combobox
        private void cbphimdung_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.phimdung = cbphimdung.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "KeyBoard", "phimdung", CurrentClient.phimdung.ToString());
            }
        }

        // Thủ tục lựa chọn phím Combobox
        private void cbphimXVAC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.phimvac = cbphimXVAC.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "KeyBoard", "phimvac", CurrentClient.phimvac.ToString());
            }
        }

        // Thủ tục lựa chọn phím Combobox
        private void cbphimVACDT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.phimvacdt = cbphimVACDT.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "KeyBoard", "phimvacdt", CurrentClient.phimvacdt.ToString());
            }
        }

        // Thủ tục lựa chọn phím Combobox
        private void cbphimDT1o_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.phimdt1o = cbphimDT1o.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "KeyBoard", "phimdt1o", CurrentClient.phimdt1o.ToString());
            }
        }

        // Thủ tục lựa chọn phím Combobox
        private void cbvaccdt_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.phimvaccdt = cbvaccdt.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "KeyBoard", "phimvaccdt", CurrentClient.phimvaccdt.ToString());
            }
        }

        // Thủ tục Update Form
        private void TmrUpdate_Tick(object sender, EventArgs e)
        {
            if (CurrentClient != null && update == true)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

                CkbkngatVAC.Checked = CurrentClient.cksutkngatvac;
                CkbxdameVAC.Checked = CurrentClient.ckxdamevac;
                CkbngatVAC.Checked = CurrentClient.cksutngatvac;
                CkbVACDT.Checked = CurrentClient.ckcombodt;
                Ckbvaccdt.Checked = CurrentClient.ckvaccdt;
                CkbĐT1o.Checked = CurrentClient.ckdt1o;

                // Lấy giá trị vào Project
                txbtimeVAC.Text = CurrentClient.timevac.ToString();
                txbtimeDT.Text = CurrentClient.timedt.ToString();
                txbtimecdt.Text = CurrentClient.timecdt.ToString();
                txbtimerunVAC.Text = CurrentClient.timerun.ToString();

                // Keyboard
                cbphimdung.Text = CurrentClient.phimdung.ToString();
                cbphimXVAC.Text = CurrentClient.phimvac.ToString();
                cbphimVACDT.Text = CurrentClient.phimvacdt.ToString();
                cbvaccdt.Text = CurrentClient.phimvaccdt.ToString();
                cbphimDT1o.Text = CurrentClient.phimdt1o.ToString();
            }
        }

        // Thủ tục tạm dừng Update Form
        private void cbphimdung_Click(object sender, EventArgs e)
        {
            update = false;
        }

        // Thủ tục tạm dừng Update Form
        private void cbphimXVAC_Click(object sender, EventArgs e)
        {
            update = false;
        }

        // Thủ tục tạm dừng Update Form
        private void cbphimVACDT_Click(object sender, EventArgs e)
        {
            update = false;
        }

        // Thủ tục tạm dừng Update Form
        private void cbphimDT1o_Click(object sender, EventArgs e)
        {
            update = false;
        }

        // Thủ tục tạm dừng Update Form
        private void cbvaccdt_Click(object sender, EventArgs e)
        {
            update = false;
        }

        // Thủ tục giải phóng bộ nhớ khi đóng Form
        private void FrmComboTLQ_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
    }
}
