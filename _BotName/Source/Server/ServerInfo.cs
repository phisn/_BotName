using Discord;
using Discord.Commands;
using System.Text;
using System.Threading.Tasks;

namespace _BotName.Source.Server
{
    [Group("server")]
    class ServerInfo : ModuleBase<SocketCommandContext>
    {
        [Command("info")]
        public Task ClaimAsync()
        {
            return ReplyAsync("Currently not available");
        }
    }
}
