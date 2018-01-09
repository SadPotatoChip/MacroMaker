using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace MacroMaker {
	class FileScanner {
		[DllImport("user32.dll")]
		public static extern byte VkKeyScan(char ch);

		string fileName;

		Stack<char> keyDown;

		int skipChar = 0;

		public FileScanner(string fileName) {
			this.fileName = fileName;
		}

		public LinkedList<Command> commandListFromFile() {
			LinkedList<Command>lltmp = new LinkedList<Command>();
			string[] lines;
			try {
				lines = System.IO.File.ReadAllLines(fileName);
			} catch (Exception) {
				Console.WriteLine("Error: file " + fileName + "not found in program directory");
				return lltmp;
			}

			foreach (string line in lines) {
				lltmp.AddLast(commandFromLine(line));
			}
			return lltmp;
		}

		private Command commandFromLine(string line) {
			int mode = 0;
			string description = "";
			string[] tokens=new string[0];
			CommandToken[] combination= new CommandToken[0];
			char[] c = line.ToCharArray();
			keyDown = new Stack<char>();

			LinkedList<string> tokensList = new LinkedList<string>();
			LinkedList<CommandToken> commandList = new LinkedList<CommandToken>();
			for (int i=0; i < c.Length; i++) { 
				if (c[i] == '=') {
					mode++;
				} else {
					switch (mode) {
						case 0:
							description += c[i];
							break;
						case 1:
							if (skipChar <= 0) {
								string tmp = tokenParse(c[i], tokensList,i,c);
								if (tmp != "") {
									tokensList.AddLast(tmp);
								}
							} else {
								skipChar--;
							}
							break;
						case 2:
							if (skipChar <= 0) {
								commandParse(c[i], commandList, i, c, "press");
							} else {
								skipChar--;
							}
							break;
						default:
							wrongInput(line);
							return new Command();
					}
				}
			}
			tokens = tokensList.ToArray<string>();
			combination = commandList.ToArray<CommandToken>();

			Command cmd = new Command(description, tokens, combination);
			Console.Out.WriteLine(cmd.ToString());
			return cmd;
		}

		private string tokenParse(char c, LinkedList<string> tokensList, int i, char[] line) {
			string s = "";
			if (c != ' ') {
				switch (c) {
					case '^':
						s = Program.lctrlCode;
						break;
					case '#':
						s = Program.winCode;
						break;
					case '+':
						s = Program.shiftCode;
						break;
					case '~':
						s = Program.tildeCode;
						break;
					case '|':
						s = Program.tabCode;
						break;
					case '>':
						s = Program.enterCode;
						break;
					case '<':
						s = Program.backspaceCode;
						break;
					case '_':
						s = Program.spaceCode;
						break;
					case ',':
						s = Program.commaCode;
						break;
					case '.':
						s = Program.periodCode;
						break;
					case '/':
						s = Program.forwardslashCode;
						break;
					case ';':
						s = Program.semicolonCode;
						break;
					case '\'':
						s = Program.apostropheCode;
						break;
					case '[':
						s = Program.openBrCode;
						break;
					case ']':
						s = Program.closeBrCode;
						break;
					case '-':
						s = Program.minusCode;
						break;
					case '=':
						s = Program.plusCode;
						break;
					case '\\':
						s = Program.backslashCode;
						break;
					case '&':
						skipChar = 1;
						switch (line[i + 1]) {
							case '1':
								s = Program.deleteCode;
								break;
							case '2':
								s = Program.insertCode;
								break;
							case '3':
								s = Program.prtscrCode;
								break;
							case '4':
								s = Program.endCode;
								break;
							case '5':
								s = Program.homeCode;
								break;
							case '6':
								s = Program.scrollCode;
								break;
							case '7':
								s = Program.pgDownCode;
								break;
							case '8':
								s = Program.pgupCode;
								break;
							case '9':
								s = Program.pauseCode;
								break;
						}
						break;
					case '$':
						skipChar = 1;
						s = "D" + line[i + 1];
						break;
					case '%':
						skipChar = 2;
						if (line[i + 1] != '0')
							s = "F" + line[i + 1] + line[i + 2];
						else
							s = "F" + line[i + 2];
						break;
					default:
						s += c;
						break;
				}
			}

			return s;
		}
	
		private bool commandParse(char c, LinkedList<CommandToken> commandList, int i, char[] line, string pressType) {
			CommandToken ct=null;
			if(pressType=="up" && (c=='(' || c == ')')){
				Console.Out.WriteLine("Error in input file: bad brackets at "+ line[i-1] +""+line[i]);
				return false;
			}
			if (c != ' ') {
				switch (c) {
					case '^':
						ct = new CommandToken(VirtualKeyCode.CONTROL, pressType);
						break;
					case '#':
						ct = new CommandToken(VirtualKeyCode.LWIN, pressType);
						break;
					case '+':
						ct = new CommandToken(VirtualKeyCode.SHIFT, pressType);
						break;
					case '_':
						ct = new CommandToken(VirtualKeyCode.SPACE, pressType);
						break;
					case '<':
						ct = new CommandToken(VirtualKeyCode.BACK, pressType);
						break;
					case '>':
						ct = new CommandToken(VirtualKeyCode.RETURN, pressType);
						break;
					case '|':
						ct = new CommandToken(VirtualKeyCode.CAPITAL, pressType);
						break;
					case ',':
						ct = new CommandToken(VirtualKeyCode.OEM_COMMA, pressType);
						break;
					case '.':
						ct = new CommandToken(VirtualKeyCode.OEM_PERIOD, pressType);
						break;
					case '/':
						ct = new CommandToken((VirtualKeyCode)191, pressType);
						break;
					case ';':
						ct = new CommandToken((VirtualKeyCode)186, pressType);
						break;
					case '\'':
						ct = new CommandToken((VirtualKeyCode)222, pressType);
						break;
					case '[':
						ct = new CommandToken((VirtualKeyCode)219, pressType);
						break;
					case ']':
						ct = new CommandToken((VirtualKeyCode)221, pressType);
						break;
					case '-':
						ct = new CommandToken(VirtualKeyCode.OEM_MINUS, pressType);
						break;
					case '\\':
						ct = new CommandToken((VirtualKeyCode)220, pressType);
						break;
					case '&':
						skipChar = 1;
						switch (line[i + 1]) {
							case '1':
								ct = new CommandToken(VirtualKeyCode.DELETE, pressType);
								break;
							case '2':
								ct = new CommandToken(VirtualKeyCode.INSERT, pressType);
								break;
							case '3':
								ct = new CommandToken(VirtualKeyCode.PRINT, pressType);
								break;
							case '4':
								ct = new CommandToken(VirtualKeyCode.END, pressType);
								break;
							case '5':
								ct = new CommandToken(VirtualKeyCode.HOME, pressType);
								break;
							case '6':
								ct = new CommandToken(VirtualKeyCode.SCROLL, pressType);
								break;
							case '7':
								ct = new CommandToken((VirtualKeyCode)34, pressType);
								break;
							case '8':
								ct = new CommandToken((VirtualKeyCode)33, pressType);
								break;
							case '9':
								ct = new CommandToken(VirtualKeyCode.PAUSE, pressType);
								break;
						}
						break;
					case '!':
						skipChar = 1;
						switch (line[i + 1]) {
							case 'L':
								ct = new CommandToken(VirtualKeyCode.LEFT, pressType);
								break;
							case 'R':
								ct = new CommandToken(VirtualKeyCode.RIGHT, pressType);
								break;
							case 'U':
								ct = new CommandToken(VirtualKeyCode.UP, pressType);
								break;
							case 'D':
								ct = new CommandToken(VirtualKeyCode.DOWN, pressType);
								break;
						}
						break;
					case '@':
						skipChar = 1;
						switch (line[i + 1]) {
							case '1':
								ct = new CommandToken("LMB", pressType);
								break;
							case '2':
								ct = new CommandToken("RMB", pressType);
								break;
						}
						break;
					case '(':
						CommandToken tmp = commandList.Last<CommandToken>();
						tmp.changePressType("down");
						keyDown.Push(line[i - 1]);
						break;
					case ')':
						commandParse(keyDown.Pop(), commandList, i, line, "up");
						break;
					case '?':
						int n = 2;
						if (line[i + 1] == '{') {
							string time = "";
							while (line[i + n] != '}') {
								time += line[i + n];
								n++;
							}
							ct = new CommandToken(int.Parse(time));
							skipChar = n;
						} else {
							ct = new CommandToken();
						}
						break;
					case '*':
						int m = 2;
						if (line[i + 1] == '{') {
							string x = "";
							string y = "";
							while (line[i + m] != ',') {
								x += line[i + m];
								m++;
							}
							m++;
							while (line[i + m] != '}') {
								y += line[i + m];
								m++;
							}
							ct = new CommandToken(int.Parse(x)*1000, int.Parse(y)*1000);
							skipChar = m;
						} else {
							ct = new CommandToken();
						}
						break;
					default:
						ct = new CommandToken((VirtualKeyCode)VkKeyScan(c), pressType);
						break;
				}
			}
			if(ct!= null)
				commandList.AddLast(ct);
			return true;
		}

		private void wrongInput(string line) {
			Console.WriteLine("Error in input file on line: \n"+ line);
		}
	}
}
