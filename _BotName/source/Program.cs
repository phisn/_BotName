using _BotName.source;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _BotName
{
	public class Program
	{
		private static string DefaultTokenFileName = "token.txt";
		
		public static void Main(string[] args)
		{
			string token = AquireToken(
				args.Length == 2 
				? args[2] 
				: DefaultTokenFileName);

			Console.WriteLine("Starting bot with '" + token + "'");
			new Program().MainAsync(token).GetAwaiter().GetResult();
		}

		private static string AquireToken(string path)
		{
			return File.ReadAllText(path);
		}

		private CommandHandler commandHandler;
		private CommandService commandService;
		private DiscordSocketClient client;

		public async Task MainAsync(string token)
		{
			client = new DiscordSocketClient(CreateDiscordSocketConfig());
			commandService = new CommandService(CreateCommandServiceConfig());

			client.Log += Log;

			await client.LoginAsync(TokenType.Bot, token);
			await client.StartAsync();

			commandHandler = new CommandHandler(
				client,
				commandService);

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		private DiscordSocketConfig CreateDiscordSocketConfig()
		{
			return new DiscordSocketConfig
			{
				LogLevel = LogSeverity.Info,
			};
		}

		private CommandServiceConfig CreateCommandServiceConfig()
		{
			return new CommandServiceConfig
			{
				LogLevel = LogSeverity.Info,
				CaseSensitiveCommands = false,
			};

		}

		private Task Log(LogMessage message)
		{
			switch (message.Severity)
			{
				case LogSeverity.Critical:
				case LogSeverity.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				case LogSeverity.Warning:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case LogSeverity.Info:
					Console.ForegroundColor = ConsoleColor.White;
					break;
				case LogSeverity.Verbose:
				case LogSeverity.Debug:
					Console.ForegroundColor = ConsoleColor.DarkGray;
					break;
			}
			Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
			Console.ResetColor();

			return Task.CompletedTask;
		}
	}
}
