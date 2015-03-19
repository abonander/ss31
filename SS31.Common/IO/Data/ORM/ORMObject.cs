using System;
using System.Collections.Generic;
using System.Linq;

namespace SS31.Common.IO.ORM
{
	// Class that represents the root of a set of data parsed from Json or Yaml
	public class ORMObject : ORMMap
	{
		public Dictionary<string, IORMNode> Children { get { return Value; } }

		public ORMObject(string name = null) : base(name)
		{

		}

		public void AddChild(IORMNode node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			if (HasChild(node.Name))
				throw new DuplicateKeyException(node.Name);

			Children.Add(node.Name, node);
		}

		public bool HasChild(string s)
		{
			IORMNode node = (from n in Children
			                 where n.Key == s
			                 select n.Value).FirstOrDefault();
			return node != null;
		}
	}
}