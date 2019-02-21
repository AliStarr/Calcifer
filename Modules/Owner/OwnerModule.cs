using Booper.Common;
using Booper.Preconditions;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;

namespace Booper.Modules.Owner
{
    [MinPermissions(AccessLevel.BotOwner)]
    public class OwnerModule : ModuleBase<SocketCommandContext>
    {
        private static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.Unicode.GetBytes(value ?? ""));
        }

        Random rnd = new Random();
        int rndResult;


        [Command("Stop")]
        [Alias("Quit")]
        [Summary("Exits the bot from running - _Owner Only_")]
        [Remarks("~Stop")]
        [Priority(1)]
        public async Task Stop()
        {
            rndResult = rnd.Next(1, CommonStrings.quitStrings.Length);
            await ReplyAsync(CommonStrings.quitStrings[rndResult]);
            Environment.Exit(0x0);
        }
        /*
        [Command("Restart")]
        [Alias("Reboot")]
        [Summary("Restarts the Bot - _Owner Only_")]
        [Remarks("~Restart")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Priority(1)]
        public async Task Restart()
        {
            rndResult = rnd.Next(1, CommonStrings.restartStrings.Length);
            await ReplyAsync(CommonStrings.restartStrings[rndResult]);
            Process.Start(Application.ExecutablePath);
            Environment.Exit(0x0);

        }
        */




        [Command("ServerList")]
        [Summary("Gets all the servers the bot is connected to.")]
        [Remarks("~Serverlist")]
        [Alias("sl")]
        public async Task ServerListAsync()
        {
            var client = Context.Client as DiscordSocketClient;
            var embed = new EmbedBuilder();
            foreach (SocketGuild guild in client.Guilds)
            {
                embed.AddField(x =>
                {
                    x.Name = $"{guild.Name} || {guild.Id}";
                    x.Value = $"Guild Owner: {guild.Owner} || {guild.OwnerId}\nGuild Members: {guild.MemberCount}";
                    x.IsInline = true;
                });
            }
            embed.Title = "=== Server List ===";
            embed.Color = new Color(255, 210, 50);
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = $"Total Guilds: {client.Guilds.Count.ToString()}"
            };
            await ReplyAsync("", embed: embed.Build());
        }

        [Command("Leave")]
        [Summary("Forces the bot to leave the specified server")]
        [Remarks("~Leave <ServerID> <Message>")]
        public async Task LeaveAsync(ulong ServerId, [Remainder] string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
                await ReplyAsync("You must provide a reason for leaving the server.");
            var client = Context.Client;
            var gld = client.GetGuild(ServerId);
            var ch = gld.DefaultChannel;
            var embed = new EmbedBuilder()
            {
                Description = $"Calcifer has been forced to leave this Server by its owner.\n**Reason:** {msg}",
                Color = new Color(255, 0, 0),
                Author = new EmbedAuthorBuilder()
                {
                    Name = Context.User.Username,
                    IconUrl = Context.User.GetAvatarUrl()
                }
            };
            await ch.SendMessageAsync("", embed: embed.Build());
            await Task.Delay(5000);
            await gld.LeaveAsync();
            await ReplyAsync($"Calcifer has left {ServerId}.");
        }

        [Command("Broadcast")]
        [Summary("Sends a message to ALL severs that the bot is connected to.")]
        [Remarks("~Broadcast <Message>")]
        [Alias("Yell", "Shout")]
        public async Task AsyncBroadcast([Remainder] string msg)
        {
            var glds = (Context.Client as DiscordSocketClient).Guilds;
            var defaultchan = glds.Select(g => g.GetChannel(g.Id)).Cast<ITextChannel>();
            await Task.WhenAll(defaultchan.Select(c => c.SendMessageAsync(msg)));
        }
    }
}
