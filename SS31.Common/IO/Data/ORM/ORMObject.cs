using System;
using System.Collections.Generic;
using System.Linq;

namespace SS31.Common.IO.ORM
{
	// Class that represents the root of a set of data parsed from Json or Yaml
	public class ORMObject
	{
		public Dictionary<string, IORMNode> Children { get; private set; }

		public int Count { get { return Children.Count; } }
		public bool Empty { get { return Children.Count < 1; } }

		public ORMObject()
		{
			Children = new Dictionary<string, IORMNode>();
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