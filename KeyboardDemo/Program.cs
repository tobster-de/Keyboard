using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Keyboard;

namespace KeyboardDemo
{
	class Program
	{
        // Get a handle to an application window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);

        static void Main(string[] args)
		{
            //var p = new Process {StartInfo = {FileName = "notepad.exe"}};
            //p.Start();

            // Get a handle to the Calculator application. The window class
            // and window name were obtained using the Spy++ tool.
            IntPtr handle = FindWindow("CalcFrame", "Calculator");

            try
			{
				List<Key> keys = "111*11=".Select(c => new Key(c)).ToList();
				//var procId = p.Id;
				//Console.WriteLine("ID: " + procId);
				Console.WriteLine("Sending background keypresses to write \"hello world\"");
				//p.WaitForInputIdle();
				foreach (var key in keys)
				{
				    key.PressForeground(handle); //p.MainWindowHandle);
				}

			}
			catch (InvalidOperationException)
			{
			}
		}
	}
}
