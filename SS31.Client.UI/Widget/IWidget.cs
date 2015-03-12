using System;
using Microsoft.Xna.Framework;

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
}