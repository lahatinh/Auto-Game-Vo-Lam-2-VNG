using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using AutoClient;
using AutoClient.Entities;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;


namespace AutoPK
{
    public partial class Main : Form
    {
        private Dictionary<IntPtr, AutoClientBS> _clients = new Dictionary<IntPtr, AutoClientBS>();
        private List<IntPtr> _clientHwnds = new List<IntPtr>();
        private List<String> _Danhsachbuff = new List<string>();
        private IntPtr currentSelectedChar;
        private bool Autobuffnmk = true;
        private string PlayerTheoSau = string.Empty;
        public int StepbufNMK;
        private AutoClientBS HanldeBuffNMK;
        private AutoClientBS HanldeBuffTLQ;
        public int ProsX;
        public int ProsY;



        // Chương trình Hook keyboard
        #region Windows structure definitions

        /// <summary>
        /// The POINT structure defines the x- and y- coordinates of a point. 
        /// </summary>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/gdi/rectangl_0tiq.asp
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private class POINT
        {
            /// <summary>
            /// Specifies the x-coordinate of the point. 
            /// </summary>
            public int x;
            /// <summary>
            /// Specifies the y-coordinate of the point. 
            /// </summary>
            public int y;
        }

        /// <summary>
        /// The MOUSEHOOKSTRUCT structure contains information about a mouse event passed to a WH_MOUSE hook procedure, MouseProc. 
        /// </summary>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookstructures/cwpstruct.asp
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private class MouseHookStruct
        {
            /// <summary>
            /// Specifies a POINT structure that contains the x- and y-coordinates of the cursor, in screen coordinates. 
            /// </summary>
            public POINT pt;
            /// <summary>
            /// Handle to the window that will receive the mouse message corresponding to the mouse event. 
            /// </summary>
            public int hwnd;
            /// <summary>
            /// Specifies the hit-test value. For a list of hit-test values, see the description of the WM_NCHITTEST message. 
            /// </summary>
            public int wHitTestCode;
            /// <summary>
            /// Specifies extra information associated with the message. 
            /// </summary>
            public int dwExtraInfo;
        }

        /// <summary>
        /// The MSLLHOOKSTRUCT structure contains information about a low-level keyboard input event. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private class MouseLLHookStruct
        {
            /// <summary>
            /// Specifies a POINT structure that contains the x- and y-coordinates of the cursor, in screen coordinates. 
            /// </summary>
            public POINT pt;
            /// <summary>
            /// If the message is WM_MOUSEWHEEL, the high-order word of this member is the wheel delta. 
            /// The low-order word is reserved. A positive value indicates that the wheel was rotated forward, 
            /// away from the user; a negative value indicates that the wheel was rotated backward, toward the user. 
            /// One wheel click is defined as WHEEL_DELTA, which is 120. 
            ///If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP,
            /// or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released, 
            /// and the low-order word is reserved. This value can be one or more of the following values. Otherwise, mouseData is not used. 
            ///XBUTTON1
            ///The first X button was pressed or released.
            ///XBUTTON2
            ///The second X button was pressed or released.
            /// </summary>
            public int mouseData;
            /// <summary>
            /// Specifies the event-injected flag. An application can use the following value to test the mouse flags. Value Purpose 
            ///LLMHF_INJECTED Test the event-injected flag.  
            ///0
            ///Specifies whether the event was injected. The value is 1 if the event was injected; otherwise, it is 0.
            ///1-15
            ///Reserved.
            /// </summary>
            public int flags;
            /// <summary>
            /// Specifies the time stamp for this message.
            /// </summary>
            public int time;
            /// <summary>
            /// Specifies extra information associated with the message. 
            /// </summary>
            public int dwExtraInfo;
        }


        /// <summary>
        /// The KBDLLHOOKSTRUCT structure contains information about a low-level keyboard input event. 
        /// </summary>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookstructures/cwpstruct.asp
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private class KeyboardHookStruct
        {
            /// <summary>
            /// Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
            /// </summary>
            public int vkCode;
            /// <summary>
            /// Specifies a hardware scan code for the key. 
            /// </summary>
            public int scanCode;
            /// <summary>
            /// Specifies the extended-key flag, event-injected flag, context code, and transition-state flag.
            /// </summary>
            public int flags;
            /// <summary>
            /// Specifies the time stamp for this message.
            /// </summary>
            public int time;
            /// <summary>
            /// Specifies extra information associated with the message. 
            /// </summary>
            public int dwExtraInfo;
        }
        #endregion

        #region Windows function imports
        /// <summary>
        /// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain. 
        /// You would install a hook procedure to monitor the system for certain types of events. These events 
        /// are associated either with a specific thread or with all threads in the same desktop as the calling thread. 
        /// </summary>
        /// <param name="idHook">
        /// [in] Specifies the type of hook procedure to be installed. This parameter can be one of the following values.
        /// </param>
        /// <param name="lpfn">
        /// [in] Pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a 
        /// thread created by a different process, the lpfn parameter must point to a hook procedure in a dynamic-link 
        /// library (DLL). Otherwise, lpfn can point to a hook procedure in the code associated with the current process.
        /// </param>
        /// <param name="hMod">
        /// [in] Handle to the DLL containing the hook procedure pointed to by the lpfn parameter. 
        /// The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by 
        /// the current process and if the hook procedure is within the code associated with the current process. 
        /// </param>
        /// <param name="dwThreadId">
        /// [in] Specifies the identifier of the thread with which the hook procedure is to be associated. 
        /// If this parameter is zero, the hook procedure is associated with all existing threads running in the 
        /// same desktop as the calling thread. 
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the handle to the hook procedure.
        /// If the function fails, the return value is NULL. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
           CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int SetWindowsHookEx(
            int idHook,
            HookProc lpfn,
            IntPtr hMod,
            int dwThreadId);

        /// <summary>
        /// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function. 
        /// </summary>
        /// <param name="idHook">
        /// [in] Handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to SetWindowsHookEx. 
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);

        /// <summary>
        /// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain. 
        /// A hook procedure can call this function either before or after processing the hook information. 
        /// </summary>
        /// <param name="idHook">Ignored.</param>
        /// <param name="nCode">
        /// [in] Specifies the hook code passed to the current hook procedure. 
        /// The next hook procedure uses this code to determine how to process the hook information.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies the wParam value passed to the current hook procedure. 
        /// The meaning of this parameter depends on the type of hook associated with the current hook chain. 
        /// </param>
        /// <param name="lParam">
        /// [in] Specifies the lParam value passed to the current hook procedure. 
        /// The meaning of this parameter depends on the type of hook associated with the current hook chain. 
        /// </param>
        /// <returns>
        /// This value is returned by the next hook procedure in the chain. 
        /// The current hook procedure must also return this value. The meaning of the return value depends on the hook type. 
        /// For more information, see the descriptions of the individual hook procedures.
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
             CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(
            int idHook,
            int nCode,
            int wParam,
            IntPtr lParam);

        /// <summary>
        /// The CallWndProc hook procedure is an application-defined or library-defined callback 
        /// function used with the SetWindowsHookEx function. The HOOKPROC type defines a pointer 
        /// to this callback function. CallWndProc is a placeholder for the application-defined 
        /// or library-defined function name.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/callwndproc.asp
        /// </remarks>
        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        /// <summary>
        /// The ToAscii function translates the specified virtual-key code and keyboard 
        /// state to the corresponding character or characters. The function translates the code 
        /// using the input language and physical keyboard layout identified by the keyboard layout handle.
        /// </summary>
        /// <param name="uVirtKey">
        /// [in] Specifies the virtual-key code to be translated. 
        /// </param>
        /// <param name="uScanCode">
        /// [in] Specifies the hardware scan code of the key to be translated. 
        /// The high-order bit of this value is set if the key is up (not pressed). 
        /// </param>
        /// <param name="lpbKeyState">
        /// [in] Pointer to a 256-byte array that contains the current keyboard state. 
        /// Each element (byte) in the array contains the state of one key. 
        /// If the high-order bit of a byte is set, the key is down (pressed). 
        /// The low bit, if set, indicates that the key is toggled on. In this function, 
        /// only the toggle bit of the CAPS LOCK key is relevant. The toggle state 
        /// of the NUM LOCK and SCROLL LOCK keys is ignored.
        /// </param>
        /// <param name="lpwTransKey">
        /// [out] Pointer to the buffer that receives the translated character or characters. 
        /// </param>
        /// <param name="fuState">
        /// [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise. 
        /// </param>
        /// <returns>
        /// If the specified key is a dead key, the return value is negative. Otherwise, it is one of the following values. 
        /// Value Meaning 
        /// 0 The specified virtual key has no translation for the current state of the keyboard. 
        /// 1 One character was copied to the buffer. 
        /// 2 Two characters were copied to the buffer. This usually happens when a dead-key character 
        /// (accent or diacritic) stored in the keyboard layout cannot be composed with the specified 
        /// virtual key to form a single character. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
        /// </remarks>
        [DllImport("user32")]
        private static extern int ToAscii(
            int uVirtKey,
            int uScanCode,
            byte[] lpbKeyState,
            byte[] lpwTransKey,
            int fuState);

        /// <summary>
        /// The GetKeyboardState function copies the status of the 256 virtual keys to the 
        /// specified buffer. 
        /// </summary>
        /// <param name="pbKeyState">
        /// [in] Pointer to a 256-byte array that contains keyboard key states. 
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
        /// </remarks>
        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        #endregion

        #region Windows constants

        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level keyboard  input events.
        /// </summary>
        private const int WH_KEYBOARD_LL = 13;
        /// <summary>
        /// The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem 
        /// key is pressed. A nonsystem key is a key that is pressed when the ALT key is not pressed.
        /// </summary>
        private const int WM_KEYDOWN = 0x100;
        /// <summary>
        /// The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem 
        /// key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed, 
        /// or a keyboard key that is pressed when a window has the keyboard focus.
        /// </summary>
        private const int WM_KEYUP = 0x101;
        /// <summary>
        /// The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user 
        /// presses the F10 key (which activates the menu bar) or holds down the ALT key and then 
        /// presses another key. It also occurs when no window currently has the keyboard focus; 
        /// in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that 
        /// receives the message can distinguish between these two contexts by checking the context 
        /// code in the lParam parameter. 
        /// </summary>
        private const int WM_SYSKEYDOWN = 0x104;
        /// <summary>
        /// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user 
        /// releases a key that was pressed while the ALT key was held down. It also occurs when no 
        /// window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent 
        /// to the active window. The window that receives the message can distinguish between 
        /// these two contexts by checking the context code in the lParam parameter. 
        /// </summary>
        private const int WM_SYSKEYUP = 0x105;

        private const byte VK_SHIFT = 0x10;
        private const byte VK_CONTROL = 0x11;
        private const byte VK_MENU = 0x12;
        private const byte VK_CAPITAL = 0x14;
        private const byte VK_NUMLOCK = 0x90;

        #endregion

        #region Chạy chương trình Hook Keyboard & Auto Thay do

        /// <summary>
        /// Stores the handle to the keyboard hook procedure.
        /// </summary>
        private int hKeyboardHook = 0;


        /// <summary>
        /// Declare KeyboardHookProcedure as HookProc type.
        /// </summary>
        private static HookProc KeyboardHookProcedure;


        /// <summary>
        /// Installs both mouse and keyboard hooks and starts rasing events
        /// </summary>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public void Start()
        {
            this.Start(true);
        }

