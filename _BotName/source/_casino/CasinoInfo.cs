using _BotName.source._casino;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _BotName.source._casino
{
	public class CasinoInfo : ModuleBase<SocketCommandContext>
	{
		[Command("casino")]
		public Task InfoAsync()
		{
			return ReplyAsync(
				"Casino Commands:" +
				" - info       Get Account Information");
		}

		[Command("casino info")]
		public Task QueryAsync(IUser user = null)
		{
			user = user ?? Context.User;
			CasinoUser casinoUser = CasinoController.Instance.GetUser(user.Id);

			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine($"User: {user.Username}#{user.Discriminator}");
			stringBuilder.AppendLine($"Money: {casinoUser.Money}");

			return ReplyAsync(stringBuilder.ToString());
		}
	}
}
