using System;
using System.Collections.Generic;
using Lidgren.Network;
using SS31.Common;

namespace SS31.Server
{
	public class SSNetServer : Service
	{
		private NetServer _server; // The server that this wraps

		#region Properties
		public NetPeerStatus Status { get { return _server.Status; } }
		public long Uid { get { return _server.UniqueIdentifier; } }
		public int Port { get { return _server.Port; } }
		public NetUPnP UPnP { get { return _server.UPnP; } }
		public object Tag { get { return _server.Tag; } set { _server.Tag = value; } }
		public List<NetConnection> Connections { get { return _server.Connections; } }
		public int ConnectionsCount { get { return _server.ConnectionsCount; } }
		public NetPeerStatistics Statistics { get { return _server.Statistics; } }
		public NetPeerConfiguration Configuration { get { return _server.Configuration; } }
		public PlatformSocket Socket { get { return _server.Socket; } }
		#endregion

		public SSNetServer()
		{
			_server = new NetServer(loadConfig());
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