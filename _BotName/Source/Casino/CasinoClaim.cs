using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace _BotName.Source.Casino
{
	[Group("casino")]
	public class CasinoClaim : ModuleBase<SocketCommandContext>
	{
		private static Dictionary<ulong, DateTime> claims = new Dictionary<ulong, DateTime>();
		private static int minClaim = 1;
		private static int maxClaim = 100;

		[Command("claim")]
		public Task ClaimAsync()
		{
			DateTime lastClaim;
			if (claims.TryGetValue(Context.User.Id, out lastClaim))
			{
				DateTime nextClaim = lastClaim.AddHours(1);
				if (DateTime.Now < nextClaim)
					return ReplyAsync($"You already claimed your ₩, next claim available in {(int)Math.Round((nextClaim - DateTime.Now).TotalMinutes)} minutes");

				claims[Context.User.Id] = DateTime.Now;
			}
			else
			{
				claims.Add(Context.User.Id, DateTime.Now);
			}

			int claimedMoney = new Random().Next(minClaim, maxClaim + 1);

			CasinoController.Instance.GetUser(Context.User.Id).Money += claimedMoney;
			CasinoController.Instance.Save();

			return ReplyAsync($"You claimed {claimedMoney} ₩");
		}


		[Command("claim reset")]
		[RequireOwner]
		public Task ClaimResetAsync()
		{
			claims.Clear();
			return ReplyAsync("Reseted all claims");
		}

		[Command("claim force")]
		[RequireOwner]
		public Task ClaimForceAsync()
		{
			if (claims.ContainsKey(Context.User.Id))
			{
				claims[Context.User.Id] = DateTime.Now;
			}
			else
			{
				claims.Add(Context.User.Id, DateTime.Now);
			}

			int claimedMoney = new Random().Next(minClaim, maxClaim + 1);

			CasinoController.Instance.GetUser(Context.User.Id).Money += claimedMoney;
			CasinoController.Instance.Save();

			return ReplyAsync($"You instant claimed {claimedMoney} ₩");
		}
	}
}
