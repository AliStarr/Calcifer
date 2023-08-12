using Calcifer.Utility;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Calcifer.Modules
{
    public class PublicModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ping", "Returns Gateway latency, Response latency and Delta (response - gateway).")]
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
            await RespondAsync("", embed: embed.Build());
        }

        [SlashCommand("urban", "Search term dictionary for a term.")]
        public async Task Urban([Remainder] string term = null)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                throw new NullReferenceException("Please provide a search term");
            }

            var embed = new EmbedBuilder();
            var vc = new HttpClient();
            embed.WithAuthor(x =>
            {
                x.Name = "Urban Dictionary";
                x.WithIconUrl("https://lh3.googleusercontent.com/4hpSJ4pAfwRUg-RElZ2QXNh_pV01Z96iJGT2BFuk_RRsNc-AVY7cZhbN2g1zWII9PBQ=w170");
            });
            string req = await vc.GetStringAsync("http://api.urbandictionary.com/v0/define?term=" + term);
            embed.WithColor(new Color(153, 30, 87));

            MatchCollection col = Regex.Matches(req, @"(?<=definition"":"")[ -z~-🧀]+(?="",""permalink)");
            MatchCollection col2 = Regex.Matches(req, @"(?<=example"":"")[ -z~-🧀]+(?="",""thumbs_down)");

            if (col.Count == 0)
            {
                await RespondAsync("Couldn't find anything with that input");
                return;
            }
            Random r = new();
            string outpt = "Failed fetching embed from Urban Dictionary, please try later!";
            string outpt2 = "No Example";
            int max = r.Next(0, col.Count);
            for (int i = 0; i <= max; i++)
            {
                outpt = term + "\r\n\r\n" + col[i].Value;
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

            await RespondAsync("", embed: embed.Build());
        }

        [SlashCommand("guildinfo", "Displays information about a guild")]
        public async Task GuildInfo()
        {
            var embed = new EmbedBuilder();
            var gld = Context.Guild;
            if (!string.IsNullOrWhiteSpace(gld.IconUrl))
            {
                embed.ThumbnailUrl = gld.IconUrl;
            }

            embed.Color = new Color(153, 30, 87);
            embed.Title = $"{gld.Name} Information";
            embed.Description = $"**Guild ID: **{gld.Id}\n**Guild Owner: **{gld.Owner.Mention}\n" +
                $"**Created At: **{gld.CreatedAt}\n" +
                $"**MFA Level: **{gld.MfaLevel}\n**Verification Level: **{gld.VerificationLevel}\n";
            await RespondAsync("", embed: embed.Build());
        }

        [SlashCommand("info", "Displays Bot Information")]
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
                $"**Github Repo: **https://github.com/AliStarr/Calcifer\n" +
                $"**Discord .Net Libary version: **{DiscordConfig.Version}\n" +
                $"**Bot Version and Release: **\n" +
                $"**Runtime: **{RuntimeInformation.FrameworkDescription}\n" +
                $"**Uptime (D.H:M:S): **{GetUpTime()}\n\n" +
                $"**Heap Size: **{GetHeapSize()}MB\n" +
                $"**Guilds: **{(Context.Client as DiscordSocketClient).Guilds.Count}\n" +
                $"**Channels: **{(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Channels.Count)}\n" +
                $"**Users: **{(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count)}\n";
            await RespondAsync("", embed: embed.Build());
        }

        [SlashCommand("bug", "Pings Alister with bug report info")]
        public async Task ReportBug()
        {
            await RespondAsync("Pinging Alister to tell him that he sucks at programming...");
            var msg = $"{Context.User.Mention} submitted a bug report! Guild: {Context.Guild.Name} Channel: {Context.Channel.Name}.";
            var dmChannel = await Context.Guild.Owner.CreateDMChannelAsync();
            await dmChannel.SendMessageAsync(msg);
            await RespondAsync(":ok:");
        }

        [SlashCommand("apotd", "NASA astronomy picture of the day.")]
        public async Task APOTD()
        {
            var embed = new EmbedBuilder();
            // Quick and dirty JSON API consumer.
            HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync("https://api.nasa.gov/planetary/apod?api_key=DEMO_KEY");
           try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                await RespondAsync($"Got non-success response. {response.StatusCode} {response.ReasonPhrase}\n\nEXCEPTION: {ex.Message}");
                return;
            }
            
            string resposeBody = await response.Content.ReadAsStringAsync();
            var jsonify = JsonConvert.DeserializeObject<APOTDHelper>(resposeBody);

            embed.ImageUrl = jsonify.URL.ToString();
            embed.Title = jsonify.Title;
            embed.WithAuthor(x =>
            {
                x.Name = "NASA Astronomy Picture Of The Day";
            });
            embed.WithFooter(x =>
            {
                x.Text = $"HD Image link: {jsonify.HDUrl}\n{jsonify.Copyright}";
            });
            embed.Description = $"{jsonify.Explanation}";

            await RespondAsync("", embed: embed.Build());
        }

        // Helpers
        private static string GetUpTime()
            => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
        private static string GetHeapSize()
            => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
    }
}
