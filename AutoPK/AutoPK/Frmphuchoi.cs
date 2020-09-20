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
    public partial class Frmphuchoi : Form
    {
        public Frmphuchoi()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        public AutoClientBS CurrentClient { set; get; }
        private bool update = true;

        // Thủ tục Load Form
        private void Frmphuchoi_Load(object sender, EventArgs e)
        {
            if (CurrentClient != null && update == true)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

                CkbBommau.Checked = CurrentClient.ckbommau;
                Ckbansinhnoi.Checked = CurrentClient.ckbomsinhnoi;
                CkbBommana.Checked = CurrentClient.ckbommana;
                CkbAnmauCC.Checked = CurrentClient.ckancc;
                CkbAnLakCtr.Checked = CurrentClient.ckanlakctr;
                Ckbanbanhngo.Checked = CurrentClient.ckantheluc;

                txbHPuse.Text = CurrentClient.HPuse.ToString();
                txbCCuse.Text = CurrentClient.CCuse.ToString();
                txbDelay.Text = CurrentClient.Delay.ToString();
                txbSinhnoiUse.Text = CurrentClient.SinhnoiUse.ToString();
                txbManaUse.Text = CurrentClient.ManaUse.ToString();
                txbLakCTUse.Text = CurrentClient.LakCTUse.ToString();
                txbThelucUse.Text = CurrentClient.ThelucUse.ToString();
                CobMau.Text = CurrentClient.ItemSinhLuc.ToString();
                CobSinhNoi.Text = CurrentClient.ItemSinhNoi.ToString();
                CobMana.Text = CurrentClient.ItemNoiLuc.ToString();
                CobCuuChuyen.Text = CurrentClient.ItemCuuChuyen.ToString();
                CobLakCtr.Text = CurrentClient.ItemLakCtr.ToString();
                CobTheLuc.Text = CurrentClient.ItemTheLuc.ToString();

            }
        }

        // Thủ tục lưu Checkbox
        private void CkbBommau_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckbommau = CkbBommau.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckPhucHoi", "CheckPhucHoi.Check2", CurrentClient.ckbommau.ToString());
            }
        }

        // Thủ tục lưu Checkbox
        private void Ckbansinhnoi_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckbomsinhnoi = Ckbansinhnoi.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckPhucHoi", "CheckPhucHoi.Check3", CurrentClient.ckbomsinhnoi.ToString());
            }
        }

        // Thủ tục lưu Checkbox
        private void CkbBommana_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckbommana = CkbBommana.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckPhucHoi", "CheckPhucHoi.Check4", CurrentClient.ckbommana.ToString());
            }
        }

        // Thủ tục lưu Checkbox
        private void CkbAnmauCC_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckancc = CkbAnmauCC.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckPhucHoi", "CheckPhucHoi.Check5", CurrentClient.ckancc.ToString());
            }
        }

        // Thủ tục lưu Checkbox
        private void CkbAnLakCtr_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckanlakctr = CkbAnLakCtr.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckPhucHoi", "CheckPhucHoi.Check6", CurrentClient.ckanlakctr.ToString());
            }
        }

        // Thủ tục lưu Checkbox
        private void Ckbanbanhngo_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckantheluc = Ckbanbanhngo.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckPhucHoi", "CheckPhucHoi.Check7", CurrentClient.ckantheluc.ToString());
            }
        }

        // Thủ tục lựa chọn giá trị ComboBox
        private void CobMau_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ItemSinhLuc = CobMau.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "ItemPhucHoi", "ItemSinhLuc", CurrentClient.ItemSinhLuc.ToString());
            }
        }

        // Thủ tục lựa chọn giá trị ComboBox
        private void CobSinhNoi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ItemSinhNoi = CobSinhNoi.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "ItemPhucHoi", "ItemSinhNoi", CurrentClient.ItemSinhNoi.ToString());
            }
        }

        // Thủ tục lựa chọn giá trị ComboBox
        private void CobMana_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ItemNoiLuc = CobMana.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "ItemPhucHoi", "ItemNoiLuc", CurrentClient.ItemNoiLuc.ToString());
            }
        }

        // Thủ tục lựa chọn giá trị ComboBox
        private void CobCuuChuyen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ItemCuuChuyen = CobCuuChuyen.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "ItemPhucHoi", "ItemCuuChuyen", CurrentClient.ItemCuuChuyen.ToString());
            }
        }

        // Thủ tục lựa chọn giá trị ComboBox
        private void CobLakCtr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ItemLakCtr = CobLakCtr.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "ItemPhucHoi", "ItemLakCtr", CurrentClient.ItemLakCtr.ToString());
            }
        }

        // Thủ tục lựa chọn giá trị ComboBox
        private void CobTheLuc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ItemTheLuc = CobTheLuc.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "ItemPhucHoi", "ItemTheLuc", CurrentClient.ItemTheLuc.ToString());
            }
        }

        // Thủ tục thoát khỏi Textbox
        private void txbHPuse_Validated(object sender, EventArgs e)
        {
            if (txbHPuse.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbHPuse.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.HPuse = Convert.ToInt32(txbHPuse.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                    WinAPI.Ghifile(text, "PhucHoi", "HPuse", CurrentClient.HPuse.ToString());
                }
            }
        }

        // Thủ tục thoát khỏi Textbox
        private void txbSinhnoiUse_Validated(object sender, EventArgs e)
        {
            if (txbSinhnoiUse.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbSinhnoiUse.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.SinhnoiUse = Convert.ToInt32(txbSinhnoiUse.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                    WinAPI.Ghifile(text, "PhucHoi", "SinhnoiUse", CurrentClient.SinhnoiUse.ToString());
                }
            }
        }

        // Thủ tục thoát khỏi Textbox
        private void txbManaUse_Validated(object sender, EventArgs e)
        {
            if (txbManaUse.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbManaUse.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.ManaUse = Convert.ToInt32(txbManaUse.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                    WinAPI.Ghifile(text, "PhucHoi", "ManaUse", CurrentClient.ManaUse.ToString());
                }
            }
        }

        // Thủ tục thoát khỏi Textbox
        private void txbCCuse_Validated(object sender, EventArgs e)
        {
            if (txbCCuse.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbManaUse.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.CCuse = Convert.ToInt32(txbCCuse.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                    WinAPI.Ghifile(text, "PhucHoi", "CCuse", CurrentClient.CCuse.ToString());
                }
            }
        }

        // Thủ tục thoát khỏi Textbox
        private void txbLakCTUse_Validated(object sender, EventArgs e)
        {
            if (txbLakCTUse.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbManaUse.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.LakCTUse = Convert.ToInt32(txbLakCTUse.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                    WinAPI.Ghifile(text, "PhucHoi", "LakCTUse", CurrentClient.LakCTUse.ToString());
                }
            }
        }

        // Thủ tục thoát khỏi Textbox
        private void txbThelucUse_Validated(object sender, EventArgs e)
        {
            if (txbThelucUse.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbThelucUse.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.ThelucUse = Convert.ToInt32(txbThelucUse.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                    WinAPI.Ghifile(text, "PhucHoi", "ThelucUse", CurrentClient.ThelucUse.ToString());
                }
            }
        }

        // Thủ tục thoát khỏi Textbox
        private void txbDelay_Validated(object sender, EventArgs e)
        {
            if (txbDelay.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbDelay.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.Delay = Convert.ToInt32(txbDelay.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                    WinAPI.Ghifile(text, "PhucHoi", "Delay", CurrentClient.Delay.ToString());
                }
            }
        }

        // Thủ tục gõ phím trên textbox
        private void txbHPuse_KeyPress(object sender, KeyPressEventArgs e)
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

        // Thủ tục gõ phím trên textbox
        private void txbSinhnoiUse_KeyPress(object sender, KeyPressEventArgs e)
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

        // Thủ tục gõ phím trên textbox
        private void txbManaUse_KeyPress(object sender, KeyPressEventArgs e)
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

        // Thủ tục gõ phím trên textbox
        private void txbCCuse_KeyPress(object sender, KeyPressEventArgs e)
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

        // Thủ tục gõ phím trên textbox
        private void txbLakCTUse_KeyPress(object sender, KeyPressEventArgs e)
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

        // Thủ tục gõ phím trên textbox
        private void txbThelucUse_KeyPress(object sender, KeyPressEventArgs e)
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

        // Thủ tục gõ phím trên textbox
        private void txbDelay_KeyPress(object sender, KeyPressEventArgs e)
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

        // Thủ tục update Form
        private void TmrUpdate_Tick(object sender, EventArgs e)
        {
            if (CurrentClient != null && update == true)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

                CkbBommau.Checked = CurrentClient.ckbommau;
                Ckbansinhnoi.Checked = CurrentClient.ckbomsinhnoi;
                CkbBommana.Checked = CurrentClient.ckbommana;
                CkbAnmauCC.Checked = CurrentClient.ckancc;
                CkbAnLakCtr.Checked = CurrentClient.ckanlakctr;
                Ckbanbanhngo.Checked = CurrentClient.ckantheluc;

                txbHPuse.Text = CurrentClient.HPuse.ToString();
                txbCCuse.Text = CurrentClient.CCuse.ToString();
                txbDelay.Text = CurrentClient.Delay.ToString();
                txbSinhnoiUse.Text = CurrentClient.SinhnoiUse.ToString();
                txbManaUse.Text = CurrentClient.ManaUse.ToString();
                txbLakCTUse.Text = CurrentClient.LakCTUse.ToString();
                txbThelucUse.Text = CurrentClient.ThelucUse.ToString();
                CobMau.Text = CurrentClient.ItemSinhLuc.ToString();
                CobSinhNoi.Text = CurrentClient.ItemSinhNoi.ToString();
                CobMana.Text = CurrentClient.ItemNoiLuc.ToString();
                CobCuuChuyen.Text = CurrentClient.ItemCuuChuyen.ToString();
                CobLakCtr.Text = CurrentClient.ItemLakCtr.ToString();
                CobTheLuc.Text = CurrentClient.ItemTheLuc.ToString();

            }
        }

        //Tạm dừng Update Form
        private void CobMau_Click(object sender, EventArgs e)
        {
            update = false;
        }

        //Tạm dừng Update Form
        private void CobSinhNoi_Click(object sender, EventArgs e)
        {
            update = false;
        }

        //Tạm dừng Update Form
        private void CobMana_Click(object sender, EventArgs e)
        {
            update = false;
        }

        //Tạm dừng Update Form
        private void CobCuuChuyen_Click(object sender, EventArgs e)
        {
            update = false;
        }

        //Tạm dừng Update Form
        private void CobLakCtr_Click(object sender, EventArgs e)
        {
            update = false;
        }

        //Tạm dừng Update Form
        private void CobTheLuc_Click(object sender, EventArgs e)
        {
            update = false;
        }

        // Thủ tục giải phóng bộ nhớ khi đóng Form
        private void Frmphuchoi_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
    }
}
