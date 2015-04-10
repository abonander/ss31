using System;
using SS31.Core.Service;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SS31.Core.Input
{
	// While the client is the only part that will ever implement this, the gui library will need this as well.
	public interface IInputManager : IGameService
	{
		KeyboardState PreviousKeyState { get; }
		KeyboardState CurrentKeyState { get; }
		MouseState PreviousMouseState { get; }
		MouseState CurrentMouseState { get; }

		event KeyEvent KeyPressed; // A key changes from unpressed to pressed
		event KeyEvent KeyReleased; // A key changes from pressed to unpressed
		event KeyEvent KeyHeld; // A key remains in the pressed state for longer than KeyHoldTime
		event MouseButtonEvent MouseButtonPressed; // A mouse button changes from unpressed to pressed
		event MouseButtonEvent MouseButtonReleased; // A mouse button changes from pressed to unpressed
		event MouseButtonEvent MouseClicked; // A mouse button is pressed an released in a shorter time than given by MouseClickTime
		event MouseButtonEvent MouseDoubleClicked; // A mouse button is clicked twice in a shorter time than given by MouseDoubleClickTime
		event MouseMoveEvent MouseMoved; // The mouse position changes
		event MouseDragEvent MouseDragged; // The mouse position changes with one or more mouse buttons pressed, and the pressed mouse buttons are allowed to drag
		event MouseScrollEvent MouseScrolled; // The mouse wheel value is changed

		void Update(GameTime gameTime);

		bool IsKeyDown(Keys key);
		bool IsKeyUp(Keys key);
		bool IsKeyPreviouslyDown(Keys key);
		bool IsKeyPreviouslyUp(Keys key);
		bool IsKeyPressed(Keys key);
		bool IsKeyReleased(Keys key);
		bool IsKeyHeld(Keys key);
		ButtonState GetButtonState(MouseButton b);
		ButtonState GetPreviousButtonState(MouseButton b);
		bool IsButtonDown(MouseButton b);
		bool IsButtonUp(MouseButton b);
		bool IsButtonPreviouslyDown(MouseButton b);
		bool IsButtonPreviouslyUp(MouseButton b);
		bool IsButtonPressed(MouseButton b);
		bool IsButtonReleased(MouseButton b);

		Keys[] GetPressedKeys();
		MouseButton[] GetPressedButtons();

		Point GetMousePosition();
		Point GetLastMousePosition();
		void SetMousePosition(Point p);
		void SetMousePosition(int x, int y);
		Point GetMouseDeltaPosition();
		bool IsMouseMoving();
		bool IsMouseDragging();

		MouseButton GetDraggingMouseButtons();
		int GetScrollValue();
		int GetLastScrollValue();
		int GetDeltaScrollValue();

		bool CanMouseButtonDrag(MouseButton b);
	}
}