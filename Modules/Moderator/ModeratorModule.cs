using Booper.Common;
using Booper.Preconditions;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using System.Threading.Tasks;

namespace Booper.Modules.Moderator
{
    [MinPermissions(AccessLevel.ServerMod)]
    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(GuildPermission.BanMembers)]
    [RequireBotPermission(GuildPermission.KickMembers)]
    public class ModeratorModule : ModuleBase<SocketCommandContext>
    {
        [Command("Kick")]
        [Summary("Kicks a @mentioned user with an optional reason.")]
        [Remarks("~Kick <@username#1234> [reason]")]
        [Priority(10)]
        public async Task KickAsync(IGuildUser user, [Remainder] string reason = null)
        {
            if (user.Id == Context.User.Id)
                await ReplyAsync("You can't kick your self.");
            else
            {
                await user.KickAsync(reason: reason);
                await ReplyAsync($"{Context.User.Mention} kicked {user.Mention} for reason {reason}");
            }
        }

        [Command("Ban")]
        [Summary("Bans a @mentioned user with an optional reason.")]
        [Remarks("~Ban <@username#1234> [reason]")]
        [Priority(10)]
        public async Task BanAsync(IGuildUser user, [Remainder] string reason = null)
        {
            if (user.Id == Context.User.Id)
                await ReplyAsync("You can't ban your self.");
            else
            {
                await user.Guild.AddBanAsync(user, reason: reason);
                await ReplyAsync($"{Context.User.Mention} Banned {user.Mention}.");
            }
        }

        [Command("UnBan")]
        [Alias("Forgive")]
        [Summary("UnBans a previously banned user.")]
        [Remarks("~UnBan <@username#1234>")]
        [Priority(10)]
        public async Task UnBanAsync(SocketGuildUser user)
        {
            await Context.Guild.RemoveBanAsync(user);
            await ReplyAsync($"{Context.User.Mention} unbanned {user.Mention}.");
        }

        /*
        [Command("Purge")]
        [Summary("Deletes the specified amout of messages from the channel. (Limit 100)")]
        [Remarks("~Purge <amount>")]
        [Priority(20)]
        public async Task DeleteMessagesAsync(int amount)
        {
            var toDelete = Context.Channel.GetMessagesAsync(amount + 1).Flatten();
            // currently I do not want to check if the messages are over 14 days old
            //var inAge = toDelete.Where(x => (x.CreatedAt - DateTime.Now).TotalDays < 15).Take(amount);
            var channel = (ITextChannel)Context.Channel;

            try
            {
                if (amount <= 100)
                {
                    channel.DeleteMessagesAsync(toDelete);
                    await ReplyAsync($"Removed {amount} message(s) from this channel :wastebasket:\nThis message will be remmoved in 10 seconds.");
                    await Task.Delay(10000); // Wait 10 seconds
                    var orig = Context.Channel.GetMessagesAsync(1).Flatten();
                    await channel.DeleteMessagesAsync(orig); // Remove the generated message
                }
                else if (amount > 100)
                    foreach (IMessage msg in toDelete)
                        await msg.DeleteAsync(); // could be rate limited so might be slow.
            }
            catch (ArgumentOutOfRangeException)
            {
                await ReplyAsync($"Some or all of the messages were older than 14 days. These can not be bulk deleted.");
                throw;
            }
        }
        */

        [Command("Topic")]
        [Summary("Changes the topic of the current channel.")]
        [Remarks("~topic <Text to input into the topic>")]
        public async Task ChangeTopicAsnyc([Remainder]string input)
        {
            var channel = (ITextChannel)Context.Channel;
            // Totally stolen from https://github.com/Lomztein/Adminthulhu/blob/57cdd792106d98597761471e192e6bda4a053974/AdminthulhuCore/AutomatedTextChannels.cs
            await channel.ModifyAsync(delegate (TextChannelProperties properties)
            {
                (properties).Topic = input;
            });
        }
    }
}
