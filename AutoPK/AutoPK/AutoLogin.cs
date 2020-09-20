using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoClient.Entities;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using AutoClient;

namespace AutoPK
{
    class AutoLogin
    {
        public IntPtr OpenProcessHandle;
        public IntPtr WindowHwnd;
        public uint ProcessID;
        public uint HookMsg;

        // Thủ tục nạp handle và client
        public void Attach(IntPtr hwnd)
        {
            this.WindowHwnd = hwnd;
            WinAPI.GetWindowThreadProcessId(WindowHwnd, out ProcessID);
            OpenProcessHandle = WinAPI.OpenProcess(WinAPI.ProcessAccessFlags.All, true, Convert.ToInt32(ProcessID));
        }

        // Thủ tục Auto login
        #region Các thủ tục Auto Login

        // Thủ tục định nghĩa chưa Inject hook
        private bool _isInjected = false;

        // Thủ tục định nghĩa chưa Inject hook
        public bool isInjected
        {
            get { return _isInjected; }
        }

        // Thủ tục Inject hook
        public int Inject()
        {
            var result = HookGame.InjectDll(WindowHwnd);
            if (result == 1)
            {
                _isInjected = true;
                this.HookMsg = HookGame.GetMsg();
            }
            return result;
        }

        // Thủ tục DeInject hook
        public int DeInject()
        {
            var result = HookGame.UnmapDll(WindowHwnd);
            _isInjected = false;
            return result;
        }

        // Gửi thông điệp đến hook xử lý
        public void BatDau() //Bắt đầu vào Game
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_BAT_DAU);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }

        // Gửi thông điệp đến hook xử lý
        public void ChonSever(int idcum, int idserver, int Base) //Chọn sever
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_CHON_SEVER);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, idcum);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, idserver);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, Base);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }

        // Gửi thông điệp đến hook xử lý
        public void ChonNV(int nv) //Chọn sever
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_CHON_NV);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_push, nv);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }

        // Gửi thông điệp đến hook xử lý
        public void BatDauNV() //Bắt đầu vào nhân vật
        {
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_start, GameConst.FUNC_BAT_DAU_NV);
            WinAPI.PostMessage(WindowHwnd, HookMsg, GameConst.cmd_end, 0);
        }

        // Send phím đến cho các Handle
        public void sendphim(int A)
        {
            WinAPI.PostMessage(WindowHwnd, (uint)WinAPI.Keyvisual.WM_KEYDOWN, A, A);
        }

        // Send string đến cho các Handle
        public void sendstring(string A)
        {
            byte[] array = Encoding.Default.GetBytes(A);

            for (int i = 0; i < array.Length; i++)
            {
                WinAPI.PostMessage(WindowHwnd, (uint)WinAPI.Keyvisual.WM_CHAR, (int)(array[i]), 1);
            }
        }

        // Send Click API
        private static int MAKEPARAM(int l, int h)
        {
            return ((l & 0xffff) | (h << 0x10));
        }

        // Send Click API
        public void SendClick(int x, int y)
        {
            int Lparam = MAKEPARAM(x, y);

            WinAPI.PostMessage(WindowHwnd, (uint)WinAPI.WMessenges.WM_LBUTTONDOWN, (int)WinAPI.WMessenges.MK_LBUTTON, Lparam);
            WinAPI.PostMessage(WindowHwnd, (uint)WinAPI.WMessenges.WM_LBUTTONUP, (int)0, Lparam);

            WinAPI.PostMessage(WindowHwnd, (uint)WinAPI.WMessenges.WM_LBUTTONDBCLICK, (int)WinAPI.WMessenges.MK_LBUTTON, Lparam);
            WinAPI.PostMessage(WindowHwnd, (uint)WinAPI.WMessenges.WM_LBUTTONUP, (int)0, Lparam);
        }

        // Kiểm tra đã load được bảng lựa chọn nhân vật chưa
        public Int32 Bangnhanvat
        {
            get
            {
                var step1 = WinAPI.ReadProcessMemoryIntPtr(OpenProcessHandle, GameConst.Bangnhanvat);
                var step2 = IntPtr.Add(step1, 0x000054D4);
                var step3 = WinAPI.ReadProcessMemoryInt32(OpenProcessHandle, step2);
                return step3;
            }
        }



        #endregion
    }
}
