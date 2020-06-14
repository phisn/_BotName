using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using _BotName.Source.Casino;

namespace _BotName.Source.Discord.Commands
{
	class Challenge
	{
		public string name;
		public int amount;
	}

	[Group("casino")]
	public class CasinoChallenge : ModuleBase<SocketCommandContext>
	{
		private static Dictionary<ulong, Dictionary<ulong, Challenge>> allChallenges = new Dictionary<ulong, Dictionary<ulong, Challenge>>();

		[Command("challenge info")]
		[Summary("Get the info about all your challenges Or info about your challenges to someone Or info about challenges from someone to you")]
		public Task InfoAsync(IUser user = null)
		{
			Dictionary<ulong, Challenge> challenges = EmplaceChallenges(Context.User.Id);

			if (user == null)
			{
				if (challenges.Count == 0)
					return ReplyAsync("You currently dont have any challenges"); 

				StringBuilder builder = new StringBuilder("Your challenges: ");

				foreach (KeyValuePair<ulong, Challenge> challenge in challenges)
				{
					builder.AppendLine($" -> {challenge.Value.name} for {challenge.Value.amount}");
				}

				return ReplyAsync(builder.ToString());
			}
			else
			{
				Challenge challenge;
				if (!challenges.TryGetValue(user.Id, out challenge))
				{
					challenges = EmplaceChallenges(user.Id);
					if (challenges.TryGetValue(Context.User.Id, out challenge))
						return ReplyAsync($"You challenged {user.Username}#{user.Discriminator} for {challenge.amount}");

					return ReplyAsync($"Got no challenge from or to {user.Username}#{user.Discriminator}");
				}

				return ReplyAsync($"Got challenge from {user.Username}#{user.Discriminator} for {challenge.amount} ₩");
			}
		}

		[Command("challenge accept")]
		[Summary("Accept a challenge from someone")]
		public Task AcceptAsync(IUser user = null)
		{
			if (user == null)
				return ReplyAsync("Usage: challenge accept <user>");

			Dictionary<ulong, Challenge> challenges = EmplaceChallenges(Context.User.Id);
			Challenge challenge;

			if (!challenges.TryGetValue(user.Id, out challenge))
				return ReplyAsync($"Got no challenge from {user.Username}#{user.Discriminator}");

			challenges.Remove(user.Id);

			var casinoUserRepository = CasinoController.Instance.GetCasinoUserRepository();
			
			CasinoUser casinoUserChallenged = casinoUserRepository.FindOrCreateById(Context.User.Id);

			if (casinoUserChallenged.Money < challenge.amount)
				return ReplyAsync("You do no longer have enough ₩");

			CasinoUser casinoUserChallenger = casinoUserRepository.FindOrCreateById(user.Id);

			if (casinoUserChallenger.Money < challenge.amount)
				return ReplyAsync($"{user.Username}#{user.Discriminator} has no longer enough ₩");

			CasinoUser casinoWinner, casinoLoser;
			IUser winner, loser;

			if (new Random().Next(0, 2) == 0)
			{
				casinoWinner = casinoUserChallenger;
				winner = user;

				casinoLoser = casinoUserChallenged;
				loser = Context.User;
			}
			else
			{
				casinoWinner = casinoUserChallenged;
				winner = Context.User;

				casinoLoser = casinoUserChallenger;
				loser = user;
			}

			casinoUserRepository.AddMoney(casinoWinner, challenge.amount);
			casinoUserRepository.SubtractMoney(casinoLoser, challenge.amount);

			return ReplyAsync($"{winner.Username}#{winner.Discriminator} won from {loser.Username}#{loser.Discriminator} {challenge.amount} ₩");
		}

		[Command("challenge decline")]
		[Summary("Decline a challenge from someone or decline all your challenges")]
		public Task DeclineAsync(IUser user = null)
		{
			Dictionary<ulong, Challenge> challenges = EmplaceChallenges(Context.User.Id);

			if (user == null)
			{
				challenges.Clear();
				return ReplyAsync("Declined all challenges");
			}
			else
			{
				if (challenges.Remove(user.Id))
					return ReplyAsync($"Declined challenge from {user.Username}#{user.Discriminator}");
				else
					return ReplyAsync($"No challenge from {user.Username}#{user.Discriminator} found");
			}
		}

		[Command("challenge")]
		[Summary("Challange someone and maybe win his money")]
		public Task ChallengeAsync(IUser user = null, int? amount = null)
		{
			if (amount == null || user == null)
				return ReplyAsync("Usage: challenge <user> <amount>");

			if (amount <= 0)
				return ReplyAsync("Amount has to be over 0");

			Dictionary<ulong, Challenge> challenges = EmplaceChallenges(user.Id);

			if (challenges.ContainsKey(Context.User.Id))
				return ReplyAsync($"You already challenged {user.Username}#{user.Discriminator}");

			CasinoUser casinoUserChallenger = CasinoController.Instance.GetCasinoUserRepository().FindOrCreateById(Context.User.Id);

			if (casinoUserChallenger.Money < amount.Value)
				return ReplyAsync("Not enough ₩");

			CasinoUser casinoUserChallenged = CasinoController.Instance.GetCasinoUserRepository().FindOrCreateById(user.Id);

			if (casinoUserChallenged.Money < amount.Value)
				return ReplyAsync($"{user.Username}#{user.Discriminator} has not enough ₩");

			Challenge challenge = new Challenge();
			challenge.amount = amount.Value;
			challenge.name = $"{user.Username}#{user.Discriminator}";
			challenges.Add(Context.User.Id, challenge);

			return ReplyAsync($"{Context.User.Username}#{Context.User.Discriminator} challenged {user.Username}#{user.Discriminator} for {amount} ₩");
		}

		[Command("attack")]
		public Task AttackAsync(int? amount, IUser user = null)
		{
			if (amount == null || user == null)
				return ReplyAsync("Usage: give <amount> <user<");

			CasinoUser casinoUserGiver = CasinoController.Instance.GetCasinoUserRepository().FindOrCreateById(Context.User.Id);

			if (casinoUserGiver.Money <= amount.Value)
				return ReplyAsync("Not enough ₩");

			CasinoUser casinoUserGetter = CasinoController.Instance.GetCasinoUserRepository().FindOrCreateById(user.Id);

			casinoUserGiver.Money -= amount.Value;
			casinoUserGetter.Money += amount.Value;

			// CasinoController.Instance.Save();

			return ReplyAsync($"{Context.User.Username}#{Context.User.Discriminator} gave {user.Username}#{user.Discriminator} {amount} ₩");
		}

		private Dictionary<ulong, Challenge> EmplaceChallenges(ulong userID)
		{
			Dictionary<ulong, Challenge> result;
			if (!allChallenges.TryGetValue(userID, out result))
			{
				result = new Dictionary<ulong, Challenge>();
				allChallenges.Add(userID, result);
			}

			return result;
		}
	}
}
