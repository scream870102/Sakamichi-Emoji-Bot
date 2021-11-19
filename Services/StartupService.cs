using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Seb.Misc;
using System.Reflection;

namespace Seb.Services
{
	public class StartupService
	{
		private readonly IServiceProvider provider;
		private readonly DiscordSocketClient discord;
		private readonly CommandService commands;
		private readonly IConfigurationRoot config;

		public StartupService(
			IServiceProvider provider,
			DiscordSocketClient discord,
			CommandService commands,
			IConfigurationRoot config)
		{
			this.provider = provider;
			this.config = config;
			this.discord = discord;
			this.commands = commands;
		}

		public async Task StartAsync()
		{
			string discordToken = config["tokens:discord"];
			if (string.IsNullOrWhiteSpace(discordToken))
				throw new Exception("Please enter your bot's token into the `_configuration.json` file found in the applications root directory.");
			await discord.SetActivityAsync(new DefaultActivity());
			await discord.LoginAsync(TokenType.Bot, discordToken);
			await discord.StartAsync();
			await commands.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
		}
	}
}
