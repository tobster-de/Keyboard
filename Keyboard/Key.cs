using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;

namespace Keyboard
{
	/// <summary>An individual keyboard key.</summary>
	[Serializable]
	public class Key : UITypeEditor
	{
        [StructLayout(LayoutKind.Explicit)]
        struct Helper
        {
            [FieldOffset(0)]
            public short Value;
            [FieldOffset(0)]
            public byte Low;
            [FieldOffset(1)]
            public byte High;
        }

        /// <summary>The shift key's virtual key code.</summary>
        public Messaging.VKeys ShiftKey { get; set; }

		/// <summary>The shift type (alt, ctrl, shift).</summary>
		public Messaging.ShiftType ShiftType { get; set; }

		/// <summary>The virtual key associated with it.</summary>
		public Messaging.VKeys Vk { get; set; }

		/// <summary>
		///     An internal counter used to count the number of attempts a button has tried to be pressed to exit after 4
		///     attempts.
		/// </summary>
		private int buttonCounter;

		/// <summary>Default constructor</summary>
		public Key(Messaging.VKeys vk = Messaging.VKeys.Null, Messaging.VKeys shiftKey = Messaging.VKeys.Null, Messaging.ShiftType shiftType = Messaging.ShiftType.None)
		{
		    this.buttonCounter = 0;
		    this.Vk = vk;
		    this.ShiftKey = shiftKey;
		    this.ShiftType = shiftType;
		}

		public Key(char c)
		{
		    this.buttonCounter = 0;

            var helper = new Helper { Value = Messaging.VkKeyScan(c) };

		    this.Vk = (Messaging.VKeys)helper.Low;
		    this.ShiftKey = Messaging.VKeys.Null;
		    this.ShiftType = (Messaging.ShiftType)helper.High;
		}

		/// <summary>Constructor if you already have a whole key.  Good for making a dereferenced copy.</summary>
		/// <param name="key">The already built key.</param>
		public Key(Key key)
		{
		    this.buttonCounter = 0;
		    this.Vk = key.Vk;
		    this.ShiftKey = key.ShiftKey;
		    this.ShiftType = key.ShiftType;
		}

		/// <summary>Emulates a keyboard key press.</summary>
		/// <param name="hWnd">The handle to the window that will receive the key press.</param>
		/// <param name="foreground">Whether it should be a foreground key press or a background key press.</param>
		/// <returns>If the press succeeded or failed.</returns>
		public bool Press(IntPtr hWnd, bool foreground)
		{
			if (foreground)
				return this.PressForeground(hWnd);

			return this.PressBackground(hWnd);
		}

		public bool Down(IntPtr hWnd, bool foreground)
		{
			switch (this.ShiftType)
			{
				case Messaging.ShiftType.None:
					if (foreground)
					{
						if (!Messaging.ForegroundKeyDown(hWnd, this))
						{
						    this.buttonCounter++;
							if (this.buttonCounter == 2)
							{
							    this.buttonCounter = 0;
								return false;
							}
						    this.Down(hWnd, true);
						}
					}
					else
					{
						if (!Messaging.SendMessageDown(hWnd, this, true))
						{
						    this.buttonCounter++;
							if (this.buttonCounter == 2)
							{
							    this.buttonCounter = 0;
								return false;
							}
						    this.Down(hWnd, false);
						}
					}
					return true;
			}
			return true;
		}

		public bool Up(IntPtr hWnd, bool foreground)
		{
			switch (this.ShiftType)
			{
				case Messaging.ShiftType.None:
					if (foreground)
					{
						if (!Messaging.ForegroundKeyUp(hWnd, this))
						{
						    this.buttonCounter++;
							if (this.buttonCounter == 2)
							{
							    this.buttonCounter = 0;
								return false;
							}
						    this.Up(hWnd, foreground);
						}
					}
					else
					{
						if (!Messaging.SendMessageUp(hWnd, this, true))
						{
						    this.buttonCounter++;
							if (this.buttonCounter == 2)
							{
							    this.buttonCounter = 0;
								return false;
							}
						    this.Up(hWnd, foreground);
						}
					}
					return true;
			}
			return true;
		}

