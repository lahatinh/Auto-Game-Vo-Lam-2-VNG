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
    public partial class FrmFormAutoGroup : Form
    {
        public FrmFormAutoGroup()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        public AutoClientBS CurrentClient { set; get; }

        //Thủ tục loadForm
        private void FrmFormAutoGroup_Load(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

                //Lấy dữ liệu vào Project
                Ckbmoitatca.Checked = CurrentClient.Ckmoitatca;
                Ckbnhaptatca.Checked = CurrentClient.Cknhaptatca;
                Ckbnhantatcaloimoi.Checked = CurrentClient.Cknhanloimoitodoi;
                Ckbchapnhanvaodoi.Checked = CurrentClient.Ckchapnhanxinnhapdoi;
            }
        }

        //Thủ tục lưu checkbox
        private void Ckbmoitatca_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.Ckmoitatca = Ckbmoitatca.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoGroup", "AutoGroup.Check2", CurrentClient.Ckmoitatca.ToString());
            }
        }

        //Thủ tục lưu checkbox
        private void Ckbnhaptatca_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.Cknhaptatca = Ckbnhaptatca.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoGroup", "AutoGroup.Check3", CurrentClient.Cknhaptatca.ToString());
            }
        }

        //Thủ tục lưu checkbox
        private void Ckbnhantatcaloimoi_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.Cknhanloimoitodoi = Ckbnhantatcaloimoi.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoGroup", "AutoGroup.Check4", CurrentClient.Cknhanloimoitodoi.ToString());
            }
        }

        //Thủ tục lưu checkbox
        private void Ckbchapnhanvaodoi_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.Ckchapnhanxinnhapdoi = Ckbchapnhanvaodoi.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoGroup", "AutoGroup.Check5", CurrentClient.Ckchapnhanxinnhapdoi.ToString());
            }
        }

        //Giải lập đội
        private void Btnlapto_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.laptodoi();
            }
        }

        //Giải tán đội
        private void Btngiaitan_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.giaitantodoi();
            }
        }

        //Lọc điều kiện checkbox
        private void Ckbmoitatca_Click(object sender, EventArgs e)
        {
            Ckbnhaptatca.Checked = false;
            Ckbnhantatcaloimoi.Checked = false;
        }

        //Lọc điều kiện checkbox
        private void Ckbnhaptatca_Click(object sender, EventArgs e)
        {
            Ckbmoitatca.Checked = false;
            Ckbchapnhanvaodoi.Checked = false;
        }

        //Lọc điều kiện checkbox
        private void Ckbnhantatcaloimoi_Click(object sender, EventArgs e)
        {
            Ckbmoitatca.Checked = false;
            Ckbchapnhanvaodoi.Checked = false;
        }

        //Lọc điều kiện checkbox
        private void Ckbchapnhanvaodoi_Click(object sender, EventArgs e)
        {
            Ckbnhaptatca.Checked = false;
            Ckbnhantatcaloimoi.Checked = false;
        }

        // Update Form
        private void TmrUpdate_Tick(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

                //Lấy dữ liệu vào Project
                Ckbmoitatca.Checked = CurrentClient.Ckmoitatca;
                Ckbnhaptatca.Checked = CurrentClient.Cknhaptatca;
                Ckbnhantatcaloimoi.Checked = CurrentClient.Cknhanloimoitodoi;
                Ckbchapnhanvaodoi.Checked = CurrentClient.Ckchapnhanxinnhapdoi;
            }
        }

        // Thủ tục giải phóng bộ nhớ khi đóng Form
        private void FrmFormAutoGroup_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
    }
}
