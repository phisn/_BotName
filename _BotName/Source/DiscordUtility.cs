using System;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace _BotName.Source
{
    public class DiscordUtility : ModuleBase<SocketCommandContext>
    {
        public static DiscordUtility Instance { get { return lazy.Value; } }
        private static readonly Lazy<DiscordUtility> lazy =
            new Lazy<DiscordUtility>(() => new DiscordUtility());
        
        public bool AddRole(string roleName, ulong userId)
        {
            IGuildUser guildUser = Context.Guild.GetUser(userId);
            try
            {
                SocketRole luckyRole = Context.Guild.Roles.First(role => role.Name == roleName);
                guildUser.AddRoleAsync(luckyRole).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to buy role lucky: {e.Message}");
                return false;
            }
        }
    }
}