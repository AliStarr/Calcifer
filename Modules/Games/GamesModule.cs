using Discord;
using Discord.Interactions;
using System;
using System.Threading.Tasks;

namespace Calcifer.Modules.Games
{
    public class GamesModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("roll", "Rolls a die with the specified number of sides. Default is 6 sides.")]
        public async Task RollDice(int sides = 6)
        {
            if (sides < 2)
                await RespondAsync("Die must have 2 or more sides.");
            else
            {
                int result;
                Random rnd = new();
                result = rnd.Next(1, sides + 1); // the maxValue of Random() is exclusive, so it will never return the max value.
                if (result == sides)
                {
                    var critEmbed = new EmbedBuilder()
                        .WithTitle("Dice Roll")
                        .WithDescription($"{Context.User.Mention} rolled a {sides} sided die and got **{result}!** Nice!")
                        .WithColor(new Color(217, 13, 232));
                    await RespondAsync("", embed: critEmbed.Build());
                }
                else if (result == 1) // lower limit
                {
                    var failEmbed = new EmbedBuilder()
                        .WithTitle("Dice Roll")
                        .WithDescription($"{Context.User.Mention} rolled a {sides} sided die and got **{result}!** Sucks to be you!")
                        .WithColor(new Color(217, 13, 232));
                    await RespondAsync("", embed: failEmbed.Build());
                }
                else
                {
                    var embed = new EmbedBuilder()
                        .WithTitle("Dice Roll")
                        .WithDescription($"{Context.User.Mention} rolled a {sides} sided die and got **{result}**.")
                        .WithColor(new Color(217, 13, 232));
                    await RespondAsync("", embed: embed.Build());
                }
            }
        }

        
        [SlashCommand("8ball", "Ask the magic 8 ball a question and get an answer, make bad decisions then blame the 8ball.")]
        public async Task EightBall(string input)
        {
            string[] answers = { "It is certain.", "It is decidedly so.", "Without a doubt", "Yes definitely.", "You may rely on it.", "As I see it, yes.", "Most Likely", "Outlook good.", "Yes.", "Signs point to yes.",
                                "Reply hazy, try again", "Ask again later.", "Better not tell you now.", "Cannot predict now.", "Concentrate and ask again.",
                                "Don't count on it.", "My reply is no.", "My sources say no.", "Outlook is not so good.", "Very Doubtful." };
            int result;
            Random rnd = new();
            result = rnd.Next(0, answers.Length + 1);   // I can't be fucked looking at how many items are in the array.

            var embed = new EmbedBuilder()
            {
                Title = "Magic 8Ball",
                Description = $"**Question:** {input}\n**Answer:** {answers[result]}",
                Color = new Color(240, 85, 185),
            };
            await RespondAsync("", embed: embed.Build());
        }

        [SlashCommand("flip", "Flip a coin.")]
        public async Task Flip()
        {
            int result;
            Random rnd = new();
            result = rnd.Next(2);
            if (result == 0)
            {
                await RespondAsync("Heads");
            }
            else if (result == 1)
            {
                await RespondAsync("Tails");
            }
                
        }
    }
}
