using System;
using System.Collections.Generic;
using System.IO;
using SS31.Common.IO.ORM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SS31.Common.IO
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
						ret.Value.Add(node.Name, node);
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

		#region Private Helper Methods
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
						return new ORMInteger("", (int)val.Value);
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
							list.Value.Add(cnode);
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
							map.Value.Add(node.Name, node);
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