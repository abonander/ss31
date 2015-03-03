using System;
using System.Diagnostics;

namespace SS31.Common
{
	// Wrapper for the System.Diagnostics.Stopwatch class
	public class Timer
	{
		public static long Frequency { get { return Stopwatch.Frequency; } }
		public static bool IsHighResolution { get { return Stopwatch.IsHighResolution; } }

		#region Members
		private readonly Stopwatch _stopwatch;

		public bool IsRunning { get { return _stopwatch.IsRunning; } }
		public long ElapsedMillis { get { return _stopwatch.ElapsedMilliseconds; } }
		public long ElapsedTicks { get { return _stopwatch.ElapsedTicks; } }
		public TimeSpan Elapsed { get { return _stopwatch.Elapsed; } }
		public int Days { get { return _stopwatch.Elapsed.Days; } }
		public int Hours { get { return _stopwatch.Elapsed.Hours; } }
		public int Minutes { get { return _stopwatch.Elapsed.Minutes; } }
		public int Seconds { get { return _stopwatch.Elapsed.Seconds; } }
		public int Milliseconds { get { return _stopwatch.Elapsed.Milliseconds; } }
		public double TotalDays { get { return _stopwatch.Elapsed.TotalDays; } }
		public double TotalHours { get { return _stopwatch.Elapsed.TotalHours; } }
		public double TotalMinutes { get { return _stopwatch.Elapsed.TotalMinutes; } }
		public double TotalSeconds { get { return _stopwatch.Elapsed.TotalSeconds; } }
		public double TotalMilliseconds { get { return _stopwatch.Elapsed.TotalMilliseconds; } }
		#endregion

		public Timer(bool start = false)
		{
			_stopwatch = new Stopwatch();
			if (start)
				_stopwatch.Start();
		}

		public void Start()
		{
			_stopwatch.Start();
		}
		public TimeSpan Reset()
		{
			TimeSpan elap = Elapsed;
			_stopwatch.Reset();
			return elap;
		}
		public TimeSpan Restart()
		{
			TimeSpan elap = Elapsed;
			_stopwatch.Restart();
			return elap;
		}
		public TimeSpan Stop()
		{
			TimeSpan elap = Elapsed;
			_stopwatch.Stop();
			return elap;
		}
	}
}