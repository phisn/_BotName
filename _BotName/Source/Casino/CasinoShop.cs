using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace _BotName.Source.Casino
{
	[Group("casino")]
	public class CasinoShop : ModuleBase<SocketCommandContext>
	{
		private static int price_lucky = 10000;

		[Command("buy lucky")]
		public Task GiveAsync(IUser user = null)
		{
			user = user ?? Context.User;
			CasinoUser casinoUser = CasinoController.Instance.GetUser(user.Id);

			if (casinoUser.Money <= price_lucky)
				return ReplyAsync($"Not enough money, lucky costs {price_lucky} ₩");

			try
			{
				SocketRole luckyRole = Context.Guild.Roles.First(role => role.Name == "Lucky");
				IGuildUser guildUser = (IGuildUser)user;

				guildUser.AddRoleAsync(luckyRole).GetAwaiter().GetResult();

				casinoUser.Money -= price_lucky;
				CasinoController.Instance.Save();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Failed to buy role lucky: {e.Message}");
				return ReplyAsync("Failed to buy role lucky");
			}

			return ReplyAsync($"You bought lucky for {price_lucky} ₩");
		}
	}
}
