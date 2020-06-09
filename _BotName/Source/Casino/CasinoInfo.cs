using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _BotName.Source.Casino
{
	public class CasinoInfo : ModuleBase<SocketCommandContext>
	{
		[Command("casino")]
		public Task InfoAsync()
		{
			return ReplyAsync(
@"Casino Commands:
-> info [user]
Get Casino user information
-> give <user> <amount>
Give a user money
-> claim
Claim daily money
-> coin <head/tail> <amount>
Flip a Coin and double your money");
		}

		[Command("casino info")]
		[Alias("casinoinfo")]
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
