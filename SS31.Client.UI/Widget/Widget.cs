using System;
using System.Collections.Generic;
using System.Linq;
using SS31.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SS31.Client.UI
{
	public abstract class Widget : IDisposable
	{
		private static uint count;
		private static readonly Dictionary<string, uint> nameCounts;

		#region Members
		private bool _disposed;
		public bool Disposed { get { return _disposed; } }

		public Widget Parent { get; internal set; } // Parent widget to this one
		public List<Widget> Children { get; private set; } // The children widgets to this one

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

		// If this widget should use the stencil buffer to mask its children
		public bool MaskChildren { get; set; }
		#endregion
		#endregion

		protected Widget(string ident = null)
		{
			Children = new List<Widget>();

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

			// Default values
			MaskChildren = true;

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

		#region Input Management
		// See the InputManager for descriptions of these states
		public virtual void OnFocusGained() { } // When the widget loses focus
		public virtual void OnFocusLost() { } // When a widget gains focus
		public virtual void OnHoverEnter() { } // When the mouse starts to hover over the widget
		public virtual void OnHoverExit() { } // When the mouse stops hovering over the widget

		// The events below can only be triggered if this is the FocusedWidget in the InputManager
		public virtual void OnMousePress(MouseButton button, MouseState current, MouseState last) { } // Called when a mouse button is pressed
		public virtual void OnMouseRelease(MouseButton button, MouseState current, MouseState last) { } // Called when a mouse button is released
		public virtual void OnMouseClick(MouseButton button, MouseState current, MouseState last) { } // Called when a mouse button is clicked
		public virtual void OnMouseDoubleClick(MouseButton button, MouseState current, MouseState last) { } // Called when a mouse button is double clicked

		// The move and drag functions will only be called if the mouse starting and ending point were both within the widget
		public virtual void OnMouseMove(Point position, MouseState current, MouseState last) { } // Called if the mouse moves within the widget
		public virtual void OnMouseDrag(MouseButton buttons, Point position, MouseState current, MouseState last) { } // Called if the mouse is dragged within the widget
		public virtual void OnMouseScroll(int value, MouseState current, MouseState last) { } // Called if the mouse is scrolled within the widget

		public virtual void OnKeyDown(Keys key, KeyboardState current, KeyboardState last) { } // Called when a key is pressed
		public virtual void OnKeyUp(Keys key, KeyboardState current, KeyboardState last) { } // Called when a key is released
		public virtual void OnKeyHeld(Keys key, KeyboardState current, KeyboardState last) { } // Called when a key is held down for an amount of time
		public virtual void OnCharacterTyped(Keys key, bool shift, bool control, bool alt) { } // Contains specific data for character typing, convinient for text entry
		#endregion

		#region Children Management
		public virtual void AddChild(Widget widget)
		{
			if (widget.Parent == this)
				return;
			if (widget.Parent != null)
				throw new InvalidOperationException("Cannot add a widget as a child if it is already a child of another widget.");

			widget.Parent = this;
			Children.Add(widget);
		}

		public virtual void RemoveChild(Widget widget)
		{
			if (widget.Parent != this)
				return;

			widget.Parent = null;
			Children.Remove(widget);
			widget.Dispose();
		}
		public virtual void RemoveChild(string ident)
		{
			Widget w = (from wid in Children
						where wid.Identifier == ident
			            select wid).FirstOrDefault();
			if (w == null)
				return;

			w.Parent = null;
			Children.Remove(w);
			w.Dispose();
		}
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