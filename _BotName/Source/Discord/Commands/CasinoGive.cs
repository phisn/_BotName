using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using _BotName.Source.Casino;
using _BotName.Source.Casino.Currency;

namespace _BotName.Source.Discord.Commands
{
	[Group("casino")]
	public class CasinoGive : ModuleBase<SocketCommandContext>
	{
		[Command("give")]
		public Task Give2Async(IUser user = null, int? amount = null)
		{
			return GiveAsync(amount, user);
		}

		[Command("give")]
		[Summary("Give someone a amount of money")]
		public Task GiveAsync(int? amount, IUser user = null)
		{
			if (amount == null || user == null)
				return ReplyAsync("Usage: give <amount> <user>");

			var giveResult = new Give().GiveMoney(Context.User.Id, user.Id, amount.Value);

			switch (giveResult.Status)
			{
				case GiveError.Okay:
					return ReplyAsync($"{Context.User.Username}#{Context.User.Discriminator} gave {user.Username}#{user.Discriminator} {amount} ₩");
				case GiveError.AmountLessThanZero:
					return ReplyAsync("Amount has to be over 0");
				case GiveError.NotEnoughMoney:
					return ReplyAsync("Not enough ₩");
				default:
					return ReplyAsync("Unknown error occured. Please contact an server administrator.");
			}
		}

		[Command("reset")]
		[RequireOwner]
		public Task ResetAsync(IUser user = null)
		{
			user = user ?? Context.User;
			CasinoUser casinoUser = CasinoController.Instance.GetCasinoUserRepository().FindOrCreateById(user.Id);

			casinoUser.Money = 0;
			// CasinoController.Instance.Save();

			return ReplyAsync($"{user.Username}#{user.Discriminator}'s ₩ was all taken away");
		}

		[Command("cheat")]
		[RequireOwner]
		public Task CheatAsync(int? amount = null, IUser user = null)
		{
			if (amount == null)
				return ReplyAsync("Usage: cheat <amount> [user]");

			user = user ?? Context.User;
			CasinoUser casinoUser = CasinoController.Instance.GetCasinoUserRepository().FindOrCreateById(user.Id);

			casinoUser.Money += amount.Value;
			// CasinoController.Instance.Save();

			return ReplyAsync($"{amount} ₩ appeared in {user.Username}#{user.Discriminator} pocket");
		}
	}
}
