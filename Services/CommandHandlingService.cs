using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Calcifer.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            _commands.CommandExecuted += CommandExecutedAsync;
            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync() 
            => await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore non-user messages 
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return; // Bot can't me made to run commands from its self

            var argPos = 0;
            if (!(message.HasMentionPrefix(_discord.CurrentUser, ref argPos) || message.HasStringPrefix("~", ref argPos))) return;


            var context = new SocketCommandContext(_discord, message);
            await _commands.ExecuteAsync(context, argPos, _services); // handle result in CommandExecutedAsync
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "IDC")]
        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found) we dont care about these errors
            if (!command.IsSpecified)
                return;

            // the command was successful, we dont care about this result unless we want to log that a command secceeded
            if (result.IsSuccess)
                return;

            if (!result.IsSuccess)
            {
                if (result.Error == CommandError.ParseFailed)
                {
                    _ = await context.Channel.SendMessageAsync($":x: Error: {result.ErrorReason} This means that you tried to enter something that the bot didnt understand.\n" +
                        $"The command **{command.Value.Name}** is used like this: _{command.Value.Remarks}_\n" +
                        $"Please use the **~reportbug** command to notify Alister if this keeps happening!");
                }

                if (result.Error == CommandError.BadArgCount)
                {
                    await context.Channel.SendMessageAsync($":x: Error: {result.ErrorReason} This means that you didnt supply enough arguments to make the command work.\n" +
                        $"The command **{command.Value.Name}** is used like this: _{command.Value.Remarks}_\n" +
                        $"Please use the **~reportbug** command to notify Alister if this keeps happening!");
                }

                if (result.Error == CommandError.UnmetPrecondition)
                {
                    await context.Channel.SendMessageAsync($":x: Error: {result.ErrorReason} This means you don't have permission to run this command.\n" +
                        $"Please use the **~reportbug** command to notify Alister if you should have permission!");
                }
                if (result.Error == CommandError.Exception)
                {
                    await context.Channel.SendMessageAsync($":x: Error: {result.ToString()}\n" +
                        $"Oh shit something broke\n" +
                        $"please use the ~reportbug command to notify Alister!");
                }

                if (result.Error == CommandError.UnknownCommand)
                {
                    return; // Don't do anything. We dont care if you tried to get a command that doesn't exist. Prevents spam.
                }

                else
                {
                    // something happened that wasnt covered
                    await context.Channel.SendMessageAsync($":x: Error: {result.ToString()} \n please use the ~reportbug command to notify Alister if this keeps happening!");
                }

               
            }
            

        }
    }
}
