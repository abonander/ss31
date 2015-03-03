using System;
using Microsoft.Xna.Framework;
using SS31.Common;

namespace SS31.Client
{
	// Base class that represents a "scene" in the game, only one can be active at a time.
	// For allowing different states, i.e. "Main Menu" -> "Multiplayer Menu" -> "In Game" ect.
	public abstract class Scene : IDisposable
	{
		public string TypeName { get { return GetType().ToString(); } }

		#region Members
		public abstract bool ShouldSuspend { get; } // If the scene should be suspended and cached instead of disposed.

		// If the scene has been initialized
		private bool _isInitialized;
		public bool IsInitialized
		{
			get
			{
				if (IsDisposed)
					throw new ObjectDisposedException("Scene");
				return _isInitialized;
			}
		}
		// If the scene is the current active scene, mutually exclusive with IsSuspended
		private bool _isCurrent;
		public bool IsCurrent
		{
			get
			{
				if (IsDisposed)
					throw new ObjectDisposedException("Scene");
				return _isCurrent;
			}
		}
		// If the scene is suspended, as it was active but ShouldSuspend was true, so it was cached instead of disposed
		private bool _isSuspended;
		public bool IsSuspended
		{
			get
			{
				if (IsDisposed)
					throw new ObjectDisposedException("Scene");
				return _isSuspended;
			}
		}

		private bool _isDisposed;
		public bool IsDisposed { get { return _isDisposed; } }

		private SSClient _game;
		public SSClient Game
		{
			get
			{
				if (IsDisposed)
					throw new ObjectDisposedException("Scene");
				return _game;
			}
			set
			{
				if (_game != null)
					throw new InvalidOperationException("Cannot set the Scene's game more than once.");
				_game = value;
			}
		}
		#endregion

		protected Scene()
		{
			_isInitialized = false;
			_isCurrent = false;
			_isSuspended = false;
			_isDisposed = false;
			_game = null;
		}
		~Scene()
		{
			Dispose(false);
		}

		// Nothing outside of SceneManager should ever call the next three functions
		public void DoInitialize()
		{
			if (_isInitialized)
				throw new Exception("The state " + TypeName + " was already initialized when Initialize() was called.");

			ServiceManager.Resolve<Profiler>().BeginBlock("Scene Initialize (" + TypeName + ")");
			Initialize();
			ServiceManager.Resolve<Profiler>().EndBlock();
			_isInitialized = true;
		}
		public void DoSuspend()
		{
			if (!ShouldSuspend)
				throw new Exception("The state " + TypeName + " should not be suspended, but instead should be disposed.");
			if (_isSuspended)
				throw new Exception("The state " + TypeName + " was already suspended when Suspend() was called.");

			ServiceManager.Resolve<Profiler>().BeginBlock("Scene Suspend (" + TypeName + ")");
			Suspend();
			ServiceManager.Resolve<Profiler>().EndBlock();
			_isSuspended = true;
			_isCurrent = false;
		}
		public void DoResume()
		{
			if (!ShouldSuspend)
				throw new Exception("The state " + TypeName + " should not be suspended, and therefore should never be resumed.");
			if (!_isSuspended)
				throw new Exception("The state " + TypeName + " was not suspended when Resume() was called.");

			ServiceManager.Resolve<Profiler>().BeginBlock("Scene Resume (" + TypeName + ")");
			Resume();
			ServiceManager.Resolve<Profiler>().EndBlock();
			_isSuspended = false;
			_isCurrent = true;
		}

		protected abstract void Initialize(); // Called when the scene is first initialized
		public virtual void PreUpdate(GameTime gameTime) { } // Called before Update() for optional pre-update actions
		public abstract void Update(GameTime gameTime); // Called when the scene is updated
		public virtual void PreDraw(GameTime gameTime) { } // Called after Update() and before Draw(), for any pre-drawing actions
		public abstract void Draw(GameTime gameTime); // Called when the scene is drawn
		protected virtual void Suspend() { } // If DisposeOnLeave is false, this will be called before the scene is cached
		protected virtual void Resume() { } // If this scene is suspended, this will be called when it is moved from the cache to the active scene

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		public void Dispose(bool disposing)
		{
			if (IsDisposed)
				return;

			_game = null;
			_isDisposed = true;
		}
	}
}