using Discord.Commands;
using System;
using System.Text;
using System.Threading.Tasks;
using _BotName.Source.Casino;

namespace _BotName.Source.Discord.Commands
{
	enum CoinSide
	{
		Head,
		Tail
	}

	[Group("casino")]
	public class CasinoCoinFlip : ModuleBase<SocketCommandContext>
	{
		private static string coin_usage = "Usage: coin <head/tail> <amount>";
		private static string dice_usage = "Usage: dice <1 - 6> <amount>";

		[Command("coin all")]
		[Alias("flip all", "coin max", "flip max")]
		public Task CoinAllAsync(string mode = null)
		{
			CasinoUser user = CasinoController.Instance.GetCasinoUserRepository().FindOrCreateById(Context.User.Id);

			if (user.Money == 0)
				return ReplyAsync("You don't have any ₩");

			return CoinAsync(mode, user.Money);
		}

		[Command("coin")]
		[Alias("flip")]
		public Task CoinAsync(string mode = null, int? amount = null)
		{
			if (mode == null || amount == null)
				return ReplyAsync(coin_usage);

			if (amount <= 0)
				return ReplyAsync("Amount has to be over 0");

			CoinSide? coinSide = GetCoinSide(mode);

			if (coinSide == null)
				return ReplyAsync("First argument has to be either 'head' or 'tail'");

			CasinoUser user = CasinoController.Instance.GetCasinoUserRepository().FindOrCreateById(Context.User.Id);

			if (user.Money < amount)
				return ReplyAsync("You don't have enough money");

			StringBuilder builder = new StringBuilder();

			CoinSide flippedSide = FlipCoin();
			builder.AppendLine($"Flipped '{flippedSide.ToString()}'");
			
			if (flippedSide == coinSide)
			{
				builder.AppendLine($"You earned {amount * 2} ₩");
				user.Money += amount.Value;
			}
			else
			{
				builder.AppendLine($"You lost {amount} ₩");
				user.Money -= amount.Value;
			}

			CasinoController.Instance.Save();

			return ReplyAsync(builder.ToString());
		}

		[Command("dice")]
		[Alias("throw")]
		public Task QueryAsync(int number = 0, int? amount = null)
		{
			if (number <= 0 || number > 6 || amount == null)
				return ReplyAsync(dice_usage);

			if (amount <= 0)
				return ReplyAsync("Amount has to be over 0");

			CasinoUser user = CasinoController.Instance.GetCasinoUserRepository().FindOrCreateById(Context.User.Id);

			if (user.Money < amount)
				return ReplyAsync("You don't have enough money");

			StringBuilder builder = new StringBuilder();

			int thrownNumber = new Random().Next(1, 7);
			builder.AppendLine($"Thrown '{thrownNumber}'");

			if (thrownNumber == number)
			{
				builder.AppendLine($"You earned {amount * 6} ₩");
				user.Money += amount.Value * 5;
			}
			else
			{
				builder.AppendLine($"You lost {amount} ₩");
				user.Money -= amount.Value;
			}

			CasinoController.Instance.Save();

			return ReplyAsync(builder.ToString());
		}

		private CoinSide FlipCoin()
		{
			return new Random().Next(0, 2) == 0
				? CoinSide.Head
				: CoinSide.Tail;
		}

		private CoinSide? GetCoinSide(string mode)
		{
			switch (mode.ToLower())
			{
				case "head":
					return CoinSide.Head;
				case "tail":
					return CoinSide.Tail;
			};

			return null;
		}
	}
}
