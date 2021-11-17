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

		public static SheetRawValue GetRawValue(ulong guild, params string[] tags)
		{
			if (!dic.ContainsKey(guild))
			{
				return null;
			}
			var guildDic = dic[guild];
			var toCheckSet = new List<HashSet<Guid>>();
			foreach (var tag in tags)
			{
				if (guildDic.ContainsKey(tag))
				{
					toCheckSet.Add(guildDic[tag]);
				}
			}
			if (toCheckSet.Count == 0)
			{
				return null;
			}
			IEnumerable<Guid> ids = toCheckSet[0];
			for (int i = 1; i < toCheckSet.Count; i++)
			{
				ids = ids.Intersect(toCheckSet[i]);
			}
			if (!ids.Any())
			{
				return null;
			}
			var id = ids.ElementAt(random.Next(ids.Count()));
			var result = raw[guild][id] ?? null;
			return result;
		}

		public static List<SheetRawValue> GetRawValuesByGuild(ulong guild)
		{
			var result = new Dictionary<Guid, SheetRawValue>();
			if (!dic.ContainsKey(guild))
			{
				return null;
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
	}
}