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
		private readonly Dictionary<string, ContentCache> _contentCaches;

		public ContentManager ContentManager { get; private set; }
		public bool Initialized { get { return ContentManager != null; } }

		public ContentCache this[string name]
		{
			get
			{
				return GetContentCache(name);
			}
			private set
			{
				if (_contentCaches.ContainsKey(name))
					_contentCaches[name] = value;
				else
					_contentCaches.Add(name, value);
			}
		}
		#endregion

		public ContentManagerService()
		{
			_contentCaches = new Dictionary<string, ContentCache>();
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

		public void LoadManagedContent<T>(string name, ContentCache cache)
			where T : class
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			object content = LoadContent<T>(name);
			Type type = typeof(T);
			if (type == typeof(Texture2D) || type == typeof(Texture3D) || type == typeof(TextureCube))
				cache.AddTexture(name, (Texture)content);
			else if (type == typeof(SpriteFont))
				cache.AddSpriteFont(name, (SpriteFont)content);
			else if (type == typeof(SoundEffect))
				cache.AddSoundEffect(name, (SoundEffect)content);
			else if (type == typeof(Song))
				cache.AddSong(name, (Song)content);
			else if (type == typeof(Effect))
				cache.AddEffect(name, (Effect)content);
			else
				throw new Exception("Something broke. You shouldn't see this.");
		}
		public void LoadManagedTexture2D(string name, ContentCache cache)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			Texture2D content = LoadTexture2D(name);
			if (content == null)
				return;
			cache.AddTexture(name, content);
		}
		public void LoadManagedTexture3D(string name, ContentCache cache)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			Texture3D content = LoadTexture3D(name);
			if (content == null)
				return;
			cache.AddTexture(name, content);
		}
		public void LoadManagedTextureCube(string name, ContentCache cache)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			TextureCube content = LoadTextureCube(name);
			if (content == null)
				return;
			cache.AddTexture(name, content);
		}
		public void LoadManagedSpriteFont(string name, ContentCache cache)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			SpriteFont content = LoadSpriteFont(name);
			if (content == null)
				return;
			cache.AddSpriteFont(name, content);
		}
		public void LoadManagedSoundEffect(string name, ContentCache cache)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			SoundEffect content = LoadSoundEffect(name);
			if (content == null)
				return;
			cache.AddSoundEffect(name, content);
		}
		public void LoadManagedSong(string name, ContentCache cache)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			Song content = LoadSong(name);
			if (content == null)
				return;
			cache.AddSong(name, content);
		}
		public void LoadManagedEffect(string name, ContentCache cache)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			Effect content = LoadEffect(name);
			if (content == null)
				return;
			cache.AddEffect(name, content);
		}
		#endregion

		#region Content Cache Management
		public bool ContainsContentCache(string name)
		{
			return _contentCaches.ContainsKey(name);
		}
		public bool ContainsContentCache(ContentCache cache)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			return _contentCaches.ContainsKey(cache.Name);
		}

		public void RegisterContentCache(ContentCache cache)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			if (ContainsContentCache(cache.Name))
				return;

			_contentCaches.Add(cache.Name, cache);
		}
		public void UnregisterContentCache(string name)
		{
			if (!ContainsContentCache(name))
				throw new ArgumentException("The passed name does not correspond to a registered ContentCache.");

			ContentCache cache = _contentCaches[name];
			cache.Dispose();
			_contentCaches.Remove(name);
		}
		public void UnregisterContentCache(ContentCache cache, bool dispose = true)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			if (!ContainsContentCache(cache.Name))
				throw new ArgumentException("The passed cache is not one of the caches registered.");

			if (dispose)
				cache.Dispose();
			_contentCaches.Remove(cache.Name);
		}

		public ContentCache GetContentCache(string name)
		{
			if (_contentCaches.ContainsKey(name))
				return _contentCaches[name];
			else
				return null;
		}
		#endregion

		#region Disposal
		public override void Dispose(bool disposing)
		{
			if (!Disposed && disposing)
			{
				if (_contentCaches.Count >= 1)
				{
					foreach (ContentCache cache in _contentCaches.Values)
						cache.Dispose();

					_contentCaches.Clear();
				}
			}

			base.Dispose(disposing);
		}
		#endregion
	}
}