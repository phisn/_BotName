using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _BotName.source.modules
{
	public class EchoModule : ModuleBase<SocketCommandContext>
	{
		[Command("say")]
		[Summary("Echoes a message.")]
		public Task SayAsync([Remainder] [Summary("The text to echo")] string echo)
			=> ReplyAsync(echo);
	}
}
