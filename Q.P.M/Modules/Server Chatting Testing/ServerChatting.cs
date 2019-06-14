using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q.P.M.Modules.Server_Chatting_Testing
{
    public class ServerChatting : ModuleBase<SocketCommandContext>
    {
        [Command("chat")]
        public void ChattingTogetherTest(ulong ServerId, ulong chatId)
        {
            var servers = ServerChattingWs.workStation;

            ServerAndChat SC = new ServerAndChat();
            ServerAndChat SC2 = new ServerAndChat();

            servers.ServerId = ServerId;
            servers.ChannelId = chatId;

            ServerChattingWs.Save();
        }

        public static async Task Message(SocketMessage s)
        {
            var msg = (SocketUserMessage)s;
            var context = new SocketCommandContext(Global.Client, msg);

            if(context.User.IsBot)
            {
                return;
            }

            var servers = ServerChattingWs.workStation;

            if(context.IsPrivate)
            {
                var channel = Global.Client.GetGuild(servers.ServerId).GetTextChannel(servers.ChannelId);
                await channel.SendMessageAsync($"`{context.User.Username} from dm's:`\n{s}");
                await context.Channel.SendMessageAsync("test");
            }
        }

        [Command("chat2")]
        public void ChattingTogetherTestDM(ulong ServerId, ulong chatId)
        {
            var servers = ServerChattingWs.workStation;

            ServerAndChat SC = new ServerAndChat();
            ServerAndChat SC2 = new ServerAndChat();

            servers.ServerId = ServerId;
            servers.ChannelId = chatId;

            ServerChattingWs.Save();
        }
    }
}
