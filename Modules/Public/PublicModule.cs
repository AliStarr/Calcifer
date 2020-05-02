using Calcifer.Common;
using Calcifer.Preconditions;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Calcifer.Modules
{
    public class PublicModule : ModuleBase
    {

        [MinPermissions(AccessLevel.BotOwner)]
        [Command("Invite")]
        [Summary("Creates an Oauth2 invite for the bot.")]
        [Remarks("~Invite")]
        public async Task Invite()
        {
            var application = await Context.Client.GetApplicationInfoAsync();
            await ReplyAsync($"A user with 'MANAGE_SERVER' can invite me to your server here:" +
                $" <https://discordapp.com/oauth2/authorize?client_id={application.Id}&scope=bot>");
        }


        [Command("Say")]
        [Alias("echo")]
        [Summary("Echos the input.")]
        [Remarks("~Say <input>")]
        public async Task Say([Remainder] string input)
        {
            if (input.StartsWith("~"))
            {
                await ReplyAsync("Nice try.");
                return; // Prevents the bot from being issued commands from this command creating a clusterfuck. Although this is handled by the commandhandler
            }
            else
            {
                await ReplyAsync('\u200B' + input); // ZWS avoid triggering other bots.
            }
        }

        [Command("Ping")]
        [Summary("Returns Gateway latency, Response latency and Delta (response - gateway).")]
        [Remarks("~Ping")]
        public async Task Ping()
        {
            var sw = Stopwatch.StartNew();
            var client = Context.Client as DiscordSocketClient;
            var Gateway = client.Latency;
            var embed = new EmbedBuilder()
                .WithTitle("Ping results")
                .WithDescription($"**Gateway Latency:** {Gateway} ms" +
                $"\n**Response Latency:** {sw.ElapsedMilliseconds} ms" +
                $"\n**Delta:** {Gateway - sw.ElapsedMilliseconds} ms")
                .WithColor(new Color(244, 66, 125));
            await ReplyAsync("", embed: embed.Build());
        }

        [Command("Urban")]
        [Alias("ud")]
        [Summary("Returns the first result of the input from Urbandictonary.com.")]
        [Remarks("~Urban <input>")]
        public async Task UrbanAsync([Remainder] string urban = null)
        {
            if (string.IsNullOrWhiteSpace(urban))
                throw new NullReferenceException("Please provide a search term");
            var embed = new EmbedBuilder();
            var vc = new HttpClient();
            embed.WithAuthor(x =>
            {
                x.Name = "Urban Dictionary";
                x.WithIconUrl("https://lh3.googleusercontent.com/4hpSJ4pAfwRUg-RElZ2QXNh_pV01Z96iJGT2BFuk_RRsNc-AVY7cZhbN2g1zWII9PBQ=w170");
            });
            string req = await vc.GetStringAsync("http://api.urbandictionary.com/v0/define?term=" + urban);
            embed.WithColor(new Color(153, 30, 87));

            MatchCollection col = Regex.Matches(req, @"(?<=definition"":"")[ -z~-🧀]+(?="",""permalink)");
            MatchCollection col2 = Regex.Matches(req, @"(?<=example"":"")[ -z~-🧀]+(?="",""thumbs_down)");

            if (col.Count == 0)
            {
                await ReplyAsync("Couldn't find anything with that input");
                return;
            }
            Random r = new Random();
            string outpt = "Failed fetching embed from Urban Dictionary, please try later!";
            string outpt2 = "No Example";
            int max = r.Next(0, col.Count);
            for (int i = 0; i <= max; i++)
            {
                outpt = urban + "\r\n\r\n" + col[i].Value;
            }

            for (int i = 0; i <= max; i++)
            {
                outpt2 = "\r\n\r\n" + col2[i].Value;
            }

            outpt = outpt.Replace("\\r", "\r");
            outpt = outpt.Replace("\\n", "\n");
            outpt2 = outpt2.Replace("\\r", "\r");
            outpt2 = outpt2.Replace("\\n", "\n");

            embed.AddField(x =>
            {
                x.Name = $"Definition";
                x.Value = outpt;
            });

            embed.AddField(x =>
            {
                x.Name = "Example";
                x.Value = outpt2;
            });

            await ReplyAsync("", embed: embed.Build());
        }

        [Command("GuildInfo")]
        [Alias("gi", "ServerInfo")]
        [Summary("Displays information about a guild")]
        [Remarks("~GuildInfo")]
        public async Task GuildInfoAsync()
        {
            var embed = new EmbedBuilder();
            var gld = Context.Guild;
            if (!string.IsNullOrWhiteSpace(gld.IconUrl))
                embed.ThumbnailUrl = gld.IconUrl;
            embed.Color = new Color(153, 30, 87);
            embed.Title = $"{gld.Name} Information";
            embed.Description = $"**Guild ID: **{gld.Id}\n**Guild Owner: **{gld.GetOwnerAsync().GetAwaiter().GetResult().Mention}\n" +
                $"**Default Channel: **{gld.GetDefaultChannelAsync().GetAwaiter().GetResult().Mention}\n**Voice Region: **{gld.VoiceRegionId}\n" +
                $"**Created At: **{gld.CreatedAt}\n**Available? **{gld.Available}\n" +
                $"**Default Msg Notif: **{gld.DefaultMessageNotifications}\n**Embeddable? **{gld.IsEmbeddable}\n" +
                $"**MFA Level: **{gld.MfaLevel}\n**Verification Level: **{gld.VerificationLevel}\n";
            await ReplyAsync("", false, embed.Build());
        }

        [Command("Info")]
        [Summary("Displays bot information")]
        [Remarks("~Info")]
        public async Task Info()
        {
            var embed = new EmbedBuilder();
            var application = await Context.Client.GetApplicationInfoAsync();
            var gld = Context.Guild;
            if (!string.IsNullOrWhiteSpace(gld.IconUrl))
                embed.ThumbnailUrl = "https://imageog.flaticon.com/icons/png/512/36/36601.png";
            embed.Color = new Color(126, 172, 247);
            embed.Title = $"Calcifer  info and stats";
            embed.Description =
                $"**Author: **{application.Owner.Mention} ID ({application.Owner.Id})\n" +
                // $"**Github Repo: **{CommonStrings.gitRepo}\n" +
                $"**Discord .Net Libary version: **{DiscordConfig.Version}\n" +
                $"**Bot Version and Release: **{CommonStrings.BotVersion}\n" +
                $"**Runtime: **{RuntimeInformation.FrameworkDescription}\n" +
                $"**Uptime (D.H:M:S): **{GetUpTime()}\n\n" +
                $"**Heap Size: **{GetHeapSize()}MB\n" +
                $"**Guilds: **{(Context.Client as DiscordSocketClient).Guilds.Count}\n" +
                $"**Channels: **{(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Channels.Count)}\n" +
                $"**Users: **{(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count)}\n";
            await ReplyAsync("", false, embed.Build());
        }

        [Command("reportbug"), Alias("rb")]
        [Summary("Pings Alister so he can fix issues with the bot.")]
        [Remarks("~reportbug OR ~rb")]
        public async Task ReportBugAsync()
        {
            await ReplyAsync("Pinging Alister to tell him that he sucks at programming...");
            var msg = $"{Context.Message.Author.Mention} submitted a bug report! Guild: {Context.Guild.Name} Channel: {Context.Channel.Name}.";
            var dmChannel = await Context.Guild.GetOwnerAsync();
            await dmChannel.SendMessageAsync(msg);
        }

        private static string GetUpTime()
            => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");

        private static string GetHeapSize()
            => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
    }
}
