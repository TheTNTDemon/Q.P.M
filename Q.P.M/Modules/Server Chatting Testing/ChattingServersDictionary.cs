using System.Collections.Generic;

namespace Q.P.M.Modules.Server_Chatting_Testing
{
    public class ChattingServersDictionary
    {
        public ulong ServerId { get; set; }
        public ulong ChannelId { get; set; }
    }

    public class ServerAndChat
    {
        public ulong ServerId { get; set; }
        public ulong ChannelId { get; set; }
    }
}
