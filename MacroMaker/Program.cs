using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Media;
using WindowsInput.Native;

namespace MacroMaker {

	class Program {
		private const int SW_HIDE = 0;
		private const int SW_SHOW = 5;

		private static int WH_KEYBOARD_LL = 13;
		private const int WM_KEYDOWN = 0x0100;
		private const int WM_KEYUP = 0x0101;
		public static int visible = SW_SHOW;
		private static bool isStarting=true;

		public static Process thisProcess;

			public static string lctrlCode = "LControlKey";
			public static string tildeCode = "Oemtilde";
			public static string winCode = "LWin";
			public static string shiftCode = "LShiftKey";
			public static string tabCode = "Tab";
			public static string enterCode = "Return";
			public static string backspaceCode = "Back";
			public static string capslockCode = "Capital";

			public static string commaCode = "Oemcomma";
			public static string periodCode = "OemPeriod";
			public static string forwardslashCode = "OemQuestion";
			public static string semicolonCode = "Oem1";
			public static string apostropheCode = "Oem7";
			public static string openBrCode = "OemOpenBrackets";
			public static string closeBrCode = "Oem6";
			public static string minusCode = "OemMinus";
			public static string plusCode = "OemPlus";
			public static string backslashCode = "Oem5";

			public static string insertCode = "Insert";
			public static string deleteCode = "Delete";
			public static string homeCode = "Home";
			public static string endCode = "End";
			public static string pgupCode = "PageUp";
			public static string pgDownCode = "Next";
			public static string prtscrCode = "PrintScreen";
			public static string scrollCode = "Scroll";
			public static string pauseCode = "Pause";

		public static string spaceCode = "Space";


		public static bool programEnabled = true;
		public static bool readInput = true;


		private static IntPtr hook = IntPtr.Zero;
		private static LowLevelKeyboardProc llkProcedure = HookCallback;

		public static CommandList cmdList;

		private static Command currentCommand;


		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		static void Main(string[] args) {
			currentCommand =new Command();
			cmdList = new CommandList();


			hook = SetHook(llkProcedure);
			thisProcess = Process.GetCurrentProcess();
			toogleVisibility();

			Application.Run();
			UnhookWindowsHookEx(hook);

		}

		/// <summary>
		///	Called on key event.
		/// </summary>
		private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
			if (programEnabled) {
				if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN) {
				int vkCode = Marshal.ReadInt32(lParam);
				currentCommand.addToken(vkCode);
				}
				if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP) {
					int vkCode = Marshal.ReadInt32(lParam);
					currentCommand.removeToken(vkCode);
				}
				Console.Out.WriteLine(currentCommand.ToString());

				if (readInput) {
					readInput = false;
					if (cmdList.checkIfCommandInList(currentCommand)) {
						currentCommand = new Command();
						System.Threading.Thread.Sleep(200);
					}
					readInput = true;
				}
			}

			if(currentCommand.compareTo(new Command("toogleVisibility", 
				new string[] { lctrlCode,shiftCode, "F10"},
				new CommandToken[]{ new CommandToken()})) && readInput) {
				readInput = false;
				Console.Out.WriteLine("visibility toogled");
				Program.toogleVisibility();
				readInput = true;
			}
			if (currentCommand.compareTo(new Command("reload txt",
					new string[] { lctrlCode, shiftCode, "F11" },
					new CommandToken[] { new CommandToken() })) && readInput) {
				readInput = false;
				Console.Out.WriteLine("command list refreshed");
				currentCommand = new Command();
				cmdList = new CommandList();
				readInput = true;
			}

			return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
		}



		private static IntPtr SetHook(LowLevelKeyboardProc proc) {
			Process currentProcess = Process.GetCurrentProcess();
			ProcessModule currentModule = currentProcess.MainModule;
			String moduleName = currentModule.ModuleName;
			IntPtr moduleHandle = GetModuleHandle(moduleName);
			return SetWindowsHookEx(WH_KEYBOARD_LL, llkProcedure, moduleHandle, 0);
		}

		[DllImport("user32.dll")]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll")]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetModuleHandle(String lpModuleName);

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);



		//fordebuging
		private static void outputKeyToConsole(int vkCode) {
			if (((Keys)vkCode).ToString() == "OemPeriod") {
				Console.Out.Write(".");
			} else if (((Keys)vkCode).ToString() == "Oemcomma") {
				Console.Out.Write(",");
			} else if (((Keys)vkCode).ToString() == "Space") {
				Console.Out.Write(" ");
			} else {
				Console.Out.Write((Keys)vkCode);
			}
		}

		public static void toogleVisibility() {
			if (visible == SW_HIDE) {
				visible = SW_SHOW;
				ShowWindow(thisProcess.MainWindowHandle, SW_SHOW);
			} else {
				visible = SW_HIDE;
				ShowWindow(thisProcess.MainWindowHandle, SW_HIDE);
			}
		}


	}
}