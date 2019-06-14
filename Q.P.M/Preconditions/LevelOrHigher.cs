using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Q.P.M.Core.UserAccounts;

namespace Q.P.M.Preconditions
{
    public class LevelOrHigher : PreconditionAttribute
    {
        public int? LevelNeeded { get; }

        public LevelOrHigher(int LevelSpecified)
        {
            LevelNeeded = LevelSpecified;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var guildUser = context.User as IGuildUser;
            var account = UserAccounts.GetAccount(guildUser as SocketGuildUser);

            if (LevelNeeded.HasValue)
            {
                if (account.CurrentCharacter.Level < LevelNeeded)
                {
                    context.Channel.SendMessageAsync($"You must be level {LevelNeeded} so {LevelNeeded - account.CurrentCharacter.Level} left to go to use this command!");
                    return Task.FromResult(PreconditionResult.FromError("An user tried to do a command but they haven't reached that level yet."));
                }
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
