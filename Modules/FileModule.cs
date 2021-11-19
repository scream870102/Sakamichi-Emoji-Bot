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
		[Command("add"), Alias("a")]
		[Summary("Add the gif")]
		public Task AddTask([Remainder] string tags)
		{
			var tag = TagParser.ParseTag(tags);
			return AddTaskAsync(tag);
		}

		public async Task AddTaskAsync(string[] tags)
		{
			var guild = Context.Guild.Id;
			var url = tags[^1];
			var tag = tags[..^1];
			//TODO: If content exist
			// var contentCheck = DataHandler.GetRawValuesByContent(url, guild);
			// if (contentCheck.Count > 0)
			// {

			// }
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
			var builder = EmbedCreator.CreateBasicEmbedBuilder("Add new content successful");
			builder
				.WithThumbnailUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
				.WithDescription($"Congratulation {MentionUtils.MentionUser(Context.User.Id)}!!!\nYou are the best momoko oshi")
				.AddField("Id", id)
				.AddField("Tag", allTags.ToString())
				.AddField("Url", url);
			var embed = builder.Build();
			await ReplyAsync(null, false, embed);
		}

		[Command("update"), Alias("u")]
		[Summary("Update the cache from db")]
		public Task UpdateCacheTask()
		{
			return UpdateCacheTaskAsync();
		}

		public async Task UpdateCacheTaskAsync()
		{
			await SpreadSheet.UpdateAllValueFromRemoteAsync();
			DataHandler.Init();
			await ReplyAsync("Update local cache from db successful");
		}
	}
}
