using System;
using System.Threading.Tasks;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.Addons.Interactive.InteractiveBuilder;
using Discord.WebSocket;

namespace Q.P.M.Modules
{
    public class Testing : InteractiveBase
    {
        private readonly Random rnd = new Random();


        //TODO: Edit PvP to make it like the full combat system due now its just test version
        [Command("battle", RunMode = RunMode.Async)]
        public async Task PVPBAttleTest(SocketUser user)
        {
            int WhoTurnItis = rnd.Next(0, 2);
            int User1HP = 100;
            int User2HP = 100;
            string WhoTurn = null;

            if(WhoTurnItis == 0)
            {
                WhoTurn = Context.User.Username;
            }
            else if (WhoTurnItis == 1)
            {
                WhoTurn = user.Username;
            }

            var msg = await Context.Channel.SendMessageAsync($"{Context.User.Username} has you challanged to a duel {user.Username}! please say `accept` or `deny`, they have 1 minute to respond!");

            {
                var interactiveMessage = new InteractiveMessageBuilder("")
                        .EnableLoop()
                        .SetWrongResponseMessage("please say `accept` or `deny`")
                         .ListenUsers(Context.User ,user)
                        .SetOptions("accept", "deny")
                        .WithTimeSpan(TimeSpan.FromMinutes(1))
                        .SetTimeoutMessage("sorry but the player didn't react in time!")
                        .Build();

                var response = await StartInteractiveMessage(interactiveMessage);

                if(response.Author == user)
                {
                    if (response != null)
                    {
                        if (response.ToString() == "accept")
                        {
                            await msg.ModifyAsync(x => x.Content = $"The duel has been accepted! it will start in 5 seconds.");
                            await Task.Delay(5000);
                            await msg.ModifyAsync(x => x.Content = $"{Context.User.Username} VS {user.Username}\nIt's {WhoTurn}'s turn\n{Context.User.Username}'s HP: {User1HP}\n{user.Username}'s HP: {User2HP}\nps all you gotta do is say `attack`!");
                            await response.DeleteAsync();
                        }
                        else if (response.ToString() == "deny")
                        {
                            await msg.ModifyAsync(x => x.Content = $"Sorry {Context.User.Username} but {user.Username} has denied the battle.");
                            await response.DeleteAsync();
                            return;
                        }
                    }
                }
            }

            {
                for (var i = 0; i < 1000; i++)
                {
                    var interactiveMessage = new InteractiveMessageBuilder("")
                        .EnableLoop()
                        .WithCancellationWord("stop")
                        .SetWrongResponseMessage("just say `attack`!")
                         .ListenUsers(Context.User, user)
                        .SetOptions("attack")
                        .Build();

                    var response = await StartInteractiveMessage(interactiveMessage);
                    if (response != null)
                    {
                        if (WhoTurnItis == 0)
                        {
                            if(response.Author == user)
                            {
                                await ReplyAndDeleteAsync("It's not your turn.", timeout: TimeSpan.FromSeconds(5));
                            }
                            else
                            {
                                int damage = rnd.Next(12, 18);
                                int NewHP = User2HP - damage;
                                User2HP = NewHP;
                                if (NewHP <= 0)
                                {
                                    await msg.ModifyAsync(x => x.Content = $"{Context.User.Username} has won from {user.Username} with {User1HP} HP left!");
                                    await response.DeleteAsync();
                                    return;
                                }
                                else
                                {
                                    WhoTurnItis = 1;
                                    await msg.ModifyAsync(x => x.Content = $"{Context.User.Username} VS {user.Username}\nIt's {user.Username}'s turn\n{Context.User.Username}'s HP: {User1HP}\n{user.Username}'s HP: {NewHP}");
                                }
                            }
                        }
                        else if (WhoTurnItis == 1)
                        {
                            if (response.Author == Context.User)
                            {
                                await ReplyAndDeleteAsync("It's not your turn.", timeout: TimeSpan.FromSeconds(5));
                            }
                            else
                            {
                                int damage = rnd.Next(12, 18);
                                int NewHP = User1HP - damage;
                                User1HP = NewHP;
                                if (NewHP <= 0)
                                {
                                    await msg.ModifyAsync(x => x.Content = $"{user.Username} has won from {Context.User.Username} with {User2HP} HP left!");
                                    await response.DeleteAsync();
                                    return;
                                }
                                else
                                {
                                    WhoTurnItis = 0;
                                    await msg.ModifyAsync(x => x.Content = $"{Context.User.Username} VS {user.Username}\nIt's {Context.User.Username}'s turn\n{Context.User.Username}'s HP: {NewHP}\n{user.Username}'s HP: {User2HP}");
                                }
                            }
                        }
                        await Task.Delay(400);
                        await response.DeleteAsync();
                    }
                }
            }
        }

