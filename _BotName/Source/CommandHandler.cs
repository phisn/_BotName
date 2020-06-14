﻿using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Threading.Tasks;

namespace _BotName.Source
{
    public class CommandHandler
    {
        public static char Prefix = '$';

        private readonly DiscordSocketClient client;
        private readonly CommandService commands;

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            this.commands = commands;
            this.client = client;
        }

        public async Task InstallCommandsAsync()
        {
            client.MessageReceived += HandleCommandAsync;
            await commands.AddModulesAsync(
                Assembly.GetEntryAssembly(),
                null);
        }
		
        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            SocketUserMessage message = (SocketUserMessage)messageParam;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasCharPrefix(Prefix, ref argPos) ||
                message.HasMentionPrefix(client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            await commands.ExecuteAsync(
                new SocketCommandContext(client, message),
                argPos,
                null);
        }
    }
}
