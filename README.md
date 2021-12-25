# linears-utils
> Discord bot made with C# and [**DiscordNet**](https://github.com/discord-net/Discord.Net) library for the [**Linears project**](https://discord.gg/tC482G3GXu) server on [**FiveM**](https://fivem.net/)

Commands list
| Accessibility       | Command Name  | Parameters       | Description                                                                                         | Example                                                        |
|---------------------|---------------|------------------|-----------------------------------------------------------------------------------------------------|----------------------------------------------------------------|
| Guild Administrator | config        | role, channel    | Configure your server settings and send information to the bot.                                     | config @staff #linears-bot-logs                                |
| Guild Administrator | getconfig     | NONE             | Get your current saved settings from the bot.                                                       | getconfig                                                      |
| Everyone            | ping          | NONE             | Get the bot ping                                                                                    | ping                                                           |
| Bot Owner           | staff         | add/remove, user | Add a member to the bot's staff list                                                                | staff add @Cuchi                                               |
| Everyone            | stafflist     | NONE             | Get the current bot's staff list                                                                    | stafflist                                                      |
| Bot Owner           | staffrefresh  | NONE             | Refresh the bot's staff list in all servers                                                         | staffrefresh                                                   |
| Bot Owner           | persistent    | message          | Send a persistent message in the channel where the message is sended (needs to be manually removed) | peristent Hey everyone, currently working on the server :wink: |
| Bot Owner           | rempersistent | NONE             | Remove the persistent message in the channel where the message is sended                            | rempersistent                                                  |
Configuration file structure
```json
{
    "token": "",
    "prefixes": ["lns.", "linears."],
    "ownerId": 0,
    "keepUpdated": true
}
```
`token` -> your bot token from [dev portal](https://discord.com/developers/applications)  
`prefixes` -> List of possible prefixes  
`ownerId` -> owner of bot id  
`keepUpdated` -> log a message when an update is available  

*We ❤️‍🔥 collaborators.*
