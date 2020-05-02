using Calcifer.Common;
using Calcifer.Preconditions;
using Calcifer.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Calcifer.Modules.Bot
{
    [MinPermissions(AccessLevel.BotOwner)]
    [Group("Set")]

    public class BotModule : ModuleBase<SocketCommandContext>
    {
        [Command("Username"), RequireContext(ContextType.Guild)]
        [Summary("Sets the Bot's username. _Owner Only_")]
        [Remarks("~Set Username <input> ")]
        public async Task SetUsernameAsync(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Input can not be empty.");
            var client = Context.Client as DiscordSocketClient;
            await Context.Client.CurrentUser.ModifyAsync(x => x.Username = value).ConfigureAwait(false);
            await ReplyAsync("Username updated :ok:").ConfigureAwait(false);

        }

        [Command("Game"), RequireContext(ContextType.Guild)]
        [Summary("Sets the bots game - _Owner only command_")]
        [Remarks("~Set Game <input>")]
        public async Task GameAsync([Remainder] string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be empty");
            var client = Context.Client as DiscordSocketClient;
            await client.SetGameAsync(value).ConfigureAwait(false);
            await ReplyAsync("Bot Game updated!").ConfigureAwait(false);
        }

        [Command("Avatar"), RequireContext(ContextType.Guild)]
        [Summary("Sets the bots avatar - _Owner Only_")]
        [Remarks("~Set Avatar <url>")]
        public async Task AvatarAsync([Remainder] string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be empty");
            var url = value;
            if (value == "reset")
            {
                var app = await Context.Client.GetApplicationInfoAsync().ConfigureAwait(false);
                url = app.IconUrl;
            }
            var q = new Uri(url);
            using (var client = new HttpClient())
            {
                await client.DownloadAsync(q, q.LocalPath.Replace("/", "")).ConfigureAwait(false);
                using (var imagestream = new FileStream(q.LocalPath.Replace("/", ""), FileMode.Open))
                {
                    await Context.Client.CurrentUser.ModifyAsync(x => x.Avatar = new Image(imagestream)).ConfigureAwait(false);
                }
                File.Delete(q.LocalPath.Replace("/", ""));
            }
            await ReplyAsync("Bot Avatar Updated!").ConfigureAwait(false);

        }

        // Can be either Offline, Online, Idle, AFK, DoNotDisturb or Invisible
        [Command("Status"), RequireContext(ContextType.Guild)]
        [Summary("Sets the bots status (Offline, Online, Idle, AFK, DoNotDisturb or Invisible) - _Owner Only_")]
        [Remarks("~Set Status <input>")]
        public async Task StatusAsync([Remainder] string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be empty");
            var newStatus = Enum.Parse(typeof(UserStatus), value);
            await (Context.Client as DiscordSocketClient).SetStatusAsync((UserStatus)newStatus).ConfigureAwait(false);
            await ReplyAsync($"Set status to: {value}").ConfigureAwait(false);
        }
    }
}
