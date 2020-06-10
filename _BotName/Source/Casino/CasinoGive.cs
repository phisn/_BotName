﻿using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _BotName.Source.Casino
{
	[Group("casino")]
	public class CasinoGive : ModuleBase<SocketCommandContext>
	{
		[Command("give")]
		public Task GiveAsync(int? amount, IUser user = null)
		{
			if (amount == null || user == null)
				return ReplyAsync("Usage: give <amount> <user>");

			if (amount.Value <= 0)
				return ReplyAsync("Amount has to be over 0");

			CasinoUser casinoUserGiver = CasinoController.Instance.GetUser(Context.User.Id);

			if (casinoUserGiver.Money < amount.Value)
				return ReplyAsync("Not enough money");

			CasinoUser casinoUserGetter = CasinoController.Instance.GetUser(user.Id);

			casinoUserGiver.Money -= amount.Value;
			casinoUserGetter.Money += amount.Value;

			CasinoController.Instance.Save();

			return ReplyAsync($"{Context.User.Username}#{Context.User.Discriminator} gave {user.Username}#{user.Discriminator} {amount} money");
		}

		[Command("cheat")]
		[RequireOwner]
		public Task CheatAsync(int? amount = null, IUser user = null)
		{
			if (amount == null)
				return ReplyAsync("Usage: cheat <amount> [user]");

			user = user ?? Context.User;
			CasinoUser casinoUser = CasinoController.Instance.GetUser(user.Id);

			casinoUser.Money += amount.Value;
			CasinoController.Instance.Save();

			return ReplyAsync($"{amount} money appeared in {user.Username}#{user.Discriminator} pocket");
		}
	}
}
