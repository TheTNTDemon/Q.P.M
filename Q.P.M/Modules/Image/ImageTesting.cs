using Discord.Commands;
using PuppeteerSharp;
using Q.P.M.Core.UserAccounts;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Q.P.M.Modules.Image
{
    public class ImageTesting : ModuleBase<SocketCommandContext>
    {
        //TODO: Add more image gen
        [Command("generate", RunMode = RunMode.Async)]
        public async Task GenerateImage()
        {
            var user = Context.User;
            var account = UserAccounts.GetAccount(user);
            var cchar = account.CurrentCharacter;

            var currentLevelXpNeeded = Math.Pow(cchar.Level, 2) * 12;
            var nextLevelXpNeeded = Math.Pow(cchar.Level + 1, 2) * 12;
            var currentLevelXp = cchar.Xp - currentLevelXpNeeded;
            var xpsize = nextLevelXpNeeded - currentLevelXpNeeded;
            var currentPercentage = (cchar.Xp - currentLevelXpNeeded) * 100 / xpsize;

            //Gets information from HTML file
            string htmlstring = File.ReadAllText("HTML/Image.html")
                .Replace("{CHARACTERNAME}", cchar.Name)
                .Replace("{AVATARURL}", user.GetAvatarUrl(size: 64))
                .Replace("{LVL}", cchar.Level.ToString())
                .Replace("{WEAPON BOX}", cchar.EquipedWeapon)
                .Replace("{SHIELD BOX}", cchar.EquipedShield)
                .Replace("{XP}", currentPercentage.ToString());


            //PuppeteerSharp Nuget packet to generate HTML to Image
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                //False: Opens a browser on computer/True: Doesn't open anything.
                Headless = true
            });
            var page = await browser.NewPageAsync();
            //HTML Page Size.
            await page.SetViewportAsync(new ViewPortOptions
            {
                Width = 800,
                Height = 600
            });
            //Sets the HTML code with changes for HTML to Image to take screenshot off.
            await page.SetContentAsync(htmlstring);
            //Add a delay to screenshot. Helps if html hasn't fully loaded images when screenshot is taken.
            await page.WaitForTimeoutAsync(500);
            //Saves screenshot to computer with desired name.
            await page.ScreenshotAsync($"{user.Username}-image.png");
            //Sends screenshot to discord.
            await Context.Channel.SendFileAsync($"{user.Username}-image.png");
            //Searches for image and if found will delete it after running entire script.
            if (File.Exists($"{user.Username}-image.png"))
            {
                File.Delete($"{user.Username}-image.png");
            }
        }
    }
}
