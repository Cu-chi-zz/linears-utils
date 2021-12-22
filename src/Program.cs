using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Discord_Bot
{
    public class Program
    {
		static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            using var services = ConfigureServices();

			ColoredMessage(ConsoleColor.Yellow, "-> Le bot démarre...");
			DiscordSocketClient client = services.GetRequiredService<DiscordSocketClient>();

			client.Log += Log;
            services.GetRequiredService<CommandService>().Log += Log;

            JObject config = Functions.GetConfig();
            string token = config["token"].Value<string>();
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

			await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

			await Task.Delay(-1);
        }

        public ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
				.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
				{
					MessageCacheSize = 50,
					LogLevel = LogSeverity.Info,
					AlwaysDownloadUsers = true,
					GatewayIntents = GatewayIntents.All
				}))
				.AddSingleton(new CommandService(new CommandServiceConfig
                { 
                    LogLevel = LogSeverity.Info,
                    DefaultRunMode = RunMode.Async,
                    CaseSensitiveCommands = false,
                }))
                .AddSingleton<CommandHandlingService>()
                .BuildServiceProvider();
        }

        private Task Log(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

		private void ColoredMessage(ConsoleColor color, string msg)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(msg);
			Console.ForegroundColor = ConsoleColor.White;
		}
    }
}
