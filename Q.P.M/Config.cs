using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Q.P.M
{
    internal class Config
    {
        /// <summary> the bot config </summary>
        public static readonly BotConfig Bot;

        /// <summary> the folder where the config is located </summary>
        private const string ConfigFolder = "Resources";

        /// <summary> the file where the config is located </summary>
        private const string ConfigFile = "config.json";

        /// <summary> Initializes static members of the <see cref="Config"/> class </summary>
        static Config()
        {
            if (!Directory.Exists(ConfigFolder))
            {
                Directory.CreateDirectory(ConfigFolder);
            }

            if (!File.Exists(ConfigFolder + "/" + ConfigFile))
            {
                Bot = new BotConfig
                {
                    PrefixDictionary = new Dictionary<ulong, string>()
                };
                var json = JsonConvert.SerializeObject(Bot, Formatting.Indented);
                File.WriteAllText(ConfigFolder + "/" + ConfigFile, json);
            }
            else
            {
                var json = File.ReadAllText(ConfigFolder + "/" + ConfigFile);
                Bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }

        /// <summary> The save prefix. </summary>
        public static void Save()
        {
            var json = JsonConvert.SerializeObject(Bot, Formatting.Indented);
            File.WriteAllText(ConfigFolder + "/" + ConfigFile, json);
        }

        /// <summary> the bot's config </summary>
        public struct BotConfig
        {
            public Dictionary<ulong, string> PrefixDictionary;

            public string Token { get; set; }
        }
    }
}
