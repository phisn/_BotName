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
            new Lazy<DiscordUtility>(() => new DiscordUtility(true));

        /**
         * Do not use this constructor directly. Please use .Instance property.
         */
        public DiscordUtility(bool usedFactory = false)
        {
            if (!usedFactory)
            {
                throw new Exception("Please access singleton instance by .Instance property!");
            }
        }
        
        public virtual bool AddRole(string roleName, ulong userId)
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