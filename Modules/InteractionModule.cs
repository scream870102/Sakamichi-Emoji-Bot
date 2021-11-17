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
		public Task ShowTask([Remainder] string data)
		{
			return ShowTaskAsync(data);
		}

		public async Task ShowTaskAsync([Remainder] string tag)
		{
			var splitValue = tag.Split('|');
			var value = DataHandler.GetRawValue(Context.Guild.Id, splitValue);
			if (value != null)
			{
				await ReplyAsync(value.Url);
				return;
			}
			await ReplyAsync("This is not a valid context");
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

			var footer = new EmbedFooterBuilder()
			.WithIconUrl(@"https://i.imgur.com/oC0Yo5s.png")
			.WithText($"Created by scream870102");
			var builder = new EmbedBuilder()
				.WithDescription($"Hey {MentionUtils.MentionUser(Context.User.Id)}!!!\nHere is the manual for this BOT")
				.WithColor(new Color(240, 93, 154))
				.WithTitle("Tutorial")
				.WithImageUrl(@"https://i.imgur.com/Mw51xtx.jpg")
				.WithFooter(footer)
				.WithCurrentTimestamp();

			for (int i = 0; i < readText.Length / 2; i += 2)
			{
				var title = readText[i];
				var content = readText[i + 1];
				builder.AddField(title, content);
			}
			var embed = builder.Build();
			await ReplyAsync(null, false, embed);
		}
	}
}
