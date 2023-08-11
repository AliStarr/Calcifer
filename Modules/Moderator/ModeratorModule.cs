using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Calcifer.Modules.Moderator
{
    public class ModeratorModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("kick", "Kick the specified user.")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser user)
        {
            await RespondAsync($"Kicking {user.Mention}.");
            await user.KickAsync();
        }

        [SlashCommand("ban", "Ban the specified user.")]
        public async Task Ban(SocketGuildUser user)
        {
            if (user.Id == Context.User.Id)
            {
                await RespondAsync("You can't ban your self.");
            }
            else
            {
                await user.Guild.AddBanAsync(user);
                await RespondAsync($"Banned {user.Mention}.");
            }
        }

        [SlashCommand("unban", "Unbans the spesified user.")]
        public async Task UnBan(SocketGuildUser user)
        {
            await Context.Guild.RemoveBanAsync(user);
            await RespondAsync($"{user.Mention} Has been unbanned.");
        }

        [SlashCommand("purge", "Delete an amount of messages from the channel. THIS MAY NOT WORK")]
        public async Task DeleteMessages(int amount = 1)
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
                    await RespondAsync("Can't delete more than 100 messages or messages that are over 14 days old."); // Discord doesn't like removing more than 100 messages at a time via the API
            }
            catch (ArgumentOutOfRangeException)
            {
                await RespondAsync($"Some or all of the messages were older than 14 days. These can not be bulk deleted."); // Discord doesn't like us removing messages over 14 days old.
                throw;
            }
        }

        [SlashCommand("topic", "Changes the channels topic.")]
        public async Task ChangeTopic(string input)
        {
            var channel = (ITextChannel)Context.Channel;
            // Totally stolen from https://github.com/Lomztein/Adminthulhu/blob/57cdd792106d98597761471e192e6bda4a053974/AdminthulhuCore/AutomatedTextChannels.cs
            await channel.ModifyAsync(delegate (TextChannelProperties properties)
            {
                (properties).Topic = input;
            });
            await RespondAsync(":ok_hand:");
        }
    }
}
