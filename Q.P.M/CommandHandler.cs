// System
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

// Helpers

// Discord
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;

// Microsoft
using Microsoft.Extensions.DependencyInjection;
using Q.P.M.Modules.Inventory;
using Q.P.M.Modules.Server_Chatting_Testing;

namespace Q.P.M
{
    public class CommandHandler
    {
        /// <summary> The <see cref="DiscordSocketClient"/> </summary>
        private DiscordSocketClient client;

        /// <summary> The <see cref="CommandService"/> </summary>
        private CommandService service;

        /// <summary> The <see cref="IServiceProvider"/> </summary>
        private IServiceProvider services;

        /// <summary> Initialize the bot </summary>
        /// <param name="currentClient"> The used <see cref="DiscordSocketClient"/> </param>
        /// <param name="config"> The used <see cref="DiscordSocketConfig"/> </param>
        /// <returns> bot initialization </returns>
        public async Task InitializeAsync(DiscordSocketClient currentClient, DiscordSocketConfig config)
        {
            this.client = currentClient;

            this.services = new ServiceCollection()
               .AddSingleton(this.client)
               .AddSingleton<InteractiveService>()
               .BuildServiceProvider();

            service = new CommandService();
            await service.AddModulesAsync(Assembly.GetExecutingAssembly(), services);

            // Real Handling event
            this.client.MessageReceived += this.HandleMessageAsync;
            client.ReactionAdded += InventoryEmojis;
        }

        private async Task InventoryEmojis(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            await Example.HandleReactionAdded(arg1, arg3);
            await InventoryShowing.HandleReactionAdded(arg1, arg3);
        }

        /// <summary> Handle a message, leveling, commands. </summary>
        /// <param name="s"> The socket message to make the context </param>
        /// <returns> Handle message </returns>
        private async Task HandleMessageAsync(SocketMessage s)
        {
            //await MessageDM(s);
            await ServerChatting.Message(s);

            var msg = (SocketUserMessage)s;
            var context = new SocketCommandContext(this.client, msg);
            if (!context.IsPrivate)
            {
                if (context.User.IsBot)
                {
                    return;
                }

                if (!Config.Bot.PrefixDictionary.ContainsKey(context.Guild.Id))
                {
                    Config.Bot.PrefixDictionary.Add(context.Guild.Id, "~");
                    Config.Save();
                }
            }

            // Executing commands
            var argPos = 0;
            var currentPrefix = Config.Bot.PrefixDictionary[context.Guild.Id];
            if (msg.Content.StartsWith($"{currentPrefix}{currentPrefix}"))
            {
                return;
            }

            if ((msg.HasStringPrefix(currentPrefix, ref argPos) ||
                msg.HasStringPrefix(currentPrefix.ToUpper(), ref argPos) ||
                msg.HasStringPrefix(currentPrefix.ToLowerInvariant(), ref argPos) ||
                msg.HasStringPrefix(currentPrefix.First().ToString().ToUpper() + currentPrefix.Substring(1), ref argPos))
                || msg.HasMentionPrefix(this.client.CurrentUser, ref argPos))
            {
                var result = await this.service.ExecuteAsync(context, argPos, this.services, MultiMatchHandling.Best);
                if (!result.IsSuccess)
                {
                    if (result.Error == CommandError.UnknownCommand)
                    {
                        var guildEmote = Emote.Parse("<:unknowscmd:461157571701506049>");
                        await msg.AddReactionAsync(guildEmote);
                    }
                    if (result.Error != CommandError.UnknownCommand)
                    {
                        Console.WriteLine(result.ErrorReason);
                    }
                }
            }
        }

        private async Task MessageDM(SocketMessage s)
        {
            var msg = (SocketUserMessage)s;
            var context = new SocketCommandContext(this.client, msg);
            if (context.IsPrivate)
            {
                if (context.User.IsBot)
                {
                    return;
                }
            }

            // Executing commands
            var argPos = 0;
            var currentPrefix = "~";
            if (msg.Content.StartsWith($"{currentPrefix}{currentPrefix}"))
            {
                return;
            }

            if ((msg.HasStringPrefix(currentPrefix, ref argPos) ||
                msg.HasStringPrefix(currentPrefix.ToUpper(), ref argPos) ||
                msg.HasStringPrefix(currentPrefix.ToLowerInvariant(), ref argPos) ||
                msg.HasStringPrefix(currentPrefix.First().ToString().ToUpper() + currentPrefix.Substring(1), ref argPos))
                || msg.HasMentionPrefix(this.client.CurrentUser, ref argPos))
            {
                var result = await this.service.ExecuteAsync(context, argPos, this.services, MultiMatchHandling.Best);
                if (!result.IsSuccess)
                {
                    if (result.Error == CommandError.UnknownCommand)
                    {
                        var guildEmote = Emote.Parse("<:unknowscmd:461157571701506049>");
                        await msg.AddReactionAsync(guildEmote);
                    }
                    if (result.Error != CommandError.UnknownCommand)
                    {
                        Console.WriteLine(result.ErrorReason);
                    }
                }
            }
        }
    }
}
