using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Globalization;

namespace LinearsBot
{
    public class Informations : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping() => await ReplyAsync($"Latency: {Context.Client.Latency} ms");

    }
}
