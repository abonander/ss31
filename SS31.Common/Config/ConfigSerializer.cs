using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SS31.Common
{
	public enum SerializeAs
	{
		Json,
		Yaml,
		Xml
	}

	internal static class ConfigSerializer
	{
		public static T DeserializeYamlObject<T>(string input)
		{
			using (StringReader reader = new StringReader(input))
			{
				Deserializer serial = new Deserializer(namingConvention: new CamelCaseNamingConvention());
				T t = serial.Deserialize<T>(reader);
				return t;
			}
		}

		public static string SerializeYamlObject<T>(T obj)
		{
			using (StringWriter writer = new StringWriter())
			{
				Serializer serial = new Serializer();
				serial.Serialize(writer, obj);
				return writer.ToString();
			}
		}

		public static T DeserializeJsonObject<T>(string input)
		{
			return (T)JsonConvert.DeserializeObject<T>(input);
		}

		public static string SerializeJsonObject<T>(T obj)
		{
			return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
		}

		public static T DeserializeXmlObject<T>(string input)
		{
			using (StringReader reader = new StringReader(input))
			{
				XmlSerializer serial = new XmlSerializer(typeof(T));
				T t = (T)serial.Deserialize(reader);
				return t;
			}
		}

		public static string SerializeXmlObject<T>(T obj)
		{
			using (StringWriter writer = new StringWriter())
			{
				XmlSerializer serial = new XmlSerializer(typeof(T));
				serial.Serialize(writer, obj);
				writer.Flush();
				return writer.ToString();
			}
		}
	}
}