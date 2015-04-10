using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SS31.Core;
using SS31.Core.Service;
using SS31.Core.Input;

namespace SS31.Client
{
	// Higher level input class, provides support for both polling and event based input
	// Methods here are pretty self explanitory
	public class InputManager : IInputManager
	{
		#region Members
		public KeyboardState PreviousKeyState { get; private set; }
		public KeyboardState CurrentKeyState { get; private set; }
		public MouseState PreviousMouseState { get; private set; }
		public MouseState CurrentMouseState { get; private set; }

		private readonly Dictionary<Keys, double> holdTimes; // Keeps track of the hold time (in seconds) for each key
		private readonly Dictionary<MouseButton, double> lastPressTime; // The last time the mouse button was pressed
		private readonly Dictionary<MouseButton, double> lastReleaseTime; // The last time the mouse button was released
		private readonly Dictionary<MouseButton, double> lastClickTime; // The last time the release was considered a click
		private readonly Dictionary<MouseButton, bool> doubleClickNext; // If the next click event for a mouse button can be a double click event

		public float KeyHoldTime = 0.5f; // The time (in seconds) before key hold events begin to get fired
		public float MouseClickTime = 0.25f; // The max time (in seconds) between a press and release event to qualify as a click event
		public float MouseDoubleClickTime = 0.5f; // The max time (in seconds) between two click events to qualify as a double click

		// These represent if the corresponding mouse button can raise the MouseDragged event
		public bool LeftButtonDrag = true;
		public bool MiddleButtonDrag = false;
		public bool RightButtonDrag = true;
		public bool X1ButtonDrag = false;
		public bool X2ButtonDrag = false;

		public event KeyEvent KeyPressed; // A key changes from unpressed to pressed
		public event KeyEvent KeyReleased; // A key changes from pressed to unpressed
		public event KeyEvent KeyHeld; // A key remains in the pressed state for longer than KeyHoldTime
		public event MouseButtonEvent MouseButtonPressed; // A mouse button changes from unpressed to pressed
		public event MouseButtonEvent MouseButtonReleased; // A mouse button changes from pressed to unpressed
		public event MouseButtonEvent MouseClicked; // A mouse button is pressed an released in a shorter time than given by MouseClickTime
		public event MouseButtonEvent MouseDoubleClicked; // A mouse button is clicked twice in a shorter time than given by MouseDoubleClickTime
		public event MouseMoveEvent MouseMoved; // The mouse position changes
		public event MouseDragEvent MouseDragged; // The mouse position changes with one or more mouse buttons pressed, and the pressed mouse buttons are allowed to drag
		public event MouseScrollEvent MouseScrolled; // The mouse wheel value is changed

		private Profiler _profiler;
		private bool _disposed;
		public bool Disposed { get { return _disposed; } }
		#endregion

		public void Update(GameTime gameTime)
		{
			_profiler.BeginBlock("Input Update");
			double totalSeconds = gameTime.TotalGameTime.TotalSeconds;
			double totalMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;
			double elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;
			double elapsedMilliseconds = gameTime.ElapsedGameTime.TotalMilliseconds;

			PreviousKeyState = CurrentKeyState;
			PreviousMouseState = CurrentMouseState;
			CurrentKeyState = Keyboard.GetState();
			CurrentMouseState = Mouse.GetState();

			MouseButton[] pressedButtons = GetPressedButtons();

			if (CurrentMouseState.Position != PreviousMouseState.Position)
				mouseMovedEvent();

			MouseButton drag = MouseButton.None;
			if ((drag = GetDraggingMouseButtons()) != MouseButton.None)
				mouseDraggedEvent(drag);

			if (CurrentMouseState.ScrollWheelValue != PreviousMouseState.ScrollWheelValue)
				scrollEvent();

			foreach (Keys key in Enum.GetValues(typeof(Keys)))
			{
				if (IsKeyDown(key))
				{
					holdTimes[key] += elapsedSeconds;
					if (holdTimes[key] > KeyHoldTime)
						keyHeldEvent(key);

					if (IsKeyPressed(key))
						keyPressedEvent(key);
				}
				else
				{
					if (IsKeyReleased(key))
					{
						keyReleasedEvent(key);
						holdTimes[key] = 0.0;
					}
				}
			}

			foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
			{
				if (button == MouseButton.None)
					continue; // Ignore the None button

				if (IsButtonDown(button))
				{
					if (IsButtonPressed(button))
					{
						buttonPressedEvent(button);
						lastPressTime[button] = totalSeconds;
					}
				}
				else
				{
					if (IsButtonReleased(button))
					{
						buttonReleasedEvent(button);
						lastReleaseTime[button] = totalSeconds;

						if (canDoubleClick(button))
						{
							buttonClickedEvent(button);
							buttonDoubleClickEvent(button);
							lastClickTime[button] = totalSeconds;
							doubleClickNext[button] = false; // It just double clicked, consider the next click a single click
						}
						else if (canClick(button))
						{
							buttonClickedEvent(button);
							lastClickTime[button] = totalSeconds;
							doubleClickNext[button] = true; // It just single clicked, consider the next click a possible double click
						}
						else
						{
							doubleClickNext[button] = false; // This will only be reached if a click times out, cancelling the possiblility of a double click.
							// We dont need to worry about cancelling a double click if the second click took too long, that is handled by the else-if block above.
						}
					}
				}
			}

			_profiler.EndBlock();
		}

