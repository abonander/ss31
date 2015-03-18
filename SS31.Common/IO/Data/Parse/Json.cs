using System;
using System.Collections.Generic;
using System.IO;
using SS31.Common.IO.ORM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SS31.Common.IO
{
	public class JsonParser
	{
		public List<ORMObject> ParseJsonString(string s)
		{
			List<ORMObject> retlist = new List<ORMObject>();

			using (StringReader stringReader = new StringReader(s))
			using (JsonTextReader reader = new JsonTextReader(stringReader))
			{
				JObject obj = (JObject)JToken.ReadFrom(reader);
			}
		}
	}
}