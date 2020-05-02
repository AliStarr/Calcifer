using Calcifer.Common;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Calcifer.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MinPermissionsAttribute : PreconditionAttribute
    {
        private readonly AccessLevel _level;

        public MinPermissionsAttribute(AccessLevel Level) => _level = Level;

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            var access = GetPermission(context);

            if (access >= _level)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
            {
                return Task.FromResult(PreconditionResult.FromError("Insuffcient permissions."));
            }
        }

        private AccessLevel GetPermission(ICommandContext c)
        {
            if (c.User.IsBot)
                return AccessLevel.Blocked;
            if (c.User.Id == c.Guild.OwnerId) // Server owner is usally me
                return AccessLevel.BotOwner;

            var user = c.User as SocketGuildUser;

            if (user != null)
            {
                if (c.Guild.OwnerId == user.Id)
                    return AccessLevel.ServerOwner;

                if (user.GuildPermissions.Administrator)
                    return AccessLevel.ServerAdmin;

                if (user.GuildPermissions.ManageMessages ||
                    user.GuildPermissions.BanMembers ||
                    user.GuildPermissions.KickMembers)
                    return AccessLevel.ServerMod;
            }
            return AccessLevel.User;
        }
    }

}
