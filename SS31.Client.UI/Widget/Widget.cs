using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SS31.Client.UI
{
	public abstract class Widget
	{
		private static uint _count;
		private static readonly Dictionary<string, uint> _nameCounts;

		#region Members
		public Widget Parent { get; set; } // Parent widget to this one
		public List<Widget> Children { get; private set; } // The children widgets to this one

		public readonly string Identifier; // String that identitifies this widget uniquely

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
		#endregion

		protected Widget(string ident = null)
		{
			Children = new List<Widget>();

			if (!String.IsNullOrEmpty(ident))
			{
				if (!_nameCounts.ContainsKey(ident))
					Identifier = ident;
				else
				{
					_nameCounts[ident]++;
					Identifier = ident + _nameCounts[ident];
				}
			}
			else
				Identifier = "Widget" + _count;

			++_count;
		}

		// Called when any changes are made that would affect layout
		protected abstract void onResize(); // TODO: This may need to be virtual and call onResize() for children, don't know yet...

		#region Children Management
		#endregion

		#region Static Functions
		static Widget()
		{
			_count = 0;
			_nameCounts = new Dictionary<string, uint>();
		}
		#endregion
	}
}