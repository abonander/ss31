using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SS31.Client.UI
{
	// A base class for widgets that contain other widgets, with the default IWidgetContainer implementation
	public class WidgetContainer : Widget, IWidgetContainer
	{
		#region Members
		public List<Widget> Children { get; private set; }
		public bool MaskChildren { get; set; }
		#endregion

		public WidgetContainer(string ident = null) : base(ident)
		{
			Children = new List<Widget>();

			MaskChildren = true;
		}

		public override void Update(GameTime gameTime)
		{
			foreach (Widget w in Children)
				w.Update(gameTime);
		}

		public override void Draw(SpriteBatch batch)
		{
			if (MaskChildren)
				RenderManager.Instance.PushStencilBuffer(AbsoluteInnerArea);

			foreach (Widget w in Children)
				w.Draw(batch);

			if (MaskChildren)
				RenderManager.Instance.PopStencilBuffer(AbsoluteInnerArea);
		}

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

		public override void Dispose(bool disposing)
		{
			if (!Disposed && disposing)
			{
				foreach (Widget w in Children)
					w.Dispose();

				Children.Clear();
			}

			base.Dispose(disposing);
		}
	}
}