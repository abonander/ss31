using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SS31.Common
{
	// Delegate for KeyPressed, KeyReleased, KeyHeld
	public delegate void KeyEvent(Keys key, KeyboardState current, KeyboardState last);

	// Delegate for MousePressed, MouseReleased, MouseClicked, MouseDoubleClicked
	public delegate void MouseButtonEvent(MouseButton button, MouseState current, MouseState last);

	// Delegate for MouseMoved
	public delegate void MouseMoveEvent(Point position, MouseState current, MouseState last);

	// Delegate for MouseDragged
	public delegate void MouseDragEvent(MouseButton buttons, Point position, MouseState current, MouseState last);

	// Delegate for MouseScrolled
	public delegate void MouseScrollEvent(int value, MouseState current, MouseState last);
}