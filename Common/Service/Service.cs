using System;
using System.Collections.Generic;

namespace SS31
{
	// Base class for objects that act as global singleton services, like Profiler
	public abstract class Service : IDisposable
	{
		private readonly static List<Type> instances; // The list of the activated instances

		private bool disposed;
		public bool Disposed { get { return disposed; } }

		protected Service()
		{
			if (instances.Contains(GetType()))
				throw new InvalidOperationException("More than one instance of the service " + GetType() + " is not allowed.");

			instances.Add(GetType());

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
		}
		// Dont forget to call this in base classes
		public virtual void Dispose(bool disposing)
		{
			if (disposed)
				return; 

			if (!instances.Contains(GetType()))
				Logger.LogError("The service " + GetType() + " has no instances on disposal.");

			disposed = true;
		}

		static Service()
		{
			instances = new List<Type>();
		}
	}
}