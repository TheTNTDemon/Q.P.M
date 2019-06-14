//System
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

//Helpers
using Q.P.M.Core.UserAccounts;

//Discord
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Q.P.M.Preconditions;

namespace Q.P.M.Modules
{
    public class Starting : InteractiveBase<SocketCommandContext>
    {
        //TODO: Add a full character creation
        [Command("getStarted", RunMode = RunMode.Async), Alias("gs", "start", "get started")]
        [Summary("Important stuff needed to get started")]
        public async Task GettingStartedTask(SocketGuildUser user)
        {
            string file1 = null;
            string file2 = null;
            string file3 = null;

            var acc = UserAccounts.GetAccount(Context.User);

            if (acc.Character1.IsMade == false)
            {
                file1 = "Empty";
            }

            if (acc.Character2.IsMade == false)
            {
                file2 = "Empty";
            }

            if (acc.Character3.IsMade == false)
            {
                file3 = "Empty";
            }
            // Start 
            var channel = await Context.Guild.CreateTextChannelAsync($"Character-Creation");
            var start1 = await channel.SendMessageAsync($"hey {Context.User.Mention}!, I'm Quantum Processing Matrix but call me Q.P.M I'll be your assistent on your adventure!");

            // Ask Profile Slot
            {
                await Task.Delay(TimeSpan.FromSeconds(7));
                await start1.ModifyAsync(a => a.Content = $"Choose a character file to create and start your adventure!\n**1.** `{file1}`\n**2.** `{file2}`\n**3.** `{file3}`");

                var response = await this.NextMessageAsync(true, true, TimeSpan.FromMinutes(5));
                if (response != null)
                {
                    if (response.Content.ToLower().Equals("1"))
                    {
                        await start1.ModifyAsync(a => a.Content = "Alright we are gonna make a new character for you in file 1!");
                        acc.Character1.CurentCharacter = 1;
                        acc.CurrentCharacter = acc.Character1;
                    }
                    else if (response.Content.ToLower().Equals("2"))
                    {
                        if(acc.Character1.IsMade == true)
                        {
                            await start1.ModifyAsync(a => a.Content = "Alright we are gonna make a new character for you in file 2!");
                            acc.Character2.CurentCharacter = 2;
                            acc.CurrentCharacter = acc.Character2;
                        }
                        else
                        {
                            await ReplyAndDeleteAsync("hmmmm maybe try to fill up the first slot yes?", timeout: TimeSpan.FromSeconds(10));
                        }
                    }
                    else if (response.Content.ToLower().Equals("3"))
                    {
                        if (acc.Character1.IsMade == false)
                        {
                            await ReplyAndDeleteAsync("hmmmm maybe try to fill up the first slot yes?", timeout: TimeSpan.FromSeconds(10));
                        }
                        else if (acc.Character2.IsMade == true)
                        {
                            await start1.ModifyAsync(a => a.Content = "Alright we are gonna make a new character for you in file 3!");
                            acc.Character3.CurentCharacter = 3;
                            acc.CurrentCharacter = acc.Character3;
                        }
                        else
                        {
                            await ReplyAndDeleteAsync("hmmmm maybe try to fill up the second slot first yes?", timeout: TimeSpan.FromSeconds(10));
                        }
                    }
                }
                else
                {
                    await start1.ModifyAsync(a => a.Content = "You did not reply in the given time span");
                    await Task.Delay(10000);
                    await channel.DeleteAsync();
                    return;
                }
            }

            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                SocketMessage response2;

                await start1.ModifyAsync(a =>
                {
                    a.Content = "What name would you like?";
                    a.Embed = new EmbedBuilder()
                        .WithTitle("Your character sheet:")
                        .AddField("Name:", "None", true)
                        .AddField("Gender:", "None", true)
                        .AddField("Class:", "None", true)
                        .AddField("Special Class:", "None", true)
                        .Build();
                });

                response2 = await NextMessageAsync(true, true, timeout: TimeSpan.FromMinutes(5));
            }
            // All Done
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                await this.ReplyAndDeleteAsync($"{start1.ModifyAsync(a => a.Content = "You are all set now, you can finally start your adventure!")}", timeout: TimeSpan.FromSeconds(20));
            }
        }
    }
}
