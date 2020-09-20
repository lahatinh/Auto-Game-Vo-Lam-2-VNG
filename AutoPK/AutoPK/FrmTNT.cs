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
    public partial class FrmTNT : Form
    {
        public FrmTNT()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        public AutoClientBS CurrentClient { set; get; }
        private bool update = true;

        // Cập nhật Form
        private void FrmTNT_Load(object sender, EventArgs e)
        {
            if (CurrentClient != null && update == true)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

                Ckbchukey.Checked = CurrentClient.CkChukey;
                Ckbacctreo.Checked = CurrentClient.Ckacctreo;
                CkblattheTKL.Checked = CurrentClient.CklattheTKL;
                CkbmoruongTKL.Checked = CurrentClient.CkmoruongTKL;
                CkbvuotaiTKL.Checked = CurrentClient.CkVuotaiTKL;
            }
        }

        // Thủ tục lưu CheckBox
        private void Ckbchukey_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.CkChukey = Ckbchukey.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTNT", "AutoTNT.Check2", CurrentClient.CkChukey.ToString());
            }
        }

        // Thủ tục lưu CheckBox
        private void Ckbacctreo_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.Ckacctreo = Ckbacctreo.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTNT", "AutoTNT.Check6", CurrentClient.Ckacctreo.ToString());
            }
        }

        // Thủ tục lưu CheckBox
        private void CkbmoruongTKL_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.CkmoruongTKL = CkbmoruongTKL.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTNT", "AutoTNT.Check3", CurrentClient.CkmoruongTKL.ToString());
            }
        }

        // Thủ tục lưu CheckBox
        private void CkbvuotaiTKL_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.CkVuotaiTKL = CkbvuotaiTKL.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTNT", "AutoTNT.Check5", CurrentClient.CkVuotaiTKL.ToString());
            }
        }

        // Thủ tục lưu CheckBox
        private void CkblattheTKL_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.CklattheTKL = CkblattheTKL.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTNT", "AutoTNT.Check4", CurrentClient.CklattheTKL.ToString());
            }
        }

        // Thủ tục Update Form
        private void TmrUpdate_Tick(object sender, EventArgs e)
        {
            if (CurrentClient != null && update == true)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

                Ckbchukey.Checked = CurrentClient.CkChukey;
                Ckbacctreo.Checked = CurrentClient.Ckacctreo;
                CkblattheTKL.Checked = CurrentClient.CklattheTKL;
                CkbmoruongTKL.Checked = CurrentClient.CkmoruongTKL;
                CkbvuotaiTKL.Checked = CurrentClient.CkVuotaiTKL;
            }
        }

        // Thủ tục giải phóng bộ nhớ khi đóng Form
        private void FrmTNT_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
    }
}
