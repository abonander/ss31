using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using YamlDotNet.Serialization;

namespace SS31.Common.Config
{
	public delegate void HandleConfigValueChange(string name, object oldVal, object newVal);

	public interface IConfiguration
	{
		event HandleConfigValueChange OnValueChange;

		SerializeAs GetSerializeType();
	}
}