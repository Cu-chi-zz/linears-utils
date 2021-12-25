using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using Discord;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Linears;

namespace LinearsBot
{
	public partial class CommandHandlingService
    {
		private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;

		public CommandHandlingService(IServiceProvider services)
        {
			_commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            // Event handlers
            _client.Ready += ClientReadyAsync;
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage rawMessage)
        {
			if (rawMessage.Author.IsBot || !(rawMessage is SocketUserMessage message))
				return;

			if (PersistentMessages.persistentMessages != null)
			{
				JObject cfg = Functions.GetConfig();
				var channelInfo = rawMessage.Channel as SocketGuildChannel;
				if (channelInfo.Guild.OwnerId == JsonConvert.DeserializeObject<ulong>(cfg["ownerId"].ToString()))
				{
					List<ulong> needModification = new List<ulong>();
					foreach (var (key, value) in PersistentMessages.persistentMessages)
					{
						if (rawMessage.Channel.Id == key)
						{
							needModification.Add(key);
						}
					}

					if (needModification.Count > 0)
					{
						foreach (ulong channelId in needModification)
						{
							await rawMessage.Channel.GetCachedMessage(PersistentMessages.persistentMessages[channelId].lastMessage).DeleteAsync();

							PersistentMessages.persistentMessages[channelId] = new PersistentMessages.StructPersistentMessages
							{
								embed = PersistentMessages.persistentMessages[channelId].embed,
								lastMessage = rawMessage.Channel.SendMessageAsync("", false, PersistentMessages.persistentMessages[channelId].embed.Build()).Result.Id
							};
						}
					}
				}
			}
			
			var context = new SocketCommandContext(_client, message);

			int argPos = 0;

			JObject config = Functions.GetConfig();
			string[] prefixes = JsonConvert.DeserializeObject<string[]>(config["prefixes"].ToString());

			// Check if message has any of the prefixes or mentiones the bot.
			if (prefixes.Any(x => message.HasStringPrefix(x, ref argPos)) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
			{
				// Execute the command.
				var result = await _commands.ExecuteAsync(context, argPos, _services);
				if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
			}
        }

        private async Task ClientReadyAsync()
            => await Functions.SetBotStatusAsync(_client);

        public async Task InitializeAsync()
            => await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }
}
