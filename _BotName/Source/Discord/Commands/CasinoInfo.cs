using System;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using System.Text;
using System.Threading.Tasks;
using _BotName.Source.Casino;

namespace _BotName.Source.Discord.Commands
{
	public class CasinoInfo : ModuleBase<SocketCommandContext>
	{
		[Command("casino")]
		[Alias("casino help")]
		public Task InfoAsync()
		{

			var commandService = Program.GetCommandService();
			var commandText = "";
			foreach (var command in commandService.Commands)
			{
				var parameters = new List<string>();
				foreach (var param in command.Parameters)
				{
					if (param.IsOptional)
					{
						parameters.Add("<" + param.Name + ">");
					}
					else
					{
						parameters.Add("[" + param.Name + "]");
					}
				}

				//	commandText += string.Format("> {0} {1}{3}{2}{3}", command.Name, command.Parameters.ToString(), command.Summary, Environment.NewLine);
				commandText += string.Format("> {0} {1}{3}{2} {3}", command.Name, string.Join(" ", parameters.ToArray()), command.Summary??"???", Environment.NewLine);
			}
			
			return ReplyAsync(
$@"**Casino Commands**:
All commands need the {CommandHandler.Prefix} casino prefix
(₩ = _Waehrungname)
```
{commandText}
```");
		}

		[Command("casino info")]
		[Summary("Get information about a user or yourself")]
		[Alias("casinoinfo")]
		public Task QueryAsync(IUser user = null)
		{
			user = user ?? Context.User;
			CasinoUser casinoUser = CasinoController.Instance.GetCasinoUserRepository().FindOrCreateById(user.Id);

			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine($"User: {user.Username}#{user.Discriminator}");
			stringBuilder.AppendLine($"Money: {casinoUser.Money} ₩");

			return ReplyAsync(stringBuilder.ToString());
		}
	}
}
