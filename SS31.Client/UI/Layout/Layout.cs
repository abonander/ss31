using System;
using Microsoft.Xna.Framework;

namespace SS31.Client.UI
{
	// Vertical alignment enum for widgets
	public enum VerticalAlignment
	{
		Top,
		Center,
		Bottom
	}

	// Horizonatal alignment enum for widgets
	public enum HorizontalAlignment
	{
		Left,
		Center,
		Right
	}

	// Utility class for generting and calculating alignments
	internal static class UIAligner
	{
		// Returns the point to place the toAlign rectangle at in the area rectangle with the given alignment settings
		public static Point AlignRectangle(this Rectangle toAlign, Rectangle area, VerticalAlignment valign, HorizontalAlignment halign)
		{
			Point ret = Point.Zero;

			switch (valign)
			{
				case VerticalAlignment.Top:
					ret.Y = area.Top;
					break;
				case VerticalAlignment.Center:
					ret.Y = area.Top + ((area.Height / 2) - (toAlign.Height / 2));
					break;
				case VerticalAlignment.Bottom:
					ret.Y = area.Bottom - toAlign.Height;
					break;
			}
			switch (halign)
			{
				case HorizontalAlignment.Left:
					ret.X = area.Left;
					break;
				case HorizontalAlignment.Center:
					ret.X = area.Left + ((area.Width / 2) - (toAlign.Width / 2));
					break;
				case HorizontalAlignment.Right:
					ret.X = area.Right - toAlign.Width;
					break;
			}

			return ret;
		}

		// Quick utility method for aligning text
		public static Point AlignText(this Text text, Rectangle area, VerticalAlignment valign, HorizontalAlignment halign)
		{
			if (text == null)
				throw new ArgumentNullException("text");

			return text.CachedRect.AlignRectangle(area, valign, halign);
		}
	}
}