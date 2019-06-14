using Discord.Commands;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Q.P.M.Core.UserAccounts;
using Discord.WebSocket;
using static Q.P.M.Modules.Methods.ItemMethod;
using Q.P.M.Preconditions;
using Discord;
using Q.P.M.Addons;
using System.Linq;
using Q.P.M.Workstations;

namespace Q.P.M.Modules
{
    public class BasicCommands : ModuleBase<SocketCommandContext>
    {
        private readonly Random rnd = new Random();

        private enum RewardType { Common, Uncommon, Rare, Legendary };

        [Command("link")]
        [Dm(true)]
        public async Task TestHyperlink()
        {
            var emb = new EmbedBuilder();
            emb.WithTitle("test")
                .WithUrl("https://www.youtube.com/watch?v=eG9UKf5DfrA)");
            emb.WithDescription($"**Test:**\nif you click [this](https://www.youtube.com/watch?v=eG9UKf5DfrA) then you wil get brought to a youtube vid.");
            emb.AddField("test:", "[this](https://www.youtube.com/watch?v=eG9UKf5DfrA)");

            await ReplyAsync("", false, emb.Build());
        }

        [Command("vckick")]
        public async Task TestingModifyAsync(SocketGuildUser User)
        {
            var channel = await Context.Guild.CreateVoiceChannelAsync("test");

            await channel.DeleteAsync();
        }

        [Command("test")]
        public async Task SetCharacter()
        {
            var acc = UserAccounts.GetAccount(Context.User);
            acc.Character1.CurentCharacter = 1;
            acc.CurrentCharacter = acc.Character1;
            UserAccounts.SaveAccounts(acc);
        }

        [Command("test2")]
        public async Task TestShit2(int GiveAmount)
        {
            var account = UserAccounts.GetAccount(Context.User);
            var current = account.CurrentCharacter.Inventory;
            var temp2 = current.Where(x => x.ID == 1).Select(x => x.Amount).FirstOrDefault();
            temp2 += GiveAmount;

            UserAccounts.SaveAccounts(account);
        }

        [Command("test6")]
        [Dm(true)]
        [LevelOrHigher(40)]
        public async Task CoolExp()
        {
            var emojis = new Emote[] {
        Emote.Parse("<:HeartFull:491245550956576768>"),
        Emote.Parse("<:HeartFull:491245550956576768>"),
        Emote.Parse("<:HeartFull:491245550956576768>"),
        Emote.Parse("<:HeartHalf:491245562285391912>"),
        Emote.Parse("<:HeartHalf:491245562285391912>"),
        Emote.Parse("<:HeartHalf:491245562285391912>"),
        Emote.Parse("<:HeartHalf:491245562285391912>"),
        Emote.Parse("<:HeartHalf:491245562285391912>"),
        Emote.Parse("<:HeartEmpty:491245570896166931>"),
        Emote.Parse("<:HeartEmpty:491245570896166931>"),
        Emote.Parse("<:HeartEmpty:491245570896166931>")
        };

            var account = UserAccounts.GetAccount(Context.User);
            var currentPercentage = (account.CurrentCharacter.Maxhealth - account.CurrentCharacter.CurrentHealth) * 100 / account.CurrentCharacter.Maxhealth;
            var bar = "";
            var filled = (int)currentPercentage / 10;
            var remaining = (int)(currentPercentage - filled * 10);
            for (var i = 0; i < 9 - filled; i++)
            {
                bar += emojis[0];
            }

            bar += emojis[remaining];

            for (var i = 0; i < filled; i++)
            {
                bar += emojis[10];
            }

            await ReplyAsync($"{account.CurrentCharacter.CurrentHealth} out of {account.CurrentCharacter.Maxhealth}HP\n{bar}");
        }

        [Command("test3")]
        public async Task AddShit(int itemid)
        {
            Item itemInfo = new Item();
            itemInfo.Stackable = true;
            itemInfo.Class = "misc";
            itemInfo.Description = "it's a fucking potion...";

            var account = ItemsListWs.workStation.Items;
            account.Add(itemid, itemInfo);
            ItemsListWs.Save();
        }

        /*[Command("equipped")]
        public async Task EqquipedWeapon()
        {
            var account = UserAccounts.GetAccount(Context.User);
            var Equiped = account.CurrentCharacter.EquipedWeapon;
            var testing = account.CurrentCharacter.Inventory[Equiped];
            await ReplyAsync($"Name: {Equiped}\nClass: {testing.Class}\nDamage: {testing.Damage}");
        }

        [Command("inventory")]
        public async Task ShowsInv()
        {
            var account = UserAccounts.GetAccount(Context.User);
            var testing = account.CurrentCharacter;
            string itemname = null;
            string fullstring = null;

            foreach (var item in testing.Inventory)
            {
                if (item.Value.Amount > 0)
                {
                    fullstring += $"name: {item.Key} | amount: {item.Value.Amount} | class: {item.Value.Class} | desc: {item.Value.Description}\n";
                }
                else
                {
                    itemname = item.Key;
                    fullstring += $"name: {item.Key} | damage: {item.Value.Damage} | class: {item.Value.Class} | desc: {item.Value.Description}\n";
                }
            }

            await ReplyAsync($"{fullstring}");
        }

        [Command("inv2")]
        public async Task InventoryTesting()
        {
            var account = UserAccounts.GetAccount(Context.User);
            var testing = account.CurrentCharacter;
            var currentItem = testing.Inventory["stick"];
            await ReplyAsync($"{currentItem.Class}, {currentItem.Description}");
        }*/

