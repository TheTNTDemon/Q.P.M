namespace Q.P.M.Modules.Crafting
{
    using Discord.Commands;
    using System.Threading.Tasks;
    using Discord.WebSocket;
    //using static CraftingMethods.ShowCrafting;


    public class Crafting : ModuleBase<SocketCommandContext>
    {
        [Command("crafting")]
        public async Task ShowCrafting()
        {
            //await ReplyAsync($"```╔════════════════════════════════════════════════════════════════════════════════════════════════════╗\n{EachRecipe(Context.User as SocketGuildUser)}╚════════════════════════════════════════════════════════════════════════════════════════════════════╝```");
        }
    }
}
