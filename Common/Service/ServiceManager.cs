using System;
using System.Collections.Generic;

namespace SS31
{
	// Pseudo-static class that holds and provides access to the Services for the game.
	// Analysis disable once ConvertToStaticType
	public class ServiceManager : Service
	{
		private static Dictionary<Type, Service> registeredServices;

		#region Registration Functions
		public static void RegisterService(Service service)
		{
			if (service == null)
				throw new ArgumentNullException("service");

			Type rtype = service.GetType();
			if (registeredServices.ContainsKey(rtype))
				throw new ArgumentException("Service manager already contains an instance of the service " + rtype + ".");

			registeredServices.Add(rtype, service);
		}

		public static void UnregisterService(Service service, bool dispose = true)
		{
			if (service == null)
				throw new ArgumentNullException("service");

			Type rtype = service.GetType();
			if (!registeredServices.ContainsKey(rtype))
				return;
			Service s = registeredServices[rtype];
			if (s != service)
				throw new Exception("There are two difference instances of the service " + rtype);

			registeredServices.Remove(rtype);
			if (dispose)
				s.Dispose();
		}
		public static void UnregisterService(Type type, bool dispose = true)
		{
			if (type == null)
				throw new ArgumentNullException("type");
				
			if (!registeredServices.ContainsKey(type))
				return;
			Service s = registeredServices[type];

			registeredServices.Remove(type);
			if (dispose)
				s.Dispose();
		}
		public static void UnregisterService<T>(bool dispose = true) where T : Service
		{
			UnregisterService(typeof(T), dispose);
		}
		#endregion

		#region Accessor Functions
		public static bool HasService(Type t)
		{
			return registeredServices.ContainsKey(t);
		}
		public static bool HasService<T>() where T : Service
		{
			return registeredServices.ContainsKey(typeof(T));
		}

		public static Service GetService(Type t)
		{
			if (!registeredServices.ContainsKey(t))
				return null;

			return registeredServices[t];
		}
		public static T GetService<T>() where T : Service
		{
			Type t = typeof(T);
			if (!registeredServices.ContainsKey(t))
				return null;

			return (T)registeredServices[t];
		}
		#endregion

		public override void Dispose(bool disposing)
		{
			if (!Disposed && disposing)
			{
				foreach (Service s in registeredServices.Values)
					s.Dispose();

				registeredServices.Clear();
			}

			base.Dispose(disposing);
		}

		private ServiceManager() { }
		static ServiceManager()
		{
			registeredServices = new Dictionary<Type, Service>();
			RegisterService(new ServiceManager());
		}
	}
}