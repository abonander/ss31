using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Microsoft.Xna.Framework;
using SS31.Core;
using SS31.Core.Service;

namespace SS31.Client
{
	// Manages the active state and a list of cached states
	public class StateManager : GameService
	{
		#region Members
		private Dictionary<Type, State> _cachedScenes;
		private SSClient _client; // The client to create new scenes with
		private Profiler _profiler;

		private bool _changeQueued; // If a scene change will be made at the end of this frame.
		private Type _typeQueued; // The type of scene that will be switched to.

		public State ActiveScene { get; private set; }
		public bool HasScene { get { return ActiveScene != null; } }
		#endregion

		public StateManager()
		{
			_cachedScenes = new Dictionary<Type, State>();
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

		// Queue a state change to the T state at the end of this frame.
		// If the type T is cached, it will use that one, else, it will create a new instance of T and use that.
		// It will also cache the active state, if the state's ShouldSuspend is true.
		public bool SwitchTo<T>() where T : State
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

		// These next three are to manage cached states. Currently cached states can be explicitly deleted.
		public bool HasCachedType<T>() where T : State
		{
			return _cachedScenes.ContainsKey(typeof(T));
		}
		public void DeleteCachedType<T>() where T : State
		{
			Type type = typeof(T);
			if (!_cachedScenes.ContainsKey(type))
				throw new ArgumentException("The scene manager cannot delete the requested scene type, because it is not cached.");

			State delete = _cachedScenes[type];
			_cachedScenes.Remove(type);
			delete.Dispose();
		}
		public State GetCachedType<T>() where T : State
		{
			Type type = typeof(T);

			if (_cachedScenes.ContainsKey(type))
				return _cachedScenes[type];

			return null;
		}

		// These two methods are called from Client and will update and draw the active state.
		// If a state change is queued, then it will happen after the active state is done drawing.
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

		// Do all of the switching stuff.
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

				State ns = (State)Activator.CreateInstance(_typeQueued);
				ns.Game = _client;
				ns.DoInitialize();
				_cachedScenes[_typeQueued] = ns;
				ActiveScene = ns;
			}
			else
			{
				State ns = _cachedScenes[_typeQueued];
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