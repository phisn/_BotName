using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _BotName.Source.Casino
{
	class Challange
	{
		public string name;
		public int amount;
	}

	[Group("casino")]
	public class CasinoChallange : ModuleBase<SocketCommandContext>
	{
		private Dictionary<ulong, Dictionary<ulong, Challange>> allChallanges = new Dictionary<ulong, Dictionary<ulong, Challange>>();

		[Command("challange info")]
		public Task InfoAsync(IUser user = null)
		{
			Dictionary<ulong, Challange> challanges = EmplaceChallanges(Context.User.Id);

			if (user == null)
			{
				StringBuilder builder = new StringBuilder("Your challanges: ");
				foreach (KeyValuePair<ulong, Challange> challange in challanges)
				{
					builder.AppendLine($" -> {challange.Value.name} for {challange.Value.amount}");
				}

				return ReplyAsync(builder.ToString());
			}
			else
			{
				Challange challange;
				if (!challanges.TryGetValue(user.Id, out challange))
					return ReplyAsync($"Got no challange from {user.Username}#{user.Discriminator}");

				return ReplyAsync($"Got challange from {user.Username}#{user.Discriminator} for {challange.amount} money");
			}
		}

		[Command("challange accept")]
		public Task AcceptAsync(IUser user = null)
		{
			if (user == null)
				return ReplyAsync("Usage: challange accept <user>");

			Dictionary<ulong, Challange> challanges = EmplaceChallanges(Context.User.Id);
			Challange challange;

			if (!challanges.TryGetValue(user.Id, out challange))
				return ReplyAsync($"Got no challange from {user.Username}#{user.Discriminator}");

			CasinoUser casinoUserChallanger = CasinoController.Instance.GetUser(Context.User.Id);

			if (casinoUserChallanger.Money <= challange.amount)
				return ReplyAsync("You do no longer have enough money");

			CasinoUser casinoUserChallanged = CasinoController.Instance.GetUser(user.Id);

			if (casinoUserChallanged.Money <= challange.amount)
				return ReplyAsync($"{user.Username}#{user.Discriminator} has no longer enough money");

			CasinoUser casinoWinner, casinoLoser;
			IUser winner, loser;

			if (new Random().Next(0, 2) == 0)
			{
				casinoWinner = casinoUserChallanger;
				winner = user;

				casinoLoser = casinoUserChallanged;
				loser = Context.User;
			}
			else
			{
				casinoWinner = casinoUserChallanged;
				winner = Context.User;

				casinoLoser = casinoUserChallanger;
				loser = user;
			}

			casinoWinner.Money += challange.amount;
			casinoLoser.Money -= challange.amount;

			CasinoController.Instance.Save();

			return ReplyAsync($"{winner.Username}#{winner.Discriminator} won from {loser.Username}#{loser.Discriminator} {challange.amount} money");
		}

		[Command("challange decline")]
		public Task DeclineAsync(IUser user = null)
		{
			Dictionary<ulong, Challange> challanges = EmplaceChallanges(Context.User.Id);

			if (user == null)
			{
				challanges.Clear();
				return ReplyAsync("Declined all challanges");
			}
			else
			{
				if (challanges.Remove(user.Id))
					return ReplyAsync($"Decline challange from {user.Username}#{user.Discriminator}");
				else
					return ReplyAsync($"No challange from {user.Username}#{user.Discriminator} found");
			}
		}

		[Command("challange")]
		public Task ChallangeAsync(IUser user = null, int? amount = null)
		{
			if (amount == null || user == null)
				return ReplyAsync("Usage: challange <user> <amount>");

			if (amount <= 0)
				return ReplyAsync("Amount has to be over 0");

			Dictionary<ulong, Challange> challanges = EmplaceChallanges(user.Id);

			if (challanges.ContainsKey(Context.User.Id))
				return ReplyAsync($"You already challanged {user.Username}#{user.Discriminator}");

			CasinoUser casinoUserChallanger = CasinoController.Instance.GetUser(Context.User.Id);

			if (casinoUserChallanger.Money <= amount.Value)
				return ReplyAsync("Not enough money");

			CasinoUser casinoUserChallanged = CasinoController.Instance.GetUser(user.Id);

			if (casinoUserChallanged.Money <= amount.Value)
				return ReplyAsync($"{user.Username}#{user.Discriminator} has not enough money");

			Challange challange = new Challange();
			challange.amount = amount.Value;
			challange.name = $"{user.Username}#{user.Discriminator}";
			challanges.Add(Context.User.Id, challange);

			return ReplyAsync($"{Context.User.Username}#{Context.User.Discriminator} challanged {user.Username}#{user.Discriminator} for {amount} money\n" +
				$"accept with 'challange accept <username>' or" +
				$"decline with 'challange decline <username>");
		}

		[Command("attack")]
		public Task AttackAsync(int? amount, IUser user = null)
		{
			if (amount == null || user == null)
				return ReplyAsync("Usage: give <amount> <user<");

			CasinoUser casinoUserGiver = CasinoController.Instance.GetUser(Context.User.Id);

			if (casinoUserGiver.Money <= amount.Value)
				return ReplyAsync("Not enough money");

			CasinoUser casinoUserGetter = CasinoController.Instance.GetUser(user.Id);

			casinoUserGiver.Money -= amount.Value;
			casinoUserGetter.Money += amount.Value;

			CasinoController.Instance.Save();

			return ReplyAsync($"{Context.User.Username}#{Context.User.Discriminator} gave {user.Username}#{user.Discriminator} {amount} money");
		}

		private Dictionary<ulong, Challange> EmplaceChallanges(ulong userID)
		{
			Dictionary<ulong, Challange> result;
			if (!allChallanges.TryGetValue(userID, out result))
			{
				result = new Dictionary<ulong, Challange>();
				allChallanges.Add(userID, result);
			}

			return result;
		}
	}
}
