using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroMaker {
	/// <summary>
	/// List of commands loaded from a text file
	/// </summary>
	class CommandList {

		LinkedList<Command> cmdLList;
		FileScanner fs;

		public CommandList() {
			cmdLList = generateFromFile("commands.txt");
		}

		public void addCmd(Command cmd) {
			cmdLList.AddLast(cmd);
		}

		public bool checkIfCommandInList(Command current) {
			bool ret = false;
			foreach(Command cmd in cmdLList) {
				if (current.compareTo(cmd)) {
					Console.WriteLine(cmd.description);
					cmd.executeCombinations();
					ret = true;
				}
			}
			return ret;
		}

		public LinkedList<Command> generateFromFile(string fileName) {
			fs = new FileScanner(fileName);
			return fs.commandListFromFile();
		}

	}
}
