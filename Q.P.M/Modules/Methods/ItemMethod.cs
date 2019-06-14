using Discord.Commands;
using Discord.WebSocket;
using Q.P.M.Addons;
using Q.P.M.Core.UserAccounts;
using Q.P.M.Workstations;
using System.Collections.Generic;
using System.Linq;

namespace Q.P.M.Modules.Methods
{
    //TODO: Finish the item's and redo inventory a lil
    public class ItemMethod : ModuleBase<SocketCommandContext>
    {
        Dictionary<string, Item> AddItem = new Dictionary<string, Item>();

        public struct Slot
        {
            public int ID { get; set; }
            public int Amount { get; set; }
            public string Name { get; set; }

            public Slot(int ID, int Amount, string Name)
            {
                this.ID = ID;
                this.Amount = Amount;
                this.Name = Name;
            }
        }

        public static string ItemsAdding(int ItemId, SocketGuildUser socketUser, int GiveAmount = 0)
        {
            var item = ItemsListWs.workStation.Items[ItemId];

            //getting the account + currentcharacter
            var account = UserAccounts.GetAccount(socketUser);
            var current = account.CurrentCharacter.Inventory;

            //new Slot
            Slot slot = new Slot();

            //new temp List
            List<int> TempList = new List<int>();

            //stackable items adding
            if (item.Stackable)
            {
                foreach (var temp in current)
                {
                    TempList.Add(temp.ID);
                }

                //if inventory contains the item it wants to add
                if (TempList.Contains(ItemId))
                {
                    var temp2 = current.Where(x => x.ID == ItemId).FirstOrDefault();
                    temp2.Amount += GiveAmount;

                    UserAccounts.SaveAccounts(account);
                    return $"Added {GiveAmount} {item}'s to your current amount of {item} {socketUser.Username} :wink:";
                }
                //if inventory doesn't contain what it wants to add
                else
                {
                    slot.ID = ItemId;
                    slot.Amount = GiveAmount;
                    slot.Name = item.Name;

                    current.Add(slot);

                    UserAccounts.SaveAccounts(account);
                    return $"{socketUser.Username} has revcieved {GiveAmount} {item}'s!";
                }
            }
            //unstackable items adding
            else if (item.Stackable == false)
            {
                slot.ID = ItemId;
                slot.Amount = GiveAmount;
                slot.Name = item.Name;

                current.Add(slot);

                UserAccounts.SaveAccounts(account);
                return $"{socketUser.Username} has revcieved {GiveAmount}!";
            }
            return "eeeuuuhm, something went terribly wrong... :D";
        }
    }
}