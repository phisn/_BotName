using Discord;
using Discord.Commands;
using System.Text;
using System.Threading.Tasks;

namespace _BotName.Source.Casino
{
	public class CasinoInfo : ModuleBase<SocketCommandContext>
	{
		[Command("casino")]
		[Alias("casino help")]
		public Task InfoAsync()
		{
			return ReplyAsync(
$@"**Casino Commands**:
All commands need the {CommandHandler.Prefix}casino prefix
₩ for _waehrungname
```
> info [user]
  Get information about a user or yourself.
> give <user> <amount>
  Give someone a amount of money
> claim
  Claim your hourly money between 1 and 100
> coin <head/tail> <amount>
  Flip a coin and maybe double your money
> dice <1 - 6> <amount>
  Throw a dice and sextuple your money
> slot <amount>
  Use a slot machine and get up to 200x
  your money.
>> slot info
   For more information about slot wins
>> slot quick <count (< 100)> <amount>
   Play slot machine count times quickly
> challange <user> <amount>
  Challange someone and maybe win his money
>> challange info [user]
   Get the info about all your challanges
   Or info about your challanges to someone
   Or info about challanges from someone to you
>> challange accept <user>
   Accept a challange from someone
>> challange decline [user]
   Decline a challange from someone
   Or decline all your challanges
> buy <item>
  Buy a item for casino money
>> buy lucky
   Buy the lucky role for 10000 ₩
```");
		}

		[Command("casino info")]
		[Alias("casinoinfo")]
		public Task QueryAsync(IUser user = null)
		{
			user = user ?? Context.User;
			CasinoUser casinoUser = CasinoController.Instance.GetUser(user.Id);

			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine($"User: {user.Username}#{user.Discriminator}");
			stringBuilder.AppendLine($"Money: {casinoUser.Money} ₩");

			return ReplyAsync(stringBuilder.ToString());
		}
	}
}
