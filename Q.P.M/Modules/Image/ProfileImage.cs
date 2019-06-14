using Discord.Commands;
using PuppeteerSharp;
using System.IO;
using System.Threading.Tasks;

namespace Q.P.M.Modules.Image
{
    public class ProfileImage : ModuleBase<SocketCommandContext>
    {
        [Command("prof", RunMode = RunMode.Async)]
        public async Task ProfileImageGen()
        {
            var user = Context.User;

            //Gets information from HTML file
            string htmlstring = File.ReadAllText("HTML/Profile.html");


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
            await page.ScreenshotAsync($"{user.Username}-profile.png");
            //Sends screenshot to discord.
            await Context.Channel.SendFileAsync($"{user.Username}-profile.png");
            //Searches for image and if found will delete it after running entire script.
            if (File.Exists($"{user.Username}-profile.png"))
            {
                File.Delete($"{user.Username}-profile.png");
            }
        }
    }
}

