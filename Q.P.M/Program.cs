// System
using System;
using System.Threading.Tasks;

// Discord
using Discord;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;

namespace Q.P.M
{
    internal class Program
    {
        /// <summary> the made by text </summary>
        private const string MadeBy = "\n ///////////////////////////////////////////////\n"
                                      + " //                                           //\n"
                                      + " //            Coded and made by:             //\n"
                                      + " //           Cute Little Dinosaur!           //\n"
                                      + " //    If you need any help please contact    //\n"
                                      + " //    Cute Little DinoSaur#911 on Discord!   //\n"
                                      + " //                                           //\n"
                                      + " ///////////////////////////////////////////////\n\n";

        /// <summary> The bot's name </summary>
        private const string BotName = "Q.P.M";

        /// <summary> the <see cref="DiscordSocketClient"/> client </summary>
        private DiscordSocketClient client;

        /// <summary> The command handler <see cref="CommandHandler"/> </summary>
        private CommandHandler handler;

        /// <summary> The <see cref="DiscordSocketConfig"/> </summary>
        private DiscordSocketConfig config;

        /// <summary> Main void, called on startup, goes to <see cref="InitializeAsync"/> to start the bot </summary>
        private static void Main()
            => new Program().InitializeAsync().GetAwaiter().GetResult();

        /// <summary> Start the bot </summary>
        /// <returns> returns nothing </returns>
        private async Task InitializeAsync()
        {
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Title = BotName;
                Console.WriteLine(MadeBy);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" Starting...\n Loading...\n");
            }

            if (string.IsNullOrEmpty(Config.Bot.Token))
            {
                Console.WriteLine("No token detected");
                return;
            }

            this.client = new DiscordSocketClient(
                new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    WebSocketProvider = WS4NetProvider.Instance,
                    MessageCacheSize = 50000
                });

            this.config = new DiscordSocketConfig
            {
                MessageCacheSize = 50000
            };

            {
                // Events
                this.client.Log += this.Log;
                this.client.Ready += this.ClientReady;

                await this.client.LoginAsync(TokenType.Bot, Config.Bot.Token);
                await this.client.StartAsync();
                await this.client.SetStatusAsync(UserStatus.Online);
                await this.client.SetActivityAsync(new Game($"@Q.P.M help", ActivityType.Watching));
            }

            {
                this.handler = new CommandHandler();
                await this.handler.InitializeAsync(this.client, this.config);
            }

            await this.ConsoleRead();

            await Task.Delay(-1);
        }

        /// <summary> Read the console </summary>
        /// <returns> returns nothing </returns>
        private async Task ConsoleRead()
        {
            while (true)
            {
                var input = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Green;
                if (input == null)
                {
                    continue;
                }

                switch (input.ToLower())
                {
                    default:
                        Console.WriteLine("The CmdList:\nStop: end the console commands.\n");
                        break;
                    case "cancel":
                    case "stop":
                    case "end":
                        Console.WriteLine("Console commands have been disabled!");
                        return;
                }

                await Task.CompletedTask;
            }
        }

        /// <summary> Log discord information messages </summary>
        /// <param name="msg"> the msg from Discord to write in the console </param>
        /// <returns> returns nothing </returns>
        private async Task Log(LogMessage msg)
        {
            Console.WriteLine(DateTimeOffset.UtcNow.UtcDateTime + "  " + msg.Message);
            await Task.CompletedTask;
        }

        /// <summary> The client_ ready event </summary>
        /// <returns> The <see cref="Task"/> </returns>
        private Task ClientReady()
        {
            Global.Client = this.client;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("I am in " + this.client.Guilds.Count + " guilds.");
            Console.ForegroundColor = ConsoleColor.White;
            return Task.CompletedTask;
        }
    }
}
