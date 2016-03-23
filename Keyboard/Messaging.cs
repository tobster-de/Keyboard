using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Keyboard
{
	/// <summary>Class for messaging and key presses</summary>
	[Serializable]
	public class Messaging
	{
		#region Unmanaged Items

		#region Constants

		/// <summary>Maps a virtual key to a key code.</summary>
		private const uint MapvkVkToVsc = 0x00;

		/// <summary>Maps a key code to a virtual key.</summary>
		private const uint MapvkVscToVk = 0x01;

		/// <summary>Maps a virtual key to a character.</summary>
		private const uint MapvkVkToChar = 0x02;

		/// <summary>Maps a key code to a virtual key with specified keyboard.</summary>
		private const uint MapvkVscToVkEx = 0x03;

		/// <summary>Maps a virtual key to a key code with specified keyboard.</summary>
		private const uint MapvkVkToVscEx = 0x04;

		/// <summary>Code if the key is toggled.</summary>
		private const ushort KeyToggled = 0x1;

		/// <summary>Code for if the key is pressed.</summary>
		private const ushort KeyPressed = 0xF000;

		/// <summary>Code for no keyboard event.</summary>
		private const uint KeyeventfNone = 0x0;

		/// <summary>Code for extended key pressed.</summary>
		private const uint KeyeventfExtendedkey = 0x1;

		/// <summary>Code for keyup event.</summary>
		private const uint KeyeventfKeyup = 0x2;

		/// <summary>Mouse input type.</summary>
		private const int InputMouse = 0;

		/// <summary>Keyboard input type.</summary>
		private const int InputKeyboard = 1;

		/// <summary>Hardware input type.</summary>
		private const int InputHardware = 2;

        #endregion Constants

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern short VkKeyScan(char ch);

		[DllImport("user32.dll", SetLastError = false)]
		private static extern IntPtr GetMessageExtraInfo();

		/// <summary>Gets the key state of the specified key.</summary>
		/// <param name="nVirtKey">The key to check.</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		private static extern ushort GetKeyState(int nVirtKey);

		/// <summary>Gets the state of the entire keyboard.</summary>
		/// <param name="lpKeyState">The byte array to receive all the keys states.</param>
		/// <returns>Whether it succeed or failed.</returns>
		[DllImport("user32.dll")]
		private static extern bool GetKeyboardState(byte[] lpKeyState);

		/// <summary>Allows for foreground hardware keyboard key presses</summary>
		/// <param name="nInputs">The number of inputs in pInputs</param>
		/// <param name="pInputs">A Input structure for what is to be pressed.</param>
		/// <param name="cbSize">The size of the structure.</param>
		/// <returns>A message.</returns>
		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint SendInput(uint nInputs, ref Input pInputs, int cbSize);

		/// <summary>
		///     The GetForegroundWindow function returns a handle to the foreground window.
		/// </summary>
		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern bool SendMessage(IntPtr hWnd, int wMsg, uint wParam, uint lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool PostMessage(IntPtr hWnd, int msg, uint wParam, uint lParam);

		[DllImport("user32.dll")]
		private static extern uint MapVirtualKey(uint uCode, uint uMapType);

		#endregion //Unmanaged Items

		#region Structures

		#region Public

		public enum WindowsMessages
		{
			WmNull = 0x00,
			WmCreate = 0x01,
			WmDestroy = 0x02,
			WmMove = 0x03,
			WmSize = 0x05,
			WmActivate = 0x06,
			WmSetfocus = 0x07,
			WmKillfocus = 0x08,
			WmEnable = 0x0A,
			WmSetredraw = 0x0B,
			WmSettext = 0x0C,
			WmGettext = 0x0D,
			WmGettextlength = 0x0E,
			WmPaint = 0x0F,
			WmClose = 0x10,
			WmQueryendsession = 0x11,
			WmQuit = 0x12,
			WmQueryopen = 0x13,
			WmErasebkgnd = 0x14,
			WmSyscolorchange = 0x15,
			WmEndsession = 0x16,
			WmSystemerror = 0x17,
			WmShowwindow = 0x18,
			WmCtlcolor = 0x19,
			WmWininichange = 0x1A,
			WmSettingchange = 0x1A,
			WmDevmodechange = 0x1B,
			WmActivateapp = 0x1C,
			WmFontchange = 0x1D,
			WmTimechange = 0x1E,
			WmCancelmode = 0x1F,
			WmSetcursor = 0x20,
			WmMouseactivate = 0x21,
			WmChildactivate = 0x22,
			WmQueuesync = 0x23,
			WmGetminmaxinfo = 0x24,
			WmPainticon = 0x26,
			WmIconerasebkgnd = 0x27,
			WmNextdlgctl = 0x28,
			WmSpoolerstatus = 0x2A,
			WmDrawitem = 0x2B,
			WmMeasureitem = 0x2C,
			WmDeleteitem = 0x2D,
			WmVkeytoitem = 0x2E,
			WmChartoitem = 0x2F,

			WmSetfont = 0x30,
			WmGetfont = 0x31,
			WmSethotkey = 0x32,
			WmGethotkey = 0x33,
			WmQuerydragicon = 0x37,
			WmCompareitem = 0x39,
			WmCompacting = 0x41,
			WmWindowposchanging = 0x46,
			WmWindowposchanged = 0x47,
			WmPower = 0x48,
			WmCopydata = 0x4A,
			WmCanceljournal = 0x4B,
			WmNotify = 0x4E,
			WmInputlangchangerequest = 0x50,
			WmInputlangchange = 0x51,
			WmTcard = 0x52,
			WmHelp = 0x53,
			WmUserchanged = 0x54,
			WmNotifyformat = 0x55,
			WmContextmenu = 0x7B,
			WmStylechanging = 0x7C,
			WmStylechanged = 0x7D,
			WmDisplaychange = 0x7E,
			WmGeticon = 0x7F,
			WmSeticon = 0x80,

			WmNccreate = 0x81,
			WmNcdestroy = 0x82,
			WmNccalcsize = 0x83,
			WmNchittest = 0x84,
			WmNcpaint = 0x85,
			WmNcactivate = 0x86,
			WmGetdlgcode = 0x87,
			WmNcmousemove = 0xA0,
			WmNclbuttondown = 0xA1,
			WmNclbuttonup = 0xA2,
			WmNclbuttondblclk = 0xA3,
			WmNcrbuttondown = 0xA4,
			WmNcrbuttonup = 0xA5,
			WmNcrbuttondblclk = 0xA6,
			WmNcmbuttondown = 0xA7,
			WmNcmbuttonup = 0xA8,
			WmNcmbuttondblclk = 0xA9,

			WmInput = 0x00FF,

			WmKeyfirst = 0x100,
			WmKeydown = 0x100,
			WmKeyup = 0x101,
			WmChar = 0x102,
			WmDeadchar = 0x103,
			WmSyskeydown = 0x104,
			WmSyskeyup = 0x105,
			WmSyschar = 0x106,
			WmSysdeadchar = 0x107,
			WmKeylast = 0x108,

			WmImeStartcomposition = 0x10D,
			WmImeEndcomposition = 0x10E,
			WmImeComposition = 0x10F,
			WmImeKeylast = 0x10F,

			WmInitdialog = 0x110,
			WmCommand = 0x111,
			WmSyscommand = 0x112,
			WmTimer = 0x113,
			WmHscroll = 0x114,
			WmVscroll = 0x115,
			WmInitmenu = 0x116,
			WmInitmenupopup = 0x117,
			WmMenuselect = 0x11F,
			WmMenuchar = 0x120,
			WmEnteridle = 0x121,

			WmCtlcolormsgbox = 0x132,
			WmCtlcoloredit = 0x133,
			WmCtlcolorlistbox = 0x134,
			WmCtlcolorbtn = 0x135,
			WmCtlcolordlg = 0x136,
			WmCtlcolorscrollbar = 0x137,
			WmCtlcolorstatic = 0x138,

			WmMousefirst = 0x200,
			WmMousemove = 0x200,
			WmLbuttondown = 0x201,
			WmLbuttonup = 0x202,
			WmLbuttondblclk = 0x203,
			WmRbuttondown = 0x204,
			WmRbuttonup = 0x205,
			WmRbuttondblclk = 0x206,
			WmMbuttondown = 0x207,
			WmMbuttonup = 0x208,
			WmMbuttondblclk = 0x209,
			WmMousewheel = 0x20A,
			WmMousehwheel = 0x20E,

			WmParentnotify = 0x210,
			WmEntermenuloop = 0x211,
			WmExitmenuloop = 0x212,
			WmNextmenu = 0x213,
			WmSizing = 0x214,
			WmCapturechanged = 0x215,
			WmMoving = 0x216,
			WmPowerbroadcast = 0x218,
			WmDevicechange = 0x219,

			WmMdicreate = 0x220,
			WmMdidestroy = 0x221,
			WmMdiactivate = 0x222,
			WmMdirestore = 0x223,
			WmMdinext = 0x224,
			WmMdimaximize = 0x225,
			WmMditile = 0x226,
			WmMdicascade = 0x227,
			WmMdiiconarrange = 0x228,
			WmMdigetactive = 0x229,
			WmMdisetmenu = 0x230,
			WmEntersizemove = 0x231,
			WmExitsizemove = 0x232,
			WmDropfiles = 0x233,
			WmMdirefreshmenu = 0x234,

			WmImeSetcontext = 0x281,
			WmImeNotify = 0x282,
			WmImeControl = 0x283,
			WmImeCompositionfull = 0x284,
			WmImeSelect = 0x285,
			WmImeChar = 0x286,
			WmImeKeydown = 0x290,
			WmImeKeyup = 0x291,

			WmMousehover = 0x2A1,
			WmNcmouseleave = 0x2A2,
			WmMouseleave = 0x2A3,

			WmCut = 0x300,
			WmCopy = 0x301,
			WmPaste = 0x302,
			WmClear = 0x303,
			WmUndo = 0x304,

			WmRenderformat = 0x305,
			WmRenderallformats = 0x306,
			WmDestroyclipboard = 0x307,
			WmDrawclipboard = 0x308,
			WmPaintclipboard = 0x309,
			WmVscrollclipboard = 0x30A,
			WmSizeclipboard = 0x30B,
			WmAskcbformatname = 0x30C,
			WmChangecbchain = 0x30D,
			WmHscrollclipboard = 0x30E,
			WmQuerynewpalette = 0x30F,
			WmPaletteischanging = 0x310,
			WmPalettechanged = 0x311,

			WmHotkey = 0x312,
			WmPrint = 0x317,
			WmPrintclient = 0x318,

			WmHandheldfirst = 0x358,
			WmHandheldlast = 0x35F,
			WmPenwinfirst = 0x380,
			WmPenwinlast = 0x38F,
			WmCoalesceFirst = 0x390,
			WmCoalesceLast = 0x39F,
			WmDdeFirst = 0x3E0,
			WmDdeInitiate = 0x3E0,
			WmDdeTerminate = 0x3E1,
			WmDdeAdvise = 0x3E2,
			WmDdeUnadvise = 0x3E3,
			WmDdeAck = 0x3E4,
			WmDdeData = 0x3E5,
			WmDdeRequest = 0x3E6,
			WmDdePoke = 0x3E7,
			WmDdeExecute = 0x3E8,
			WmDdeLast = 0x3E8,

			WmUser = 0x400,
			WmApp = 0x8000
		}

		public struct Keybdinput
		{
			public ushort WVk;
			public ushort WScan;
			public uint DwFlags;
			public long Time;
			public uint DwExtraInfo;
		};

		[StructLayout(LayoutKind.Explicit, Size = 28)]
		public struct Input
		{
			[FieldOffset(0)] public uint type;
			[FieldOffset(4)] public Keybdinput ki;
		};

        // https://msdn.microsoft.com/en-us/library/ms646329.aspx
        [Serializable, Flags]
		public enum ShiftType
		{
			None = 0x0,
            Shift = 0x1,
			Ctrl = 0x2,
			ShiftCtrl = Shift | Ctrl,
			Alt = 0x4,
			ShiftAlt = Shift | Alt,
			CtrlAlt = Ctrl | Alt,
			ShiftCtrlAlt = Shift | Ctrl | Alt
		}

		public enum Message
		{
			Nchittest = (0x0084),
			KeyDown = (0x0100), //Key down
			KeyUp = (0x0101), //Key Up
			VmChar = (0x0102), //The character being pressed
			Syskeydown = (0x0104), //An Alt/ctrl/shift + key down message
			Syskeyup = (0x0105), //An Alt/Ctrl/Shift + Key up Message
			Syschar = (0x0106), //An Alt/Ctrl/Shift + Key character Message
			Lbuttondown = (0x201), //Left mousebutton down 
			Lbuttonup = (0x202), //Left mousebutton up 
			Lbuttondblclk = (0x203), //Left mousebutton doubleclick 
			Rbuttondown = (0x204), //Right mousebutton down 
			Rbuttonup = (0x205), //Right mousebutton up 
			Rbuttondblclk = (0x206), //Right mousebutton doubleclick

			/// <summary>Middle mouse button down</summary>
			Mbuttondown = (0x207),

			/// <summary>Middle mouse button up</summary>
			Mbuttonup = (0x208)
		}

		[Serializable]
		public enum VKeys
		{
			Key0 = 0x30, //0 key 
			Key1 = 0x31, //1 key 
			Key2 = 0x32, //2 key 
			Key3 = 0x33, //3 key 
			Key4 = 0x34, //4 key 
			Key5 = 0x35, //5 key 
			Key6 = 0x36, //6 key 
			Key7 = 0x37, //7 key 
			Key8 = 0x38, //8 key 
			Key9 = 0x39, //9 key
			KeyMinus = 0xBD, // - key
			KeyPlus = 0xBB, // + key
			KeyA = 0x41, //A key 
			KeyB = 0x42, //B key 
			KeyC = 0x43, //C key 
			KeyD = 0x44, //D key 
			KeyE = 0x45, //E key 
			KeyF = 0x46, //F key 
			KeyG = 0x47, //G key 
			KeyH = 0x48, //H key 
			KeyI = 0x49, //I key 
			KeyJ = 0x4A, //J key 
			KeyK = 0x4B, //K key 
			KeyL = 0x4C, //L key 
			KeyM = 0x4D, //M key 
			KeyN = 0x4E, //N key 
			KeyO = 0x4F, //O key 
			KeyP = 0x50, //P key 
			KeyQ = 0x51, //Q key 
			KeyR = 0x52, //R key 
			KeyS = 0x53, //S key 
			KeyT = 0x54, //T key 
			KeyU = 0x55, //U key 
			KeyV = 0x56, //V key 
			KeyW = 0x57, //W key 
			KeyX = 0x58, //X key 
			KeyY = 0x59, //Y key 
			KeyZ = 0x5A, //Z key 
			KeyLbutton = 0x01, //Left mouse button 
			KeyRbutton = 0x02, //Right mouse button 
			KeyCancel = 0x03, //Control-break processing 
			KeyMbutton = 0x04, //Middle mouse button (three-button mouse) 
			KeyBack = 0x08, //BACKSPACE key 
			KeyTab = 0x09, //TAB key 
			KeyClear = 0x0C, //CLEAR key 
			KeyReturn = 0x0D, //ENTER key 
			KeyShift = 0x10, //SHIFT key 
			KeyControl = 0x11, //CTRL key 
			KeyMenu = 0x12, //ALT key 
			KeyPause = 0x13, //PAUSE key 
			KeyCapital = 0x14, //CAPS LOCK key 
			KeyEscape = 0x1B, //ESC key 
			KeySPACE = 0x20, //SPACEBAR 
			KeyPrior = 0x21, //PAGE UP key 
			KeyNext = 0x22, //PAGE DOWN key 
			KeyEnd = 0x23, //END key 
			KeyHome = 0x24, //HOME key 
			KeyLeft = 0x25, //LEFT ARROW key 
			KeyUp = 0x26, //UP ARROW key 
			KeyRight = 0x27, //RIGHT ARROW key 
			KeyDown = 0x28, //DOWN ARROW key 
			KeySelect = 0x29, //SELECT key 
			KeyPrint = 0x2A, //PRINT key 
			KeyExecute = 0x2B, //EXECUTE key 
			KeySnapshot = 0x2C, //PRINT SCREEN key 
			KeyInsert = 0x2D, //INS key 
			KeyDelete = 0x2E, //DEL key 
			KeyHelp = 0x2F, //HELP key 
			KeyNumpad0 = 0x60, //Numeric keypad 0 key 
			KeyNumpad1 = 0x61, //Numeric keypad 1 key 
			KeyNumpad2 = 0x62, //Numeric keypad 2 key 
			KeyNumpad3 = 0x63, //Numeric keypad 3 key 
			KeyNumpad4 = 0x64, //Numeric keypad 4 key 
			KeyNumpad5 = 0x65, //Numeric keypad 5 key 
			KeyNumpad6 = 0x66, //Numeric keypad 6 key 
			KeyNumpad7 = 0x67, //Numeric keypad 7 key 
			KeyNumpad8 = 0x68, //Numeric keypad 8 key 
			KeyNumpad9 = 0x69, //Numeric keypad 9 key 
			KeySeparator = 0x6C, //Separator key 
			KeySubtract = 0x6D, //Subtract key 
			KeyDecimal = 0x6E, //Decimal key 
			KeyDivide = 0x6F, //Divide key 
			KeyF1 = 0x70, //F1 key 
			KeyF2 = 0x71, //F2 key 
			KeyF3 = 0x72, //F3 key 
			KeyF4 = 0x73, //F4 key 
			KeyF5 = 0x74, //F5 key 
			KeyF6 = 0x75, //F6 key 
			KeyF7 = 0x76, //F7 key 
			KeyF8 = 0x77, //F8 key 
			KeyF9 = 0x78, //F9 key 
			KeyF10 = 0x79, //F10 key 
			KeyF11 = 0x7A, //F11 key 
			KeyF12 = 0x7B, //F12 key 
			KeyScroll = 0x91, //SCROLL LOCK key 
			KeyLshift = 0xA0, //Left SHIFT key 
			KeyRshift = 0xA1, //Right SHIFT key 
			KeyLcontrol = 0xA2, //Left CONTROL key 
			KeyRcontrol = 0xA3, //Right CONTROL key 
			KeyLmenu = 0xA4, //Left MENU key 
			KeyRmenu = 0xA5, //Right MENU key 
			KeyComma = 0xBC, //, key
			KeyPeriod = 0xBE, //. key
			KeyPlay = 0xFA, //Play key 
			KeyZoom = 0xFB, //Zoom key 
			Null = 0x0,
		}

		#endregion //Public

		#endregion //Structures

		#region Methods

		#region Public

		public static bool GetKeyState(Key key)
		{
			if ((GetKeyState((int) key.Vk) & 0xF0) == 1)
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
			PostMessage(hWnd, (int) WindowsMessages.WmMousemove, 0, GetLParam(x, y));
		}

		public static void BackgroundMouseClick(IntPtr hWnd, Key key, int x, int y, int delay = 100)
		{
			switch (key.Vk)
			{
				case VKeys.KeyMbutton:
					PostMessage(hWnd, (int) Message.Mbuttondown, (uint) key.Vk, GetLParam(x, y));
					Thread.Sleep(delay);
					PostMessage(hWnd, (int) Message.Mbuttonup, (uint) key.Vk, GetLParam(x, y));
					break;
				case VKeys.KeyLbutton:
					PostMessage(hWnd, (int) Message.Lbuttondown, (uint) key.Vk, GetLParam(x, y));
					Thread.Sleep(delay);
					PostMessage(hWnd, (int) Message.Lbuttonup, (uint) key.Vk, GetLParam(x, y));
					break;
				case VKeys.KeyRbutton:
					PostMessage(hWnd, (int) Message.Rbuttondown, (uint) key.Vk, GetLParam(x, y));
					Thread.Sleep(delay);
					PostMessage(hWnd, (int) Message.Rbuttonup, (uint) key.Vk, GetLParam(x, y));
					break;
			}
		}

		public static void SendChatTextPost(IntPtr hWnd, string msg)
		{
			PostMessage(hWnd, new Key(VKeys.KeyReturn));
			foreach (char c in msg)
			{
				PostMessage(hWnd, new Key(c));
			}
			PostMessage(hWnd, new Key(VKeys.KeyReturn));
		}

		public static void SendChatTextSend(IntPtr hWnd, string msg)
		{
			SendMessage(hWnd, new Key(VKeys.KeyReturn), true);
			foreach (char c in msg)
			{
				SendChar(hWnd, c, true);
			}
			SendMessage(hWnd, new Key(VKeys.KeyReturn), true);
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
			uint intReturn;
			Input structInput;
			structInput = new Input();
			structInput.type = InputKeyboard;

			// Key down shift, ctrl, and/or alt
			structInput.ki.WScan = 0;
			structInput.ki.Time = 0;
			structInput.ki.DwFlags = 0;
			// Key down the actual key-code
			structInput.ki.WVk = (ushort) key.Vk;
			intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));

			// Key up shift, ctrl, and/or alt
			//keybd_event((int)key.VK, GetScanCode(key.VK) + 0x80, KEYEVENTF_NONE, 0);
			//keybd_event((int)key.VK, GetScanCode(key.VK) + 0x80, KEYEVENTF_KEYUP, 0);
			return true;
		}

		public static bool ForegroundKeyUp(Key key)
		{
			uint intReturn;
			Input structInput;
			structInput = new Input();
			structInput.type = InputKeyboard;

			// Key down shift, ctrl, and/or alt
			structInput.ki.WScan = 0;
			structInput.ki.Time = 0;
			structInput.ki.DwFlags = 0;
			// Key down the actual key-code
			structInput.ki.WVk = (ushort) key.Vk;

			// Key up the actual key-code
			structInput.ki.DwFlags = KeyeventfKeyup;
			intReturn = SendInput(1, ref structInput, Marshal.SizeOf(typeof (Input)));
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

		public static bool ForegroundKeyPressAll(IntPtr hWnd, Key key, bool alt, bool ctrl, bool shift, int delay = 100)
		{
			if (GetForegroundWindow() != hWnd)
			{
				if (!SetForegroundWindow(hWnd))
					return false;
			}
			uint intReturn;
			Input structInput;
			structInput = new Input();
			structInput.type = InputKeyboard;

			// Key down shift, ctrl, and/or alt
			structInput.ki.WScan = 0;
			structInput.ki.Time = 0;
			structInput.ki.DwFlags = 0;
			if (alt)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyMenu;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);
			}
			if (ctrl)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyControl;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);
			}
			if (shift)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyShift;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);

				if (key.ShiftKey != VKeys.Null)
				{
					structInput.ki.WVk = (ushort) key.ShiftKey;
					intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
					Thread.Sleep(delay);
				}
			}

			// Key up the actual key-code			
			ForegroundKeyPress(hWnd, key);

			structInput.ki.DwFlags = KeyeventfKeyup;
			if (shift && key.ShiftKey == VKeys.Null)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyShift;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);
			}
			if (ctrl)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyControl;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);
			}
			if (alt)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyMenu;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);
			}
			return true;
		}

		public static bool PostMessage(IntPtr hWnd, Key key, int delay = 100)
		{
			//Send KEY_DOWN
			if (PostMessage(hWnd, (int) Message.KeyDown, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0)))
				return false;
			Thread.Sleep(delay);
			//Send VM_CHAR
			if (PostMessage(hWnd, (int) Message.VmChar, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0)))
				return false;
			Thread.Sleep(delay);
			if (PostMessage(hWnd, (int) Message.KeyUp, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0)))
				return false;
			Thread.Sleep(delay);

			return true;
		}

		public static bool PostMessageAll(IntPtr hWnd, Key key, bool alt, bool ctrl, bool shift, int delay = 100)
		{
			CheckKeyShiftState();
			uint intReturn;
			Input structInput;
			structInput = new Input();
			structInput.type = InputKeyboard;

			// Key down shift, ctrl, and/or alt
			structInput.ki.WScan = 0;
			structInput.ki.Time = 0;
			structInput.ki.DwFlags = 0;
			if (alt)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyMenu;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);
			}
			if (ctrl)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyControl;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);
			}
			if (shift)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyShift;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);

				if (key.ShiftKey != VKeys.Null)
				{
					//Send KEY_DOWN
					if (PostMessage(hWnd, (int) Message.KeyDown, (uint) key.Vk, GetLParam(1, key.ShiftKey, 0, 0, 0, 0)))
						return false;
					Thread.Sleep(delay);
				}
			}

			PostMessage(hWnd, key);

			structInput.ki.DwFlags = KeyeventfKeyup;
			if (shift && key.ShiftKey == VKeys.Null)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyShift;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);
			}
			if (ctrl)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyControl;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);
			}
			if (alt)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyMenu;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);
			}

			return true;
		}

		public static bool SendMessageDown(IntPtr hWnd, Key key, bool checkKeyboardState, int delay = 100)
		{
			if (checkKeyboardState)
				CheckKeyShiftState();
			//Send KEY_DOWN
			if (SendMessage(hWnd, (int) Message.KeyDown, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0)))
				return false;
			Thread.Sleep(delay);

			//Send VM_CHAR
			if (SendMessage(hWnd, (int) Message.VmChar, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0)))
				return false;
			Thread.Sleep(delay);

			return true;
		}

		public static bool SendMessageUp(IntPtr hWnd, Key key, bool checkKeyboardState, int delay = 100)
		{
			if (checkKeyboardState)
				CheckKeyShiftState();

			//Send KEY_UP
			if (SendMessage(hWnd, (int) Message.KeyUp, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 1, 1)))
				return false;
			Thread.Sleep(delay);

			return true;
		}

		public static bool SendChar(IntPtr hWnd, char c, bool checkKeyboardState)
		{
			if (checkKeyboardState)
				CheckKeyShiftState();

			//Send VM_CHAR
			if (SendMessage(hWnd, (int) Message.VmChar, c, 0))
				return false;

			return true;
		}

		public static bool SendMessage(IntPtr hWnd, Key key, bool checkKeyboardState, int delay = 100)
		{
			if (checkKeyboardState)
				CheckKeyShiftState();

			//Send KEY_DOWN
			if (SendMessage(hWnd, (int) Message.KeyDown, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0)))
				return false;
			Thread.Sleep(delay);

			//Send VM_CHAR
			if (SendMessage(hWnd, (int) Message.VmChar, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 0, 0)))
				return false;
			Thread.Sleep(delay);

			//Send KEY_UP
			if (SendMessage(hWnd, (int) Message.KeyUp, (uint) key.Vk, GetLParam(1, key.Vk, 0, 0, 1, 1)))
				return false;
			Thread.Sleep(delay);

			return true;
		}

		public static bool SendMessageAll(IntPtr hWnd, Key key, bool alt, bool ctrl, bool shift, int delay = 100)
		{
			CheckKeyShiftState();
			uint intReturn;
			Input structInput = new Input {type = InputKeyboard, ki = {WScan = 0, Time = 0, DwFlags = 0}};

			// Key down shift, ctrl, and/or alt
			if (alt)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyMenu;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);
			}
			if (ctrl)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyControl;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);
			}
			if (shift)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyShift;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);

				if (key.ShiftKey != VKeys.Null)
				{
					//Send KEY_DOWN
					if (SendMessage(hWnd, (int) Message.KeyDown, (uint) key.Vk, GetLParam(1, key.ShiftKey, 0, 0, 0, 0)))
						return false;
					Thread.Sleep(delay);
				}
			}

			SendMessage(hWnd, key, false);

			structInput.ki.DwFlags = KeyeventfKeyup;
			if (shift && key.ShiftKey == VKeys.Null)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyShift;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);
			}
			if (ctrl)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyControl;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);
			}
			if (alt)
			{
				structInput.ki.WVk = (ushort) VKeys.KeyMenu;
				intReturn = SendInput(1, ref structInput, Marshal.SizeOf(new Input()));
				Thread.Sleep(delay);
			}

			return true;
		}

		public static void CheckKeyShiftState()
		{
			while ((GetKeyState((int) VKeys.KeyMenu) & KeyPressed) == KeyPressed ||
			       (GetKeyState((int) VKeys.KeyControl) & KeyPressed) == KeyPressed ||
			       (GetKeyState((int) VKeys.KeyShift) & KeyPressed) == KeyPressed)
			{
				Thread.Sleep(1);
			}
		}

		#endregion //Public

		#region Private

		private static uint GetScanCode(VKeys key)
		{
			return MapVirtualKey((uint) key, MapvkVkToVscEx);
		}

		private static uint GetDwExtraInfo(Int16 repeatCount, VKeys key, byte extended, byte contextCode, byte previousState,
			byte transitionState)
		{
			var lParam = (uint) repeatCount;
			uint scanCode = MapVirtualKey((uint) key, MapvkVkToVscEx) + 0x80;
			lParam += scanCode*0x10000;
			lParam += (uint) ((extended)*0x1000000);
			lParam += (uint) ((contextCode*2)*0x10000000);
			lParam += (uint) ((previousState*4)*0x10000000);
			lParam += (uint) ((transitionState*8)*0x10000000);
			return lParam;
		}

		private static uint GetLParam(int x, int y)
		{
			return (uint) ((y << 16) | (x & 0xFFFF));
		}

		private static uint GetLParam(Int16 repeatCount, VKeys key, byte extended, byte contextCode, byte previousState,
			byte transitionState)
		{
			var lParam = (uint) repeatCount;
			//uint scanCode = MapVirtualKey((uint)key, MAPVK_VK_TO_CHAR);
			uint scanCode = GetScanCode(key);
			lParam += scanCode*0x10000;
			lParam += (uint) ((extended)*0x1000000);
			lParam += (uint) ((contextCode*2)*0x10000000);
			lParam += (uint) ((previousState*4)*0x10000000);
			lParam += (uint) ((transitionState*8)*0x10000000);
			return lParam;
		}

		private static uint RemoveLeadingDigit(uint number)
		{
			return (number - ((number%(0x10000000))*(0x10000000)));
		}

		#endregion Private

		#endregion //Methods
	}
}
