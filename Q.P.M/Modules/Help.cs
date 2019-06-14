// System
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

// Discord
using Discord;
using Discord.Commands;

namespace Q.P.M.Modules
{
    public class Help : ModuleBase
    {
        private readonly CommandService commands;

        public Help(CommandService service)
        {
            this.commands = service;
        }

        [Command("help")]
        [Alias("h")]
        [Summary("Lists all the commands")]
        public async Task HelpAsync()
        {
            var prefix = Config.Bot.PrefixDictionary[this.Context.Guild.Id];
            Random rand = new Random();
            var builder = new EmbedBuilder();
            {
                builder.WithColor(new Color(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256)));
                builder.WithTimestamp(DateTimeOffset.UtcNow.UtcDateTime);
                builder.WithTitle($"Hey {this.Context.User.Username}, here is a list of all my commands.");
                builder.WithFooter(f => { f.WithText("Do `help [command]` for more information"); });
            }

            foreach (var module in this.commands.Modules.OrderBy(x => x.Commands.Count))
            {
                string description = null;
                foreach (var cmd in module.Commands.OrderBy(x => x.Name))
                {
                    var result = await cmd.CheckPreconditionsAsync(this.Context);
                    if (result.IsSuccess)
                    {
                        description += $"{prefix}{cmd.Aliases.First()}\n";
                    }
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    description += $"\n";
                    builder.AddField(
                        x =>
                        {
                            x.Name = $"__{module.Name}__";
                            x.Value = $"```fix\n{description}```";
                            x.IsInline = true;
                        });
                }
            }

            await this.ReplyAsync(string.Empty, false, builder.Build());
        }
    }
}
