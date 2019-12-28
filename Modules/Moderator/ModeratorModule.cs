using Booper.Common;
using Booper.Preconditions;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
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

        
        [Command("Purge")]
        [Summary("Deletes the specified amout of messages from the channel. (Limit 100)")]
        [Remarks("~Purge <amount>")]
        [Priority(20)]
        public async Task DeleteMessagesAsync([Remainder]int amount = 1)
        {
            var toDelete = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();
            try
            {
                if (amount <= 100)
                {
                    await (Context.Channel as ITextChannel).DeleteMessagesAsync(toDelete);
                    var m = await ReplyAsync($"Removed **{amount}** message(s) from this channel.\nThis message will be removed in 5 seconds."); // Pop a message and store its ID so we can target it for removal.
                    await Task.Delay(5000); // Wait 5 seconds
                    await (Context.Channel as ITextChannel).DeleteMessageAsync(m); // Remove the generated message.
                }
                else if (amount > 100)
                    await ReplyAsync("Can't delete more than 100 messages or messages that are over 14 days old."); // Discord doesn't like removing more than 100 messages at a time via the API
            }
            catch (ArgumentOutOfRangeException)
            {
                await ReplyAsync($"Some or all of the messages were older than 14 days. These can not be bulk deleted."); // Discord doesn't like us removing messages over 14 days old.
                throw;
            }
        }
        

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
            await ReplyAsync(":ok_hand:");
        }
    }
}
