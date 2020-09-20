using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using AutoClient;
using AutoClient.Entities;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;

namespace AutoPK
{
    public partial class FromAutoBuff : Form
    {
        public FromAutoBuff()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        public AutoClientBS CurrentClient { set; get; }
        private bool update = true;



        // Thủ tục cập nhật nhân vật danh sách buff
        private void btncapnhat_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                update = false;

                CurrentClient.RefreshEntityList();

                cbbuff.DataSource = (from entity in CurrentClient.EntityList where entity.EntityType == NPCType.Player && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 500 orderby entity.EntityNameNoMark select entity.EntityNameNoMark).ToList();
            }
        }

        // Thủ tục thêm nhân vật vào danh sách buff
        private void btnthem_Click(object sender, EventArgs e)
        {
                update = false;

                if (cbbuff.Text.ToString() == "" || cbbuff.Text.ToString() == null)
                {
                    MessageBox.Show("Bạn phải ấn cập nhật trước!!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    for (int i = 0; i < lstbuff.Items.Count; i++)
                    {
                        if ((lstbuff.Items[i].ToString() == "" || lstbuff.Items[i].ToString() == null) && !lstbuff.Items.Contains(cbbuff.Text.ToString()))
                        lstbuff.Items[i] = cbbuff.Text.ToString();
                    }

                    List<string> List1 = new List<string>(7);

                    foreach (string item in lstbuff.Items)
                    {
                        List1.Add(item);
                    }

                    CurrentClient.Listbuff = List1;

                    for (int k = 0; k < CurrentClient.Listbuff.Count; k++)
                    {
                        string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                        string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                        WinAPI.Ghifile(text, "Listbuff", "PlayerBuff" + k, CurrentClient.Listbuff[k].ToString());

                        if(k == CurrentClient.Listbuff.Count - 1)
                        {
                        update = true;
                        }
                    }

            }


        }

        // Thủ tục xóa nhân vật trong danh sách buff
        private void btnxoa_Click(object sender, EventArgs e)
        {
            update = false;

            if (lstbuff.SelectedItems.Count != 0)
            {
                if (lstbuff.SelectedIndex > -1)
                {
                    for (int i = 0; i < lstbuff.Items.Count; i++)
                    {
                        if (lstbuff.SelectedIndex == i)
                        {
                            string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                            string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                            CurrentClient.Listbuff.RemoveAt(lstbuff.SelectedIndex);
                            WinAPI.Ghifile(text, "ListBuff", "PlayerBuff" + i, "");
                        }

                    }

                }

                update = true;
            }
        }

        // Thủ tục cập nhật danh sách buff
        private void btnCapnhatPlayer_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                update = false;

                CurrentClient.RefreshEntityList();

                CobListPlayer.DataSource = (from entity in CurrentClient.EntityList where entity.EntityType == NPCType.Player && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 500 orderby entity.EntityNameUnicode select entity.EntityNameUnicode).ToList();
            }
        }

        // Thủ tục checkbox buff theo danh sách
        private void cbbuffdanhsach_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.buffdanhsach = cbbuffdanhsach.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckBuff", "CheckBuff.Check3", CurrentClient.buffdanhsach.ToString());
            }
        }

        // Thủ tục checkbox theo sau
        private void Ckbtheosau_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.Bufftheosau = Ckbtheosau.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckBuff", "CheckBuff.Check2", CurrentClient.Bufftheosau.ToString());
            }
        }

        // Thủ tục lựa chọn nhân vật theo sau
        private void CobListPlayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentClient.PlayerTheoSau = CobListPlayer.Text.ToString();
        }

        // Thủ tục LoadForm
        private void FromAutoBuff_Load(object sender, EventArgs e)
        {
            if (CurrentClient != null && update == true && CurrentClient.Listbuff != null)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();
                Ckbtheosau.Checked = CurrentClient.Bufftheosau;
                cbbuffdanhsach.Checked = CurrentClient.buffdanhsach;
                CobListPlayer.Text = CurrentClient.PlayerTheoSau;

                lstbuff.Items.Clear();
                foreach (var Items in CurrentClient.Listbuff)
                {
                    lstbuff.Items.Add(Items);
                }
            }
        }

        private void lstbuff_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbbuff.Invoke(new MethodInvoker(() =>
            {
                update = false;

                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;

                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";

                for (int i = 0; i < 6; i++)
                {
                    WinAPI.Ghifile(text, "Listbuff", "PlayerBuff" + i, CurrentClient.Listbuff[i].ToString());

                    if (CurrentClient.Listbuff[i].ToString() == "")
                        break;
                }
            }));
        }

        //Timer Cập nhật Form

        private void TmrUpdate_Tick(object sender, EventArgs e)
        {
            if(CurrentClient!=null && update == true && CurrentClient.Listbuff != null)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();
                Ckbtheosau.Checked = CurrentClient.Bufftheosau;
                cbbuffdanhsach.Checked = CurrentClient.buffdanhsach;
                CobListPlayer.Text = CurrentClient.PlayerTheoSau;

                lstbuff.Items.Clear();
                foreach(var Items in CurrentClient.Listbuff)
                {
                    lstbuff.Items.Add(Items);
                }
            }

        }

        // Thủ tục giải phóng bộ nhớ khi đóng Form
        private void FromAutoBuff_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }

        // Thủ tục tạm dừng update
        private void lstbuff_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            update = false;
        }
    }
}
