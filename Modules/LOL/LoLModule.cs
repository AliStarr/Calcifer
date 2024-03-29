﻿using Camille.Enums;
using Camille.RiotGames;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GroupAttribute = Discord.Interactions.GroupAttribute;


namespace Calcifer.Modules
{
    [Group("lol", "Commands related to Leauge of Legends")]
    public class LoLModule : InteractionModuleBase<SocketInteractionContext>
    {
        // Grabbing the API key
        private readonly IConfiguration _config;
        public LoLModule(IConfiguration config)
        {
            _config = config;
        }

        // TODO: Maybe change this to be able to search multiple players at once.
        [SlashCommand("top10", "Get a players top 10 champions.")]
        public async Task TopChamps(string playerName)
        {
            // get API key from our fancy config manager
            var riotApi = RiotGamesApi.NewInstance(_config["RiotApiKey"]); // TODO: Maybe move this out into its own thing so it's not called in every function that needs it.

            var summoner = await riotApi.SummonerV4().GetBySummonerNameAsync(PlatformRoute.OC1, playerName);
            var masteries = await riotApi.ChampionMasteryV4().GetAllChampionMasteriesAsync(PlatformRoute.OC1, summoner.Id);
            // Store results in key value pairs
            Dictionary<string, int> results = new();
            var embed = new EmbedBuilder
            {
                Title = $"{playerName}'s Top 10 champions by mastery level"
            };

            // Iterate over the top 10 returned by riot
            for (var i = 0; i < 10; i++)
            {
                // Grab our mastery levels
                var mastery = masteries[i];
                // Grab the champion associated
                var champ = (Champion)mastery.ChampionId;
                // Lump them in the Dictionary
                results.Add(champ.ToString(), mastery.ChampionLevel);

            }
            // Pile them into the embed with each in a new field
            foreach (var str in results)
            {
                embed.AddField($"Champion: {str.Key}", $"Mastery Level: {str.Value}");
            }
            await RespondAsync("", embed: embed.Build());
        }

        [SlashCommand("last10matches", "Get the last 10 matches played by a player by queue. Default queue is Draft Pick.")]
        public async Task LastTenMatches(string playerName/*, string queue = "Draft Pick" */) // We don't care about anything but Draft pick for now.
        {
            /* According to the riotAPI, you have to get matches with the players puuid which is riots internal id.
            * So we pull that from SummonerV4. THEN we have to call two seperate endpoints to get the matches (match/v5/by-puuid) and
            * the information about the matches (match/v5/matches/{matchid} like win/loss, who was in it ect. This adds up real quck against the 
            * rate limits of the dev api key (20 req per sec).
            */

            var riotApi = RiotGamesApi.NewInstance(_config["RiotApiKey"]);
            var summoner = await riotApi.SummonerV4().GetBySummonerNameAsync(PlatformRoute.OC1, playerName);
            string playerPuuid = summoner.Puuid;
            string[] matchesArr = await riotApi.MatchV5().GetMatchIdsByPUUIDAsync(RegionalRoute.SEA ,playerPuuid, 10, null, Queue.SUMMONERS_RIFT_5V5_DRAFT_PICK);

            var embed = new EmbedBuilder().WithThumbnailUrl($"http://ddragon.leagueoflegends.com/cdn/13.19.1/img/profileicon/{summoner.ProfileIconId}.png");
            embed.Title = $"{playerName}'s Last 10 matches";

            // Collecting data takes longer than 3 seconds which is the timeout for responding to an interaction so we'll cheat a little bit.
            var timer = new Stopwatch();
            timer.Start();
            await RespondAsync("Collecting data...");
            // Grab that ^ message id.
            var msg = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
            var msgID = msg.First().Id;

            foreach (var match in matchesArr)
            {
                var matchesInfo = await riotApi.MatchV5().GetMatchAsync(RegionalRoute.SEA, match);
                var getParticipants = matchesInfo.Info.Participants;

                // What do we want to know?
                int kills = 0;
                int deaths = 0;
                string position = string.Empty;
                int totalDamageDelt = 0;
                int visionScore = 0;
                string champPlayed = string.Empty;
                double? kda = 0.0;
                double? gpm = 0.0;
                double? gamelength = 0;
                bool win = false;
                // Grab the games players
                foreach (var participant in getParticipants)
                {
                    // Find the player we're searching for and grab some stats
                    if (participant.Puuid == playerPuuid)
                    {
                        win = participant.Win;
                        kills = participant.Kills;
                        deaths = participant.Deaths;
                        position = participant.TeamPosition;
                        totalDamageDelt = participant.TotalDamageDealt;
                        visionScore = participant.VisionScore;
                        champPlayed = participant.ChampionName;
                        kda = participant.Challenges.Kda;
                        gpm = participant.Challenges.GoldPerMinute;
                        gamelength = participant.Challenges.GameLength;
                    }
                }
                var didWin = win ? "Victory" : "Defeat";

                embed.AddField($"{match}", $":trophy: Outcome: {didWin}\n:knife: Kills: {kills}\n:skull: Deaths: {deaths}\n:radioactive: Lane: {position}\n:crossed_swords: Total Damage Delt: " +
                    $"{totalDamageDelt}\n:eyes: Vision Score: {visionScore}\n:frame_photo: Champion Played: {champPlayed}\n:bar_chart: KDA: {Math.Round((double)kda)}\n" +
                    $":coin: GPM: {Math.Round((double)gpm)} per min\n:timer: Game Length: {Math.Round((decimal)(gamelength / 60))} Mins", true);
            }
            // Use the Id we grabbed earlier to modify the original message so we dont spam.
            await Context.Channel.ModifyMessageAsync(msgID, x =>
            {
                x.Embed = embed.Build();
                timer.Stop(); // Do some benchmarking.
                x.Content = $"Response took {timer.Elapsed.TotalSeconds.ToString("0.00")} Seconds.";
            });
        }
    }
}
