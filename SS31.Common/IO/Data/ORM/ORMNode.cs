using System;
using System.Collections.Generic;

namespace SS31.Common.IO.ORM
{
	// Base interface allowing for lists of generics
	public interface IORMNode
	{
		string Name { get; }
	}

	// Represents a data node in an Object Representation Model tree
	public class ORMNode<T> : IORMNode
	{
		public string Name { get; private set; }
		public T Value { get; set; }

		protected ORMNode(string name, T initialValue = default(T))
		{
			Name = name;
			Value = initialValue;
		}

		public static implicit operator T(ORMNode<T> n)
		{
			return n.Value;
		}
	}

	// Node for an integer value
	public class ORMInteger : ORMNode<int>
	{
		public ORMInteger(string name, int i) : base (name, i) { }
	}

	// Node for a floating point value
	public class ORMDecimal : ORMNode<double>
	{
		public ORMDecimal(string name, double d) : base(name, d) { }
	}

	// Node for a string value
	public class ORMString : ORMNode<string>
	{
		public ORMString(string name, string s) : base(name, s) { }
	}

	// Node for a boolean value
	public class ORMBoolean : ORMNode<bool>
	{
		public ORMBoolean(string name, bool b) : base(name, b) { }
	}

	// Node for a list of nodes of the same type.
	// Specific types of lists are defined in ORMList.cs
	public class ORMList<T> : ORMNode<List<T>>
		where T : IORMNode
	{
		public ORMList(string name, List<T> lst = null) : base(name, lst) 
		{
			if (lst == null)
				Value = new List<T>();
		}

		public T Get(int i)
		{
			return Value[i];
		}

		public T this[int i]
		{
			get { return Value[i]; }
			set { Value[i] = value; }
		}

		public int Count { get { return Value.Count; } }
	}

	// Node for an associative list of nodes
	public class ORMDictionary : ORMNode<Dictionary<string, IORMNode>>
	{
		public ORMDictionary(string name, Dictionary<string, IORMNode> dict = null) : base (name, dict)
		{
			if (dict == null)
				Value = new Dictionary<string, IORMNode>();
		}

		public bool HasNode(string s)
		{
			return Value.ContainsKey(s);
		}

		public IORMNode Get(string s)
		{
			return Value[s];
		}

		public void AddNode(IORMNode node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			Value.Add(node.Name, node);
		}

		public IORMNode this[string s]
		{
			get { return Value[s]; }
			set { Value[s] = value; }
		}

		public int Count { get { return Value.Count; } }
	}
}