using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using _BotName.Source.Casino.Shop;

namespace _BotName.Source.Discord.Commands
{
	[Group("casino")]
	public class CasinoShop : ModuleBase<SocketCommandContext>
	{
		[Command("buy lucky")]
		public Task BuyRoleLuckyAsync(IUser user = null)
		{
			return BuyRole("Lucky", user);
		}

		protected Task BuyRole(string roleName, IUser user = null)
		{
			
			user = user ?? Context.User;

			ShopOrder order = new ShopOrder(user.Id, roleName);
			ShopError orderResult = order.perform();
			if (orderResult != ShopError.Okay)
			{
				switch (orderResult)
				{
					case ShopError.NotEnoughMoney:
						return ReplyAsync($"Not enough money. The role costs {order.GetPrice()} ₩");
					case ShopError.UnknownItem:
						return ReplyAsync("Unknown item.");
					case ShopError.RoleAwardError:
						return ReplyAsync($"Failed to buy the role \"{roleName}\". Please try again later.");
					default:
						return ReplyAsync("Unknown error occured. Please contact the server administrator.");
				}
			}

			return ReplyAsync($"You bought the role \"{roleName}\" for {order.GetPrice()} ₩");
		}
	}
}
