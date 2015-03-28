using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using SS31.Common.Service;

namespace SS31.Client.Content
{
	// Custom service that manages content on top of Monogame's built in content manager.
	// Resource caches will automatically register themselves with this. This will also automatically
	// dispose of all remaining resource caches when it is disposed.
	public class ContentManagerService : GameService
	{
		#region Members
		private Dictionary<string, ResourceCache> _resourceCaches;

		public ContentManager ContentManager { get; private set; }
		public bool Initialized { get { return ContentManager != null; } }
		#endregion

		public ContentManagerService()
		{
			_resourceCaches = new Dictionary<string, ResourceCache>();
			ContentManager = null;
		}

		public void Initialize(ContentManager content)
		{
			if (Initialized)
				return;

			ContentManager = content;
		}

		#region Resource Cache Management
		public bool ContainsResourceCache(string name)
		{
			return _resourceCaches.ContainsKey(name);
		}
		public bool ContainsResourceCache(ResourceCache cache)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			return _resourceCaches.ContainsKey(cache.Name);
		}

		public void RegisterResourceCache(ResourceCache cache)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			if (ContainsResourceCache(cache.Name))
				return;

			_resourceCaches.Add(cache.Name, cache);
		}
		public void UnregisterResourceCache(string name)
		{
			if (!ContainsResourceCache(name))
				throw new ArgumentException("The passed name does not correspond to a registered ResourceCache.");

			ResourceCache cache = _resourceCaches[name];
			cache.Dispose();
			_resourceCaches.Remove(name);
		}
		public void UnregisterResourceCache(ResourceCache cache, bool dispose = true)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			if (!ContainsResourceCache(cache.Name))
				throw new ArgumentException("The passed cache is not one of the caches registered.");

			if (dispose)
				cache.Dispose();
			_resourceCaches.Remove(cache.Name);
		}
		#endregion

		#region Disposal
		public override void Dispose(bool disposing)
		{
			if (!Disposed && disposing)
			{
				if (_resourceCaches.Count >= 1)
				{
					foreach (ResourceCache cache in _resourceCaches.Values)
						cache.Dispose();

					_resourceCaches.Clear();
				}
			}

			base.Dispose(disposing);
		}
		#endregion
	}
}