using System;
using System.Collections.Generic;
using System.IO;
using SS31.Core.IO.ORM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SS31.Core.IO
{
	public static class JsonParser
	{
		public static ORMObject ParseJsonString(string s)
		{
			ORMObject ret = new ORMObject("root");

			try
			{
				using (StringReader stringReader = new StringReader(s))
				using (JsonTextReader reader = new JsonTextReader(stringReader))
				{
					JObject obj = (JObject)JToken.ReadFrom(reader);
					foreach (JToken t in obj.Children())
					{
						IORMNode node = parseToken(t);
						ret.GetValue().Add(node.Name, node);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError("The json parser encountered an exception while parsing the json code.");
				Logger.LogException(ex);
				return null;
			}

			return ret;
		}

		public static string ToJsonString(ORMObject obj)
		{
			try
			{
				JTokenWriter writer = new JTokenWriter();
				writer.WriteStartObject();
				foreach (IORMNode node in obj.Children.Values)
				{
					parseNode(node, writer);
				}
				writer.WriteEndObject();
				return writer.Token.ToString();
			}
			catch (Exception ex)
			{
				Logger.LogError("The json parser encountered an exception while trying to generate a Json string from an ORM tree.");
				Logger.LogException(ex);
				return "{}";
			}
		}

		public static string SerializeObject<T>(T obj)
		{
			try
			{
				return JsonConvert.SerializeObject(obj, Formatting.Indented);
			}
			catch (Exception ex)
			{
				Logger.LogError("Could not serialize the object. An exception was thrown.");
				Logger.LogException(ex);
				return "{}";
			}
		}

		public static T DeserializeObject<T>(string s)
		{
			try
			{
				T obj = JsonConvert.DeserializeObject<T>(s);
				return obj;
			}
			catch (Exception ex)
			{
				Logger.LogError("Could not deserialize the json string. An exception was thrown.");
				Logger.LogException(ex);
				return default(T);
			}
		}

		#region Private Helper Methods
		private static void writeValueNode<T>(string name, T value, JTokenWriter writer)
		{
			if (!String.IsNullOrEmpty(name))
				writer.WritePropertyName(name);
			writer.WriteValue(value);
		}

		private static void parseNode(IORMNode node, JTokenWriter writer)
		{
			if (node == null)
				throw new ArgumentNullException("node");
			if (writer == null)
				throw new ArgumentNullException("writer");

			switch (node.Type)
			{
			case ORMNodeType.Boolean:
				{
					ORMBoolean value = (ORMBoolean)node;
					writeValueNode<bool>(value.Name, value.GetValue(), writer);
					return;
				}
			case ORMNodeType.Decimal:
				{
					ORMDecimal value = (ORMDecimal)node;
					writeValueNode<double>(value.Name, value.GetValue(), writer);
					return;
				}
			case ORMNodeType.Integer:
				{
					ORMInteger value = (ORMInteger)node;
					writeValueNode<long>(value.Name, value.GetValue(), writer);
					return;
				}
			case ORMNodeType.Null:
				{
					ORMNull value = (ORMNull)node;
					if (!String.IsNullOrEmpty(value.Name))
						writer.WritePropertyName(value.Name);
					writer.WriteNull();
					return;
				}
			case ORMNodeType.String:
				{
					ORMString value = (ORMString)node;
					writeValueNode<string>(value.Name, value.GetValue(), writer);
					return;
				}
			case ORMNodeType.List:
				{
					ORMList list = (ORMList)node;
					if (!String.IsNullOrEmpty(list.Name))
						writer.WritePropertyName(list.Name);
					writer.WriteStartArray();
					foreach (IORMNode n in list.GetValue())
						parseNode(n, writer);
					writer.WriteEndArray();
					return;
				}
			case ORMNodeType.Map:
				{
					ORMMap map = (ORMMap)node;
					if (!String.IsNullOrEmpty(map.Name))
						writer.WritePropertyName(map.Name);
					writer.WriteStartObject();
					foreach (IORMNode n in map.GetValue().Values)
						parseNode(n, writer);
					writer.WriteEndObject();
					return;
				}
			}
		}

		private static IORMNode parseToken(JToken t)
		{
			if (t == null)
				throw new ArgumentNullException("t");

			switch (t.Type)
			{
				case JTokenType.Property:
					{
						JProperty prop = (JProperty)t;
						IORMNode val = parseToken(prop.Value);
						val.Name = prop.Name;
						return val;
					}
				case JTokenType.Boolean:
					{
						JValue val = (JValue)t;
						return new ORMBoolean("", (bool)val.Value);
					}
				case JTokenType.Float:
					{
						JValue val = (JValue)t;
						return new ORMDecimal("", (double)val.Value);
					}
				case JTokenType.Integer:
					{
						JValue val = (JValue)t;
						return new ORMInteger("", (long)val.Value);
					}
				case JTokenType.Null:
					{
						return new ORMNull("");
					}
				case JTokenType.String:
					{
						JValue val = (JValue)t;
						return new ORMString("", (string)val.Value);
					}
				case JTokenType.Array:
					{
						JArray array = (JArray)t;
						ORMList list = new ORMList("");
						foreach (JToken token in array.Children())
						{
							IORMNode cnode = parseToken(token);
							list.GetValue().Add(cnode);
						}
						return list;
					}
				case JTokenType.Object: // This is equivalent to a map
					{
						JObject obj = (JObject)t;
						ORMMap map = new ORMMap("");
						foreach (JToken token in obj.Children())
						{
							IORMNode node = parseToken(token);
							map.GetValue().Add(node.Name, node);
						}
						return map;
					}
				default:
					Logger.LogError("Json parser encountered an unknown node type: " + t.Type + ".");
					return null;
			}
		}
		#endregion
	}
}