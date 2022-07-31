using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Gao.Gurps.Discord.Model;
using Gao.Gurps.Utility;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Gao.Gurps.Discord.ParterService
{
    class Program
    {
        public static string Token;

        private readonly DiscordSocketClient _client;
        // Keep the CommandService and IServiceCollection around for use with commands.
        private readonly IServiceCollection _map = new ServiceCollection();
        private readonly CommandService _commands = new CommandService();
        private IServiceProvider _services;
        static void Main(string[] args)
        {
            SetupConfiguration();
            new Program().MainAsync().GetAwaiter().GetResult();

        }

        private async Task MainAsync()
        {
            // Subscribe the logging handler.
            _client.Log += Logger;

            // Centralize the logic for commands into a seperate method.
            await InitCommands();

            // Login and connect.
            await _client.LoginAsync(TokenType.Bot, Token);
            await _client.StartAsync();
            //_client.Connected += RunDroppingEvent;
            await Task.Delay(5000, new System.Threading.CancellationToken());//is 5 seconds enough to connect?
            DropOldGuilds();
            await Task.Delay(-1);
        }


        private void DropOldGuilds()
        {
            var guilds = _client.Guilds.
                    Select(g=> new GuildLastUsed 
                    {
                        GuildId = g.Id, 
                        LastUsed= DateTime.UtcNow, 
                        OwnerId = g.Owner.Id 
                    }).
                    ToList();

            //flippity floppity.... I need to do something here.... serialize this to a file.
            var serializer = new XmlSerializer(typeof(List<GuildLastUsed>));
            using var fileStream = new FileStream("fuckme.xml", FileMode.Create);
            //Console.WriteLine($"Saving Data to file {_fileName} - {guildId} : {prefix}");
            serializer.Serialize(fileStream, guilds);

            int uniqueOwnerCount = guilds.Select(g => g.OwnerId).Distinct().Count();
            var guildsPerOwner = guilds.GroupBy(g => g.OwnerId).Select(g => g.Count());
            Console.WriteLine(@$"Interesting facts.
Number of guilds: {guilds.Count}
Number of unique owners: {uniqueOwnerCount}
Average number of guilds per owner:{guildsPerOwner.Average()}
Maximum number of guilds per owner:{guildsPerOwner.Max()}
Number of users that have more than one guild:{guildsPerOwner.Count(i => i>1)}");

            Console.ReadKey();
            Environment.Exit(0);
        }

        private async Task InitCommands()
        {

            _map.AddSingleton(_commands);

            _services = _map.BuildServiceProvider();

            //await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private static void SetupConfiguration()
        {

            Token = ConfigurationManager.Configuration["Gao.Gurps.Discord.AuthenticationToken"];
        }




        // Example of a logging handler. This can be re-used by addons
        // that ask for a Func<LogMessage, Task>.
        private static Task Logger(LogMessage message)
        {

            var cc = Console.ForegroundColor;
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19:u} [{message.Severity,8}] {message.Source}: {message.Message}");
            Console.ForegroundColor = cc;

            return Task.CompletedTask;
        }

        private Program()
        {

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                // How much logging do you want to see?
                LogLevel = LogSeverity.Info,

                // If you or another service needs to do anything with messages
                // (eg. checking Reactions, checking the content of edited/deleted messages),
                // you must set the MessageCacheSize. You may adjust the number as needed.
                //MessageCacheSize = 50,

                // If your platform doesn't have native websockets,
                // add Discord.Net.Providers.WS4Net from NuGet,
                // add the `using` at the top, and uncomment this line:
                //WebSocketProvider = WS4NetProvider.Instance
            });
        }
    }
}