        [Command("add")]
        public async Task AddingPotion(int name)
        {
            int amount = rnd.Next(1, 26);
            await ReplyAsync($"{ItemsAdding(name, Context.User as SocketGuildUser, amount)}");
        }

        [Command("crate")]
        public async Task RandomNumber()
        {
            int Chance = rnd.Next(1, 101);

            if (Chance <= 50)
            {
                int rewardNumber = rnd.Next(0, 8);

                await ReplyAsync($"{Context.User.Username} won a `{ItemReward(rewardNumber, RewardType.Common)}` from a simple crate!");
            }
            else if (Chance <= 80)
            {
                int rewardNumber = rnd.Next(0, 6);

                await ReplyAsync($"{Context.User.Username} won a `{ItemReward(rewardNumber, RewardType.Uncommon)}` from a simple crate!");
            }
            else if (Chance <= 95)
            {
                int rewardNumber = rnd.Next(0, 4);

                await ReplyAsync($"{Context.User.Username} won a `{ItemReward(rewardNumber, RewardType.Rare)}` from a simple crate!");
            }
            else
            {
                int rewardNumber = rnd.Next(0, 2);

                await ReplyAsync($"{Context.User.Username} won a `{ItemReward(rewardNumber, RewardType.Legendary)}` from a simple crate!");
            }
        }

        private string ItemReward(int number, RewardType type)
        {
            var acc = UserAccounts.GetAccount(Context.User);

            if (type == RewardType.Common)
            {
                switch (number)
                {
                    case 0:
                        ItemsAdding(1, Context.User as SocketGuildUser, 5);
                        return "small potion package 1";

                    case 1:
                        ItemsAdding(1, Context.User as SocketGuildUser, 5);
                        return "small potion package 2";

                    case 2:
                        ItemsAdding(1, Context.User as SocketGuildUser, 5);
                        return "small potion package 3";

                    case 3:
                        ItemsAdding(1, Context.User as SocketGuildUser, 5);
                        return "small potion package 4";

                    case 4:
                        ItemsAdding(1, Context.User as SocketGuildUser, 5);
                        return "small potion package 5";

                    case 5:
                        ItemsAdding(1, Context.User as SocketGuildUser, 5);
                        return "small potion package 6";

                    case 6:
                        ItemsAdding(1, Context.User as SocketGuildUser, 5);
                        return "small potion package 7";

                    case 7:
                        ItemsAdding(1, Context.User as SocketGuildUser, 5);
                        return "small potion package 8";
                }
            }
            else if (type == RewardType.Uncommon)
            {
                switch (number)
                {
                    case 0:
                        ItemsAdding(1, Context.User as SocketGuildUser, 10);
                        return "medium potion package 1";

                    case 1:
                        ItemsAdding(1, Context.User as SocketGuildUser, 10);
                        return "medium potion package 2";

                    case 2:
                        ItemsAdding(1, Context.User as SocketGuildUser, 10);
                        return "medium potion package 3";

                    case 3:
                        ItemsAdding(1, Context.User as SocketGuildUser, 10);
                        return "medium potion package 4";

                    case 4:
                        ItemsAdding(1, Context.User as SocketGuildUser, 10);
                        return "medium potion package 5";

                    case 5:
                        ItemsAdding(1, Context.User as SocketGuildUser, 10);
                        return "medium potion package 6";
                }
            }
            else if (type == RewardType.Rare)
            {
                switch (number)
                {
                    case 0:
                        ItemsAdding(1, Context.User as SocketGuildUser, 15);
                        return "large potion package 1";

                    case 1:
                        ItemsAdding(1, Context.User as SocketGuildUser, 15);
                        return "large potion package 2";

                    case 2:
                        ItemsAdding(1, Context.User as SocketGuildUser, 15);
                        return "large potion package 3";

                    case 3:
                        ItemsAdding(1, Context.User as SocketGuildUser, 15);
                        return "large potion package 4";
                }
            }
            else if (type == RewardType.Legendary)
            {
                switch (number)
                {
                    case 0:
                        ItemsAdding(1, Context.User as SocketGuildUser, 20);
                        return "extra large potion package 1";

                    case 1:
                        ItemsAdding(1, Context.User as SocketGuildUser, 20);
                        return "extra large potion package 1";
                }
            }
            return "eeeeuuuuhm, something went very wrong?";
        }
    }
}
