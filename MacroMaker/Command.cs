using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace MacroMaker {
	class Command {

		public string description;
		/// <summary>
		/// Required input to trigger command
		/// </summary>
		public String[] tokens;
		/// <summary>
		/// Combination of commands to execute
		/// </summary>
		public CommandToken[] combination;

		

		public Command(string description,String[] line, CommandToken[] combination) {
			this.description = description;
			this.combination = combination;
			this.tokens = new String[line.Length];
			for(int i = 0; i < line.Length; i++) {
				this.tokens[i] = line[i].Trim();
			}
			
		}

		public Command() {
			this.description = "current combination";
			tokens = new String[1] { "" };
		}

		public void executeCombinations() {
				foreach(CommandToken ct in combination) {
				ct.executeToken();
			}			
		}

		public String addToken(int vkCode) {
			if (tokens[0] == "") {
				tokens[0] = ((Keys)vkCode).ToString();
			}else if (!containsKey(vkCode)) {
				String[] old = tokens;
				tokens = new String[old.Length + 1];
				for (int i = 0; i < old.Length; i++)
					tokens[i] = old[i];
				tokens[old.Length] = ((Keys)vkCode).ToString();
			}
			return this.ToString();
		}

		public String removeToken(int vkCode) {
			if (containsKey(vkCode)) {
				if (tokens.Length == 1) {
					tokens[0] = "";
				} else { 
					String[] old = tokens;
					tokens = new String[old.Length - 1];

					int i, j = 0;
					for (i = 0; i < old.Length; i++) {
						if (old[i] != ((Keys)vkCode).ToString()) {
							tokens[j] = old[i];
							j++;
						}
					}
				}
			} else {
				Console.Out.WriteLine("removeToken error on key " + ((Keys)vkCode).ToString());
			}
			return this.ToString();
		}

		public bool containsKey(int vkCode) {
			string keyString = ((Keys)vkCode).ToString();
			foreach (string token in tokens) {
				if (keyString == token) {
					return true;
				}
			}
			return false;
		}

		public bool containsKey(string keyString) {
			foreach (string token in tokens) {
				if (keyString == token) {
					return true;
				}
			}
			return false;
		}

		public bool compareTo(Command cmd) {
			String[] other = cmd.tokens;
			if (tokens.Length == other.Length) {
				foreach(string token in tokens) {
					int pos = Array.IndexOf(other, token);
					if (pos < 0) {
						return false;
					}
				}
				return true;
			} else {
				return false;
			}
		}

		public String ToString() {
			string s = "";
			s += description + " = ";
			foreach (string token in tokens) {
				s += token;
			}
			s += " = ";
			if (combination != null) { 
				foreach (CommandToken cmd in combination) { 
					s += cmd.ToString() + " ";
				}
			}
			return s;
		}
	}
}
