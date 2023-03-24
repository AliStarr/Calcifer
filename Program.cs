using Calcifer.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Text;
using System.Linq;

namespace Calcifer
{
    public class Program
    {

        // There is no need to implement IDisposable like before as we are
        // using dependency injection, which handles calling Dispose for us.
        static void Main(string[] args)
            => new Program().RunAsync().GetAwaiter().GetResult();

        public async Task RunAsync()
        {
            // You should dispose a service provider created using ASP.NET
            // when you are finished using it, at the end of your app's lifetime.
            // If you use another dependency injection framework, you should inspect
            // its documentation for the best way to do this.
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                // Strat logging
                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                // Tokens should be considered secret data and never hard-coded.
                // We can read from the environment variable to avoid hard coding.
                string token = ConfigurationManager.AppSettings["discord"];
                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();

                // Here we init the logic required to register our commands
                await services.GetRequiredService<CommandHandlingService>().InitalizeAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }

        

        static void WriteToFile(string content)
        {
            string fileName = "log.txt";
            content += Environment.NewLine;
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            using (FileStream fs = new(fileName, FileMode.Append))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        static Task LogAsync(LogMessage log)
        {
            switch (log.Severity)
            {
                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.WriteLine("Critical: " + log.ToString());
                    WriteToFile("Critical: " + log.ToString());

                    return Task.CompletedTask;

                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: " + log.ToString());
                    WriteToFile("Error: " + log.ToString());

                    return Task.CompletedTask;

                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Warning: " + log.ToString());
                    WriteToFile("Warning: " + log.ToString());

                    return Task.CompletedTask;

                case LogSeverity.Info:

                    Console.WriteLine("Info: " + log.ToString());
                    WriteToFile("Info: " + log.ToString());
                    return Task.CompletedTask;

                case LogSeverity.Verbose:

                    Console.WriteLine("Verbose: " + log.ToString());
                    WriteToFile("Verbose: " + log.ToString());

                    return Task.CompletedTask;

                case LogSeverity.Debug:

                    Console.WriteLine("Debug: " + log.ToString());
                     WriteToFile("Debug: " + log.ToString());

                    return Task.CompletedTask;
                default:

                    Console.WriteLine(log.ToString());
                    WriteToFile("Unknown: " + log.ToString());

                    return Task.CompletedTask;
            }
        }

        static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                  .AddSingleton<DiscordSocketClient>()
                  .AddSingleton<CommandService>()
                  .AddSingleton<CommandHandlingService>()
                  .BuildServiceProvider();
        }
    }
}
