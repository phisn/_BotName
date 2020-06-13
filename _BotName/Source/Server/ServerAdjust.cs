using Discord;
using Discord.Commands;
using System.Diagnostics;
using System.Threading.Tasks;

namespace _BotName.Source.Server
{
    [Group("server")]
    class ServerAdjust : ModuleBase<SocketCommandContext>
    {
        [Command("shutdown")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public Task ShutdownAsync()
        {
            Process.Start("shutdown", "/s /t 0");
            return ReplyAsync("System shutdown");
        }

        [Command("restart")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public Task RebootAsync()
        {
            Process.Start("shutdown", "/r /t 0");
            return ReplyAsync("System shutdown");
        }
    }
}
