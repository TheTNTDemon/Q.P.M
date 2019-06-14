using Discord.WebSocket;

namespace Q.P.M
{
    internal static class Global
    {
        /// <summary> Gets or sets the client </summary>
        public static DiscordSocketClient Client { get; set; }
    }
}
