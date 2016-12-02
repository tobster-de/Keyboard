using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using static Keyboard.MessagingApi;

namespace Keyboard
{
    /// <summary>Class for messaging and key presses</summary>
    [Serializable]
    public class Messaging
    {

        #region Methods

        #region Public

        public static bool GetKeyState(Key key)
        {
            if ((MessagingApi.GetKeyState((int)key.Vk) & 0xF0) == 1)
                return true;

            return false;
        }

        public static uint GetVirtualKeyCode(char c)
        {
            //var helper = new Helper { Value = VkKeyScan(c) };

            //byte virtualKeyCode = helper.Low;
            //byte shiftState = helper.High;

            //return virtualKeyCode;
            return (uint)VkKeyScan(c);
        }

        public static void BackgroundMousePosition(IntPtr hWnd, int x, int y)
        {
            MessagingApi.PostMessage(hWnd, (int)WindowsMessages.WM_MOUSEMOVE, 0, GetLParam(x, y));
        }

        public static void BackgroundMouseClick(IntPtr hWnd, Key key, int x, int y, int delay = 100)
        {
            switch (key.Vk)
            {
                case VKeys.KEY_MBUTTON:
                    MessagingApi.PostMessage(hWnd, (int)Message.MBUTTONDOWN, (uint)key.Vk, GetLParam(x, y));
                    Thread.Sleep(delay);
                    MessagingApi.PostMessage(hWnd, (int)Message.MBUTTONUP, (uint)key.Vk, GetLParam(x, y));
                    break;
                case VKeys.KEY_LBUTTON:
                    MessagingApi.PostMessage(hWnd, (int)Message.LBUTTONDOWN, (uint)key.Vk, GetLParam(x, y));
                    Thread.Sleep(delay);
                    MessagingApi.PostMessage(hWnd, (int)Message.LBUTTONUP, (uint)key.Vk, GetLParam(x, y));
                    break;
                case VKeys.KEY_RBUTTON:
                    MessagingApi.PostMessage(hWnd, (int)Message.RBUTTONDOWN, (uint)key.Vk, GetLParam(x, y));
                    Thread.Sleep(delay);
                    MessagingApi.PostMessage(hWnd, (int)Message.RBUTTONUP, (uint)key.Vk, GetLParam(x, y));
                    break;
            }
        }

        public static bool ForegroundKeyPress(Key key, int delay = 100)
        {
            bool temp = true;

            temp &= ForegroundKeyDown(key);
            Thread.Sleep(delay);
            temp &= ForegroundKeyUp(key);
            Thread.Sleep(delay);
            return temp;
        }

        public static bool ForegroundKeyPress(IntPtr hWnd, Key key, int delay = 100)
        {
            bool temp = true;

            temp &= ForegroundKeyDown(hWnd, key);
            Thread.Sleep(delay);
            temp &= ForegroundKeyUp(hWnd, key);
            Thread.Sleep(delay);
            return temp;
        }

        public static bool ForegroundKeyDown(Key key)
        {
            INPUT structInput = new INPUT
            {
                type = INPUT_KEYBOARD,
                ki =
                                            {
                                                wScan = 0,
                                                time = 0,
                                                dwFlags = 0,
                                                wVk = (ushort)key.Vk
                                            }
            };

            // Key down shift, ctrl, and/or alt
            // Key down the actual key-code
            SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));

