using Discord.Commands;
using Discord;
using Seb.Misc;
using Seb.Sheet;
using System.Linq;
using System.Text;

namespace Seb.Modules
{
	[NamedArgumentType]
	public class ListArguments
	{
		public string Author { get; set; }
		public string[] Tags { get; set; }
	}
	[Name("File")]
	public class FileModule : ModuleBase<SocketCommandContext>
	{
		private const int embedMaxFieldCount = 25;

		[Command("add"), Alias("a")]
		[Summary("Add the gif")]
		public Task AddTask(string[] tags)
		{
			return AddTaskAsync(tags);
		}

		public async Task AddTaskAsync(string[] tags)
		{
			var guild = Context.Guild.Id;
			var url = tags[^1];
			var tag = tags[..^1];
			var contentCheck = DataHandler.GetRawValuesByContent(url, guild);
			if (contentCheck.Count > 0)
			{

			}
			var id = Guid.NewGuid().ToString();
			if (!SpreadSheet.TryAdd(new SheetRawValue(id, Context.User.Id.ToString(), url, tag), guild))
			{
				await ReplyAsync($"Can't add new content to guild database");
				return;
			}

			var allTags = new StringBuilder();
			foreach (var t in tag)
			{
				allTags.Append($"{t}  ,");
			}
			var footer = new EmbedFooterBuilder()
			.WithIconUrl(@"https://i.imgur.com/oC0Yo5s.png")
			.WithText($"Created by scream870102");
			var builder = new EmbedBuilder()
				.WithThumbnailUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
				.WithDescription($"Congratulation {MentionUtils.MentionUser(Context.User.Id)}!!!\nYou are the best momoko oshi")
				.WithColor(new Color(240, 93, 154))
				.AddField("Id", id)
				.AddField("Tag", allTags.ToString())
				.AddField("Url", url)
				.WithTitle("Add new content successful")
				.WithFooter(footer)
				.WithCurrentTimestamp();

			var embed = builder.Build();
			await ReplyAsync(null, false, embed);
		}

		[Command("list"), Alias("l")]
		[Summary("List all gif")]
		public Task ListTask(string[] tags)
		{
			return ListTaskAsync(tags);
		}
		public async Task ListTaskAsync(string[] tags)
		{
			var rawData = DataHandler.GetRawValues(Context.Guild.Id, tags);
			if (rawData == null)
			{
				await ReplyAsync("There is no any context in this guild");
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


		private Embed CreateAddEmbed(SheetRawValue[] allRawData, string titleAppend = null)
		{
			var footer = new EmbedFooterBuilder()
			.WithIconUrl(@"https://i.imgur.com/oC0Yo5s.png")
			.WithText($"Created by scream870102");
			var builder = new EmbedBuilder()
			.WithColor(new Color(240, 93, 154))
			.WithTitle($"{Context.Guild.Name} {titleAppend ?? " "}")
			.WithImageUrl(@"https://i.imgur.com/8LFgtgg.jpg")
			.WithFooter(footer)
			.WithCurrentTimestamp();
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

		private Embed CreateListEmbed(SheetRawValue[] allRawData, int totalPage, int page)
		{
			var startIndex = page * 25;
			var endIndex = startIndex + 25;
			if (endIndex >= allRawData.Length)
			{
				endIndex = allRawData.Length;
			}
			var data = allRawData[startIndex..endIndex];
			Embed embed = this.CreateAddEmbed(data, $"Page : {page + 1}/{totalPage}");
			return embed;
		}

		private async Task ReplyWithListEmbed(List<SheetRawValue> rawData)
		{
			var allRawData = rawData.ToArray();
			var totalPage = allRawData.Length / embedMaxFieldCount + 1;
			for (int i = 0; i < totalPage; i++)
			{
				Embed embed = CreateListEmbed(allRawData, totalPage, i);
				await ReplyAsync(null, false, embed);
			}
		}
	}
}
