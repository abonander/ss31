using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SS31.Client.UI
{
	public class BorderedTexture
	{
		#region Members
		private Texture2D _texture;
		public Texture2D Texture 
		{ 
			get { return _texture; } 
			set { _texture = value; update(); } 
		}
		public Color Color { get; set; }

		// TODO: Eventually make these values stupid person proof
		public int Border
		{
			set
			{
				int oldleft = _left;
				int oldright = _right;
				int oldtop = _top;
				int oldbottom = _bottom;
				_left = value;
				_right = value;
				_top = value;
				_bottom = value;
				_dirty = (oldleft != _left) || (oldright != _right) || (oldtop != _top) || (oldbottom != _bottom);
			}
		}
		private int _left;
		private int _right;
		private int _top;
		private int _bottom;
		public int LeftBorder 
		{ 
			get { return _left; } 
			set { int oldval = _left; _left = value; _dirty = oldval != _left; } 
		}
		public int RightBorder
		{ 
			get { return _right; } 
			set { int oldval = _right; _right = value; _dirty = oldval != _right; } 
		}
		public int TopBorder
		{ 
			get { return _top; } 
			set { int oldval = _top; _top = value; _dirty = oldval != _top; } 
		}
		public int BottomBorder
		{ 
			get { return _bottom; } 
			set { int oldval = _bottom; _bottom = value; _dirty = oldval != _bottom; } 
		}

		public bool HasBorder { get { return (_left != 0) || (_right != 0) || (_top != 0) || (_bottom != 0); } }

		private bool _dirty; // If the rectangles need to be regenerated
		private Rectangle[] _rectangles;
		#endregion

		public BorderedTexture(Texture2D tex, int border = 0)
		{
			Texture = tex;
			Color = Color.White;
			_left = border;
			_right = border;
			_top = border;
			_bottom = border;
			update();
		}
		public BorderedTexture(Texture2D tex, int left, int right, int top, int bottom)
		{
			Texture = tex;
			Color = Color.White;
			_left = left;
			_right = right;
			_top = top;
			_bottom = bottom;
			update();
		}

		public void Draw(SpriteBatch batch, Vector2 vec)
		{
			batch.Draw(Texture, vec, Color); // Dont bother with the rectangles
		}

		public void Draw(SpriteBatch batch, Rectangle area)
		{
			if (HasBorder)
			{
				// TODO: The amount of code called every frame with these hurts me on a fundamental level. We need to optimize the shit out of this.
				int x = area.X, y = area.Y, w = area.Width, h = area.Height;
				int wid = w - _left - _right;
				int hei = h - _top - _bottom;
				Rectangle[] rects = new Rectangle[9];
				rects[0] = new Rectangle(x, y, _left, _top);
				rects[1] = new Rectangle(x + _left, y, wid, _top);
				rects[2] = new Rectangle(x + _left + wid, y, _right, _top);
				rects[3] = new Rectangle(x, y + _top, _left, hei);
				rects[4] = new Rectangle(x + _left, y + _top, wid, hei);
				rects[5] = new Rectangle(x + _left + wid, y + _top, _right, hei);
				rects[6] = new Rectangle(x, y + _top + hei, _left, _bottom);
				rects[7] = new Rectangle(x + _left, y + _top + hei, wid, _bottom);
				rects[8] = new Rectangle(x + _left + wid, y + _top + hei, _right, _bottom);

				batch.Draw(Texture, rects[0], _rectangles[0], Color);
				batch.Draw(Texture, rects[1], _rectangles[1], Color);
				batch.Draw(Texture, rects[2], _rectangles[2], Color);
				batch.Draw(Texture, rects[3], _rectangles[3], Color);
				batch.Draw(Texture, rects[4], _rectangles[4], Color);
				batch.Draw(Texture, rects[5], _rectangles[5], Color);
				batch.Draw(Texture, rects[6], _rectangles[6], Color);
				batch.Draw(Texture, rects[7], _rectangles[7], Color);
				batch.Draw(Texture, rects[8], _rectangles[8], Color);
			}
			else
				batch.Draw(Texture, area, Color); // Dont bother with rectangles
		}

		private void update()
		{
			if (_texture == null)
				return;

			if (_rectangles == null)
				_rectangles = new Rectangle[9];

			if (!HasBorder)
				_rectangles[0] = new Rectangle(0, 0, _texture.Width, _texture.Height);
			else
			{
				int width = _texture.Width - _left - _right;
				int height = _texture.Height - _top - _bottom;

				_rectangles[0] = new Rectangle(0, 0, _left, _top);								// Top-left
				_rectangles[1] = new Rectangle(_left, 0, width, _top);							// Top-center
				_rectangles[2] = new Rectangle(width + _left, 0, _right, _top);					// Top-right
				_rectangles[3] = new Rectangle(0, _top, _left, height);							// Center-left
				_rectangles[4] = new Rectangle(_left, _top, width, height);						// Center
				_rectangles[5] = new Rectangle(width + _left, _top, _right, height);			// Center-right
				_rectangles[6] = new Rectangle(0, height + _top, _left, _bottom);				// Bottom-left
				_rectangles[7] = new Rectangle(_left, height + _top, width, _bottom);			// Bottom-center
				_rectangles[8] = new Rectangle(width + _left, height + _top, _right, _bottom);	// Bottom-right
			}
		}
	}
}