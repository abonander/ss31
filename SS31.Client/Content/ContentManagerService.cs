using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using SS31.Common.Service;
using SS31.Common;

namespace SS31.Client.Content
{
	// Custom service that manages content on top of Monogame's built in content manager.
	// Content caches will automatically register themselves with this. This will also automatically
	// dispose of all remaining resource caches when it is disposed.
	public class ContentManagerService : GameService
	{
		#region Members
		private readonly Dictionary<string, ContentCache> _resourceCaches;

		public ContentManager ContentManager { get; private set; }
		public bool Initialized { get { return ContentManager != null; } }
		#endregion

		public ContentManagerService()
		{
			_resourceCaches = new Dictionary<string, ContentCache>();
			ContentManager = null;
		}

		public void Initialize(ContentManager content)
		{
			if (Initialized)
				return;

			ContentManager = content;
		}

		public void EnsureInitialized()
		{
			if (!Initialized)
				throw new Exception("The ContentManagerService has not been initialized.");
		}

		#region Content Loading
		public T LoadContent<T>(string name)
			where T : class
		{
			try
			{
				EnsureInitialized();
				return ContentManager.Load<T>(name);
			}
			catch (Exception e)
			{
				Logger.LogError("ContentManagerService could not load the " + typeof(T).Name + " \'" + name + "\'.");
				Logger.LogException(e);
				return null;
			}
		}
		public Texture2D LoadTexture2D(string name)
		{
			try
			{
				EnsureInitialized();
				return ContentManager.Load<Texture2D>(name);
			}
			catch (Exception e)
			{
				Logger.LogError("ContentManagerService could not load the Texture2D \'" + name + "\'.");
				Logger.LogException(e);
				return null;
			}
		}
		public Texture3D LoadTexture3D(string name)
		{
			try
			{
				EnsureInitialized();
				return ContentManager.Load<Texture3D>(name);
			}
			catch (Exception e)
			{
				Logger.LogError("ContentManagerService could not load the Texture3D \'" + name + "\'.");
				Logger.LogException(e);
				return null;
			}
		}
		public TextureCube LoadTextureCube(string name)
		{
			try
			{
				EnsureInitialized();
				return ContentManager.Load<TextureCube>(name);
			}
			catch (Exception e)
			{
				Logger.LogError("ContentManagerService could not load the TextureCube \'" + name + "\'.");
				Logger.LogException(e);
				return null;
			}
		}
		public SpriteFont LoadSpriteFont(string name)
		{
			try
			{
				EnsureInitialized();
				return ContentManager.Load<SpriteFont>(name);
			}
			catch (Exception e)
			{
				Logger.LogError("ContentManagerService could not load the SpriteFont \'" + name + "\'.");
				Logger.LogException(e);
				return null;
			}
		}
		public SoundEffect LoadSoundEffect(string name)
		{
			try
			{
				EnsureInitialized();
				return ContentManager.Load<SoundEffect>(name);
			}
			catch (Exception e)
			{
				Logger.LogError("ContentManagerService could not load the SoundEffect \'" + name + "\'.");
				Logger.LogException(e);
				return null;
			}
		}
		public Song LoadSong(string name)
		{
			try
			{
				EnsureInitialized();
				return ContentManager.Load<Song>(name);
			}
			catch (Exception e)
			{
				Logger.LogError("ContentManagerService could not load the Song \'" + name + "\'.");
				Logger.LogException(e);
				return null;
			}
		}
		public Effect LoadEffect(string name)
		{
			try
			{
				EnsureInitialized();
				return ContentManager.Load<Effect>(name);
			}
			catch (Exception e)
			{
				Logger.LogError("ContentManagerService could not load the Effect \'" + name + "\'.");
				Logger.LogException(e);
				return null;
			}
		}
		#endregion

		#region Content Cache Management
		public bool ContainsContentCache(string name)
		{
			return _resourceCaches.ContainsKey(name);
		}
		public bool ContainsContentCache(ContentCache cache)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			return _resourceCaches.ContainsKey(cache.Name);
		}

		public void RegisterContentCache(ContentCache cache)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			if (ContainsContentCache(cache.Name))
				return;

			_resourceCaches.Add(cache.Name, cache);
		}
		public void UnregisterContentCache(string name)
		{
			if (!ContainsContentCache(name))
				throw new ArgumentException("The passed name does not correspond to a registered ContentCache.");

			ContentCache cache = _resourceCaches[name];
			cache.Dispose();
			_resourceCaches.Remove(name);
		}
		public void UnregisterContentCache(ContentCache cache, bool dispose = true)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			if (!ContainsContentCache(cache.Name))
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
					foreach (ContentCache cache in _resourceCaches.Values)
						cache.Dispose();

					_resourceCaches.Clear();
				}
			}

			base.Dispose(disposing);
		}
		#endregion
	}
}