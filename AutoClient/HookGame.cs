using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AutoClient
{
    public static class HookGame
    {
        [DllImport("hookxh.dll", SetLastError = true)]
        public static extern Int32 InjectDll(IntPtr gameHwnd);
        [DllImport("hookxh.dll", SetLastError = true)]
        public static extern Int32 UnmapDll(IntPtr gameHwnd);
        [DllImport("hookxh.dll", SetLastError = true)]
        public static extern UInt32 GetMsg();
    }
}
