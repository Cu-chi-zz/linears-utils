using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace LinearsBot
{
    public class Admin : ModuleBase<SocketCommandContext>
    {
        [Command("config")]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task SetConfiguration(SocketRole role, SocketTextChannel channel)
        {
			// Verif si ROLE DU BOT AU DESSUS DU ROLE STAFF !

			ServerList serverList = Functions.ReadFromBinaryFile<ServerList>("serverlist");

			if (serverList != null)
			{
				if (serverList.serverList.ContainsKey(Convert.ToString(Context.Guild.Id)))
				{
					serverList.serverList[Convert.ToString(Context.Guild.Id)][0] = Convert.ToString(role.Id);
					serverList.serverList[Convert.ToString(Context.Guild.Id)][1] = Convert.ToString(channel.Id);
					Functions.WriteToBinaryFile("serverlist", serverList);

					var embed = new EmbedBuilder
					{
						Color = Color.Blue,
						Title = " - Configuration - ",
						Description = $"Serveur `{Context.Guild.Name}` ajouté.",
						Timestamp = DateTime.Now,
						Footer = new EmbedFooterBuilder()
						{
							IconUrl = Functions.GetAvatarUrl(Context.User, 32),
							Text = Context.User.Username + "#" + Context.User.Discriminator
						}
					};
					embed.AddField(new EmbedFieldBuilder
					{
						IsInline = true,
						Name = "Rôle",
						Value = role.Mention
					}).AddField(new EmbedFieldBuilder
					{
						IsInline = true,
						Name = "Channel",
						Value = channel.Mention
					});

					await Context.Channel.SendMessageAsync("", false, embed.Build());
				}
				else
				{
					serverList.serverList.Add(Convert.ToString(Context.Guild.Id), new string[2]
					{
						Convert.ToString(role.Id),
						Convert.ToString(channel.Id)
					});
					Functions.WriteToBinaryFile("serverlist", serverList);

					var embed = new EmbedBuilder
					{
						Color = Color.Blue,
						Title = " - Configuration - ",
						Description = $"Serveur `{Context.Guild.Name}` ajouté.",
						Timestamp = DateTime.Now,
						Footer = new EmbedFooterBuilder()
						{
							IconUrl = Functions.GetAvatarUrl(Context.User, 32),
							Text = Context.User.Username + "#" + Context.User.Discriminator
						}
					};
					embed.AddField(new EmbedFieldBuilder
					{
						IsInline = true,
						Name = "Rôle",
						Value = role.Mention
					}).AddField(new EmbedFieldBuilder
					{
						IsInline = true,
						Name = "Channel",
						Value = channel.Mention
					});

					await Context.Channel.SendMessageAsync("", false, embed.Build());
				}
			}
			else
			{
				Functions.WriteToBinaryFile("serverlist", new ServerList
				{
					serverList = new Dictionary<string, string[]>()
				});
			}
		}

		[Command("getconfig")]
		[RequireBotPermission(GuildPermission.Administrator)]
		public async Task GetConfiguration()
		{
			ServerList serverList = Functions.ReadFromBinaryFile<ServerList>("serverlist");

			if (serverList != null)
			{
				if (serverList.serverList.ContainsKey(Convert.ToString(Context.Guild.Id)))
				{
					string role = serverList.serverList[Convert.ToString(Context.Guild.Id)][0];
					string channel = serverList.serverList[Convert.ToString(Context.Guild.Id)][1];

					var embed = new EmbedBuilder
					{
						Color = Color.Blue,
						Title = " - Configuration - ",
						Description = $"Serveur `{Context.Guild.Name}` trouvé.",
						Timestamp = DateTime.Now,
						Footer = new EmbedFooterBuilder()
						{
							IconUrl = Functions.GetAvatarUrl(Context.User, 32),
							Text = Context.User.Username + "#" + Context.User.Discriminator
						}
					};
					embed.AddField(new EmbedFieldBuilder
					{
						IsInline = true,
						Name = "Rôle",
						Value = $"<@&{role}>"
					}).AddField(new EmbedFieldBuilder
					{
						IsInline = true,
						Name = "Channel",
						Value = $"<#{channel}>"
					});

					await Context.Channel.SendMessageAsync("", false, embed.Build());
				}
				else
				{
					
					var embed = new EmbedBuilder
					{
						Color = Color.Red,
						Title = " - Configuration - ",
						Description = $"Serveur `{Context.Guild.Name}` introuvable, essayez d'ajouter une configuration via la commande :\n`config [rôle] [channel]`.",
						Timestamp = DateTime.Now,
						Footer = new EmbedFooterBuilder()
						{
							IconUrl = Functions.GetAvatarUrl(Context.User, 32),
							Text = Context.User.Username + "#" + Context.User.Discriminator
						}
					};

					await Context.Channel.SendMessageAsync("", false, embed.Build());
				}
			}
			else
			{
				Functions.WriteToBinaryFile("serverlist", new ServerList
				{
					serverList = new Dictionary<string, string[]>()
				});
			}
		}

		[Command("reloadconfig")]
        [RequireOwner] // Require the bot owner to execute the command successfully.
        public async Task ReloadConfig()
        {
            await Functions.SetBotStatusAsync(Context.Client);
            await ReplyAsync("Reloaded!");
        }
    }
}
