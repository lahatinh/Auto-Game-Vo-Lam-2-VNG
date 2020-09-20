using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoClient;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace AutoPK
{
    public partial class FrmAutoLogin : Form
    {
        private Dictionary<IntPtr, AutoLogin> _logins = new Dictionary<IntPtr, AutoLogin>();
        private List<IntPtr> _clientHwndslogin = new List<IntPtr>();
        private int IDcum;
        private int IDServer;
        private int SttNV;
        private List<string> Acc = new List<string>();

        private static FrmAutoLogin _instance = null;

        //Đảm bảo luôn chỉ có duy nhất 1 instance của FrmAutoLogin được khởi tạo
        public static FrmAutoLogin Instance
        {
            get
            {
                if (FrmAutoLogin._instance == null || FrmAutoLogin._instance.IsDisposed)
                {
                    FrmAutoLogin._instance = new FrmAutoLogin();
                }
                return FrmAutoLogin._instance;
            }
        }

        public FrmAutoLogin()
        {
            InitializeComponent();
        }


        // Thủ tục Load Form Login
        private void FrmAutoLogin_Load(object sender, EventArgs e)
        {
            BtnThem.Enabled = true;
            BtnSua.Enabled = false;
            BtnXoa.Enabled = false;
            BtnHuy.Enabled = false;

            lsvAcout.Columns[2].Width = 0;
            Acc = new List<string>(File.ReadAllLines(Directory.GetCurrentDirectory() + "\\Login\\Acc.txt"));
            
            for(int i = 0; i < Acc.Count; i++)
            {
                string[] aray = Acc[i].Split('-');

                ListViewItem list = new ListViewItem();

                list.SubItems.Add(aray[0]);
                list.SubItems.Add(aray[1]);
                list.SubItems.Add(aray[2]);
                list.SubItems.Add(aray[3]);
                list.SubItems.Add(aray[4]);
                list.SubItems.Add(aray[5]);

                lsvAcout.Items.Add(list);
            }

        }

        //Lưu lại các handle login
        private bool SearchForGameWindowslogin(IntPtr hwnd, IntPtr lParam)
        {
            StringBuilder title = new StringBuilder();
            WinAPI.GetWindowText(hwnd, title, title.Capacity);
            if (title.ToString() == "Vâ L©m 2 ()")
            {
                _clientHwndslogin.Add(hwnd);
            }
            return true;
        }

        private void TmrUpdatehWnd_Tick(object sender, EventArgs e)
        {
            _clientHwndslogin.Clear();
            WinAPI.EnumWindows(SearchForGameWindowslogin, new IntPtr(0));

            _logins.Clear();
            foreach (var clientHwnd in _clientHwndslogin)
            {
                var login = new AutoLogin();
                login.Attach(clientHwnd);

                _logins.Add(clientHwnd, login);
            }
        }

        // Thủ tục Button
        private void Btntim_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"E:\",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "exe",
                Filter = "exe files (*.exe)|*.exe",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Txbduongdan.Text = openFileDialog1.FileName.Substring(0,openFileDialog1.FileName.LastIndexOf("\\")+1);
            }
        }

        //Thủ tục đóng Form Login
        private void FrmAutoLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                foreach (var login in _logins.Values)
                {
                    if (login.isInjected)
                        login.DeInject();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Dong Form: " + ex.Message);
                return;
            }
        }

        // Thủ tục hoàn thành đóng Form Login
        private void FrmAutoLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        // Thủ tục Button
        private void BtnThem_Click(object sender, EventArgs e)
        {
            if(Txbtaikhoan.Text ==""|| Txbtaikhoan.Text == null || Txbmatkhau.Text == "" || Txbmatkhau.Text == null || CobSTTNV.Text == "" || CobSTTNV.Text == null || CobServer.Text == "" || CobServer.Text == null || Txbduongdan.Text == "" || Txbduongdan.Text == null)
            {
                MessageBox.Show("Có dữ liệu bạn chưa nhập! Hãy nhập đầy đủ dữ liệu trước khi thêm mới");
            }
            else
            {
                lsvAcout.Invoke(new MethodInvoker(() =>
                {
                    ListViewItem row = new ListViewItem();

                    row.SubItems.Add(Txbtaikhoan.Text.ToString());
                    row.SubItems.Add(Txbmatkhau.Text.ToString());
                    row.SubItems.Add(SttNV.ToString());
                    row.SubItems.Add(IDcum.ToString());
                    row.SubItems.Add(IDServer.ToString());
                    row.SubItems.Add(Txbduongdan.Text.ToString());
                    
                    if (lsvAcout.Items.Count < 1)
                    {
                        lsvAcout.Items.Add(row);
                    }

                    else
                    {
                        List<ListViewItem> lsvAcoutnew = new List<ListViewItem>();

                        for (int i = 0; i< lsvAcout.Items.Count; i++)
                        {
                            if (lsvAcout.Items[i].SubItems[1].Text == Txbtaikhoan.Text)
                            {
                                lsvAcoutnew.Clear();
                                break;
                            }
                            else
                            {
                                lsvAcoutnew.Add(row);
                            }
                        }

                        if (lsvAcoutnew.Count>=1)
                        {
                            foreach(var Items in lsvAcoutnew)
                            {
                                lsvAcout.Items.Add(Items);
                                break;
                            }
                        }
                    }
            }));

        }
    }

        // Thủ tục Button
        private void BtnSua_Click(object sender, EventArgs e)
        {
           for(int i = 0; i < lsvAcout.Items.Count; i++)
            {
                if (lsvAcout.Items[i].Selected)
                {
                    lsvAcout.Items[i].SubItems[1].Text = Txbtaikhoan.Text.ToString();
                    lsvAcout.Items[i].SubItems[2].Text = Txbmatkhau.Text.ToString();
                    lsvAcout.Items[i].SubItems[3].Text = CobSTTNV.Text.ToString();
                    lsvAcout.Items[i].SubItems[4].Text = IDcum.ToString();
                    lsvAcout.Items[i].SubItems[5].Text = IDServer.ToString();
                    lsvAcout.Items[i].SubItems[6].Text = Txbduongdan.Text.ToString();
                }
            }

        }

        // Thủ tục Button
        private void BtnXoa_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lsvAcout.Items.Count; i++)
            {
                if (lsvAcout.Items[i].Selected)
                {
                    lsvAcout.Items[i].Remove();
                }
            }

            BtnThem.Enabled = true;
            BtnSua.Enabled = false;
            BtnXoa.Enabled = false;
            BtnHuy.Enabled = false;
        }

        // Thủ tục Button
        private void BtnHuy_Click(object sender, EventArgs e)
        {
            Txbduongdan.Text = "";
            Txbmatkhau.Text = "";
            Txbtaikhoan.Text = "";
            CobServer.Text = "";
            CobSTTNV.Text = "";

            BtnThem.Enabled = true;
            BtnSua.Enabled = false;
            BtnXoa.Enabled = false;
            BtnHuy.Enabled = false;
        }

        // Thủ tục Button
        private void BtnDangNhap_Click(object sender, EventArgs e)
        {
            // Tạo thread mới chạy song song với Main chạy hàm bơm máu
            Thread thropengame = new Thread(new ThreadStart(opengame));
            thropengame.IsBackground = true;
            thropengame.Start();

        }

        // Thủ tục Button
        private void BtnLuu_Click(object sender, EventArgs e)
        {
            List<string> luu = new List<string>();
            
            for (int i = 0; i < lsvAcout.Items.Count; i++)
            {
                luu.Add(lsvAcout.Items[i].SubItems[1].Text + "-" + lsvAcout.Items[i].SubItems[2].Text + "-" + lsvAcout.Items[i].SubItems[3].Text + "-" + lsvAcout.Items[i].SubItems[4].Text + "-" + lsvAcout.Items[i].SubItems[5].Text + "-" + lsvAcout.Items[i].SubItems[6].Text);
            }

            File.WriteAllLines(string.Concat(Directory.GetCurrentDirectory(), "\\Login\\Acc.txt"), luu.ToArray());

            MessageBox.Show("Lưu thành công", "Thông báo");
        }

        // Thủ tục CheckBox
        private void checkBoxAcount_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                for (int i = 0; i < this.lsvAcout.Items.Count; i++)
                {
                    lsvAcout.Items[i].Checked = true;
                }
            }
            else
            {
                for (int i = 0; i < this.lsvAcout.Items.Count; i++)
                {
                    lsvAcout.Items[i].Checked = false;
                }
            }
        }

        // Thủ tục lựa chọn giá trị trong Combobox
        private void CobSTTNV_SelectedIndexChanged(object sender, EventArgs e)
        {
            SttNV = int.Parse(CobSTTNV.Text.ToString());
        }

        // Thủ tục lựa chọn giá trị trong Combobox
        private void CobServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CobServer.Text == "Bạch Hổ")
            {
                IDcum = 0;
                IDServer = 0;
            }
            else if (CobServer.Text == "Phục Hổ")
            {
                IDcum = 0;
                IDServer = 1;
            }
            else if (CobServer.Text == "Quán Hổ")
            {
                IDcum = 0;
                IDServer = 2;
            }
            else if (CobServer.Text == "Phi Hổ")
            {
                IDcum = 0;
                IDServer = 3;
            }
            else if (CobServer.Text == "Tàng Long")
            {
                IDcum = 1;
                IDServer = 0;
            }
            else if (CobServer.Text == "Thiên Long")
            {
                IDcum = 1;
                IDServer = 1;
            }
            else if (CobServer.Text == "Linh Bảo Sơn")
            {
                IDcum = 1;
                IDServer = 2;
            }
            else if (CobServer.Text == "Hoả Phụng")
            {
                IDcum = 1;
                IDServer = 3;
            }
            else if (CobServer.Text == "Hàng Long")
            {
                IDcum = 1;
                IDServer = 4;
            }

        }
        // Thủ tục trong Listviews
        private void lsvAcout_SelectedIndexChanged(object sender, EventArgs e)
        {
            for(int i = 0; i < lsvAcout.Items.Count; i++)
            {
                if(lsvAcout.Items[i].Selected)
                {
                    Txbtaikhoan.Text = lsvAcout.Items[i].SubItems[1].Text;
                    Txbmatkhau.Text = lsvAcout.Items[i].SubItems[2].Text;
                    CobSTTNV.Text = lsvAcout.Items[i].SubItems[3].Text;
                    IDcum = int.Parse(lsvAcout.Items[i].SubItems[4].Text);
                    IDServer = int.Parse(lsvAcout.Items[i].SubItems[5].Text);
                    Txbduongdan.Text = lsvAcout.Items[i].SubItems[6].Text;

                    if (IDcum == 0 && IDServer ==0)
                    {
                        CobServer.Text = "Bạch Hổ";
                    }
                    else if (IDcum == 0 && IDServer == 1)
                    {
                        CobServer.Text = "Phục Hổ";
                    }
                    else if (IDcum == 0 && IDServer == 2)
                    {
                        CobServer.Text = "Quán Hổ";
                    }
                    else if (IDcum == 0 && IDServer == 3)
                    {
                        CobServer.Text = "Phi Hổ";
                    }
                    else if (IDcum == 1 && IDServer == 0)
                    {
                        CobServer.Text = "Tàng Long";
                    }
                    else if (IDcum == 1 && IDServer == 1)
                    {
                        CobServer.Text = "Thiên Long";
                    }
                    else if (IDcum == 1 && IDServer == 2)
                    {
                        CobServer.Text = "Linh Bảo Sơn";
                    }
                    else if (IDcum == 1 && IDServer == 3)
                    {
                        CobServer.Text = "Hoả Phụng";
                    }
                    else if (IDcum == 1 && IDServer == 4)
                    {
                        CobServer.Text = "Hàng Long";
                    }
                }
            }

            BtnThem.Enabled = false;
            BtnSua.Enabled = true;
            BtnXoa.Enabled = true;
            BtnHuy.Enabled = true;
        }

        // Thủ tục trong Listviews
        private void lsvAcout_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            lsvAcout.Items[e.Index].Selected = true;
        }

        //Thủ tục đăng nhập
        private void opengame()
        {
            try
            {
                for (int i = 0; i < lsvAcout.Items.Count; i++)
                {
                    if (lsvAcout.Items[i].Checked)
                    {
                        Process pros = new Process();
                        pros.StartInfo.Arguments = lsvAcout.Items[i].SubItems[6].Text.ToString() + "so2game.exe";
                        pros.StartInfo.Verb = "OPEN";
                        pros.StartInfo.FileName = lsvAcout.Items[i].SubItems[6].Text.ToString() + "so2game.exe";
                        pros.StartInfo.UseShellExecute = true;
                        pros.StartInfo.WorkingDirectory = lsvAcout.Items[i].SubItems[6].Text.ToString();
                        pros.Start();
                        pros.WaitForInputIdle();

                        Thread.Sleep(1600);

                        foreach (var login in _logins.Values)
                        {
                            if (!login.isInjected)
                                login.Inject();

                            login.BatDau();
                            login.ChonSever(int.Parse(lsvAcout.Items[i].SubItems[4].Text), int.Parse(lsvAcout.Items[i].SubItems[5].Text), GameConst.BaseSV);
                            login.sendstring(lsvAcout.Items[i].SubItems[1].Text);
                            login.sendphim((int)WinAPI.keyflag.KEY_TAB);
                            login.sendstring(lsvAcout.Items[i].SubItems[2].Text);
                            login.sendphim((int)WinAPI.keyflag.KEY_RETURN);

                            break;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        //Thủ tục Timer lựa chọn nhân vật
        private void Tmrchonnv_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < lsvAcout.Items.Count; i++)
            {
                if (lsvAcout.Items[i].Checked)
                {
                    foreach (var login in _logins.Values)
                    {
                        if (login.Bangnhanvat == 1)
                        {
                            if (!login.isInjected)
                                login.Inject();

                            login.ChonNV(int.Parse(lsvAcout.Items[i].SubItems[3].Text));

                            break;
                        }
                    }
                }
            }
        }

    }
}
