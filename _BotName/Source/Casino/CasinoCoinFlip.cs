using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _BotName.Source.Casino
{
	enum CoinSide
	{
		Head,
		Tail
	}

	public class CasinoCoinFlip : ModuleBase<SocketCommandContext>
	{
		private static string usage = "Usage: coin <head/tail> <amount>";

		[Command("casino coin")]
		[Alias("casino flip")]
		public Task QueryAsync(string mode = null, int? amount = null)
		{
			if (mode == null || amount == null)
			{
				return ReplyAsync(usage);
			}

			if (amount <= 0)
			{
				return ReplyAsync("Amount has to be over 0");
			}

			CoinSide? coinSide = GetCoinSide(mode);

			if (coinSide == null)
			{
				return ReplyAsync("First argument has to be either 'head' or 'tail'");
			}

			CasinoUser user = CasinoController.Instance.GetUser(Context.User.Id);

			if (user.Money < amount)
			{
				return ReplyAsync("You don't have enough money");
			}

			StringBuilder builder = new StringBuilder();

			CoinSide flippedSide = FlipCoin();
			builder.AppendLine($"Flipped '{flippedSide.ToString()}'");
			
			if (flippedSide == coinSide)
			{
				builder.AppendLine($"You earned {amount * 2}");
				user.Money += amount.Value;
			}
			else
			{
				builder.AppendLine($"You lost {amount}");
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
