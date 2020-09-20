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
using System.Diagnostics;
using System.Collections;


namespace AutoPK
{
    public partial class Main : Form
    {
        private Dictionary<IntPtr, AutoClientBS> _clients = new Dictionary<IntPtr, AutoClientBS>();

        private List<IntPtr> _clientHwnds = new List<IntPtr>();

        private IntPtr currentSelectedChar;

        // Thủ tục khởi tạo giá trị Form khách
        #region Tạo Form mới

        private FromAutoBuff frmbuff = new FromAutoBuff();
        private FrmFormAutoGroup frmGroup= new FrmFormAutoGroup();
        private FrmAutoTrain frmTrain = new FrmAutoTrain();
        private FrmClickNPC frmClickNPC = new FrmClickNPC();
        private FrmTNC frmTNC = new FrmTNC();
        private FrmRaoban frmRaoban = new FrmRaoban();
        private FrmTNT frmTNT = new FrmTNT();
        private FrmThaydo frmthaydo = new FrmThaydo();
        private Frmphuchoi frmphuchoi = new Frmphuchoi();
        private FrmComboTLQ frmCombo = new FrmComboTLQ();

        #endregion

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

                    foreach (var autoClient in _clients.Values)
                    {
                        if (!autoClient.isInjected)
                            autoClient.Inject();

                        /////////////////////////////////////////Auto combo TLQ /////////////////////////////////////////////
                        if (e.KeyData.ToString() == autoClient.phimdung.ToString() && autoClient.ComboPKTLQ == true)
                        {
                            log = e.KeyData.ToString();

                            return (Int32)1;
                        }
                        //
                        else if (e.KeyData.ToString() == autoClient.phimvac.ToString() && autoClient.ComboPKTLQ == true)
                        {
                            log = e.KeyData.ToString();

                            return (Int32)1;
                        }
                        //
                        else if (e.KeyData.ToString() == autoClient.phimvacdt.ToString() && autoClient.ComboPKTLQ == true)
                        {

                            log = e.KeyData.ToString();

                            return (Int32)1;
                        }
                        //
                        else if (e.KeyData.ToString() == autoClient.phimvaccdt.ToString() && autoClient.ComboPKTLQ == true)
                        {

                            log = e.KeyData.ToString();

                            return (Int32)1;
                        }
                        //
                        else if (e.KeyData.ToString() == autoClient.phimdt1o.ToString() && autoClient.ComboPKTLQ == true)
                        {
                            log = e.KeyData.ToString();

                            return (Int32)1;
                        }


                        /////////////////////////////////////////Auto Thay đồ /////////////////////////////////////////////
                        if(e.KeyData.ToString() == autoClient.phimthaydo1.ToString() && autoClient.ckthaydobo1 == true && autoClient.ckthaydo == true)
                        {
                            for (int i = 0; i< autoClient.Listthaydo1.Count;i++)
                            {
                                if(autoClient.Listthaydo1[i] != "Null" && !autoClient.Listthaydo1[i].Contains("/5/0"))
                                {
                                    autoClient.FindAndUseItemID(autoClient.Listthaydo1[i]);
                                }
                                else if (autoClient.Listthaydo1[i] != "Null" && autoClient.Listthaydo1[i].Contains("/5/0"))
                                {
                                    autoClient.Thayngoc2(autoClient.Listthaydo1[i]);
                                }
                            }

                            return (Int32)1;
                        }

                        //
                        else if (e.KeyData.ToString() == autoClient.phimthaydo2.ToString() && autoClient.ckthaydobo2 == true && autoClient.ckthaydo == true)
                        {
                            for (int i = 0; i < autoClient.Listthaydo2.Count; i++)
                            {
                                if (autoClient.Listthaydo2[i] != "Null" && !autoClient.Listthaydo2[i].Contains("/5/0"))
                                {
                                    autoClient.FindAndUseItemID(autoClient.Listthaydo2[i]);
                                }
                                else if (autoClient.Listthaydo2[i] != "Null" && autoClient.Listthaydo2[i].Contains("/5/0"))
                                {
                                    autoClient.Thayngoc2(autoClient.Listthaydo2[i]);
                                }
                            }

                            return (Int32)1;

                        }

                        //
                        else if (e.KeyData.ToString() == autoClient.phimthaydo3.ToString() && autoClient.ckthaydobo3 == true && autoClient.ckthaydo == true)
                        {
                            for (int i = 0; i < autoClient.Listthaydo3.Count; i++)
                            {
                                if (autoClient.Listthaydo3[i] != "Null" && !autoClient.Listthaydo3[i].Contains("/5/0"))
                                {
                                    autoClient.FindAndUseItemID(autoClient.Listthaydo3[i]);
                                }
                                else if (autoClient.Listthaydo3[i] != "Null" && autoClient.Listthaydo3[i].Contains("/5/0"))
                                {
                                    autoClient.Thayngoc2(autoClient.Listthaydo3[i]);
                                }
                            }

                            return (Int32)1;
                        }

                        else if (e.KeyData.ToString() == autoClient.phimthaydo4.ToString() && autoClient.ckthaydobo4 == true && autoClient.ckthaydo == true)
                        {
                            for (int i = 0; i < autoClient.Listthaydo4.Count; i++)
                            {
                                if (autoClient.Listthaydo4[i] != "Null" && !autoClient.Listthaydo4[i].Contains("/5/0"))
                                {
                                    autoClient.FindAndUseItemID(autoClient.Listthaydo4[i]);
                                }
                                else if (autoClient.Listthaydo4[i] != "Null" && autoClient.Listthaydo4[i].Contains("/5/0"))
                                {
                                    autoClient.Thayngoc2(autoClient.Listthaydo4[i]);
                                }
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

        // Main load
        #region Main load
        private void Main_Load(object sender, EventArgs e)
        {            

            // Tạo thread mới chạy song song với Main chạy hàm bơm máu
            Thread thrbommau = new Thread(new ThreadStart(bommau));
            thrbommau.Priority = ThreadPriority.Normal;
            thrbommau.IsBackground = true;
            thrbommau.Start();

            // Tạo thread mới chạy song song với Main chạy hàm lấy tọa độ nhân vậtClickNPc()
            Thread thrgettoadonv = new Thread(new ThreadStart(Gettoadonv));
            thrgettoadonv.Priority = ThreadPriority.Normal;
            thrgettoadonv.IsBackground = true;
            thrgettoadonv.Start();

            // Chạy thead auto Buff NMK
            Thread thrAutobuff = new Thread(new ThreadStart(Autobuff));
            thrAutobuff.Priority = ThreadPriority.Normal;
            thrAutobuff.IsBackground = true;
            thrAutobuff.Start();

            // Tạo một thread mới để chạy hàm pkcombo
            Thread thrpkcombo = new Thread(new ThreadStart(pkcombo));
            thrpkcombo.Priority = ThreadPriority.Normal;
            thrpkcombo.IsBackground = true;
            thrpkcombo.Start();

            // Tạo một thread mới để chạy hàm pkcombo
            Thread thrtheosau = new Thread(new ThreadStart(theosau));
            thrtheosau.Priority = ThreadPriority.Normal;
            thrtheosau.IsBackground = true;
            thrtheosau.Start();

            // Tạo một thread mới để chạy hàm click NPC
            Thread thrClickNpc = new Thread(new ThreadStart(ClickNPc));
            thrClickNpc.Priority = ThreadPriority.Normal;
            thrClickNpc.IsBackground = true;
            thrClickNpc.Start();

            // Tạo một thread mới để chạy hàm auto TNC
            Thread thrAutoTNC = new Thread(new ThreadStart(autoTNC));
            thrAutoTNC.Priority = ThreadPriority.Normal;
            thrAutoTNC.IsBackground = true;
            thrAutoTNC.Start();


            // Tạo một thread mới để chạy hàm auto Train
            Thread thrAutoTrain = new Thread(new ThreadStart(AutoTrain));
            thrAutoTrain.Priority = ThreadPriority.Normal;
            thrAutoTrain.IsBackground = true;
            thrAutoTrain.Start();

            // Tạo một thread mới để chạy hàm auto Kills Boss
            Thread thrAutoKillBoss = new Thread(new ThreadStart(AutoKillBoss));
            thrAutoKillBoss.Priority = ThreadPriority.Normal;
            thrAutoKillBoss.IsBackground = true;
            thrAutoKillBoss.Start();

            // Tạo một thread mới để chạy hàm auto Kills Boss vuot ai
            Thread thrAutoKillBossvuotai = new Thread(new ThreadStart(AutoKillBossvuotai));
            thrAutoKillBossvuotai.Priority = ThreadPriority.Normal;
            thrAutoKillBossvuotai.IsBackground = true;
            thrAutoKillBossvuotai.Start();

            // Tạo một thread mới để chạy hàm auto Thái nhất tháp
            Thread thrAutoTNT = new Thread(new ThreadStart(AutoTNT));
            thrAutoTNT.Priority = ThreadPriority.Normal;
            thrAutoTNT.IsBackground = true;
            thrAutoTNT.Start();

            // Tạo một thread mới để chạy hàm auto Rao bán
            Thread thrAutoraoban = new Thread(new ThreadStart(Autoraoban));
            thrAutoraoban.Priority = ThreadPriority.Normal;
            thrAutoraoban.IsBackground = true;
            thrAutoraoban.Start();

            // Tạo một thread mới để chạy hàm auto Lật thẻ bài
            Thread thrAutoLatthebai = new Thread(new ThreadStart(AutoLatthebai));
            thrAutoLatthebai.Priority = ThreadPriority.Normal;
            thrAutoLatthebai.IsBackground = true;
            thrAutoLatthebai.Start();

            // Tạo một thread mới để chạy hàm auto tổ đội
            Thread thrAutoGroup = new Thread(new ThreadStart(AutoGroup));
            thrAutoGroup.Priority = ThreadPriority.Normal;
            thrAutoGroup.IsBackground = true;
            thrAutoGroup.Start();

            // Tạo một thread mới để chạy hàm auto an bảo hạp + đốt pháo
            Thread thrAutoDotPhaoBH = new Thread(new ThreadStart(AutoDotPhaoBH));
            thrAutoDotPhaoBH.Priority = ThreadPriority.Normal;
            thrAutoDotPhaoBH.IsBackground = true;
            thrAutoDotPhaoBH.Start();

            Start();
        }

        #endregion

        // Thủ tục get tọa độ nhân vật lên label form
        #region Thủ tục get tọa độ nhân vật lên label form
        private void Gettoadonv()
        {
            while (true)
            {
                Thread.Sleep(500);

                try
                {
                    if (_clients.Count < 1)
                        continue;

                    var _clientsgettoado = new Dictionary<IntPtr, AutoClientBS>(_clients);

                    foreach (var autoClient in _clientsgettoado.Values)
                    {
                        if (CurrentClient != null)
                        {
                            if (!autoClient.isInjected)
                                autoClient.Inject();

                            autoClient.ProsX = CurrentClient.CurrentPlayer.PositionX;
                            autoClient.ProsY = CurrentClient.CurrentPlayer.PositionY;

                            lbprosX.Invoke(new MethodInvoker(() =>
                            {
                                lbprosX.Text = autoClient.ProsX.ToString();
                            }));

                            lbProsY.Invoke(new MethodInvoker(() =>
                            {
                                lbProsY.Text = autoClient.ProsY.ToString();
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Auto get toa do NV: " + ex.Message);
                    return;
                }
            }
        }
        #endregion

        // Hàm bơm máu
        #region Hàm bơm máu
        private void bommau()
        {
            while (true)
            {
                Thread.Sleep(500);

                try
                {
                    if (_clients.Count < 1)
                        continue;

                    var _clientsbommau = new Dictionary<IntPtr, AutoClientBS>(_clients);

                    foreach (var autoClient in _clientsbommau.Values)
                    {
                        if (autoClient.FeatureHoiPhuc == true && autoClient.ischecked == true)
                        {

                            if (!autoClient.isInjected)
                                autoClient.Inject();

                            // Thủ tục ăn máu
                            if (autoClient.CurrentPlayer.HitPoint <= autoClient.HPuse && autoClient.CurrentPlayer.HitPoint > 1000 && (autoClient.ckbommau == true || autoClient.ckbomsinhnoi == true || autoClient.ckancc == true))
                            {
                                autoClient.FindAndUseItem(autoClient.ItemSinhLuc.ToString());
                                Thread.Sleep(autoClient.Delay);

                                //Tạm nghỉ threat trong thời gian nhất định
                                if (autoClient.CurrentPlayer.HitPoint <= autoClient.SinhnoiUse && autoClient.CurrentPlayer.HitPoint > 1000 && (autoClient.ckbomsinhnoi == true || autoClient.ckancc == true))
                                {
                                    autoClient.FindAndUseItem(autoClient.ItemSinhNoi.ToString());
                                    Thread.Sleep(autoClient.Delay);

                                    //Tạm nghỉ threat trong thời gian nhất định
                                    if (autoClient.CurrentPlayer.HitPoint <= autoClient.CCuse && autoClient.CurrentPlayer.HitPoint > 1000 && autoClient.ckancc == true)
                                    {
                                        autoClient.FindAndUseItem(autoClient.ItemCuuChuyen.ToString());
                                        Thread.Sleep(autoClient.Delay);
                                    }
                                }
                            }

                            // Thủ tục ăn mana
                            if (autoClient.CurrentPlayer.Mana <= autoClient.ManaUse && autoClient.ckbommana == true)
                            {
                                autoClient.FindAndUseItem(autoClient.ItemNoiLuc.ToString());
                            }
                            // Thủ tục Ăn Thể lực
                            if (autoClient.CurrentPlayer.TheLuc <= autoClient.ThelucUse && autoClient.ckantheluc == true)
                            {
                                autoClient.FindAndUseItem(autoClient.ItemTheLuc.ToString());
                            }

                            // Thủ tục Ăn lak chiến trường
                            if (autoClient.CurrentPlayer.HitPoint <= autoClient.LakCTUse && autoClient.CurrentPlayer.HitPoint > 1000 && autoClient.ckanlakctr == true)
                            {
                                autoClient.FindAndUseItem(autoClient.ItemLakCtr.ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Auto bom mau: " + ex.Message);
                    return;
                }
            }
        }
        #endregion


        // Main đóng
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Giải phóng bộ nhớ khi đóng form
            this.Dispose();
            GC.Collect();
        }
      

        // Hàm pkcombo
        #region Hàm pkcombo
        private void pkcombo()
        {
            while (true)
            {
                Thread.Sleep(100);

                try
                {
                    if (_clients.Count < 1)
                        continue;

                    var _clientspkcombo = new Dictionary<IntPtr, AutoClientBS>(_clients);

                    foreach (var autoClient in _clientspkcombo.Values)
                    {
                        if (autoClient.ComboPKTLQ == true && autoClient.ischecked == true && autoClient.CurrentPlayer.Hephai == 4)
                        {
                            if (!autoClient.isInjected)
                                autoClient.Inject();
                            //
                            if (log.ToString() == autoClient.phimvac.ToString() && autoClient.cksutkngatvac == true)
                            {
                                var ProsXnew = 2 * Convert.ToInt32(autoClient.CurrentPlayer.PositionX.ToString()) - autoClient.ProsX;
                                var ProsYnew = 2 * Convert.ToInt32(autoClient.CurrentPlayer.PositionY.ToString()) - autoClient.ProsY;

                                while (log.ToString() == autoClient.phimvac.ToString() && autoClient.cksutkngatvac == true)
                                {
                                    autoClient.BuffSkill(GameConst.VACSKILL, ProsXnew, ProsYnew);
                                    Thread.Sleep(30);
                                }
                            }
                            else if (log.ToString() == autoClient.phimvac.ToString() && autoClient.cksutngatvac == true)
                            {
                                var ProsXnew = 2 * Convert.ToInt32(autoClient.CurrentPlayer.PositionX.ToString()) - autoClient.ProsX;
                                var ProsYnew = 2 * Convert.ToInt32(autoClient.CurrentPlayer.PositionY.ToString()) - autoClient.ProsY;

                                while (log.ToString() == autoClient.phimvac.ToString() && autoClient.cksutngatvac == true)
                                {
                                    autoClient.BuffSkill(GameConst.VACSKILL, ProsXnew, ProsYnew);
                                    Thread.Sleep(autoClient.timevac);
                                    autoClient.LeftClick(ProsXnew, ProsYnew);
                                }
                            }
                            else if (log.ToString() == autoClient.phimvac.ToString() && autoClient.ckxdamevac == true)
                            {
                                var ProsXnew = 2 * Convert.ToInt32(autoClient.CurrentPlayer.PositionX.ToString()) - autoClient.ProsX;
                                var ProsYnew = 2 * Convert.ToInt32(autoClient.CurrentPlayer.PositionY.ToString()) - autoClient.ProsY;
                                //
                                autoClient.BuffSkill(GameConst.VACSKILL, ProsXnew, ProsYnew);
                                Thread.Sleep(autoClient.timevac);
                                autoClient.LeftClick(ProsXnew, ProsYnew);
                                //
                                while (log.ToString() == autoClient.phimvac.ToString() && autoClient.ckxdamevac == true)
                                {
                                    autoClient.BuffSkill(GameConst.VACSKILL, ProsXnew, ProsYnew);
                                    Thread.Sleep(autoClient.timevac);
                                    autoClient.LeftClick(ProsXnew, ProsYnew);
                                }
                            }
                            else if (log.ToString() == autoClient.phimvacdt.ToString() && autoClient.ckcombodt == true)
                            {
                                var ProsXnew = 2 * Convert.ToInt32(autoClient.CurrentPlayer.PositionX.ToString()) - autoClient.ProsX;
                                var ProsYnew = 2 * Convert.ToInt32(autoClient.CurrentPlayer.PositionY.ToString()) - autoClient.ProsY;
                                //
                                while (log.ToString() == autoClient.phimvacdt.ToString() && autoClient.ckcombodt == true)
                                {
                                    autoClient.BuffSkill(GameConst.VACSKILL, ProsXnew, ProsYnew);
                                    Thread.Sleep(autoClient.timevac);
                                    autoClient.LeftClick(ProsXnew, ProsYnew);
                                    //
                                    autoClient.BuffSkill(GameConst.DtuSKILL, ProsXnew, ProsYnew);
                                    Thread.Sleep(autoClient.timevac);
                                    autoClient.LeftClick(ProsXnew, ProsYnew);
                                }
                            }
                            else if (log.ToString() == autoClient.phimvaccdt.ToString() && autoClient.ckvaccdt == true)
                            {
                                var ProsXnew = 2 * Convert.ToInt32(autoClient.CurrentPlayer.PositionX.ToString()) - autoClient.ProsX;
                                var ProsYnew = 2 * Convert.ToInt32(autoClient.CurrentPlayer.PositionY.ToString()) - autoClient.ProsY;
                                //
                                while (log.ToString() == autoClient.phimvaccdt.ToString() && autoClient.ckvaccdt == true)
                                {
                                    autoClient.BuffSkill(GameConst.VACSKILL, ProsXnew, ProsYnew);
                                    Thread.Sleep(autoClient.timevac);
                                    autoClient.LeftClick(ProsXnew, ProsYnew);
                                    autoClient.BuffSkill(GameConst.CDTSKILL5, ProsXnew, ProsYnew);
                                    Thread.Sleep(autoClient.timecdt);
                                    autoClient.LeftClick(ProsXnew, ProsYnew);
                                    autoClient.BuffSkill(GameConst.DtuSKILL, ProsXnew, ProsYnew);
                                    Thread.Sleep(autoClient.timedt);
                                    autoClient.LeftClick(ProsXnew, ProsYnew);
                                }
                            }
                            else if (log.ToString() == autoClient.phimdt1o.ToString() && autoClient.ckdt1o == true)
                            {
                                var ProsXnew = 2 * Convert.ToInt32(autoClient.CurrentPlayer.PositionX.ToString()) - autoClient.ProsX;
                                var ProsYnew = 2 * Convert.ToInt32(autoClient.CurrentPlayer.PositionY.ToString()) - autoClient.ProsY;
                                //
                                autoClient.BuffSkill(GameConst.VDQSKILL, ProsXnew, ProsYnew);
                                Thread.Sleep(autoClient.timedt);
                                autoClient.BuffSkill(GameConst.VDQSKILL, ProsXnew, ProsYnew);
                                Thread.Sleep(autoClient.timedt);
                                //
                                while (log.ToString() == autoClient.phimdt1o.ToString() && autoClient.ckdt1o == true)
                                {
                                    autoClient.BuffSkill(GameConst.DtuSKILL, ProsXnew, ProsYnew);
                                    Thread.Sleep(30);
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Auto Combo: " + ex.Message);
                    return;
                }
            }
        }
        #endregion

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

        //Lưu lại các handle vào Client
        private bool SearchForGameWindows(IntPtr hwnd, IntPtr lParam)
        {
            StringBuilder title = new StringBuilder();
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
        
        
        // Sự kiện lựa chọn combobox 
        private void CBchayAutoCombo_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ComboPKTLQ = CBchayAutoCombo.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckComboTLQ", "CheckComboTLQ.Check1", CurrentClient.ComboPKTLQ.ToString());
            }
        }

     
        // Thủ tục tự động buff NMK
        #region Thủ tục tự động buff NMK

        private void Autobuff()
        {
            while (true)
            {

                Thread.Sleep(1000);

                try
                {
                    if (_clients.Count < 1)
                        continue;

                    var _clientsAutoBuff = new Dictionary<IntPtr, AutoClientBS>(_clients);

                    foreach (var autoClient in _clientsAutoBuff.Values)
                    {

                        if (autoClient.Featurenmk == true && autoClient.ischecked == true && autoClient.Autobuffnmk == true && autoClient.CurrentPlayer.Hephai == 8)
                        {
                            if (!autoClient.isInjected)
                                autoClient.Inject();

                            if (autoClient.CurrentPlayer.IsOnHorse != true)
                            {
                                autoClient.XuongNgua(6);
                            }

                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                            {
                                autoClient.VeThanh(3);
                            }

                            // Lựa chọn giá trị của StepBuff
                            switch (autoClient.StepbuffNMK)
                            {
                                case 0:
                                    {
                                        autoClient.SelfBuffSkill(GameConst.SenIDKill);

                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                        {

                                            autoClient.StepbuffNMK = 1;
                                        }
                                        break;
                                    }
                                case 1:
                                    {
                                        autoClient.SelfBuffSkill(GameConst.LTIDKill);

                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                        {

                                            autoClient.StepbuffNMK = 2;
                                        }
                                        break;
                                    }
                                case 2:
                                    {
                                        autoClient.SelfBuffSkill(GameConst.BDIDKill);

                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                        {

                                            autoClient.StepbuffNMK = 3;
                                        }
                                        break;
                                    }
                                case 3:
                                    {
                                        autoClient.SelfBuffSkill(GameConst.PhoteIDKill);
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                        {

                                            autoClient.StepbuffNMK = 4;
                                        }
                                        break;
                                    }
                                case 4:
                                    {
                                        autoClient.SelfBuffSkill(GameConst.NgoaiIDKill);

                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                        {

                                            autoClient.StepbuffNMK = 5;
                                        }
                                        break;
                                    }
                                case 5:
                                    {
                                        autoClient.SelfBuffSkill(GameConst.NoiIDKill);
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                        {

                                            autoClient.StepbuffNMK = 6;
                                        }
                                        break;
                                    }
                                case 6:
                                    {
                                        autoClient.SelfBuffSkill(GameConst.ManaIDKill);

                                        // Bắt đầu lựa chọn sen theo danh sách hoặc sen cho tất cả người chơi
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                        {
                                            if (autoClient.Featurenmk == true && autoClient.Autobuffnmk == true && autoClient.ischecked == true && autoClient.buffdanhsach == false && autoClient.CurrentPlayer.Hephai == 8)
                                            {
                                                autoClient.RefreshEntityList();

                                                autoClient._Danhsachbuff = (from entity in autoClient.EntityList where entity.EntityType == NPCType.Player && Convert.ToInt32(GameGeneral.Distance(autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 400 orderby entity.EntityId select entity.EntityNameNoMark).ToList();

                                                int i = autoClient._Danhsachbuff.Count;

                                                if (i == 1)
                                                {

                                                    autoClient.StepbuffNMK = 7;

                                                }
                                                if (i == 2)
                                                {

                                                    autoClient.StepbuffNMK = 8;
                                                }
                                                if (i == 3)
                                                {

                                                    autoClient.StepbuffNMK = 10;
                                                }
                                                if (i == 4)
                                                {

                                                    autoClient.StepbuffNMK = 13;
                                                }
                                                if (i == 5)
                                                {

                                                    autoClient.StepbuffNMK = 17;
                                                }
                                                if (i == 6)
                                                {

                                                    autoClient.StepbuffNMK = 22;
                                                }
                                                if (i == 7)
                                                {

                                                    autoClient.StepbuffNMK = 28;
                                                }
                                                if (i >= 8)
                                                {

                                                    autoClient.StepbuffNMK = 35;
                                                }
                                            }
                                            else if (autoClient.Featurenmk == true && autoClient.Autobuffnmk == true && autoClient.ischecked == true && autoClient.buffdanhsach == true && autoClient.CurrentPlayer.Hephai == 8)
                                            {

                                                foreach (string item in CurrentClient.Listbuff)
                                                {
                                                    if ((item.ToString() != "" || item.ToString() != null) && !autoClient._Danhsachbuff.Contains(item))
                                                        autoClient._Danhsachbuff.Add(item);
                                                }

                                                int i = autoClient._Danhsachbuff.Count;

                                                if (i == 0)
                                                {

                                                    autoClient.StepbuffNMK = 0;

                                                }

                                                if (i == 1)
                                                {

                                                    autoClient.StepbuffNMK = 7;

                                                }
                                                if (i == 2)
                                                {

                                                    autoClient.StepbuffNMK = 8;
                                                }
                                                if (i == 3)
                                                {

                                                    autoClient.StepbuffNMK = 10;
                                                }
                                                if (i == 4)
                                                {

                                                    autoClient.StepbuffNMK = 13;
                                                }
                                                if (i == 5)
                                                {

                                                    autoClient.StepbuffNMK = 17;
                                                }
                                                if (i == 6)
                                                {

                                                    autoClient.StepbuffNMK = 22;
                                                }
                                                if (i == 7)
                                                {

                                                    autoClient.StepbuffNMK = 28;
                                                }
                                                if (i >= 8)
                                                {

                                                    autoClient.StepbuffNMK = 35;
                                                }

                                            }
                                        }
                                        break;
                                    }

                                case 7:
                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[0].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 0;
                                    }
                                    break;

                                case 8:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[0].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 9;

                                    }
                                    break;

                                case 9:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[1].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 0;

                                    }
                                    break;


                                case 10:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[0].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 11;

                                    }
                                    break;

                                case 11:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[1].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 12;

                                    }
                                    break;
                                case 12:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[2].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 0;

                                    }
                                    break;
                                case 13:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[0].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 14;

                                    }
                                    break;
                                case 14:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[1].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 15;

                                    }
                                    break;

                                case 15:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[2].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 16;

                                    }
                                    break;

                                case 16:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[3].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 0;

                                    }
                                    break;
                                case 17:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[0].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 18;

                                    }
                                    break;
                                case 18:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[1].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 19;

                                    }
                                    break;
                                case 19:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[2].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 20;

                                    }
                                    break;
                                case 20:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[3].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 21;

                                    }
                                    break;
                                case 21:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[4].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 0;

                                    }
                                    break;
                                case 22:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[0].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 23;

                                    }
                                    break;
                                case 23:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[1].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 24;

                                    }
                                    break;
                                case 24:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[2].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 25;

                                    }
                                    break;
                                case 25:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[3].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 26;

                                    }
                                    break;
                                case 26:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[4].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 27;

                                    }
                                    break;
                                case 27:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[5].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 0;

                                    }
                                    break;
                                case 28:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[0].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 29;

                                    }
                                    break;
                                case 29:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[1].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 30;

                                    }
                                    break;
                                case 30:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[2].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 31;

                                    }
                                    break;
                                case 31:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[3].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 32;

                                    }
                                    break;
                                case 32:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[4].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 33;

                                    }
                                    break;
                                case 33:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[5].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 34;

                                    }
                                    break;
                                case 34:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[6].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 0;

                                    }
                                    break;

                                case 35:


                                    autoClient.BuffPlayer(autoClient._Danhsachbuff[1].ToString());

                                    if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                    {

                                        autoClient.StepbuffNMK = 0;

                                    }
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Auto Buff: " + ex.Message);
                    return;
                }
            }
        }

        #endregion

        // Thủ tục chuẩn bị đóng Form
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop(); // dừng Hook Keyboard


            // Đóng Hook Game
            try
            { 
                foreach (var autoClient in _clients.Values)
                {
                    if (autoClient.isInjected)
                        autoClient.DeInject();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Dong Form: " + ex.Message);
                return;
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
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";

                WinAPI.Ghifile(text, "CheckPhucHoi", "CheckPhucHoi.Check1", CurrentClient.FeatureHoiPhuc.ToString());
            }
        }

        // Thủ tục theo sau
        #region Thủ tục theo sau
        private void theosau()
        {
            while (true)
            {
                Thread.Sleep(500);

                try
                {
                    if (_clients.Count < 1)
                        continue;

                    var _clientstheosau = new Dictionary<IntPtr, AutoClientBS>(_clients);

                    foreach (var autoClient in _clientstheosau.Values)
                    {
                        if (autoClient.PlayerTheoSau.ToString() != "" && autoClient.PlayerTheoSau.ToString() != null && autoClient.Bufftheosau == true)
                        {
                            if (!autoClient.isInjected)
                                autoClient.Inject();

                            autoClient.RefreshEntityList();

                            var posX = autoClient.CurrentPlayer.PositionX;
                            var posY = autoClient.CurrentPlayer.PositionY;

                            foreach (var entity in autoClient.EntityList)
                            {

                                if (entity.EntityNameUnicode.Contains(autoClient.PlayerTheoSau))
                                {
                                    int d = Convert.ToInt32(GameGeneral.Distance(posX, posY, entity.PositionX, entity.PositionY));

                                    if (d > 300)
                                    {
                                        autoClient.Autobuffnmk = false;

                                        autoClient.ShortMove(entity.PositionX, entity.PositionY);
                                        break;
                                    }
                                    else
                                    {
                                        autoClient.Autobuffnmk = true;

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Auto Theo sau: " + ex.Message);
                    return;
                }
            }
        }
        #endregion

                
        // Thủ tục click NPC
        #region Thủ tục click NPC
        private void ClickNPc()
        {
            while (true)
            {
                Thread.Sleep(100);

                try
                {
                    if (_clients.Count < 1)
                        continue;

                    var _clientsclickNPC = new Dictionary<IntPtr, AutoClientBS>(_clients);

                    foreach (var autoClient in _clientsclickNPC.Values)
                    {
                        if (autoClient.ckClickNPC == true && autoClient.ischecked == true)
                        {

                            if (!autoClient.isInjected)
                                autoClient.Inject();

                            if (autoClient.ckClickNPC == true && autoClient.ischecked == true && autoClient.ckClickMenu1 == true && autoClient.ckClickMenu2 == false && autoClient.ckClickMenu3 == false && autoClient.ckClickMenu4 == false)
                            {
                                autoClient.ClickNPC(autoClient.ValueClickNPC.ToString());
                                Thread.Sleep(autoClient.ValueGiancach);
                                autoClient.Chonmenu(autoClient.ValueClickMenu1.ToString());
                                Thread.Sleep(autoClient.ValueGiancach);
                            }
                            else if (autoClient.ckClickNPC == true && autoClient.ischecked == true && autoClient.ckClickMenu1 == true && autoClient.ckClickMenu2 == true && autoClient.ckClickMenu3 == false && autoClient.ckClickMenu4 == false)
                            {
                                autoClient.ClickNPC(autoClient.ValueClickNPC.ToString());
                                Thread.Sleep(autoClient.ValueGiancach);
                                autoClient.Chonmenu(autoClient.ValueClickMenu1.ToString());
                                Thread.Sleep(autoClient.ValueGiancach);
                                autoClient.Chonmenu(autoClient.ValueClickMenu2.ToString());
                                Thread.Sleep(autoClient.ValueGiancach);
                            }
                            else if (autoClient.ckClickNPC == true && autoClient.ischecked == true && autoClient.ckClickMenu1 == true && autoClient.ckClickMenu2 == true && autoClient.ckClickMenu3 == true && autoClient.ckClickMenu4 == false)
                            {
                                autoClient.ClickNPC(autoClient.ValueClickNPC.ToString());
                                Thread.Sleep(autoClient.ValueGiancach);
                                autoClient.Chonmenu(autoClient.ValueClickMenu1.ToString());
                                Thread.Sleep(autoClient.ValueGiancach);
                                autoClient.Chonmenu(autoClient.ValueClickMenu2.ToString());
                                Thread.Sleep(autoClient.ValueGiancach);
                                autoClient.Chonmenu(autoClient.ValueClickMenu3.ToString());
                                Thread.Sleep(autoClient.ValueGiancach);
                            }
                            else if (autoClient.ckClickMenu1 == true && autoClient.ckClickMenu2 == true && autoClient.ckClickMenu3 == true && autoClient.ckClickMenu4 == true )
                            {
                                autoClient.ClickNPC(autoClient.ValueClickNPC.ToString());
                                Thread.Sleep(autoClient.ValueGiancach);
                                autoClient.Chonmenu(autoClient.ValueClickMenu1.ToString());
                                Thread.Sleep(autoClient.ValueGiancach);
                                autoClient.Chonmenu(autoClient.ValueClickMenu2.ToString());
                                Thread.Sleep(autoClient.ValueGiancach);
                                autoClient.Chonmenu(autoClient.ValueClickMenu3.ToString());
                                Thread.Sleep(autoClient.ValueGiancach);
                                autoClient.Chonmenu(autoClient.ValueClickMenu4.ToString());
                                Thread.Sleep(autoClient.ValueGiancach);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Auto Click NPC: " + ex.Message);
                    return;
                }
            }
        }
        #endregion

        //Thủ tục auto click tài nguyên chiến
        #region Thủ tục auto click tài nguyên chiến
        private void autoTNC()
        {
            while (true)
            {
                Thread.Sleep(500);

                try
                {
                    if (_clients.Count < 1)
                        continue;

                    var _clientTNC = new Dictionary<IntPtr, AutoClientBS>(_clients);

                    foreach (var autoClient in _clientTNC.Values)
                    {

                        if (autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true)
                        {
                            if (!autoClient.isInjected)
                                autoClient.Inject();

                            if (autoClient.ToadovitridaoTNC[0] == 0 && autoClient.ToadovitridaoTNC[1] == 0)
                                continue;

                            if (autoClient.CurrentMapId == 606 && Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaithietphu12T[0], GameConst.Toadobaithietphu12T[1], autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1])) < 1000)
                            {

                                autoClient.BaidaoTNC = 1;
                            }
                            else if (autoClient.CurrentMapId == 606 && Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao12T[0], GameConst.Toadobaichebidao12T[1], autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1])) < 1000)
                            {

                                autoClient.BaidaoTNC = 2;
                            }
                            else if (autoClient.CurrentMapId == 606 && Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaicuocchim5T[0], GameConst.Toadobaicuocchim5T[1], autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1])) < 700)
                            {

                                autoClient.BaidaoTNC = 3;
                            }
                            else if (autoClient.CurrentMapId == 606 && Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaicuocchim12L[0], GameConst.Toadobaicuocchim12L[1], autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1])) < 1100)
                            {

                                autoClient.BaidaoTNC = 4;
                            }
                            else if (autoClient.CurrentMapId == 606 && Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaicuocthuoc12L[0], GameConst.Toadobaicuocthuoc12L[1], autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1])) < 1000)
                            {

                                autoClient.BaidaoTNC = 5;
                            }
                            else if (autoClient.CurrentMapId == 606 && Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1])) < 700)
                            {

                                autoClient.BaidaoTNC = 6;
                            }
                            else if (autoClient.CurrentMapId == 606 && Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaithietphu6Tren[0], GameConst.Toadobaithietphu6Tren[1], autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1])) < 700)
                            {

                                autoClient.BaidaoTNC = 7;
                            }
                            else if (autoClient.CurrentMapId == 606 && Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaicuocthuoc5Duoi[0], GameConst.Toadobaicuocthuoc5Duoi[1], autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1])) < 800)
                            {

                                autoClient.BaidaoTNC = 8;
                            }
                            else if (autoClient.CurrentMapId == 606 && Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaicuocchim6Duoi[0], GameConst.Toadobaicuocchim6Duoi[1], autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1])) < 700)
                            {

                                autoClient.BaidaoTNC = 9;
                            }
                            else if (autoClient.CurrentMapId == 606 && Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaithietphu5Duoi[0], GameConst.Toadobaithietphu5Duoi[1], autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1])) < 700)
                            {

                                autoClient.BaidaoTNC = 10;
                            }

                            switch (autoClient.BaidaoTNC)
                            {
                                //////////////////////////////Thủ tục ở bãi đào 12 gỗ tài nguyên tống //////////////////////////////
                                case 1:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (autoClient.FindItem("Thiet phu").ToString() == "True" && autoClient.SolandaoTNC < 3000)
                                        {
                                            autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);

                                            if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 50)
                                            {
                                                autoClient.ClickTNC();
                                                Thread.Sleep(200);
                                            }
                                        }

                                        else if (autoClient.FindItem("Thiet phu").ToString() == "False" && autoClient.cktumuadungcu == true && autoClient.SolandaoTNC < 3000)
                                        {
                                            autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);

                                            if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100 && autoClient.FindItem("Thiet phu").ToString() == "False" && autoClient.cktumuadungcu == true)
                                            {
                                                autoClient.ClickNPCvuotai("Hoang Tri thuong nhan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua dung cu pho thong (10 bac/cai)");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua 1 Thiet Phu (gia 10 bac)");
                                                Thread.Sleep(500);

                                                // Send phim
                                                autoClient.sendstring("10");
                                                Thread.Sleep(500);
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_RETURN));
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);
                                            }
                                        }

                                        if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == true)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 1;
                                        }
                                        else if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == false)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 21;
                                        }

                                        break;
                                    }

                                //////////////////////////////Thủ tục ở bãi đào 12 Thuộc da tài nguyên tống //////////////////////////////
                                case 2:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (autoClient.FindItem("Che Bi dao").ToString() == "True" && autoClient.SolandaoTNC < 3000)
                                        {

                                            if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(43002, 95259);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(43002, 95259, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(43002, 95259, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 50 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 50)
                                            {
                                                autoClient.ClickTNC();
                                                Thread.Sleep(200);
                                            }
                                        }

                                        else if (autoClient.FindItem("Che Bi dao").ToString() == "False" && autoClient.cktumuadungcu == true && autoClient.SolandaoTNC < 3000)
                                        {
                                            if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 1000)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(43082, 94535);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(43082, 94535, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(43082, 94535, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100 && autoClient.FindItem("Che Bi dao").ToString() == "False" && autoClient.cktumuadungcu == true)
                                            {
                                                autoClient.ClickNPCvuotai("Hoang Tri thuong nhan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua dung cu pho thong (10 bac/cai)");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua 1 Che Bi Dao (gia 10 bac)");
                                                Thread.Sleep(500);

                                                // Send phim
                                                autoClient.sendstring("10");
                                                Thread.Sleep(500);
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_RETURN));
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);

                                            }
                                        }

                                        if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == true)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 3;
                                        }
                                        else if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == false)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 23;
                                        }
                                        break;
                                    }


                                //////////////////////////////Thủ tục ở bãi đào 5 rương châu báu tài nguyên tống //////////////////////////////
                                case 3:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (autoClient.FindItem("Cuoc chim").ToString() == "True" && autoClient.SolandaoTNC < 3000)
                                        {
                                            autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);

                                            if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 50)
                                            {
                                                autoClient.ClickTNC();
                                                Thread.Sleep(200);
                                            }
                                        }

                                        else if (autoClient.FindItem("Cuoc chim").ToString() == "False" && autoClient.cktumuadungcu == true && autoClient.SolandaoTNC < 3000)
                                        {
                                            autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);

                                            if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100 && autoClient.FindItem("Cuoc chim").ToString() == "False" && autoClient.cktumuadungcu == true)
                                            {
                                                autoClient.ClickNPCvuotai("Hoang Tri thuong nhan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua dung cu pho thong (10 bac/cai)");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua 1 Cuoc Chim (gia 10 bac)");
                                                Thread.Sleep(500);

                                                // Send phim
                                                autoClient.sendstring("10");
                                                Thread.Sleep(500);
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_RETURN));
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);

                                            }
                                        }

                                        if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == true)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 5;
                                        }
                                        else if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == false)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 25;
                                        }
                                        break;
                                    }

                                //////////////////////////////Thủ tục ở bãi đào 12 rương châu báu tài nguyên liêu //////////////////////////////
                                case 4:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (autoClient.FindItem("Cuoc chim").ToString() == "True" && autoClient.SolandaoTNC < 3000)
                                        {
                                            autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);

                                            if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 50)
                                            {
                                                autoClient.ClickTNC();
                                                Thread.Sleep(200);
                                            }
                                        }

                                        else if (autoClient.FindItem("Cuoc chim").ToString() == "False" && autoClient.cktumuadungcu == true && autoClient.SolandaoTNC < 3000)
                                        {
                                            autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);

                                            if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100 && autoClient.FindItem("Cuoc chim").ToString() == "False" && autoClient.cktumuadungcu == true)
                                            {
                                                autoClient.ClickNPCvuotai("Hoang Tri thuong nhan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua dung cu pho thong (10 bac/cai)");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua 1 Cuoc Chim (gia 10 bac)");
                                                Thread.Sleep(500);

                                                // Send phim
                                                autoClient.sendstring("10");
                                                Thread.Sleep(500);
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_RETURN));
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);
                                            }
                                        }

                                        if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == true)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 7;
                                        }
                                        else if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == false)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 27;
                                        }

                                        break;
                                    }

                                //////////////////////////////Thủ tục ở bãi đào 12 thảo dược tài nguyên liêu //////////////////////////////

                                case 5:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (autoClient.FindItem("Cuoc thuoc").ToString() == "True" && autoClient.SolandaoTNC < 3000)
                                        {
                                            if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(44633, 96179);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 50 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 50)
                                            {
                                                autoClient.ClickTNC();
                                                Thread.Sleep(200);
                                            }
                                        }

                                        else if (autoClient.FindItem("Cuoc thuoc").ToString() == "False" && autoClient.cktumuadungcu == true && autoClient.SolandaoTNC < 3000)
                                        {
                                            if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 1000)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(44633, 96179);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100 && autoClient.FindItem("Cuoc thuoc").ToString() == "False" && autoClient.cktumuadungcu == true)
                                            {
                                                autoClient.ClickNPCvuotai("Hoang Tri thuong nhan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua dung cu pho thong (10 bac/cai)");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua 1 Cuoc Thuoc (gia 10 bac)");
                                                Thread.Sleep(500);

                                                // Send phim
                                                autoClient.sendstring("10");
                                                Thread.Sleep(500);
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_RETURN));
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);
                                            }
                                        }

                                        if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == true)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 9;
                                        }
                                        else if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == false)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 29;
                                        }

                                        break;
                                    }


                                //////////////////////////////Thủ tục ở bãi đào 5 thuộc da tài nguyên liêu //////////////////////////////
                                case 6:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (autoClient.FindItem("Che Bi dao").ToString() == "True" && autoClient.SolandaoTNC < 3000)
                                        {
                                            autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);

                                            if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 50)
                                            {
                                                autoClient.ClickTNC();
                                                Thread.Sleep(200);
                                            }
                                        }

                                        else if (autoClient.FindItem("Che Bi dao").ToString() == "False" && autoClient.cktumuadungcu == true && autoClient.SolandaoTNC < 3000)
                                        {
                                            autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);

                                            if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100 && autoClient.FindItem("Che Bi dao").ToString() == "False" && autoClient.cktumuadungcu == true)
                                            {
                                                autoClient.ClickNPCvuotai("Hoang Tri thuong nhan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua dung cu pho thong (10 bac/cai)");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua 1 Che Bi Dao (gia 10 bac)");
                                                Thread.Sleep(500);

                                                // Send phim
                                                autoClient.sendstring("10");
                                                Thread.Sleep(500);
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_RETURN));
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);
                                            }
                                        }

                                        if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == true)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 11;
                                        }
                                        else if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == false)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 31;
                                        }
                                        break;
                                    }

                                //////////////////////////////Thủ tục ở bãi đào 6 gỗ tài nguyên trên //////////////////////////////
                                case 7:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }


                                        if (autoClient.FindItem("Thiet phu").ToString() == "True" && autoClient.SolandaoTNC < 3000)
                                        {
                                            if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(44633, 96179);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(44873, 94263);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(44873, 94263, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(44873, 94263, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(46054, 93132);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(46054, 93132, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(46054, 93132, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(47281, 91336);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(47281, 91336, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(47281, 91336, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 50 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 50)
                                            {
                                                autoClient.ClickTNC();
                                                Thread.Sleep(200);
                                            }
                                        }

                                        else if (autoClient.FindItem("Thiet phu").ToString() == "False" && autoClient.cktumuadungcu == true && autoClient.SolandaoTNC < 3000)
                                        {
                                            if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 1000)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(47281, 91336);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(47281, 91336, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(47281, 91336, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(46054, 93132);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(46054, 93132, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(46054, 93132, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(44873, 94263);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(44873, 94263, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(44873, 94263, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(44633, 96179);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100 && autoClient.FindItem("Thiet phu").ToString() == "False" && autoClient.cktumuadungcu == true)
                                            {
                                                autoClient.ClickNPCvuotai("Hoang Tri thuong nhan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua dung cu pho thong (10 bac/cai)");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua 1 Thiet Phu (gia 10 bac)");
                                                Thread.Sleep(500);

                                                // Send phim
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_1));
                                                Thread.Sleep(500);
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_1));
                                                Thread.Sleep(500);
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_RETURN));
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);
                                            }
                                        }

                                        if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == true)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 13;
                                        }
                                        else if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == false)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 33;
                                        }
                                        break;
                                    }

                                //////////////////////////////Thủ tục ở bãi đào 5 dược thảo tài nguyên dưới //////////////////////////////

                                case 8:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (autoClient.FindItem("Cuoc thuoc").ToString() == "True" && autoClient.SolandaoTNC < 3000)
                                        {
                                            if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 300)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(42729, 96537);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(42729, 96537, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(42729, 96537, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(41450, 97820);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(40834, 100361);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(40834, 100361, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(40834, 100361, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 50 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 50)
                                            {
                                                autoClient.ClickTNC();
                                                Thread.Sleep(200);
                                            }
                                        }

                                        else if (autoClient.FindItem("Cuoc thuoc").ToString() == "False" && autoClient.cktumuadungcu == true && autoClient.SolandaoTNC < 3000)
                                        {
                                            if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 800)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(40834, 100361);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(40834, 100361, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(40834, 100361, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 800)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(41450, 97820);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 800)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100 && autoClient.FindItem("Cuoc thuoc").ToString() == "False" && autoClient.cktumuadungcu == true)
                                            {
                                                autoClient.ClickNPCvuotai("Hoang Tri thuong nhan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua dung cu pho thong (10 bac/cai)");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua 1 Cuoc Thuoc (gia 10 bac)");
                                                Thread.Sleep(500);

                                                // Send phim
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_1));
                                                Thread.Sleep(500);
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_1));
                                                Thread.Sleep(500);
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_RETURN));
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);
                                            }
                                        }

                                        if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == true)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 15;
                                        }
                                        else if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == false)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 35;
                                        }
                                        break;
                                    }

                                //////////////////////////////Thủ tục ở bãi đào 6 cuốc chim tài nguyên dưới //////////////////////////////
                                case 9:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (autoClient.FindItem("Cuoc chim").ToString() == "True" && autoClient.SolandaoTNC < 3000)
                                        {
                                            if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(41450, 97820);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(40811, 98393);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 50 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 50)
                                            {
                                                autoClient.ClickTNC();
                                                Thread.Sleep(200);
                                            }
                                        }

                                        else if (autoClient.FindItem("Cuoc chim").ToString() == "False" && autoClient.cktumuadungcu == true && autoClient.SolandaoTNC < 3000)
                                        {

                                            if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 800)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(40811, 98393);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 800)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(41450, 97820);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 800)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100 && autoClient.FindItem("Cuoc chim").ToString() == "False" && autoClient.cktumuadungcu == true)
                                            {
                                                autoClient.ClickNPCvuotai("Hoang Tri thuong nhan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua dung cu pho thong (10 bac/cai)");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua 1 Cuoc Chim (gia 10 bac)");
                                                Thread.Sleep(500);

                                                // Send phim
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_1));
                                                Thread.Sleep(500);
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_1));
                                                Thread.Sleep(500);
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_RETURN));
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);
                                            }
                                        }

                                        if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == true)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 17;
                                        }
                                        else if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == false)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 37;
                                        }

                                        break;
                                    }

                                //////////////////////////////Thủ tục ở bãi đào 5 thiết phủ tài nguyên dưới //////////////////////////////

                                case 10:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (autoClient.FindItem("Thiet phu").ToString() == "True" && autoClient.SolandaoTNC < 3000)
                                        {
                                            if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(41450, 97820);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(40811, 98393);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 50)
                                            {
                                                autoClient.ClickTNC();
                                                Thread.Sleep(200);
                                            }
                                        }

                                        else if (autoClient.FindItem("Thiet phu").ToString() == "False" && autoClient.cktumuadungcu == true && autoClient.SolandaoTNC < 3000)
                                        {
                                            if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 800)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(40811, 98393);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 800)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(41450, 97820);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 800)
                                            {
                                                do
                                                {
                                                    autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                    Thread.Sleep(500);
                                                }
                                                while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                            }

                                            if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 100 && autoClient.FindItem("Thiet phu").ToString() == "False" && autoClient.cktumuadungcu == true)
                                            {
                                                autoClient.ClickNPCvuotai("Hoang Tri thuong nhan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua dung cu pho thong (10 bac/cai)");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Mua 1 Thiet Phu (gia 10 bac)");
                                                Thread.Sleep(500);

                                                // Send phim
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_1));
                                                Thread.Sleep(500);
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_1));
                                                Thread.Sleep(500);
                                                autoClient.sendphim(Convert.ToInt32(WinAPI.keyflag.KEY_RETURN));
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);
                                            }
                                        }

                                        if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == true)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 19;
                                        }
                                        else if (autoClient.SolandaoTNC >= 3000 && autoClient.cktutranv == true && autoClient.ckphetong == false)
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(500);
                                            autoClient.Tranhiemvu = 39;
                                        }

                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }

                            }

                            switch (autoClient.Tranhiemvu)
                            {
                                ////////////////////////////////////////Thủ tục trả nhiệm vụ bãi gỗ 12 tống /////////////////////////////////////////(ok)
                                case 1:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42804, 94264);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42804, 94264, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42804, 94264, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Trieu To Quyen");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);

                                                if (autoClient.SolandaoTNC != 0)
                                                    continue;

                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 2;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);

                                                if (autoClient.SolandaoTNC != 0)
                                                    continue;


                                                autoClient.Tranhiemvu = 2;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);

                                                if (autoClient.SolandaoTNC != 0)
                                                    continue;


                                                autoClient.Tranhiemvu = 2;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);

                                                if (autoClient.SolandaoTNC != 0)
                                                    continue;


                                                autoClient.Tranhiemvu = 2;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);

                                                if (autoClient.SolandaoTNC != 0)
                                                    continue;


                                                autoClient.Tranhiemvu = 2;
                                            }
                                        }

                                        break;
                                    }

                                case 2:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42804, 94264);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42804, 94264, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42804, 94264, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 1;
                                        }

                                        break;
                                    }

                                ////////////////////////////////////////Thủ tục trả nhiệm vụ bãi thuộc da 12 tống ///////////////////////////////////////// (ok)
                                case 3:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42804, 94264);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42804, 94264, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42804, 94264, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Trieu To Quyen");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 4;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 4;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 4;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 4;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 4;
                                            }
                                        }

                                        break;
                                    }

                                case 4:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42804, 94264);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42804, 94264, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42804, 94264, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 2;
                                        }

                                        break;
                                    }

                                ////////////////////////////////////////Thủ tục trả nhiệm vụ châu báu 5 tống ///////////////////////////////////////// (ok)
                                case 5:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Trieu To Quyen");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 6;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 6;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 6;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 6;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 6;
                                            }
                                        }

                                        break;
                                    }

                                case 6:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 3;
                                        }

                                        break;
                                    }

                                ////////////////////////////////////////Thủ tục trả nhiệm vụ châu báu 12 Liêu ///////////////////////////////////////// (ok)
                                case 7:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42361, 93803);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Trieu To Quyen");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 8;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 8;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 8;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 8;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 8;
                                            }
                                        }

                                        break;
                                    }

                                case 8:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42361, 93803);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 4;
                                        }

                                        break;
                                    }

                                ////////////////////////////////////////Thủ tục trả nhiệm vụ bãi thảo dược 12 liêu ///////////////////////////////////////// (ok)
                                case 9:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44055, 94775);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44055, 94775, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44055, 94775, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42361, 93803);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Trieu To Quyen");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 10;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 10;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 10;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 10;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 10;
                                            }
                                        }

                                        break;
                                    }

                                case 10:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42361, 93803);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44055, 94775);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44055, 94775, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44055, 94775, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 5;
                                        }

                                        break;
                                    }

                                ////////////////////////////////////////Thủ tục trả nhiệm vụ bãi thuộc da 5 liêu ///////////////////////////////////////// (ok)
                                case 11:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44055, 94775);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44055, 94775, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44055, 94775, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42361, 93803);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Trieu To Quyen");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 12;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 12;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 12;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 12;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 12;
                                            }
                                        }

                                        break;
                                    }

                                case 12:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42361, 93803);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44055, 94775);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44055, 94775, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44055, 94775, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 6;
                                        }

                                        break;
                                    }

                                ////////////////////////////////////////Thủ tục trả nhiệm vụ bãi gỗ 6 trên ///////////////////////////////////////// (ok)
                                case 13:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(47281, 91336);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(47281, 91336, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(47281, 91336, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(46054, 93132);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(46054, 93132, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(46054, 93132, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44873, 94263);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44873, 94263, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44873, 94263, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42361, 93803);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Trieu To Quyen");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 14;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 14;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 14;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 14;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 14;
                                            }
                                        }

                                        break;
                                    }


                                case 14:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42361, 93803);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44873, 94263);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44873, 94263, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44873, 94263, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(46054, 93132);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(46054, 93132, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(46054, 93132, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(47281, 91336);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(47281, 91336, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(47281, 91336, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 7;
                                        }

                                        break;
                                    }

                                ////////////////////////////////////////Thủ tục trả nhiệm vụ bãi thảo dược 5 dưới ///////////////////////////////////////// (ok)
                                case 15:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(40834, 100361);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(40834, 100361, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(40834, 100361, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(41450, 97820);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42361, 93803);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Trieu To Quyen");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 16;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 16;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 16;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 16;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 16;
                                            }
                                        }

                                        break;
                                    }


                                case 16:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42361, 93803);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(41450, 97820);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(40834, 100361);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(40834, 100361, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(40834, 100361, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 8;
                                        }

                                        break;
                                    }

                                ////////////////////////////////////////Thủ tục trả nhiệm vụ bãi châu báu 5 dưới ///////////////////////////////////////// (ok)
                                case 17:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(40811, 98393);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(41450, 97820);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42361, 93803);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Trieu To Quyen");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 18;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 18;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 18;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 18;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 18;
                                            }
                                        }

                                        break;
                                    }


                                case 18:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42361, 93803);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(41450, 97820);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(40811, 98393);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 9;
                                        }

                                        break;
                                    }

                                ////////////////////////////////////////Thủ tục trả nhiệm vụ bãi thiết phủ 5 dưới ///////////////////////////////////////// (ok)
                                case 19:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(40811, 98393);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(41450, 97820);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42361, 93803);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Trieu To Quyen");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 18;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 18;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 18;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 18;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Trieu To Quyen");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 20;
                                            }
                                        }

                                        break;
                                    }

                                case 20:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvTong[0], GameConst.ToadotranvTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianTong[0], GameConst.ToadotrunggianTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongTong[0], GameConst.ToadocongTong[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(42361, 93803);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(42361, 93803, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(41450, 97820);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(40811, 98393);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 10;
                                        }
                                        break;
                                    }
                                /////////////////////////////////////////////////////////////////////Phe liêu ///////////////////////////////////////////////////

                                ////////////////////////////////////////Thủ tục trả nhiệm vụ bãi gỗ 12 tống - Phe liêu ///////////////////////////////////////// (ok)
                                case 21:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44633, 96179);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 22;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 22;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 22;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 22;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 22;
                                            }
                                        }

                                        break;
                                    }

                                case 22:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44633, 96179);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 1;
                                        }
                                        break;
                                    }

                                ////////////////////////////////////////Thủ tục trả nhiệm vụ bãi thuộc da 12 tống - Phe liêu ///////////////////////////////////////// (ok)
                                case 23:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44055, 94775);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44055, 94775, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44055, 94775, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44633, 96179);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 24;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 24;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 24;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 24;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 24;
                                            }
                                        }

                                        break;
                                    }

                                case 24:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44633, 96179);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44055, 94775);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44055, 94775, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44055, 94775, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 2;
                                        }
                                        break;
                                    }

                                ////////////////////////////////////////Thủ tục trả nhiệm vụ bãi Châu báu 5 tống - Phe liêu ///////////////////////////////////////// (ok)
                                case 25:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44633, 96179);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 26;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 26;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 26;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 26;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 26;
                                            }
                                        }

                                        break;
                                    }

                                case 26:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44633, 96179);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44633, 96179, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 3;
                                        }
                                        break;
                                    }
                                ////////////////////////////////////////Thủ tục trả nhiệm vụ bãi châu báu 12 liêu - phe liêu ///////////////////////////////////////// (ok)
                                case 27:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44547, 96052);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44547, 96052, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44547, 96052, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 28;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 28;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 28;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 28;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 28;
                                            }
                                        }

                                        break;
                                    }

                                case 28:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44547, 96052);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44547, 96052, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44547, 96052, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 4;
                                        }

                                        break;
                                    }
                                ////////////////////////////////////////Thủ tục trả nhiệm vụ bãi thảo dược 12 liêu - phe liêu ///////////////////////////////////////// (ok)
                                case 29:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44547, 96052);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44547, 96052, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44547, 96052, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 30;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 30;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 30;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 30;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 30;
                                            }
                                        }

                                        break;
                                    }

                                case 30:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44547, 96052);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44547, 96052, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44547, 96052, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 5;
                                        }

                                        break;
                                    }

                                ////////////////////////////////////////Thủ tục trả nhiệm vụ bãi thuộc da 5 liêu - phe liêu ///////////////////////////////////////// (ok)
                                case 31:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 800)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 32;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 32;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 32;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 32;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 32;
                                            }
                                        }

                                        break;
                                    }

                                case 32:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 6;
                                        }

                                        break;
                                    }

                                ////////////////////////////////////////Thủ tục trả nhiệm vụ bãi gỗ 6 trên - phe liêu///////////////////////////////////////// (ok)
                                case 33:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(47281, 91336);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(47281, 91336, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(47281, 91336, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(46054, 93132);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(46054, 93132, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(46054, 93132, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44873, 94263);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44873, 94263, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44873, 94263, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 34;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 34;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 34;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 34;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 34;
                                            }
                                        }

                                        break;
                                    }


                                case 34:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(44873, 94263);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(44873, 94263, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(44873, 94263, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(46054, 93132);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(46054, 93132, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(46054, 93132, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(47281, 91336);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(47281, 91336, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(47281, 91336, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 7;
                                        }

                                        break;
                                    }

                                ////////////////////////////////////////Thủ tục trả nhiệm vụ dược thảo 5 dưới - phe liêu///////////////////////////////////////// (ok)
                                case 35:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(40834, 100361);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(40834, 100361, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(40834, 100361, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(41450, 97820);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 36;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 36;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 36;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 36;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 36;
                                            }
                                        }

                                        break;
                                    }


                                case 36:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(41450, 97820);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(40834, 100361);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(40834, 100361, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(40834, 100361, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 8;
                                        }

                                        break;
                                    }
                                ////////////////////////////////////////Thủ tục trả nhiệm vụ châu báu 6 dưới - phe liêu///////////////////////////////////////// (ok)
                                case 37:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(40811, 98393);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(41450, 97820);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 38;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 38;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 38;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 38;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 38;
                                            }
                                        }

                                        break;
                                    }


                                case 38:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(41450, 97820);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(40811, 98393);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 9;
                                        }

                                        break;
                                    }

                                ////////////////////////////////////////Thủ tục trả nhiệm vụ bãi gỗ 5 dưới - phe liêu///////////////////////////////////////// (ok)
                                case 39:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 1000)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(40811, 98393);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(41450, 97820);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                            Thread.Sleep(500);
                                            if ((autoClient.ValueQuancong.Equals("Khong dung quan cong") || autoClient.ValueQuancong.Equals("") || autoClient.ValueQuancong.ToString() == null) && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Khong muon su dung quan cong");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Khong muon su dung quan cong").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 40;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong chuong") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Chuong");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 40;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong dai") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Dai");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 40;
                                            }

                                            else if (autoClient.ValueQuancong.Equals("Quan cong huy hoang") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Huy Hoang");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 40;
                                            }
                                            else if (autoClient.ValueQuancong.Equals("Quan cong vinh du") && Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.Chonmenu("Hoan thanh nhiem vu Thu Thap Tai Nguyen hang ngay");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Su dung Quan Cong Vinh Du");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Dong y");
                                                Thread.Sleep(500);

                                                if (autoClient.FindMenu("Dong y").ToString() == "True")
                                                    continue;

                                                autoClient.ClickNPCvuotai("Tieu Vien Tuan");
                                                Thread.Sleep(500);
                                                autoClient.Chonmenu("Nhan nhiem vu Thu Thap Tai Nguyen");
                                                Thread.Sleep(500);
                                                autoClient.SelfBuffSkill(0);


                                                autoClient.Tranhiemvu = 40;
                                            }
                                        }

                                        break;
                                    }


                                case 40:
                                    {
                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                        {
                                            autoClient.VeThanh(17);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotranvLieu[0], GameConst.ToadotranvLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadotrunggianLieu[0], GameConst.ToadotrunggianLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.ToadocongLieu[0], GameConst.ToadocongLieu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadobaichebidao5L[0], GameConst.Toadobaichebidao5L[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(GameConst.Toadomuadungcu[0], GameConst.Toadomuadungcu[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(41450, 97820);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(41450, 97820, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(40811, 98393);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(40811, 98393, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {
                                            do
                                            {
                                                autoClient.ShortMove(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1]);
                                                Thread.Sleep(500);
                                            }
                                            while (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY))  > 100 && autoClient.CurrentMapId == 606 && autoClient.ckautoTNC == true && autoClient.ischecked == true);
                                        }

                                        if (Convert.ToInt32(GameGeneral.Distance(autoClient.ToadovitridaoTNC[0], autoClient.ToadovitridaoTNC[1], autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                        {

                                            autoClient.BaidaoTNC = 10;
                                        }

                                        break;
                                    }
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Auto TNC: " + ex.Message);
                    return;
                }
            }
        }
        #endregion

        // Thủ tục lưu tài nguyên chiến
        private void Ckbtainguyenchien_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckautoTNC = Ckbtainguyenchien.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTNC", "AutoTNC.Check1", CurrentClient.ckautoTNC.ToString());
            }
        }

        // Thủ tục AutoTrain Quai
        #region Thủ tục AutoTrain Quai

        private void AutoTrain()
        {
            while (true)
            {
                Thread.Sleep(1000);
                try
                {

                    if (_clients.Count < 1)
                        continue;

                    var _clientstrain = new Dictionary<IntPtr, AutoClientBS>(_clients);

                    foreach (var autoClient in _clientstrain.Values)
                    {
                        if (autoClient.ckautotrain == true && autoClient.ischecked == true)
                        {

                            if (!autoClient.isInjected)
                                autoClient.Inject();

                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                            {
                                autoClient.VeThanh(3);
                            }

                            autoClient.arraytoadonv = CurrentClient.ValueToadoNV.Split('/');
                            autoClient.arraytoadoTrain01 = CurrentClient.ValueToadoTrain01.Split('/');
                            autoClient.arraytoadoTrain02 = CurrentClient.ValueToadoTrain02.Split('/');
                            autoClient.arraytoadoTrain03 = CurrentClient.ValueToadoTrain03.Split('/');

                            // Đối với phái thúy yên VT
                            if (autoClient.ckautotrain == true && autoClient.ischecked == true && autoClient.CkToadoTrain01 == true && autoClient.CkToadoTrain02 == true && autoClient.CkToadoTrain03 == true && autoClient.CkSkindanhquai == true && autoClient.CkSkinBuff01 == true && autoClient.CkSkinBuff02 == true && autoClient.CurrentPlayer.Hephai == (int)HePhai.Tyvt)
                            {
                                switch (autoClient.StepTrain)
                                {
                                    case 0:
                                        {
                                            autoClient.MoveTo(Convert.ToInt32(autoClient.arraytoadonv[0].ToString()), Convert.ToInt32(autoClient.arraytoadonv[1].ToString()), autoClient.CurrentMapId);

                                            if (Convert.ToInt32(GameGeneral.Distance(Convert.ToInt32(autoClient.arraytoadonv[0].ToString()), Convert.ToInt32(autoClient.arraytoadonv[1].ToString()), autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {

                                                autoClient.SelfBuffSkill(Convert.ToInt32(autoClient.ValueSkinBuff01.ToString()));

                                                Thread.Sleep(1000);

                                                autoClient.StepTrain = 1;
                                            
                                            }
                                            break;
                                        }
                                    case 1:
                                        {
                                            autoClient.SelfBuffSkill(Convert.ToInt32(autoClient.ValueSkinBuff02.ToString()));

                                            Thread.Sleep(1000);

                                            autoClient.StepTrain = 2;
                                        
                                            break;
                                        }
                                    case 2:
                                        {
                                            autoClient.SelfBuffSkill(Convert.ToInt32(autoClient.ValueSkindanhquai.ToString()));
                                            Thread.Sleep(100);
                                            autoClient.MoveTo(Convert.ToInt32(autoClient.arraytoadoTrain01[0].ToString()), Convert.ToInt32(autoClient.arraytoadoTrain01[1].ToString()), autoClient.CurrentMapId);

                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing || Convert.ToInt32(GameGeneral.Distance(Convert.ToInt32(autoClient.arraytoadoTrain01[0].ToString()), Convert.ToInt32(autoClient.arraytoadoTrain01[1].ToString()), autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100)
                                                continue;

                                                autoClient.StepTrain = 3;
                                        
                                            break;
                                        }
                                    case 3:
                                        {
                                            autoClient.SelfBuffSkill(Convert.ToInt32(autoClient.ValueSkindanhquai.ToString()));
                                            Thread.Sleep(100);
                                            autoClient.MoveTo(Convert.ToInt32(autoClient.arraytoadoTrain02[0].ToString()), Convert.ToInt32(autoClient.arraytoadoTrain02[1].ToString()), autoClient.CurrentMapId);

                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing || Convert.ToInt32(GameGeneral.Distance(Convert.ToInt32(autoClient.arraytoadoTrain02[0].ToString()), Convert.ToInt32(autoClient.arraytoadoTrain02[1].ToString()), autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100)
                                                continue;

                                                autoClient.StepTrain = 4;
                                        
                                            break;
                                        }
                                    case 4:
                                        {
                                            autoClient.SelfBuffSkill(Convert.ToInt32(autoClient.ValueSkindanhquai.ToString()));
                                            Thread.Sleep(100);
                                            autoClient.MoveTo(Convert.ToInt32(autoClient.arraytoadoTrain03[0].ToString()), Convert.ToInt32(autoClient.arraytoadoTrain03[1].ToString()), autoClient.CurrentMapId);

                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing || Convert.ToInt32(GameGeneral.Distance(Convert.ToInt32(autoClient.arraytoadoTrain03[0].ToString()), Convert.ToInt32(autoClient.arraytoadoTrain03[1].ToString()), autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100)
                                                continue;

                                                autoClient.StepTrain = 5;
                                        
                                            break;
                                        }
                                    case 5:
                                        {
                                            autoClient.SelfBuffSkill(Convert.ToInt32(autoClient.ValueSkindanhquai.ToString()));
                                            Thread.Sleep(100);
                                            autoClient.MoveTo(Convert.ToInt32(autoClient.arraytoadonv[0].ToString()), Convert.ToInt32(autoClient.arraytoadonv[1].ToString()), autoClient.CurrentMapId);

                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing || Convert.ToInt32(GameGeneral.Distance(Convert.ToInt32(autoClient.arraytoadoTrain03[0].ToString()), Convert.ToInt32(autoClient.arraytoadoTrain03[1].ToString()), autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 100)
                                                continue;

                                                autoClient.StepTrain = 0;
                                        
                                            break;
                                        }
                                }
                            }
                            // Cac phai khac
                            else if (autoClient.ckautotrain == true && autoClient.ischecked == true && autoClient.CkToadoTrain01 == true && autoClient.CkToadoTrain02 == true && autoClient.CkToadoTrain03 == true && autoClient.CkSkindanhquai == true && autoClient.CkSkinBuff01 == true && autoClient.CkSkinBuff02 == true && autoClient.CurrentPlayer.Hephai != (int)HePhai.Tyvt)
                            {
                                switch (autoClient.StepTrain1)
                                {
                                    case 0:
                                        {
                                            autoClient.MoveTo(int.Parse(autoClient.arraytoadonv[0].ToString()), Convert.ToInt32(autoClient.arraytoadonv[1].ToString()), autoClient.CurrentMapId);

                                            if (Convert.ToInt32(GameGeneral.Distance(Convert.ToInt32(autoClient.arraytoadonv[0].ToString()), Convert.ToInt32(autoClient.arraytoadonv[1].ToString()), autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {

                                                autoClient.SelfBuffSkill(Convert.ToInt32(autoClient.ValueSkinBuff01.ToString()));

                                                Thread.Sleep(1000);

                                                autoClient.StepTrain1 = 1;
                                            
                                            }
                                            break;
                                        }
                                    case 1:
                                        {
                                            autoClient.SelfBuffSkill(Convert.ToInt32(autoClient.ValueSkinBuff02.ToString()));

                                            Thread.Sleep(1000);

                                            autoClient.StepTrain1 = 2;
                                        
                                            break;
                                        }
                                    case 2:
                                        {
                                            autoClient.MoveTo(Convert.ToInt32(autoClient.arraytoadoTrain01[0].ToString()), Convert.ToInt32(autoClient.arraytoadoTrain01[1].ToString()), autoClient.CurrentMapId);
                                            if (Convert.ToInt32(GameGeneral.Distance(Convert.ToInt32(autoClient.arraytoadoTrain01[0].ToString()), Convert.ToInt32(autoClient.arraytoadoTrain01[1].ToString()), autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.RefreshEntityList();

                                                autoClient._Danhsachquai = new List<string>(from entity in CurrentClient.EntityList where entity.EntityType == NPCType.Beast && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 500 orderby entity.EntityId select entity.EntityId + "/" + entity.EntityNameNoMark).ToList();

                                                foreach (var quai in autoClient._Danhsachquai)
                                                {
                                                    if (quai.ToString() != "" && quai.ToString() != null)
                                                    {
                                                        autoClient.Trainquai(Convert.ToInt32(autoClient.ValueSkindanhquai.ToString()), quai.ToString());

                                                        MessageBox.Show(autoClient.FindBossTrain(quai).ToString());

                                                       if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing && autoClient.FindBossTrain(quai).ToString() != "True")
                                                            continue;
                                                    }
                                                }

                                                if (autoClient._Danhsachquai.Count < 1)
                                                {
                                                    autoClient.StepTrain1 = 3;
                                                }
                                            }
                                            break;
                                        }
                                    case 3:
                                        {
                                            autoClient.MoveTo(Convert.ToInt32(autoClient.arraytoadoTrain02[0].ToString()), Convert.ToInt32(autoClient.arraytoadoTrain02[1].ToString()), autoClient.CurrentMapId);
                                            if (Convert.ToInt32(GameGeneral.Distance(Convert.ToInt32(autoClient.arraytoadoTrain02[0].ToString()), Convert.ToInt32(autoClient.arraytoadoTrain02[1].ToString()), autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.RefreshEntityList();

                                                autoClient._Danhsachquai = new List<string>(from entity in CurrentClient.EntityList where entity.EntityType == NPCType.Beast && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 500 orderby entity.EntityId select entity.EntityId + "/" + entity.EntityNameNoMark).ToList();

                                                foreach (var quai in autoClient._Danhsachquai)
                                                {
                                                    if (quai.ToString() != "" && quai.ToString() != null)
                                                    {
                                                        autoClient.Trainquai(Convert.ToInt32(autoClient.ValueSkindanhquai.ToString()), quai.ToString());

                                                        MessageBox.Show(autoClient.FindBossTrain(quai).ToString());


                                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing && autoClient.FindBossTrain(quai).ToString() != "True")
                                                            continue;
                                                    }
                                                }


                                                if (autoClient._Danhsachquai.Count < 1)
                                                {
                                                    autoClient.StepTrain1 = 4;
                                                }

                                            
                                            }
                                            break;
                                        }
                                    case 4:
                                        {
                                            autoClient.MoveTo(Convert.ToInt32(autoClient.arraytoadoTrain03[0].ToString()), Convert.ToInt32(autoClient.arraytoadoTrain03[1].ToString()), autoClient.CurrentMapId);
                                            if (Convert.ToInt32(GameGeneral.Distance(Convert.ToInt32(autoClient.arraytoadoTrain03[0].ToString()), Convert.ToInt32(autoClient.arraytoadoTrain03[1].ToString()), autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.RefreshEntityList();

                                                autoClient._Danhsachquai = new List<string>(from entity in CurrentClient.EntityList where entity.EntityType == NPCType.Beast && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 500 orderby entity.EntityId select entity.EntityId + "/" + entity.EntityNameNoMark).ToList();

                                                foreach (var quai in autoClient._Danhsachquai)
                                                {
                                                    if (quai.ToString() != "" && quai.ToString() != null)
                                                    {
                                                        autoClient.Trainquai(Convert.ToInt32(autoClient.ValueSkindanhquai.ToString()), quai.ToString());

                                                        MessageBox.Show(autoClient.FindBossTrain(quai).ToString());


                                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing && autoClient.FindBossTrain(quai).ToString() != "True")
                                                            continue;
                                                    }
                                                }

                                                if(autoClient._Danhsachquai.Count<1)
                                                {
                                                    autoClient.StepTrain1 = 5;
                                                }
                                            }
                                            break;
                                        }
                                    case 5:
                                        {
                                            autoClient.MoveTo(Convert.ToInt32(autoClient.arraytoadonv[0].ToString()), Convert.ToInt32(autoClient.arraytoadonv[1].ToString()), autoClient.CurrentMapId);

                                            if (Convert.ToInt32(GameGeneral.Distance(Convert.ToInt32(autoClient.arraytoadonv[0].ToString()), Convert.ToInt32(autoClient.arraytoadonv[1].ToString()), autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 100)
                                            {
                                                autoClient.RefreshEntityList();
                                                autoClient._Danhsachquai = new List<string>(from entity in CurrentClient.EntityList where entity.EntityType == NPCType.Beast && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 500 orderby entity.EntityId select entity.EntityId + "/" + entity.EntityNameNoMark).ToList();

                                                foreach (var quai in autoClient._Danhsachquai)
                                                {
                                                    if (quai.ToString() != "" && quai.ToString() != null)
                                                    {
                                                        autoClient.Trainquai(Convert.ToInt32(autoClient.ValueSkindanhquai.ToString()), quai.ToString());

                                                        MessageBox.Show(autoClient.FindBossTrain(quai).ToString());

                                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing && autoClient.FindBossTrain(quai).ToString() != "True")
                                                            continue;


                                                    }
                                                }

                                                if (autoClient._Danhsachquai.Count < 1)
                                                {
                                                    autoClient.StepTrain1 = 0;
                                                }

                                            }
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Auto Train: " + ex.Message);
                    return;
                }
            }
        }
        #endregion

        // Thủ tục kill boss vuot ai
        #region Thủ tục kill boss vuot ai
        private void AutoKillBossvuotai()
        {
            while (true)
            {
                Thread.Sleep(300);

                try
                {
                    if (_clients.Count < 1)
                        continue;

                    var _clientskillsbossvuotai = new Dictionary<IntPtr, AutoClientBS>(_clients);

                    foreach (var autoClient in _clientskillsbossvuotai.Values)
                    {
                        if (autoClient.CkKillBossvuotai == true && autoClient.ischecked == true)
                        {

                            if (!autoClient.isInjected)
                                autoClient.Inject();


                            if (autoClient.ValueBossKillsvuotai != "" && autoClient.ValueBossKillsvuotai != null && autoClient.FindBoss(autoClient.ValueBossKillsvuotai).ToString() == "True"&& autoClient.CkKillBossvuotai == true && autoClient.ischecked == true)
                            {
                                switch (autoClient.StepKillBossvuotai)
                                {
                                    case 1:
                                        {
                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq)
                                            {
                                                autoClient.SelfBuffSkill(GameConst.ThatbaoSKILL);

                                                Thread.Sleep(150);

                                                if (autoClient.CurrentPlayer.TukhiTLQ < autoClient.CurrentPlayer.TukhiTLQMax)
                                                    continue;

                                                autoClient.StepKillBossvuotai = 2;
                                            
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkinBuff01.ToString() != "" && autoClient.ValueSkinBuff01.ToString() != null)
                                            {
                                                autoClient.SelfBuffSkill(Convert.ToInt32(autoClient.ValueSkinBuff01));

                                                Thread.Sleep(1000);

                                                autoClient.StepKillBossvuotai = 2;
                                            
                                            }
                                            break;
                                        }
                                    case 2:
                                        {

                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq && autoClient.ValueSkinBuff01.ToString() != "" && autoClient.ValueSkinBuff01.ToString() != null)
                                            {

                                                autoClient.SelfBuffSkill(Convert.ToInt32(autoClient.ValueSkinBuff01));

                                                Thread.Sleep(1000);

                                                autoClient.StepKillBossvuotai = 3;
                                            
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkinBuff02.ToString() != "" && autoClient.ValueSkinBuff02.ToString() != null)
                                            {
                                                autoClient.SelfBuffSkill(Convert.ToInt32(autoClient.ValueSkinBuff02));

                                                Thread.Sleep(1000);

                                                autoClient.StepKillBossvuotai = 3;
                                            
                                            }
                                            break;
                                        }
                                    case 3:
                                        {
                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.IsOnHorse == true)
                                                autoClient.XuongNgua(6);

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq && autoClient.ValueSkinBuff02.ToString() != "" && autoClient.ValueSkinBuff02.ToString() != null)
                                            {

                                                autoClient.SelfBuffSkill(Convert.ToInt32(autoClient.ValueSkinBuff02));

                                                Thread.Sleep(1000);

                                                autoClient.StepKillBossvuotai = 4;
                                            
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {
                                                autoClient.Trainquaivuotai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKillsvuotai.ToString());

                                                Thread.Sleep(50);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBossvuotai = 4;
                                            
                                            }
                                            break;
                                        }
                                    case 4:
                                        {
                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {

                                                autoClient.Trainquaivuotai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKillsvuotai.ToString());

                                                Thread.Sleep(200);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBossvuotai = 5;
                                            
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {
                                                autoClient.Trainquaivuotai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKillsvuotai.ToString());

                                                Thread.Sleep(50);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBossvuotai = 5;
                                            
                                            }
                                            break;
                                        }
                                    case 5:
                                        {
                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {

                                                autoClient.Trainquaivuotai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKillsvuotai.ToString());

                                                Thread.Sleep(200);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBossvuotai = 6;
                                            
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {
                                                autoClient.Trainquaivuotai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKillsvuotai.ToString());

                                                Thread.Sleep(50);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBossvuotai = 6;
                           
                                            }
                                            break;
                                        }
                                    case 6:
                                        {
                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {

                                                autoClient.Trainquaivuotai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKillsvuotai.ToString());

                                                Thread.Sleep(200);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBossvuotai = 7;
                                            
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {
                                                autoClient.Trainquaivuotai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKillsvuotai.ToString());

                                                Thread.Sleep(50);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBossvuotai = 7;
                          
                                            }
                                            break;
                                        }
                                    case 7:
                                        {
                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {

                                                autoClient.Trainquaivuotai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKillsvuotai.ToString());

                                                Thread.Sleep(200);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBossvuotai = 8;
                                
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {
                                                autoClient.Trainquaivuotai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKillsvuotai.ToString());

                                                Thread.Sleep(50);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBossvuotai = 8;
                                            
                                            }
                                            break;
                                        }
                                    case 8:
                                        {
                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {

                                                autoClient.Trainquaivuotai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKillsvuotai.ToString());

                                                Thread.Sleep(200);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBossvuotai = 9;
                                            
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {
                                                autoClient.Trainquaivuotai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKillsvuotai.ToString());

                                                Thread.Sleep(50);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBossvuotai = 9;
                                            
                                            }
                                            break;
                                        }
                                    case 9:
                                        {
                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {

                                                autoClient.Trainquaivuotai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKillsvuotai.ToString());

                                                Thread.Sleep(200);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing || autoClient.CurrentPlayer.TukhiTLQ > 1)
                                                    continue;

                                                autoClient.StepKillBossvuotai = 1;
                                            
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {
                                                autoClient.Trainquaivuotai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKillsvuotai.ToString());

                                                Thread.Sleep(50);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing || autoClient.FindBoss(autoClient.ValueBossKillsvuotai).ToString() == "True")
                                                    continue;

                                                autoClient.StepKillBossvuotai = 1;
                                            
                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            int stepKillBoss = autoClient.StepKillBossvuotai;
                                            autoClient.StepKillBossvuotai = 1;

                                            break;
                                        }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Auto kill boss vuot ai: " + ex.Message);
                    return;
                }
            }
        }
        #endregion

        // Thủ tục kill boss
        #region Thủ tục kill boss

        private void AutoKillBoss()
        {
            while (true)
            {
                Thread.Sleep(300);

                try
                {
                    if (_clients.Count < 1)
                        continue;

                    var _clientsKillBoss = new Dictionary<IntPtr, AutoClientBS>(_clients);

                    foreach (var autoClient in _clientsKillBoss.Values)
                    {
                        if (autoClient.CkKillBoss == true && autoClient.ischecked == true)
                        {

                            if (!autoClient.isInjected)
                                autoClient.Inject();


                            if (autoClient.ValueBossKills != "" && autoClient.ValueBossKills != null && autoClient.FindBoss(autoClient.ValueBossKills).ToString()=="True" && autoClient.CkKillBoss == true && autoClient.ischecked == true)
                            {
                                switch (autoClient.StepKillBoss)
                                {
                                    case 1:
                                        {
                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq)
                                            {
                                                autoClient.SelfBuffSkill(GameConst.ThatbaoSKILL);
                                                Thread.Sleep(150);

                                                if (autoClient.CurrentPlayer.TukhiTLQ != autoClient.CurrentPlayer.TukhiTLQMax)
                                                    continue;

                                                autoClient.StepKillBoss = 2;
                                            
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkinBuff01.ToString() != "" && autoClient.ValueSkinBuff01.ToString() != null)
                                            {
                                                autoClient.SelfBuffSkill(Convert.ToInt32(autoClient.ValueSkinBuff01));
                                                Thread.Sleep(100);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBoss = 2;
                                            
                                            }
                                            break;
                                        }
                                    case 2:
                                        {

                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq && autoClient.ValueSkinBuff01.ToString() != "" && autoClient.ValueSkinBuff01.ToString() != null)
                                            {

                                                autoClient.SelfBuffSkill(Convert.ToInt32(autoClient.ValueSkinBuff01));
                                                Thread.Sleep(150);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBoss = 3;
                                            
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkinBuff02.ToString() != "" && autoClient.ValueSkinBuff02.ToString() != null)
                                            {
                                                autoClient.SelfBuffSkill(Convert.ToInt32(autoClient.ValueSkinBuff02));
                                                Thread.Sleep(150);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBoss = 3;
                                            
                                            }
                                            break;
                                        }
                                    case 3:
                                        {
                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.IsOnHorse == true)
                                                autoClient.XuongNgua(6);

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq && autoClient.ValueSkinBuff02.ToString() != "" && autoClient.ValueSkinBuff02.ToString() != null)
                                            {

                                                autoClient.SelfBuffSkill(Convert.ToInt32(autoClient.ValueSkinBuff02));
                                                Thread.Sleep(150);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBoss = 4;
                                            
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {
                                                autoClient.Trainquai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKills.ToString());
                                                Thread.Sleep(100);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBoss = 4;
                                            
                                            }
                                            break;
                                        }
                                    case 4:
                                        {
                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {

                                                autoClient.Trainquai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKills.ToString());
                                                Thread.Sleep(150);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBoss = 5;
                                            
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {
                                                autoClient.Trainquai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKills.ToString());
                                                Thread.Sleep(100);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBoss = 5;
                                            
                                            }
                                            break;
                                        }
                                    case 5:
                                        {
                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {

                                                autoClient.Trainquai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKills.ToString());
                                                Thread.Sleep(150);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBoss = 6;
                                            
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {
                                                autoClient.Trainquai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKills.ToString());
                                                Thread.Sleep(100);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBoss = 6;
                                            
                                            }
                                            break;
                                        }

                                    case 6:
                                        {
                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {

                                                autoClient.Trainquai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKills.ToString());
                                                Thread.Sleep(150);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBoss = 7;
                                            
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {
                                                autoClient.Trainquai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKills.ToString());
                                                Thread.Sleep(100);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBoss = 7;
                                            
                                            }
                                            break;
                                        }
                                    case 7:
                                        {
                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {

                                                autoClient.Trainquai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKills.ToString());
                                                Thread.Sleep(150);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBoss = 8;
                                            
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {
                                                autoClient.Trainquai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKills.ToString());
                                                Thread.Sleep(100);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                                    continue;

                                                autoClient.StepKillBoss = 8;
                                            
                                            }
                                            break;
                                        }
                                    case 9:
                                        {
                                            if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.Death)
                                            {
                                                autoClient.VeThanh(3);
                                            }

                                            if (autoClient.CurrentPlayer.Hephai == (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {

                                                autoClient.Trainquai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKills.ToString());
                                                Thread.Sleep(150);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing || autoClient.CurrentPlayer.TukhiTLQ > 1)
                                                    continue;

                                                autoClient.StepKillBoss = 1;
                                            
                                            }
                                            else if (autoClient.CurrentPlayer.Hephai != (int)HePhai.Tlq && autoClient.ValueSkindanhquai.ToString() != "" && autoClient.ValueSkindanhquai.ToString() != null)
                                            {
                                                autoClient.Trainquai(Convert.ToInt32(autoClient.ValueSkindanhquai), autoClient.ValueBossKills.ToString());
                                                Thread.Sleep(100);

                                                if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing || autoClient.FindBoss(autoClient.ValueBossKills).ToString() == "True")
                                                    continue;

                                                autoClient.StepKillBoss = 1;
                                            
                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            int stepKillBoss = autoClient.StepKillBoss;
                                            autoClient.StepKillBoss = 1;

                                            break;
                                        }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Auto Kill Boss: " + ex.Message);
                    return;
                }
            }
        }
        #endregion

        // Thủ tục checkbox Auto Thái nhất tháp
        private void Ckbthainhatthap_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.CkThainhatthap = Ckbthainhatthap.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTNT", "AutoTNT.Check1", CurrentClient.CkThainhatthap.ToString());
            }
        }

        
        //Thủ tục auto Thái nhất tháp
        #region Thủ tục auto Thái nhất tháp

        private void AutoTNT()
        {
            while (true)
            {
                Thread.Sleep(500);
                try
                {

                    if (_clients.Count < 1)
                        continue;

                    var _clientsTNT = new Dictionary<IntPtr, AutoClientBS>(_clients);

                    foreach (var autoClient in _clientsTNT.Values)
                    {
                        if (!autoClient.CkThainhatthap || !autoClient.ischecked)
                            continue;

                        if (!autoClient.isInjected)
                            autoClient.Inject();

                        if (autoClient.CkThainhatthap == true && autoClient.ischecked == true && autoClient.CkVuotaiTKL == false && autoClient.Ckacctreo == false)
                        {
                            //////////////////////////// Thủ tục lấy giá trị mặc định của ải đang thực hiện /////////////////////////////////////////

                            switch (autoClient.CkChukey)
                            {
                                //////////////////////////// Nhân vật chủ key /////////////////////////////////////////
                                case true:
                                    {
                                        if (autoClient.CurrentMapId == 350)
                                        {
                                            int aiphoban = autoClient.Aiphoban;
                                            autoClient.Aiphoban = 33;
                                        }
                                        else if (autoClient.CurrentMapId == 6075 && Convert.ToInt32(GameGeneral.Distance(42509, 90929, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 1100)//
                                        {
                                            int aiphoban = autoClient.Aiphoban;
                                            autoClient.Aiphoban = 1;
                                        }
                                        else if (autoClient.CurrentMapId == 6075 && Convert.ToInt32(GameGeneral.Distance(46653, 92341, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 1400)//
                                        {
                                            if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindNPC("Ngoc Tu La Tieu Dich").ToString() == "False" && autoClient.FindNPC("Ruong Bac").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 2;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindNPC("Ngoc Tu La Tieu Dich").ToString() == "True" && autoClient.FindNPC("Ruong Bac").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 3;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindNPC("Ngoc Tu La Tieu Dich").ToString() == "False" && autoClient.FindNPC("Ruong Bac").ToString() == "True")
                                            {

                                                autoClient.Aiphoban = 4;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && autoClient.FindBoss("Ngoc Tu La Tieu Dich").ToString() == "True" && autoClient.FindNPC("Ruong Bac").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 3;
                                            }
                                        }
                                        else if (autoClient.CurrentMapId == 6075 && Convert.ToInt32(GameGeneral.Distance(52205, 92435, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 1600)//
                                        {
                                            if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindBoss("Cuu Long Thon Van Thu").ToString() == "False" && autoClient.FindNPC("Ruong Bac").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 5;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && autoClient.FindBoss("Cuu Long Thon Van Thu").ToString() == "True" && autoClient.FindNPC("Ruong Bac").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 6;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindBoss("Cuu Long Thon Van Thu").ToString() == "False" && autoClient.FindNPC("Ruong Bac").ToString() == "True")
                                            {

                                                autoClient.Aiphoban = 7;
                                            }
                                        }
                                        else if (autoClient.CurrentMapId == 6075 && Convert.ToInt32(GameGeneral.Distance(51568, 102559, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 1700)//
                                        {
                                            if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindBoss("Duong Ton Bao").ToString() == "False" && autoClient.FindNPC("Kim Ruong").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 8;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && autoClient.FindBoss("Duong Ton Bao").ToString() == "True" && autoClient.FindNPC("Kim Ruong").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 9;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindBoss("Duong Ton Bao").ToString() == "False" && autoClient.FindNPC("Kim Ruong").ToString() == "True")
                                            {

                                                autoClient.Aiphoban = 10;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && autoClient.FindBoss("Moc Que Anh").ToString() == "True" && autoClient.FindNPC("Kim Ruong").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 9;
                                            }

                                        }
                                        else if (autoClient.CurrentMapId == 6075 && Convert.ToInt32(GameGeneral.Distance(39410, 110724, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 1700)//
                                        {
                                            if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindBoss("Than Thu Tich Ta").ToString() == "False" && autoClient.FindNPC("Ruong Bac").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 11;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && autoClient.FindBoss("Than Thu Tich Ta").ToString() == "True" && autoClient.FindNPC("Ruong Bac").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 12;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindBoss("Than Thu Tich Ta").ToString() == "False" && autoClient.FindNPC("Ruong Bac").ToString() == "True")
                                            {

                                                autoClient.Aiphoban = 13;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && autoClient.FindBoss("Than Thu Tich Ta").ToString() == "False" && autoClient.FindNPC("Ruong Bac").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 13;
                                            }
                                        }
                                        else if (autoClient.CurrentMapId == 6075 && Convert.ToInt32(GameGeneral.Distance(46434, 111855, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 1900)//
                                        {
                                            if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindBoss("Hoang Hoa Luu Kim").ToString() == "False" && autoClient.FindNPC("Ruong Bac").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 14;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && autoClient.FindBoss("Hoang Hoa Luu Kim").ToString() == "True" && autoClient.FindNPC("Ruong Bac").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 15;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindBoss("Hoang Hoa Luu Kim").ToString() == "False" && autoClient.FindNPC("Ruong Bac").ToString() == "True")
                                            {

                                                autoClient.Aiphoban = 16;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && autoClient.FindBoss("Hoang Hoa Luu Kim").ToString() == "False" && autoClient.FindNPC("Ruong Bac").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 14;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && autoClient.FindBoss("Hoang Hoa Luu Kim").ToString() == "False" && autoClient.FindNPC("Ruong Bac").ToString() == "True")
                                            {

                                                autoClient.Aiphoban = 16;
                                            }
                                        }
                                        else if (autoClient.CurrentMapId == 6075 && Convert.ToInt32(GameGeneral.Distance(52581, 112166, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 2100)//
                                        {
                                            if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindNPC("Gia Luat Phi Hong").ToString() == "False" && autoClient.FindNPC("Kim Ruong").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 17;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindNPC("Gia Luat Phi Hong").ToString() == "True" && autoClient.FindNPC("Kim Ruong").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 18;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindNPC("Gia Luat Phi Hong").ToString() == "True" && autoClient.FindNPC("Kim Ruong").ToString() == "True")
                                            {

                                                autoClient.Aiphoban = 19;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && autoClient.FindBoss("Gia Luat Phi Hong").ToString() == "True" && autoClient.FindNPC("Kim Ruong").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 18;
                                            }
                                        }
                                        else if (autoClient.CurrentMapId == 6075 && Convert.ToInt32(GameGeneral.Distance(45540, 100228, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 800)
                                        {
                                            int aiphoban = autoClient.Aiphoban;
                                            autoClient.Aiphoban = 32;
                                        }

                                        break;
                                    }

                                //////////////////////////// Nhân vật thành viên tổ đội /////////////////////////////////////////
                                case false:
                                    {
                                        if (autoClient.CurrentMapId == 6075 && Convert.ToInt32(GameGeneral.Distance(46653, 92341, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 1400)//
                                        {
                                            if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindBoss("Ngoc Tu La Tieu Dich").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 20;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && autoClient.FindBoss("Ngoc Tu La Tieu Dich").ToString() == "True")
                                            {

                                                autoClient.Aiphoban = 21;
                                            }
                                        }
                                        else if (autoClient.CurrentMapId == 6075 && Convert.ToInt32(GameGeneral.Distance(52205, 92435, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 1600)//
                                        {
                                            if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindBoss("Cuu Long Thon Van Thu").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 22;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && autoClient.FindBoss("Cuu Long Thon Van Thu").ToString() == "True")
                                            {

                                                autoClient.Aiphoban = 23;
                                            }
                                        }
                                        else if (autoClient.CurrentMapId == 6075 && Convert.ToInt32(GameGeneral.Distance(51568, 102559, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 1700)//
                                        {
                                            if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindBoss("Duong Ton Bao").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 24;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && autoClient.FindBoss("Duong Ton Bao").ToString() == "True")
                                            {

                                                autoClient.Aiphoban = 25;
                                            }
                                        }
                                        else if (autoClient.CurrentMapId == 6075 && Convert.ToInt32(GameGeneral.Distance(39410, 110724, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 1700)//
                                        {
                                            if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindBoss("Than Thu Tich Ta").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 26;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && autoClient.FindBoss("Than Thu Tich Ta").ToString() == "True")
                                            {

                                                autoClient.Aiphoban = 27;
                                            }
                                        }
                                        else if (autoClient.CurrentMapId == 6075 && Convert.ToInt32(GameGeneral.Distance(46434, 111855, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 1900)//
                                        {
                                            if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindBoss("Hoang Hoa Luu Kim").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 28;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && autoClient.FindBoss("Hoang Hoa Luu Kim").ToString() == "True")
                                            {

                                                autoClient.Aiphoban = 29;
                                            }
                                        }
                                        else if (autoClient.CurrentMapId == 6075 && Convert.ToInt32(GameGeneral.Distance(52581, 112166, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 2100)//
                                        {
                                            if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && autoClient.FindBoss("Gia Luat Phi Hong").ToString() == "False")
                                            {

                                                autoClient.Aiphoban = 30;
                                            }
                                            else if (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && autoClient.FindBoss("Gia Luat Phi Hong").ToString() == "True")
                                            {

                                                autoClient.Aiphoban = 31;
                                            }
                                        }
                                        else if (autoClient.CurrentMapId == 6075 && Convert.ToInt32(GameGeneral.Distance(45540, 100228, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) < 800)
                                        {
                                            int aiphoban = autoClient.Aiphoban;
                                            autoClient.Aiphoban = 32;
                                        }

                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }

                            //////////////////////////// Thủ tục auto đánh ải Thái Nhất Tháp /////////////////////////////////////////
                            switch (autoClient.Aiphoban)
                            {

                                //////////////////////////// Nhân vật chủ key /////////////////////////////////////////

                                // Bắt đầu ở map tương dương
                                case 33:
                                    {
                                        if (autoClient.CurrentMapId != 350)
                                            continue;

                                        if (autoClient.CkChukey == true)
                                        {
                                            autoClient.MoveTo(45154, 91514, 350);

                                            if (autoClient.FindNPC("Tien Phong Moc Que Anh").ToString() == "True" && Convert.ToInt32(GameGeneral.Distance(45154, 91514, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 300)
                                            {
                                                autoClient.ClickNPCvuotai("Tien Phong Moc Que Anh");
                                                Thread.Sleep(500);
                                                if (autoClient.FindMenu("Ta muon tham gia Phan Thien Lam-Anh Hung (Ton Tinh Luc*20)").ToString() == "True")
                                                {
                                                    autoClient.Chonmenu("Ta muon tham gia Phan Thien Lam-Anh Hung (Ton Tinh Luc*20)");
                                                    Thread.Sleep(1500);
                                                }
                                            }
                                        }

                                        break;
                                    }

                                // Bắt đầu nơi đăng ký vào ải
                                case 1:
                                    {
                                        if (autoClient.CurrentMapId != 6075)
                                            continue;

                                        autoClient.ShortMove(42328, 90862);

                                        if (autoClient.FindNPC("Moc Que Anh").ToString() == "True" && Convert.ToInt32(GameGeneral.Distance(42328, 90862, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 300)
                                        {
                                            autoClient.ClickNPCvuotai("Moc Que Anh");
                                            Thread.Sleep(500);

                                            if (autoClient.FindMenu("Vao ai 1: Nhien Tich Chi Thuong").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Vao ai 1: Nhien Tich Chi Thuong");
                                                Thread.Sleep(1500);
                                            }

                                        }
                                        break;
                                    }

                                // Bắt đầu đánh ải 1
                                case 2:
                                    {
                                        if (autoClient.FindNPC("Moc Que Anh").ToString() == "True")
                                        {
                                            autoClient.ClickNPCvuotai("Moc Que Anh");
                                            Thread.Sleep(500);

                                            if (autoClient.FindMenu("Mo ai").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Mo ai");
                                                Thread.Sleep(500);
                                                break;
                                            }

                                            if (autoClient.FindMenu("Mo ai 2").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Mo ai 2");
                                                Thread.Sleep(500);
                                                break;
                                            }
                                        }
                                        break;

                                    }

                                case 3:
                                    {
                                        if(autoClient.FindNPC("Ngoc Tu La Tieu Dich").ToString() == "True")
                                        {
                                            while ((autoClient.FindNPC("Ngoc Tu La Tieu Dich").ToString() == "True" && Convert.ToInt32(GameGeneral.Distance(46693, 92126, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 500))
                                            {
                                                autoClient.SelfBuffSkill(0);
                                                Thread.Sleep(200);
                                                autoClient.ShortMove(46693, 92126);
                                                Thread.Sleep(600);
                                            }
                                            
                                        }

                                        if (autoClient.FindBoss("Ngoc Tu La Tieu Dich").ToString() == "True" && autoClient.FindBoss("Tu La Toai anh").ToString() == "False")
                                        {
                                            autoClient.ValueBossKillsvuotai = "Ngoc Tu La Tieu Dich";
                                            autoClient.CkKillBossvuotai = true;
                                            Thread.Sleep(100);
                                        }
                                        else if (autoClient.FindBoss("Tu La Toai anh").ToString() == "True" && autoClient.FindBoss("Tu La Toai anh").ToString() == "True")
                                        {
                                            autoClient.ValueBossKillsvuotai = "Tu La Toai anh";
                                            autoClient.CkKillBossvuotai = true;
                                            Thread.Sleep(100);
                                        }
                                        else if (autoClient.FindBoss("Ngoc Tu La Tieu Dich").ToString() == "False" && autoClient.FindNPC("Ruong Bac").ToString() == "True")
                                        {
                                            autoClient.ValueBossKillsvuotai = "";
                                            autoClient.CkKillBossvuotai = true;
                                        }
                                        break;
                                    }

                                case 4:
                                    {
                                        Thread.Sleep(1000);
                                        if(autoClient.FindNPC("Ruong Dong").ToString() == "True")
                                        {
                                            autoClient.ClickNPCvuotai("Ruong Dong");
                                            Thread.Sleep(200);
                                        }

                                        if (autoClient.FindNPC("Moc Que Anh").ToString() == "True")
                                        {
                                            autoClient.ClickNPCvuotai("Moc Que Anh");
                                            Thread.Sleep(500);

                                            if (autoClient.FindMenu("Mo ai").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Mo ai");
                                                Thread.Sleep(1500);
                                                break;
                                            }

                                            if (autoClient.FindMenu("Mo ai 2").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Mo ai 2");
                                                Thread.Sleep(1500);
                                                break;
                                            }
                                        }

                                        break;
                                    }

                                // Bắt đầu đánh ải 3
                                case 5:
                                    {
                                        if (autoClient.FindNPC("Moc Que Anh").ToString() == "True")
                                        {
                                            autoClient.ClickNPCvuotai("Moc Que Anh");
                                            Thread.Sleep(500);

                                            if (autoClient.FindMenu("Mo ai 2").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Mo ai 2");
                                                Thread.Sleep(500);
                                            }

                                            if (autoClient.FindMenu("Mo ai 3").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Mo ai 3");
                                                Thread.Sleep(500);
                                            }
                                        }
                                        else
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(200);
                                        }

                                        break;

                                    }

                                case 6:
                                    {
                                        while ((autoClient.FindBoss("Cuu Long Thon Van Thu").ToString() == "True" && autoClient.FindNPC("Ruong Bac").ToString() == "False" && Convert.ToInt32(GameGeneral.Distance(52304, 92578, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 500))
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(200);
                                            autoClient.ShortMove(52304, 92578);
                                            Thread.Sleep(600);
                                        }
                                        

                                        if (autoClient.FindBoss("Cuu Long Thon Van Thu").ToString() == "True" && autoClient.FindNPC("Ruong Bac").ToString() == "False")
                                        {
                                            autoClient.ValueBossKillsvuotai = "Cuu Long Thon Van Thu";
                                            autoClient.CkKillBossvuotai = true;
                                            Thread.Sleep(500);
                                        }
                                        else if (autoClient.FindBoss("Cuu Long Thon Van Thu").ToString() == "False" && autoClient.FindNPC("Ruong Bac").ToString() == "True")
                                        {
                                            autoClient.ValueBossKillsvuotai = "";
                                            autoClient.CkKillBossvuotai = true;

                                            Thread.Sleep(500);
                                        }

                                        if (autoClient.FindNPC("Cot Thu Loi").ToString() == "True")
                                        {
                                            autoClient.ClickNPCvuotai("Cot Thu Loi");
                                        }

                                        break;
                                    }

                                case 7:
                                    {
                                        Thread.Sleep(1000);
                                        if (autoClient.FindNPC("Ruong Dong").ToString() == "True")
                                        {
                                            autoClient.ClickNPCvuotai("Ruong Dong");
                                            Thread.Sleep(200);
                                            break;
                                        }
                                        if (autoClient.FindNPC("Moc Que Anh").ToString() == "True")
                                        {
                                            autoClient.ClickNPCvuotai("Moc Que Anh");
                                            Thread.Sleep(500);

                                            if (autoClient.FindMenu("Mo ai 2").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Mo ai 2");
                                                Thread.Sleep(1500);
                                                break;
                                            }

                                            if (autoClient.FindMenu("Mo ai 3").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Mo ai 3");
                                                Thread.Sleep(1500);
                                                break;
                                            }

                                        }

                                        break;
                                    }


                                // Bắt đầu đánh ải 4
                                case 8:
                                    {
                                        if (autoClient.FindNPC("Moc Que Anh").ToString() == "True")
                                        {
                                            autoClient.ClickNPCvuotai("Moc Que Anh");
                                            Thread.Sleep(100);

                                            if (autoClient.FindMenu("Mo ai").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Mo ai");
                                                Thread.Sleep(500);
                                            }

                                             if (autoClient.FindMenu("Toi muon tham gia Thai Nhat Thap").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Toi muon tham gia Thai Nhat Thap");
                                                Thread.Sleep(500);
                                            }
                                        
                                        }

                                        break;
                                    }

                                case 9:
                                    {
                                        while ((autoClient.FindBoss("Moc Que Anh").ToString() == "True" && Convert.ToInt32(GameGeneral.Distance(51669, 101775, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 500))
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(200);
                                            autoClient.ShortMove(51669, 101775);
                                            Thread.Sleep(1000);
                                        }
                                        
                                        if (autoClient.FindBoss("Duong Ton Bao").ToString() == "True" && autoClient.FindNPC("Kim Ruong").ToString() == "False")
                                        {
                                            autoClient.ValueBossKillsvuotai = "Duong Ton Bao";
                                            autoClient.CkKillBossvuotai = true;

                                            Thread.Sleep(500);
                                        }
                                        else if (autoClient.FindBoss("Duong Ton Bao").ToString() == "False" && autoClient.FindNPC("Kim Ruong").ToString() == "True")
                                        {
                                            autoClient.ValueBossKillsvuotai = "";
                                            autoClient.CkKillBossvuotai = true;

                                            Thread.Sleep(500);
                                        }
                                        break;

                                    }

                                case 10:
                                    {
                                        Thread.Sleep(2000);
                                        autoClient.ClickNPCvuotai("Kim Ruong");
                                        Thread.Sleep(200);

                                        if (autoClient.FindNPC("Moc Que Anh").ToString() == "True")
                                        {
                                            autoClient.ClickNPCvuotai("Moc Que Anh");
                                            Thread.Sleep(100);

                                            if (autoClient.FindMenu("Mo ai").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Mo ai");
                                                Thread.Sleep(1500);
                                                break;
                                            }

                                            if (autoClient.FindMenu("Toi muon tham gia Thai Nhat Thap").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Toi muon tham gia Thai Nhat Thap");
                                                Thread.Sleep(1500);
                                                break;
                                            }
                                        }

                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                            continue;

                                        break;
                                    }

                                // Bắt đầu đánh ải 5
                                case 11:
                                    {
                                        if (autoClient.FindNPC("Moc Que Anh").ToString() == "True")
                                        {
                                            autoClient.ClickNPCvuotai("Moc Que Anh");
                                            Thread.Sleep(100);
                                            if (autoClient.FindMenu("Mo ai 1: Tich Ta Chi Hi").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Mo ai 1: Tich Ta Chi Hi");
                                                Thread.Sleep(500);
                                            }

                                            if (autoClient.FindMenu("Truyen tong den Thai Nhat Thap-Trung").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Truyen tong den Thai Nhat Thap-Trung");
                                                Thread.Sleep(500);
                                            }
                                        }

                                        break;
                                    }
                                case 12:
                                    {
                                        if (autoClient.FindBoss("Than Thu Tich Ta").ToString() == "True" && autoClient.FindNPC("Ruong Bac").ToString() == "False")
                                        {
                                            autoClient.SelfBuffSkill(0);

                                            autoClient.ValueBossKillsvuotai = "Than Thu Tich Ta";
                                            autoClient.CkKillBossvuotai = true;
                                            Thread.Sleep(500);
                                        }
                                        else if (autoClient.FindBoss("Than Thu Tich Ta").ToString() == "False" && autoClient.FindNPC("Ruong Bac").ToString() == "True")
                                        {

                                            autoClient.ValueBossKillsvuotai = "";
                                            autoClient.CkKillBossvuotai = true;
                                        }
                                        break;
                                    }
                                case 13:
                                    {
                                        do
                                        {
                                            autoClient.ShortMove(39770, 110343);
                                            Thread.Sleep(600);
                                        }
                                        while (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && Convert.ToInt32(GameGeneral.Distance(39770, 110343, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 200);


                                        if (autoClient.FindNPC("Ruong Bac").ToString() == "True" && Convert.ToInt32(GameGeneral.Distance(39770, 110343, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) <= 200)
                                        {
                                            Thread.Sleep(1000);
                                            autoClient.ClickNPCvuotai("Ruong Dong");
                                            Thread.Sleep(200);

                                            if (autoClient.FindNPC("Moc Que Anh").ToString() == "True")
                                            {
                                                autoClient.ClickNPCvuotai("Moc Que Anh");
                                                Thread.Sleep(100);
                                                if (autoClient.FindMenu("Mo ai 1: Tich Ta Chi Hi").ToString() == "True")
                                                {
                                                    autoClient.Chonmenu("Mo ai 1: Tich Ta Chi Hi");
                                                    Thread.Sleep(1500);
                                                    break;
                                                }

                                                if (autoClient.FindMenu("Truyen tong den Thai Nhat Thap-Trung").ToString() == "True")
                                                {
                                                    autoClient.Chonmenu("Truyen tong den Thai Nhat Thap-Trung");
                                                    Thread.Sleep(1500);
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                // Bắt đầu đánh ải 6
                                case 14:
                                    {
                                        while (autoClient.FindNPC("Moc Que Anh").ToString() == "False" && Convert.ToInt32(GameGeneral.Distance(46274, 112914, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 300)
                                        {
                                            autoClient.ShortMove(46274, 112914);
                                            Thread.Sleep(600);
                                        }

                                        if (autoClient.FindNPC("Moc Que Anh").ToString() == "True")
                                        {
                                            autoClient.ClickNPCvuotai("Moc Que Anh");
                                            Thread.Sleep(100);

                                            if (autoClient.FindMenu("Mo ai 2: Nghiep Nhan").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Mo ai 2: Nghiep Nhan");
                                                Thread.Sleep(500);
                                            }

                                            if (autoClient.FindMenu("Truyen tong den Thai Nhat Thap-Dinh").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Truyen tong den Thai Nhat Thap-Dinh");
                                                Thread.Sleep(500);
                                            }
                                        }

                                        break;
                                    }

                                case 15:
                                    {
                                        while ((autoClient.FindBoss("Hoang Hoa Luu Kim").ToString() == "True" && autoClient.FindBoss("Hoang Hoa Luu Kim Dao").ToString() == "False" && Convert.ToInt32(GameGeneral.Distance(46307, 112124, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 500))
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(200);
                                            autoClient.ShortMove(46307, 112124);
                                            Thread.Sleep(600);
                                        }
                                       

                                        if (autoClient.FindBoss("Hoang Hoa Luu Kim").ToString() == "True" && autoClient.FindBoss("Hoang Hoa Luu Kim Dao").ToString() == "False" && autoClient.FindNPC("Ruong Bac").ToString() == "False" )
                                        {
                                            autoClient.ValueBossKillsvuotai = "Hoang Hoa Luu Kim";
                                            autoClient.CkKillBossvuotai = true;
                                            Thread.Sleep(500);
                                            autoClient.FindAndUseItem("Ngung Bang Quyet");
                                        }
                                        else if (autoClient.FindBoss("Hoang Hoa Luu Kim").ToString() == "False" && autoClient.FindBoss("Hoang Hoa Luu Kim Dao").ToString() == "True" && autoClient.FindNPC("Ruong Bac").ToString() == "False")
                                        {
                                            autoClient.ValueBossKillsvuotai = "Hoang Hoa Luu Kim Dao";
                                            Thread.Sleep(500);
                                        }
                                        else if (autoClient.FindBoss("Hoang Hoa Luu Kim").ToString() == "False" && autoClient.FindBoss("Hoang Hoa Luu Kim Dao").ToString() == "False" && autoClient.FindNPC("Ruong Bac").ToString() == "True")
                                        {
                                            autoClient.ValueBossKillsvuotai = "";
                                            autoClient.CkKillBossvuotai = true;

                                        }
                                        break;
                                    }

                                case 16:
                                    {
                                        Thread.Sleep(1000);
                                        if (autoClient.FindNPC("Ruong Dong").ToString() == "True")
                                        {
                                            autoClient.ClickNPCvuotai("Ruong Dong");
                                            Thread.Sleep(200);
                                        }

                                        if (autoClient.FindNPC("Moc Que Anh").ToString() == "True")
                                        {
                                            autoClient.ClickNPCvuotai("Moc Que Anh");
                                            Thread.Sleep(100);

                                            if (autoClient.FindMenu("Mo ai 2: Nghiep Nhan").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Mo ai 2: Nghiep Nhan");
                                                Thread.Sleep(1500);
                                                break;
                                            }

                                            if (autoClient.FindMenu("Truyen tong den Thai Nhat Thap-Dinh").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Truyen tong den Thai Nhat Thap-Dinh");
                                                Thread.Sleep(1500);
                                                break;
                                            }

                                        }

                                        break;
                                    }
                                // Bắt đầu đánh ải 7
                                case 17:
                                    {
                                        if (autoClient.FindNPC("Moc Que Anh").ToString() == "True")
                                        {
                                            autoClient.ClickNPCvuotai("Moc Que Anh");
                                            Thread.Sleep(100);

                                            if (autoClient.FindMenu("Mo ai 3: Hang Long Huu Hoi").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Mo ai 3: Hang Long Huu Hoi");
                                                Thread.Sleep(500);
                                                break;
                                            }

                                            if (autoClient.FindMenu("Ta muon roi khoi ai").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Ta muon roi khoi ai");
                                                Thread.Sleep(500);
                                                break;
                                            }

                                        }

                                        break;
                                    }
                                case 18:
                                    {

                                        while ((autoClient.FindNPC("Gia Luat Phi Hong").ToString() == "True" && Convert.ToInt32(GameGeneral.Distance(53018, 111729, autoClient.CurrentPlayer.PositionX, autoClient.CurrentPlayer.PositionY)) > 300))
                                        {
                                            autoClient.SelfBuffSkill(0);
                                            Thread.Sleep(200);
                                            autoClient.ShortMove(53018, 111729);
                                            Thread.Sleep(600);
                                        }
                         
                                        if (autoClient.FindBoss("Gia Luat Phi Hong").ToString() == "True" && autoClient.FindNPC("Kim Ruong").ToString() == "False")
                                        {
                                            autoClient.ValueBossKillsvuotai = "Gia Luat Phi Hong";
                                            autoClient.CkKillBossvuotai = true;
                                            Thread.Sleep(500);
                                        }
                                        else if (autoClient.FindBoss("Gia Luat Phi Hong").ToString() == "False" && autoClient.FindNPC("Kim Ruong").ToString() == "True")
                                        {
                                            autoClient.ValueBossKillsvuotai = "";
                                            autoClient.CkKillBossvuotai = true;

                                        }
                                        break;
                                    }

                                case 19:
                                    {
                                        Thread.Sleep(1000);
                                        if (autoClient.FindNPC("Kim Ruong").ToString() == "True")
                                        {
                                            autoClient.ClickNPCvuotai("Kim Ruong");
                                            Thread.Sleep(200);
                                        }

                                        if (autoClient.FindNPC("Moc Que Anh").ToString() == "True")
                                        {
                                            autoClient.ClickNPCvuotai("Moc Que Anh");
                                            Thread.Sleep(100);

                                            if (autoClient.FindMenu("Mo ai 3: Hang Long Huu Hoi").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Mo ai 3: Hang Long Huu Hoi");
                                                Thread.Sleep(1500);
                                                break;
                                            }

                                            if (autoClient.FindMenu("Ta muon roi khoi ai").ToString() == "True")
                                            {
                                                autoClient.Chonmenu("Ta muon roi khoi ai");
                                                Thread.Sleep(1500);
                                                break;
                                            }

                                        }

                                        if (autoClient.CurrentPlayer.PlayerStatus == PlayerStatus.DoNothing)
                                            continue;

                                        Thread.Sleep(30000);
     
                                        break;
                                    }
                                //////////////////////////// Nhân vật thành viên tổ đội /////////////////////////////////////////

                                case 20:
                                    {
                                        autoClient.ShortMove(46693, 92126);

                                        break;
                                    }
                                case 21:
                                    {
                                        if (autoClient.FindBoss("Ngoc Tu La Tieu Dich").ToString() == "True" && autoClient.FindBoss("Tu La Toai anh").ToString() == "False")
                                        {
                                            autoClient.ValueBossKillsvuotai = "Ngoc Tu La Tieu Dich";
                                            autoClient.CkKillBossvuotai = true;
                                            Thread.Sleep(100);
                                        }
                                        else if (autoClient.FindBoss("Tu La Toai anh").ToString() == "True" && autoClient.FindBoss("Tu La Toai anh").ToString() == "True")
                                        {
                                            autoClient.ValueBossKillsvuotai = "Tu La Toai anh";
                                            autoClient.CkKillBossvuotai = true;

                                            Thread.Sleep(100);
                                        }
                                        else if (autoClient.FindBoss("Ruong Bac").ToString() == "True" && autoClient.FindBoss("Ngoc Tu La Tieu Dich").ToString() == "False")
                                        {
                                            autoClient.ValueBossKillsvuotai = "";
                                            autoClient.CkKillBossvuotai = true;

                                        }
                                        break;
                                    }
                                // Bắt đầu đánh ải 3
                                case 22:
                                    {
                                        autoClient.ShortMove(52304, 92578);

                                        break;
                                    }

                                case 23:
                                    {
                                        if (autoClient.FindBoss("Cuu Long Thon Van Thu").ToString() == "True")
                                        {
                                            autoClient.ValueBossKillsvuotai = "Cuu Long Thon Van Thu";
                                            autoClient.CkKillBossvuotai = true;
                                        }
                                        else if (autoClient.FindBoss("Cuu Long Thon Van Thu").ToString() == "False")
                                        {
                                            autoClient.ValueBossKillsvuotai = "";
                                            autoClient.CkKillBossvuotai = true;

                                            Thread.Sleep(500);
                                        }

                                        break;
                                    }
                                // Bắt đầu đánh ải 4
                                case 24:
                                    {
                                        autoClient.ShortMove(51712, 101376);

                                        break;
                                    }
                                case 25:
                                    {
                                        if (autoClient.FindBoss("Duong Ton Bao").ToString() == "True")
                                        {
                                            autoClient.ValueBossKillsvuotai = "Duong Ton Bao";
                                            autoClient.CkKillBossvuotai = true;
                                            Thread.Sleep(500);

                                        }
                                        else if (autoClient.FindBoss("Duong Ton Bao").ToString() == "False")
                                        {
                                            autoClient.ValueBossKillsvuotai = "";
                                            autoClient.CkKillBossvuotai = true;

                                            Thread.Sleep(500);

                                        }

                                        break;
                                    }
                                // Bắt đầu đánh ải 5
                                case 26:
                                    {

                                        autoClient.ShortMove(39424, 110592);

                                        break;
                                    }
                                case 27:
                                    {
                                        if (autoClient.FindBoss("Than Thu Tich Ta").ToString() == "True")
                                        {
                                            autoClient.ValueBossKillsvuotai = "Than Thu Tich Ta";
                                            autoClient.CkKillBossvuotai = true;
                                            Thread.Sleep(500);
                                        }
                                        else if (autoClient.FindBoss("Than Thu Tich Ta").ToString() == "False")
                                        {
                                            autoClient.ValueBossKillsvuotai = "";
                                            autoClient.CkKillBossvuotai = true;

                                            Thread.Sleep(500);
                                        }
                                        break;
                                    }
                                // Bắt đầu đánh ải 6
                                case 28:
                                    {
                                        autoClient.ShortMove(46336, 111616);

                                        break;
                                    }
                                case 29:
                                    {
                                        if (autoClient.FindBoss("Hoang Hoa Luu Kim").ToString() == "True" && autoClient.FindBoss("Hoang Hoa Luu Kim Dao").ToString() == "False")
                                        {
                                            autoClient.ValueBossKillsvuotai = "Hoang Hoa Luu Kim";
                                            autoClient.CkKillBossvuotai = true;
                                            Thread.Sleep(500);
                                            autoClient.FindAndUseItem("Ngung Bang Quyet");
                                        }
                                        else if (autoClient.FindBoss("Hoang Hoa Luu Kim").ToString() == "False" && autoClient.FindBoss("Hoang Hoa Luu Kim Dao").ToString() == "True")
                                        {
                                            autoClient.ValueBossKillsvuotai = "Hoang Hoa Luu Kim Dao";
                                            Thread.Sleep(500);
                                        }
                                        else if (autoClient.FindBoss("Hoang Hoa Luu Kim").ToString() == "False" && autoClient.FindBoss("Hoang Hoa Luu Kim Dao").ToString() == "False")
                                        {
                                            autoClient.ValueBossKillsvuotai = "";
                                            autoClient.CkKillBossvuotai = true;
                                        }
                                        break;
                                    }
                                // Bắt đầu đánh ải 7
                                case 30:
                                    {
                                        autoClient.ShortMove(52992, 111104);

                                        break;
                                    }
                                case 31:
                                    {
                                        if (autoClient.FindBoss("Gia Luat Phi Hong").ToString() == "True")
                                        {
                                            autoClient.ValueBossKillsvuotai = "Gia Luat Phi Hong";
                                            autoClient.CkKillBossvuotai = true;
                                            Thread.Sleep(500);
                                        }
                                        else if (autoClient.FindBoss("Gia Luat Phi Hong").ToString() == "False")
                                        {
                                            autoClient.ValueBossKillsvuotai = "";
                                            autoClient.CkKillBossvuotai = true;

                                            Thread.Sleep(500);
                                        }

                                        break;
                                    }
                                case 32:
                                    {
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Auto TNT: " + ex.Message);
                    return;
                }
            }
        }
        #endregion

        //Thủ tục Auto chat
        private void Ckbautochat_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckraovat = Ckbautochat.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTNC", "AutoTNC.Check4", CurrentClient.ckraovat.ToString());
            }
        }

        //Thủ tục Auto rao bán
        #region Thủ tục Auto rao bán
        private void Autoraoban()
        {
            while (true)
            {
                Thread.Sleep(2000);

                try
                {
                    if (_clients.Count < 1)
                        continue;

                    Dictionary<IntPtr,AutoClientBS>  _clientstraoban = new Dictionary<IntPtr, AutoClientBS>(_clients);

                    foreach (var autoClient in _clientstraoban.Values)
                    {

                        if (!autoClient.ckraovat || !autoClient.ischecked)
                            continue;

                        if (!autoClient.isInjected)
                            autoClient.Inject();


                        if (autoClient.ValueLoirao != "" && autoClient.ValueLoirao != null)
                        {
                            autoClient.SendClick(369, 544);
                            Thread.Sleep(5);
                            autoClient.SendClick(368, 650);
                            Thread.Sleep(5);
                            autoClient.sendphim((int)WinAPI.keyflag.KEY_RETURN);
                            Thread.Sleep(5);
                            autoClient.sendstring(autoClient.ValueLoirao);
                            Thread.Sleep(5);
                            autoClient.sendphim((int)WinAPI.keyflag.KEY_RETURN);
                        }

                        Thread.Sleep(18000);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Auto Chat: " + ex.Message);
                    return;
                }
            }
        }
        #endregion

        /////////////////////////////////////Thủ tục mở thẻ bài miễn phí /////////////////////////////
        #region Thủ tục mở thẻ bài miễn phí
        private void AutoLatthebai()
        {
            while (true)
            {
                Thread.Sleep(500);
                try
                {
                    if (_clients.Count < 1)
                        continue;

                    var _clientstamthoi = new Dictionary<IntPtr, AutoClientBS>(_clients);

                    foreach (var autoClient in _clientstamthoi.Values)
                    {
                        if (!autoClient.CkThainhatthap || !autoClient.ischecked)
                            continue;

                        if (!autoClient.isInjected)
                            autoClient.Inject();

                        if (autoClient.Menumothe == 1 && autoClient.CklattheTKL == true)
                        {
                            autoClient.LatBai(GameConst.Thebaimienphi[0]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.Thebaimienphi[1]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.Thebaimienphi[2]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.Thebaimienphi[3]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.Thebaimienphi[4]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.Thebaimienphi[5]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.Thebaimienphi[6]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.Thebaimienphi[7]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.ThebaiTKL[0]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.ThebaiTKL[1]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.ThebaiTKL[2]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.ThebaiTKL[3]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.ThebaiTKL[4]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.ThebaiTKL[5]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.ThebaiTKL[6]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.ThebaiTKL[7]);
                            Thread.Sleep(50);
                        }
                        else if (autoClient.Menumothe == 1 && autoClient.CklattheTKL == false)
                        {
                            autoClient.LatBai(GameConst.Thebaimienphi[0]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.Thebaimienphi[1]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.Thebaimienphi[2]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.Thebaimienphi[3]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.Thebaimienphi[4]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.Thebaimienphi[5]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.Thebaimienphi[6]);
                            Thread.Sleep(50);
                            autoClient.LatBai(GameConst.Thebaimienphi[7]);
                            Thread.Sleep(50);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Auto lat the bai: " + ex.Message);
                    return;
                }
            }
        }
        #endregion

        // Thủ tục Button lưu vị trí đào
        private void BtnluuvitridaoTNC_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ToadovitridaoTNC[0] = CurrentClient.CurrentPlayer.PositionX;
                CurrentClient.ToadovitridaoTNC[1] = CurrentClient.CurrentPlayer.PositionY;
            }
        }

        // Thủ tục Auto tổ đội
        #region Thủ tục Auto tổ đội

        private void AutoGroup()
        {
            while (true)
            {
                Thread.Sleep(500);
                try
                {
                    if (_clients.Count < 1)
                        continue;

                    var _clientsGroup = new Dictionary<IntPtr, AutoClientBS>(_clients);

                    foreach (var autoClient in _clientsGroup.Values)
                    {
                        if (autoClient.Ckautotodoi == true && autoClient.ischecked == true)
                        {

                            if (!autoClient.isInjected)
                                autoClient.Inject();

                            if (autoClient.Ckmoitatca == true)
                            {
                                autoClient.RefreshEntityList();

                                autoClient._DanhsachPlayerGroup = (from entity in CurrentClient.EntityList where entity.EntityType == NPCType.Player && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 1100 && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) > 0 orderby entity.EntityId select entity.EntityId.ToString()).ToList();

                                Thread.Sleep(100);

                                foreach(var items in autoClient._DanhsachPlayerGroup)
                                {
                                    autoClient.moitodoi(Convert.ToInt32(items.ToString()));
                                    Thread.Sleep(50);
                                }
                            }

                            if (autoClient.Cknhaptatca == true)
                            {
                                autoClient.RefreshEntityList();

                                autoClient._DanhsachPlayerGroup = (from entity in CurrentClient.EntityList where entity.EntityType == NPCType.Player && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 1100 && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) > 0 orderby entity.EntityId select entity.EntityId.ToString()).ToList();

                                Thread.Sleep(100);

                                foreach (var items in autoClient._DanhsachPlayerGroup)
                                {
                                    autoClient.xinnhaptodoi(Convert.ToInt32(items.ToString()));
                                    Thread.Sleep(50);
                                }
                            }

                            if (autoClient.Cknhanloimoitodoi == true)
                            {
                                autoClient.RefreshEntityList();

                                autoClient._DanhsachPlayerGroup = (from entity in CurrentClient.EntityList where entity.EntityType == NPCType.Player && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 1100 && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) > 0 orderby entity.EntityId select entity.EntityId.ToString()).ToList();

                                Thread.Sleep(100);

                                foreach (var items in autoClient._DanhsachPlayerGroup)
                                {
                                    autoClient.nhanloimoitodoi(Convert.ToInt32(items.ToString()));
                                    Thread.Sleep(50);
                                }
                            }

                            if (autoClient.Ckchapnhanxinnhapdoi == true)
                            {
                                autoClient.RefreshEntityList();

                                autoClient._DanhsachPlayerGroup = (from entity in CurrentClient.EntityList where entity.EntityType == NPCType.Player && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) < 1100 && Convert.ToInt32(GameGeneral.Distance(CurrentClient.CurrentPlayer.PositionX, CurrentClient.CurrentPlayer.PositionY, entity.PositionX, entity.PositionY)) > 0 orderby entity.EntityId select entity.EntityId.ToString()).ToList();

                                Thread.Sleep(100);

                                foreach (var items in autoClient._DanhsachPlayerGroup)
                                {
                                    autoClient.chapnhanchonhaptodoi(Convert.ToInt32(items.ToString()));
                                    Thread.Sleep(50);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Auto Group: " + ex.Message);
                    return;
                }
            }
        }
        #endregion

        // Thủ tục Auto đốt pháo + bảo hạp
        #region Thủ tục Auto đốt pháo + bảo hạp

        private void AutoDotPhaoBH()
        {
            while (true)
            {
                Thread.Sleep(1000);

                try
                {
                    if (_clients.Count < 1)
                        continue;

                    var _clientsdotphaoBH = new Dictionary<IntPtr, AutoClientBS>(_clients);

                    foreach (var autoClient in _clientsdotphaoBH.Values)
                    {

                        if (autoClient.Ckdotphao == true && autoClient.ischecked == true)
                        {
                            if (!autoClient.isInjected)
                                autoClient.Inject();

                            autoClient.FindAndUseItem("Phao chien thang ");
                            Thread.Sleep(4000);
                        }

                        if (autoClient.Cknuoabaohap = true && autoClient.ischecked == true)
                        {
                            if (!autoClient.isInjected)
                                autoClient.Inject();

                            autoClient.FindAndUseItem("Nu Oa Bao Hap ");
                            Thread.Sleep(1000);
                        }

                        if (autoClient.Ckngoisaomayman = true && autoClient.ischecked == true)
                        {
                            if (!autoClient.isInjected)
                                autoClient.Inject();

                            autoClient.FindAndUseItem("Ngoi Sao May Man ");
                            Thread.Sleep(4000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Auto BH: " + ex.Message);
                    return;
                }
            }
        }
        #endregion

        // Thủ tục button lập tổ đội
        private void Btnlapto_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
               CurrentClient.laptodoi();
            }
        }

        // Thủ tục button Giải tán tổ đội
        private void Btngiaitan_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
               CurrentClient.giaitantodoi();
            }
        }

        // Thủ tục Timer CheckChars
        #region Thủ tục Timer CheckChars
        private void TmrCheckChars_Tick(object sender, EventArgs e)
        {
            lsvplayer.Invoke(new MethodInvoker(() =>
            {
                _clientHwnds.Clear();
                WinAPI.EnumWindows(SearchForGameWindows, new IntPtr(0));


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

                // Cập nhật Client

                foreach (var clientHwnd in _clientHwnds)
                {
                    var client = new AutoClientBS();
                    client.Attach(clientHwnd);

                    _clients.Add(clientHwnd, client);

                    ListViewItem row = new ListViewItem(clientHwnd.ToInt32().ToString(), 0);

                    row.SubItems.Add(client.CurrentPlayer.EntityNameUnicode);
                    row.SubItems.Add(client.CurrentPlayer.TrangThaiOnline);

                    lsvplayer.Items.Add(row);
                }

                ///////////////////Update ListView////////////////
                var _rowNeedToBeRemoved = new List<ListViewItem>();

                foreach (ListViewItem row in lsvplayer.Items)
                {
                    var key = new IntPtr(Convert.ToInt32(row.Text));
                    if (_clients.ContainsKey(key))
                    {
                        var client = _clients[key];

                        row.SubItems[1].Text = client.CurrentPlayer.EntityNameUnicode;
                        row.SubItems[2].Text = client.CurrentPlayer.TrangThaiOnline;
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

                ///////////////////Update Checkbox////////////////

                if (lsvplayer.Items.Count < 1)
                {
                    checkBoxAcount.Checked = false;
                    cbbatdau.Checked = false;
                    Ckbautochat.Checked = false;
                    Ckbautothaydo.Checked = false;
                    Ckbdotphao.Checked = false;
                    Ckbnuoabaohap.Checked = false;
                    Ckbngoisaomayman.Checked = false;
                    Ckbautotodoi.Checked = false;
                    CkbAutoTrain.Checked = false;
                    CkbClickNPC.Checked = false;
                    Ckbtainguyenchien.Checked = false;
                    Ckbthainhatthap.Checked = false;
                    Ckbautothaydo.Checked = false;
                    ckbhoiphuc.Checked = false;
                    CBchayAutoCombo.Checked = false;
                }

            }));

        }
        #endregion

        // Thay đổi chiều rộng Header cột Listview
        private void lsvplayer_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.GreenYellow, e.Bounds);
            e.DrawText();
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
            capnhatthongtinchoAcount();
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

        // Cập nhật thông tin nhân vật
        #region Cập nhật thông tin nhân vật

        private void capnhatthongtinchoAcount()
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

                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";

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

                CurrentClient.ckthaydobo1 = bool.Parse(WinAPI.Docfile(text, "CheckThaydo", "CheckThaydo.Check1", "false"));
                CurrentClient.ckthaydobo2 = bool.Parse(WinAPI.Docfile(text, "CheckThaydo", "CheckThaydo.Check2", "false"));
                CurrentClient.ckthaydobo3 = bool.Parse(WinAPI.Docfile(text, "CheckThaydo", "CheckThaydo.Check3", "false"));
                CurrentClient.ckthaydobo4 = bool.Parse(WinAPI.Docfile(text, "CheckThaydo", "CheckThaydo.Check4", "false"));
                CurrentClient.ckthaydo = bool.Parse(WinAPI.Docfile(text, "CheckThaydo", "CheckThaydo.Check5", "false"));

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

                CurrentClient.ckautoTNC = bool.Parse(WinAPI.Docfile(text, "AutoTNC", "AutoTNC.Check1", "false"));
                CurrentClient.cktutranv = bool.Parse(WinAPI.Docfile(text, "AutoTNC", "AutoTNC.Check2", "false"));
                CurrentClient.cktumuadungcu = bool.Parse(WinAPI.Docfile(text, "AutoTNC", "AutoTNC.Check3", "false"));
                CurrentClient.ckraovat = bool.Parse(WinAPI.Docfile(text, "AutoTNC", "AutoTNC.Check4", "false"));
                CurrentClient.ckphetong = bool.Parse(WinAPI.Docfile(text, "AutoTNC", "AutoTNC.Check5", "false"));

                CurrentClient.Bufftheosau = bool.Parse(WinAPI.Docfile(text, "CheckBuff", "CheckBuff.Check2", "false"));
                CurrentClient.buffdanhsach = bool.Parse(WinAPI.Docfile(text, "CheckBuff", "CheckBuff.Check3", "false"));

                CurrentClient.ckautotrain = bool.Parse(WinAPI.Docfile(text, "AutoTrain", "AutoTrain.Check1", "false"));
                CurrentClient.CkToadoTrain01 = bool.Parse(WinAPI.Docfile(text, "AutoTrain", "AutoTrain.Check2", "false"));
                CurrentClient.CkToadoTrain02 = bool.Parse(WinAPI.Docfile(text, "AutoTrain", "AutoTrain.Check3", "false"));
                CurrentClient.CkToadoTrain03 = bool.Parse(WinAPI.Docfile(text, "AutoTrain", "AutoTrain.Check4", "false"));
                CurrentClient.CkSkindanhquai = bool.Parse(WinAPI.Docfile(text, "AutoTrain", "AutoTrain.Check5", "false"));
                CurrentClient.CkSkinBuff01 = bool.Parse(WinAPI.Docfile(text, "AutoTrain", "AutoTrain.Check6", "false"));
                CurrentClient.CkSkinBuff02 = bool.Parse(WinAPI.Docfile(text, "AutoTrain", "AutoTrain.Check7", "false"));
                CurrentClient.CkKillBoss = bool.Parse(WinAPI.Docfile(text, "AutoTrain", "AutoTrain.Check8", "false"));

                CurrentClient.CkThainhatthap = bool.Parse(WinAPI.Docfile(text, "AutoTNT", "AutoTNT.Check1", "false"));
                CurrentClient.CkChukey = bool.Parse(WinAPI.Docfile(text, "AutoTNT", "AutoTNT.Check2", "false"));
                CurrentClient.CkmoruongTKL = bool.Parse(WinAPI.Docfile(text, "AutoTNT", "AutoTNT.Check3", "false"));
                CurrentClient.CklattheTKL = bool.Parse(WinAPI.Docfile(text, "AutoTNT", "AutoTNT.Check4", "false"));
                CurrentClient.CkVuotaiTKL = bool.Parse(WinAPI.Docfile(text, "AutoTNT", "AutoTNT.Check5", "false"));
                CurrentClient.Ckacctreo = bool.Parse(WinAPI.Docfile(text, "AutoTNT", "AutoTNT.Check6", "false"));

                // Auto tổ đội
                CurrentClient.Ckautotodoi = bool.Parse(WinAPI.Docfile(text, "AutoGroup", "AutoGroup.Check1", "false"));
                CurrentClient.Ckmoitatca = bool.Parse(WinAPI.Docfile(text, "AutoGroup", "AutoGroup.Check2", "false"));
                CurrentClient.Cknhaptatca = bool.Parse(WinAPI.Docfile(text, "AutoGroup", "AutoGroup.Check3", "false"));
                CurrentClient.Cknhanloimoitodoi = bool.Parse(WinAPI.Docfile(text, "AutoGroup", "AutoGroup.Check4", "false"));
                CurrentClient.Ckchapnhanxinnhapdoi = bool.Parse(WinAPI.Docfile(text, "AutoGroup", "AutoGroup.Check5", "false"));

                // Auto đốt pháo hoa + nữ oa bảo hạp
                CurrentClient.Ckdotphao = bool.Parse(WinAPI.Docfile(text, "AutoBH", "AutoBH.Check1", "false"));
                CurrentClient.Cknuoabaohap = bool.Parse(WinAPI.Docfile(text, "AutoBH", "AutoBH.Check2", "false"));
                CurrentClient.Ckngoisaomayman = bool.Parse(WinAPI.Docfile(text, "AutoBH", "AutoBH.Check3", "false"));

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

                // Auto Click NPC
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
                CurrentClient.phimthaydo4 = WinAPI.Docfile(text, "KeyBoard", "phimthaydo4", "F9");

                // Auto train quái
                CurrentClient.ValueToadoNV = WinAPI.Docfile(text, "AutoTrain", "ValueToadoNV", "");
                CurrentClient.ValueToadoTrain01 = WinAPI.Docfile(text, "AutoTrain", "ValueToadoTrain01", "");
                CurrentClient.ValueToadoTrain02 = WinAPI.Docfile(text, "AutoTrain", "ValueToadoTrain02", "");
                CurrentClient.ValueToadoTrain03 = WinAPI.Docfile(text, "AutoTrain", "ValueToadoTrain03", "");
                CurrentClient.ValueSkindanhquai = WinAPI.Docfile(text, "AutoTrain", "ValueSkindanhquai", "");
                CurrentClient.ValueSkinBuff01 = WinAPI.Docfile(text, "AutoTrain", "ValueSkinBuff01", "");
                CurrentClient.ValueSkinBuff02 = WinAPI.Docfile(text, "AutoTrain", "ValueSkinBuff02", "");
                CurrentClient.ValueBossKills = WinAPI.Docfile(text, "AutoTrain", "ValueBossKills", "");

                // Lấy vật phẩm phục hồi
                CurrentClient.ItemSinhLuc = WinAPI.Docfile(text, "ItemPhucHoi", "ItemSinhLuc", "");
                CurrentClient.ItemSinhNoi = WinAPI.Docfile(text, "ItemPhucHoi", "ItemSinhNoi", "");
                CurrentClient.ItemNoiLuc = WinAPI.Docfile(text, "ItemPhucHoi", "ItemNoiLuc", "");
                CurrentClient.ItemCuuChuyen = WinAPI.Docfile(text, "ItemPhucHoi", "ItemCuuChuyen", "");
                CurrentClient.ItemLakCtr = WinAPI.Docfile(text, "ItemPhucHoi", "ItemLakCtr", "");
                CurrentClient.ItemTheLuc = WinAPI.Docfile(text, "ItemPhucHoi", "ItemTheLuc", "");

                // Auto tài nguyên chiến và auto rao vặt
                CurrentClient.ValueQuancong = WinAPI.Docfile(text, "AutoTNC", "ValueQuancong", "");
                CurrentClient.ValueLoirao = WinAPI.Docfile(text, "AutoTNC", "ValueLoirao", "");


                // Lấy danh sách buff đồ 1
                List<string> List2 = new List<string>(25);
                for (int i = 0; i < 25; i++)
                {
                    List2.Add(WinAPI.Docfile(text, "Listthaydo1", "Listthaydo1.Item" + i, "Null"));
                }
                CurrentClient.Listthaydo1 = List2;

                // Lấy danh sách buff đồ 2
                List<string> List3 = new List<string>(25);
                for (int i = 0; i < 25; i++)
                {
                    List3.Add(WinAPI.Docfile(text, "Listthaydo2", "Listthaydo2.Item" + i, "Null"));
                }
                CurrentClient.Listthaydo2 = List3;

                // Lấy danh sách buff đồ 3
                List<string> List4 = new List<string>(25);
                for (int i = 0; i < 25; i++)
                {
                    List4.Add(WinAPI.Docfile(text, "Listthaydo3", "Listthaydo3.Item" + i, "Null"));
                }
                CurrentClient.Listthaydo3 = List4;

                // Lấy danh sách buff đồ 4
                List<string> List5 = new List<string>(25);
                for (int i = 0; i < 25; i++)
                {
                    List5.Add(WinAPI.Docfile(text, "Listthaydo4", "Listthaydo4.Item" + i, "Null"));
                }
                CurrentClient.Listthaydo4 = List5;

                // load giá trị vào Project
                CBchayAutoCombo.Checked = CurrentClient.ComboPKTLQ;

                cbbatdau.Checked = CurrentClient.Featurenmk;

                ckbhoiphuc.Checked = CurrentClient.FeatureHoiPhuc;

                CkbClickNPC.Checked = CurrentClient.ckClickNPC;

                Ckbtainguyenchien.Checked = CurrentClient.ckautoTNC;

                CkbAutoTrain.Checked = CurrentClient.ckautotrain;

                Ckbautochat.Checked = CurrentClient.ckraovat;

                Ckbthainhatthap.Checked = CurrentClient.CkThainhatthap;

                Ckbautotodoi.Checked = CurrentClient.Ckautotodoi;

                Ckbautothaydo.Checked = CurrentClient.ckthaydo;

                // Auto dốt pháo + nữ oa bảo hạp
                Ckbdotphao.Checked = CurrentClient.Ckdotphao;
                Ckbnuoabaohap.Checked = CurrentClient.Cknuoabaohap;
                Ckbngoisaomayman.Checked = CurrentClient.Ckngoisaomayman;


                //Lấy danh sách buff
                List<string> List1 = new List<string>(7);
                for (int i = 0; i < 7; i++)
                {
                    List1.Add(WinAPI.Docfile(text, "ListBuff", "PlayerBuff" + i, ""));
                }
                CurrentClient.Listbuff = List1;

                
                frmbuff.CurrentClient = CurrentClient;
                frmGroup.CurrentClient = CurrentClient;
                frmTrain.CurrentClient = CurrentClient;
                frmClickNPC.CurrentClient = CurrentClient;
                frmTNC.CurrentClient = CurrentClient;
                frmRaoban.CurrentClient = CurrentClient;
                frmTNT.CurrentClient = CurrentClient;
                frmthaydo.CurrentClient = CurrentClient;
                frmphuchoi.CurrentClient = CurrentClient;
                frmCombo.CurrentClient = CurrentClient;

            }));
        }
        #endregion

        // Thủ tục DoubleClick vào listviews
        #region Thủ tục DoubleClick vào listviews
        private void ListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            foreach(var Item in ListView1.SelectedItems)
            {
                if(Item.ToString() == "ListViewItem: {      Auto Buff}" && e.Button == MouseButtons.Left)
                {
                    try
                    {
                        frmbuff.Refresh();
                        frmbuff.CurrentClient = CurrentClient;
                        frmbuff.StartPosition = FormStartPosition.Manual;
                        frmbuff.Left = Convert.ToInt32(this.Left - 240);
                        frmbuff.Top = Convert.ToInt32(this.Top);
                        frmbuff.Show();
                    }
                    catch
                    {
                        frmbuff = new FromAutoBuff();
                        frmbuff.CurrentClient = CurrentClient;
                        frmbuff.StartPosition = FormStartPosition.Manual;
                        frmbuff.Left = Convert.ToInt32(this.Left - 240);
                        frmbuff.Top = Convert.ToInt32(this.Top);
                        frmbuff.Show();
                    }
                }
                else if(Item.ToString() == "ListViewItem: {      Auto Tổ đội}" && e.Button == MouseButtons.Left)
                {
                    try
                    {
                        frmGroup.Refresh();
                        frmGroup.CurrentClient = CurrentClient;
                        frmGroup.StartPosition = FormStartPosition.Manual;
                        frmGroup.Left = Convert.ToInt32(this.Left - 240);
                        frmGroup.Top = Convert.ToInt32(this.Top);
                        frmGroup.Show();
                    }
                    catch
                    {
                        frmGroup = new FrmFormAutoGroup();
                        frmGroup.CurrentClient = CurrentClient;
                        frmGroup.StartPosition = FormStartPosition.Manual;
                        frmGroup.Left = Convert.ToInt32(this.Left - 240);
                        frmGroup.Top = Convert.ToInt32(this.Top);
                        frmGroup.Show();
                    }


                }
                else if (Item.ToString() == "ListViewItem: {      Auto Train}" && e.Button == MouseButtons.Left)
                {
                    try
                    {
                        frmTrain.Refresh();
                        frmTrain.CurrentClient = CurrentClient;
                        frmTrain.StartPosition = FormStartPosition.Manual;
                        frmTrain.Left = Convert.ToInt32(this.Left - 240);
                        frmTrain.Top = Convert.ToInt32(this.Top);
                        frmTrain.Show();
                    }
                    catch
                    {
                        frmTrain = new FrmAutoTrain();
                        frmTrain.CurrentClient = CurrentClient;
                        frmTrain.StartPosition = FormStartPosition.Manual;
                        frmTrain.Left = Convert.ToInt32(this.Left - 240);
                        frmTrain.Top = Convert.ToInt32(this.Top);
                        frmTrain.Show();
                    }
                }
                else if (Item.ToString() == "ListViewItem: {      Auto Click NPC}" && e.Button == MouseButtons.Left)
                {
                    try
                    {
                        frmClickNPC.Refresh();
                        frmClickNPC.CurrentClient = CurrentClient;
                        frmClickNPC.StartPosition = FormStartPosition.Manual;
                        frmClickNPC.Left = Convert.ToInt32(this.Left - 240);
                        frmClickNPC.Top = Convert.ToInt32(this.Top);
                        frmClickNPC.Show();
                    }
                    catch
                    {
                        frmClickNPC = new FrmClickNPC();
                        frmClickNPC.CurrentClient = CurrentClient;
                        frmClickNPC.StartPosition = FormStartPosition.Manual;
                        frmClickNPC.Left = Convert.ToInt32(this.Left - 240);
                        frmClickNPC.Top = Convert.ToInt32(this.Top);
                        frmClickNPC.Show();
                    }

                }
                else if (Item.ToString() == "ListViewItem: {      Auto TNC}" && e.Button == MouseButtons.Left)
                {
                    try
                    {
                        frmTNC.Refresh();
                        frmTNC.CurrentClient = CurrentClient;
                        frmTNC.StartPosition = FormStartPosition.Manual;
                        frmTNC.Left = Convert.ToInt32(this.Left - 240);
                        frmTNC.Top = Convert.ToInt32(this.Top);
                        frmTNC.Show();
                    }
                    catch
                    {
                        frmTNC = new FrmTNC();
                        frmTNC.CurrentClient = CurrentClient;
                        frmTNC.StartPosition = FormStartPosition.Manual;
                        frmTNC.Left = Convert.ToInt32(this.Left - 240);
                        frmTNC.Top = Convert.ToInt32(this.Top);
                        frmTNC.Show();
                    }
                }
                else if (Item.ToString() == "ListViewItem: {      Auto rao bán}" && e.Button == MouseButtons.Left)
                {
                    try
                    {
                        frmRaoban.Refresh();
                        frmRaoban.CurrentClient = CurrentClient;
                        frmRaoban.StartPosition = FormStartPosition.Manual;
                        frmRaoban.Left = Convert.ToInt32(this.Left - 240);
                        frmRaoban.Top = Convert.ToInt32(this.Top);
                        frmRaoban.Show();
                    }
                    catch
                    {
                        frmRaoban = new FrmRaoban();
                        frmRaoban.CurrentClient = CurrentClient;
                        frmRaoban.StartPosition = FormStartPosition.Manual;
                        frmRaoban.Left = Convert.ToInt32(this.Left - 240);
                        frmRaoban.Top = Convert.ToInt32(this.Top);
                        frmRaoban.Show();
                    }
                }

                else if (Item.ToString() == "ListViewItem: {      Auto Thái Nhất Tháp}" && e.Button == MouseButtons.Left)
                {
                    try
                    {
                        frmTNT.Refresh();
                        frmTNT.CurrentClient = CurrentClient;
                        frmTNT.StartPosition = FormStartPosition.Manual;
                        frmTNT.Left = Convert.ToInt32(this.Left - 240);
                        frmTNT.Top = Convert.ToInt32(this.Top);
                        frmTNT.Show();
                    }
                    catch
                    {
                        frmTNT = new FrmTNT();
                        frmTNT.CurrentClient = CurrentClient;
                        frmTNT.StartPosition = FormStartPosition.Manual;
                        frmTNT.Left = Convert.ToInt32(this.Left - 240);
                        frmTNT.Top = Convert.ToInt32(this.Top);
                        frmTNT.Show();
                    }
                }

                else if (Item.ToString() == "ListViewItem: {      Auto Thay đồ}" && e.Button == MouseButtons.Left)
                {
                    try
                    {
                        frmthaydo.Refresh();
                        frmthaydo.CurrentClient = CurrentClient;
                        frmthaydo.StartPosition = FormStartPosition.Manual;
                        frmthaydo.Left = Convert.ToInt32(this.Left - 240);
                        frmthaydo.Top = Convert.ToInt32(this.Top);
                        frmthaydo.Show();
                    }
                    catch
                    {
                        frmthaydo = new FrmThaydo();
                        frmthaydo.CurrentClient = CurrentClient;
                        frmthaydo.StartPosition = FormStartPosition.Manual;
                        frmthaydo.Left = Convert.ToInt32(this.Left - 240);
                        frmthaydo.Top = Convert.ToInt32(this.Top);
                        frmthaydo.Show();
                    }
                }

                else if (Item.ToString() == "ListViewItem: {      Auto Phục hồi}" && e.Button == MouseButtons.Left)
                {
                    try
                    {
                        frmphuchoi.Refresh();
                        frmphuchoi.CurrentClient = CurrentClient;
                        frmphuchoi.StartPosition = FormStartPosition.Manual;
                        frmphuchoi.Left = Convert.ToInt32(this.Left - 240);
                        frmphuchoi.Top = Convert.ToInt32(this.Top);
                        frmphuchoi.Show();
                    }
                    catch
                    {
                        frmphuchoi = new Frmphuchoi();
                        frmphuchoi.CurrentClient = CurrentClient;
                        frmphuchoi.StartPosition = FormStartPosition.Manual;
                        frmphuchoi.Left = Convert.ToInt32(this.Left - 240);
                        frmphuchoi.Top = Convert.ToInt32(this.Top);
                        frmphuchoi.Show();
                    }
                }

                else if (Item.ToString() == "ListViewItem: {      Auto Combo TLQ}" && e.Button == MouseButtons.Left)
                {
                    try
                    {
                        frmCombo.Refresh();
                        frmCombo.CurrentClient = CurrentClient;
                        frmCombo.StartPosition = FormStartPosition.Manual;
                        frmCombo.Left = Convert.ToInt32(this.Left - 240);
                        frmCombo.Top = Convert.ToInt32(this.Top);
                        frmCombo.Show();
                    }
                    catch
                    {
                        frmCombo = new FrmComboTLQ();
                        frmCombo.CurrentClient = CurrentClient;
                        frmCombo.StartPosition = FormStartPosition.Manual;
                        frmCombo.Left = Convert.ToInt32(this.Left - 240);
                        frmCombo.Top = Convert.ToInt32(this.Top);
                        frmCombo.Show();
                    }
                }
            }
        }
        #endregion

        // Thủ tục lưu Checkbox
        #region Thủ tục lưu check box hoạt động
        private void cbbatdau_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.Featurenmk = cbbatdau.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckBuff", "CheckBuff.Check1", CurrentClient.Featurenmk.ToString());
            }
        }

        // Thủ tục lưu Checkbox
        private void Ckbdotphao_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.Ckdotphao = Ckbdotphao.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoBH", "AutoBH.Check1", CurrentClient.Ckdotphao.ToString());
            }
        }

        // Thủ tục lưu Checkbox
        private void Ckbnuoabaohap_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.Cknuoabaohap = Ckbnuoabaohap.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoBH", "AutoBH.Check2", CurrentClient.Cknuoabaohap.ToString());
            }
        }

        // Thủ tục lưu Checkbox
        private void Ckbngoisaomayman_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.Ckngoisaomayman = Ckbngoisaomayman.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoBH", "AutoBH.Check3", CurrentClient.Ckngoisaomayman.ToString());
            }
        }

        // Thủ tục lưu Checkbox
        private void Ckbautotodoi_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.Ckautotodoi = Ckbautotodoi.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoGroup", "AutoGroup.Check1", CurrentClient.Ckautotodoi.ToString());
            }
        }

        // Thủ tục lưu Checkbox
        private void CkbAutoTrain_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckautotrain = CkbAutoTrain.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoTrain", "AutoTrain.Check1", CurrentClient.ckautotrain.ToString());
            }
        }

        // Thủ tục lưu Checkbox
        private void CkbClickNPC_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckClickNPC = CkbClickNPC.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "AutoClickNPC", "AutoClickNPC.Check1", CurrentClient.ckClickNPC.ToString());
            }
        }

        // Thủ tục lưu Checkbox
        private void Ckbautothaydo_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentClient != null)
            {
                CurrentClient.ckthaydo = Ckbautothaydo.Checked;
                string entityNameUnicode = CurrentClient.CurrentPlayer.EntityNameUnicode;
                string text = "//UserData//" + entityNameUnicode.Replace("*", ".") + ".ini";
                WinAPI.Ghifile(text, "CheckThaydo", "CheckThaydo.Check5", CurrentClient.ckthaydo.ToString());
            }
        }


        #endregion

        // Thủ tục Button Login
        private void BtnAutoLogin_Click(object sender, EventArgs e)
        {
                FrmAutoLogin frmautolog = FrmAutoLogin.Instance;
                frmautolog.StartPosition = FormStartPosition.Manual;
                frmautolog.Left = Convert.ToInt32(this.Left - 240);
                frmautolog.Top = Convert.ToInt32(this.Top);
                frmautolog.Show();
        }

        #region Thủ tục lọc điều kiện checkbox
        // Thủ tục lọc hệ phái NMK
        private void cbbatdau_Click(object sender, EventArgs e)
        {
            if(CurrentClient!=null && CurrentClient.CurrentPlayer.Hephai != (int)HePhai.Nmk)
            {
                cbbatdau.Checked = false;
                MessageBox.Show("Nhân vật này không phải NMK không thể sử dụng chức năng này", "Cảnh báo!");
            }
            else if (CurrentClient == null && lsvplayer.Items.Count>=1)
            {
                cbbatdau.Checked = false;
                MessageBox.Show("Bạn chưa chọn nhân vật!", "Cảnh báo!");
            }
            else if (CurrentClient == null && lsvplayer.Items.Count < 1)
            {
                cbbatdau.Checked = false;
                MessageBox.Show("Chưa đăng nhập nhân vật!", "Cảnh báo!");
            }
        }

        // Thủ tục lọc hệ phái TLQ
        private void CBchayAutoCombo_Click(object sender, EventArgs e)
        {
            if (CurrentClient != null && CurrentClient.CurrentPlayer.Hephai != (int)HePhai.Tlq)
            {
                CBchayAutoCombo.Checked = false;
                MessageBox.Show("Nhân vật này không phải TLQ không thể sử dụng chức năng này", "Cảnh báo!");
            }
            else if (CurrentClient == null && lsvplayer.Items.Count >= 1)
            {
                CBchayAutoCombo.Checked = false;
                MessageBox.Show("Bạn chưa chọn nhân vật!", "Cảnh báo!");
            }
            else if (CurrentClient == null && lsvplayer.Items.Count < 1)
            {
                CBchayAutoCombo.Checked = false;
                MessageBox.Show("Chưa đăng nhập nhân vật!", "Cảnh báo!");
            }
        }

        // Thủ tục kiểm tra lựa chọn nhân vật
        private void Ckbdotphao_Click(object sender, EventArgs e)
        {
            if (CurrentClient == null && lsvplayer.Items.Count >= 1)
            {
                Ckbdotphao.Checked = false;
                MessageBox.Show("Bạn chưa chọn nhân vật!", "Cảnh báo!");
            }
            else if (CurrentClient == null && lsvplayer.Items.Count < 1)
            {
                Ckbdotphao.Checked = false;
                MessageBox.Show("Chưa đăng nhập nhân vật!", "Cảnh báo!");
            }
        }

        // Thủ tục kiểm tra lựa chọn nhân vật
        private void Ckbnuoabaohap_Click(object sender, EventArgs e)
        {
            if (CurrentClient == null && lsvplayer.Items.Count >= 1)
            {
                Ckbnuoabaohap.Checked = false;
                MessageBox.Show("Bạn chưa chọn nhân vật!", "Cảnh báo!");
            }
            else if (CurrentClient == null && lsvplayer.Items.Count < 1)
            {
                Ckbnuoabaohap.Checked = false;
                MessageBox.Show("Chưa đăng nhập nhân vật!", "Cảnh báo!");
            }
        }

        // Thủ tục kiểm tra lựa chọn nhân vật
        private void Ckbngoisaomayman_Click(object sender, EventArgs e)
        {
            if (CurrentClient == null && lsvplayer.Items.Count >= 1)
            {
                Ckbngoisaomayman.Checked = false;
                MessageBox.Show("Bạn chưa chọn nhân vật!", "Cảnh báo!");
            }
            else if (CurrentClient == null && lsvplayer.Items.Count < 1)
            {
                Ckbngoisaomayman.Checked = false;
                MessageBox.Show("Chưa đăng nhập nhân vật!", "Cảnh báo!");
            }
        }

        // Thủ tục kiểm tra lựa chọn nhân vật
        private void Ckbautotodoi_Click(object sender, EventArgs e)
        {
            if (CurrentClient == null && lsvplayer.Items.Count >= 1)
            {
                Ckbautotodoi.Checked = false;
                MessageBox.Show("Bạn chưa chọn nhân vật!", "Cảnh báo!");
            }
            else if (CurrentClient == null && lsvplayer.Items.Count < 1)
            {
                Ckbautotodoi.Checked = false;
                MessageBox.Show("Chưa đăng nhập nhân vật!", "Cảnh báo!");
            }
        }

        // Thủ tục kiểm tra lựa chọn nhân vật
        private void CkbAutoTrain_Click(object sender, EventArgs e)
        {
            if (CurrentClient == null && lsvplayer.Items.Count >= 1)
            {
                CkbAutoTrain.Checked = false;
                MessageBox.Show("Bạn chưa chọn nhân vật!", "Cảnh báo!");
            }
            else if (CurrentClient == null && lsvplayer.Items.Count < 1)
            {
                CkbAutoTrain.Checked = false;
                MessageBox.Show("Chưa đăng nhập nhân vật!", "Cảnh báo!");
            }
        }

        // Thủ tục kiểm tra lựa chọn nhân vật
        private void CkbClickNPC_Click(object sender, EventArgs e)
        {
            if (CurrentClient == null && lsvplayer.Items.Count >= 1)
            {
                CkbClickNPC.Checked = false;
                MessageBox.Show("Bạn chưa chọn nhân vật!", "Cảnh báo!");
            }
            else if (CurrentClient == null && lsvplayer.Items.Count < 1)
            {
                CkbClickNPC.Checked = false;
                MessageBox.Show("Chưa đăng nhập nhân vật!", "Cảnh báo!");
            }
        }

        // Thủ tục kiểm tra lựa chọn nhân vật
        private void Ckbtainguyenchien_Click(object sender, EventArgs e)
        {
            if (CurrentClient == null && lsvplayer.Items.Count >= 1)
            {
                Ckbtainguyenchien.Checked = false;
                MessageBox.Show("Bạn chưa chọn nhân vật!", "Cảnh báo!");
            }
            else if (CurrentClient == null && lsvplayer.Items.Count < 1)
            {
                Ckbtainguyenchien.Checked = false;
                MessageBox.Show("Chưa đăng nhập nhân vật!", "Cảnh báo!");
            }
        }

        // Thủ tục kiểm tra lựa chọn nhân vật
        private void Ckbautochat_Click(object sender, EventArgs e)
        {
            if (CurrentClient == null && lsvplayer.Items.Count >= 1)
            {
                Ckbautochat.Checked = false;
                MessageBox.Show("Bạn chưa chọn nhân vật!", "Cảnh báo!");
            }
            else if (CurrentClient == null && lsvplayer.Items.Count < 1)
            {
                Ckbautochat.Checked = false;
                MessageBox.Show("Chưa đăng nhập nhân vật!", "Cảnh báo!");
            }
        }

        // Thủ tục kiểm tra lựa chọn nhân vật
        private void Ckbthainhatthap_Click(object sender, EventArgs e)
        {
            if (CurrentClient == null && lsvplayer.Items.Count >= 1)
            {
                Ckbthainhatthap.Checked = false;
                MessageBox.Show("Bạn chưa chọn nhân vật!", "Cảnh báo!");
            }
            else if (CurrentClient == null && lsvplayer.Items.Count < 1)
            {
                Ckbthainhatthap.Checked = false;
                MessageBox.Show("Chưa đăng nhập nhân vật!", "Cảnh báo!");
            }
        }

        // Thủ tục kiểm tra lựa chọn nhân vật
        private void Ckbautothaydo_Click(object sender, EventArgs e)
        {
            if (CurrentClient == null && lsvplayer.Items.Count >= 1)
            {
                Ckbautothaydo.Checked = false;
                MessageBox.Show("Bạn chưa chọn nhân vật!", "Cảnh báo!");
            }
            else if (CurrentClient == null && lsvplayer.Items.Count < 1)
            {
                Ckbautothaydo.Checked = false;
                MessageBox.Show("Chưa đăng nhập nhân vật!", "Cảnh báo!");
            }
        }

        // Thủ tục kiểm tra lựa chọn nhân vật
        private void ckbhoiphuc_Click(object sender, EventArgs e)
        {
            if (CurrentClient == null && lsvplayer.Items.Count >= 1)
            {
                ckbhoiphuc.Checked = false;
                MessageBox.Show("Bạn chưa chọn nhân vật!", "Cảnh báo!");
            }
            else if (CurrentClient == null && lsvplayer.Items.Count < 1)
            {
                ckbhoiphuc.Checked = false;
                MessageBox.Show("Chưa đăng nhập nhân vật!", "Cảnh báo!");
            }
        }
        #endregion
    }
}
