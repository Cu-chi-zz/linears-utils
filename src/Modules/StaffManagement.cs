using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace Discord_Bot
{
    public class StaffManagement : ModuleBase<SocketCommandContext>
    {
        
        [Command("staff")]
        public async Task AddStaff(string addOrRemove = "", SocketGuildUser user = null)
		{
			if (addOrRemove != "add" && addOrRemove != "remove" || user == null)
			{
				await ReplyAsync($"{Context.User.Mention} voici l'usage de la commande : \n" +
					$"**staff** *[add/remove]* *[id/mention]*");
				return;
			}

			if (addOrRemove == "add")
			{
				StaffList newStaffList = ReadFromBinaryFile<StaffList>("stafflist");
				if (newStaffList != null)
				{
					newStaffList.staffList.Add(Convert.ToString(user.Id));
					WriteToBinaryFile("stafflist", newStaffList);
				}
				else
				{
					WriteToBinaryFile("stafflist", new StaffList
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
				StaffList newStaffList = ReadFromBinaryFile<StaffList>("stafflist");
				if (newStaffList != null)
				{
					while (newStaffList.staffList.Contains(Convert.ToString(user.Id)))
						newStaffList.staffList.Remove(Convert.ToString(user.Id));

					WriteToBinaryFile("stafflist", newStaffList);

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
			StaffList newStaffList = ReadFromBinaryFile<StaffList>("stafflist");
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

		/// <summary>
		/// Writes the given object instance to a binary file.
		/// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
		/// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
		/// </summary>
		/// <typeparam name="T">The type of object being written to the XML file.</typeparam>
		/// <param name="filePath">The file path to write the object instance to.</param>
		/// <param name="objectToWrite">The object instance to write to the XML file.</param>
		/// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
		public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
		{
			using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
			{
				var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				binaryFormatter.Serialize(stream, objectToWrite);
			}
		}

		/// <summary>
		/// Reads an object instance from a binary file.
		/// </summary>
		/// <typeparam name="T">The type of object to read from the XML.</typeparam>
		/// <param name="filePath">The file path to read the object instance from.</param>
		/// <returns>Returns a new instance of the object read from the binary file.</returns>
		public static T ReadFromBinaryFile<T>(string filePath)
		{
			using (Stream stream = File.Open(filePath, FileMode.Open))
			{
				if (stream.Length != 0)
				{
					var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
					return (T)binaryFormatter.Deserialize(stream);
				}
				else
				{
					return default;
				}
			}
		}
	}
}
