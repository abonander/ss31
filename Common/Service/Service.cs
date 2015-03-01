using System;
using System.Collections.Generic;

namespace SS31
{
	// Base class for objects that act as global singleton services, like Profiler
	public abstract class Service : IDisposable
	{
		private static Dictionary<Type, int> instances; // The number of instances of this service type

		private bool disposed;
		public bool Disposed { get { return disposed; } }

		protected Service()
		{
			if (instances[GetType()]++ > 0)
				throw new InvalidOperationException("More than one instance of the service " + GetType() + " is not allowed.");

			disposed = false;
		}
		~Service()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
			Logger.LogInfo("Service Unregistered: " + GetType());
		}
		// Dont forget to call this in base classes
		public virtual void Dispose(bool disposing)
		{
			if (disposed)
				return; 

			if (--instances[GetType()] < 0)
				Logger.LogError("The service " + GetType() + " has no instances on disposal.");

			disposed = true;
		}

		static Service()
		{
			instances = new Dictionary<Type, int>();
		}
	}
}