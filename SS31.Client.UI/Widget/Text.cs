using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SS31.Client.UI
{
	// Simple class that represents a string with a font and color associtated with it
	public class Text
	{
		#region Members
		private string _text;
		public string String { get { return _text; } set { _text = value; updateCache(); } }
		private SpriteFont _font;
		public SpriteFont Font { get { return _font; } set { _font = value; updateCache(); } }
		public Color Color { get; set; }

		private Point _cachedSize;
		public Point CachedSize { get { return _cachedSize; } }
		public Rectangle CachedRect { get { return new Rectangle(0, 0, _cachedSize.X, _cachedSize.Y); } }
		#endregion

		public Text(string s = null, Color c = default(Color), SpriteFont font = null)
		{
			_text = s;
			Color = c;
			_font = font;

			_cachedSize = Point.Zero;
			updateCache();
		}

		public void Draw(SpriteBatch batch, Vector2 position)
		{
			if (_font == null || String.IsNullOrEmpty(_text))
				return;

			batch.DrawString(_font, _text, position, Color);
		}

		private void updateCache()
		{
			if (String == null || Font == null)
				return;

			Vector2 sz = Font.MeasureString(String);
			_cachedSize = new Point((int)sz.X, (int)sz.Y);
		}
	}
}