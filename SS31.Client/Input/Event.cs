using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SS31.Client
{
	public delegate void KeyEvent(Keys key, KeyboardState current, KeyboardState last);

	public delegate void MouseButtonEvent(MouseButton button, MouseState current, MouseState last);

	public delegate void MouseMoveEvent(Point position, MouseState current, MouseState last);

	public delegate void MouseDragEvent(MouseButton buttons, Point position, MouseState current, MouseState last);

	public delegate void MouseScrollEvent(int value, MouseState current, MouseState last);
}