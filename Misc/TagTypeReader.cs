using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;

namespace Seb.Misc
{
	public class TagTypeReader : TypeReader
	{
		public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
		{
			//TODO:
			// var tags = Regex.Matches(input, @"[\""].+?[\""]|[^ ]+")
			// 	.Cast<Match>()
			// 	.Select(m => m.Value)
			// 	.ToArray();
			var tags = input.Split('|', StringSplitOptions.RemoveEmptyEntries);
			if (tags.Length > 0)
				return Task.FromResult(TypeReaderResult.FromSuccess(tags));

			return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as a boolean."));
		}
	}
}
