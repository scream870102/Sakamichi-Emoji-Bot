using System.Text;
using Discord;
using Discord.Commands;
using Seb.Misc;
using Seb.Sheet;

namespace Seb.Modules
{
	[NamedArgumentType]
	public class ListArguments
	{
		public string Author { get; set; }
		public string Tags { get; set; }
	}

	[Name("List")]
	public class ListModule : ModuleBase<SocketCommandContext>
	{
		private const int embedMaxFieldCount = 25;

		[Command("list"), Alias("l")]
		[Summary("List all gif")]
		public Task ListTask([Remainder] string tags)
		{
			var tag = TagParser.ParseTag(tags);
			return ListTaskAsync(tag);
		}
		public async Task ListTaskAsync(string[] tags)
		{
			var rawData = DataHandler.GetRawValues(Context.Guild.Id, tags);
			if (rawData == null || rawData.Count == 0)
			{
				await ReplyAsync("There is no any matched content with tags.");
				return;
			}

			await ReplyWithListEmbed(rawData);
		}

		[Command("list"), Alias("l")]
		[Summary("List all gif")]
		public Task ListArgTask(ListArguments arg)
		{
			return ListArgTaskAsync(arg);
		}
		public async Task ListArgTaskAsync(ListArguments arg)
		{
			List<SheetRawValue> tagFilterResult = null;
			List<SheetRawValue> authorFilterResult = null;
			var isTagFilterExist = false;
			var isAuthorFilterExist = false;
			var guild = Context.Guild.Id;
			if (arg.Tags != null && arg.Tags.Length != 0)
			{
				isTagFilterExist = true;
				tagFilterResult = DataHandler.GetRawValues(guild, arg.Tags);
				if (tagFilterResult == null || tagFilterResult.Count == 0)
				{
					await ReplyAsync($"There is no any matched content with tags.");
					return;
				}
			}
			if (!string.IsNullOrEmpty(arg.Author) && MentionUtils.TryParseUser(arg.Author, out var authorId))
			{
				isAuthorFilterExist = true;
				authorFilterResult = DataHandler.GetRawValuesByAuthor(authorId, guild);
				if (authorFilterResult == null || authorFilterResult.Count == 0)
				{
					await ReplyAsync($"There is no any matched content with author.");
					return;
				}
			}
			if (isAuthorFilterExist && isTagFilterExist && tagFilterResult != null && authorFilterResult != null)
			{
				var result = tagFilterResult.Intersect(authorFilterResult);
				if (result.Count() != 0)
				{
					await ReplyWithListEmbed(result.ToList());
					return;
				}
				await ReplyAsync("There is no any matched content with tags and author");
				return;
			}
			if (isAuthorFilterExist && authorFilterResult != null && authorFilterResult.Count != 0)
			{
				await ReplyWithListEmbed(authorFilterResult);
				return;
			}
			if (isTagFilterExist && tagFilterResult != null && tagFilterResult.Count != 0)
			{
				await ReplyWithListEmbed(tagFilterResult);
				return;
			}
			await ReplyAsync($"There is no any matched content");
		}

		private async Task ReplyWithListEmbed(List<SheetRawValue> rawData)
		{
			var allRawData = rawData.ToArray();
			var totalPage = allRawData.Length / embedMaxFieldCount + 1;
			for (int page = 0; page < totalPage; page++)
			{
				var startIndex = page * embedMaxFieldCount;
				var endIndex = startIndex + embedMaxFieldCount;
				if (endIndex >= allRawData.Length)
				{
					endIndex = allRawData.Length;
				}
				var data = allRawData[startIndex..endIndex];
				var title = $"Page : {page + 1}/{totalPage}";
				Embed embed = CreateListContentEmbed(data,Context,title);
				await ReplyAsync(null, false, embed);
			}
		}

		private static Embed CreateListContentEmbed(SheetRawValue[] allRawData, SocketCommandContext context, string titleAppend = null)
		{
			var title = $"{context.Guild.Name} {titleAppend ?? " "}";
			var builder = EmbedCreator.CreateBasicEmbedBuilder(title, true);
			builder.WithImageUrl(@"https://i.imgur.com/8LFgtgg.jpg");
			var index = 0;
			foreach (var rawData in allRawData)
			{
				index++;
				var allTags = new StringBuilder();
				foreach (var tag in rawData.Tags)
				{
					allTags.Append($"{tag}  ,");
				}
				builder.AddField(index.ToString(), $" Tags  :  {allTags} \n Author : {MentionUtils.MentionUser(rawData.Author)} \nId  :  {rawData.Id} \nUrl  :  {rawData.Url}\n");
			}
			var embed = builder.Build();
			return embed;
		}
	}
}