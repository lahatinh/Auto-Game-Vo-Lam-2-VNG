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
    public partial class FrmAutoTrain : Form
    {
        public FrmAutoTrain()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        public AutoClientBS CurrentClient { set; get; }
        private bool update = true;

        // Thủ tục load form
        private void FrmAutoTrain_Load(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

                //Lấy dữ liệu vào Project
                CkbTrain01.Checked = CurrentClient.CkToadoTrain01;
                CkbTrain02.Checked = CurrentClient.CkToadoTrain02;
                CkbTrain03.Checked = CurrentClient.CkToadoTrain03;
                CkbSkinDanhquai.Checked = CurrentClient.CkSkindanhquai;
                CkbSkinBuff01.Checked = CurrentClient.CkSkinBuff01;
                CkbSkinBuff02.Checked = CurrentClient.CkSkinBuff02;
                Ckbkillboss.Checked = CurrentClient.CkKillBoss;

                TxbtoadoNV.Text = CurrentClient.ValueToadoNV.ToString();
                TxbToadoTrain01.Text = CurrentClient.ValueToadoTrain01.ToString();
                TxbToadoTrain02.Text = CurrentClient.ValueToadoTrain02.ToString();
                TxbToadoTrain03.Text = CurrentClient.ValueToadoTrain03.ToString();
                TxbSkinDanhquai.Text = CurrentClient.ValueSkindanhquai.ToString();
                TxbSkinBuff01.Text = CurrentClient.ValueSkinBuff01.ToString();
                TxbSkinBuff02.Text = CurrentClient.ValueSkinBuff02.ToString();
                Cobdanhsachboss.Text = CurrentClient.ValueBossKills.ToString();
            }
        }

        // Thủ tục Button cập nhật dữ liệu
        private void BtnLuuToadoBV_Click(object sender, EventArgs e)
        {
            update = false;

            if (CurrentClient != null)
            {
                CurrentClient.ValueToadoNV = CurrentClient.CurrentPlayer.PositionX.ToString() + "/" + CurrentClient.CurrentPlayer.PositionY.ToString();
                TxbtoadoNV.Text = CurrentClient.ValueToadoNV.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTrain", "ValueToadoNV", CurrentClient.ValueToadoNV.ToString());
            }
        }

        // Thủ tục Button cập nhật dữ liệu
        private void BtnLuuToadoTrain1_Click(object sender, EventArgs e)
        {
            update = false;

            if (CurrentClient != null)
            {
                CurrentClient.ValueToadoTrain01 = CurrentClient.CurrentPlayer.PositionX.ToString() + "/" + CurrentClient.CurrentPlayer.PositionY.ToString();
                TxbToadoTrain01.Text = CurrentClient.ValueToadoTrain01.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTrain", "ValueToadoTrain01", CurrentClient.ValueToadoTrain01.ToString());
            }
        }

        // Thủ tục Button cập nhật dữ liệu
        private void BtnLuuToadoTrain2_Click(object sender, EventArgs e)
        {
            update = false;

            if (CurrentClient != null)
            {
                CurrentClient.ValueToadoTrain02 = CurrentClient.CurrentPlayer.PositionX.ToString() + "/" + CurrentClient.CurrentPlayer.PositionY.ToString();
                TxbToadoTrain02.Text = CurrentClient.ValueToadoTrain02.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTrain", "ValueToadoTrain02", CurrentClient.ValueToadoTrain02.ToString());
            }
        }

        // Thủ tục Button cập nhật dữ liệu
        private void BtnLuuToadoTrain3_Click(object sender, EventArgs e)
        {
            update = false;

            if (CurrentClient != null)
            {
                CurrentClient.ValueToadoTrain03 = CurrentClient.CurrentPlayer.PositionX.ToString() + "/" + CurrentClient.CurrentPlayer.PositionY.ToString();
                TxbToadoTrain03.Text = CurrentClient.ValueToadoTrain03.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTrain", "ValueToadoTrain03", CurrentClient.ValueToadoTrain03.ToString());
            }
        }

        // Thủ tục Button cập nhật dữ liệu
        private void BtnResetSkinQuai_Click(object sender, EventArgs e)
        {
            update = false;

            if (CurrentClient != null)
            {
                if (CurrentClient.IDSKILLPHAI == 0)
                {

                    MessageBox.Show("Bạn phải chọn skill trước!!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                if (CurrentClient.IDSKILLPHAI < 100 && CurrentClient.IDSKILLPHAI > 0)
                {
                    int Again = int.Parse(CurrentClient.LVSKILLPHAI.ToString() + "00" + CurrentClient.IDSKILLPHAI2.ToString(), System.Globalization.NumberStyles.HexNumber);
                    TxbSkinDanhquai.Text = Again.ToString();
                    CurrentClient.ValueSkindanhquai = Again.ToString();
                    WinAPI.Ghifile("\\UserData\\" + CurrentClient.CurrentPlayer.EntityNameUnicode + ".ini", "AutoTrain", "ValueSkindanhquai", CurrentClient.ValueSkindanhquai.ToString());
                }

                if (CurrentClient.IDSKILLPHAI >= 100 && CurrentClient.IDSKILLPHAI > 0 && CurrentClient.IDSKILLPHAI < 256)
                {

                    int Again = int.Parse(CurrentClient.LVSKILLPHAI.ToString() + "00" + CurrentClient.IDSKILLPHAI2.ToString(), System.Globalization.NumberStyles.HexNumber);
                    TxbSkinDanhquai.Text = Again.ToString();
                    CurrentClient.ValueSkindanhquai = Again.ToString();
                    WinAPI.Ghifile("\\UserData\\" + CurrentClient.CurrentPlayer.EntityNameUnicode + ".ini", "AutoTrain", "ValueSkindanhquai", CurrentClient.ValueSkindanhquai.ToString());
                }

                if (CurrentClient.IDSKILLPHAI >= 256)
                {
                    int Again = int.Parse(CurrentClient.LVSKILLPHAI.ToString() + "0" + CurrentClient.IDSKILLPHAI2.ToString(), System.Globalization.NumberStyles.HexNumber);
                    TxbSkinDanhquai.Text = Again.ToString();
                    CurrentClient.ValueSkindanhquai = Again.ToString();
                    WinAPI.Ghifile("\\UserData\\" + CurrentClient.CurrentPlayer.EntityNameUnicode + ".ini", "AutoTrain", "ValueSkindanhquai", CurrentClient.ValueSkindanhquai.ToString());
                }
            }

        }

        // Thủ tục Button cập nhật dữ liệu
        private void BtnResetSkinBuff01_Click(object sender, EventArgs e)
        {
            update = false;

            if (CurrentClient != null)
            {
                if (CurrentClient.IDSKILLPHAI == 0)
                {

                    MessageBox.Show("Bạn phải chọn skill trước!!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                if (CurrentClient.IDSKILLPHAI < 100 && CurrentClient.IDSKILLPHAI > 0)
                {
                    int Again = int.Parse(CurrentClient.LVSKILLPHAI.ToString() + "00" + CurrentClient.IDSKILLPHAI2.ToString(), System.Globalization.NumberStyles.HexNumber);
                    TxbSkinBuff01.Text = Again.ToString();
                    CurrentClient.ValueSkinBuff01 = Again.ToString();
                    WinAPI.Ghifile("\\UserData\\" + CurrentClient.CurrentPlayer.EntityNameUnicode + ".ini", "AutoTrain", "ValueSkinBuff01", CurrentClient.ValueSkinBuff01.ToString());
                }

                if (CurrentClient.IDSKILLPHAI >= 100 && CurrentClient.IDSKILLPHAI > 0 && CurrentClient.IDSKILLPHAI < 256)
                {

                    int Again = int.Parse(CurrentClient.LVSKILLPHAI.ToString() + "00" + CurrentClient.IDSKILLPHAI2.ToString(), System.Globalization.NumberStyles.HexNumber);
                    TxbSkinBuff01.Text = Again.ToString();
                    CurrentClient.ValueSkinBuff01 = Again.ToString();
                    WinAPI.Ghifile("\\UserData\\" + CurrentClient.CurrentPlayer.EntityNameUnicode + ".ini", "AutoTrain", "ValueSkinBuff01", CurrentClient.ValueSkinBuff01.ToString());
                }

                if (CurrentClient.IDSKILLPHAI >= 256)
                {
                    int Again = int.Parse(CurrentClient.LVSKILLPHAI.ToString() + "0" + CurrentClient.IDSKILLPHAI2.ToString(), System.Globalization.NumberStyles.HexNumber);
                    TxbSkinBuff01.Text = Again.ToString();
                    CurrentClient.ValueSkinBuff01 = Again.ToString();
                    WinAPI.Ghifile("\\UserData\\" + CurrentClient.CurrentPlayer.EntityNameUnicode + ".ini", "AutoTrain", "ValueSkinBuff01", CurrentClient.ValueSkinBuff01.ToString());
                }
            }
        }

        // Thủ tục Button cập nhật dữ liệu
        private void BtnResetSkinBuff02_Click(object sender, EventArgs e)
        {
            update = false;

            if (CurrentClient != null)
            {
                if (CurrentClient.IDSKILLPHAI == 0)
                {

                    MessageBox.Show("Bạn phải chọn skill trước!!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                if (CurrentClient.IDSKILLPHAI < 100 && CurrentClient.IDSKILLPHAI > 0)
                {
                    int Again = int.Parse(CurrentClient.LVSKILLPHAI.ToString() + "00" + CurrentClient.IDSKILLPHAI2.ToString(), System.Globalization.NumberStyles.HexNumber);
                    TxbSkinBuff02.Text = Again.ToString();
                    CurrentClient.ValueSkinBuff02 = Again.ToString();
                    WinAPI.Ghifile("\\UserData\\" + CurrentClient.CurrentPlayer.EntityNameUnicode + ".ini", "AutoTrain", "ValueSkinBuff02", CurrentClient.ValueSkinBuff02.ToString());
                }

                if (CurrentClient.IDSKILLPHAI >= 100 && CurrentClient.IDSKILLPHAI > 0 && CurrentClient.IDSKILLPHAI < 256)
                {

                    int Again = int.Parse(CurrentClient.LVSKILLPHAI.ToString() + "00" + CurrentClient.IDSKILLPHAI2.ToString(), System.Globalization.NumberStyles.HexNumber);
                    TxbSkinBuff02.Text = Again.ToString();
                    CurrentClient.ValueSkinBuff02 = Again.ToString();
                    WinAPI.Ghifile("\\UserData\\" + CurrentClient.CurrentPlayer.EntityNameUnicode + ".ini", "AutoTrain", "ValueSkinBuff02", CurrentClient.ValueSkinBuff02.ToString());
                }

                if (CurrentClient.IDSKILLPHAI >= 256)
                {
                    int Again = int.Parse(CurrentClient.LVSKILLPHAI.ToString() + "0" + CurrentClient.IDSKILLPHAI2.ToString(), System.Globalization.NumberStyles.HexNumber);
                    TxbSkinBuff02.Text = Again.ToString();
                    CurrentClient.ValueSkinBuff02 = Again.ToString();
                    WinAPI.Ghifile("\\UserData\\" + CurrentClient.CurrentPlayer.EntityNameUnicode + ".ini", "AutoTrain", "ValueSkinBuff02", CurrentClient.ValueSkinBuff02.ToString());
                }
            }
        }

        // Thủ tục Button cập nhật dữ liệu
        private void Btncapnhatlistboss_Click(object sender, EventArgs e)
        {
            update = false;

            if (CurrentClient != null)
            {
                CurrentClient.RefreshEntityList();

                Cobdanhsachboss.DataSource = (from entity in CurrentClient.EntityList where entity.EntityType == NPCType.Beast && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 1000 orderby entity.EntityId select entity.EntityId + "/" + entity.EntityNameNoMark).ToList();
            }
        }

        // Thủ tục lưu checkbox
        private void CkbTrain01_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.CkToadoTrain01 = CkbTrain01.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTrain", "AutoTrain.Check2", CurrentClient.CkToadoTrain01.ToString());
            }
        }

        // Thủ tục lưu checkbox
        private void CkbTrain02_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.CkToadoTrain02 = CkbTrain02.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTrain", "AutoTrain.Check3", CurrentClient.CkToadoTrain02.ToString());
            }
        }

        // Thủ tục lưu checkbox
        private void CkbTrain03_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.CkToadoTrain03 = CkbTrain03.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTrain", "AutoTrain.Check4", CurrentClient.CkToadoTrain03.ToString());
            }
        }

        // Thủ tục lưu checkbox
        private void CkbSkinDanhquai_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.CkSkindanhquai = CkbSkinDanhquai.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTrain", "AutoTrain.Check5", CurrentClient.CkSkindanhquai.ToString());
            }
        }

        // Thủ tục lưu checkbox
        private void CkbSkinBuff01_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.CkSkinBuff01 = CkbSkinBuff01.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTrain", "AutoTrain.Check6", CurrentClient.CkSkinBuff01.ToString());
            }
        }

        // Thủ tục lưu checkbox
        private void CkbSkinBuff02_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.CkSkinBuff02 = CkbSkinBuff02.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTrain", "AutoTrain.Check7", CurrentClient.CkSkinBuff02.ToString());
            }
        }

        // Thủ tục lưu checkbox
        private void Ckbkillboss_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.CkKillBoss = Ckbkillboss.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTrain", "AutoTrain.Check8", CurrentClient.CkKillBoss.ToString());
            }
        }

        // Thủ tục lựa chọn giá trị trong combobox
        private void Cobdanhsachboss_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ValueBossKills = Cobdanhsachboss.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTrain", "ValueBossKills", CurrentClient.ValueBossKills.ToString());
            }
        }

        //Cập nhật Form

        private void TmrUpdate_Tick(object sender, EventArgs e)
        {
            if (CurrentClient != null && update == true)
            {
                this.Text = CurrentClient.CurrentPlayer.EntityNameNoMark.ToString();

                //Lấy dữ liệu vào Project
                CkbTrain01.Checked = CurrentClient.CkToadoTrain01;
                CkbTrain02.Checked = CurrentClient.CkToadoTrain02;
                CkbTrain03.Checked = CurrentClient.CkToadoTrain03;
                CkbSkinDanhquai.Checked = CurrentClient.CkSkindanhquai;
                CkbSkinBuff01.Checked = CurrentClient.CkSkinBuff01;
                CkbSkinBuff02.Checked = CurrentClient.CkSkinBuff02;
                Ckbkillboss.Checked = CurrentClient.CkKillBoss;

                TxbtoadoNV.Text = CurrentClient.ValueToadoNV.ToString();
                TxbToadoTrain01.Text = CurrentClient.ValueToadoTrain01.ToString();
                TxbToadoTrain02.Text = CurrentClient.ValueToadoTrain02.ToString();
                TxbToadoTrain03.Text = CurrentClient.ValueToadoTrain03.ToString();
                TxbSkinDanhquai.Text = CurrentClient.ValueSkindanhquai.ToString();
                TxbSkinBuff01.Text = CurrentClient.ValueSkinBuff01.ToString();
                TxbSkinBuff02.Text = CurrentClient.ValueSkinBuff02.ToString();
                Cobdanhsachboss.Text = CurrentClient.ValueBossKills.ToString();
            }
        }

        // Tạm dừng Update Form
        private void Cobdanhsachboss_Click(object sender, EventArgs e)
        {
            update = false;
        }

        // Thủ tục giải phóng bộ nhớ khi đóng Form
        private void FrmAutoTrain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
    }
}
