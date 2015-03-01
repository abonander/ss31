using System;

namespace SS31
{
	public class SSServer
	{
		public bool Initialized { get; private set; } // If this server has been initialized
		public bool Running { get; private set; }

		private object _mutex;

		public SSServer()
		{
			Initialized = false;

			_mutex = new object();
		}

		public bool Initialize()
		{
			if (Initialized)
				return true;

			// TODO: Initialization stuff

			Initialized = true;
			return true;
		}
	}
}

