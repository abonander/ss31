using System;
using System.Collections.Generic;

namespace SS31.Common.IO.ORM
{
	// Base interface allowing for lists of generics
	public interface IORMNode
	{
		string Name { get; set; }
		ORMNodeType Type { get; }
		object Value { get; set; }
	}

	// Represents a data node in an Object Representation Model tree
	public abstract class ORMNode<T> : IORMNode
	{
		public string Name { get; set; }
		public object Value { get; set; }
		public abstract ORMNodeType Type { get; }

		protected ORMNode(string name, T initialValue = default(T))
		{
			Name = name;
			Value = initialValue;
		}

		public static implicit operator T(ORMNode<T> n)
		{
			return (T)n.Value;
		}

		public T GetValue()
		{
			return (T)Value;
		}
	}

	// Node for an integer value
	public class ORMInteger : ORMNode<long>
	{
		public override ORMNodeType Type { get { return ORMNodeType.Integer; } }

		public ORMInteger(string name, long i) : base (name, i) { }
	}

	// Node for a floating point value
	public class ORMDecimal : ORMNode<double>
	{
		public override ORMNodeType Type { get { return ORMNodeType.Decimal; } }

		public ORMDecimal(string name, double d) : base(name, d) { }
	}

	// Node for a string value
	public class ORMString : ORMNode<string>
	{
		public override ORMNodeType Type { get { return ORMNodeType.String; } }

		public ORMString(string name, string s) : base(name, s) { }
	}

	// Node for a boolean value
	public class ORMBoolean : ORMNode<bool>
	{
		public override ORMNodeType Type { get { return ORMNodeType.Boolean; } }

		public ORMBoolean(string name, bool b) : base(name, b) { }
	}

	// Node for a null value
	public class ORMNull : ORMNode<object>
	{
		public override ORMNodeType Type { get { return ORMNodeType.Null; } }

		public ORMNull(string name) : base(name, null) { }
	}

	// Node for a list of nodes of the same type.
	// Specific types of lists are defined in ORMList.cs
	public class ORMList : ORMNode<List<IORMNode>>
	{
		public override ORMNodeType Type { get { return ORMNodeType.List; } }

		public ORMList(string name, List<IORMNode> lst = null) : base(name, lst) 
		{
			if (lst == null)
				Value = new List<IORMNode>();
		}

		public IORMNode Get(int i)
		{
			return GetValue()[i];
		}

		public IORMNode this[int i]
		{
			get { return GetValue()[i]; }
			set { GetValue()[i] = value; }
		}

		public int Count { get { return GetValue().Count; } }
	}

	// Node for an associative list of nodes
	public class ORMMap : ORMNode<Dictionary<string, IORMNode>>
	{
		public override ORMNodeType Type { get { return ORMNodeType.Map; } }

		public ORMMap(string name, Dictionary<string, IORMNode> dict = null) : base (name, dict)
		{
			if (dict == null)
				Value = new Dictionary<string, IORMNode>();
		}

		public bool HasNode(string s)
		{
			return GetValue().ContainsKey(s);
		}

		public IORMNode Get(string s)
		{
			return GetValue()[s];
		}

		public void AddNode(IORMNode node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			GetValue().Add(node.Name, node);
		}

		public IORMNode this[string s]
		{
			get { return GetValue()[s]; }
			set { GetValue()[s] = value; }
		}

		public int Count { get { return GetValue().Count; } }
		public bool Empty { get { return GetValue().Count < 1; } }
	}
}