using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Keyboard
{
	/// <summary>The form for setting a keyboard key.</summary>
	public partial class SetKey : Form
	{
		#region Members
		/// <summary>The key being created or editted.</summary>
		private Key key { get; set; }
		#endregion Members

		#region Properties
		/// <summary>Gets or sets the internal key.</summary>
		public Key Key
		{
			get { return this.key; }
			set { this.key = value; }
		}
		#endregion Properties

		#region Constructors
		/// <summary>Default constructor, used in property grid.</summary>
		public SetKey()
		{
		    this.InitializeComponent();
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ControlBox = false;
			this.ShowInTaskbar = false;
			this.TopLevel = false;
			this.acceptBtn.Visible = false;
			this.cancelBtn.Visible = false;
			this.FormBorderStyle = FormBorderStyle.None;
		    this.key = new Key();
			foreach (Messaging.VKeys vk in Enum.GetValues(typeof(Messaging.VKeys)))
			{
				this.keyChoices.Items.Add(vk);
			}
			for (int i = 0x30; i < 0x40; i++)
			{
				this.shiftKeys.Items.Add((Messaging.VKeys)i);
			}
		}

		/// <summary>Default constructor for editting a key. Used for property grid.</summary>
		/// <param name="key">The key to be editted.</param>
		public SetKey(Key key)
		{
		    this.InitializeComponent();
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ControlBox = false;
			this.ShowInTaskbar = false;
			this.TopLevel = false;
			this.TopMost = true;
			this.acceptBtn.Visible = false;
			this.cancelBtn.Visible = false;
			this.FormBorderStyle = FormBorderStyle.None;
			this.key = key;

			foreach (Messaging.VKeys tempVk in Enum.GetValues(typeof(Messaging.VKeys)))
			{
				this.keyChoices.Items.Add(tempVk);
			}
			for (int i = 0x30; i < 0x40; i++)
			{
				this.shiftKeys.Items.Add((Messaging.VKeys)i);
			}
			if ((this.key.ShiftType & Messaging.ShiftType.Alt) == Messaging.ShiftType.Alt)
				this.altChkBox.Checked = true;
			if ((this.key.ShiftType & Messaging.ShiftType.Ctrl) == Messaging.ShiftType.Ctrl)
				this.ctrlChkBox.Checked = true;
			if ((this.key.ShiftType & Messaging.ShiftType.Shift) == Messaging.ShiftType.Shift)
				this.shiftChkBox.Checked = true;
			this.keyChoices.SelectedItem = key.Vk;
			this.shiftKeys.SelectedItem = key.ShiftKey;
		}

		/// <summary>Constructor for key bindings not in a property grid.</summary>
		/// <param name="p">Where the form should load.</param>
		/// <param name="key">The key to be editted.</param>
		public SetKey(Point p, Key key)
		{
		    this.InitializeComponent();
			this.key = key;
			this.TopMost = true;
			foreach (Messaging.VKeys tempVk in Enum.GetValues(typeof(Messaging.VKeys)))
			{
				this.keyChoices.Items.Add(tempVk);
			}
			for (int i = 0x30; i < 0x3A; i++)
			{
				this.shiftKeys.Items.Add((Messaging.VKeys)i);
			}
			if ((this.key.ShiftType & Messaging.ShiftType.Alt) == Messaging.ShiftType.Alt)
				this.altChkBox.Checked = true;
			if ((this.key.ShiftType & Messaging.ShiftType.Ctrl) == Messaging.ShiftType.Ctrl)
				this.ctrlChkBox.Checked = true;
			if ((this.key.ShiftType & Messaging.ShiftType.Shift) == Messaging.ShiftType.Shift)
				this.shiftChkBox.Checked = true;
			this.keyChoices.SelectedItem = this.key.Vk;
			this.shiftKeys.SelectedItem = key.ShiftKey;
		}
		#endregion Constructors

		#region Methods
		#region Events
		/// <summary>The event for handling the accept button press.</summary>
		/// <param name="sender">The accept button.</param>
		/// <param name="e">The button click event.</param>
		private void AcceptBtnClick(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		/// <summary>Event to handle the key change.</summary>
		/// <param name="sender">The dropdown box.</param>
		/// <param name="e">The selected index change mouse event.</param>
		private void KeyChoicesSelectedIndexChanged(object sender, EventArgs e)
		{
			this.key.Vk = (Messaging.VKeys)this.keyChoices.SelectedItem;
		}

		/// <summary>Event to handle the shift keys index change.</summary>
		/// <param name="sender">The dropdown box.</param>
		/// <param name="e">The click event.</param>
		private void ShiftKeysSelectedIndexChanged(object sender, EventArgs e)
		{
			this.key.ShiftKey = (Messaging.VKeys)this.shiftKeys.SelectedItem;
		}

		/// <summary>Event to handle the alt checkbox being enabled/disabled.</summary>
		/// <param name="sender">The alt checkbox</param>
		/// <param name="e">The event.</param>
		private void AltChkBoxCheckedChanged(object sender, EventArgs e)
		{
			if (this.altChkBox.Checked)
			{
				this.key.ShiftType |= Messaging.ShiftType.Alt;
			}
			else
			{
				this.key.ShiftType ^= Messaging.ShiftType.Alt;
			}
		}

		/// <summary>Event to handle the shift checkbox being enabled/disabled.</summary>
		/// <param name="sender">The shift checkbox</param>
		/// <param name="e">The event.</param>
		private void ShiftChkBoxCheckedChanged(object sender, EventArgs e)
		{
			if (this.shiftChkBox.Checked)
			{
				this.key.ShiftType |= Messaging.ShiftType.Shift;
				this.shiftKeys.Enabled = true;
			}
			else
			{
				this.key.ShiftType ^= Messaging.ShiftType.Shift;
				this.key.ShiftKey = Messaging.VKeys.Null;
				this.shiftKeys.Enabled = false;
			}
		}

		/// <summary>Event to handle the ctrl checkbox being enabled/disabled.</summary>
		/// <param name="sender">The ctrl checkbox</param>
		/// <param name="e">The event.</param>
		private void CtrlChkBoxCheckedChanged(object sender, EventArgs e)
		{
			if (this.ctrlChkBox.Checked)
			{
				this.key.ShiftType |= Messaging.ShiftType.Ctrl;
			}
			else
			{
				this.key.ShiftType ^= Messaging.ShiftType.Ctrl;
			}
		}

		/// <summary>Event to handle the cancel button</summary>
		/// <param name="sender">The cancel button sending the event.</param>
		/// <param name="e">The event arguments.</param>
		private void CancelBtnClick(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Abort;
			this.Close();
		}

		/// <summary>Event to handle key presses inside key choices.</summary>
		/// <param name="sender">The drop down list, combo box of key choices.</param>
		/// <param name="e">The event arguments.</param>
		private void KeyChoicesKeyDown(object sender, KeyEventArgs e)
		{
		    this.HandleKey(sender, e);
		}

		/// <summary>Event to handle key presses inside the shift key combo box.</summary>
		/// <param name="sender">The shift key combo box.</param>
		/// <param name="e">The event arguments</param>
		private void ShiftKeysKeyDown(object sender, KeyEventArgs e)
		{
		    this.HandleKey(sender, e);
		}
		#endregion Events

		#region Private
		/// <summary>Handles key presses and converts to key options.</summary>
		/// <param name="sender">The caller of this function</param>
		/// <param name="e">The key press event arguments.</param>
		private void HandleKey(object sender, KeyEventArgs e)
		{
			if (e.Alt)
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
				if (this.altChkBox.Checked)
				{
				    this.altChkBox.Checked = false;
				}
				else
				{
				    this.altChkBox.Checked = true;
				}
			}
			else if (e.Control)
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
				if (this.ctrlChkBox.Checked)
				{
				    this.ctrlChkBox.Checked = false;
				}
				else
				{
				    this.ctrlChkBox.Checked = true;
				}
			}
			else if (e.Shift)
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
				if (this.shiftChkBox.Checked)
				{
				    this.shiftChkBox.Checked = false;
				}
				else
				{
				    this.shiftChkBox.Checked = true;
				}
			}
			if (e.KeyValue != 0)
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
				ComboBox lb = sender as ComboBox;
				if (lb.Name == this.shiftKeys.Name)
				{
				    this.shiftKeys.SelectedItem = (Messaging.VKeys)e.KeyValue;
				}
				else if (lb.Name == this.keyChoices.Name)
				{
				    this.keyChoices.SelectedItem = (Messaging.VKeys)e.KeyValue;
				}
			}
		}
		#endregion Private
		#endregion Methods
	}
}
