using System;
using System.Collections.Generic;
using SS31.Common;
using SS31.Common.Service;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SS31.Client.UI
{
	public class UIManager : GameService
	{
		#region Members
		internal RenderManager RenderManager { get; private set; }
		internal UIInputManager InputManager { get; private set; }

		internal List<Widget> ManagedWidgets { get; private set; } // A list of the base widgets, as the starting point for the update and draw calls

		public bool IsInitialized { get { return RenderManager != null; } }
		#endregion

		public UIManager()
		{
			ManagedWidgets = new List<Widget>();
		}

		public void Initialize(GraphicsDevice device, ContentManager content)
		{
			RenderManager = new RenderManager(device, content);
			InputManager = new UIInputManager(ManagedWidgets);
		}

		public void Update(GameTime gameTime)
		{
			if (!IsInitialized)
				return;

			InputManager.Update(gameTime);

			foreach (Widget w in ManagedWidgets)
				w.Update(gameTime);
		}

		// Call this before clearing the main screen
		public void Predraw()
		{
			if (!IsInitialized)
				return;

			RenderManager.PreDraw(ManagedWidgets);
		}

		// Call this after everything else in the frame is done drawing
		public void Draw()
		{
			if (!IsInitialized)
				return;

			RenderManager.Draw();
		}

		#region Widget Management
		// Add a widget for the manager to manage. This should only ever be a base widget, as adding a non-base widget would
		// cause that widget to be updated and drawn twice
		public void AddWidget(Widget w)
		{
			if (ManagedWidgets.Contains(w))
				return;

			ManagedWidgets.Add(w);
		}

		// Remove and dispose of a widget
		public void RemoveWidget(Widget w)
		{
			if (!ManagedWidgets.Contains(w))
				return;
				
			w.Dispose();
			ManagedWidgets.Remove(w);
		}
		#endregion

		#region Dispose
		public override void Dispose(bool disposing)
		{
			if (!Disposed && disposing)
			{
				RenderManager.Dispose();
			}

			base.Dispose(disposing);
		}
		#endregion
	}
}