        /// <summary>
        /// Installs both or one of mouse and/or keyboard hooks and starts rasing events
        /// </summary>
        /// <param name="InstallKeyboardHook"><b>true</b> if keyboard events must be monitored</param>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public void Start(bool InstallKeyboardHook)
        {
            // install Keyboard hook only if it is not installed and must be installed
            if (hKeyboardHook == 0 && InstallKeyboardHook)
            {
                // Create an instance of HookProc.
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                //install hook
                hKeyboardHook = SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    KeyboardHookProcedure,
                    Marshal.GetHINSTANCE(
                    Assembly.GetExecutingAssembly().GetModules()[0]),
                    0);
                //If SetWindowsHookEx fails.
                if (hKeyboardHook == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //do cleanup
                    Stop(true, false);
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        /// <summary>
        /// Stops monitoring both mouse and keyboard events and rasing events.
        /// </summary>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public void Stop()
        {
            this.Stop(true, true);
        }

        /// <summary>
        /// Stops monitoring both or one of mouse and/or keyboard events and rasing events.
        /// </summary>
        /// <param name="UninstallMouseHook"><b>true</b> if mouse hook must be uninstalled</param>
        /// <param name="UninstallKeyboardHook"><b>true</b> if keyboard hook must be uninstalled</param>
        /// <param name="ThrowExceptions"><b>true</b> if exceptions which occured during uninstalling must be thrown</param>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public void Stop(bool UninstallKeyboardHook, bool ThrowExceptions)
        {
            //if keyboard hook set and must be uninstalled
            if (hKeyboardHook != 0 && UninstallKeyboardHook)
            {
                //uninstall hook
                int retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                //reset invalid handle
                hKeyboardHook = 0;
                //if failed and exception must be thrown
                if (retKeyboard == 0 && ThrowExceptions)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        /// <summary>
        /// A callback function which will be called every time a keyboard activity detected.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>

        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //indicates if any of underlaing events set e.Handled flag
            bool handled = false;
            //it was ok and someone listens to events
            if ((nCode >= 0))
            {
                //read structure KeyboardHookStruct at lParam
                KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                //raise KeyDown
                if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    keyData |= ((GetKeyState(VK_SHIFT) & 0x80) == 0x80 ? Keys.Shift : Keys.None);
                    keyData |= ((GetKeyState(VK_CONTROL) & 0x80) == 0x80 ? Keys.Control : Keys.None);
                    keyData |= ((GetKeyState(VK_MENU) & 0x80) == 0x80 ? Keys.Menu : Keys.None);
                    Console.WriteLine(keyData);
                    KeyEventArgs e = new KeyEventArgs(keyData);

                    foreach (var autoClient in _clients)
                    {
                        if (!autoClient.Value.isInjected)
                            autoClient.Value.Inject();

                        if (e.KeyData.ToString() == autoClient.Value.phimdung.ToString() && autoClient.Value.ComboPKTLQ == true)
                        {
                            log = e.KeyData.ToString();

                            return (Int32)1;
                        }
                        //
                        if (e.KeyData.ToString() == autoClient.Value.phimvac.ToString() && autoClient.Value.ComboPKTLQ == true)
                        {
                            log = e.KeyData.ToString();

                            return (Int32)1;
                        }
                        //
                        if (e.KeyData.ToString() == autoClient.Value.phimvacdt.ToString() && autoClient.Value.ComboPKTLQ == true)
                        {

                            log = e.KeyData.ToString();

                            return (Int32)1;
                        }
                        //
                        if (e.KeyData.ToString() == autoClient.Value.phimvaccdt.ToString() && autoClient.Value.ComboPKTLQ == true)
                        {

                            log = e.KeyData.ToString();

                            return (Int32)1;
                        }
                        //
                        if (e.KeyData.ToString() == autoClient.Value.phimdt1o.ToString() && autoClient.Value.ComboPKTLQ == true)
                        {
                            log = e.KeyData.ToString();

                            return (Int32)1;
                        }

                        //
                        if (e.KeyData.ToString() == autoClient.Value.phimthaydo1.ToString()&& autoClient.Value.ckthaydobo1 == true)
                        {
                            foreach (string item1 in autoClient.Value.Listthaydo1)
                            {
                                autoClient.Value.FindAndUseItemID(item1);
                            }

                            return (Int32)1;
                        }

                        //
                        if (e.KeyData.ToString() == autoClient.Value.phimthaydo2.ToString() && autoClient.Value.ckthaydobo2 == true)
                        {
                            foreach (string item2 in autoClient.Value.Listthaydo2)
                            {
                                autoClient.Value.FindAndUseItemID(item2);
                            }

                            return (Int32)1;

                        }

                        //
                        if (e.KeyData.ToString() == autoClient.Value.phimthaydo3.ToString() && autoClient.Value.ckthaydobo3 == true)
                        {
                            foreach (string item3 in autoClient.Value.Listthaydo3)
                            {
                                autoClient.Value.FindAndUseItemID(item3);
                            }

                            return (Int32)1;
                        }
                    }

                }

            }

            //if event handled in application do not handoff to other listeners
            if (handled)
                return (Int32)1;
            else
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }
        #endregion


        // Định dạng toàn cục cho các biến
        string log = string.Empty;

        public Main()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
       }

        // Sự kiện chọn Handle nhân vật

        public AutoClientBS CurrentClient
        {
            get
            {
                if (currentSelectedChar.ToInt32() == 0)
                    return null;
                if (!_clients.ContainsKey(currentSelectedChar))
                    return null;
                else
                    return _clients[currentSelectedChar];
            }
        }

        // Sự kiện chọn Handle NMK
        public AutoClientBS ClientbuffNMK
        {
            get
            {
               return ClientbuffNMK = HanldeBuffNMK;
            }
            set
            {
                if (CurrentClient != null &&CurrentClient.CurrentPlayer.Hephai == 8)
                    HanldeBuffNMK = CurrentClient;
            }
        }
        // Sự kiện chọn Handle TLQ
        public AutoClientBS ClientbuffTLQ
        {
            get
            {
                return ClientbuffTLQ = HanldeBuffTLQ;
            }
            set
            {
                if (CurrentClient != null && CurrentClient.CurrentPlayer.Hephai == 4)
                    HanldeBuffTLQ = CurrentClient;
            }
        }

        // Main load
        private void Main_Load(object sender, EventArgs e)
        {
            // Tạo thread mới chạy song song với Main chạy hàm checkchars
            Thread thrCheckChars = new Thread(new ThreadStart(CheckChars));
            thrCheckChars.IsBackground = true;
            thrCheckChars.Start();

            // Tạo thread mới chạy song song với Main chạy hàm UpdateCharStat
            Thread thrUpdateCharStat = new Thread(new ThreadStart(UpdateCharStat));
            thrUpdateCharStat.IsBackground = true;
            thrUpdateCharStat.Start();

            // Tạo thread mới chạy song song với Main chạy hàm bơm máu
            Thread thrbommau = new Thread(new ThreadStart(bommau));
            thrbommau.IsBackground = true;
            thrbommau.Start();

            // Tạo thread mới chạy song song với Main chạy hàm lấy tọa độ nhân vật
            Thread thrgettoadonv = new Thread(new ThreadStart(Gettoadonv));
            thrgettoadonv.IsBackground = true;
            thrgettoadonv.Start();

            // Chạy thead auto Buff NMK
            Thread thrAutobuff = new Thread(new ThreadStart(Autobuff));
            thrAutobuff.IsBackground = true;
            thrAutobuff.Start();

            // Tạo một thread mới để chạy hàm pkcombo
            Thread thrpkcombo = new Thread(new ThreadStart(pkcombo));
            thrpkcombo.IsBackground = true;
            thrpkcombo.Start();

            // Tạo một thread mới để chạy hàm pkcombo
            Thread thrtheosau = new Thread(new ThreadStart(theosau));
            thrtheosau.IsBackground = true;
            thrtheosau.Start();

            // Tạo một thread mới để chạy hàm click NPC
            Thread thrClickNpc = new Thread(new ThreadStart(ClickNPc));
            thrClickNpc.IsBackground = true;
            thrClickNpc.Start();

            Start();
        }


        // Thủ tục get tọa độ nhân vật lên label form
        private void Gettoadonv()
        {
            while (true)
            {
                Thread.Sleep(500);

                foreach (var autoClient in _clients)
                {

                    if (!autoClient.Value.isInjected)
                        autoClient.Value.Inject();

                    if (CurrentClient != null)
                    {

                        ProsX = CurrentClient.CurrentPlayer.PositionX;
                        ProsY = CurrentClient.CurrentPlayer.PositionY;

                        lbprosX.Invoke(new MethodInvoker(() =>
                        {
                            lbprosX.Text = ProsX.ToString();
                        }));

                        lbProsY.Invoke(new MethodInvoker(() =>
                        {
                            lbProsY.Text = ProsY.ToString();
                        }));
                    }

                }
            }
        }

        // Hàm bơm máu
        private void bommau()
        {
            while (true)
            {
                Thread.Sleep(300);

            foreach (var autoClient in _clients)
            {
                if (!autoClient.Value.FeatureHoiPhuc || !autoClient.Value.ischecked)
                    continue;

                if (!autoClient.Value.isInjected)
                    autoClient.Value.Inject();

                // Thủ tục ăn máu
                    if (autoClient.Value.CurrentPlayer.HitPoint <= autoClient.Value.HPuse && autoClient.Value.CurrentPlayer.HitPoint > 1000 &&(autoClient.Value.ckbommau == true || autoClient.Value.ckbomsinhnoi == true || autoClient.Value.ckancc == true))
                    {
                        autoClient.Value.FindAndUseItem(autoClient.Value.ItemSinhLuc.ToString());
                        Thread.Sleep(autoClient.Value.Delay);

                        //Tạm nghỉ threat trong thời gian nhất định
                        if (autoClient.Value.CurrentPlayer.HitPoint <= autoClient.Value.SinhnoiUse && autoClient.Value.CurrentPlayer.HitPoint > 1000 && (autoClient.Value.ckbomsinhnoi == true || autoClient.Value.ckancc == true))
                        {
                        autoClient.Value.FindAndUseItem(autoClient.Value.ItemSinhNoi.ToString());
                        Thread.Sleep(autoClient.Value.Delay);

                            //Tạm nghỉ threat trong thời gian nhất định
                            if (autoClient.Value.CurrentPlayer.HitPoint <= autoClient.Value.CCuse && autoClient.Value.CurrentPlayer.HitPoint > 1000 && autoClient.Value.ckancc == true)
                            {
                            autoClient.Value.FindAndUseItem(autoClient.Value.ItemCuuChuyen.ToString());
                            Thread.Sleep(autoClient.Value.Delay);
                            }
                        }
                    }

                    // Thủ tục ăn mana
                    if (autoClient.Value.CurrentPlayer.Mana <= autoClient.Value.ManaUse && autoClient.Value.ckbommana == true)
                    {
                        autoClient.Value.FindAndUseItem(autoClient.Value.ItemNoiLuc.ToString());
                    }
                    // Thủ tục Ăn Thể lực
                    if (autoClient.Value.CurrentPlayer.TheLuc <= autoClient.Value.ThelucUse && autoClient.Value.ckantheluc == true)
                    {
                        autoClient.Value.FindAndUseItem(autoClient.Value.ItemTheLuc.ToString());
                    }

                    // Thủ tục Ăn lak chiến trường
                    if (autoClient.Value.CurrentPlayer.HitPoint <= autoClient.Value.LakCTUse && autoClient.Value.ckanlakctr == true)
                    {
                       autoClient.Value.FindAndUseItem(autoClient.Value.ItemLakCtr.ToString());
                    }
                }
            }
        }
        
        // Main đóng
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Giải phóng bộ nhớ khi đóng form
            this.Dispose();
            GC.Collect();
        }

        // Thủ tục kiểm tra giá trị khi thoát khỏi textbox
        private void txbtimeVAC_Validated(object sender, EventArgs e)
        {
            if (txbtimeVAC.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbtimeVAC.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.timevac = Convert.ToInt32(txbtimeVAC.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                    WinAPI.Ghifile(text, "ComboTLQ", "timevac", CurrentClient.timevac.ToString());
                }
            }
        }

