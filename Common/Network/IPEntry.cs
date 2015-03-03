using System;

namespace SS31.Network
{
	public struct IPEntry
	{
		public const ushort SS_DEFAULT_PORT = 8100;

		public byte N1;
		public byte N2;
		public byte N3;
		public byte N4;
		public ushort Port;

		// Must have a format of "XXX.XXX.X[XX].X[XX][:XXXXX]"
		public IPEntry(string s)
		{
			string[] nums = s.Split(new char[]{'.', ':'});
			if (nums.Length < 4)
				throw new ArgumentException("The ip string \"" + s + "\" was malformatted.");
			bool hasPort = nums.Length == 5;
			try
			{
				N1 = Convert.ToByte(nums[0]);
				N2 = Convert.ToByte(nums[1]);
				N3 = Convert.ToByte(nums[2]);
				N4 = Convert.ToByte(nums[3]);
			}
			catch (Exception)
			{
				throw new ArgumentException("The ip string \"" + s + "\" is malformatted or has invalid numbers.");
			}

			if (hasPort)
			{
				try
				{
					Port = Convert.ToUInt16(nums[4]);
				}
				catch (Exception)
				{
					throw new ArgumentException("The port for the ip string \"" + s + "\" was malformatted or has an invalid value.");
				}
			}
			else
				Port = SS_DEFAULT_PORT;
		}

		public IPEntry(byte n1, byte n2, byte n3, byte n4, ushort port = SS_DEFAULT_PORT)
		{
			N1 = n1;
			N2 = n2;
			N3 = n3;
			N4 = n4;
			Port = port;
		}
	}
}