		#region Private Helpers
		private bool canClick(MouseButton b)
		{
			double time = lastReleaseTime[b] - lastPressTime[b];
			return time > 0.0 && time < MouseClickTime;
		}
		private bool canDoubleClick(MouseButton b)
		{
			double time = lastReleaseTime[b] - lastClickTime[b];
			return time > 0.0 && time < MouseDoubleClickTime && doubleClickNext[b];
		}
		#endregion

		#region Accessor Methods
		public bool IsKeyDown(Keys key)
		{
			return CurrentKeyState.IsKeyDown(key);
		}
		public bool IsKeyUp(Keys key)
		{
			return CurrentKeyState.IsKeyUp(key);
		}
		public bool IsKeyPreviouslyDown(Keys key)
		{
			return PreviousKeyState.IsKeyDown(key);
		}
		public bool IsKeyPreviouslyUp(Keys key)
		{
			return PreviousKeyState.IsKeyUp(key);
		}
		public bool IsKeyPressed(Keys key)
		{
			return CurrentKeyState.IsKeyDown(key) && PreviousKeyState.IsKeyUp(key);
		}
		public bool IsKeyReleased(Keys key)
		{
			return CurrentKeyState.IsKeyUp(key) && PreviousKeyState.IsKeyDown(key);
		}
		public bool IsKeyHeld(Keys key)
		{
			return holdTimes[key] > KeyHoldTime;
		}
		public ButtonState GetButtonState(MouseButton b)
		{
			return b.GetMappedState(CurrentMouseState);
		}
		public ButtonState GetPreviousButtonState(MouseButton b)
		{
			return b.GetMappedState(PreviousMouseState);
		}
		public bool IsButtonDown(MouseButton b)
		{
			return b.GetMappedState(CurrentMouseState) == ButtonState.Pressed;
		}
		public bool IsButtonUp(MouseButton b)
		{
			return b.GetMappedState(CurrentMouseState) == ButtonState.Released;
		}
		public bool IsButtonPreviouslyDown(MouseButton b)
		{
			return b.GetMappedState(PreviousMouseState) == ButtonState.Pressed;
		}
		public bool IsButtonPreviouslyUp(MouseButton b)
		{
			return b.GetMappedState(PreviousMouseState) == ButtonState.Released;
		}
		public bool IsButtonPressed(MouseButton b)
		{
			return b.GetMappedState(CurrentMouseState) == ButtonState.Pressed && b.GetMappedState(PreviousMouseState) == ButtonState.Released;
		}
		public bool IsButtonReleased(MouseButton b)
		{
			return b.GetMappedState(CurrentMouseState) == ButtonState.Released && b.GetMappedState(PreviousMouseState) == ButtonState.Pressed;
		}

		public Keys[] GetPressedKeys()
		{
			return CurrentKeyState.GetPressedKeys();
		}
		public MouseButton[] GetPressedButtons()
		{
			MouseButton[] res = new MouseButton[5];
			int cnt = 0;
			if (IsButtonDown(MouseButton.Left))
				res[cnt++] = MouseButton.Left;
			if (IsButtonDown(MouseButton.Right))
				res[cnt++] = MouseButton.Right;
			if (IsButtonDown(MouseButton.Middle))
				res[cnt++] = MouseButton.Middle;
			if (IsButtonDown(MouseButton.X1))
				res[cnt++] = MouseButton.X1;
			if (IsButtonDown(MouseButton.X2))
				res[cnt++] = MouseButton.X2;
			if (cnt != 5)
				Array.Resize(ref res, cnt);
			return res;
		}

