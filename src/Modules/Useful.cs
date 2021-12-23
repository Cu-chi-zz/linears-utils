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
		public async Task PersistentMessage([Remainder] string text = "")
		{
			if (text == "")
			{
				var errorEmbed = new EmbedBuilder
				{
					Color = Color.Red,
					Title = "Erreur",
					Description = $"Usage de la commande : \n`persistent` [message...]",
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
				Title = "Message persistant :",
				Description = text,
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
					embed = embed,
					lastMessage = Context.Channel.SendMessageAsync("", false, embed.Build()).Result.Id
				});
			}

			await Context.Message.DeleteAsync();
		}
	}
}
