using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Discord_Bot
{
    public static class Functions
    {
        public static async Task SetBotStatusAsync(DiscordSocketClient client)
        {
            await client.SetStatusAsync(UserStatus.Online);
            await client.SetGameAsync($"{client.Guilds.Count} serveurs | LNS", type: ActivityType.Watching);
		}

        public static JObject GetConfig()
        {
            using StreamReader configJson = new StreamReader(Directory.GetCurrentDirectory() + @"/config.json");
                return (JObject)JsonConvert.DeserializeObject(configJson.ReadToEnd());
        }

        public static string GetAvatarUrl(SocketUser user, ushort size = 1024)
        {
            return user.GetAvatarUrl(size: size) ?? user.GetDefaultAvatarUrl(); 
        }
    }
}
