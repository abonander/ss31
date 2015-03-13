using System;
using System.Collections.Generic;
using SS31.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SS31.Client.UI
{
	// Base interface for widgets
	public interface IWidget : IDisposable
	{
		IWidgetContainer Parent { get; }

		bool IsFocusedWidget { get; }
		bool IsHoveredWidget { get; }

		Rectangle InnerArea { get; }
		Rectangle OuterArea { get; set; }

		Rectangle AbsoluteInnerArea { get; }
		Rectangle AbsoluteOuterArea { get; }

		Rectangle AbsoluteInputArea { get; }

		Point Position { get; set; }
		Point AbsolutePosition{ get; }
	}

	public abstract class Widget : IWidget
	{
		private static uint count;
		private static readonly Dictionary<string, uint> nameCounts;

		#region Members
		private bool _disposed;
		public bool Disposed { get { return _disposed; } }

		public IWidgetContainer Parent { get; internal set; } // Parent widget to this one

		public readonly string Identifier; // String that identitifies this widget uniquely

		public bool IsHoveredWidget { get { return InputManager.Instance.HoveredWidget == this; } }
		public bool IsFocusedWidget { get { return InputManager.Instance.FocusedWidget == this; } }

		#region Properties
		// The inner area of the widget, accounting for the outer area and the padding, and the stecil rectangle for the children components
		protected Rectangle _innerArea;
		public virtual Rectangle InnerArea
		{
			get { return _innerArea; }
		}
		// The outer area of the widget, before padding is applied. This is relative to the parent
		protected Rectangle _outerArea;
		public virtual Rectangle OuterArea
		{
			get { return _outerArea; }
			set
			{
				if (_outerArea != value)
				{
					_outerArea = value;
					_innerArea = new Rectangle(value.X + _leftPadding, value.Y + _topPadding, value.Width - (_leftPadding + _rightPadding), value.Height - (_topPadding + _bottomPadding));
					onResize();
				}
			}
		}
		// The absolute rectangle for the inner area
		public virtual Rectangle AbsoluteInnerArea
		{
			get { Point abs = AbsolutePosition + Position; return new Rectangle(abs.X, abs.Y, _innerArea.Width, _innerArea.Height); }
		}
		// The absolute rectange for the outer area
		public virtual Rectangle AbsoluteOuterArea
		{
			get { Point abs = AbsolutePosition; return new Rectangle(abs.X, abs.Y, _outerArea.Width, _outerArea.Height); }
		}

		// This will eventually, hopefully, allow custom input areas
		public virtual Rectangle AbsoluteInputArea { get { return AbsoluteOuterArea; } }

		// The position of the widget (relative to the parent position)
		public virtual Point Position
		{
			get { return OuterArea.Location; }
			set
			{
				Point old = OuterArea.Location;
				OuterArea.Offset(value);
				if (old != value)
					onResize();
			}
		}
		// The absolute position of the widget, on the screen
		public virtual Point AbsolutePosition
		{
			get { return Position + ((Parent == null) ? Point.Zero : Parent.AbsolutePosition); }
		}

		// The paddings on the inside of the widget, these dictate the difference between the InnerArea and OuterArea
		protected int _leftPadding;
		public virtual int LeftPadding
		{
			get { return _leftPadding; }
			set
			{
				int old = _leftPadding;
				_leftPadding = value;
				if (old != value)
					onResize();
			}
		}
		protected int _rightPadding;
		public virtual int RightPadding
		{
			get { return _rightPadding; }
			set
			{
				int old = _rightPadding;
				_rightPadding = value;
				if (old != value)
					onResize();
			}
		}
		protected int _topPadding;
		public virtual int TopPadding
		{
			get { return _topPadding; }
			set
			{
				int old = _topPadding;
				_topPadding = value;
				if (old != value)
					onResize();
			}
		}
		protected int _bottomPadding;
		public virtual int BottomPadding
		{
			get { return _bottomPadding; }
			set
			{
				int old = _bottomPadding;
				_bottomPadding = value;
				if (old != value)
					onResize();
			}
		}
		public virtual int Padding
		{
			set
			{
				int oldl = _leftPadding;
				int oldr = _rightPadding;
				int oldt = _topPadding;
				int oldb = _bottomPadding;
				_leftPadding = _rightPadding = _topPadding = _bottomPadding = value;
				if (oldl != value || oldr != value || oldt != value || oldb != value)
					onResize();
			}
		}
		#endregion

		#region Events
		// Public events that can be subscribed to, allows custom code to be run without making custom UI classes.
		// These will not be fired if child classes do not call base.on[event] for the event.
		public event UIKeyEvent OnKeyPress;
		public event UIKeyEvent OnKeyRelease;
		public event UIKeyEvent OnKeyHeld;
		public event UICharTypedEvent OnCharacterTyped;

		public event UIMouseButtonEvent OnMousePress;
		public event UIMouseButtonEvent OnMouseRelease;
		public event UIMouseButtonEvent OnMouseClick;
		public event UIMouseButtonEvent OnMouseDoubleClick;

		public event UIMouseMoveEvent OnMouseMove;
		public event UIMouseDragEvent OnMouseDrag;
		public event UIMouseScrollEvent OnMouseScroll;

		public event UIWidgetEvent OnFocusGained;
		public event UIWidgetEvent OnFocusLost;
		public event UIWidgetEvent OnHoverEnter;
		public event UIWidgetEvent OnHoverExit;
		#endregion
		#endregion

		protected Widget(string ident = null)
		{
			if (!String.IsNullOrEmpty(ident))
			{
				if (!nameCounts.ContainsKey(ident))
				{
					Identifier = ident;
					nameCounts[ident] = 1;
				}
				else
				{
					nameCounts[ident]++;
					Identifier = ident + nameCounts[ident];
				}
			}
			else
				Identifier = "Widget" + count;

			++count;

			_disposed = false;
		}
		~Widget()
		{
			Dispose(false);
		}

		// Update this all children widgets
		public abstract void Update(GameTime gameTime);
		// Draw this and all children widgets
		public abstract void Draw(SpriteBatch batch);

		// Called when any changes are made that would affect layout
		protected virtual void onResize() { } // TODO: This may need to call onResize() for children, don't know yet...

		// TODO: This region is a GIANT UNREADABLE FUCKING MESS OF CODE AND COMMENTS. Clean this up at some point.
		#region Input Management
		// See the InputManager for descriptions of these states, default just calls the events
		// When the widget loses focus
		internal virtual void onFocusGained() 
			{ if (OnFocusGained != null) OnFocusGained(this); }
		// When a widget gains focus
		internal virtual void onFocusLost()
			{ if (OnFocusLost != null) OnFocusLost(this); } 
		// When the mouse starts to hover over the widget
		internal virtual void onHoverEnter() 
			{ if (OnHoverEnter != null) OnHoverEnter(this); }
		// When the mouse stops hovering over the widget
		internal virtual void onHoverExit() 
			{ if (OnHoverExit != null) OnHoverExit(this); } 

		// The events below can only be triggered if this is the FocusedWidget in the InputManager
		// Called when a mouse button is pressed
		internal virtual void onMousePress(MouseButton button, MouseState current, MouseState last) 
			{ if (OnMousePress != null) OnMousePress(this, button, current, last); } 
		// Called when a mouse button is released
		internal virtual void onMouseRelease(MouseButton button, MouseState current, MouseState last) 
			{ if (OnMouseRelease != null) OnMouseRelease(this, button, current, last); } 
		// Called when a mouse button is clicked
		internal virtual void onMouseClick(MouseButton button, MouseState current, MouseState last) 
			{ if (OnMouseClick != null) OnMouseClick(this, button, current, last); } 
		// Called when a mouse button is double clicked
		internal virtual void onMouseDoubleClick(MouseButton button, MouseState current, MouseState last)
			{ if (OnMouseDoubleClick != null) OnMouseDoubleClick(this, button, current, last); } 

		// The move and drag functions will only be called if the mouse starting and ending point were both within the widget
		// Called if the mouse moves within the widget
		internal virtual void onMouseMove(Point position, MouseState current, MouseState last) 
			{ if (OnMouseMove != null) OnMouseMove(this, position, current, last); } 
		// Called if the mouse is dragged within the widget
		internal virtual void onMouseDrag(MouseButton buttons, Point position, MouseState current, MouseState last) 
			{ if (OnMouseDrag != null) OnMouseDrag(this, buttons, position, current, last); } 
		// Called if the mouse is scrolled within the widget
		internal virtual void onMouseScroll(int value, MouseState current, MouseState last) 
			{ if (OnMouseScroll != null) OnMouseScroll(this, value, current, last); } 

		// Called when a key is pressed
		internal virtual void onKeyDown(Keys key, KeyboardState current, KeyboardState last) 
			{ if (OnKeyPress != null) OnKeyPress(this, key, current, last); } 
		// Called when a key is released
		internal virtual void onKeyUp(Keys key, KeyboardState current, KeyboardState last) 
			{ if (OnKeyRelease != null) OnKeyRelease(this, key, current, last); } 
		// Called when a key is held down for an amount of time
		internal virtual void onKeyHeld(Keys key, KeyboardState current, KeyboardState last) 
			{ if (OnKeyHeld != null) OnKeyHeld(this, key, current, last); }
		// Contains specific data for character typing, convinient for text entry
		internal virtual void onCharacterTyped(Keys key, bool shift, bool control, bool alt) 
			{ if (OnCharacterTyped != null) OnCharacterTyped(this, key, shift, control, alt); } 
		#endregion

		#region Disposal
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		public virtual void Dispose(bool disposing)
		{
			_disposed = true;
		}
		#endregion

		#region Static Functions
		static Widget()
		{
			count = 0;
			nameCounts = new Dictionary<string, uint>();
		}
		#endregion
	}
}