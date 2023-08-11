using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calcifer.Modules.Owner
{
    [RequireOwner()]
    public class OwnerModule : InteractionModuleBase<SocketInteractionContext>
    {

        [SlashCommand("stop", "Stops the bot from running.")]
        public async Task Stop()
        {
            await RespondAsync("Stopping Calcifer.");
            Console.WriteLine("Bot Commanded to Stop!");
            Environment.Exit(1);
        }

        
        [SlashCommand("restart", "Restarts the bot.")]
        public async Task Restart()
        {
            await RespondAsync("Restarting. Currently doesn't restart. Gotta rework it.");
            Console.WriteLine("Bot Commanded to restart!");
            Environment.Exit(0); // TODO: Actually restart the bot. 
        }

        [SlashCommand("serverlist", "Gets a list of all servers the bot is connected to")]
        public async Task ServerList()
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
                Text = $"Total Guilds: {client.Guilds.Count}"
            };
            await RespondAsync("", embed: embed.Build());
        }

        [SlashCommand("leave", "Leave the specified server with a message.")]
        public async Task Leave(ulong ServerId, string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                await ReplyAsync("You must provide a reason for leaving the server.");
            }
                
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
            await RespondAsync($"Calcifer has left {ServerId}.");
        }

        [SlashCommand("broadcast", "Sends a message to the default channel of all connected servers.")]
        public async Task AsyncBroadcast(string msg)
        {
            var glds = (Context.Client as DiscordSocketClient).Guilds;
            var defaultchan = glds.Select(g => g.GetChannel(g.Id)).Cast<ITextChannel>();
            await Task.WhenAll(defaultchan.Select(c => c.SendMessageAsync(msg)));
        }

        private static MemoryStream GenerateStreamFromString(string value)
            => new(Encoding.Unicode.GetBytes(value ?? ""));
    }
}
