using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Microsoft.Xna.Framework;
using SS31.Common;

namespace SS31.Client
{
	public class SceneManager : Service
	{
		#region Members
		private Dictionary<Type, Scene> _cachedScenes;
		private SSClient _client; // The client to create new scenes with
		private Profiler _profiler;

		private bool _changeQueued; // If a scene change will be made at the end of this frame.
		private Type _typeQueued; // The type of scene that will be switched to.

		public Scene ActiveScene { get; private set; }
		public bool HasScene { get { return ActiveScene != null; } }
		#endregion

		public SceneManager()
		{
			_cachedScenes = new Dictionary<Type, Scene>();
			_changeQueued = false;
			_typeQueued = null;
			ActiveScene = null;
			_profiler = ServiceManager.Resolve<Profiler>();
		}

		public void Initialize(SSClient c)
		{
			if (_client != null)
				return;

			_client = c;
		}

		public bool SwitchTo<T>()
		{
			if (_client == null)
				throw new InvalidOperationException("The client being passed to Scenes from SceneManager cannot be null.");

			Type type = typeof(T);
			bool cached = _cachedScenes.ContainsKey(type);

			if (ActiveScene != null && ActiveScene.GetType() == type)
				throw new ArgumentException("Cannot change the active scene to a scene of the same type.");

			_changeQueued = true;
			_typeQueued = type;
			return cached;
		}

		public bool HasCachedType<T>() where T : Scene
		{
			return _cachedScenes.ContainsKey(typeof(T));
		}
		public void DeleteCachedType<T>() where T : Scene
		{
			Type type = typeof(T);
			if (!_cachedScenes.ContainsKey(type))
				throw new ArgumentException("The scene manager cannot delete the requested scene type, because it is not cached.");

			Scene delete = _cachedScenes[type];
			_cachedScenes.Remove(type);
			delete.Dispose();
		}
		public Scene GetCachedType<T>() where T : Scene
		{
			Type type = typeof(T);

			if (_cachedScenes.ContainsKey(type))
				return _cachedScenes[type];

			return null;
		}

		public void Update(GameTime gameTime)
		{
			if (ActiveScene != null)
			{
				_profiler.BeginBlock("Scene PreUpdate");
				ActiveScene.PreUpdate(gameTime);
				_profiler.EndBlock();
				_profiler.BeginBlock("Scene Update");
				ActiveScene.Update(gameTime);
				_profiler.EndBlock();
			}
		}
		public void Draw(GameTime gameTime)
		{
			if (ActiveScene != null)
			{
				_profiler.BeginBlock("Scene PreDraw");
				ActiveScene.PreDraw(gameTime);
				_profiler.EndBlock();
				_profiler.BeginBlock("Scene Draw");
				ActiveScene.Draw(gameTime);
				_profiler.EndBlock();
			}

			if (_changeQueued)
				doSwitch();
		}

		private void doSwitch()
		{
			if (!_changeQueued || _typeQueued == null)
				throw new Exception("Cannot do a scene switch, invalid arguments for next scene.");

			_profiler.BeginBlock("Scene Switch");

			string last = "null";
			if (ActiveScene != null)
			{
				if (ActiveScene.ShouldSuspend)
					ActiveScene.DoSuspend();
				else
				{
					ActiveScene.Dispose();
					_cachedScenes.Remove(ActiveScene.GetType());
				}
				last = ActiveScene.GetType().ToString();
			}

			ActiveScene = null;

			if (!_cachedScenes.ContainsKey(_typeQueued))
			{
				ConstructorInfo cinfo = _typeQueued.GetConstructors().FirstOrDefault();
				if (cinfo == null)
					throw new NoPublicConstructorException(_typeQueued);
				ParameterInfo[] pinfo = cinfo.GetParameters();
				if (pinfo.Any())
					throw new InvalidConstructorParameterException("Subclasses of Scene must have a zero-argument constructor.");

				Scene ns = (Scene)Activator.CreateInstance(_typeQueued);
				ns.Game = _client;
				ns.DoInitialize();
				_cachedScenes[_typeQueued] = ns;
				ActiveScene = ns;
			}
			else
			{
				Scene ns = _cachedScenes[_typeQueued];
				ns.DoResume();
				ActiveScene = ns;
			}

			Logger.LogInfo("Changed to the active scene from " + last + " to " + ActiveScene.GetType() + ".");

			_typeQueued = null;
			_changeQueued = false;

			_profiler.EndBlock();
		}
	}
}