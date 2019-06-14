using Newtonsoft.Json;
using Q.P.M.Addons;
using System.IO;

namespace Q.P.M.Workstations
{
    class ItemsListWs
    {
        public static readonly ItemDictionary workStation;

        private const string ConfigFolder = "Resources";

        private const string ItemConfigFile = "AllItems.json";

        static ItemsListWs()
        {

            if (!Directory.Exists(ConfigFolder))
            {
                Directory.CreateDirectory(ConfigFolder);
            }

            if (!File.Exists(ConfigFolder + "/" + ItemConfigFile))
            {
                workStation = new ItemDictionary
                {
                };
                var json = JsonConvert.SerializeObject(workStation, Formatting.Indented);
                File.WriteAllText(ConfigFolder + "/" + ItemConfigFile, json);
            }
            else
            {
                var json = File.ReadAllText(ConfigFolder + "/" + ItemConfigFile);
                workStation = JsonConvert.DeserializeObject<ItemDictionary>(json);
            }
        }

        public static void Save()
        {
            var json = JsonConvert.SerializeObject(workStation, Formatting.Indented);
            File.WriteAllText(ConfigFolder + "/" + ItemConfigFile, json);
        }
    }
}


