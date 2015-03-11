using System;
using SS31.Common;

namespace SS31.Client.UI
{
	// This is not like the input manager for client. This takes information from the clients Input Manager, which is passed
	// into UIManager in Initialize(). This also keeps track of the widget currently selected, and any changes to the mouse position,
	// so it can send out input events to the proper widgets.
	internal class InputManager
	{
		public IInputManager BaseInputManager { get; private set; } // This is the input manager service on the client

		public InputManager(IInputManager manager)
		{
			BaseInputManager = manager;
		}
	}
}