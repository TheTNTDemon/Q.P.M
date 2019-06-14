/*using Discord.WebSocket;
using Q.P.M.Core.UserAccounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Q.P.M.Modules.Crafting.CraftingMethods
{
    public class ShowCrafting
    {
        public static string EachRecipe(SocketGuildUser context)
        {
            var acc = UserAccounts.GetAccount(context);
            var inv = acc.CurrentCharacter.Inventory;

            string test = null;

            if(inv.ContainsKey("rock") && inv.ContainsKey("potion") && inv.ContainsKey("stick"))
            {
                if(inv["rock"].Amount >= 3 && inv["potion"].Amount >= 10 && inv["stick"].Amount >= 2)
                {
                    test += string.Format("║ {0, -99}║\n", "a healing rock on a stick = 3 rocks + 10 potions + 2 sticks");
                }
            }
            else if (inv.ContainsKey("test"))
            {
                test += "you can craft idfk2";
            }
            return test;
        }
    }
}*/
