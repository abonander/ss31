using System;

namespace SS31.Core.Service
{
	// Base interface for all game service classes.
	public interface IGameService : IDisposable
	{
		bool Disposed { get; }

		void Dispose(bool disposing);
	}
}