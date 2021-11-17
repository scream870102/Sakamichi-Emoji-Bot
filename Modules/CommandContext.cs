namespace Seb.Modules
{
	class CommandContext
	{
		public string[] Tags { get; private set; }
		public string Url { get; private set; }
		public CommandContext(string[] tags, string url)
		{
			Tags = tags;
			Url = url;
		}
		public static bool TryParse(string origin, out CommandContext result)
		{
			var splitText = origin.Split('|');
			var url = splitText[^1];
			var tags = splitText[..^1];
			result = new CommandContext(tags, url);
			return true;
		}
	}
}