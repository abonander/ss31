using System;
using System.Windows.Forms;
using SS31.Common;

namespace SS31.Server
{
	public class ServerForm : Form
	{
		private SSServer _server; // The instance of the server.

		public ServerForm(SSServer server)
		{
			if (server == null)
				throw new ArgumentNullException("server");
			if (!server.Initialized)
				throw new ArgumentException("The passed server must be initialized.");

			_server = server;
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (_server.Running)
			{
				// TODO: Stop the server gracefully
			}

			ServiceManager.UnregisterAll();
			base.OnFormClosing(e);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _server.Running)
			{
				// TODO: stop the server gracefully
			}

			base.Dispose(disposing);
		}
	}
}