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
			if (PersistentMessages.persistentMessages != null)
			{
				foreach (var (key, value) in PersistentMessages.persistentMessages)
				{
					if (rawMessage.Channel.Id == key)
					{
						PersistentMessages.persistentMessages[key] = new PersistentMessages.StructPersistentMessages
						{
							lastMessage = rawMessage.Channel.SendMessageAsync("", false, PersistentMessages.persistentMessages[key].embed.Build()).Result.Id
						};
					}
				}
			}

            if (rawMessage.Author.IsBot || !(rawMessage is SocketUserMessage message))
                return;
			
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
