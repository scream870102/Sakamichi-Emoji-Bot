using Discord;
using System;
using System.Threading.Tasks;
using System.IO;
using Discord.WebSocket;
using Discord.Commands;

namespace Seb
{
	class Program
	{
		public static Task Main(string[] args)
			=> Startup.RunAsync(args);
	}
}
