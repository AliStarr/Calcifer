using Booper.Common;
using Discord;
using Discord.Commands;

using System;
using System.Threading.Tasks;

namespace Booper.Modules.Games
{
    public class GamesModule : ModuleBase<SocketCommandContext>
    {
        [Command("Roll")]
        [Summary("Rolls a die with the specified number of sides. If no number is given then the die rolls with 6 sides as the default.")]
        [Remarks("~Roll <sides of the die>")]
        public async Task RollDice(int sides = 6)
        {
            if (sides < 2)
                await ReplyAsync("Die must have 2 or more sides.");

            else
            {
                int result;
                Random rnd = new Random();
                result = rnd.Next(1, sides + 1); // the maxValue of Random() is exclusive, so it will never return the max value.
                if (result == sides)
                {
                    var critEmbed = new EmbedBuilder()
                        .WithTitle("Dice Roll")
                        .WithDescription($"{Context.User.Mention} rolled a {sides} sided die and got **{result}!** Nice!")
                        .WithColor(new Color(217, 13, 232));
                    await ReplyAsync("", embed: critEmbed.Build());
                }
                else if (result == 1) // lower limit
                {
                    var failEmbed = new EmbedBuilder()
                        .WithTitle("Dice Roll")
                        .WithDescription($"{Context.User.Mention} rolled a {sides} sided die and got **{result}!** Sucks to be you!")
                        .WithColor(new Color(217, 13, 232));
                    await ReplyAsync("", embed: failEmbed.Build());
                }
                else
                {
                    var embed = new EmbedBuilder()
                        .WithTitle("Dice Roll")
                        .WithDescription($"{Context.User.Mention} rolled a {sides} sided die and got **{result}**.")
                        .WithColor(new Color(217, 13, 232));
                     await ReplyAsync("", embed: embed.Build());
                }
            }
        }

        [Command("8Ball")]
        [Summary("Ask the magic 8 ball a question and get an answer, make bad decisions then blame the 8ball.")]
        [Remarks("~8Ball <question>")]
        public async Task EightBallAsync([Remainder] string input)
        {
            int result;
            Random rnd = new Random();
            result = rnd.Next(0, CommonStrings.answers.Length + 1);   // The maxValue of Random() is exclusive, so it will never return the max value
                                                                      // and I cant be fucked looking at how many items are in the array.
            var embed = new EmbedBuilder()
            {
                Title = "Magic 8Ball",
                Description = $"**Question:** {input}\n**Answer:** {CommonStrings.answers[result]}!",
                Color = new Color(240, 85, 185),
            };
            await ReplyAsync("", embed: embed.Build());
        }

        [Command("Flip")]
        [Summary("Flip a coin. Heads or Tails.")]
        [Remarks("~flip")]
        public async Task FlipAsync()
        {
            int result;
            Random rnd = new Random();
            result = rnd.Next(2);
            if (result == 0)
               await ReplyAsync("Heads");
            else if (result == 1)
               await ReplyAsync("Tails");
        }
    }
}
