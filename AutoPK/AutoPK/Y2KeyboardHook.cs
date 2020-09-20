using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace AutoPK
{
    class Y2KeyboardHook
    {
    #region Win32 API Functions and Constants
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        KeyboardHookDelegate lpfn, IntPtr hMod, int dwThreadId);
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x101;
    #endregion
    private KeyboardHookDelegate _hookProc;
    private IntPtr _hookHandle = IntPtr.Zero;
    public delegate IntPtr KeyboardHookDelegate(int nCode, IntPtr wParam, IntPtr lParam);
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardHookStruct
    {
        public int VirtualKeyCode;
        public int ScanCode;
        public int Flags;
        public int Time;
        public int ExtraInfo;
    }
    // destructor
    ~Y2KeyboardHook()
    {
        Uninstall();
    }
    public void Install()
    {
        _hookProc = KeyboardHookProc;
        _hookHandle = SetupHook(_hookProc);
        if (_hookHandle == IntPtr.Zero)
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }
    private IntPtr SetupHook(KeyboardHookDelegate hookProc)
    {
        IntPtr hInstance = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]);
        return SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hInstance, 0);
    }
    private IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode & amp; gt;= 0)
        {
            KeyboardHookStruct kbStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
            if (wParam == (IntPtr)WM_KEYDOWN)
            {
                Keys k = (Keys)kbStruct.VirtualKeyCode;
                // disable Left Windows, Left Shift and Left Control keys
                if (k == Keys.A)
                {
                    SendKeys.Send(k.ToString());
                    return (IntPtr)1;
                }
                else if (k == (Keys.Escape)) // exit program
                    Application.Exit();
            }
        }
        return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
    }
    public void Uninstall()
    {
        UnhookWindowsHookEx(_hookHandle);
    }
  }
}
