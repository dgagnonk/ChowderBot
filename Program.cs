using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using System.IO;

namespace ChowderBot
{
    class Program
    {

        private string[] _8ballanswers =
        {
            "no", "yes", "i don't speak spanish, sorry", "probably", "maybe", "best recourse is to probably have a bowl of chowder", "latin is a dead language, stop using it", "i don't like that question ask me a better one", "i see your lips moving but all i hear is poop plopping on the floor", "absolutely", "definitely", "it's unlikely", "si senor", "ask me again when you're not drunk", "consult the clams", "i'm 99.9% sure", "my gut tells me no"
        };

        private Random random = new Random();
        private static string token = "";
        private DiscordSocketClient client;

        public static void Main(string[] args)
        {
            token = File.ReadAllText("./token.txt");
            new Program().MainAsync().GetAwaiter().GetResult();
        }
            

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            client.Log += Log;
            client.MessageReceived += Client_MessageReceived;

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            Console.WriteLine("Working directory: " + GetCurrentDir());


            // Block this task until the program is closed.
            await Task.Delay(-1);
        }



        private async Task Client_MessageReceived(SocketMessage arg)
        {

            if (arg.Content.StartsWith("!random"))
            {
                Console.WriteLine("!random issued by " + arg.Author + ": " + arg.Content);

                string[] args = arg.Content.Split(' ');

                int min = 0, max = 1;

                if (args.Length > 3)
                {
                    await arg.Channel.SendMessageAsync("What are you doing? That's not how you do that. Type !help or !howto.");
                    return;
                }

                int r;

                if (args.Length == 0)
                {
                    r = (new Random()).Next();
                }

                if (!Int32.TryParse(args[1], out min))
                {
                    await arg.Channel.SendMessageAsync("That max or min isn't a proper number");
                    return;
                }

                if (args.Length == 3)
                {
                    if (!Int32.TryParse(args[2], out max))
                    {
                        await arg.Channel.SendMessageAsync("That max or min isn't a proper number");
                        return;
                    }

                    r = (new Random()).Next(min, max);
                }
                else if (args.Length == 2)
                {
                    r = (new Random().Next(min));
                }
                else
                {
                    r = (new Random()).Next();
                }

                await arg.Channel.SendMessageAsync(r.ToString());
            }
            else if (arg.Content.StartsWith("!help") || arg.Content.StartsWith("!howto"))
            {
                // how-to goes here

            }
            else if (arg.Content.ToLower().StartsWith("hi chowderbot"))
            {
                await arg.Channel.SendMessageAsync("hi " + arg.Author.Username);
                return;
            }
            else if (arg.Content.ToLower().StartsWith("how are you chowderbot"))
            {
                await arg.Channel.SendMessageAsync("All I know is chowder. Don't make this complicated.");
                return;
            }
            else if (arg.Content.ToLower().StartsWith("show me chowder"))
            {
                Console.WriteLine("Working on sending chowder for " + arg.Author.Username);

                string theChowder = GetRandomChowder(GetCurrentDir() + "chowder\\");

                if (File.Exists(theChowder))
                    await arg.Channel.SendFileAsync(theChowder);
                else
                {
                    Console.WriteLine(theChowder + " not found");
                    await arg.Channel.SendMessageAsync("no");
                }
                return;
            }
            else if (arg.Content.ToLower().StartsWith("good bot"))
            {
                await arg.Channel.SendMessageAsync(":)");
                return;
            }
            else if (arg.Content.ToLower().StartsWith("bad bot"))
            {
                await arg.Channel.SendMessageAsync("you're wrong");
                return;
            }
            else if (arg.Content.ToLower().StartsWith("hey chowderbot,") && arg.Content.EndsWith("?"))
            {
                await arg.Channel.SendMessageAsync(_8ballanswers[random.Next(_8ballanswers.Length)]);
                return;
            } 
            else if (arg.Content.ToLower().StartsWith("what is chowder") || arg.Content.ToLower().StartsWith("what's chowder"))
            {
                await arg.Channel.SendMessageAsync("Chowder is a rich soup typically containing fish, clams, or corn with potatoes and onions.");
                return;
            }
            else if (arg.Content.ToLower().StartsWith("goodnight chowderbot"))
            {
                await arg.Channel.SendMessageAsync("hope you dream of chowder :) goodbye");
                Environment.Exit(1);
            }
        }

        private string GetRandomChowder(string chowderdir)
        {
            string[] chowders = Directory.GetFiles(chowderdir);

            int max = chowders.Length - 1;
            int rand = random.Next(chowders.Length);

            Console.WriteLine("Random chowder max: " + max.ToString());
            Console.WriteLine("Random chowder: " + rand.ToString());

            return chowders[rand];
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private string GetCurrentDir()
        {
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(location) + "\\";
            return directory;
        }
    }
}