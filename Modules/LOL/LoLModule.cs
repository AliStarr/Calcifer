using Discord.Interactions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camille.Enums;
using Camille.RiotGames;
using Microsoft.Extensions.Configuration;
using Discord;


namespace Calcifer.Modules
{
    public class LoLModule : InteractionModuleBase<SocketInteractionContext>
    {
        // Grabbing the API key
        private readonly IConfiguration _config;
        public LoLModule(IConfiguration config)
        {
            _config = config;
        }

        [SlashCommand("top10", "Get a players top 10 champions.")]
        public async Task TopChamps(string playerName)
        {
            // get API key from our fancy config manager
            var riotApi = RiotGamesApi.NewInstance(_config["RiotApiKey"]); // TODO: Maybe move this out into its own thing so it's not called everytime we want to call the api?

            var summoner = await riotApi.SummonerV4().GetBySummonerNameAsync(PlatformRoute.OC1, playerName);
            var masteries = await riotApi.ChampionMasteryV4().GetAllChampionMasteriesAsync(PlatformRoute.OC1, summoner.Id);
            // Store results in key value pairs
            Dictionary<string, int> results = new();
            var embed = new EmbedBuilder();
            embed.Title = $"{playerName}'s Top 10 champions by mastery level";
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
    }
}
