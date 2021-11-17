using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Seb.Services
{
	public class LoggingService
	{
		private readonly DiscordSocketClient discord;
		private readonly CommandService commands;

		private readonly string logDirectory;

		private string GetlogFilePath()
		{
			return Path.Combine(logDirectory, $"{DateTime.UtcNow:yyyy-MM-dd}.txt");
		}

		public LoggingService(DiscordSocketClient discord, CommandService commands)
		{
			logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");

			this.discord = discord;
			this.commands = commands;

			this.discord.Log += OnLogAsync;
			this.commands.Log += OnLogAsync;
		}

		private Task OnLogAsync(LogMessage msg)
		{
			if (!Directory.Exists(logDirectory))     
				Directory.CreateDirectory(logDirectory);
			if (!File.Exists(GetlogFilePath()))    
				File.Create(GetlogFilePath()).Dispose();

			string logText = $"{DateTime.UtcNow:hh:mm:ss} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
			File.AppendAllText(GetlogFilePath(), logText + "\n");     

			return Console.Out.WriteLineAsync(logText);       
		}
	}
}
