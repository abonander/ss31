using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SS31.Core.Service
{
	// Base class for Service managers in Client and Server. Anything that has the IGameService interface can
	// be registered as a server. There can only be one instance of each service type, which acts like a singleton,
	// but it is stored and managed by this class, which makes it safer overall to use.
	public static class ServiceManager
	{
		private readonly static Dictionary<Type, IGameService> registeredServices;

		public static void UnregisterService<T>() where T : IGameService
		{
			Type type = typeof(T);
			if (!registeredServices.ContainsKey(type))
				return;
			T s = (T)registeredServices[type];

			registeredServices.Remove(type);
			s.Dispose();
		}

		// Called at the end of the program.
		public static void UnregisterAll()
		{
			Type loggerType = typeof(Logger);
			foreach(Type type in registeredServices.Keys)
			{
				if (type == loggerType)
					continue; // We want to remove the logger last, so the other services can use it while they are disposing.

				IGameService s = registeredServices[type];
				s.Dispose();
			}
			Logger logger = (Logger)registeredServices[loggerType];
			logger.Close();
			logger.Dispose();
			registeredServices.Clear();
		}

		// Check if a service has been registered.
		public static bool HasService<T>() where T : IGameService
		{
			return registeredServices.ContainsKey(typeof(T));
		}
			
		// Returns the service of type T, if it exists, or creates it if it doesnt.
		public static T Resolve<T>() where T : IGameService
		{
			Type t = typeof(T);
			if (!registeredServices.ContainsKey(t))
			{
				ConstructorInfo cinfo = t.GetConstructors().FirstOrDefault();
				if (cinfo == null)
					throw new NoPublicConstructorException(t);

				ParameterInfo[] pinfo = cinfo.GetParameters();
				if (pinfo.Any())
					throw new InvalidConstructorParameterException("The service constructors cannot have any arguments.");

				IGameService inst = Activator.CreateInstance<T>();
				registeredServices.Add(t, inst);
			}

			return (T)registeredServices[t];
		}
			
		static ServiceManager()
		{
			registeredServices = new Dictionary<Type, IGameService>();
		}
	}
}