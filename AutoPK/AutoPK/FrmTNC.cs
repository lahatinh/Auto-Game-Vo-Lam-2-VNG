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
    public partial class FrmTNC : Form
    {
        public FrmTNC()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        public AutoClientBS CurrentClient { set; get; }
        private bool update = true;

        //Thủ tục load Form
        private void FrmTNC_Load(object sender, EventArgs e)
        {

        }

        // Thủ tục lưu CheckBox
        private void CkbPheTong_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckphetong = CkbPheTong.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTNC", "AutoTNC.Check5", CurrentClient.ckphetong.ToString());
            }
        }

        // Thủ tục lưu CheckBox
        private void Ckbtutranv_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.cktutranv = Ckbtutranv.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTNC", "AutoTNC.Check2", CurrentClient.cktutranv.ToString());
            }
        }

        // Thủ tục lưu CheckBox
        private void Ckbtumuadungcu_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.cktumuadungcu = Ckbtumuadungcu.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTNC", "AutoTNC.Check3", CurrentClient.cktumuadungcu.ToString());
            }
        }

        // Thủ tục lựa chọn quân công để trả nhiệm vụ
        private void Cobquancong_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ValueQuancong = Cobquancong.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTNC", "ValueQuancong", CurrentClient.ValueQuancong.ToString());
            }
        }

        // Thủ tục button lưu vị trí đào
        private void BtnluuvitridaoTNC_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ToadovitridaoTNC[0] = CurrentClient.CurrentPlayer.PositionX;
                CurrentClient.ToadovitridaoTNC[1] = CurrentClient.CurrentPlayer.PositionY;
            }
        }
        //Thủ tục tạm dừng Timer Update Form
        private void Cobquancong_Click(object sender, EventArgs e)
        {
            update = false;
        }

        // Thủ tục Update Form
        private void TmrUpdateForm_Tick(object sender, EventArgs e)
        {
            if (CurrentClient != null && update == true)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

                CkbPheTong.Checked = CurrentClient.ckphetong;
                Ckbtumuadungcu.Checked = CurrentClient.cktumuadungcu;
                Ckbtutranv.Checked = CurrentClient.cktutranv;
                Cobquancong.Text = CurrentClient.ValueQuancong;
            }
        }

        // Thủ tục giải phóng bộ nhớ khi đóng Form
        private void FrmTNC_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
    }
}
