using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Diagnostics;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace LinearsBot
{
    public class StaffManagement : ModuleBase<SocketCommandContext>
    {
        
        [Command("staff")]
		[RequireOwner()]
        public async Task AddStaff(string addOrRemove, SocketGuildUser user)
		{
			if (addOrRemove != "add" && addOrRemove != "remove" || user == null)
			{
				await ReplyAsync($"{Context.User.Mention} voici l'usage de la commande : \n" +
					$"**staff** *[add/remove]* *[id/mention]*");
				return;
			}

			if (addOrRemove == "add")
			{
				StaffList newStaffList = Functions.ReadFromBinaryFile<StaffList>("stafflist");
				if (newStaffList != null)
				{
					newStaffList.staffList.Add(Convert.ToString(user.Id));
					Functions.WriteToBinaryFile("stafflist", newStaffList);
				}
				else
				{
					Functions.WriteToBinaryFile("stafflist", new StaffList
					{
						staffList = new List<string> { Convert.ToString(user.Id) }
					});
				}

				var embed = new EmbedBuilder
				{
					Color = Color.Green,
					Title = "Modification de la liste des staffs :",
					Description = $"Ajout de {user.Mention}",
					Timestamp = DateTime.Now,
					Footer = new EmbedFooterBuilder()
					{
						IconUrl = Functions.GetAvatarUrl(Context.User, 32),
						Text = Context.User.Username + "#" + Context.User.Discriminator
					}
				};

				await Context.Channel.SendMessageAsync("", false, embed.Build());
			}
			else
			{
				StaffList newStaffList = Functions.ReadFromBinaryFile<StaffList>("stafflist");
				if (newStaffList != null)
				{
					while (newStaffList.staffList.Contains(Convert.ToString(user.Id)))
						newStaffList.staffList.Remove(Convert.ToString(user.Id));

					Functions.WriteToBinaryFile("stafflist", newStaffList);

					var embed = new EmbedBuilder
					{
						Color = Color.Red,
						Title = "Modification de la liste des staffs :",
						Description = $"Suppression de {user.Mention}",
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
		}

		[Command("stafflist")]
		public async Task StaffList()
		{
			StaffList newStaffList = Functions.ReadFromBinaryFile<StaffList>("stafflist");
			if (newStaffList != null)
			{
				string list = null;
				foreach (string staff in newStaffList.staffList)
				{
					list += $"<@{staff}>\n";
				}
				if (!string.IsNullOrEmpty(list))
					list.Substring(list.Length - 2);

				var embed = new EmbedBuilder
				{
					Color = Color.Purple,
					Title = "Liste des staffs :",
					Description = list ?? "*Vide*",
					Timestamp = DateTime.Now,
					Footer = new EmbedFooterBuilder()
					{
						IconUrl = Functions.GetAvatarUrl(Context.User, 32),
						Text = Context.User.Username + "#" + Context.User.Discriminator
					}
				};

				await Context.Channel.SendMessageAsync("", false, embed.Build());
			}
			else
			{
				var embed = new EmbedBuilder
				{
					Color = Color.Purple,
					Title = "Liste des staffs :",
					Description = $"*Vide*",
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

		[Command("staffrefresh")]
		[RequireOwner()]
		public async Task ReloadStaffList()
		{
			StaffList staffList = Functions.ReadFromBinaryFile<StaffList>("stafflist");
			if (staffList != null)
			{
				await Context.Channel.SendMessageAsync("Rafraichissement...");
				ServerList serverList = Functions.ReadFromBinaryFile<ServerList>("serverlist");

				if (serverList != null)
				{
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					foreach (var (key, value) in serverList.serverList)
					{
						SocketGuild socketGuild = Context.Client.GetGuild(Convert.ToUInt64(key));
						SocketTextChannel socketTextChannel = socketGuild.GetTextChannel(Convert.ToUInt64(value[1]));

						foreach (var role in socketGuild.Roles)
						{
							if (role.Id == Convert.ToUInt64(value[0]))
							{
								foreach (var user in role.Members)
								{
									bool isInStaffList = false;
									foreach (string staffId in staffList.staffList)
									{
										if (Convert.ToString(user.Id) == staffId)
										{
											isInStaffList = true;
											break;
										}
									}
									if (!isInStaffList)
										await user.RemoveRoleAsync(Convert.ToUInt64(value[0]));
								}
							}
						}

						foreach (string staffId in staffList.staffList)
						{
							SocketGuildUser socketGuildUser = socketGuild.GetUser(Convert.ToUInt64(staffId));

							if (!socketGuildUser.Roles.Contains(socketGuild.GetRole(Convert.ToUInt64(value[0]))))
							{
								await socketGuildUser.AddRoleAsync(Convert.ToUInt64(value[0]));
							}
						}

						var embed = new EmbedBuilder
						{
							Color = Color.Teal,
							Title = "Rafraichissement de la liste des staffs",
							Description = $"Liste des staffs rafraichie avec succès pour votre serveur !",
							Timestamp = DateTime.Now,
							Footer = new EmbedFooterBuilder()
							{
								IconUrl = Functions.GetAvatarUrl(Context.User, 32),
								Text = Context.User.Username + "#" + Context.User.Discriminator
							}
						};
						await socketTextChannel.SendMessageAsync("", false, embed.Build());
						Console.WriteLine("Sended for server " + socketGuild.Name);
					}
					stopwatch.Stop();
					await Context.Channel.SendMessageAsync($"Le raffraichissement à prit {stopwatch.Elapsed}");
				}
				else
				{
					var embed = new EmbedBuilder
					{
						Color = Color.Red,
						Title = "Erreur",
						Description = $"La liste des serveurs est *vide*...",
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
				var embed = new EmbedBuilder
				{
					Color = Color.Red,
					Title = "Erreur",
					Description = $"La liste des staffs est *vide*...",
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
	}
}
