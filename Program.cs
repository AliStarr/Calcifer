using Calcifer.Common;
using Calcifer.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace Calcifer
{
    public class Program
    {
        static string assemLoc = Assembly.GetExecutingAssembly().Location;
        public static string version = FileVersionInfo.GetVersionInfo(assemLoc).FileVersion;

        static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            // Get version number
            

            Console.Title = $"Calcifer version {version}";

            string discordToken = ConfigurationManager.AppSettings["discord"];  // Grab token
            using var services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();

            client.Log += LogAsync; // Start logging
            services.GetRequiredService<CommandService>().Log += LogAsync;

            await client.LoginAsync(TokenType.Bot, discordToken);   // Login

            await client.StartAsync();
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            await client.SetGameAsync($"Version: {version}");

            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices() => new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
    }
}
