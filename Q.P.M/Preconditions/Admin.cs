using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Q.P.M.Preconditions
{
    class Admin : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (!(context.User as IGuildUser).GuildPermissions.Administrator) {
                return Task.FromResult(PreconditionResult.FromError(":x: You Don't have permission to use that command..."));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