		public Point GetMousePosition()
		{
			return CurrentMouseState.Position;
		}
		public Point GetLastMousePosition()
		{
			return PreviousMouseState.Position;
		}
		public void SetMousePosition(Point p)
		{
			Mouse.SetPosition(p.X, p.Y);
		}
		public void SetMousePosition(int x, int y)
		{
			Mouse.SetPosition(x, y);
		}
		public Point GetMouseDeltaPosition()
		{
			return CurrentMouseState.Position - PreviousMouseState.Position;
		}
		public bool IsMouseMoving()
		{
			return CurrentMouseState.Position != PreviousMouseState.Position;
		}
		public bool IsMouseDragging()
		{
			return CurrentMouseState.Position != PreviousMouseState.Position &&
				((LeftButtonDrag && IsButtonDown(MouseButton.Left)) ||
					(RightButtonDrag && IsButtonDown(MouseButton.Right)) ||
					(MiddleButtonDrag && IsButtonDown(MouseButton.Middle)) ||
					(X1ButtonDrag && IsButtonDown(MouseButton.X1)) ||
					(X2ButtonDrag && IsButtonDown(MouseButton.X2)));
		}
		public MouseButton GetDraggingMouseButtons()
		{
			if (CurrentMouseState.Position == PreviousMouseState.Position)
				return MouseButton.None;

			MouseButton res = MouseButton.None;
			if (LeftButtonDrag && IsButtonDown(MouseButton.Left))
				res &= MouseButton.Left;
			if (RightButtonDrag && IsButtonDown(MouseButton.Right))
				res &= MouseButton.Right;
			if (MiddleButtonDrag && IsButtonDown(MouseButton.Middle))
				res &= MouseButton.Middle;
			if (X1ButtonDrag && IsButtonDown(MouseButton.X1))
				res &= MouseButton.X1;
			if (X2ButtonDrag && IsButtonDown(MouseButton.X2))
				res &= MouseButton.X2;
			return res;
		}
		public int GetScrollValue()
		{
			return CurrentMouseState.ScrollWheelValue;
		}
		public int GetLastScrollValue()
		{
			return PreviousMouseState.ScrollWheelValue;
		}
		public int GetDeltaScrollValue()
		{
			return CurrentMouseState.ScrollWheelValue - PreviousMouseState.ScrollWheelValue;
		}

		public bool CanMouseButtonDrag(MouseButton b)
		{
			switch (b)
			{
				case MouseButton.Left:
					return LeftButtonDrag;
				case MouseButton.Middle:
					return MiddleButtonDrag;
				case MouseButton.Right:
					return RightButtonDrag;
				case MouseButton.X1:
					return X1ButtonDrag;
				case MouseButton.X2:
					return X2ButtonDrag;
				default:
					throw new Exception("Invalid mouse button.");
			}
		}
		#endregion

		#region Event Methods
		private void keyPressedEvent(Keys key)
		{
			if (KeyPressed != null)
				KeyPressed(key, CurrentKeyState, PreviousKeyState);
		}
		private void keyReleasedEvent(Keys key)
		{
			if (KeyReleased != null)
				KeyReleased(key, CurrentKeyState, PreviousKeyState);
		}
		private void keyHeldEvent(Keys key)
		{
			if (KeyHeld != null)
				KeyHeld(key, CurrentKeyState, PreviousKeyState);
		}
		private void buttonPressedEvent(MouseButton b)
		{
			if (MouseButtonPressed != null)
				MouseButtonPressed(b, CurrentMouseState, PreviousMouseState);
		}
		private void buttonReleasedEvent(MouseButton b)
		{
			if (MouseButtonReleased != null)
				MouseButtonReleased(b, CurrentMouseState, PreviousMouseState);
		}
		private void buttonClickedEvent(MouseButton b)
		{
			if (MouseClicked != null)
				MouseClicked(b, CurrentMouseState, PreviousMouseState);
		}
		private void buttonDoubleClickEvent(MouseButton b)
		{
			if (MouseDoubleClicked != null)
				MouseDoubleClicked(b, CurrentMouseState, PreviousMouseState);
		}
		private void mouseMovedEvent()
		{
			if (MouseMoved != null)
				MouseMoved(CurrentMouseState.Position, CurrentMouseState, PreviousMouseState);
		}
		private void mouseDraggedEvent(MouseButton b)
		{
			if (MouseDragged != null)
				MouseDragged(b, CurrentMouseState.Position, CurrentMouseState, PreviousMouseState);
		}
		private void scrollEvent()
		{
			if (MouseScrolled != null)
				MouseScrolled(CurrentMouseState.ScrollWheelValue, CurrentMouseState, PreviousMouseState);
		}
		#endregion

		#region Initialization
		public InputManager()
		{
			holdTimes = new Dictionary<Keys, double>();
			foreach (Keys k in Enum.GetValues(typeof(Keys)))
				holdTimes.Add(k, 0.0);

			lastPressTime = new Dictionary<MouseButton, double>();
			lastReleaseTime = new Dictionary<MouseButton, double>();
			lastClickTime = new Dictionary<MouseButton, double>();
			doubleClickNext = new Dictionary<MouseButton, bool>();
			foreach (MouseButton b in Enum.GetValues(typeof(MouseButton)))
			{
				lastPressTime.Add(b, 0.0);
				lastReleaseTime.Add(b, 0.0);
				lastClickTime.Add(b, 0.0);
				doubleClickNext.Add(b, false);
			}

			_profiler = ServiceManager.Resolve<Profiler>();
			_disposed = false;
		}
		~InputManager()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		public void Dispose(bool disposing)
		{
			if (!Disposed && disposing)
			{
				holdTimes.Clear();
				lastPressTime.Clear();
				lastReleaseTime.Clear();
				lastClickTime.Clear();
				doubleClickNext.Clear();

				KeyPressed = null;
				KeyReleased = null;
				KeyHeld = null;
				MouseButtonPressed = null;
				MouseButtonReleased = null;
				MouseClicked = null;
				MouseDoubleClicked = null;
				MouseMoved = null;
				MouseDragged = null;
				MouseScrolled = null;
			}

			_disposed = true;
		}
		#endregion
	}
}