        //TODO: Edit PvE to make it like the full combat system + add it to adventures
        [Command("PVE", RunMode = RunMode.Async)]
        public async Task PVEBattleTest()
        {
            int WhoTurnItis = rnd.Next(0, 2);
            int User1HP = 100;
            int EnemeyHP = 100;

            string WhoTurn = null;

            if (WhoTurnItis == 0)
            {
                WhoTurn = Context.User.Username;
            }
            else if (WhoTurnItis == 1)
            {
                WhoTurn = "monster";
            }

            var msg = await Context.Channel.SendMessageAsync($"{Context.User.Username} VS Monster\nIt's {Context.User.Username}'s turn\n{Context.User.Username}'s HP: {User1HP}\nMonster's HP: {EnemeyHP}");

            {
                for (var i = 0; i < 1000; i++)
                {
                    var interactiveMessage = new InteractiveMessageBuilder("")
                        .EnableLoop()
                        .WithCancellationWord("stop")
                        .SetWrongResponseMessage("just say `attack`!")
                         .ListenUsers(Context.User)
                        .SetOptions("attack")
                        .Build();

                    var response = await StartInteractiveMessage(interactiveMessage);
                    if (response != null)
                    {
                        if (WhoTurnItis == 0)
                        {
                            int damage = rnd.Next(12, 18);
                            int NewHP = EnemeyHP - damage;
                            EnemeyHP = NewHP;
                            if (NewHP <= 0)
                            {
                                await msg.ModifyAsync(x => x.Content = $"{Context.User.Username} has won from Monster with {User1HP} HP left!");
                                await response.DeleteAsync();
                                return;
                            }
                            else
                            {
                                WhoTurnItis = 1;
                                await msg.ModifyAsync(x => x.Content = $"{Context.User.Username} VS Monster\nIt's Monster's turn\n{Context.User.Username}'s HP: {User1HP}\nMonster's HP: {NewHP}");
                            }

                            await Task.Delay(400);
                            await response.DeleteAsync();

                            await Task.Delay(3000);

                            int Enemydamage = rnd.Next(12, 18);
                            NewHP = User1HP - Enemydamage;
                            User1HP = NewHP;
                            if (NewHP <= 0)
                            {
                                await msg.ModifyAsync(x => x.Content = $"Monster has won from {Context.User.Username} with {EnemeyHP} HP left!");
                                await response.DeleteAsync();
                                return;
                            }
                            else
                            {
                                WhoTurnItis = 0;
                                await msg.ModifyAsync(x => x.Content = $"{Context.User.Username} VS Monster\nIt's {Context.User.Username}'s turn\n{Context.User.Username}'s HP: {NewHP}\nMonster's HP: {EnemeyHP}");
                            }
                        }
                        else if (WhoTurnItis == 1)
                        {
                            if (response.Author == Context.User)
                            {
                                await ReplyAndDeleteAsync("It's not your turn.", timeout: TimeSpan.FromSeconds(5));
                            }
                            else
                            {
                                int damage = rnd.Next(12, 18);
                                int NewHP = User1HP - damage;
                                User1HP = NewHP;
                                if (NewHP <= 0)
                                {
                                    await msg.ModifyAsync(x => x.Content = $"Monster has won from {Context.User.Username} with {EnemeyHP} HP left!");
                                    await response.DeleteAsync();
                                    return;
                                }
                                else
                                {
                                    WhoTurnItis = 0;
                                    await msg.ModifyAsync(x => x.Content = $"{Context.User.Username} VS Monster\nIt's {Context.User.Username}'s turn\n{Context.User.Username}'s HP: {NewHP}\nMonster's HP: {EnemeyHP}");
                                }
                            }
                        }
                    }
                }
            }
        }

        /*[Command("next", RunMode = RunMode.Async)]
        public async Task Test_NextMessageAsync()
        {
            var message = await Context.Channel.SendMessageAsync("What is 2+2?");

            for (var i = 0; i < 3; i++)
            {
                var response = await NextMessageAsync();
                if (response != null)
                    await message.ModifyAsync(x => x.Content = $"You replied: {response.Content}");
                else
                    await message.ModifyAsync(x => x.Content = "You did not reply before the timeout");
            }

            await ReplyAsync("you replied more then 3 times");
        }*/
    }
}
