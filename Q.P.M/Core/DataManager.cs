namespace Q.P.M.Core
{
    // System
    using System.IO;

    // Bot dir
    using UserAccounts;

    // Json
    using Newtonsoft.Json;

    /// <summary> The data manager. </summary>
    public static class DataManager
    {
        private const string AccountPath = "Resources/Users/{0}.json";

        private const string GuildPath = "Resources/Guilds/{0}.json"; //add later when needed

        public static void SaveUserAccounts(UserAccount accounts)
        {
            // save all user accounts
            string filePath = string.Format(AccountPath, accounts.Id.ToString());
            var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        // get all user accounts
        
        public static UserAccount GetUserAccounts(string filePath)
        {
            filePath = string.Format(AccountPath, filePath);
            if (!File.Exists(filePath))
            {
                return null;
            }

            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<UserAccount>(json);
        }
        
        public static bool ExistsFile(string file)
        {
            file = string.Format(AccountPath, file);
            return File.Exists(file);
        }
    }
}
