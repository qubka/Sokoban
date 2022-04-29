using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Sokoban {
	public class ButtonEventArgs : EventArgs {
		public string Data { get; private set; }

		public ButtonEventArgs(string data) {
			Data = data;
		}
	}
	
	public class Button {
		public Button(string text, EventHandler eventHandler) {
			Text = text;
			Color = Color.Black;
			IsActive = true;
			Click += eventHandler;
		}

		public string Text { get; set; }
		public Color Color { get; set; }
		public ButtonEventArgs Data { get; set; }
		public bool IsHovering { get; set; }
		public bool IsActive { get; set; }
		public bool IsCentered { get; set;  }
		public MouseState CurrentMouse { get; set; }
		public MouseState PreviousMouse { get; set; }
		
		private event EventHandler Click;
		
		public void OnClick() {
			Click?.Invoke(this, Data);
		}
	}
}