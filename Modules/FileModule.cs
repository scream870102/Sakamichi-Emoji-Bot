using Discord.Commands;
using Discord;
using Seb.Misc;
using Seb.Sheet;
using System.Text;

namespace Seb.Modules
{
	[Name("File")]
	public class FileModule : ModuleBase<SocketCommandContext>
	{
		private const int embedMaxFieldCount = 25;

		[Command("add"), Alias("a")]
		[Summary("Add the gif")]
		public Task AddTask([Remainder] string tag)
		{
			return AddTaskAsync(tag);
		}

		public async Task AddTaskAsync([Remainder] string data)
		{
			if (!CommandContext.TryParse(data, out var context))
			{
				await ReplyAsync($"{data} is not a valid content");
				return;
			}
			var id = Guid.NewGuid().ToString();
			if (!SpreadSheet.TryAdd(new SheetRawValue(id, Context.User.Id.ToString(), context.Url, context.Tags), Context.Guild.Id))
			{
				await ReplyAsync($"{id} already exists. Can't add new gif");
				return;
			}

			var allTags = new StringBuilder();
			foreach (var tag in context.Tags)
			{
				allTags.Append($"{tag}  ,");
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
				.AddField("Url", context.Url)
				.WithTitle("Add new content successful")
				.WithFooter(footer)
				.WithCurrentTimestamp();

			var embed = builder.Build();
			await ReplyAsync(null, false, embed);
		}

		[Command("list"), Alias("l")]
		[Summary("List all gif")]
		public Task ListTask()
		{
			return ListTaskAsync();
		}

		public async Task ListTaskAsync()
		{
			var rawData = DataHandler.GetRawValuesByGuild(Context.Guild.Id);
			if (rawData == null)
			{
				await ReplyAsync("There is no any context in this guild");
				return;
			}
			var allRawData = rawData.ToArray();
			var totalPage = allRawData.Length / embedMaxFieldCount + 1;
			for (int i = 0; i < totalPage; i++)
			{
				var startIndex = i * 25;
				var endIndex = startIndex + 25;
				if (endIndex >= allRawData.Length)
				{
					endIndex = allRawData.Length;
				}
				var data = allRawData[startIndex..endIndex];
				Embed embed = CreateEmbed(data, $"Page : {i + 1}/{totalPage}");
				await ReplyAsync(null, false, embed);
			}
		}

		private Embed CreateEmbed(SheetRawValue[] allRawData, string titleAppend = null)
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
	}
}