        // Thủ tục không cho nhập giá trị khác số vào textbox
        private void txbtimeDT_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("0123456789".IndexOf(e.KeyChar) != -1)
            {
                e.Handled = false;
            }
            else
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        // Thủ tục không cho nhập giá trị khác số vào textbox
        private void txbtimerunVAC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("0123456789".IndexOf(e.KeyChar) != -1)
            {
                e.Handled = false;
            }
            else
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        // Thủ tục không cho nhập giá trị khác số vào textbox
        private void txbtimekkVAC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("0123456789".IndexOf(e.KeyChar) != -1)
            {
                e.Handled = false;
            }
            else
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        // Thủ tục không cho nhập giá trị khác số vào textbox
        private void txbtimeVAC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("0123456789".IndexOf(e.KeyChar) != -1)
            {
                e.Handled = false;
            }
            else
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        // Thủ tục kiểm tra giá trị khi thoát khỏi textbox
        private void txbtimeDT_Validated(object sender, EventArgs e)
        {
            if (txbtimeDT.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbtimeDT.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {

                    CurrentClient.timedt = Convert.ToInt32(txbtimeDT.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                    WinAPI.Ghifile(text, "ComboTLQ", "timedt", CurrentClient.timedt.ToString());
                }
            }
        }

        // Thủ tục kiểm tra giá trị khi thoát khỏi textbox
        private void txbtimerunVAC_Validated(object sender, EventArgs e)
        {
            if (txbtimecdt.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbtimecdt.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.timerun = Convert.ToInt32(txbtimerunVAC.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                    WinAPI.Ghifile(text, "ComboTLQ", "timerun", CurrentClient.timerun.ToString());
                }
            }
        }

        // Hàm pkcombo
        private void pkcombo()
        {
            while (true)
            {
                Thread.Sleep(100);

                if (ClientbuffTLQ != null && ClientbuffTLQ.ComboPKTLQ == true && ClientbuffTLQ.ischecked == true && ClientbuffTLQ.CurrentPlayer.Hephai == 4)
                {

                    ClientbuffTLQ.Inject();
                    //
                    if (log.ToString() == ClientbuffTLQ.phimvac.ToString() && ClientbuffTLQ.cksutkngatvac == true)
                    {
                        var ProsXnew = 2 * Convert.ToInt32(ClientbuffTLQ.CurrentPlayer.PositionX.ToString()) - ProsX;
                        var ProsYnew = 2 * Convert.ToInt32(ClientbuffTLQ.CurrentPlayer.PositionY.ToString()) - ProsY;

                        while (log.ToString() == ClientbuffTLQ.phimvac.ToString() && ClientbuffTLQ.cksutkngatvac == true)
                        {
                            ClientbuffTLQ.BuffSkill(GameConst.VACSKILL, ProsXnew, ProsYnew);
                            Thread.Sleep(100);
                        }
                    }
                    else if (log.ToString() == ClientbuffTLQ.phimvac.ToString() && ClientbuffTLQ.cksutngatvac == true)
                    {
                        var ProsXnew = 2 * Convert.ToInt32(ClientbuffTLQ.CurrentPlayer.PositionX.ToString()) - ProsX;
                        var ProsYnew = 2 * Convert.ToInt32(ClientbuffTLQ.CurrentPlayer.PositionY.ToString()) - ProsY;

                        while (log.ToString() == ClientbuffTLQ.phimvac.ToString() && ClientbuffTLQ.cksutngatvac == true)
                        {
                            ClientbuffTLQ.BuffSkill(GameConst.VACSKILL, ProsXnew, ProsYnew);
                            Thread.Sleep(ClientbuffTLQ.timevac);
                            ClientbuffTLQ.LeftClick(ProsXnew, ProsYnew);
                        }
                    }
                    else if (log.ToString() == ClientbuffTLQ.phimvac.ToString() && ClientbuffTLQ.ckxdamevac == true)
                    {
                        var ProsXnew = 2 * Convert.ToInt32(ClientbuffTLQ.CurrentPlayer.PositionX.ToString()) - ProsX;
                        var ProsYnew = 2 * Convert.ToInt32(ClientbuffTLQ.CurrentPlayer.PositionY.ToString()) - ProsY;
                        //
                        ClientbuffTLQ.BuffSkill(GameConst.VACSKILL, ProsXnew, ProsYnew);
                        Thread.Sleep(ClientbuffTLQ.timevac);
                        ClientbuffTLQ.LeftClick(ProsXnew, ProsYnew);
                        //
                        while (log.ToString() == ClientbuffTLQ.phimvac.ToString() && ClientbuffTLQ.ckxdamevac == true)
                        {
                            ClientbuffTLQ.BuffSkill(GameConst.VACSKILL, ProsXnew, ProsYnew);
                            Thread.Sleep(ClientbuffTLQ.timevac);
                            ClientbuffTLQ.LeftClick(ProsXnew, ProsYnew);
                        }
                    }
                    else if (log.ToString() == ClientbuffTLQ.phimvacdt.ToString() && ClientbuffTLQ.ckcombodt == true)
                    {
                        var ProsXnew = 2 * Convert.ToInt32(ClientbuffTLQ.CurrentPlayer.PositionX.ToString()) - ProsX;
                        var ProsYnew = 2 * Convert.ToInt32(ClientbuffTLQ.CurrentPlayer.PositionY.ToString()) - ProsY;
                        //
                        while (log.ToString() == ClientbuffTLQ.phimvacdt.ToString() && ClientbuffTLQ.ckcombodt == true)
                        {
                            ClientbuffTLQ.BuffSkill(GameConst.VACSKILL, ProsXnew, ProsYnew);
                            Thread.Sleep(ClientbuffTLQ.timevac);
                            ClientbuffTLQ.LeftClick(ProsXnew, ProsYnew);
                            //
                            ClientbuffTLQ.BuffSkill(GameConst.DtuSKILL, ProsXnew, ProsYnew);
                            Thread.Sleep(ClientbuffTLQ.timevac);
                            ClientbuffTLQ.LeftClick(ProsXnew, ProsYnew);
                        }
                    }
                    else if (log.ToString() == ClientbuffTLQ.phimvaccdt.ToString() && ClientbuffTLQ.ckvaccdt == true)
                    {
                        var ProsXnew = 2 * Convert.ToInt32(ClientbuffTLQ.CurrentPlayer.PositionX.ToString()) - ProsX;
                        var ProsYnew = 2 * Convert.ToInt32(ClientbuffTLQ.CurrentPlayer.PositionY.ToString()) - ProsY;
                        //
                        while (log.ToString() == ClientbuffTLQ.phimvaccdt.ToString() && ClientbuffTLQ.ckvaccdt == true)
                        {
                            ClientbuffTLQ.BuffSkill(GameConst.VACSKILL, ProsXnew, ProsYnew);
                            Thread.Sleep(ClientbuffTLQ.timevac);
                            ClientbuffTLQ.LeftClick(ProsXnew, ProsYnew);
                            ClientbuffTLQ.BuffSkill(GameConst.CDTSKILL5, ProsXnew, ProsYnew);
                            Thread.Sleep(ClientbuffTLQ.timecdt);
                            ClientbuffTLQ.LeftClick(ProsXnew, ProsYnew);
                            ClientbuffTLQ.BuffSkill(GameConst.DtuSKILL, ProsXnew, ProsYnew);
                            Thread.Sleep(ClientbuffTLQ.timedt);
                            ClientbuffTLQ.LeftClick(ProsXnew, ProsYnew);
                        }
                    }
                    else if (log.ToString() == ClientbuffTLQ.phimdt1o.ToString() && ClientbuffTLQ.ckdt1o == true)
                    {
                        var ProsXnew = 2 * Convert.ToInt32(ClientbuffTLQ.CurrentPlayer.PositionX.ToString()) - ProsX;
                        var ProsYnew = 2 * Convert.ToInt32(ClientbuffTLQ.CurrentPlayer.PositionY.ToString()) - ProsY;
                        //
                        ClientbuffTLQ.BuffSkill(GameConst.VDQSKILL, ProsXnew, ProsYnew);
                        Thread.Sleep(ClientbuffTLQ.timedt);
                        ClientbuffTLQ.BuffSkill(GameConst.VDQSKILL, ProsXnew, ProsYnew);
                        Thread.Sleep(ClientbuffTLQ.timedt);
                        //
                        while (log.ToString() == ClientbuffTLQ.phimdt1o.ToString() && ClientbuffTLQ.ckdt1o == true)
                        {
                            ClientbuffTLQ.BuffSkill(GameConst.DtuSKILL, ProsXnew, ProsYnew);
                            Thread.Sleep(100);
                        }
                    }
                }
            }

        }

        // Thủ tục không cho nhập giá trị khác số vào textbox
        private void txbHPuse_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("0123456789".IndexOf(e.KeyChar) != -1)
            {
                e.Handled = false;
            }
            else
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        // Thủ tục không cho nhập giá trị khác số vào textbox
        private void txtDelay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("0123456789".IndexOf(e.KeyChar) != -1)
            {
                e.Handled = false;
            }
            else
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }


        // Thủ tục kiểm tra giá trị khi thoát khỏi textbox
        private void txtDelay_Validated(object sender, EventArgs e)
        {
            if (txbDelay.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbDelay.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.Delay = Convert.ToInt32(txbDelay.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                    WinAPI.Ghifile(text, "PhucHoi", "Delay", CurrentClient.Delay.ToString());
                }
            }
        }

        //Lưu lại các handle vào Client
        private bool SearchForGameWindows(IntPtr hwnd, IntPtr lParam)
        {
            var title = new StringBuilder(100);
            WinAPI.GetWindowText(hwnd, title, title.Capacity);
            if (title.ToString() == "Vâ L©m 2 ()")
            {
                _clientHwnds.Add(hwnd);
            }
            return true;
        }

