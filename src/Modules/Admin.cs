using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord_Bot
{
    public class Admin : ModuleBase<SocketCommandContext>
    {
        [Command("purge")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int delNumber)
        {
            // var channel = Context.Channel as SocketTextChannel;
            // var items = await channel.GetMessagesAsync(delNumber + 1).FlattenAsync();
            // await channel.DeleteMessagesAsync(items);
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
