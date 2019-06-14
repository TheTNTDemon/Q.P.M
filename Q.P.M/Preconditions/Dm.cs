using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Q.P.M.Preconditions
{
    public class Dm : PreconditionAttribute
    {
        public bool Accesibility { get; }

        public Dm(bool Accesibility)
        {
            this.Accesibility = Accesibility;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if ((context as SocketCommandContext).IsPrivate)
            {
                if (Accesibility)
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }
                else
                {
                    return Task.FromResult(PreconditionResult.FromError("this was tried in a dm but isn't allowed"));
                }
            }
            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
