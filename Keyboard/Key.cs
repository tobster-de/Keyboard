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
        /// <summary>
        /// Number of retries for sending a key press
        /// </summary>
        const int RetryCount = 3;

        [StructLayout(LayoutKind.Explicit)]
        private struct Helper
        {
            [FieldOffset(0)]
            public short Value;
            [FieldOffset(0)]
            public byte Low;
            [FieldOffset(1)]
            public byte High;
        }

        /// <summary>The shift key's virtual key code.</summary>
        internal MessagingApi.VKeys ShiftKey { get; set; }

        /// <summary>The shift type (alt, ctrl, shift).</summary>
        public MessagingApi.ShiftType ShiftType { get; set; }

        /// <summary>The virtual key associated with it.</summary>
        public MessagingApi.VKeys Vk { get; set; }

        /// <summary>Default constructor</summary>
        public Key(
            MessagingApi.VKeys vk = MessagingApi.VKeys.NULL,
            MessagingApi.ShiftType shiftType = MessagingApi.ShiftType.NONE)
        {
            this.Vk = vk;
            //this.ShiftKey = MessagingApi.VKeys.NULL;
            this.ShiftType = shiftType;
        }

        public Key(char c)
        {
            var helper = new Helper { Value = MessagingApi.VkKeyScan(c) };

            this.Vk = (MessagingApi.VKeys)helper.Low;
            this.ShiftKey = MessagingApi.VKeys.NULL;
            this.ShiftType = (MessagingApi.ShiftType)helper.High;
        }

        /// <summary>Constructor if you already have a whole key. Good for making a dereferenced copy.</summary>
        /// <param name="key">The already built key.</param>
        public Key(Key key)
        {
            this.Vk = key.Vk;
            this.ShiftKey = key.ShiftKey;
            this.ShiftType = key.ShiftType;
        }

        /// <summary>Emulates a keyboard key press.</summary>
        /// <param name="hWnd">The handle to the window that will receive the key press.</param>
        /// <param name="foreground">Whether it should be a foreground key press or a background key press.</param>
        /// <returns>If the press succeeded or failed.</returns>
        public bool Press(IntPtr hWnd, bool foreground = false)
        {
            if (foreground)
            {
                return this.PressForeground(hWnd);
            }

            return this.PressBackground(hWnd);
        }

        public bool Down(IntPtr hWnd, bool foreground = false)
        {
            if (this.ShiftType != MessagingApi.ShiftType.NONE)
            {
                return false;
            }

            int counter = 0;
            while (counter < RetryCount)
            {
                if (foreground)
                {
                    if (Messaging.ForegroundKeyDown(hWnd, this))
                    {
                        return true;
                    }
                }
                else
                {
                    if (!Messaging.SendMessageKeyDown(hWnd, this, true))
                    {
                        return true;
                    }
                }

                counter++;
            }

            return false;
        }

        public bool Up(IntPtr hWnd, bool foreground)
        {
            if (this.ShiftType != MessagingApi.ShiftType.NONE)
            {
                return false;
            }

            int counter = 0;
            while (counter < RetryCount)
            {
                if (foreground)
                {
                    if (Messaging.ForegroundKeyUp(hWnd, this))
                    {
                        return true;
                    }
                }
                else
                {
                    if (Messaging.SendMessageKeyUp(hWnd, this, true))
                    {
                        return true;
                    }
                }

                counter++;
            }

            return false;
        }

        public bool PressForeground()
        {
            if (this.ShiftType != MessagingApi.ShiftType.NONE)
            {
                return false;
            }

            int counter = 0;
            while (counter < RetryCount)
            {
                if (Messaging.ForegroundKeyPress(this))
                {
                    return true;
                }

                counter++;
            }

            return false;
        }

        /// <summary>Emulates a background keyboard key press.</summary>
        /// <param name="hWnd">The handle to the window that will receive the key press.</param>
        /// <returns>If the key press succeeded or failed.</returns>
        public bool PressBackground(IntPtr hWnd)
        {
            bool alt = (this.ShiftType & MessagingApi.ShiftType.ALT) != 0;
            bool ctrl = (this.ShiftType & MessagingApi.ShiftType.CTRL) != 0;
            bool shift = (this.ShiftType & MessagingApi.ShiftType.SHIFT) != 0;

            int counter = 0;

            while (counter < RetryCount)
            {
                if (Messaging.SendMessageKey(hWnd, this, alt, ctrl, shift))
                {
                    return true;
                }

                counter++;
            }

            return false;
        }

        /// <summary>Emulates a foreground key press.</summary>
        /// <param name="hWnd">The handle to the window that will receive the key press.</param>
        /// <returns>Returns whether the key succeeded to be pressed or not.</returns>
        public bool PressForeground(IntPtr hWnd)
        {
            bool alt = (this.ShiftType & MessagingApi.ShiftType.ALT) != 0;
            bool ctrl = (this.ShiftType & MessagingApi.ShiftType.CTRL) != 0;
            bool shift = (this.ShiftType & MessagingApi.ShiftType.SHIFT) != 0;

            int counter = 0;

            while (counter < RetryCount)
            {
                if (Messaging.ForegroundKeyPress(hWnd, this, alt, ctrl, shift))
                {
                    return true;
                }

                counter++;
            }

            return false;
        }

        /// <summary>Override to return the key's string</summary>
        /// <returns>Returns the proper string.</returns>
        public override string ToString() => $"{this.ShiftType} {this.Vk}";
    }
}