        // Click vào check box chọn tất cả các acout trên listviews
        private void checkBoxAcount_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                for (int i = 0; i < this.lsvplayer.Items.Count; i++)
                {
                    lsvplayer.Items[i].Checked = true;
                }
            }
            else
            {
                for (int i = 0; i < this.lsvplayer.Items.Count; i++)
                {
                    lsvplayer.Items[i].Checked = false;
                }
            }
        }

        // Click vào check box chọn acout trên listviews
        private void lsvplayer_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            lsvplayer.Invoke(new MethodInvoker(() =>
            {
            lsvplayer.Items[e.Index].Selected = true;
            CurrentClient.ischecked = (e.CurrentValue == CheckState.Unchecked) ? true : false;
            }));
        }

        // Click vào chọn dòng acout trên listviews
        private void lsvplayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            lsvplayer.Invoke(new MethodInvoker(() =>
            {
                if (lsvplayer.SelectedItems.Count == 0)
                {
                    currentSelectedChar = new IntPtr(0);
                    return;
                }
                var st = lsvplayer.SelectedItems[0].Text;

                currentSelectedChar = new IntPtr(Convert.ToInt32(st));

                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;

                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";

                // Checkbox
                CurrentClient.FeatureHoiPhuc = bool.Parse(WinAPI.Docfile(text, "CheckPhucHoi", "CheckPhucHoi.Check1", "false"));
                CurrentClient.ComboPKTLQ = bool.Parse(WinAPI.Docfile(text, "CheckComboTLQ", "CheckComboTLQ.Check1", "false"));
                CurrentClient.cksutkngatvac = bool.Parse(WinAPI.Docfile(text, "CheckComboTLQ", "CheckComboTLQ.Check2", "false"));
                CurrentClient.ckxdamevac = bool.Parse(WinAPI.Docfile(text, "CheckComboTLQ", "CheckComboTLQ.Check3", "false"));
                CurrentClient.cksutngatvac = bool.Parse(WinAPI.Docfile(text, "CheckComboTLQ", "CheckComboTLQ.Check4", "false"));
                CurrentClient.ckcombodt = bool.Parse(WinAPI.Docfile(text, "CheckComboTLQ", "CheckComboTLQ.Check5", "false"));
                CurrentClient.ckvaccdt = bool.Parse(WinAPI.Docfile(text, "CheckComboTLQ", "CheckComboTLQ.Check6", "false"));
                CurrentClient.ckdt1o = bool.Parse(WinAPI.Docfile(text, "CheckComboTLQ", "CheckComboTLQ.Check7", "false"));
                CurrentClient.Featurenmk = bool.Parse(WinAPI.Docfile(text, "CheckBuff", "CheckBuff.Check1", "false"));

                CurrentClient.Bufftheosau = bool.Parse(WinAPI.Docfile(text, "CheckBuff", "CheckBuff.Check2", "false"));
                CurrentClient.buffdanhsach = bool.Parse(WinAPI.Docfile(text, "CheckBuff", "CheckBuff.Check3", "false"));

                CurrentClient.ckthaydobo1 = bool.Parse(WinAPI.Docfile(text, "CheckThaydo", "CheckThaydo.Check1", "false"));
                CurrentClient.ckthaydobo2 = bool.Parse(WinAPI.Docfile(text, "CheckThaydo", "CheckThaydo.Check2", "false"));
                CurrentClient.ckthaydobo3 = bool.Parse(WinAPI.Docfile(text, "CheckThaydo", "CheckThaydo.Check3", "false"));

                CurrentClient.ckbommau = bool.Parse(WinAPI.Docfile(text, "CheckPhucHoi", "CheckPhucHoi.Check2", "false"));
                CurrentClient.ckbomsinhnoi = bool.Parse(WinAPI.Docfile(text, "CheckPhucHoi", "CheckPhucHoi.Check3", "false"));
                CurrentClient.ckbommana = bool.Parse(WinAPI.Docfile(text, "CheckPhucHoi", "CheckPhucHoi.Check4", "false"));
                CurrentClient.ckancc = bool.Parse(WinAPI.Docfile(text, "CheckPhucHoi", "CheckPhucHoi.Check5", "false"));
                CurrentClient.ckanlakctr = bool.Parse(WinAPI.Docfile(text, "CheckPhucHoi", "CheckPhucHoi.Check6", "false"));
                CurrentClient.ckantheluc = bool.Parse(WinAPI.Docfile(text, "CheckPhucHoi", "CheckPhucHoi.Check7", "false"));

                CurrentClient.ckClickNPC = bool.Parse(WinAPI.Docfile(text, "AutoClickNPC", "AutoClickNPC.Check1", "false"));
                CurrentClient.ckClickMenu1 = bool.Parse(WinAPI.Docfile(text, "AutoClickNPC", "AutoClickNPC.Check2", "false"));
                CurrentClient.ckClickMenu2 = bool.Parse(WinAPI.Docfile(text, "AutoClickNPC", "AutoClickNPC.Check3", "false"));
                CurrentClient.ckClickMenu3 = bool.Parse(WinAPI.Docfile(text, "AutoClickNPC", "CAutoClickNPC.Check4", "false"));
                CurrentClient.ckClickMenu4 = bool.Parse(WinAPI.Docfile(text, "AutoClickNPC", "AutoClickNPC.Check5", "false"));


                // Value
                CurrentClient.timevac = Convert.ToInt32(WinAPI.Docfile(text, "ComboTLQ", "timeVAC", "0"));
                CurrentClient.timedt = Convert.ToInt32(WinAPI.Docfile(text, "ComboTLQ", "timedt", "0"));
                CurrentClient.timecdt = Convert.ToInt32(WinAPI.Docfile(text, "ComboTLQ", "timecdt", "0"));
                CurrentClient.timerun = Convert.ToInt32(WinAPI.Docfile(text, "ComboTLQ", "timerun", "0"));

                CurrentClient.HPuse = Convert.ToInt32(WinAPI.Docfile(text, "PhucHoi", "HPuse", "0"));
                CurrentClient.CCuse = Convert.ToInt32(WinAPI.Docfile(text, "PhucHoi", "CCuse", "0"));
                CurrentClient.Delay = Convert.ToInt32(WinAPI.Docfile(text, "PhucHoi", "Delay", "0"));

                CurrentClient.SinhnoiUse = Convert.ToInt32(WinAPI.Docfile(text, "PhucHoi", "SinhnoiUse", "0"));
                CurrentClient.ManaUse = Convert.ToInt32(WinAPI.Docfile(text, "PhucHoi", "ManaUse", "0"));
                CurrentClient.LakCTUse = Convert.ToInt32(WinAPI.Docfile(text, "PhucHoi", "LakCTUse", "0"));
                CurrentClient.ThelucUse = Convert.ToInt32(WinAPI.Docfile(text, "PhucHoi", "ThelucUse", "0"));

                CurrentClient.ValueClickNPC = WinAPI.Docfile(text, "AutoClickNPC", "AutoClickNPC.ValueClickNPC", "");
                CurrentClient.ValueClickMenu1 = WinAPI.Docfile(text, "AutoClickNPC", "AutoClickNPC.ValueClickMenu1", "");
                CurrentClient.ValueClickMenu2 = WinAPI.Docfile(text, "AutoClickNPC", "AutoClickNPC.ValueClickMenu2", "");
                CurrentClient.ValueClickMenu3 = WinAPI.Docfile(text, "AutoClickNPC", "AutoClickNPC.ValueClickMenu3", "");
                CurrentClient.ValueClickMenu4 = WinAPI.Docfile(text, "AutoClickNPC", "AutoClickNPC.ValueClickMenu4", "");
                CurrentClient.ValueGiancach = Convert.ToInt32(WinAPI.Docfile(text, "AutoClickNPC", "AutoClickNPC.ValueGiancach", "0"));

                // Keyboard
                CurrentClient.phimdung = WinAPI.Docfile(text, "KeyBoard", "phimdung", "");
                CurrentClient.phimvac = WinAPI.Docfile(text, "KeyBoard", "phimvac", "");
                CurrentClient.phimvacdt = WinAPI.Docfile(text, "KeyBoard", "phimvacdt", "");
                CurrentClient.phimvaccdt = WinAPI.Docfile(text, "KeyBoard", "phimvaccdt", "");
                CurrentClient.phimdt1o = WinAPI.Docfile(text, "KeyBoard", "phimdt1o", "");
                CurrentClient.phimthaydo1 = WinAPI.Docfile(text, "KeyBoard", "phimthaydo1", "F6");
                CurrentClient.phimthaydo2 = WinAPI.Docfile(text, "KeyBoard", "phimthaydo2", "F7");
                CurrentClient.phimthaydo3 = WinAPI.Docfile(text, "KeyBoard", "phimthaydo3", "F8");

                // Lấy vật phẩm phục hồi
                CurrentClient.ItemSinhLuc = WinAPI.Docfile(text, "ItemPhucHoi", "ItemSinhLuc", "");
                CurrentClient.ItemSinhNoi = WinAPI.Docfile(text, "ItemPhucHoi", "ItemSinhNoi", "");
                CurrentClient.ItemNoiLuc = WinAPI.Docfile(text, "ItemPhucHoi", "ItemNoiLuc", "");
                CurrentClient.ItemCuuChuyen = WinAPI.Docfile(text, "ItemPhucHoi", "ItemCuuChuyen", "");
                CurrentClient.ItemLakCtr = WinAPI.Docfile(text, "ItemPhucHoi", "ItemLakCtr", "");
                CurrentClient.ItemTheLuc = WinAPI.Docfile(text, "ItemPhucHoi", "ItemTheLuc", "");




                //Lấy danh sách buff
                List<string> List1 = new List<string>(7);
                for (int i = 0; i < 7; i++)
                {
                    List1.Add(WinAPI.Docfile(text, "ListBuff", "PlayerBuff"+i, ""));
                }
                CurrentClient.Listbuff = List1;

                // Lấy danh sách buff đồ 1
                List<string> List2 = new List<string>(21);
                for (int i = 0; i < 25; i++)
                {
                    List2.Add(WinAPI.Docfile(text, "Listthaydo1", "Listthaydo1.Item" + i, "Null"));
                }
                CurrentClient.Listthaydo1 = List2;

                // Lấy danh sách buff đồ 2
                List<string> List3 = new List<string>(21);
                for (int i = 0; i < 25; i++)
                {
                    List3.Add(WinAPI.Docfile(text, "Listthaydo2", "Listthaydo2.Item" + i, "Null"));
                }
                CurrentClient.Listthaydo2 = List3;

                // Lấy danh sách buff đồ 3
                List<string> List4 = new List<string>(21);
                for (int i = 0; i < 25; i++)
                {
                    List4.Add(WinAPI.Docfile(text, "Listthaydo3", "Listthaydo3.Item" + i, "Null"));
                }
                CurrentClient.Listthaydo3 = List4;

                // load giá trị vào Project
                CBchayAutoCombo.Checked = CurrentClient.ComboPKTLQ;
                CkbkngatVAC.Checked = CurrentClient.cksutkngatvac;
                CkbxdameVAC.Checked = CurrentClient.ckxdamevac;
                CkbngatVAC.Checked = CurrentClient.cksutngatvac;
                CkbVACDT.Checked = CurrentClient.ckcombodt;
                Ckbvaccdt.Checked = CurrentClient.ckvaccdt;
                CkbĐT1o.Checked = CurrentClient.ckdt1o;

                cbbatdau.Checked = CurrentClient.Featurenmk;
                cbbuffdanhsach.Checked = CurrentClient.buffdanhsach;

                ckbthaydo1.Checked=CurrentClient.ckthaydobo1;
                ckbthaydo2.Checked = CurrentClient.ckthaydobo2;
                ckbthaydo3.Checked = CurrentClient.ckthaydobo3;

                ckbhoiphuc.Checked = CurrentClient.FeatureHoiPhuc;
                CkbBommau.Checked = CurrentClient.ckbommau;
                Ckbansinhnoi.Checked = CurrentClient.ckbomsinhnoi;
                CkbBommana.Checked = CurrentClient.ckbommana;
                CkbAnmauCC.Checked = CurrentClient.ckancc;
                CkbAnLakCtr.Checked = CurrentClient.ckanlakctr;
                Ckbanbanhngo.Checked = CurrentClient.ckantheluc;
                Ckbtheosau.Checked = CurrentClient.Bufftheosau;

                CkbClickNPC.Checked = CurrentClient.ckClickNPC;
                Ckbdoithoai1.Checked = CurrentClient.ckClickMenu1;
                Ckbdoithoai2.Checked = CurrentClient.ckClickMenu2;
                Ckbdoithoai3.Checked = CurrentClient.ckClickMenu3;
                Ckbdoithoai4.Checked = CurrentClient.ckClickMenu4;

                // Lấy giá trị vào Project
                txbtimeVAC.Text = CurrentClient.timevac.ToString();
                txbtimeDT.Text = CurrentClient.timedt.ToString();
                txbtimecdt.Text = CurrentClient.timecdt.ToString();
                txbtimerunVAC.Text = CurrentClient.timerun.ToString();

                txbHPuse.Text = CurrentClient.HPuse.ToString();
                txbCCuse.Text = CurrentClient.CCuse.ToString();
                txbDelay.Text = CurrentClient.Delay.ToString();
                txbSinhnoiUse.Text = CurrentClient.SinhnoiUse.ToString();
                txbManaUse.Text = CurrentClient.ManaUse.ToString();
                txbLakCTUse.Text = CurrentClient.LakCTUse.ToString();
                txbThelucUse.Text = CurrentClient.ThelucUse.ToString();
                CobMau.Text = CurrentClient.ItemSinhLuc.ToString();
                CobSinhNoi.Text = CurrentClient.ItemSinhNoi.ToString();
                CobMana.Text = CurrentClient.ItemNoiLuc.ToString();
                CobCuuChuyen.Text = CurrentClient.ItemCuuChuyen.ToString();
                CobLakCtr.Text = CurrentClient.ItemLakCtr.ToString();
                CobTheLuc.Text = CurrentClient.ItemTheLuc.ToString();

                CobListNPC.Text = CurrentClient.ValueClickNPC.ToString();
                CobListMenu1.Text = CurrentClient.ValueClickMenu1.ToString();
                CobListMenu2.Text = CurrentClient.ValueClickMenu2.ToString();
                CobListMenu3.Text = CurrentClient.ValueClickMenu3.ToString();
                CobListMenu4.Text = CurrentClient.ValueClickMenu4.ToString();
                TxbGiancach.Text = CurrentClient.ValueGiancach.ToString();


                // Keyboard
                cbphimdung.Text = CurrentClient.phimdung.ToString();
                cbphimXVAC.Text = CurrentClient.phimvac.ToString();
                cbphimVACDT.Text = CurrentClient.phimvacdt.ToString();
                cbvaccdt.Text = CurrentClient.phimvaccdt.ToString();
                cbphimDT1o.Text = CurrentClient.phimdt1o.ToString();

                cbphimbo1.Text=CurrentClient.phimthaydo1;
                cbphimbo2.Text = CurrentClient.phimthaydo2;
                cbphimbo3.Text = CurrentClient.phimthaydo3;

                //Lấy danh sách buff
                lstbuff.Items.Clear();

                foreach(string item in CurrentClient.Listbuff)
                {
                    lstbuff.Items.Add(item);
                }

                //Lấy danh sách thay đồ 1
                Cbdanhsach1.Items.Clear();

                foreach (string item in CurrentClient.Listthaydo1)
                {
                    Cbdanhsach1.Items.Add(item);
                }

                //Lấy danh sách thay đồ 2
                Cbdanhsach2.Items.Clear();

                foreach (string item in CurrentClient.Listthaydo2)
                {
                    Cbdanhsach2.Items.Add(item);
                }

                //Lấy danh sách thay đồ 1
                Cbdanhsach3.Items.Clear();
                foreach (string item in CurrentClient.Listthaydo3)
                {
                    Cbdanhsach3.Items.Add(item);
                }
            }));

        }

        // Thay đổi chiều rộng Header cột Listview
        private void lsvplayer_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.GreenYellow, e.Bounds);
            e.DrawText();
        }

        // Nhả chuột trên Listview
        private void lsvplayer_MouseUp(object sender, MouseEventArgs e)
        {
            lsvplayer.Invoke(new MethodInvoker(() =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right && CurrentClient != null)
                    WinAPI.ShowWindow(CurrentClient.WindowHwnd.ToInt32(), 0); //param2 0 = hide
            }));
        }

        // Nhả chuột trên Listview
        private void lsvplayer_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (e.ColumnIndex == 0 && lsvplayer.Columns[e.ColumnIndex].Width != 23)
            {
                lsvplayer.Columns[e.ColumnIndex].Width = 23;
            }
        }

        // Thay đổi chiều rộng cột trên listview
        private void lsvplayer_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.Cancel = true;
            }
        }

        //Sự kiện kích đúp chuột lên listview
        private void lsvplayer_DoubleClick(object sender, EventArgs e)
        {
            lsvplayer.Invoke(new MethodInvoker(() =>
            {
                WinAPI.ShowWindow(CurrentClient.WindowHwnd.ToInt32(), 1);//param2 1 = show
            }));
        }

        // Chỉ cho nhập phím dạng số vào textbox
        private void txbtimeLHQ_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("0123456789".IndexOf(e.KeyChar) != -1)
            {
                e.Handled = false;
            }
            else
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        // Sự kiện thoát khỏi textbox
        private void txbtimeLHQ_Validated(object sender, EventArgs e)
        {
            if (txbtimecdt.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbtimecdt.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {

                    CurrentClient.timecdt = Convert.ToInt32(txbtimecdt.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                    WinAPI.Ghifile(text, "ComboTLQ", "timecdt", CurrentClient.timecdt.ToString());
                }
            }
        }

        // Sự kiện lựa chọn combobox 
        private void CBchayAutoCombo_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ComboPKTLQ = CBchayAutoCombo.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckComboTLQ", "CheckComboTLQ.Check1", CurrentClient.ComboPKTLQ.ToString());

                if (CBchayAutoCombo.Checked == true)
                {
                    // Giá trị khởi tạo
                    CkbxdameVAC.Enabled = false;
                    CkbkngatVAC.Enabled = false;
                    CkbngatVAC.Enabled = false;
                    CkbVACDT.Enabled = false;
                    CkbĐT1o.Enabled = false;
                    Ckbvaccdt.Enabled = false;

                    txbtimecdt.Enabled = false;
                    txbtimeVAC.Enabled = false;
                    txbtimeDT.Enabled = false;
                    txbtimerunVAC.Enabled = false;

                    cbphimdung.Enabled = false;
                    cbphimXVAC.Enabled = false;
                    cbphimVACDT.Enabled = false;
                    cbvaccdt.Enabled = false;
                    cbphimDT1o.Enabled = false;
                }
                else
                {
                    CkbxdameVAC.Enabled = true;
                    CkbkngatVAC.Enabled = true;
                    CkbngatVAC.Enabled = true;
                    CkbVACDT.Enabled = true;
                    CkbĐT1o.Enabled = true;
                    Ckbvaccdt.Enabled = true;

                    txbtimecdt.Enabled = true;
                    txbtimeVAC.Enabled = true;
                    txbtimeDT.Enabled = true;
                    txbtimerunVAC.Enabled = true;
                    CurrentClient.ComboPKTLQ = false;

                    cbphimdung.Enabled = true;
                    cbphimXVAC.Enabled = true;
                    cbphimVACDT.Enabled = true;
                    cbvaccdt.Enabled = true;
                    cbphimDT1o.Enabled = true;

                }

            }
        }

        // Sự kiện lựa chọn combobox 
        private void CkbkngatVAC_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.cksutkngatvac = CkbkngatVAC.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckComboTLQ", "CheckComboTLQ.Check2", CurrentClient.cksutkngatvac.ToString());
            }
        }

        // Sự kiện lựa chọn combobox 
        private void CkbxdameVAC_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckxdamevac = CkbxdameVAC.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckComboTLQ", "CheckComboTLQ.Check3", CurrentClient.ckxdamevac.ToString());
            }
        }

        // Sự kiện lựa chọn combobox 
        private void CkbngatVAC_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.cksutngatvac = CkbngatVAC.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckComboTLQ", "CheckComboTLQ.Check4", CurrentClient.cksutngatvac.ToString());
            }
        }

        // Sự kiện lựa chọn combobox 
        private void CkbVACDT_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckcombodt = CkbVACDT.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckComboTLQ", "CheckComboTLQ.Check5", CurrentClient.ckcombodt.ToString());
            }
        }

        // Sự kiện lựa chọn combobox 
        private void Ckbvaccdt_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckvaccdt = Ckbvaccdt.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckComboTLQ", "CheckComboTLQ.Check6", CurrentClient.ckvaccdt.ToString());
            }
        }

        // Sự kiện lựa chọn combobox 
        private void CkbĐT1o_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckdt1o = CkbĐT1o.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckComboTLQ", "CheckComboTLQ.Check7", CurrentClient.ckdt1o.ToString());
            }
        }

        // Thủ tục đưa thông tin nhân vật lên listview
        private void CheckChars()
        {
            while (true)
            {
                Thread.Sleep(1000);

                _clientHwnds.Clear();
                WinAPI.EnumWindows(SearchForGameWindows, new IntPtr(0));

                lsvplayer.Invoke(new MethodInvoker(() =>
                {
                    var keys = _clients.Keys.ToList();

                    //////////////////////////////ListView//////////////////////
                    for (int i = 0; i < keys.Count; i++)
                    {
                        var clientKey = keys[i];

                        if (_clientHwnds.Contains(clientKey))
                        {
                            _clientHwnds.Remove(clientKey);
                        }

                        else
                        {
                            _clients.Remove(clientKey);
                            var add = clientKey.ToInt32().ToString();
                            foreach (ListViewItem item in lsvplayer.Items)
                            {
                                if (item.ToString().Substring(0, add.Length) == add)
                                {
                                    lsvplayer.Items.Remove(item);
                                }
                            }
                        }
                    }

                    foreach (var clientHwnd in _clientHwnds)
                    {
                        var client = new AutoClientBS();
                        client.Attach(clientHwnd);

                        _clients.Add(clientHwnd, client);

                        ListViewItem row = new ListViewItem(clientHwnd.ToInt32().ToString(), 0);
                        row.SubItems.Add(client.CurrentPlayer.EntityNameUnicode);
                        lsvplayer.Items.Add(row);
                        row.SubItems.Add(client.CurrentPlayer.TrangThaiOnline.ToString());
                    }
                }));
                
            }
        }

        // Thủ tục Update thông tin nhân vật trên listview
        private void UpdateCharStat()
        {
            while (true)
            {
                Thread.Sleep(1000);

                lsvplayer.Invoke(new MethodInvoker(() =>
                {
                    ///////////////////ListView////////////////
                    var _rowNeedToBeRemoved = new List<ListViewItem>();
                    foreach (ListViewItem row in lsvplayer.Items)
                    {
                        var key = new IntPtr(Convert.ToInt32(row.Text));  ///row.Cells[0].Value));
                        if (_clients.ContainsKey(key))
                        {
                            var client = _clients[key];
                            row.SubItems[1].Text = client.CurrentPlayer.EntityNameUnicode;
                            row.SubItems[2].Text = client.CurrentPlayer.TrangThaiOnline.ToString();
                        }
                        else
                        {
                            _rowNeedToBeRemoved.Add(row);
                        }

                    }
                    if (_rowNeedToBeRemoved.Count > 0)
                    {
                        foreach (ListViewItem item in _rowNeedToBeRemoved)
                        {
                            lsvplayer.Items.Remove(item);
                        }
                    }
                }));
                
            }
        }

        //Sự kiện chọn phím combo TLQ
        private void cbphimdung_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.phimdung = cbphimdung.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "KeyBoard", "phimdung", CurrentClient.phimdung.ToString());
            }
        }

        //Sự kiện chọn phím combo TLQ
        private void cbphimXVAC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.phimvac = cbphimXVAC.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "KeyBoard", "phimvac", CurrentClient.phimvac.ToString());
            }
        }

        //Sự kiện chọn phím combo TLQ
        private void cbphimVACDT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.phimvacdt = cbphimVACDT.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "KeyBoard", "phimvacdt", CurrentClient.phimvacdt.ToString());
            }
        }

        //Sự kiện chọn phím combo TLQ
        private void cbphimDT1o_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.phimdt1o = cbphimDT1o.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "KeyBoard", "phimdt1o", CurrentClient.phimdt1o.ToString());
            }
        }

        //Sự kiện chọn phím combo TLQ
        private void cbvaccdt_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.phimvaccdt = cbvaccdt.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "KeyBoard", "phimvaccdt", CurrentClient.phimvaccdt.ToString());
            }
        }

        //Sự kiện cập nhật danh sách Player vào combobox cbbuff
        private void btncapnhat_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.RefreshEntityList();

                cbbuff.DataSource = (from entity in CurrentClient.EntityList where entity.EntityType == NPCType.Player && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 500 orderby entity.EntityNameNoMark select entity.EntityNameNoMark).ToList();
                cbbuff.SelectedItem = CurrentClient.Featurebuff;
            }
        }

        //Sự kiện xóa nhân vật khỏi danh sách buff
        private void btnxoa_Click(object sender, EventArgs e)
        {
            if (lstbuff.SelectedItems.Count != 0)
            {
                if (lstbuff.SelectedIndex > -1)
                {
                    for (int i = 0; i < lstbuff.Items.Count; i++)
                    {
                        if(lstbuff.SelectedIndex == i)
                        {
                            string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                            string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                            CurrentClient.Listbuff.RemoveAt(lstbuff.SelectedIndex);
                            lstbuff.Items.RemoveAt(lstbuff.SelectedIndex);
                            WinAPI.Ghifile(text, "ListBuff", "PlayerBuff" + i, "");
                        }

                    }

                }
            }
        }

        //Sự kiện thêm nhân vật khỏi danh sách buff
        private void btnthem_Click(object sender, EventArgs e)
        {
            cbbuff.Invoke(new MethodInvoker(() =>
            { 
            if (cbbuff.Text.ToString() == ""|| cbbuff.Text.ToString() == null)
            {
                MessageBox.Show("Bạn phải ấn cập nhật trước!!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                for(int i = 0; i < lstbuff.Items.Count;i++)
                {
                    if ((lstbuff.Items[i].ToString() == "" || lstbuff.Items[i].ToString() == null) && !lstbuff.Items.Contains(cbbuff.Text.ToString()))
                    lstbuff.Items[i] = cbbuff.Text.ToString();
                }

                List<string> List1 = new List<string>(7);

                foreach (string item in lstbuff.Items)
                {
                    List1.Add(item);
                }

                CurrentClient.Listbuff = List1;

                for (int k=0; k < CurrentClient.Listbuff.Count; k++)
                {
                        string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                        string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                        WinAPI.Ghifile(text, "Listbuff", "PlayerBuff"+k, CurrentClient.Listbuff[k].ToString());
                }
            }
            }));
        }

        //Sự kiện check vào lựa chọn bắt đầu autobuff
        private void cbbatdau_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.Featurenmk = cbbatdau.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckBuff", "CheckBuff.Check1", CurrentClient.Featurenmk.ToString());
            }
        }

        //Sự kiện Selected trên listbox
        private void lstbuff_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbbuff.Invoke(new MethodInvoker(() =>
            {
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";

                for(int i =0; i < 6; i++)
                {
                    WinAPI.Ghifile(text, "Listbuff", "PlayerBuff" + i, CurrentClient.Listbuff[i].ToString());
                    if (CurrentClient.Listbuff[i].ToString() == "")
                        break;
                }
            }));
        }

        //Sự kiện check vào lựa chọn bắt đầu buff theo danh sách
        private void cbbuffdanhsach_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.buffdanhsach = cbbuffdanhsach.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckBuff", "CheckBuff.Check3", CurrentClient.buffdanhsach.ToString());
            }
        }

        // Thủ tục buff cho nhân vật Game
        public void BuffPlayer(string NpcName)
        {
            if (ClientbuffNMK != null)
            {
                ClientbuffNMK.RefreshEntityList();

                PlayerEntity result = null;
                int minDistance = int.MaxValue;

                var posX = ClientbuffNMK.CurrentPlayer.PositionX;
                var posY = ClientbuffNMK.CurrentPlayer.PositionY;

                foreach (var entity in ClientbuffNMK.EntityList)
                {
                    if (entity.EntityType != NPCType.Player && entity.PlayerStatus != PlayerStatus.DoNothing)
                        continue;
                    if (entity.EntityNameNoMark.Contains(NpcName))
                    {
                        int d = Convert.ToInt32(GameGeneral.Distance(posX, posY, entity.PositionX, entity.PositionY));
                        if (d < minDistance)
                        {
                            minDistance = d;
                            result = entity;
                            break;
                        }
                    }
                }
                if (result != null)
                {
                    ClientbuffNMK.AttackVictim(GameConst.SenIDKill, result.EntityId);
                }
            }
        }

        // Thủ tục tự động buff NMK
        private void Autobuff()
        {
            while (true)
            {
                Thread.Sleep(1000);

                if (ClientbuffNMK != null && ClientbuffNMK.Featurenmk == true && ClientbuffNMK.ischecked == true && Autobuffnmk == true &&ClientbuffNMK.CurrentPlayer.Hephai == 8)
                {
                        ClientbuffNMK.Inject();

                        // Lựa chọn giá trị của StepBuff
                        switch (StepbufNMK)
                        {
                            case 0:
                                {
                                    ClientbuffNMK.SelfBuffSkill(GameConst.SenIDKill);

                                    if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {
                                        int stepbufNMK = StepbufNMK;
                                        StepbufNMK = 1;
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    ClientbuffNMK.SelfBuffSkill(GameConst.LTIDKill);

                                    if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {
                                        int stepbufNMK = StepbufNMK;
                                        StepbufNMK = 2;
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    ClientbuffNMK.SelfBuffSkill(GameConst.BDIDKill);

                                    if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {
                                        int stepbufNMK = StepbufNMK;
                                        StepbufNMK = 3;
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    ClientbuffNMK.SelfBuffSkill(GameConst.PhoteIDKill);
                                    if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {
                                        int stepbufNMK = StepbufNMK;
                                        StepbufNMK = 4;
                                    }
                                    break;
                                }
                            case 4:
                                {
                                    ClientbuffNMK.SelfBuffSkill(GameConst.NgoaiIDKill);

                                    if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {
                                        int stepbufNMK = StepbufNMK;
                                        StepbufNMK = 5;
                                    }
                                    break;
                                }
                            case 5:
                                {
                                    ClientbuffNMK.SelfBuffSkill(GameConst.NoiIDKill);
                                    if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {
                                        int stepbufNMK = StepbufNMK;
                                        StepbufNMK = 6;
                                    }
                                    break;
                                }
                            case 6:
                                {
                                    ClientbuffNMK.SelfBuffSkill(GameConst.ManaIDKill);

                                    // Bắt đầu lựa chọn sen theo danh sách hoặc sen cho tất cả người chơi
                                    if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {
                                        if (ClientbuffNMK !=null && ClientbuffNMK.Featurenmk == true && Autobuffnmk == true && ClientbuffNMK.ischecked == true && ClientbuffNMK.buffdanhsach == false && ClientbuffNMK.CurrentPlayer.Hephai == 8)
                                        {
                                                ClientbuffNMK.RefreshEntityList();

                                                _Danhsachbuff = (from entity in ClientbuffNMK.EntityList where entity.EntityType == NPCType.Player && Convert.ToInt32(GameGeneral.Distance(ClientbuffNMK.CurrentPlayer.PositionX, ClientbuffNMK.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 400 orderby entity.EntityNameNoMark select entity.EntityNameNoMark).ToList();

                                                int i = _Danhsachbuff.Count;

                                                if (i == 1)
                                                {
                                                    int stepbufNMK = StepbufNMK;
                                                    StepbufNMK = 7;

                                                }
                                                if (i == 2)
                                                {
                                                    int stepbufNMK = StepbufNMK;
                                                    StepbufNMK = 8;
                                                }
                                                if (i == 3)
                                                {
                                                    int stepbufNMK = StepbufNMK;
                                                    StepbufNMK = 10;
                                                }
                                                if (i == 4)
                                                {
                                                    int stepbufNMK = StepbufNMK;
                                                    StepbufNMK = 13;
                                                }
                                                if (i == 5)
                                                {
                                                    int stepbufNMK = StepbufNMK;
                                                    StepbufNMK = 17;
                                                }
                                                if (i == 7)
                                                {
                                                    int stepbufNMK = StepbufNMK;
                                                    StepbufNMK = 22;
                                                }
                                                if (i >= 8)
                                                {
                                                    int stepbufNMK = StepbufNMK;
                                                    StepbufNMK = 28;
                                                }
                                        }
                                        else if (ClientbuffNMK != null && ClientbuffNMK.Featurenmk == true && Autobuffnmk == true && ClientbuffNMK.ischecked == true && ClientbuffNMK.buffdanhsach == true && ClientbuffNMK.CurrentPlayer.Hephai == 8)
                                        {

                                            foreach(string item in CurrentClient.Listbuff)
                                            {
                                            if ((item.ToString() != "" || item.ToString() != null)&&!_Danhsachbuff.Contains(item))
                                                _Danhsachbuff.Add(item);
                                            }

                                            int i = _Danhsachbuff.Count;

                                            if (i == 0)
                                            {
                                                int stepbufNMK = StepbufNMK;
                                                StepbufNMK = 0;

                                            }

                                            if (i == 1)
                                            {
                                                int stepbufNMK = StepbufNMK;
                                                StepbufNMK = 7;

                                            }
                                            if (i == 2)
                                            {
                                                int stepbufNMK = StepbufNMK;
                                                StepbufNMK = 8;
                                            }
                                            if (i == 3)
                                            {
                                                int stepbufNMK = StepbufNMK;
                                                StepbufNMK = 10;
                                            }
                                            if (i == 4)
                                            {
                                                int stepbufNMK = StepbufNMK;
                                                StepbufNMK = 13;
                                            }
                                            if (i == 5)
                                            {
                                                int stepbufNMK = StepbufNMK;
                                                StepbufNMK = 17;
                                            }
                                            if (i == 7)
                                            {
                                                int stepbufNMK = StepbufNMK;
                                                StepbufNMK = 22;
                                            }
                                            if (i >= 8)
                                            {
                                                int stepbufNMK = StepbufNMK;
                                                StepbufNMK = 28;
                                            }

                                        }
                                    }
                                    break;
                                }

                            case 7:
                                BuffPlayer(_Danhsachbuff[0].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 0;

                                }
                                break;

                            case 8:


                                BuffPlayer(_Danhsachbuff[0].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 9;

                                }
                                break;

                            case 9:


                                BuffPlayer(_Danhsachbuff[1].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 0;

                                }
                                break;


                            case 10:


                                BuffPlayer(_Danhsachbuff[0].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 11;

                                }
                                break;

                            case 11:


                                BuffPlayer(_Danhsachbuff[1].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 12;

                                }
                                break;
                            case 12:


                                BuffPlayer(_Danhsachbuff[2].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 0;

                                }
                                break;
                            case 13:


                                BuffPlayer(_Danhsachbuff[0].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 14;

                                }
                                break;
                            case 14:


                                BuffPlayer(_Danhsachbuff[1].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 15;

                                }
                                break;

                            case 15:


                                BuffPlayer(_Danhsachbuff[2].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 16;

                                }
                                break;

                            case 16:


                                BuffPlayer(_Danhsachbuff[3].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 0;

                                }
                                break;
                            case 17:


                                BuffPlayer(_Danhsachbuff[0].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 18;

                                }
                                break;
                            case 18:


                                BuffPlayer(_Danhsachbuff[1].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 19;

                                }
                                break;
                            case 19:


                                BuffPlayer(_Danhsachbuff[2].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 20;

                                }
                                break;
                            case 20:


                                BuffPlayer(_Danhsachbuff[3].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 21;

                                }
                                break;
                            case 21:


                                BuffPlayer(_Danhsachbuff[4].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 0;

                                }
                                break;
                            case 22:


                                BuffPlayer(_Danhsachbuff[0].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 23;

                                }
                                break;
                            case 23:


                                BuffPlayer(_Danhsachbuff[1].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 24;

                                }
                                break;
                            case 24:


                                BuffPlayer(_Danhsachbuff[2].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 25;

                                }
                                break;
                            case 25:


                                BuffPlayer(_Danhsachbuff[3].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 26;

                                }
                                break;
                            case 26:


                                BuffPlayer(_Danhsachbuff[4].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 27;

                                }
                                break;
                            case 27:


                                BuffPlayer(_Danhsachbuff[5].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 0;

                                }
                                break;
                            case 28:


                                BuffPlayer(_Danhsachbuff[0].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 29;

                                }
                                break;
                            case 29:


                                BuffPlayer(_Danhsachbuff[1].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 30;

                                }
                                break;
                            case 30:


                                BuffPlayer(_Danhsachbuff[2].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 31;

                                }
                                break;
                            case 31:


                                BuffPlayer(_Danhsachbuff[3].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 32;

                                }
                                break;
                            case 32:


                                BuffPlayer(_Danhsachbuff[4].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 33;

                                }
                                break;
                            case 33:


                                BuffPlayer(_Danhsachbuff[5].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 34;

                                }
                                break;
                            case 34:


                                BuffPlayer(_Danhsachbuff[6].ToString());

                                if (ClientbuffNMK.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                {
                                    int stepbufNMK = StepbufNMK;
                                    StepbufNMK = 0;

                                }
                                break;
                        }
                    }
                }
                  
            
        }

        // Thủ tục chuẩn bị đóng Form
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Đóng Hook Game
            foreach (var autoClient in _clients)
            {
                autoClient.Value.DeInject();
            }

            Stop(); // dừng Hook Keyboard


        }

        // Cập nhật Item 01
        private void BtnUpdate1_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.RefreshStorageItems();

                Cbdanhsach1.DataSource = (from entity in CurrentClient.ItemList where entity.Location == 1 orderby entity.ItemId select entity.ItemId.ToString()+"-"+entity.ItemNameNoMark.ToString()).ToList();
                CurrentClient.Listthaydo1 = (from entity in CurrentClient.ItemList where entity.Location == 1 orderby entity.ItemId select entity.ItemId.ToString() + "-" + entity.ItemNameNoMark.ToString()).ToList();

                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";

                for(int i = 0; i < CurrentClient.Listthaydo1.Count; i++)
                {
                    WinAPI.Ghifile(text, "Listthaydo1", "Listthaydo1.Item" + i, CurrentClient.Listthaydo1[i].ToString());
                }
            }
        }

        // Cập nhật Item 02
        private void BtnUpdate2_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.RefreshStorageItems();

                Cbdanhsach2.DataSource = (from entity in CurrentClient.ItemList where entity.Location == 1 orderby entity.ItemId select entity.ItemId.ToString() + "-" + entity.ItemNameNoMark.ToString()).ToList();
                CurrentClient.Listthaydo2 = (from entity in CurrentClient.ItemList where entity.Location == 1 orderby entity.ItemId select entity.ItemId.ToString() + "-" + entity.ItemNameNoMark.ToString()).ToList();

                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";

                for (int i = 0; i < CurrentClient.Listthaydo2.Count; i++)
                {
                    WinAPI.Ghifile(text, "Listthaydo2", "Listthaydo2.Item" + i, CurrentClient.Listthaydo2[i].ToString());
                }

            }
        }

        // Cập nhật Item 03
        private void BtnUpdate3_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.RefreshStorageItems();

                Cbdanhsach3.DataSource = (from entity in CurrentClient.ItemList where entity.Location == 1 orderby entity.Location == 1 orderby entity.ItemId select entity.ItemId.ToString() + "-" + entity.ItemNameNoMark.ToString()).ToList();
                CurrentClient.Listthaydo3 = (from entity in CurrentClient.ItemList where entity.Location == 1 orderby entity.Location == 1 orderby entity.ItemId select entity.ItemId.ToString() + "-" + entity.ItemNameNoMark.ToString()).ToList();

                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";

                for (int i = 0; i < CurrentClient.Listthaydo3.Count; i++)
                {
                    WinAPI.Ghifile(text, "Listthaydo3", "Listthaydo3.Item" + i, CurrentClient.Listthaydo3[i].ToString());
                }
            }
        }

        // Checkbox thay do bo 1
        private void ckbthaydo1_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckthaydobo1 = ckbthaydo1.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckThaydo", "CheckThaydo.Check1", CurrentClient.ckthaydobo1.ToString());
            }
        }

        // Checkbox thay do bo 2
        private void ckbthaydo2_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckthaydobo2 = ckbthaydo2.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckThaydo", "CheckThaydo.Check2", CurrentClient.ckthaydobo2.ToString());
            }

        }

        // Checkbox thay do bo 3
        private void ckbthaydo3_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckthaydobo3 = ckbthaydo3.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckThaydo", "CheckThaydo.Check3", CurrentClient.ckthaydobo3.ToString());
            }
        }

        // Phím tắt chạy auto thay đồ 1
        private void cbphimbo1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.phimthaydo1 = cbphimbo1.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "KeyBoard", "phimthaydo1", CurrentClient.phimthaydo1.ToString());
            }
        }

        // Phím tắt chạy auto thay đồ 2
        private void cbphimbo2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.phimthaydo2 = cbphimbo2.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "KeyBoard", "phimthaydo2", CurrentClient.phimthaydo2.ToString());
            }
        }

        // Phím tắt chạy auto thay đồ 3
        private void cbphimbo3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.phimthaydo3 = cbphimbo3.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "KeyBoard", "phimthaydo3", CurrentClient.phimthaydo3.ToString());
            }
        }
        // Không cho nhập ký tự đặc biệt vào TextBox
        private void txbSinhnoiUse_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("0123456789".IndexOf(e.KeyChar) != -1)
            {
                e.Handled = false;
            }
            else
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        // Không cho nhập ký tự đặc biệt vào TextBox
        private void txbManaUse_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("0123456789".IndexOf(e.KeyChar) != -1)
            {
                e.Handled = false;
            }
            else
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        // Không cho nhập ký tự đặc biệt vào TextBox
        private void txbLakCTUse_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("0123456789".IndexOf(e.KeyChar) != -1)
            {
                e.Handled = false;
            }
            else
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        // Không cho nhập ký tự đặc biệt vào TextBox
        private void txbThelucUse_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("0123456789".IndexOf(e.KeyChar) != -1)
            {
                e.Handled = false;
            }
            else
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        // Thủ tục thoát khỏi Textbox
        private void txbSinhnoiUse_Validated(object sender, EventArgs e)
        {
            if (txbSinhnoiUse.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbSinhnoiUse.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.SinhnoiUse = Convert.ToInt32(txbSinhnoiUse.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                    WinAPI.Ghifile(text, "PhucHoi", "SinhnoiUse", CurrentClient.SinhnoiUse.ToString());
                }
            }
        }

        // Thủ tục thoát khỏi Textbox
        private void txbManaUse_Validated(object sender, EventArgs e)
        {
            if (txbManaUse.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbManaUse.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.ManaUse = Convert.ToInt32(txbManaUse.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                    WinAPI.Ghifile(text, "PhucHoi", "ManaUse", CurrentClient.ManaUse.ToString());
                }
            }
        }

        // Thủ tục thoát khỏi Textbox
        private void txbLakCTUse_Validated(object sender, EventArgs e)
        {
            if (txbLakCTUse.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbManaUse.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.LakCTUse = Convert.ToInt32(txbLakCTUse.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                    WinAPI.Ghifile(text, "PhucHoi", "LakCTUse", CurrentClient.LakCTUse.ToString());
                }
            }
        }

        // Thủ tục thoát khỏi Textbox
        private void txbThelucUse_Validated(object sender, EventArgs e)
        {
            if (txbThelucUse.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbThelucUse.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.ThelucUse = Convert.ToInt32(txbThelucUse.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                    WinAPI.Ghifile(text, "PhucHoi", "ThelucUse", CurrentClient.ThelucUse.ToString());
                }
            }
        }

        // Thủ tục thoát khỏi Textbox
        private void txbCCuse_Validated(object sender, EventArgs e)
        {
            if (txbCCuse.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbManaUse.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.CCuse = Convert.ToInt32(txbCCuse.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                    WinAPI.Ghifile(text, "PhucHoi", "CCuse", CurrentClient.CCuse.ToString());
                }
            }
        }

        // Không cho nhập ký tự đặc biệt vào TextBox
        private void txbCCuse_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("0123456789".IndexOf(e.KeyChar) != -1)
            {
                e.Handled = false;
            }
            else
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        // Thủ tục click Checkbox bơm máu
        private void ckbhoiphuc_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.FeatureHoiPhuc = ckbhoiphuc.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";

                WinAPI.Ghifile(text, "CheckPhucHoi", "CheckPhucHoi.Check1", CurrentClient.FeatureHoiPhuc.ToString());

                if (ckbhoiphuc.Checked == true)
                {
                    txbHPuse.Enabled = false;
                    txbCCuse.Enabled = false;
                    txbDelay.Enabled = false;
                    txbSinhnoiUse.Enabled = false;
                    txbManaUse.Enabled = false;
                    txbLakCTUse.Enabled = false;
                    txbThelucUse.Enabled = false;
                    CkbBommau.Enabled = false;
                    Ckbansinhnoi.Enabled = false;
                    CkbBommana.Enabled = false;
                    CkbAnmauCC.Enabled = false;
                    CkbAnLakCtr.Enabled = false;
                    Ckbanbanhngo.Enabled = false;
                    CobMau.Enabled = false;
                    CobSinhNoi.Enabled = false;
                    CobMana.Enabled = false;
                    CobLakCtr.Enabled = false;
                    CobCuuChuyen.Enabled = false;
                    CobTheLuc.Enabled = false;

                }
                else
                {
                    txbHPuse.Enabled = true;
                    txbCCuse.Enabled = true;
                    txbDelay.Enabled = true;
                    txbSinhnoiUse.Enabled = true;
                    txbManaUse.Enabled = true;
                    txbLakCTUse.Enabled = true;
                    txbThelucUse.Enabled = true;
                    CkbBommau.Enabled = true;
                    Ckbansinhnoi.Enabled = true;
                    CkbBommana.Enabled = true;
                    CkbAnmauCC.Enabled = true;
                    CkbAnLakCtr.Enabled = true;
                    Ckbanbanhngo.Enabled = true;
                    CobMau.Enabled = true;
                    CobSinhNoi.Enabled = true;
                    CobMana.Enabled = true;
                    CobLakCtr.Enabled = true;
                    CobCuuChuyen.Enabled = true;
                    CobTheLuc.Enabled = true;
                    //
                }
            }
        }

        // Thủ tục click Checkbox bơm máu
        private void CkbBommau_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckbommau = CkbBommau.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckPhucHoi", "CheckPhucHoi.Check2", CurrentClient.ckbommau.ToString());
            }
        }

        // Thủ tục click Checkbox bơm sinh nội
        private void Ckbansinhnoi_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckbomsinhnoi = Ckbansinhnoi.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckPhucHoi", "CheckPhucHoi.Check3", CurrentClient.ckbomsinhnoi.ToString());
            }
        }

        // Thủ tục click Checkbox bơm mana
        private void CkbBommana_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckbommana = CkbBommana.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckPhucHoi", "CheckPhucHoi.Check4", CurrentClient.ckbommana.ToString());
            }

        }

        // Thủ tục click Checkbox bơm máu cửu chuyển
        private void CkbAnmauCC_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckancc = CkbAnmauCC.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckPhucHoi", "CheckPhucHoi.Check5", CurrentClient.ckancc.ToString());
            }
        }

        // Thủ tục click Checkbox bơm lak chiến trường
        private void CkbAnLakCtr_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckanlakctr = CkbAnLakCtr.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckPhucHoi", "CheckPhucHoi.Check6", CurrentClient.ckanlakctr.ToString());
            }
        }

        // Thủ tục click Checkbox ăn bánh ngô
        private void Ckbanbanhngo_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckantheluc = Ckbanbanhngo.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckPhucHoi", "CheckPhucHoi.Check7", CurrentClient.ckantheluc.ToString());
            }
        }

        // Thủ tục kiểm tra giá trị khi thoát khỏi textbox
        private void txbHPuse_Validated(object sender, EventArgs e)
        {
            if (txbHPuse.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                txbHPuse.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.HPuse = Convert.ToInt32(txbHPuse.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                    WinAPI.Ghifile(text, "PhucHoi", "HPuse", CurrentClient.HPuse.ToString());
                }
            }
        }

        // Thủ tục Checkbox theo sau
        private void Ckbtheosau_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.Bufftheosau = Ckbtheosau.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*",".") + ".txt";
                WinAPI.Ghifile(text, "CheckBuff", "CheckBuff.Check2", CurrentClient.Bufftheosau.ToString());
            }
        }

        // Thủ tục cập nhật nhân vật
        private void btnCapnhatPlayer_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.RefreshEntityList();

                CobListPlayer.DataSource = (from entity in CurrentClient.EntityList where entity.EntityType == NPCType.Player && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 500 orderby entity.EntityNameUnicode select entity.EntityNameUnicode).ToList();
            }
        }

        // Thủ tục theo sau
        private void theosau()
        {
            while (true)
            {
                Thread.Sleep(500);

                if (ClientbuffNMK != null && PlayerTheoSau.ToString()!="" && PlayerTheoSau.ToString()!=null && ClientbuffNMK.Bufftheosau==true)
                {
                    ClientbuffNMK.Inject();

                    ClientbuffNMK.RefreshEntityList();

                    var posX = ClientbuffNMK.CurrentPlayer.PositionX;
                    var posY = ClientbuffNMK.CurrentPlayer.PositionY;

                    foreach (var entity in ClientbuffNMK.EntityList)
                    {

                        if (entity.EntityNameUnicode.Contains(PlayerTheoSau))
                        {
                            int d = Convert.ToInt32(GameGeneral.Distance(posX, posY, entity.PositionX, entity.PositionY));

                            if (d > 300)
                            {
                                Autobuffnmk = false;

                                ClientbuffNMK.ShortMove(entity.PositionX, entity.PositionY);
                                break;
                            }
                            else
                            {
                                Autobuffnmk = true;

                                break;
                            }
                        }
                    }
                }
            }
        }
        // Nhập giá trị vào biến string
        private void CobListPlayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            PlayerTheoSau = CobListPlayer.Text.ToString();
        }

        // Điều kiện thực hiện checkbox
        private void CkbkngatVAC_Click(object sender, EventArgs e)
        {
            CkbxdameVAC.Checked = false;
            CkbngatVAC.Checked = false;
        }

        // Điều kiện thực hiện checkbox
        private void CkbngatVAC_Click(object sender, EventArgs e)
        {
            CkbxdameVAC.Checked = false;
            CkbkngatVAC.Checked = false;
        }

        // Điều kiện thực hiện checkbox
        private void CkbxdameVAC_Click(object sender, EventArgs e)
        {
            CkbngatVAC.Checked = false;
            CkbkngatVAC.Checked = false;
        }

        // Thủ tục lưu vật phẩm phục hồi
        private void CobMau_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ItemSinhLuc = CobMau.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                WinAPI.Ghifile(text, "ItemPhucHoi", "ItemSinhLuc", CurrentClient.ItemSinhLuc.ToString());
            }
        }

        // Thủ tục lưu vật phẩm phục hồi
        private void CobSinhNoi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ItemSinhNoi = CobSinhNoi.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                WinAPI.Ghifile(text, "ItemPhucHoi", "ItemSinhNoi", CurrentClient.ItemSinhNoi.ToString());
            }
        }

        // Thủ tục lưu vật phẩm phục hồi
        private void CobMana_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ItemNoiLuc = CobMana.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                WinAPI.Ghifile(text, "ItemPhucHoi", "ItemNoiLuc", CurrentClient.ItemNoiLuc.ToString());
            }
        }

        // Thủ tục lưu vật phẩm phục hồi
        private void CobCuuChuyen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ItemCuuChuyen = CobCuuChuyen.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                WinAPI.Ghifile(text, "ItemPhucHoi", "ItemCuuChuyen", CurrentClient.ItemCuuChuyen.ToString());
            }
        }

        // Thủ tục lưu vật phẩm phục hồi
        private void CobLakCtr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ItemLakCtr = CobLakCtr.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                WinAPI.Ghifile(text, "ItemPhucHoi", "ItemLakCtr", CurrentClient.ItemLakCtr.ToString());
            }
        }

        // Thủ tục lưu vật phẩm phục hồi
        private void CobTheLuc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ItemTheLuc = CobTheLuc.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                WinAPI.Ghifile(text, "ItemPhucHoi", "ItemTheLuc", CurrentClient.ItemTheLuc.ToString());
            }
        }

        // Cập nhật danh sách NPC
        private void BtnResetNPC_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.RefreshEntityList();

                CobListNPC.DataSource = (from entity in CurrentClient.EntityList where entity.EntityType == NPCType.Item && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 500 orderby entity.EntityId select entity.EntityId.ToString()+"-"+entity.EntityNameNoMark.ToString()).ToList();
            }
        }

        // Cập nhật danh sách Menu1
        private void BtnResetMenu1_Click(object sender, EventArgs e)
        {
            var line = CurrentClient.GetDialogLines();
            CobListMenu1.DataSource = line;
        }

        // Cập nhật danh sách Menu2
        private void BtnResetMenu2_Click(object sender, EventArgs e)
        {
            var line = CurrentClient.GetDialogLines();
            CobListMenu2.DataSource = line;
        }

        // Cập nhật danh sách Menu3
        private void BtnResetMenu3_Click(object sender, EventArgs e)
        {
            var line = CurrentClient.GetDialogLines();
            CobListMenu3.DataSource = line;
        }

        // Cập nhật danh sách Menu4
        private void BtnResetMenu4_Click(object sender, EventArgs e)
        {
            var line = CurrentClient.GetDialogLines();
            CobListMenu4.DataSource = line;
        }

        // Thủ tục checkbox click NPC
        private void CkbClickNPC_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckClickNPC = CkbClickNPC.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.Check1", CurrentClient.ckClickNPC.ToString());

                if (((CheckBox)sender).Checked)
                {
                    Ckbdoithoai1.Enabled = false;
                    Ckbdoithoai2.Enabled = false;
                    Ckbdoithoai3.Enabled = false;
                    Ckbdoithoai4.Enabled = false;
                    TxbGiancach.Enabled = false;

                    CobListMenu1.Enabled = false;
                    CobListMenu2.Enabled = false;
                    CobListMenu3.Enabled = false;
                    CobListMenu4.Enabled = false;

                }
                else
                {
                    Ckbdoithoai1.Enabled = true;
                    Ckbdoithoai2.Enabled = true;
                    Ckbdoithoai3.Enabled = true;
                    Ckbdoithoai4.Enabled = true;
                    TxbGiancach.Enabled = true;

                    CobListMenu1.Enabled = true;
                    CobListMenu2.Enabled = true;
                    CobListMenu3.Enabled = true;
                    CobListMenu4.Enabled = true;
                }
            }
        }

        // Thủ tục click NPC
        private void ClickNPc()
        {
            while (true)
            {
                Thread.Sleep(100);
                foreach (var autoClient in _clients)
                {
                    if (!autoClient.Value.ckClickNPC ||!autoClient.Value.ischecked)
                        continue;

                    if (!autoClient.Value.isInjected)
                        autoClient.Value.Inject();

                            if (autoClient.Value.ckClickNPC == true && autoClient.Value.ischecked == true && autoClient.Value.ckClickMenu1 == true && autoClient.Value.ckClickMenu2 == false && autoClient.Value.ckClickMenu3 == false && autoClient.Value.ckClickMenu4 == false)
                            {
                                autoClient.Value.ClickNPC(autoClient.Value.ValueClickNPC.ToString());
                                Thread.Sleep(autoClient.Value.ValueGiancach);
                                autoClient.Value.Chonmenu(autoClient.Value.ValueClickMenu1.ToString());
                                Thread.Sleep(autoClient.Value.ValueGiancach);
                            }
                            else if (autoClient.Value.ckClickNPC == true && autoClient.Value.ischecked == true && autoClient.Value.ckClickMenu1 == true && autoClient.Value.ckClickMenu2 == true && autoClient.Value.ckClickMenu3 == false && autoClient.Value.ckClickMenu4 == false)
                            {
                                autoClient.Value.ClickNPC(autoClient.Value.ValueClickNPC.ToString());
                                Thread.Sleep(autoClient.Value.ValueGiancach);
                                autoClient.Value.Chonmenu(autoClient.Value.ValueClickMenu1.ToString());
                                Thread.Sleep(autoClient.Value.ValueGiancach);
                                autoClient.Value.Chonmenu(autoClient.Value.ValueClickMenu2.ToString());
                                Thread.Sleep(autoClient.Value.ValueGiancach);
                            }
                            else if (autoClient.Value.ckClickNPC == true && autoClient.Value.ischecked == true && autoClient.Value.ckClickMenu1 == true && autoClient.Value.ckClickMenu2 == true && autoClient.Value.ckClickMenu3 == true && autoClient.Value.ckClickMenu4 == false)
                            {
                                autoClient.Value.ClickNPC(autoClient.Value.ValueClickNPC.ToString());
                                Thread.Sleep(autoClient.Value.ValueGiancach);
                                autoClient.Value.Chonmenu(autoClient.Value.ValueClickMenu1.ToString());
                                Thread.Sleep(autoClient.Value.ValueGiancach);
                                autoClient.Value.Chonmenu(autoClient.Value.ValueClickMenu2.ToString());
                                Thread.Sleep(autoClient.Value.ValueGiancach);
                                autoClient.Value.Chonmenu(autoClient.Value.ValueClickMenu3.ToString());
                                Thread.Sleep(autoClient.Value.ValueGiancach);
                            }
                            else if (autoClient.Value.ckClickMenu1 == true && autoClient.Value.ckClickMenu2 == true && autoClient.Value.ckClickMenu3 == true && autoClient.Value.ckClickMenu4 == true)
                            {
                                autoClient.Value.ClickNPC(autoClient.Value.ValueClickNPC.ToString());
                                Thread.Sleep(autoClient.Value.ValueGiancach);
                                autoClient.Value.Chonmenu(autoClient.Value.ValueClickMenu1.ToString());
                                Thread.Sleep(autoClient.Value.ValueGiancach);
                                autoClient.Value.Chonmenu(autoClient.Value.ValueClickMenu2.ToString());
                                Thread.Sleep(autoClient.Value.ValueGiancach);
                                autoClient.Value.Chonmenu(autoClient.Value.ValueClickMenu3.ToString());
                                Thread.Sleep(autoClient.Value.ValueGiancach);
                                autoClient.Value.Chonmenu(autoClient.Value.ValueClickMenu4.ToString());
                                Thread.Sleep(autoClient.Value.ValueGiancach);
                            }
                }
            }
        }

        // Thủ tục lọc ký tự nhập vào textbox
        private void TxbGiancach_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("0123456789".IndexOf(e.KeyChar) != -1)
            {
                e.Handled = false;
            }
            else
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        // Thủ tục thoát khỏi textbox giãn cách
        private void TxbGiancach_Validated(object sender, EventArgs e)
        {
            if (TxbGiancach.Text.Equals("0"))
            {
                MessageBox.Show("Giá trị nhập vào phải lớn hơn 0");
                TxbGiancach.Focus();
            }
            else
            {
                if (CurrentClient != null)
                {
                    CurrentClient.ValueGiancach = Convert.ToInt32(TxbGiancach.Text.ToString());
                    string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                    string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                    WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.ValueGiancach", CurrentClient.ValueGiancach.ToString());
                }
            }
        }

        // Thủ tục kiểm tra khi kích textbox
        private void CkbClickNPC_Click(object sender, EventArgs e)
        {
            if(TxbGiancach.Text == "" || TxbGiancach.Text == null)
            {
                MessageBox.Show("Bạn phải nhập thời gian giãn cách click trước");
                TxbGiancach.Focus();
                CkbClickNPC.Checked = false;
            }
        }

        // Thủ tục kiểm tra khi kích textbox
        private void Ckbdoithoai1_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckClickMenu1 = Ckbdoithoai1.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.Check2", CurrentClient.ckClickMenu1.ToString());
            }
        }

        // Thủ tục kiểm tra khi kích textbox
        private void Ckbdoithoai2_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckClickMenu2 = Ckbdoithoai2.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.Check3", CurrentClient.ckClickMenu2.ToString());
            }
        }

        // Thủ tục kiểm tra khi kích textbox
        private void Ckbdoithoai3_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckClickMenu3 = Ckbdoithoai3.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.Check4", CurrentClient.ckClickMenu3.ToString());
            }
        }

        // Thủ tục kiểm tra khi kích textbox
        private void Ckbdoithoai4_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckClickMenu4 = Ckbdoithoai4.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.Check5", CurrentClient.ckClickMenu4.ToString());
            }
        }

        // Thủ tục lưu NPC Click
        private void CobListNPC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ValueClickNPC = CobListNPC.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.ValueClickNPC", CurrentClient.ValueClickNPC.ToString());
            }
        }

        // Thủ tục lưu Menu Click
        private void CobListMenu1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ValueClickMenu1 = CobListMenu1.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.ValueClickMenu1", CurrentClient.ValueClickMenu1.ToString());
            }
        }

        // Thủ tục lưu Menu Click
        private void CobListMenu2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ValueClickMenu2 = CobListMenu2.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.ValueClickMenu2", CurrentClient.ValueClickMenu2.ToString());
            }
        }

        // Thủ tục lưu Menu Click
        private void CobListMenu3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ValueClickMenu3 = CobListMenu3.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.ValueClickMenu3", CurrentClient.ValueClickMenu3.ToString());
            }
        }

        // Thủ tục lưu Menu Click
        private void CobListMenu4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ValueClickMenu4 = CobListMenu4.Text.ToString();
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".txt";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.ValueClickMenu4", CurrentClient.ValueClickMenu4.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
          textBox1.Text = CurrentClient.MessengeHT.ToString();
        }
    }
}
