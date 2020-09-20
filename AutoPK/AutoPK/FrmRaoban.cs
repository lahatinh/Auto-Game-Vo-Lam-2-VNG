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
    public partial class FrmRaoban : Form
    {
        public FrmRaoban()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        public AutoClientBS CurrentClient { set; get; }
        private bool update = true;

        // Thủ tục Load Form
        private void FrmRaoban_Load(object sender, EventArgs e)
        {
            this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

            Txbloirao.Text = CurrentClient.ValueLoirao;
        }

        // Thủ tục Update Form
        private void TmrUpdate_Tick(object sender, EventArgs e)
        {
            if (CurrentClient != null && update == true)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

                Txbloirao.Text = CurrentClient.ValueLoirao;
            }
        }

        // Thủ tục Button nhập lời rao
        private void button1_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ValueLoirao = Txbloirao.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTNC", "ValueLoirao",CurrentClient.ValueLoirao);
            }
        }

        // Thủ tục tạm dừng update Form
        private void Txbloirao_KeyPress(object sender, KeyPressEventArgs e)
        {
            update = false;
        }

        // Thủ tục giải phóng bộ nhớ khi đóng Form
        private void FrmRaoban_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
    }
}
