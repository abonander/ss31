using System;
using System.Collections.Generic;

namespace SS31
{
	public class ProfileBlock
	{
		private static int count = 0;

		#region Members
		public string Name { get; private set; } // Name of the profile block
		public TimeSpan Time { get; private set; } // Time in the current frame
		public TimeSpan MaxTime { get; private set; } // Maximum time on the current frame
		public int Count { get; private set; } // Number of times this block has executed in this frame
		public TimeSpan LastTime { get; private set; } // Time in the last frame
		public TimeSpan LastMaxTime { get; private set; } // Maximum time in the last frame
		public int LastCount { get; private set; } // Number of times this block executed in the last frame
		public TimeSpan IntervalTime { get; private set; } // Time in this interval
		public TimeSpan IntervalMaxTime { get; private set; } // Maximum time in this interval
		public int IntervalCount { get; private set; } // Number of times this block executed in this interval
		public TimeSpan TotalTime { get; private set; } // Total accumulated time
		public TimeSpan TotalMaxTime { get; private set; } // Total maximum time
		public int TotalCount { get; private set; } // Total accumulated execution times
		public ProfileBlock Parent { get; private set; }
		public List<ProfileBlock> Children { get; private set; }

		private Timer timer;
		#endregion

		#region Functions
		internal ProfileBlock(ProfileBlock p, string name)
		{
			timer = new Timer();
			Parent = p;
			Children = new List<ProfileBlock>();
			Time = MaxTime = LastTime = LastMaxTime = IntervalTime = IntervalMaxTime = TotalTime = TotalMaxTime = TimeSpan.Zero;
			Count = LastCount = IntervalCount = TotalCount = 0;

			int num = count++;
			if (String.IsNullOrEmpty(name))
				Name = "ProfileBlock" + num;
			else
				Name = name;
		}

		internal void Begin()
		{
			timer.Restart();
			++Count;
		}
		internal void End()
		{
			TimeSpan t = timer.Elapsed;
			if (t > MaxTime)
				MaxTime = t;
			Time += t;
		}

		internal void EndFrame()
		{
			LastTime = Time;
			LastMaxTime = MaxTime;
			LastCount = Count;
			IntervalTime += Time;
			if (MaxTime > IntervalMaxTime)
				IntervalMaxTime = MaxTime;
			IntervalCount += Count;
			TotalTime += Time;
			if (MaxTime > TotalMaxTime)
				TotalMaxTime = MaxTime;
			TotalCount += Count;
			Time = TimeSpan.Zero;
			MaxTime = TimeSpan.Zero;
			Count = 0;

			foreach (ProfileBlock p in Children)
				p.EndFrame();
		}
		internal void BeginInterval()
		{
			IntervalTime = TimeSpan.Zero;
			IntervalMaxTime = TimeSpan.Zero;
			IntervalCount = 0;

			foreach (ProfileBlock p in Children)
				p.BeginInterval();
		}

		public ProfileBlock GetChild(string name)
		{
			foreach (ProfileBlock p in Children)
				if (p.Name == name)
					return p;

			ProfileBlock np = new ProfileBlock(this, name);
			Children.Add(np);
			return np;
		}
		#endregion
	}

	public class Profiler : Service
	{
		#region Members
		private ProfileBlock root;
		private ProfileBlock current;
		private int intervalFrames;
		private int totalFrames;
		#endregion

		#region Functions
		public Profiler()
		{
			current = root = new ProfileBlock(null, "Root");
			intervalFrames = totalFrames = 0;
		}

		public override void Dispose(bool disposing)
		{
			if (!Disposed)
				EndFrame();

			base.Dispose(disposing);
		}

		public void BeginBlock(string name)
		{
			current = current.GetChild(name);
			current.Begin();
		}
		public void EndBlock()
		{
			if (root != current)
			{
				current.End();
				current = current.Parent;
			}
		}

		public void BeginFrame()
		{
			EndFrame();
			BeginBlock("BaseFrame");
		}
		public void EndFrame()
		{
			if (current != root)
			{
				EndBlock();
				++intervalFrames;
				++totalFrames;
				if (totalFrames == 0)
					++totalFrames;
				root.EndFrame();
				current = root;
			}
		}

		public void BeginInterval()
		{
			root.BeginInterval();
			intervalFrames = 0;
		}

		public override string ToString()
		{
			return String.Format("TODO: Implement Profiler ToString().");
		}
		#endregion
	}
}