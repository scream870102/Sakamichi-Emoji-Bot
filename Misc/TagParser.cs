using System.Text.RegularExpressions;

namespace Seb.Misc
{
	public static class TagParser
	{
		public static string[] ParseTag(string input)
		{
			var tags = Regex.Matches(input, @"[\""].+?[\""]|[^ ]+")
				.Cast<Match>()
				.Select(m => m.Value)
				.ToArray();
			return tags;
		}
	}

}