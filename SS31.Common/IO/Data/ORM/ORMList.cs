using System;
using System.Collections.Generic;

namespace SS31.Common.IO.ORM
{
	// List of ORMIntegers
	public class ORMIntegerList : ORMList<ORMInteger>
	{
		public ORMIntegerList(string name, List<ORMInteger> lst = null) : base(name, lst) { }
	}

	// List of ORMDecimals
	public class ORMDecimalList : ORMList<ORMDecimal>
	{
		public ORMDecimalList(string name, List<ORMDecimal> lst = null) : base(name, lst) { }
	}

	// List of ORMStrings
	public class ORMStringList : ORMList<ORMString>
	{
		public ORMStringList(string name, List<ORMString> lst = null) : base(name, lst) { }
	}

	// List of ORMBooleans
	public class ORMBooleanList : ORMList<ORMBoolean>
	{
		public ORMBooleanList(string name, List<ORMBoolean> lst = null) : base(name, lst) { }
	}

	// List of ORMLists
	public class ORMListList<T> : ORMList<ORMList<T>>
		where T : IORMNode
	{
		public ORMListList(string name, List<ORMList<T>> lst = null) : base(name, lst) { }
	}

	// List of ORMDictionarys
	public class ORMDictionaryList : ORMList<ORMMap>
	{
		public ORMDictionaryList(string name, List<ORMMap> lst = null) : base(name, lst) { }
	}
}