            // Key up shift, ctrl, and/or alt
            //keybd_event((int)key.VK, GetScanCode(key.VK) + 0x80, KEYEVENTF_NONE, 0);
            //keybd_event((int)key.VK, GetScanCode(key.VK) + 0x80, KEYEVENTF_KEYUP, 0);
            return true;
        }

        public static bool ForegroundKeyUp(Key key)
        {
            INPUT structInput = new INPUT
            {
                type = INPUT_KEYBOARD,
                ki =
                                            {
                                                wScan = 0,
                                                time = 0,
                                                dwFlags = 0,
                                                wVk = (ushort)key.Vk
                                            }
            };

            // Key down shift, ctrl, and/or alt
            // Key down the actual key-code

            // Key up the actual key-code
            structInput.ki.dwFlags = KEYEVENTF_KEYUP;
            SendInput(1, ref structInput, Marshal.SizeOf(typeof(INPUT)));
            return true;
        }

        public static bool ForegroundKeyDown(IntPtr hWnd, Key key)
        {
            if (GetForegroundWindow() != hWnd)
            {
                if (!SetForegroundWindow(hWnd))
                    return false;
            }
            return ForegroundKeyDown(key);
        }

        public static bool ForegroundKeyUp(IntPtr hWnd, Key key)
        {
            if (GetForegroundWindow() != hWnd)
            {
                if (!SetForegroundWindow(hWnd))
                    return false;
            }
            return ForegroundKeyUp(key);
        }

        public static bool ForegroundKeyPress(IntPtr hWnd, Key key, bool alt, bool ctrl, bool shift, int delay = 100)
        {
            if (GetForegroundWindow() != hWnd)
            {
                if (!SetForegroundWindow(hWnd))
                    return false;
            }

            INPUT structInput = new INPUT
            {
                type = INPUT_KEYBOARD,
                ki =
                                            {
                                                wScan = 0,
                                                time = 0,
                                                dwFlags = 0
                                            }
            };

            // Key down shift, ctrl, and/or alt
            if (alt)
            {
                structInput.ki.wVk = (ushort)VKeys.KEY_MENU;
                SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
                Thread.Sleep(delay);
            }
            if (ctrl)
            {
                structInput.ki.wVk = (ushort)VKeys.KEY_CONTROL;
                SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
                Thread.Sleep(delay);
            }
            if (shift)
            {
                structInput.ki.wVk = (ushort)VKeys.KEY_SHIFT;
                SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
                Thread.Sleep(delay);

                if (key.ShiftKey != VKeys.NULL)
                {
                    structInput.ki.wVk = (ushort)key.ShiftKey;
                    SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
                    Thread.Sleep(delay);
                }
            }

            // Key up the actual key-code			
            ForegroundKeyPress(hWnd, key);

            structInput.ki.dwFlags = KEYEVENTF_KEYUP;
            if (shift && key.ShiftKey == VKeys.NULL)
            {
                structInput.ki.wVk = (ushort)VKeys.KEY_SHIFT;
                SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
                Thread.Sleep(delay);
            }
            if (ctrl)
            {
                structInput.ki.wVk = (ushort)VKeys.KEY_CONTROL;
                SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
                Thread.Sleep(delay);
            }
            if (alt)
            {
                structInput.ki.wVk = (ushort)VKeys.KEY_MENU;
                SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
                Thread.Sleep(delay);
            }

            return true;
        }

        private static bool PostMessageKeyDown(IntPtr hWnd, VKeys key)
        {
            return PostMessageSafe(hWnd, (int)Message.KEY_DOWN, (uint)key, GetLParam(1, key, 0, 0, 0, 0));
        }
        private static bool PostMessageKeyUp(IntPtr hWnd, VKeys key)
        {
            return PostMessageSafe(hWnd, (int)Message.KEY_UP, (uint)key, GetLParam(1, key, 0, 0, 0, 0));
        }

        // https://www-user.tu-chemnitz.de/~heha/petzold/ch06c.htm
        // http://forums.codeguru.com/showthread.php?49901-Help!-How-to-send-a-CTRL-X-key-to-another-window
        public static bool PostMessageKey(IntPtr hWnd, Key key, bool alt = false, bool ctrl = false, bool shift = false, int delay = 50)
        {
            CheckKeyShiftState();

            // Key down shift, ctrl, and/or alt
            if (alt)
            {
                if (!PostMessageKeyDown(hWnd, VKeys.KEY_MENU)) return false;
            }
            if (ctrl)
            {
                if (!PostMessageKeyDown(hWnd, VKeys.KEY_CONTROL)) return false;
            }
            if (shift)
            {
                if (!PostMessageKeyDown(hWnd, VKeys.KEY_SHIFT)) return false;
            }

            if (!PostMessageSafe(hWnd, (int)Message.VM_CHAR, (uint)key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0)))
            {
                return false;
            }
            Thread.Sleep(delay);

            // Key up shift, ctrl, and/or alt
            if (alt)
            {
                if (!PostMessageKeyUp(hWnd, VKeys.KEY_MENU)) return false;
            }
            if (ctrl)
            {
                if (!PostMessageKeyUp(hWnd, VKeys.KEY_CONTROL)) return false;
            }
            if (shift)
            {
                if (!PostMessageKeyUp(hWnd, VKeys.KEY_SHIFT)) return false;
            }

            return true;
        }

        private static bool SendMessageKeyUp(IntPtr hWnd, VKeys key)
        {
            IntPtr result = MessagingApi.SendMessage(hWnd, (int)Message.KEY_UP, (uint)key, GetLParam(1, key, 0, 0, 1, 1));
            //return result != IntPtr.Zero;
            return true;
        }

        private static bool SendMessageKeyDown(IntPtr hWnd, VKeys key)
        {
            IntPtr result = MessagingApi.SendMessage(hWnd, (int)Message.KEY_DOWN, (uint)key, GetLParam(1, key, 0, 0, 0, 0));
            //return result != IntPtr.Zero;
            return true;
        }

        public static bool SendMessageKeyDown(IntPtr hWnd, Key key, bool checkKeyboardState, int delay = 100)
        {
            if (checkKeyboardState)
            {
                CheckKeyShiftState();
            }

            //Send KEY_DOWN
            if (SendMessageKeyDown(hWnd, key.Vk))
            {
                Thread.Sleep(delay);
                return true;
            }

            return false;
        }

        public static bool SendMessageKeyUp(IntPtr hWnd, Key key, bool checkKeyboardState, int delay = 100)
        {
            if (checkKeyboardState)
            {
                CheckKeyShiftState();
            }

            //Send KEY_UP
            if (SendMessageKeyUp(hWnd, key.Vk))
            {
                Thread.Sleep(delay);
                return true;
            }

            return false;
        }

        public static bool SendChar(IntPtr hWnd, char c, bool checkKeyboardState)
        {
            if (checkKeyboardState)
                CheckKeyShiftState();

            //Send VM_CHAR
            IntPtr result = MessagingApi.SendMessage(hWnd, (int)Message.VM_CHAR, c, 0);
            return result != IntPtr.Zero;
        }

        public static bool SendMessage(IntPtr hWnd, Key key, bool checkKeyboardState, int delay = 100)
        {
            if (checkKeyboardState)
                CheckKeyShiftState();

            //Send KEY_DOWN
            if (!SendMessageKeyDown(hWnd, key.Vk))
            {
                return false;
            }
            Thread.Sleep(delay);

            //Send VM_CHAR
            //IntPtr result = MessagingApi.SendMessage(hWnd, (int)Message.VM_CHAR, (uint)key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0));
            //if (result == IntPtr.Zero)
            //{
            //    return false;
            //}
            Thread.Sleep(delay);

            //Send KEY_UP
            if (!SendMessageKeyUp(hWnd, key.Vk))
            {
                return false;
            }
            Thread.Sleep(delay);

            return true;
        }

        public static bool SendMessageKey(IntPtr hWnd, Key key, bool alt = false, bool ctrl = false, bool shift = false, int delay = 100)
        {
            CheckKeyShiftState();

            // Key down shift, ctrl, and/or alt
            if (alt)
            {
                if (!SendMessageKeyDown(hWnd, VKeys.KEY_MENU)) return false;
            }
            if (ctrl)
            {
                if (!SendMessageKeyDown(hWnd, VKeys.KEY_CONTROL)) return false;
            }
            if (shift)
            {
                if (!SendMessageKeyDown(hWnd, VKeys.KEY_SHIFT)) return false;
            }

            //Send VM_CHAR
            IntPtr result = MessagingApi.SendMessage(hWnd, (int)Message.VM_CHAR, (uint)key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0));
            //if (result == IntPtr.Zero)
            //{
            //    return false;
            //}
            Thread.Sleep(delay);

            // Key down shift, ctrl, and/or alt
            if (alt)
            {
                if (!SendMessageKeyUp(hWnd, VKeys.KEY_MENU)) return false;
            }
            if (ctrl)
            {
                if (!SendMessageKeyUp(hWnd, VKeys.KEY_CONTROL)) return false;
            }
            if (shift)
            {
                if (!SendMessageKeyUp(hWnd, VKeys.KEY_SHIFT)) return false;
            }

            return true;
        }

        public static void CheckKeyShiftState()
        {
            while ((MessagingApi.GetKeyState((int)VKeys.KEY_MENU) & KEY_PRESSED) == KEY_PRESSED ||
                   (MessagingApi.GetKeyState((int)VKeys.KEY_CONTROL) & KEY_PRESSED) == KEY_PRESSED ||
                   (MessagingApi.GetKeyState((int)VKeys.KEY_SHIFT) & KEY_PRESSED) == KEY_PRESSED)
            {
                Thread.Sleep(1);
            }
        }

        #endregion

        #region Private

        private static bool PostMessageSafe(IntPtr hWnd, uint msg, uint wParam, uint lParam)
        {
            bool returnValue = MessagingApi.PostMessage(hWnd, msg, wParam, lParam);
            if (!returnValue)
            {
                // An error occured
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return true;
        }
        
        private static uint GetScanCode(VKeys key)
        {
            return MapVirtualKey((uint)key, (uint)MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC_EX);
        }

        //private static UInt32 GetDwExtraInfo(Int16 repeatCount, VKeys key, byte extended, byte contextCode, byte previousState,
        //    byte transitionState)
        //{
        //    var lParam = (uint)repeatCount;
        //    uint scanCode = MapVirtualKey((uint)key, (uint)MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC_EX) + 0x80;
        //    lParam += scanCode * 0x10000;
        //    lParam += (uint)((extended) * 0x1000000);
        //    lParam += (uint)((contextCode * 2) * 0x10000000);
        //    lParam += (uint)((previousState * 4) * 0x10000000);
        //    lParam += (uint)((transitionState * 8) * 0x10000000);
        //    return lParam;
        //}

        private static UInt32 GetLParam(int x, int y)
        {
            return (uint)((y << 16) | (x & 0xFFFF));
        }

        private static UInt32 GetLParam(Int16 repeatCount, VKeys key, byte extended, byte contextCode, byte previousState,
            byte transitionState)
        {
            uint scanCode = GetScanCode(key);

            var lParam = (uint)repeatCount;
            lParam += (scanCode * 0x10000);
            lParam += (uint)(extended * 0x1000000);
            lParam += (uint)(contextCode * 2 * 0x10000000);
            lParam += (uint)(previousState * 4 * 0x10000000);
            lParam += (uint)(transitionState * 8 * 0x10000000);
            return lParam;
        }

        #endregion Private

        #endregion //Methods
    }
}
