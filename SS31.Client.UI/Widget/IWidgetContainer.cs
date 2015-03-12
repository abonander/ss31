using System;
using System.Collections.Generic;

namespace SS31.Client.UI
{
	// Interface for widgets that contain other widgets
	public interface IWidgetContainer : IWidget
	{
		// The children of this widget
		List<Widget> Children { get; }
		// If this widget should use the stencil buffer to mask its children
		bool MaskChildren { get; }

		// Add a widget as a child
		void AddChild(Widget w);
		// Remove a widget instance as a child
		void RemoveChild(Widget w);
		// Remove a widget with the specified identifier as a child
		void RemoveChild(string ident);
	}
}