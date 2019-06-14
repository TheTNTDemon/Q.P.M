using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Q.P.M.Modules.Inventory
{
    public class Example : ModuleBase<SocketCommandContext>
    {
        private static Dictionary<ulong, ulong> ListenForReactionMessages = new Dictionary<ulong, ulong>();

        public static IReadOnlyDictionary<string, Emoji> ControlEmojis = new Dictionary<string, Emoji>
        {
            {"Smile", new Emoji("😀") },
            {"LaughCry", new Emoji("😂") },
            {"BlowKissie", new Emoji("😘") },
            {"MONEYYY", new Emoji("💲") },
            {"Stop", new Emoji("🚫") }
        };

        [Command("TestCommand")]
        public async Task TestCommand()
        {
            //Original message
            var message = await ReplyAsync("this is a test command");

            //Makes it so the Handler only looks for specific user(s if you modify it) and specific message it temp svaes and is possible for more messages at the same time
            ListenForReactionMessages.Add(message.Id, Context.User.Id);

            //Puts the reactions on the message
            await message.AddReactionAsync(ControlEmojis["Smile"]);
            await message.AddReactionAsync(ControlEmojis["LaughCry"]);
            await message.AddReactionAsync(ControlEmojis["BlowKissie"]);
            await message.AddReactionAsync(ControlEmojis["MONEYYY"]);
            await message.AddReactionAsync(ControlEmojis["Stop"]);
        }

        public static async Task HandleReactionAdded(Cacheable<IUserMessage, ulong> cacheMessage, SocketReaction reaction)
        {
            //So the bot doesnt trigger it himself
            if (reaction.User.Value.IsBot)
            {
                return;
            }

            //Sees if the message is correct
            if (ListenForReactionMessages.ContainsKey(reaction.MessageId))
            {
                //Removes emoji the person send if you want it (and need to edit so it works atleast)
                //await reaction.Message.Value.RemoveReactionAsync(reaction.Emote, reaction.User.Value);

                //sees if the specific user is reacting on it
                if(ListenForReactionMessages[reaction.MessageId] == reaction.User.Value.Id)
                {
                    if (reaction.Emote.Name == ControlEmojis["Smile"].Name)
                    {
                        //Example code:
                        await reaction.Message.Value.ModifyAsync(x => x.Content = "Your smile is as bright as the sun! 😀");
                    }
                    else if (reaction.Emote.Name == ControlEmojis["LaughCry"].Name)
                    {
                        //Example code:
                        await reaction.Message.Value.ModifyAsync(x => x.Content = "THAT JOKE THO! 😂");
                    }
                    else if (reaction.Emote.Name == ControlEmojis["BlowKissie"].Name)
                    {
                        //Example code:
                        await reaction.Message.Value.ModifyAsync(x => x.Content = "Hi cutie~ 😘");
                    }
                    else if (reaction.Emote.Name == ControlEmojis["MONEYYY"].Name)
                    {
                        //Example code:
                        await reaction.Message.Value.ModifyAsync(x => x.Content = "MONEEEEEEEEEEEEEEEEEEEEEEEEEY 💲");
                    }
                    else if (reaction.Emote.Name == ControlEmojis["Stop"].Name)
                    {
                        //Example code:
                        await reaction.Message.Value.ModifyAsync(x => x.Content = "See you later!");
                        await reaction.Message.Value.RemoveAllReactionsAsync();
                        ListenForReactionMessages.Remove(reaction.MessageId);
                        await Task.Delay(4000);
                        await reaction.Message.Value.DeleteAsync();
                    }
                }
            }
        }
    }
}
