using System;
using System.Collections.Generic;
using Lidgren.Network;
using SS31.Common;
using SS31.Common.Service;

namespace SS31.Server
{
	public class SSNetServer : NetServer, IGameService
	{
		private bool _disposed;
		public bool Disposed { get { return _disposed; } }

		public SSNetServer() :
			base(loadConfig())
		{
			_disposed = false;
		}
		~SSNetServer()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		public void Dispose(bool disposing)
		{
			// TODO: See if something needs to be disposed here later.
		}

		public void SendToAll(NetOutgoingMessage m)
		{
			SendToAll(m, NetDeliveryMethod.ReliableOrdered);
		}
		public void SendMessage(NetOutgoingMessage m, NetConnection client)
		{
			SendMessage(m, client, NetDeliveryMethod.ReliableOrdered);
		}
		public void SendToMany(NetOutgoingMessage m, List<NetConnection> clients)
		{
			SendMessage(m, clients, NetDeliveryMethod.ReliableOrdered, 0);
		}

		#region Config
		// TODO: Actually make this load from the configuration manager
		private static NetPeerConfiguration loadConfig()
		{
			NetPeerConfiguration config = new NetPeerConfiguration(SharedConstants.SERVER_TAG);
			config.Port = (int)SharedConstants.SS_DEFAULT_PORT;
			return config;
		}
		#endregion
	}
}