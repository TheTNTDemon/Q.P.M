using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Q.P.M.Addons;
using Q.P.M.Core.UserAccounts;
using Q.P.M.Workstations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q.P.M.Modules.Inventory
{
    public class InventoryShowing : ModuleBase<SocketCommandContext>
    {
        private static string LineIndicator = " ◄";

        private static Dictionary<ulong, ulong> ListenForReactionMessages = new Dictionary<ulong, ulong>();

        public static IReadOnlyDictionary<string, Emoji> ControlEmojis = new Dictionary<string, Emoji>
        {
            {"right", new Emoji("➡") },
            {"left", new Emoji("⬅") },
            {"stopInv", new Emoji("🚫") },
            {"sell", new Emoji("💲") },
            {"yes", new Emoji("✅") },
            {"no", new Emoji("❌") },
            {"up", new Emoji("⬆") },
            {"down", new Emoji("⬇") }
        };

        [Command("inv", RunMode = RunMode.Async)]
        public async Task InventoryTesting(int pagenumber = 1)
        {
            var account = UserAccounts.GetAccount(Context.User);
            var testing = account.CurrentCharacter;
            string fullstring = null;

            List<string> items = new List<string>();

            foreach (var item in testing.Inventory)
            {
                items.Add(item.Name);
            }

            int quickmaf = 10 - items.Count();
            if (quickmaf < 0)
            {
                quickmaf = 0;
            }
            int InventoryItemCount = 10 - quickmaf;

            const int itemsPerPage = 10;
            for (var i = 0; i < itemsPerPage; i++)
            {
                var index = itemsPerPage * (pagenumber - 1) + i;
                var itemname = " ";
                if (index < items.Count)
                {
                    itemname = items[i];
                }
                fullstring += string.Format("║ {0, -35}║\n", itemname);
            }

            var lastPage = 1 + (items.Count / (itemsPerPage + 1));

            var output = string.Format("{0}'s inventory:\n╔════════════════════════════════════╗\n{1}║ {2, -35}║\n╠════════════════════════════════════╩═══════════════════════════════════╗\n", testing.Name, fullstring, $"page {pagenumber}/{lastPage}...");
            var count = 0;
            for (int i = 0; i < output.Length; i++)
            {
                if (output[i] == '\n')
                {
                    if (count == 2)
                    {
                        output = output.Insert(i, LineIndicator);
                        break;
                    }
                    else
                    {
                        count++;
                    }
                }
            }

            var Seperated = SeperateTheLines(output);
            foreach (string s in Seperated)
            {
                if (ContainsLineIndicator(s))
                {
                    string ItemName = null;
                    string Class = null;
                    string DamageOramount = null;
                    string Desc = null;

                    var GetItem = s.Split('║', StringSplitOptions.RemoveEmptyEntries)[0];
                    GetItem = GetItem.Remove(0, 1);
                    GetItem = GetItem.Remove(GetItem.Length - 1, 1);

                    char c;
                    var pos = GetItem.Length;
                    do
                    {
                        pos--;
                        c = GetItem[pos];
                    } while (c == ' ');
                    GetItem = GetItem.Remove(++pos, GetItem.Length - pos);
                    
                    var CurrentItem = testing.Inventory.Where(x => x.Name == GetItem).FirstOrDefault();
                    var nextItem = ItemsListWs.workStation.Items[CurrentItem.ID];
                    if (CurrentItem.Amount > 0)
                    {
                        ItemName = $"Item Name: {GetItem}";
                        Class = $"Class: {nextItem.Class}";
                        DamageOramount = $"Amount: {CurrentItem.Amount}";
                        Desc = nextItem.Description;
                    }
                    else
                    {
                        ItemName = $"Item Name: {GetItem}";
                        Class = $"Class: {nextItem.Class}";
                        DamageOramount = $"Damage: {nextItem.Damage}";
                        Desc = nextItem.Description;
                    }

                    var message = await ReplyAsync(string.Format("```{0}║ {1, -24}{2, -24}{3, -23}║\n║{4, -72}║\n║ {5, -71}║\n║ {6, -71}║\n║{4, -72}║\n╚════════════════════════════════════════════════════════════════════════╝```", output, ItemName, Class, DamageOramount, " ", "Description:", Desc));
                    ListenForReactionMessages.Add(message.Id, Context.User.Id);
                    account.PageNumber = pagenumber;
                    await message.AddReactionAsync(ControlEmojis["left"]);
                    await message.AddReactionAsync(ControlEmojis["up"]);
                    await message.AddReactionAsync(ControlEmojis["down"]);
                    await message.AddReactionAsync(ControlEmojis["right"]);
                    await message.AddReactionAsync(ControlEmojis["stopInv"]);
                    await message.AddReactionAsync(ControlEmojis["sell"]);

                }
            }
        }

        private static string[] SeperateTheLines(string message)
        {
            return message.Split('\n');
        }

        private static bool ContainsLineIndicator(string line)
        {
            var substringLength = line.Length - LineIndicator.Length;
            if (substringLength < 0) { return false; }

            var subLine = line.Substring(substringLength);
            return (subLine.Equals(LineIndicator));
        }

        public static async Task HandleReactionAdded(Cacheable<IUserMessage, ulong> cacheMessage, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot)
            {
                return;
            }

            //var up = ControlEmojis["up"];
            //var down = ControlEmojis["down"];
            var stopInv = ControlEmojis["stopInv"];
            var sell = ControlEmojis["sell"];

            var yes = ControlEmojis["yes"];
            var no = ControlEmojis["no"];

            if (ListenForReactionMessages.ContainsKey(reaction.MessageId))
            {
                await reaction.Message.Value.RemoveReactionAsync(reaction.Emote, reaction.User.Value);

                if (ListenForReactionMessages[reaction.MessageId] == reaction.User.Value.Id)
                {
                    if (reaction.Emote.Name == ControlEmojis["up"].Name)
                    {
                        await HandleMovement(reaction, cacheMessage.Value.Content, true);
                    }
                    else if (reaction.Emote.Name == ControlEmojis["down"].Name)
                    {
                        await HandleMovement(reaction, cacheMessage.Value.Content, false);
                    }
                    else if (reaction.Emote.Name == ControlEmojis["stopInv"].Name)
                    {
                        await reaction.Message.Value.DeleteAsync();
                    }
                    else if (reaction.Emote.Name == ControlEmojis["sell"].Name)
                    {
                        await reaction.Message.Value.RemoveAllReactionsAsync();
                        await reaction.Message.Value.AddReactionAsync(ControlEmojis["yes"]);
                        await reaction.Message.Value.AddReactionAsync(ControlEmojis["no"]);
                    }
                    else if (reaction.Emote.Name == ControlEmojis["yes"].Name)
                    {
                        await reaction.Message.Value.RemoveAllReactionsAsync();
                        await reaction.Message.Value.Channel.SendMessageAsync("you accepted the selling request.");
                        //await reaction.Message.Value.AddReactionAsync(up);
                        //await reaction.Message.Value.AddReactionAsync(down);
                        await reaction.Message.Value.AddReactionAsync(stopInv);
                        await reaction.Message.Value.AddReactionAsync(sell);
                    }
                    else if (reaction.Emote.Name == ControlEmojis["no"].Name)
                    {
                        await reaction.Message.Value.RemoveAllReactionsAsync();
                        await reaction.Message.Value.Channel.SendMessageAsync("you denied the selling request.");
                        //await reaction.Message.Value.AddReactionAsync(up);
                        //await reaction.Message.Value.AddReactionAsync(down);
                        await reaction.Message.Value.AddReactionAsync(stopInv);
                        await reaction.Message.Value.AddReactionAsync(sell);
                    }
                    else if (reaction.Emote.Name == ControlEmojis["right"].Name)
                    {
                        var account = UserAccounts.GetAccount(reaction.User.Value as SocketGuildUser);
                        var testing = account.CurrentCharacter;
                        string fullstring = null;
                        int pagenumber = account.PageNumber + 1;

                        const int itemsPerPage = 10;

                        List<string> items = new List<string>();

                        foreach (var item in testing.Inventory)
                        {
                            items.Add(item.Name);
                        }

                        var lastPage = 1 + (items.Count / (itemsPerPage + 1));

                        if (pagenumber > lastPage)
                        {
                            pagenumber = 1;
                        }

                        int quickmaf = 10 - items.Count();
                        if (quickmaf < 0)
                        {
                            quickmaf = 0;
                        }
                        int InventoryItemCount = 10 - quickmaf;

                        for (var i = 0; i < itemsPerPage; i++)
                        {
                            var index = itemsPerPage * (pagenumber - 1) + i;
                            var itemname = " ";
                            if (index < items.Count)
                            {
                                itemname = items[index];
                            }
                            fullstring += string.Format("║ {0, -35}║\n", itemname);
                        }

                        var output = string.Format("{0}'s inventory:\n╔════════════════════════════════════╗\n{1}║ {2, -35}║\n╠════════════════════════════════════╩═══════════════════════════════════╗\n", testing.Name, fullstring, $"page {pagenumber}/{lastPage}...");
                        var count = 0;
                        for (int i = 0; i < output.Length; i++)
                        {
                            if (output[i] == '\n')
                            {
                                if (count == 2)
                                {
                                    output = output.Insert(i, LineIndicator);
                                    break;
                                }
                                else
                                {
                                    count++;
                                }
                            }
                        }

                        var Seperated = SeperateTheLines(output);
                        foreach (string s in Seperated)
                        {
                            if (ContainsLineIndicator(s))
                            {
                                string ItemName = null;
                                string Class = null;
                                string DamageOramount = null;
                                string Desc = null;

                                var GetItem = s.Split('║', StringSplitOptions.RemoveEmptyEntries)[0];
                                GetItem = GetItem.Remove(0, 1);
                                GetItem = GetItem.Remove(GetItem.Length - 1, 1);

                                char c;
                                var pos = GetItem.Length;
                                do
                                {
                                    pos--;
                                    c = GetItem[pos];
                                } while (c == ' ');
                                GetItem = GetItem.Remove(++pos, GetItem.Length - pos);

                                var CurrentItem = testing.Inventory.Where(x => x.Name == GetItem).FirstOrDefault();
                                var nextItem = ItemsListWs.workStation.Items[CurrentItem.ID];
                                if (CurrentItem.Amount > 0)
                                {
                                    ItemName = $"Item Name: {GetItem}";
                                    Class = $"Class: {nextItem.Class}";
                                    DamageOramount = $"Amount: {CurrentItem.Amount}";
                                    Desc = nextItem.Description;
                                }
                                else
                                {
                                    ItemName = $"Item Name: {GetItem}";
                                    Class = $"Class: {nextItem.Class}";
                                    DamageOramount = $"Damage: {nextItem.Damage}";
                                    Desc = nextItem.Description;
                                }

                                var message = (string.Format("```{0}║ {1, -24}{2, -24}{3, -23}║\n║{4, -72}║\n║ {5, -71}║\n║ {6, -71}║\n║{4, -72}║\n╚════════════════════════════════════════════════════════════════════════╝```", output, ItemName, Class, DamageOramount, " ", "Description:", Desc));
                                await reaction.Message.Value.ModifyAsync(x => x.Content = message);
                                account.PageNumber = pagenumber;
                                UserAccounts.SaveAccounts(account);
                            }
                        }
                    }
                    else if (reaction.Emote.Name == ControlEmojis["left"].Name)
                    {
                        var account = UserAccounts.GetAccount(reaction.User.Value as SocketGuildUser);
                        var testing = account.CurrentCharacter;
                        string fullstring = null;
                        int pagenumber = account.PageNumber - 1;

                        List<string> items = new List<string>();

                        foreach (var item in testing.Inventory)
                        {
                            items.Add(item.Name);
                        }

                        const int itemsPerPage = 10;

                        var lastPage = 1 + (items.Count / (itemsPerPage + 1));

                        if (pagenumber == 0)
                        {
                            pagenumber = lastPage;
                        }

                        int quickmaf = 10 - items.Count();
                        if (quickmaf < 0)
                        {
                            quickmaf = 0;
                        }
                        int InventoryItemCount = 10 - quickmaf;

                        for (var i = 0; i < itemsPerPage; i++)
                        {
                            var index = itemsPerPage * (pagenumber - 1) + i;
                            var itemname = " ";
                            if (index < items.Count)
                            {
                                itemname = items[index];
                            }
                            fullstring += string.Format("║ {0, -35}║\n", itemname);
                        }

                        var output = string.Format("{0}'s inventory:\n╔════════════════════════════════════╗\n{1}║ {2, -35}║\n╠════════════════════════════════════╩═══════════════════════════════════╗\n", testing.Name, fullstring, $"page {pagenumber}/{lastPage}...");
                        var count = 0;
                        for (int i = 0; i < output.Length; i++)
                        {
                            if (output[i] == '\n')
                            {
                                if (count == 2)
                                {
                                    output = output.Insert(i, LineIndicator);
                                    break;
                                }
                                else
                                {
                                    count++;
                                }
                            }
                        }

                        var Seperated = SeperateTheLines(output);
                        foreach (string s in Seperated)
                        {
                            if (ContainsLineIndicator(s))
                            {
                                string ItemName = null;
                                string Class = null;
                                string DamageOramount = null;
                                string Desc = null;

                                var GetItem = s.Split('║', StringSplitOptions.RemoveEmptyEntries)[0];
                                GetItem = GetItem.Remove(0, 1);
                                GetItem = GetItem.Remove(GetItem.Length - 1, 1);

                                char c;
                                var pos = GetItem.Length;
                                do
                                {
                                    pos--;
                                    c = GetItem[pos];
                                } while (c == ' ');
                                GetItem = GetItem.Remove(++pos, GetItem.Length - pos);

                                var CurrentItem = testing.Inventory.Where(x => x.Name == GetItem).FirstOrDefault();
                                var nextItem = ItemsListWs.workStation.Items[CurrentItem.ID];
                                if (CurrentItem.Amount > 0)
                                {
                                    ItemName = $"Item Name: {GetItem}";
                                    Class = $"Class: {nextItem.Class}";
                                    DamageOramount = $"Amount: {CurrentItem.Amount}";
                                    Desc = nextItem.Description;
                                }
                                else
                                {
                                    ItemName = $"Item Name: {GetItem}";
                                    Class = $"Class: {nextItem.Class}";
                                    DamageOramount = $"Damage: {nextItem.Damage}";
                                    Desc = nextItem.Description;
                                }

                                var message = (string.Format("```{0}║ {1, -24}{2, -24}{3, -23}║\n║{4, -72}║\n║ {5, -71}║\n║ {6, -71}║\n║{4, -72}║\n╚════════════════════════════════════════════════════════════════════════╝```", output, ItemName, Class, DamageOramount, " ", "Description:", Desc));
                                await reaction.Message.Value.ModifyAsync(x => x.Content = message);
                                account.PageNumber = pagenumber;
                                UserAccounts.SaveAccounts(account);
                            }
                        }
                    }
                }
            }
        }

        private static async Task HandleMovement(SocketReaction reaction, string message, bool dirUp)
        {
            var seperatedMessage = SeperateTheLines(message);
            await reaction.Message.Value.ModifyAsync(msg => msg.Content = PerformMove(seperatedMessage, dirUp, reaction));
        }

        private static string PerformMove(string[] messageLines, bool dirUp, SocketReaction context)
        {
            string ItemName = null;
            string Class = null;
            string DamageOramount = null;
            string Desc = null;

            // Get the current index of the line indicator
            var currentIndex = GetCurrentIndex(messageLines);
            var newIndex = currentIndex + (dirUp ? -1 : 1);

            var currentItemName = GetItemName(messageLines, currentIndex);
            var newMessage = new StringBuilder();

            if (newIndex > 1 && newIndex < messageLines.Length - 7)
            {
                // Get the name of the next item
                var nextItemName = GetItemName(messageLines, newIndex, currentItemName);

                // Get the current and next item
                var account = UserAccounts.GetAccount(context.User.Value as SocketGuildUser);
                var testing = account.CurrentCharacter;
                if (nextItemName == "")
                {
                    ItemName = $"Item Name: ";
                    Class = $"Class: ";
                    DamageOramount = $" ";
                    Desc = " ";
                }
                else
                {
                    var CurrentItem = testing.Inventory.Where(x => x.Name == nextItemName).FirstOrDefault();
                    var nextItem = ItemsListWs.workStation.Items[CurrentItem.ID];
                    if (CurrentItem.Amount > 0)
                    {
                        ItemName = $"Item Name: {nextItemName}";
                        Class = $"Class: {nextItem.Class}";
                        DamageOramount = $"Amount: {CurrentItem.Amount}";
                        Desc = nextItem.Description;
                    }
                    else
                    {
                        ItemName = $"Item Name: {nextItemName}";
                        Class = $"Class: {nextItem.Class}";
                        DamageOramount = $"Damage: {nextItem.Damage}";
                        Desc = nextItem.Description;
                    }
                }

                // Move the line indicator
                var line = messageLines[currentIndex];
                messageLines[currentIndex] = line.Substring(0, line.Length - LineIndicator.Length);
                messageLines[newIndex] = $"{messageLines[newIndex]}{LineIndicator}";

                var descIndex = messageLines.Length - 6;

                for (int i = 0; i < descIndex; i++)
                {
                    newMessage.Append($"{messageLines[i]}\n");
                }

                var emptyLine = "║                                                                        ║";
                var closingLine = "╚════════════════════════════════════════════════════════════════════════╝";
                var desc = string.Format("║ {0, -24}{1, -24}{2, -23}║\n{3, -72}\n║ {4, -71}║\n║ {5, -71}║\n{3, -72}\n{6, -72}```", ItemName, Class, DamageOramount, emptyLine, "Description:", Desc, closingLine);
                newMessage.Append(desc);
            }
            else
            {
                foreach (string s in messageLines)
                {
                    newMessage.Append($"{s}\n");
                }
            }
            return newMessage.ToString();
        }

        public static string GetItemName(string[] lines, int index, string current = null)
        {
            if (index < 0 || index > lines.Length) { return null; }
            return GetItemName(lines[index], current);
        }

        public static string GetItemName(string s, string current = null)
        {
            var GetItem = s.Split('║', StringSplitOptions.RemoveEmptyEntries)[0];
            GetItem = GetItem.Remove(0, 1);
            GetItem = GetItem.Remove(GetItem.Length - 1, 1);
            char c;
            var pos = GetItem.Length;
            do
            {
                pos--;
                if (pos == -1)
                {
                    break;
                }
                c = GetItem[pos];
            } while (c == ' ');
            GetItem = GetItem.Remove(++pos, GetItem.Length - pos);
            return GetItem;
        }

        private static int GetCurrentIndex(string[] messageLines)
        {
            for (int i = 0; i < messageLines.Length; i++)
            {
                var line = messageLines[i];
                var substringStart = line.Length - LineIndicator.Length;
                if (substringStart < 0) { continue; }

                var subLine = line.Substring(substringStart);
                if (subLine.Equals(LineIndicator))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}