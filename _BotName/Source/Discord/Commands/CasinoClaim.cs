using Discord.Commands;
using System.Threading.Tasks;
using _BotName.Source.Casino.Currency;

namespace _BotName.Source.Discord.Commands
{
	[Group("casino")]
	public class CasinoClaim : ModuleBase<SocketCommandContext>
	{
		static Claim claim = new Claim();
		
		[Command("claim")]
		[Summary("Claim your hourly money between 1 and 100")]
		public Task ClaimAsync()
		{
			var claimResult = claim.ClaimMoney(Context.User.Id);

			switch (claimResult.Status)
			{
				case ClaimError.Okay:
					return ReplyAsync($"You claimed {claimResult.ClaimedAmount} ₩");
				case ClaimError.Cooldown:
					return ReplyAsync($"You already claimed your ₩, next claim available in {claimResult.NextClaimMinutes} minutes");
				default:
					return ReplyAsync("Unknown error while claiming. Please contact a server administrator.");
			}
		}
		
		[Command("claim reset")]
		[RequireOwner]
		public Task ClaimResetAsync()
		{
			claim.ResetAllClaims();
			return ReplyAsync("Resetted all claims");
		}

		[Command("claim force")]
		[RequireOwner]
		public Task ClaimForceAsync()
		{
			ClaimResult claimResult = claim.ClaimMoney(Context.User.Id, true);

			switch (claimResult.Status)
			{
				case ClaimError.Okay:
					return ReplyAsync($"You instant claimed {claimResult.ClaimedAmount} ₩");
				default:
					return ReplyAsync("Unknown error while claiming. Please contact a server administrator.");
			}
		}
	}
}
