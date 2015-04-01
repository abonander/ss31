using System;
using SS31.Client.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SS31.Client.UI
{
	// A delegate for the general events on a widget
	public delegate void UIWidgetEvent(Widget w);

	// A delegate for KeyEvent on a widget
	public delegate void UIKeyEvent(Widget w, Keys key, KeyboardState curr, KeyboardState prev);

	// A delegate for CharacterTyped event on a widget
	public delegate void UICharTypedEvent(Widget w, Keys key, bool shift, bool control, bool alt);

	// A delegate for MouseButtonEvent on a widget
	public delegate void UIMouseButtonEvent(Widget w, MouseButton button, MouseState curr, MouseState prev);

	// A delegate for MouseMoveEvent on a widget
	public delegate void UIMouseMoveEvent(Widget w, Point position, MouseState curr, MouseState prev);

	// A delegate for MouseDragEvent on a widget
	public delegate void UIMouseDragEvent(Widget w, MouseButton mouseButtons, Point position, MouseState curr, MouseState prev);

	// The arguments for a delegate for MouseScroll	Event on a widget
	public delegate void UIMouseScrollEvent(Widget w, int value, MouseState curr, MouseState prev);
}