		public bool PressForeground()
		{
			switch (this.ShiftType)
			{
				case Messaging.ShiftType.None:
					if (!Messaging.ForegroundKeyPress(this))
					{
					    this.buttonCounter++;
						if (this.buttonCounter == 2)
						{
						    this.buttonCounter = 0;
							return false;
						}
					    this.PressForeground();
					}
					return true;
			}
			return true;
		}

		/// <summary>Emulates a background keyboard key press.</summary>
		/// <param name="hWnd">The handle to the window that will receive the key press.</param>
		/// <returns>If the key press succeeded or failed.</returns>
		public bool PressBackground(IntPtr hWnd)
		{
			bool alt = false, ctrl = false, shift = false;
			switch (this.ShiftType)
			{
				case Messaging.ShiftType.Alt:
					alt = true;
					break;
				case Messaging.ShiftType.Ctrl:
					ctrl = true;
					break;
				case Messaging.ShiftType.None:
					if (!Messaging.SendMessage(hWnd, this, true))
					{
					    this.buttonCounter++;
						if (this.buttonCounter == 2)
						{
						    this.buttonCounter = 0;
							return false;
						}
					    this.PressBackground(hWnd);
					}
					return true;
				case Messaging.ShiftType.Shift:
					shift = true;
					break;
			}
			if (!Messaging.SendMessageAll(hWnd, this, alt, ctrl, shift))
			{
			    this.buttonCounter++;
				if (this.buttonCounter == 2)
				{
				    this.buttonCounter = 0;
					return false;
				}
			    this.PressBackground(hWnd);
			}
			return true;
		}

		/// <summary>Emulates a foreground key press.</summary>
		/// <param name="hWnd">The handle to the window that will receive the key press.</param>
		/// <returns>Returns whether the key succeeded to be pressed or not.</returns>
		public bool PressForeground(IntPtr hWnd)
		{
			bool alt = false, ctrl = false, shift = false;
			switch (this.ShiftType)
			{
				case Messaging.ShiftType.Alt:
					alt = true;
					break;
				case Messaging.ShiftType.Ctrl:
					ctrl = true;
					break;
				case Messaging.ShiftType.None:
					if (!Messaging.ForegroundKeyPress(hWnd, this))
					{
					    this.buttonCounter++;
						if (this.buttonCounter == 2)
						{
						    this.buttonCounter = 0;
							return false;
						}
					    this.PressForeground(hWnd);
					}
					return true;
				case Messaging.ShiftType.Shift:
					shift = true;
					break;
			}
			if (!Messaging.ForegroundKeyPressAll(hWnd, this, alt, ctrl, shift))
			{
			    this.buttonCounter++;
				if (this.buttonCounter == 2)
				{
				    this.buttonCounter = 0;
					return false;
				}
			    this.PressForeground(hWnd);
			}
			return true;
		}

		/// <summary>Allows the property grid edit form.</summary>
		/// <param name="context">The style the editor takes.</param>
		/// <returns>The drop down style.</returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		/// <summary>Allows the property grid drop down.</summary>
		/// <param name="context">The context for the type.</param>
		/// <param name="provider">The service provider.</param>
		/// <param name="value">The value that the object has.</param>
		/// <returns></returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			var wfes = provider.GetService(
				typeof (IWindowsFormsEditorService)) as
				IWindowsFormsEditorService;

			if (wfes == null) if (value != null) return value;
			var setKey = new SetKey((Key) value);

			if (wfes != null) wfes.DropDownControl(setKey);
			value = setKey.Key;

			return value;
		}

		/// <summary>Override to return the key's string</summary>
		/// <returns>Returns the proper string.</returns>
		public override string ToString()
		{
			return string.Format("{0} {1}", this.ShiftType, this.Vk);
		}
	}
}