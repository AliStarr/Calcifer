using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Calcifer.Modules.Bot
{
    [Group("set", "Set bot attributes")]
    public class BotModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("username", "Change the bots nickname.")]
        public async Task SetUsername(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                await RespondAsync("Input can not be empty.");
            }
                
            var client = Context.Client as DiscordSocketClient;
            await Context.Client.CurrentUser.ModifyAsync(x => x.Username = value).ConfigureAwait(false);
            await RespondAsync("Username updated :ok:");
        }

        [SlashCommand("game", "Sets the bots game status.")]
        public async Task Game(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value cannot be empty");
            }
                
            var client = Context.Client as DiscordSocketClient;
            await client.SetGameAsync(value).ConfigureAwait(false);
            await RespondAsync("Bot Game updated!");
        }

        // TODO: Fix this
        //[SlashCommand("Avatar", "Change the bots avatar.")]
        //public async Task Avatar(string value)
        //{
        //    if (string.IsNullOrWhiteSpace(value))
        //    {
        //        await RespondAsync("Value cannot be empty");
        //    }

        //    var url = value;
        //    if (value == "reset")
        //    {
        //        var app = await Context.Client.GetApplicationInfoAsync().ConfigureAwait(false);
        //        url = app.IconUrl;
        //    }
        //    var q = new Uri(url);
        //    using (var client = new HttpClient())
        //    {
        //        await client.DownloadAsync(q, q.LocalPath.Replace("/", "")).ConfigureAwait(false);
        //        using (var imagestream = new FileStream(q.LocalPath.Replace("/", ""), FileMode.Open))
        //        {
        //            await Context.Client.CurrentUser.ModifyAsync(x => x.Avatar = new Image(imagestream)).ConfigureAwait(false);
        //        }
        //        File.Delete(q.LocalPath.Replace("/", ""));
        //    }
        //    await RespondAsync("Bot Avatar Updated!").ConfigureAwait(false);
        //}

        readonly string[] allowedStatus = { "Offline", "Online", "Idle", "AFK", "Do Not Disturb", "Invisible" };
        // Can be either Offline, Online, Idle, AFK, DoNotDisturb or Invisible
        [SlashCommand("status","Set the bots status. Can be Offline, Online, Idle, AFK, Do Not Disturb or Invisible")]
        public async Task Status(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                await RespondAsync("Value cannot be empty");
            }
            else if (!allowedStatus.Any(value.Contains))
            {
                await RespondAsync("Input must be Offline, Online, Idle, AFK, Do Not Disturb or Invisible");
            }
                
            var newStatus = Enum.Parse(typeof(UserStatus), value);
            await (Context.Client as DiscordSocketClient).SetStatusAsync((UserStatus)newStatus).ConfigureAwait(false);
            await RespondAsync($"Set status to: {value}").ConfigureAwait(false);
        }
    }
}
