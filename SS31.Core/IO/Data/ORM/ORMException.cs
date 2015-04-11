using System;

namespace SS31.Core.IO.ORM
{
	public class DuplicateKeyException : Exception
	{
		public readonly string Key;

		public override string Message
		{
			get
			{
				return "The collection already contains a node with the key " + Key;
			}
		}

		public DuplicateKeyException(string key)
		{
			Key = key;
		}
	}
}