using System;
using System.Collections.Generic;

namespace SS31.Common
{
	// Base class for objects that act as global singleton services, like Profiler
	public abstract class Service : IDisposable
	{
		private readonly static List<Type> instances; // The list of the activated instances

		private bool _disposed;
		public bool Disposed { get { return _disposed; } }

		protected Service()
		{
			if (instances.Contains(GetType()))
				throw new InvalidOperationException("More than one instance of the service " + GetType() + " is not allowed.");

			instances.Add(GetType());

			_disposed = false;
		}
		~Service()
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

			if (!instances.Contains(GetType()))
				Logger.LogError("The service " + GetType() + " has no instances on disposal.");

			_disposed = true;
		}

		static Service()
		{
			instances = new List<Type>();
		}
	}
}