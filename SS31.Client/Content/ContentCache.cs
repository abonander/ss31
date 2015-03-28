using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using SS31.Common.Service;
using SS31.Common;

namespace SS31.Client.Content
{
	// Holds a cache of content. An easy way to group content based on use or type, and to easily
	// manage and dispose of the content. Caches will dispose of resources associated with them,
	// so associating the same resource to more than one cache can cause some very nasty problems.
	public sealed class ContentCache : IDisposable
	{
		private static ContentManagerService contentManagerService = null;
		private static int count = 0;

		#region Members
		private Dictionary<string, Texture> _textures;
		private Dictionary<string, SpriteFont> _fonts;
		private Dictionary<string, SoundEffect> _soundEffects;
		private Dictionary<string, Song> _songs;
		private Dictionary<string, Effect> _effects;

		public int TextureCount { get { return _textures.Count; } }
		public int FontCount { get { return _fonts.Count; } }
		public int SoundEffectCount { get { return _soundEffects.Count; } }
		public int SongCount { get { return _songs.Count; } }
		public int EffectCount { get { return _effects.Count; } }
		public int ContentCount { get { return TextureCount + FontCount + SoundEffectCount + SongCount + EffectCount; } }

		public readonly string Name;

		public object this[string name]
		{
			get
			{
				return GetContent(name);
			}
		}
		#endregion

		public ContentCache(string name = null)
		{
			if (contentManagerService == null)
				contentManagerService = ServiceManager.Resolve<ContentManagerService>();

			_textures = new Dictionary<string, Texture>();
			_fonts = new Dictionary<string, SpriteFont>();
			_soundEffects = new Dictionary<string, SoundEffect>();
			_songs = new Dictionary<string, Song>();
			_effects = new Dictionary<string, Effect>();

			if (String.IsNullOrEmpty(name))
				Name = "ContentCache" + (count++);
			else
			{
				if (contentManagerService.ContainsContentCache(name))
					throw new ArgumentException("The name already belongs to an existing ContentCache.");

				Name = name;
			}

			contentManagerService.RegisterContentCache(this);
		}
		~ContentCache()
		{
			Dispose(false);
		}

		#region Content Loading and Management
		#region Content Loading
		public void LoadTexture2D(string name)
		{
			if (HasTexture(name))
			{
				Logger.LogWarning("The resource \'" + name + "\' is already loaded. Use Reload() to force the resource to reload from the disk.");
				return;
			}

			Texture2D content = contentManagerService.LoadTexture2D(name);
			if (content == null)
				throw new Exception("Cound not load the content \'" + name + "\'.");
			_textures.Add(name, content);
		}
		public void LoadTexture3D(string name)
		{
			if (HasTexture(name))
			{
				Logger.LogWarning("The resource \'" + name + "\' is already loaded. Use Reload() to force the resource to reload from the disk.");
				return;
			}

			Texture3D content = contentManagerService.LoadTexture3D(name);
			if (content == null)
				throw new Exception("Cound not load the content \'" + name + "\'.");
			_textures.Add(name, content);
		}
		public void LoadTextureCube(string name)
		{
			if (HasTexture(name))
			{
				Logger.LogWarning("The resource \'" + name + "\' is already loaded. Use Reload() to force the resource to reload from the disk.");
				return;
			}

			TextureCube content = contentManagerService.LoadTextureCube(name);
			if (content == null)
				throw new Exception("Cound not load the content \'" + name + "\'.");
			_textures.Add(name, content);
		}
		public void LoadSpriteFont(string name)
		{
			if (HasSpriteFont(name))
			{
				Logger.LogWarning("The resource \'" + name + "\' is already loaded. Use Reload() to force the resource to reload from the disk.");
				return;
			}

			SpriteFont content = contentManagerService.LoadSpriteFont(name);
			if (content == null)
				throw new Exception("Cound not load the content \'" + name + "\'.");
			_fonts.Add(name, content);
		}
		public void LoadSoundEffect(string name)
		{
			if (HasSoundEffect(name))
			{
				Logger.LogWarning("The resource \'" + name + "\' is already loaded. Use Reload() to force the resource to reload from the disk.");
				return;
			}

			SoundEffect content = contentManagerService.LoadSoundEffect(name);
			if (content == null)
				throw new Exception("Cound not load the content \'" + name + "\'.");
			_soundEffects.Add(name, content);
		}
		public void LoadSong(string name)
		{
			if (HasSong(name))
			{
				Logger.LogWarning("The resource \'" + name + "\' is already loaded. Use Reload() to force the resource to reload from the disk.");
				return;
			}

			Song content = contentManagerService.LoadSong(name);
			if (content == null)
				throw new Exception("Cound not load the content \'" + name + "\'.");
			_songs.Add(name, content);
		}
		public void LoadEffect(string name)
		{
			if (HasEffect(name))
			{
				Logger.LogWarning("The resource \'" + name + "\' is already loaded. Use Reload() to force the resource to reload from the disk.");
				return;
			}

			Effect content = contentManagerService.LoadEffect(name);
			if (content == null)
				throw new Exception("Cound not load the content \'" + name + "\'.");
			_effects.Add(name, content);
		}
		#endregion

		// These functions make this ContentCache manage the passed content that has been loaded already.
		#region Pre-loaded Content Management
		public void AddTexture(string name, Texture content)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (content == null)
				throw new ArgumentNullException("content");

