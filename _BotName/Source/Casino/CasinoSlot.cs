using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _BotName.Source.Casino
{
	enum SlotSymbol
	{
		Bar,
		Seven,
		Cherry,
		Banana,
		Lemon,
		Orange
	}

	enum SlotWin
	{
		Lose,
		Bar,
		Seven,
		TripleCherry,
		DoubleCherry,
		SingleCherry,
		TreeFruits
	}

	[Group("casino")]
	public class CasinoSlot : ModuleBase<SocketCommandContext>
	{
		private static string slot_usage = "Usage: slot <amount>";

		private static int reelLength = 23;
		private static SlotSymbol[] reel1 =
		{
			SlotSymbol.Bar,
			SlotSymbol.Seven,
			SlotSymbol.Seven,
			SlotSymbol.Seven,
			SlotSymbol.Cherry,
			SlotSymbol.Cherry,
			SlotSymbol.Cherry,
			SlotSymbol.Cherry,
			SlotSymbol.Banana,
			SlotSymbol.Banana,
			SlotSymbol.Banana,
			SlotSymbol.Banana,
			SlotSymbol.Banana,
			SlotSymbol.Lemon,
			SlotSymbol.Lemon,
			SlotSymbol.Lemon,
			SlotSymbol.Lemon,
			SlotSymbol.Lemon,
			SlotSymbol.Orange,
			SlotSymbol.Orange,
			SlotSymbol.Orange,
			SlotSymbol.Orange,
			SlotSymbol.Orange,
		};

		private static SlotSymbol[] reel2 =
		{
			SlotSymbol.Bar,
			SlotSymbol.Seven,
			SlotSymbol.Cherry,
			SlotSymbol.Cherry,
			SlotSymbol.Cherry,
			SlotSymbol.Cherry,
			SlotSymbol.Banana,
			SlotSymbol.Banana,
			SlotSymbol.Banana,
			SlotSymbol.Banana,
			SlotSymbol.Banana,
			SlotSymbol.Banana,
			SlotSymbol.Lemon,
			SlotSymbol.Lemon,
			SlotSymbol.Lemon,
			SlotSymbol.Lemon,
			SlotSymbol.Lemon,
			SlotSymbol.Lemon,
			SlotSymbol.Orange,
			SlotSymbol.Orange,
			SlotSymbol.Orange,
			SlotSymbol.Orange,
			SlotSymbol.Orange,
			SlotSymbol.Orange,
		};

		private static SlotSymbol[] reel3 = reel2;

		private Random random = new Random();

		[Command("slot")]
		public Task QueryAsync(int? amount = null)
		{
			if (amount == null)
				return ReplyAsync(slot_usage);

			if (amount <= 0)
				return ReplyAsync("Amount has to be over 0");

			CasinoUser user = CasinoController.Instance.GetUser(Context.User.Id);

			if (user.Money < amount)
				return ReplyAsync("You don't have enough money");

			StringBuilder builder = new StringBuilder();

			SlotSymbol[] symbols = RandomSlotSymbols();
			SlotWin win = GetSlotWin(symbols);

			builder.AppendLine($"[{SlotSymbolToCharacter(symbols[0])}|{SlotSymbolToCharacter(symbols[1])}|{SlotSymbolToCharacter(symbols[2])}]");
			AppendWinMessage(builder, win);

			if (win == SlotWin.Lose)
			{
				user.Money -= amount.Value;
			}
			else
			{
				int multi = GetSlotWinMulti(win);
				builder.AppendLine($"You have won {multi * amount} money");
				user.Money += (multi - 1) * amount.Value;
			}

			CasinoController.Instance.Save();

			if (win == SlotWin.Bar)
			{
				try
				{
					SocketRole luckyRole = Context.Guild.Roles.First(role => role.Name == "Jackpot");
					IGuildUser guildUser = (IGuildUser) user;

					guildUser.AddRoleAsync(luckyRole).GetAwaiter().GetResult();
				}
				catch (Exception e)
				{
					Console.WriteLine($"Failed to give jackpot role: {e.Message}");
				}
			}

			return ReplyAsync(builder.ToString());
		}

		[Command("slot info")]
		public Task QueryAsync()
		{
			return ReplyAsync(
@"**Casino slot info**
```
Usage:
> slot <amount>
  Use a slot machine and get up to
  200x your money

Multi:
 3 x J => 200x
 3 x 7 =>  50x
 3 x ? =>  20x
 3 x A =>  10x
 3 x B =>  10x
 3 x C =>  10x
 2 x ? =>   3x
 1 x ? =>   1x
```");
		}

		private void AppendWinMessage(StringBuilder builder, SlotWin win)
		{
			switch (win)
			{
				case SlotWin.Bar:
					builder.AppendLine("You got three J's - JACKPOT!!!");
					break;
				case SlotWin.Seven:
					builder.AppendLine("You got three 7's!!");
					break;
				case SlotWin.TripleCherry:
					builder.AppendLine("You got three ?'s!!");
					break;
				case SlotWin.TreeFruits:
					builder.AppendLine("You got three Letters's!");
					break;
				case SlotWin.DoubleCherry:
					builder.AppendLine("You got two ?'s");
					break;
				case SlotWin.SingleCherry:
					builder.AppendLine("You got one ?");
					break;
				default:
					builder.AppendLine("You won nothing");
					break;
			}
		}

		private int GetSlotWinMulti(SlotWin win)
		{
			switch (win)
			{
				case SlotWin.Bar:
					return 200;
				case SlotWin.Seven:
					return 50;
				case SlotWin.TripleCherry:
					return 20;
				case SlotWin.TreeFruits:
					return 10;
				case SlotWin.DoubleCherry:
					return 3;
				case SlotWin.SingleCherry:
					return 1;
				default:
					return 0;
			}
		}

		private char SlotSymbolToCharacter(SlotSymbol symbol)
		{
			switch (symbol)
			{
				case SlotSymbol.Bar:
					return 'J';
				case SlotSymbol.Seven:
					return '7';
				case SlotSymbol.Cherry:
					return '?';
				case SlotSymbol.Banana:
					return 'A';
				case SlotSymbol.Lemon:
					return 'B';
				case SlotSymbol.Orange:
					return 'C';
				default:
					return ' ';
			}
		}

		private SlotWin GetSlotWin(SlotSymbol[] symbols)
		{
			if ((symbols[0] == symbols[1]) && 
				(symbols[0] == symbols[2]))
			{
				switch (symbols[0])
				{
					case SlotSymbol.Bar:
						return SlotWin.Bar;
					case SlotSymbol.Seven:
						return SlotWin.Seven;
					case SlotSymbol.Cherry:
						return SlotWin.TripleCherry;
					default:
						return SlotWin.TreeFruits;
				}
			}
			else
			{
				int cherryAmount = 0;
				for (int i = 0; i < 3; ++i)
					if (symbols[i] == SlotSymbol.Cherry)
					{
						++cherryAmount;
					}

				switch (cherryAmount)
				{
					case 1:
						return SlotWin.SingleCherry;
					case 2:
						return SlotWin.DoubleCherry;
					default:
						return SlotWin.Lose;
				}
			}
		}

		private SlotSymbol[] RandomSlotSymbols()
		{
			return new SlotSymbol[]
			{
				RandomSymbolFromReel(reel1),
				RandomSymbolFromReel(reel2),
				RandomSymbolFromReel(reel3)
			};
		}

		private SlotSymbol RandomSymbolFromReel(SlotSymbol[] reel)
		{
			return reel[random.Next(0, reelLength)];
		}
	}
}
