namespace Q.P.M.Core.UserAccounts
{
    // Bot dir
    using Core;

    // Discord
    using Discord.WebSocket;
    
    public static class UserAccounts
    {
        public static void SaveAccounts(UserAccount account)
        {
            DataManager.SaveUserAccounts(account);
        }
        
        public static UserAccount GetAccount(SocketUser user)
        {
            UpdateName(user);
            return GetOrCreateUserAccount(user);
        }
        
        private static void UpdateName(SocketUser user)
        {
            UserAccount acc = GetOrCreateUserAccount(user);
            acc.Name = user.Username;
            SaveAccounts(acc);
        }
        
        private static UserAccount GetOrCreateUserAccount(SocketUser user)
        {
            return DataManager.ExistsFile(user.Id.ToString())
                       ? DataManager.GetUserAccounts(user.Id.ToString())
                       : CreateUserAccount(user);
        }
        
        private static UserAccount CreateUserAccount(SocketUser user)
        {
            var newAccount = new UserAccount
            {
                Id = user.Id,
                Name = user.Username,
                test = null,
            };
            SaveAccounts(newAccount);
            return newAccount;
        }
    }
}

