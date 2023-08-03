using Calcifer.Services;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("_config.json", false);
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<DiscordSocketClient>();       // Add discord client to services
        services.AddSingleton<InteractionService>();        // Add the interaction service to services
        services.AddHostedService<InteractionHandlingService>(); // Add the slashcommand handler
        services.AddHostedService<DiscordStartupService>();     // Add the discord startup service
    })
    .Build();

await host.RunAsync();