			if (HasTexture(name))
			{
				Logger.LogWarning("The ContentCache \'" + Name + "\' is already managing the resource \'" + name + "\'.");
				return;
			}

			_textures.Add(name, content);
		}
		public void AddSpriteFont(string name, SpriteFont content)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (content == null)
				throw new ArgumentNullException("content");

			if (HasSpriteFont(name))
			{
				Logger.LogWarning("The ContentCache \'" + Name + "\' is already managing the resource \'" + name + "\'.");
				return;
			}

			_fonts.Add(name, content);
		}
		public void AddSoundEffect(string name, SoundEffect content)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (content == null)
				throw new ArgumentNullException("content");

			if (HasSoundEffect(name))
			{
				Logger.LogWarning("The ContentCache \'" + Name + "\' is already managing the resource \'" + name + "\'.");
				return;
			}

			_soundEffects.Add(name, content);
		}
		public void AddSong(string name, Song content)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (content == null)
				throw new ArgumentNullException("content");

			if (HasSong(name))
			{
				Logger.LogWarning("The ContentCache \'" + Name + "\' is already managing the resource \'" + name + "\'.");
				return;
			}

			_songs.Add(name, content);
		}
		public void AddEffect(string name, Effect content)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (content == null)
				throw new ArgumentNullException("content");

			if (HasEffect(name))
			{
				Logger.LogWarning("The ContentCache \'" + Name + "\' is already managing the resource \'" + name + "\'.");
				return;
			}

			_effects.Add(name, content);
		}
		#endregion

		#region Content Checking
		public bool HasTexture(string name)
		{
			return _textures.ContainsKey(name);
		}
		public bool HasSpriteFont(string name)
		{
			return _fonts.ContainsKey(name);
		}
		public bool HasSoundEffect(string name)
		{
			return _soundEffects.ContainsKey(name);
		}
		public bool HasSong(string name)
		{
			return _songs.ContainsKey(name);
		}
		public bool HasEffect(string name)
		{
			return _effects.ContainsKey(name);
		}
		#endregion

		public object GetContent(string name)
		{
			if (_textures.ContainsKey(name))
				return _textures[name];
			if (_fonts.ContainsKey(name))
				return _fonts[name];
			if (_soundEffects.ContainsKey(name))
				return _soundEffects[name];
			if (_songs.ContainsKey(name))
				return _songs[name];
			if (_effects.ContainsKey(name))
				return _effects[name];

			return null;
		}
		public T GetContent<T>(string name)
			where T : class
		{
			Type type = typeof(T);
			if (type == typeof(Texture2D))
				return GetTexture2D(name) as T;
			if (type == typeof(Texture3D))
				return GetTexture3D(name) as T;
			if (type == typeof(TextureCube))
				return GetTextureCube(name) as T;
			if (type == typeof(SpriteFont))
				return GetSpriteFont(name) as T;
			if (type == typeof(SoundEffect))
				return GetSoundEffect(name) as T;
			if (type == typeof(Song))
				return GetSong(name) as T;
			if (type == typeof(Effect))
				return GetEffect(name) as T;

			throw new Exception("The type " + typeof(T).Name + " is not a content type.");
		}
		public Texture2D GetTexture2D(string name)
		{
			if (_textures.ContainsKey(name))
				return (Texture2D)_textures[name];

			return null;
		}
		public Texture3D GetTexture3D(string name)
		{
			if (_textures.ContainsKey(name))
				return (Texture3D)_textures[name];

			return null;
		}
		public TextureCube GetTextureCube(string name)
		{
			if (_textures.ContainsKey(name))
				return (TextureCube)_textures[name];

			return null;
		}
		public SpriteFont GetSpriteFont(string name)
		{
			if (_fonts.ContainsKey(name))
				return _fonts[name];

			return null;
		}
		public SoundEffect GetSoundEffect(string name)
		{
			if (_soundEffects.ContainsKey(name))
				return _soundEffects[name];

			return null;
		}
		public Song GetSong(string name)
		{
			if (_songs.ContainsKey(name))
				return _songs[name];

			return null;
		}
		public Effect GetEffect(string name)
		{
			if (_effects.ContainsKey(name))
				return _effects[name];

			return null;
		}
		#endregion

		// Unloads and disposes all resources tied to this cache
		public void UnloadAll()
		{
			if (TextureCount >= 1)
			{
				foreach (Texture tex in _textures.Values)
					tex.Dispose();
				_textures.Clear();
			}
			if (FontCount >= 1)
			{
				_fonts.Clear();
			}
			if (SoundEffectCount >= 1)
			{
				foreach (SoundEffect effect in _soundEffects.Values)
					effect.Dispose();

				_soundEffects.Clear();
			}
			if (SongCount >= 1)
			{
				foreach (Song song in _songs.Values)
					song.Dispose();

				_songs.Clear();
			}
			if (EffectCount >= 1)
			{
				foreach (Effect effect in _effects.Values)
					effect.Dispose();

				_effects.Clear();
			}
		}

		#region Disposal
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Dispose(bool disposing)
		{
			if (disposing)
			{
				UnloadAll();
				contentManagerService.UnregisterContentCache(this, false);
			}
		}
		#endregion
	}
}