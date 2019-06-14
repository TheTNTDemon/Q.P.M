using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Q.P.M.Modules.Server_Chatting_Testing
{
    public class ServerChattingWs
    {
        private const string ConfigFolder = "Resources";

        public static readonly ChattingServersDictionary workStation;

        private const string ChatConfigFile = "ChattingTogether.json";

        static ServerChattingWs()
        {

            if (!Directory.Exists(ConfigFolder))
            {
                Directory.CreateDirectory(ConfigFolder);
            }

            if (!File.Exists(ConfigFolder + "/" + ChatConfigFile))
            {
                workStation = new ChattingServersDictionary
                {
                    ChannelId = new ulong(),
                    ServerId = new ulong()
                };
                var json = JsonConvert.SerializeObject(workStation, Formatting.Indented);
                File.WriteAllText(ConfigFolder + "/" + ChatConfigFile, json);
            }
            else
            {
                var json = File.ReadAllText(ConfigFolder + "/" + ChatConfigFile);
                workStation = JsonConvert.DeserializeObject<ChattingServersDictionary>(json);
            }
        }

        public static void Save()
        {
            var json = JsonConvert.SerializeObject(workStation, Formatting.Indented);
            File.WriteAllText(ConfigFolder + "/" + ChatConfigFile, json);
        }
    }
}
