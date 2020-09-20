using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace AutoClient
{
    public class WinAPI
    {
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        public enum Keyvisual : uint
        {
            WM_KEYDOWN = 0x100,
            WM_KEYUP = 0x101,
            WM_SENDTEXT = 0x00C,
            WM_CHAR = 0x0102,
        }

        public enum WMessenges : int
        {
            WM_LBUTTONDOWN = 0x201,
            WM_LBUTTONUP = 0x202,
            WM_LBUTTONDBCLICK = 0x203,
            WM_RBUTTONDOWN = 0x204,
            WM_RBUTTONUP = 0x205,
            WM_RBUTTONDBCLICK = 0x206,
            MK_LBUTTON = 0x1,
            MK_RBUTTON = 0x2,
        }

        public enum keyflag : int
        {
            //0 key
            KEY_0 = 0x30,
            //1 key
            KEY_1 = 0x31,
            //2 key
            KEY_2 = 0x32,
            //3 key
            KEY_3 = 0x33,
            //4 key
            KEY_4 = 0x34,
            //5 key
            KEY_5 = 0x35,
            //6 key
            KEY_6 = 0x36,
            //7 key
            KEY_7 = 0x37,
            //8 key
            KEY_8 = 0x38,
            //9 key
            KEY_9 = 0x39,
            // - key
            KEY_MINUS = 0xBD,
            // + key
            KEY_PLUS = 0xBB,
            //A key
            KEY_A = 0x41,
            //B key
            KEY_B = 0x42,
            //C key
            KEY_C = 0x43,
            //D key
            KEY_D = 0x44,
            //E key
            KEY_E = 0x45,
            //F key
            KEY_F = 0x46,
            //G key
            KEY_G = 0x47,
            //H key
            KEY_H = 0x48,
            //I key
            KEY_I = 0x49,
            //J key
            KEY_J = 0x4A,
            //L key
            KEY_L = 0x4C,
            //K key
            KEY_K = 0x4B,
            //M key
            KEY_M = 0x4D,
            //N key
            KEY_N = 0x4E,
            //O key
            KEY_O = 0x4F,
            //P key
            KEY_P = 0x50,
            //Q key
            KEY_Q = 0x51,
            //R key
            KEY_R = 0x52,
            //S key
            KEY_S = 0x53,
            //T key
            KEY_T = 0x54,
            //U key
            KEY_U = 0x55,
            //V key
            KEY_V = 0x56,
            //W key
            KEY_W = 0x57,
            //X key
            KEY_X = 0x58,
            //Y key
            KEY_Y = 0x59,
            //Z key
            KEY_Z = 0x5A,
            //Left mouse button
            KEY_LBUTTON = 0x01,
            //Right mouse button
            KEY_RBUTTON = 0x02,
            //Control-break processing
            KEY_CANCEL = 0x03,
            //Middle mouse button (three-button mouse)
            KEY_MBUTTON = 0x04,
            //BACKSPACE key  
            KEY_BACK = 0x08,
            //TAB key
            KEY_TAB = 0x09,
            //CLEAR key
            KEY_CLEAR = 0x0C,
            //ENTER key
            KEY_RETURN = 0x0D,
            //SHIFT key
            KEY_SHIFT = 0x10,
            //CTRL key
            KEY_CONTROL = 0x11,
            //ALT key
            KEY_MENU = 0x12,
            //PAUSE key
            KEY_PAUSE = 0x13,
            //CAPS LOCK key
            KEY_CAPITAL = 0x14,
            //ESC key
            KEY_ESCAPE = 0x1B,
            //SPACEBAR
            KEY_SPACE = 0x20,
            //PAGE UP key
            KEY_PRIOR = 0x21,
            //PAGE DOWN key
            KEY_NEXT = 0x22,
            //END key
            KEY_END = 0x23,
            //HOME key
            KEY_HOME = 0x24,
            //LEFT ARROW key
            KEY_LEFT = 0x25,
            //UP ARROW key
            KEY_UP = 0x26,
            //RIGHT ARROW key
            KEY_RIGHT = 0x27,
            //DOWN ARROW key
            KEY_DOWN = 0x28,
            //SELECT key
            KEY_SELECT = 0x29,
            //PRINT key
            KEY_PRINT = 0x2A,
            //EXECUTE key
            KEY_EXECUTE = 0x2B,
            //PRINT SCREEN key
            KEY_SNAPSHOT = 0x2C,
            //INS key
            KEY_INSERT = 0x2D,
            //DEL key
            KEY_DELETE = 0x2E,
            //HELP key
            KEY_HELP = 0x2F,
            //Numeric keypad 0 key
            KEY_NUMPAD0 = 0x60,
            //Numeric keypad 1 key
            KEY_NUMPAD1 = 0x61,
            //Numeric keypad 2 key
            KEY_NUMPAD2 = 0x62,
            //Numeric keypad 3 key  
            KEY_NUMPAD3 = 0x63,
            //Numeric keypad 4 key  
            KEY_NUMPAD4 = 0x64,
            //Numeric keypad 5 key  
            KEY_NUMPAD5 = 0x65,
            //Numeric keypad 6 key  
            KEY_NUMPAD6 = 0x66,
            //Numeric keypad 7 key
            KEY_NUMPAD7 = 0x67,
            //Numeric keypad 8 key  
            KEY_NUMPAD8 = 0x68,
            //Numeric keypad 9 key  
            KEY_NUMPAD9 = 0x69,
            //Separator key
            KEY_SEPARATOR = 0x6C,
            //Subtract key
            KEY_SUBTRACT = 0x6D,
            //Decimal key
            KEY_DECIMAL = 0x6E,
            //Divide key
            KEY_DIVIDE = 0x6F,
            //F1 key
            KEY_F1 = 0x70,
            //F2 key
            KEY_F2 = 0x71,
            //F3 key
            KEY_F3 = 0x72,
            //F4 key
            KEY_F4 = 0x73,
            //F5 key
            KEY_F5 = 0x74,
            //F6 key
            KEY_F6 = 0x75,
            //F7 key
            KEY_F7 = 0x76,
            //F8 key
            KEY_F8 = 0x77,
            //F9 key
            KEY_F9 = 0x78,
            //F10 key
            KEY_F10 = 0x79,
            //F11 key
            KEY_F11 = 0x7A,
            //F12 key
            KEY_F12 = 0x7B,
            //SCROLL LOCK key
            KEY_SCROLL = 0x91,
            //Left SHIFT key
            KEY_LSHIFT = 0xA0,
            //Right SHIFT key
            KEY_RSHIFT = 0xA1,
            //Left CONTROL key
            KEY_LCONTROL = 0xA2,
            //Right CONTROL key
            KEY_RCONTROL = 0xA3,
            //Left MENU key
            KEY_LMENU = 0xA4,
            //Right MENU key
            KEY_RMENU = 0xA5,
            //, key
            KEY_COMMA = 0xBC,
            //. key
            KEY_PERIOD = 0xBE,
            //Play key
            KEY_PLAY = 0xFA,
            //Zoom key
            KEY_ZOOM = 0xFB,
            NULL = 0x0,
        }

        [DllImport("User32")]
        public static extern int ShowWindow(int hwnd, int nCmdShow);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "3")]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("kernel32.dll",BestFitMapping =true, CharSet = CharSet.Unicode, SetLastError = true,CallingConvention =CallingConvention.StdCall)]
        public static extern uint GetPrivateProfileStringW(string lpSecsion, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);
        public static string Docfile(string file, string secsion, string key, string lpdefault)
        {
            StringBuilder stringBuilder = new StringBuilder();
            WinAPI.GetPrivateProfileStringW(secsion,key, lpdefault, stringBuilder,(uint)30000, Directory.GetCurrentDirectory() + file);
            return stringBuilder.ToString();
        }
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);
        // For Windows Mobile, replace user32.dll with coredll.dll
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll",BestFitMapping =true, CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrivateProfileStringW(string lpsecsion, string lpKeyName, string lpString, string lpFileName);

        public static bool Ghifile(string file, string secsion, string key, string value)
        {
            return WinAPI.WritePrivateProfileStringW(secsion,key, value,Directory.GetCurrentDirectory() + file);
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(
          IntPtr hProcess,
          IntPtr lpBaseAddress,
          [Out] byte[] lpBuffer,
          int dwSize,
          out int lpNumberOfBytesRead
         );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(
         IntPtr hProcess,
         IntPtr lpBaseAddress,
         [Out, MarshalAs(UnmanagedType.AsAny)] object lpBuffer,
         int dwSize,
         out int lpNumberOfBytesRead
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(
         IntPtr hProcess,
         IntPtr lpBaseAddress,
         IntPtr lpBuffer,
         int dwSize,
         out int lpNumberOfBytesRead
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);


        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32", SetLastError = true)]
        public static extern int GetProcessId(IntPtr hProcess);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // When you don't want the ProcessId, use this overload and pass IntPtr.Zero for the second parameter
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public static IntPtr FindWindow(string windowName, bool wait)
        {
            IntPtr hWnd = FindWindow(null, windowName);
            while (wait && hWnd.ToInt32() == 0)
            {
                System.Threading.Thread.Sleep(500);
                hWnd = FindWindow(null, windowName);
            }

            return hWnd;
        }

        public static IntPtr ReadProcessMemoryIntPtr(IntPtr hProcess, IntPtr lpBaseAddress)
        {
            return new IntPtr(ReadProcessMemoryInt32(hProcess, lpBaseAddress));

        }

        public static Int32 ReadProcessMemoryInt32(IntPtr hProcess, IntPtr lpBaseAddress)
        {
            byte[] buffer = new byte[4];
            int bytesread;

            ReadProcessMemory(hProcess, lpBaseAddress, buffer, 4, out bytesread);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static UInt32 ReadProcessMemoryUInt32(IntPtr hProcess, IntPtr lpBaseAddress)
        {
            byte[] buffer = new byte[4];
            int bytesread;

            ReadProcessMemory(hProcess, lpBaseAddress, buffer, 4, out bytesread);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public static byte[] ReadProcessMemoryByteArray(IntPtr hProcess, IntPtr lpBaseAddress, int size)
        {
            byte[] lpBuffer = new byte[size];
            ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, size, 0);
            return lpBuffer;
        }

        public static short ReadProcessMemoryWord(IntPtr hProcess, IntPtr lpBaseAddress)
        {
            byte[] buffer = new byte[2];
            int bytesread;

            ReadProcessMemory(hProcess, lpBaseAddress, buffer, 2, out bytesread);
            return BitConverter.ToInt16(buffer, 0);
        }

        public static float ReadProcessMemoryFloat(IntPtr hProcess, IntPtr lpBaseAddress)
        {
            byte[] buffer = new byte[4];
            int bytesread;

            ReadProcessMemory(hProcess, lpBaseAddress, buffer, 4, out bytesread);
            return BitConverter.ToSingle(buffer, 0);
        }


        public static string ReadProcessMemoryString(IntPtr hProcess, IntPtr lpBaseAddress, int length)
        {
            byte[] buffer = new byte[length];
            int bytesread;

            ReadProcessMemory(hProcess, lpBaseAddress, buffer, length, out bytesread);
            string result = string.Empty;
            for (int i = 0; i < bytesread; i++)
            {
                if (buffer[i] > 0)
                    result += Convert.ToChar(buffer[i]);
                else break;

            }
            return result;

        }

    }
}
