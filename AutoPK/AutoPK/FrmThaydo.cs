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
    public partial class FrmThaydo : Form
    {
        public FrmThaydo()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        public AutoClientBS CurrentClient { set; get; }
        private bool update = true;

        // Thủ tục LoadForm
        private void FrmThaydo_Load(object sender, EventArgs e)
        {
            if (CurrentClient != null && update == true)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

                ckbthaydo1.Checked = CurrentClient.ckthaydobo1;
                ckbthaydo2.Checked = CurrentClient.ckthaydobo2;
                ckbthaydo3.Checked = CurrentClient.ckthaydobo3;
                ckbthaydo4.Checked = CurrentClient.ckthaydobo4;

                cbphimbo1.Text = CurrentClient.phimthaydo1;
                cbphimbo2.Text = CurrentClient.phimthaydo2;
                cbphimbo3.Text = CurrentClient.phimthaydo3;
                cbphimbo4.Text = CurrentClient.phimthaydo4;

                //Lấy danh sách thay đồ 1
                Cbdanhsach1.Items.Clear();

                foreach (string item in CurrentClient.Listthaydo1)
                {
                    Cbdanhsach1.Items.Add(item);
                }

                //Lấy danh sách thay đồ 2
                Cbdanhsach2.Items.Clear();

                foreach (string item in CurrentClient.Listthaydo2)
                {
                    Cbdanhsach2.Items.Add(item);
                }

                //Lấy danh sách thay đồ 3
                Cbdanhsach3.Items.Clear();
                foreach (string item in CurrentClient.Listthaydo3)
                {
                    Cbdanhsach3.Items.Add(item);
                }

                //Lấy danh sách thay đồ 4
                Cbdanhsach4.Items.Clear();
                foreach (string item in CurrentClient.Listthaydo4)
                {
                    Cbdanhsach4.Items.Add(item);
                }
            }
        }

        // Thủ tục lưu CheckBox
        private void ckbthaydo1_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckthaydobo1 = ckbthaydo1.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckThaydo", "CheckThaydo.Check1", CurrentClient.ckthaydobo1.ToString());
            }
        }

        // Thủ tục lưu CheckBox
        private void ckbthaydo2_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckthaydobo2 = ckbthaydo2.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckThaydo", "CheckThaydo.Check2", CurrentClient.ckthaydobo2.ToString());
            }
        }

        // Thủ tục lưu CheckBox
        private void ckbthaydo3_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckthaydobo3 = ckbthaydo3.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckThaydo", "CheckThaydo.Check3", CurrentClient.ckthaydobo3.ToString());
            }
        }

        // Thủ tục lưu CheckBox
        private void ckbthaydo4_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckthaydobo4 = ckbthaydo4.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckThaydo", "CheckThaydo.Check4", CurrentClient.ckthaydobo4.ToString());
            }
        }

        // Thủ tục lưu CheckBox
        private void BtnUpdate1_Click(object sender, EventArgs e)
        {
            update = false;

            if (CurrentClient != null)
            {
                CurrentClient.RefreshStorageItems();

                Cbdanhsach1.DataSource = (from entity in CurrentClient.ItemList where entity.Location == 1 orderby entity.ItemId select entity.ItemOPT.ToString() + "/" + entity.PositionColumn.ToString() + "/" + entity.PositionRow.ToString()).ToList();
                CurrentClient.Listthaydo1 = (from entity in CurrentClient.ItemList where entity.Location == 1 orderby entity.ItemId select entity.ItemOPT.ToString() + "/" + entity.PositionColumn.ToString() + "/" + entity.PositionRow.ToString()).ToList();

                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";

                for (int i = 0; i < CurrentClient.Listthaydo1.Count; i++)
                {
                    WinAPI.Ghifile(text, "Listthaydo1", "Listthaydo1.Item" + i, CurrentClient.Listthaydo1[i].ToString());
                }
            }
        }

        // Thủ tục lưu CheckBox
        private void BtnUpdate2_Click(object sender, EventArgs e)
        {
            update = false;

            if (CurrentClient != null)
            {
                CurrentClient.RefreshStorageItems();

                Cbdanhsach2.DataSource = (from entity in CurrentClient.ItemList where entity.Location == 1 orderby entity.ItemId select entity.ItemOPT.ToString() + "/" + entity.PositionColumn.ToString() + "/" + entity.PositionRow.ToString()).ToList();
                CurrentClient.Listthaydo2 = (from entity in CurrentClient.ItemList where entity.Location == 1 orderby entity.ItemId select entity.ItemOPT.ToString() + "/" + entity.PositionColumn.ToString() + "/" + entity.PositionRow.ToString()).ToList();

                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";

                for (int i = 0; i < CurrentClient.Listthaydo2.Count; i++)
                {
                    WinAPI.Ghifile(text, "Listthaydo2", "Listthaydo2.Item" + i, CurrentClient.Listthaydo2[i].ToString());
                }

            }
        }

        // Thủ tục lưu CheckBox
        private void BtnUpdate3_Click(object sender, EventArgs e)
        {
            update = false;

            if (CurrentClient != null)
            {
                CurrentClient.RefreshStorageItems();

                Cbdanhsach3.DataSource = (from entity in CurrentClient.ItemList where entity.Location == 1 orderby entity.ItemId select entity.ItemOPT.ToString() + "/" + entity.PositionColumn.ToString() + "/" + entity.PositionRow.ToString()).ToList();
                CurrentClient.Listthaydo3 = (from entity in CurrentClient.ItemList where entity.Location == 1 orderby entity.ItemId select entity.ItemOPT.ToString() + "/" + entity.PositionColumn.ToString() + "/" + entity.PositionRow.ToString()).ToList();

                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";

                for (int i = 0; i < CurrentClient.Listthaydo3.Count; i++)
                {
                    WinAPI.Ghifile(text, "Listthaydo3", "Listthaydo3.Item" + i, CurrentClient.Listthaydo3[i].ToString());
                }

            }
        }

        // Thủ tục lưu CheckBox
        private void BtnUpdate4_Click(object sender, EventArgs e)
        {
            update = false;

            if (CurrentClient != null)
            {
                CurrentClient.RefreshStorageItems();

                Cbdanhsach4.DataSource = (from entity in CurrentClient.ItemList where entity.Location == 1 orderby entity.ItemId select entity.ItemOPT.ToString() + "/" + entity.PositionColumn.ToString() + "/" + entity.PositionRow.ToString()).ToList();
                CurrentClient.Listthaydo4 = (from entity in CurrentClient.ItemList where entity.Location == 1 orderby entity.ItemId select entity.ItemOPT.ToString() + "/" + entity.PositionColumn.ToString() + "/" + entity.PositionRow.ToString()).ToList();

                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";

                for (int i = 0; i < CurrentClient.Listthaydo4.Count; i++)
                {
                    WinAPI.Ghifile(text, "Listthaydo4", "Listthaydo4.Item" + i, CurrentClient.Listthaydo4[i].ToString());
                }

            }
        }

        // Thủ tục lựa chọn giá trị Combobox
        private void cbphimbo1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (CurrentClient != null)
            {
                CurrentClient.phimthaydo1 = cbphimbo1.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "KeyBoard", "phimthaydo1", CurrentClient.phimthaydo1.ToString());
            }
        }

        // Thủ tục lựa chọn giá trị Combobox
        private void cbphimbo2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.phimthaydo2 = cbphimbo2.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "KeyBoard", "phimthaydo2", CurrentClient.phimthaydo2.ToString());
            }
        }

        // Thủ tục lựa chọn giá trị Combobox
        private void cbphimbo3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.phimthaydo3 = cbphimbo3.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "KeyBoard", "phimthaydo3", CurrentClient.phimthaydo3.ToString());
            }
        }

        // Thủ tục lựa chọn giá trị Combobox
        private void cbphimbo4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.phimthaydo4 = cbphimbo4.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "KeyBoard", "phimthaydo4", CurrentClient.phimthaydo4.ToString());
            }
        }

        // Thủ tục Update Form
        private void TmrUpdate_Tick(object sender, EventArgs e)
        {
            if (CurrentClient != null && update == true)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

                ckbthaydo1.Checked = CurrentClient.ckthaydobo1;
                ckbthaydo2.Checked = CurrentClient.ckthaydobo2;
                ckbthaydo3.Checked = CurrentClient.ckthaydobo3;
                ckbthaydo4.Checked = CurrentClient.ckthaydobo4;

                cbphimbo1.Text = CurrentClient.phimthaydo1;
                cbphimbo2.Text = CurrentClient.phimthaydo2;
                cbphimbo3.Text = CurrentClient.phimthaydo3;
                cbphimbo4.Text = CurrentClient.phimthaydo4;

                //Lấy danh sách thay đồ 1
                Cbdanhsach1.Items.Clear();

                foreach (string item in CurrentClient.Listthaydo1)
                {
                    Cbdanhsach1.Items.Add(item);
                }

                //Lấy danh sách thay đồ 2
                Cbdanhsach2.Items.Clear();

                foreach (string item in CurrentClient.Listthaydo2)
                {
                    Cbdanhsach2.Items.Add(item);
                }

                //Lấy danh sách thay đồ 3
                Cbdanhsach3.Items.Clear();
                foreach (string item in CurrentClient.Listthaydo3)
                {
                    Cbdanhsach3.Items.Add(item);
                }

                //Lấy danh sách thay đồ 4
                Cbdanhsach4.Items.Clear();
                foreach (string item in CurrentClient.Listthaydo4)
                {
                    Cbdanhsach4.Items.Add(item);
                }
            }
        }

        // Tạm dừng Update Form
        private void cbphimbo1_Click(object sender, EventArgs e)
        {
            update = false;
        }

        // Tạm dừng Update Form
        private void cbphimbo2_Click(object sender, EventArgs e)
        {
            update = false;
        }

        // Tạm dừng Update Form
        private void cbphimbo3_Click(object sender, EventArgs e)
        {
            update = false;
        }

        // Tạm dừng Update Form
        private void cbphimbo4_Click(object sender, EventArgs e)
        {
            update = false;
        }

        private void FrmThaydo_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
    }
}
