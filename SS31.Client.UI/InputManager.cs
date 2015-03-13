using System;
using System.Collections.Generic;
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
		public static InputManager Instance { get; private set; }

		#region Members
		public IInputManager BaseInputManager { get; private set; } // This is the input manager service on the client

		public KeyboardState CurrentKeyState { get { return BaseInputManager.CurrentKeyState; } }
		public KeyboardState PreviousKeyState { get { return BaseInputManager.PreviousKeyState; } }
		public MouseState CurrentMouseState { get { return BaseInputManager.CurrentMouseState; } }
		public MouseState PreviousMouseState { get { return BaseInputManager.PreviousMouseState; } }

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
					_focusedWidget.onFocusLost();
				_focusedWidget = value;
				if (_focusedWidget != null)
				{
					_focusedWidget.onFocusGained();
					Widget root = _focusedWidget;
					while (root.Parent != null) // Get the new root for the focused widget
						root = _focusedParent = (Widget)root.Parent; // WARNING: CIRCULAR OWNERSHIP WILL MAKE THE INFINITE LOOPS, IS BAD, YES?
				}
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
					_hoveredWidget.onHoverExit();
				_hoveredWidget = value;
				if (_hoveredWidget != null)
					_hoveredWidget.onHoverEnter();
			}
		}

		private Point _mousePosition;
		private Widget _focusedParent; // Parent widget for the current focused widget
		private List<Widget> _managedWidgets;
		#endregion

		public InputManager(List<Widget> widgets, IInputManager manager)
		{
			if (widgets == null)
				throw new ArgumentNullException("widgets");

			Instance = this;

			_managedWidgets = widgets;
			BaseInputManager = manager;
			_mousePosition = Point.Zero;
			_focusedParent = null;

			// Set up all of the input event delegates

			// This will always be called first out of all of the delegates
			BaseInputManager.MouseMoved += (position, current, last) =>
			{
					_mousePosition = position;

					HoveredWidget = findHoverWidget();

					if (FocusedWidget != null)
						FocusedWidget.onMouseMove(position, current, last);
			};

			#region Mouse Input Handlers
			BaseInputManager.MouseButtonPressed += (button, current, last) => 
			{
					if (HoveredWidget != null)
						FocusedWidget = HoveredWidget;

					if (FocusedWidget != null)
						FocusedWidget.onMousePress(button, current, last);
			};
			BaseInputManager.MouseButtonReleased += (button, current, last) => 
			{
					if (FocusedWidget != null)
						FocusedWidget.onMouseRelease(button, current, last);
			};
			BaseInputManager.MouseClicked += (button, current, last) => 
			{
					if (FocusedWidget != null)
						FocusedWidget.onMouseClick(button, current, last);
			};
			BaseInputManager.MouseDoubleClicked += (button, current, last) => 
			{
					if (FocusedWidget != null)
						FocusedWidget.onMouseDoubleClick(button, current, last);
			};
			BaseInputManager.MouseScrolled += (value, current, last) => 
			{
					if (FocusedWidget != null)
						FocusedWidget.onMouseScroll(value, current, last);
			};
			BaseInputManager.MouseDragged += (buttons, position, current, last) => 
			{
					if (FocusedWidget != null)
						FocusedWidget.onMouseDrag(buttons, position, current, last);
			};
			#endregion

			#region Keyboard Input Handlers
			BaseInputManager.KeyPressed += (key, current, last) => 
			{
					if (FocusedWidget != null)
					{
						FocusedWidget.onKeyDown(key, current, last);
						FocusedWidget.onCharacterTyped(key, 
							current.IsKeyDown(Keys.LeftShift) || current.IsKeyDown(Keys.RightShift),
							current.IsKeyDown(Keys.LeftControl) || current.IsKeyDown(Keys.RightControl),
							current.IsKeyDown(Keys.LeftAlt) || current.IsKeyDown(Keys.RightAlt));
					}
			};
			BaseInputManager.KeyReleased += (key, current, last) => 
			{
					if (FocusedWidget != null)
						FocusedWidget.onKeyDown(key, current, last);
			};
			BaseInputManager.KeyHeld += (key, current, last) => 
			{
					if (FocusedWidget != null)
						FocusedWidget.onKeyHeld(key, current, last);
			};
			#endregion
		}

		// Start point for the recursion down
		private Widget findHoverWidget()
		{
			Widget ret = null;
			// Give preference to the current focues widget group
			if (FocusedWidget != null && _focusedParent != null)
				ret = findHoverWidget(_focusedParent);

			if (ret != null)
				return ret;

			foreach (Widget w in _managedWidgets)
			{
				if (_focusedParent != null && w != _focusedParent)
					ret = findHoverWidget(w);
				if (ret != null)
					return ret;
			}

			return null;
		}

		private Widget findHoverWidget(Widget root)
		{
			if (!root.AbsoluteInputArea.Contains(_mousePosition))
				return null;
			else if (!(root is IWidgetContainer))
				return root;

			IWidgetContainer ctnr = (IWidgetContainer)root;
			Widget ret = null;
			if (ctnr.Children.Count < 1)
				return root;
			else
			{
				foreach (Widget w in ctnr.Children)
				{
					ret = findHoverWidget(w);
					if (ret != null)
						return ret;
				}
			
				return root; // This is for a weird case where root has children, but they are all outside of the parent's input area
			}
	}

		public void Update(GameTime gameTime)
		{
			// TODO: everything input is event driven, so I dont know if we are going to need this function
		}
	}
}