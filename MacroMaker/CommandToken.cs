using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace MacroMaker {
	class CommandToken {
		public string commandType;
		public static InputSimulator inSim;

		public VirtualKeyCode keyToPress;
		public string mouseButton;
		public string pressType;
		public int pauseTime;

		public double moveMouseToX, moveMouseToY;


		public CommandToken(VirtualKeyCode vkc, string pressType) {
			commandType = "key";
			keyToPress = vkc;
			this.pressType = pressType;

			inSim = new InputSimulator();
		}

		public CommandToken(string mouseButton, string pressType) {
			commandType = "mouseBtn";
			this.mouseButton = mouseButton;
			this.pressType = pressType;

			inSim = new InputSimulator();
		}

		public CommandToken() {
			commandType = "break";
			pauseTime = 200;
			inSim = new InputSimulator();
		}


		public CommandToken(int pauseTime) {
			commandType = "break";
			this.pauseTime = pauseTime;
			inSim = new InputSimulator();
		}

		public CommandToken(double x, double y) {
			commandType = "mouseMove";
			moveMouseToX = x;
			moveMouseToY = y;
			inSim = new InputSimulator();
		}

		public void changePressType(string pressType) {
			this.pressType = pressType;
		}


		public void executeToken() {
			if (commandType == "key") {
				switch (pressType) {
					case "down":
						inSim.Keyboard.KeyDown(keyToPress);
						break;
					case "up":
						inSim.Keyboard.KeyUp(keyToPress);
						break;
					case "press":
						inSim.Keyboard.KeyDown(keyToPress);
						inSim.Keyboard.KeyUp(keyToPress);
						break;
				}
			}
			if (commandType== "mouseBtn") {
				handleMouseButton();
			}
			if (commandType == "break") {
				inSim.Keyboard.Sleep(pauseTime);
			}
			if (commandType == "mouseMove") {
				inSim.Mouse.MoveMouseTo(moveMouseToX, moveMouseToY);
			}

		}

		private void handleMouseButton() {
			switch (pressType) {
				case "up":
					switch (mouseButton) {
						case "LMB":
							inSim.Mouse.LeftButtonUp();
							break;
						case "RMB":
							inSim.Mouse.RightButtonUp();
							break;
					}
					break;
				case "down":
					switch (mouseButton) {
						case "LMB":
							inSim.Mouse.LeftButtonDown();
							break;
						case "RMB":
							inSim.Mouse.RightButtonDown();
							break;
					}
					break;
				case "press":
					switch (mouseButton) {
						case "LMB":
							inSim.Mouse.LeftButtonClick();
							break;
						case "RMB":
							inSim.Mouse.RightButtonClick();
							break;
					}
					break;
			}
		}

		public string ToString() {
			string s = "";
			if (commandType != null) {
				if (commandType == "key") {
					s += commandType;
					s += keyToPress;
					if (pressType != null) {
						s += pressType;
					}
				} else if (commandType == "mouseBtn") {
					s += commandType;
					s += mouseButton;
					if (pressType != null) {
						s += pressType;
					}
				} else if(commandType == "mouseMove") {
					s += "mouseto(" + moveMouseToX+","+moveMouseToY+")";
				} else {
					s += "pauseFor" + pauseTime;
				}
			}
			return s;
		}

	}
}
