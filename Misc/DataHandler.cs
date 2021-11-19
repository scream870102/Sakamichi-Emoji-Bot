using Seb.Sheet;
namespace Seb.Misc
{
	static class DataHandler
	{
		private static readonly Random random = new Random();
		private static Dictionary<ulong, Dictionary<Guid, SheetRawValue>> raw;
		// Key: guild
		//		Key: tag
		//			Key: id
		private static Dictionary<ulong, Dictionary<string, HashSet<Guid>>> dic;
		public static List<SheetRawValue> GetRawValues(ulong guild, params string[] tags)
		{
			var result = new List<SheetRawValue>();
			if (!dic.ContainsKey(guild))
			{
				return result;
			}
			var guildDic = dic[guild];
			var toCheckSet = new List<HashSet<Guid>>();
			foreach (var tag in tags)
			{
				if (guildDic.ContainsKey(tag))
				{
					toCheckSet.Add(guildDic[tag]);
				}
				else
				{
					toCheckSet.Add(new HashSet<Guid>());
				}
			}
			if (toCheckSet.Count == 0)
			{
				return result;
			}
			IEnumerable<Guid> ids = toCheckSet[0];
			for (int i = 1; i < toCheckSet.Count; i++)
			{
				ids = ids.Intersect(toCheckSet[i]);
			}
			if (!ids.Any())
			{
				return result;
			}
			foreach (var id in ids)
			{
				if (raw[guild].ContainsKey(id))
				{
					result.Add(raw[guild][id]);
				}
			}
			return result;
		}

		public static SheetRawValue GetRawValue(ulong guild, params string[] tags)
		{
			var values = GetRawValues(guild, tags);
			if (values.Count == 0)
			{
				return null;
			}
			return values.ElementAt(random.Next(values.Count));
		}

		public static List<SheetRawValue> GetRawValuesByGuild(ulong guild)
		{
			var result = new Dictionary<Guid, SheetRawValue>();
			if (!dic.ContainsKey(guild))
			{
				return result.Values.ToList();
			}
			foreach (var tagPair in dic[guild])
			{
				foreach (var id in tagPair.Value)
				{
					if (raw[guild].ContainsKey(id) && !result.ContainsKey(id))
					{
						result.Add(id, raw[guild][id]);
					}
				}
			}
			return result.Values.ToList();
		}

		public static List<SheetRawValue> GetRawValuesByAuthor(ulong id, ulong guild)
		{
			var result = new List<SheetRawValue>();
			if (!raw.ContainsKey(guild))
			{
				return result;
			}
			foreach (var valuePair in raw[guild])
			{
				if (valuePair.Value.Author == id)
				{
					result.Add(valuePair.Value);
				}
			}
			return result;
		}

		public static List<SheetRawValue> GetRawValuesByContent(string content, ulong guild)
		{
			var result = new List<SheetRawValue>();
			if (!raw.ContainsKey(guild))
			{
				return result;
			}
			foreach (var valuePair in raw[guild])
			{
				if (valuePair.Value.Url == content)
				{
					result.Add(valuePair.Value);
				}
			}
			return result;
		}

		public static void Init()
		{
			var rawData = SpreadSheet.RawValues;
			raw = new Dictionary<ulong, Dictionary<Guid, SheetRawValue>>();
			foreach (var data in rawData)
			{
				raw.Add(data.Key, new Dictionary<Guid, SheetRawValue>());
				foreach (var rawValue in data.Value)
				{
					raw[data.Key].Add(rawValue.Id, rawValue);
				}
			}
			InitDic();
		}

		public static void AddValue(SheetRawValue content, ulong guild)
		{
			//Check guild
			if (!dic.ContainsKey(guild))
			{
				dic.Add(guild, new Dictionary<string, HashSet<Guid>>());
			}

			//Check tags
			foreach (var tag in content.Tags)
			{
				var guildDic = dic[guild];
				if (!guildDic.ContainsKey(tag))
				{
					guildDic.Add(tag, new HashSet<Guid>());
				}
				var contentDic = dic[guild][tag];
				if (!contentDic.Contains(content.Id))
				{
					contentDic.Add(content.Id);
				}
			}
			if (!raw.ContainsKey(guild))
			{
				raw.Add(guild, new Dictionary<Guid, SheetRawValue>());
			}
			if (!raw[guild].ContainsKey(content.Id))
			{
				raw[guild].Add(content.Id, content);
			}
		}

		private static void InitDic()
		{
			dic = new Dictionary<ulong, Dictionary<string, HashSet<Guid>>>();
			foreach (var data in raw)
			{
				foreach (var content in data.Value)
				{
					AddValue(content.Value, data.Key);
				}
			}
		}

		//TODO: Add Tag and Try to update the cache
		// public static bool TryUpdateValue(Guid id, string[] tags, ulong guild)
		// {
		// 	if (raw[guild].ContainsKey(id))
		// 	{
		// 		var oldValue = raw[guild][id];
		// 		var oldTags = oldValue.Tags;
		// 		raw[guild].Remove(id);
		// 		var newTags = oldTags.Intersect(tags).ToArray();
		// 		var newValue = new SheetRawValue(id.ToString(), oldValue.Author.ToString(), oldValue.Url, newTags);
		// 		raw[guild].Add(id, newValue);
		// 		UpdateDic(newValue, guild);
		// 		return true;
		// 	}
		// 	return false;
		// }

		// private static void UpdateDic(SheetRawValue content, ulong guild)
		// {
		// 	foreach (var tag in content.Tags)
		// 	{
		// 		var guildDic = dic[guild];
		// 		if (!guildDic.ContainsKey(tag))
		// 		{
		// 			guildDic.Add(tag, new HashSet<Guid>());
		// 		}
		// 		var contentDic = dic[guild][tag];
		// 		if (!contentDic.Contains(content.Id))
		// 		{
		// 			contentDic.Add(content.Id);
		// 		}
		// 	}
		// }

	}
}