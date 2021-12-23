using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LinearsBot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Linears
{
	public class Useful : ModuleBase<SocketCommandContext>
	{
		[Command("persistent")]
		[RequireOwner()]
		public async Task PersistentMessage([Remainder] string text)
		{
			Match match = Regex.Match(text, "(^[1-5][0-9]m|^60m)");
			if (!match.Success)
			{
				var errorEmbed = new EmbedBuilder
				{
					Color = Color.Red,
					Title = "Erreur",
					Description = $"Usage de la commande : \n`persistent` [temps (10m-60m)] [Message...]",
					Timestamp = DateTime.Now,
					Footer = new EmbedFooterBuilder()
					{
						IconUrl = Functions.GetAvatarUrl(Context.User, 32),
						Text = Context.User.Username + "#" + Context.User.Discriminator
					}
				};
				await Context.Channel.SendMessageAsync("", false, errorEmbed.Build());
				return;
			}

			var embed = new EmbedBuilder
			{
				Color = Color.Green,
				Title = "Message percistant",
				Description = Regex.Replace(text, "(^[1-5][0-9]m|^60m)", ""),
				Footer = new EmbedFooterBuilder()
				{
					IconUrl = Functions.GetAvatarUrl(Context.User, 32),
					Text = Context.User.Username + "#" + Context.User.Discriminator
				}
			};

			if (PersistentMessages.persistentMessages == null)
			{
				PersistentMessages.persistentMessages = new Dictionary<ulong, PersistentMessages.StructPersistentMessages>
				{
					{
						Context.Channel.SendMessageAsync("", false, embed.Build()).Result.Channel.Id,
						new PersistentMessages.StructPersistentMessages
						{
							dateTime = new DateTime(),
							embed = embed,
							lastMessage = Context.Channel.SendMessageAsync("", false, embed.Build()).Result.Id
						}
					}
				};
			}
			else
			{
				PersistentMessages.persistentMessages.Add(Context.Channel.SendMessageAsync("", false, embed.Build()).Result.Channel.Id, new PersistentMessages.StructPersistentMessages
				{
					dateTime = new DateTime().AddMinutes(Convert.ToDouble(match.Value.Remove(match.Value.Length))),
					embed = embed,
					lastMessage = Context.Channel.SendMessageAsync("", false, embed.Build()).Result.Id
				});
			}


			/*
			 922516571165974548, new PersistentMessages.StructPersistentMessages
			{
				dateTime = new DateTime(),
				embed = embed,
			} }
			*/
			await Context.Message.DeleteAsync();
		}
	}
}
