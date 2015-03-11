using System;
using SS31.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SS31.Client.UI
{
	// This is not like the input manager for client. This takes information from the clients Input Manager, which is passed
	// into UIManager in Initialize(). This also keeps track of the widget currently selected, and any changes to the mouse position,
	// so it can send out input events to the proper widgets.
	internal class InputManager
	{
		#region Members
		public IInputManager BaseInputManager { get; private set; } // This is the input manager service on the client

		// This is the widget that has focus. This is equivalent to the topmost window in the OS, this is the widget that recieves
		// input events from the BaseInputManager
		private Widget _focusedWidget;
		public Widget FocusedWidget
		{
			get { return _focusedWidget; }
			set
			{
				if (value == _focusedWidget) { return; }
				if (_focusedWidget != null)
					_focusedWidget.OnFocusLost();
				_focusedWidget = value;
				if (_focusedWidget != null)
					_focusedWidget.OnFocusGained();
			}
		}
		// This is the widget that the mouse is hovering over, a lot of the time this will be the same as the FocusedWidget,
		// but there is the chance that the FocusedWidget will not be the same, if the mouse is moving around without pressing
		// and buttons, which would change the FocusedWidget
		private Widget _hoveredWidget;
		public Widget HoveredWidget
		{
			get { return _hoveredWidget; }
			set
			{
				if (value == _hoveredWidget) { return; }
				if (_hoveredWidget != null)
					_hoveredWidget.OnHoverExit();
				_hoveredWidget = value;
				if (_hoveredWidget != null)
					_hoveredWidget.OnHoverEnter();
			}
		}
		#endregion

		public InputManager(IInputManager manager)
		{
			BaseInputManager = manager;

			// Set up all of the input event delegates
			BaseInputManager.MouseMoved += (position, current, last) =>
			{

			};

			#region Mouse Input Handlers
			BaseInputManager.MouseButtonPressed += (button, current, last) => 
			{

			};
			BaseInputManager.MouseButtonReleased += (button, current, last) => 
			{

			};
			BaseInputManager.MouseClicked += (button, current, last) => 
			{

			};
			BaseInputManager.MouseDoubleClicked += (button, current, last) => 
			{

			};
			BaseInputManager.MouseScrolled += (value, current, last) => 
			{

			};
			BaseInputManager.MouseDragged += (buttons, position, current, last) => 
			{

			};
			#endregion

			#region Keyboard Input Handlers
			BaseInputManager.KeyPressed += (key, current, last) => 
			{

			};
			BaseInputManager.KeyReleased += (key, current, last) => 
			{

			};
			BaseInputManager.KeyHeld += (key, current, last) => 
			{

			};
			#endregion
		}

		public void Update(GameTime gameTime)
		{
			// TODO: everything input is event driven, so I dont know if we are going to need this function
		}

		#region Accessors
		#endregion
	}
}