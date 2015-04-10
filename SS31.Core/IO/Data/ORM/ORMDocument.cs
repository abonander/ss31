using System;

namespace SS31.Core.IO.ORM
{
	// Contains an instance of a document of Yaml or Json, with a ORM tree representing the data.
	public class ORMDocument
	{
		public ORMObject Root { get; private set; }

		public bool Initialized { get { return Root != null; } }

		public ORMDocument()
		{
			Root = null;
		}

		public void Load(string path)
		{
			// TODO: load
		}

		public void Save(string path)
		{
			// TODO: save
		}
	}
}