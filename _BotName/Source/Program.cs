using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using _BotName.Source.Database;
using LinqToDB.Data;

namespace _BotName.Source
{
	public class Program
	{
		private static string DefaultTokenFileName = "token.txt";
		
		public static void Main(string[] args)
		{
			DataConnection.DefaultSettings = new DatabaseSettings();
			
			var token = AquireToken(args.Length == 2 ? args[2] : DefaultTokenFileName);

			Console.WriteLine("Starting bot with '" + token + "'");
			new Program().MainAsync(token).GetAwaiter().GetResult();
		}

		private static string AquireToken(string path)
		{
			return File.ReadAllText(path);
		}

		private CommandHandler _commandHandler;
		private static CommandService _commandService;
		private DiscordSocketClient _client;

		public static CommandService GetCommandService()
		{
			return _commandService;
		}

		public async Task MainAsync(string token)
		{
			_client = new DiscordSocketClient(CreateDiscordSocketConfig());
			_commandService = new CommandService(CreateCommandServiceConfig());

			_client.Log += Log;

			await _client.LoginAsync(TokenType.Bot, token);
			await _client.StartAsync();

			_commandHandler = new CommandHandler(_client, _commandService);

			await _commandHandler.InstallCommandsAsync();

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
