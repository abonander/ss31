using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using SS31.Common.Service;

namespace SS31.Client.Content
{
	// Holds a cache of content. An easy way to group content based on use or type, and to easily
	// manage and dispose of the content. Caches will dispose of resources associated with them,
	// so associating the same resource to more than one cache can cause some very nasty problems.
	public sealed class ResourceCache : IDisposable
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
		public int ResourceCount { get { return TextureCount + FontCount + SoundEffectCount + SongCount + EffectCount; } }

		public readonly string Name;
		#endregion

		public ResourceCache(string name = null)
		{
			if (contentManagerService == null)
				contentManagerService = ServiceManager.Resolve<ContentManagerService>();

			_textures = new Dictionary<string, Texture>();
			_fonts = new Dictionary<string, SpriteFont>();
			_soundEffects = new Dictionary<string, SoundEffect>();
			_songs = new Dictionary<string, Song>();
			_effects = new Dictionary<string, Effect>();

			if (String.IsNullOrEmpty(name))
				Name = "ResourceCache" + (count++);
			else
			{
				if (contentManagerService.ContainsResourceCache(name))
					throw new ArgumentException("The name already belongs to an existing ResourceCache.");

				Name = name;
			}

			contentManagerService.RegisterResourceCache(this);
		}
		~ResourceCache()
		{
			Dispose(false);
		}

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
				contentManagerService.UnregisterResourceCache(this, false);
			}
		}
		#endregion
	}
}