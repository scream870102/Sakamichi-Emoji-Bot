using Discord;
using Discord.Commands;
using Seb.Misc;

namespace Seb.Modules
{
	[Name("Interaction")]
	public class InteractionModule : ModuleBase<SocketCommandContext>
	{
		[Command("Show"), Alias("s")]
		[Summary("Show content")]
		public Task ShowTask([Remainder] string tags)
		{
			var tag = TagParser.ParseTag(tags);
			return ShowTaskAsync(tag);
		}

		public async Task ShowTaskAsync(string[] tags)
		{
			var value = DataHandler.GetRawValue(Context.Guild.Id, tags);
			if (value != null)
			{
				await Context.Message.AddReactionAsync(new Emoji("🍑"));
				await ReplyAsync(value.Url);
				return;
			}
			await ReplyAsync("These tags are invalid");
		}

		[Command("Help"), Alias("h")]
		[Summary("Show help info")]
		public Task HelpTask()
		{
			return HelpTaskAsync();
		}

		public async Task HelpTaskAsync()
		{
			string path = @"help.txt";
			var readText = File.ReadAllLines(path);


			var builder = EmbedCreator.CreateBasicEmbedBuilder("Manual");
			builder.WithDescription($"Hey {MentionUtils.MentionUser(Context.User.Id)}!!!\nHere is the manual for this BOT");
			builder.WithImageUrl(@"https://i.imgur.com/Mw51xtx.jpg");

			var title = "";
			var content = "";
			var isTitleFind = false;
			for (int i = 0; i < readText.Length; i++)
			{
				if (!readText[i].StartsWith('~'))
				{
					if (!isTitleFind)
					{
						title = readText[i];
						isTitleFind = true;
					}
					else
					{
						builder.AddField(title, content);
						title = readText[i];
						content = "";
					}
				}
				else
				{
					content += $"{readText[i]}\n";
				}
			}
			var embed = builder.Build();
			await ReplyAsync(null, false, embed);
		}
	}
}
