﻿using System;
using System.Diagnostics;

namespace SS31
{
	// Wrapper for the System.Diagnostics.Stopwatch class
	public class Timer
	{
		public static long Frequency { get { return Stopwatch.Frequency; } }
		public static bool IsHighResolution { get { return Stopwatch.IsHighResolution; } }

		#region Members
		private readonly Stopwatch stopwatch;

		public bool IsRunning { get { return stopwatch.IsRunning; } }
		public long ElapsedMillis { get { return stopwatch.ElapsedMilliseconds; } }
		public long ElapsedTicks { get { return stopwatch.ElapsedTicks; } }
		public TimeSpan Elapsed { get { return stopwatch.Elapsed; } }
		public int Days { get { return stopwatch.Elapsed.Days; } }
		public int Hours { get { return stopwatch.Elapsed.Hours; } }
		public int Minutes { get { return stopwatch.Elapsed.Minutes; } }
		public int Seconds { get { return stopwatch.Elapsed.Seconds; } }
		public int Milliseconds { get { return stopwatch.Elapsed.Milliseconds; } }
		public double TotalDays { get { return stopwatch.Elapsed.TotalDays; } }
		public double TotalHours { get { return stopwatch.Elapsed.TotalHours; } }
		public double TotalMinutes { get { return stopwatch.Elapsed.TotalMinutes; } }
		public double TotalSeconds { get { return stopwatch.Elapsed.TotalSeconds; } }
		public double TotalMilliseconds { get { return stopwatch.Elapsed.TotalMilliseconds; } }
		#endregion

		public Timer(bool start = false)
		{
			stopwatch = new Stopwatch();
			if (start)
				stopwatch.Start();
		}

		public void Start()
		{
			stopwatch.Start();
		}
		public TimeSpan Reset()
		{
			TimeSpan elap = Elapsed;
			stopwatch.Reset();
			return elap;
		}
		public TimeSpan Restart()
		{
			TimeSpan elap = Elapsed;
			stopwatch.Restart();
			return elap;
		}
		public TimeSpan Stop()
		{
			TimeSpan elap = Elapsed;
			stopwatch.Stop();
			return elap;
		}
	}
}