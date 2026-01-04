using System;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Microsoft.Extensions.Configuration;

namespace Calcifer.Services;

public class InteractionHandler(DiscordSocketClient client, InteractionService handler, IServiceProvider services, IConfiguration config)
{
    private readonly DiscordSocketClient _client = client;
    private readonly InteractionService _handler = handler;
    private readonly IServiceProvider _services = services;
    private readonly IConfiguration _configuration = config;

    public async Task InitalizeAsync()
    {
        // Subscribe to the Ready event so we know when it's ok to register our commands.
        _client.Ready += ReadyAsync;
        _handler.Log += LogAsync;

        // Add the public modules that intherit InteractionModuleBase<T> to the InteractionService.
        await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        // Process the InteractionCreated payloads to execute Interactions commands.
        _client.InteractionCreated += HandleInteraction;

        // Also process the result of the command execution.
        _handler.InteractionExecuted += HandleInteractionExecute;
    }

    private Task LogAsync(Discord.LogMessage msg)
    {
        Console.WriteLine(msg);
        return Task.CompletedTask;
    }


    private async Task ReadyAsync()
    {
        // Register the commands globally. Commands can be registerd to a spesific guild with _handler.RegisterCommandGloballyAsync().
        await _handler.RegisterCommandsGloballyAsync();
    }


    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
            var context = new SocketInteractionContext(_client, interaction);

            // Execute the incoming command.
            var result = await _handler.ExecuteCommandAsync(context, _services);

            // Due to async nature of InteractionFramework, the result here may always be success.
            // That's why we also need to handle the InteractionExecuted event.
            if (!result.IsSuccess)
            {
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    default:
                        break;
                }
            }
        }
        catch
        {
            // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (interaction.Type is InteractionType.ApplicationCommand)
            {
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }


    private Task HandleInteractionExecute(ICommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        if (!result.IsSuccess)
        {
            switch (result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    // TODO:
                    break;

                default:
                    break;
            }
        }

        return Task.CompletedTask;
    }
}
