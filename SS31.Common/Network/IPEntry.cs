using System;

namespace SS31.Common.Network
{
	// More concrete way to hold ip addresses. Redundant and will probably be removed soon.
	public struct IPEntry
	{
		public byte N1;
		public byte N2;
		public byte N3;
		public byte N4;
		public ushort Port;

		public string IPString { get { return String.Format("{0}.{1}.{2}.{3}", N1, N2, N3, N4); } }
		public bool IsLAN { get { return false; } } // TODO: Compare to the local network (dont assume 192.168.*.*)
		public bool IsLocal { get { return N1 == 127 && N2 == 0 && N3 == 0 && N4 == 1; } }

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
				Port = SharedConstants.SS_DEFAULT_PORT;
		}
		public IPEntry(byte n1, byte n2, byte n3, byte n4, ushort port = SharedConstants.SS_DEFAULT_PORT)
		{
			N1 = n1;
			N2 = n2;
			N3 = n3;
			N4 = n4;
			Port = port;
		}

		public override string ToString()
		{
			return String.Format("{0}.{1}.{2}.{3}:{4}", N1, N2, N3, N4, Port);
		}

		// This is nowhere near a perfect hash function. In fact, it will start to collide for any ip addresses that have the same first three numbers.
		// But for now, it will work.
		public override int GetHashCode()
		{
			return ((int)(N1 << 24) & (int)(N2 << 16) & (int)(N3 << 8) & (int)(N4)) + Port.GetHashCode();
		}
	}
}