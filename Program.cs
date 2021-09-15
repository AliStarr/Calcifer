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
        static readonly string assemLoc = Assembly.GetExecutingAssembly().Location;
        private static string version = FileVersionInfo.GetVersionInfo(assemLoc).FileVersion;

        public static string Version { get => version; set => version = value; }

        static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            // Get version number
            

            Console.Title = $"Calcifer version {Version}";

            string discordToken = ConfigurationManager.AppSettings["discord"];  // Grab token
            using var services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();

            client.Log += LogAsync; // Start logging
            services.GetRequiredService<CommandService>().Log += LogAsync;

            await client.LoginAsync(TokenType.Bot, discordToken);   // Login

            await client.StartAsync();
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            await client.SetGameAsync($"Version: {Version}");

            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            switch (log.Severity)
            {
                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.WriteLine("Critical: " + log.ToString());
                    return Task.CompletedTask;

                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: " + log.ToString());
                    return Task.CompletedTask;

                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Warning: " + log.ToString());
                    return Task.CompletedTask;

                case LogSeverity.Info:

                    Console.WriteLine("Info: " + log.ToString());
                    return Task.CompletedTask;

                case LogSeverity.Verbose:

                    Console.WriteLine("Verbose: " + log.ToString());
                    return Task.CompletedTask;

                case LogSeverity.Debug:

                    Console.WriteLine("Debug: " + log.ToString());
                    return Task.CompletedTask;
                default:

                    Console.WriteLine(log.ToString());
                    return Task.CompletedTask;
            }

            
        }

        private ServiceProvider ConfigureServices() => new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
    }
}
