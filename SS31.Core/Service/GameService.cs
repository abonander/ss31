using System;

namespace SS31.Core.Service
{
	// Convinience class to implement IGameService members, for classes that do not have any other
	// base classes that they have to inherit from.
	public abstract class GameService : IGameService
	{
		private bool _disposed;
		public bool Disposed { get { return _disposed; } }

		protected GameService()
		{
			_disposed = false;
		}
		~GameService()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		// Dont forget to call this in base classes
		public virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;
		}
	}
}