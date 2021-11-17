using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Seb.Services
{
	public class CommandHandler
	{
		private readonly DiscordSocketClient discord;
		private readonly CommandService commands;
		private readonly IConfigurationRoot config;
		private readonly IServiceProvider provider;

		public CommandHandler(
			DiscordSocketClient discord,
			CommandService commands,
			IConfigurationRoot config,
			IServiceProvider provider)
		{
			this.discord = discord;
			this.commands = commands;
			this.config = config;
			this.provider = provider;

			this.discord.MessageReceived += OnMessageReceivedAsync;
		}

		private async Task OnMessageReceivedAsync(SocketMessage s)
		{
			// Ensure the message is from a user/bot
			if (s is not SocketUserMessage msg) return;
			if (msg.Author.Id == discord.CurrentUser.Id) return;     // Ignore self when checking commands

			var context = new SocketCommandContext(discord, msg);     // Create the command context

			int argPos = 0;     // Check if the message has a valid command prefix
			if (msg.HasStringPrefix(config["prefix"], ref argPos) || msg.HasMentionPrefix(discord.CurrentUser, ref argPos))
			{
				var result = await commands.ExecuteAsync(context, argPos, provider);     // Execute the command

				// ATTEND: Don't reply this to user
				// if (!result.IsSuccess)     // If not successful, reply with the error.
				// await context.Channel.SendMessageAsync(result.ToString());
			}
		}
	}
}
