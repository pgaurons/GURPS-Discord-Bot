using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Gao.Gurps.Discord.Workflow;
using Gao.Gurps.Utility;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Gao.Gurps.Discord
{

    class Program
    {
#if DEBUG
        const bool _isDebug = true;
#else
        const bool _isDebug = false;
#endif
        //https://discord.com/oauth2/authorize?client_id=242855270076645377&permissions=274877941824&scope=bot
        public static string Token;
        private readonly DiscordSocketClient _client;
        // Keep the CommandService and IServiceCollection around for use with commands.
        private readonly IServiceCollection _map = new ServiceCollection();
        private readonly CommandService _commands = new CommandService();
        private InteractionService _interactionService;
        private IServiceProvider _services;
        static void Main(string[] args)
        {
            SetupConfiguration();
            new Program().MainAsync().GetAwaiter().GetResult();

        }

        private static void SetupConfiguration()
        {
            var value = Environment.GetEnvironmentVariable("GGDAuthenticationToken");
            Token = value;
        }


        private Program()
        {
            Dice.Roller.BotStartupTime = DateTimeOffset.Now;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                // How much logging do you want to see?
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.AllUnprivileged & 
                ~(GatewayIntents.GuildScheduledEvents | GatewayIntents.GuildInvites)    //Everything but scheduled events and invites.

            });
        }
        private async Task MainAsync()
        {
            // Subscribe the logging handler.
            _client.Log += Logger;

            // Centralize the logic for commands into a seperate method.
            await InitCommands();

            _client.Ready += RegisterSlashCommands;

            // Login and connect.
            await _client.LoginAsync(TokenType.Bot, Token);
            await _client.StartAsync();
            await _client.SetGameAsync("\\help and \\dont-sue-me for help and contact info.", ConfigurationManager.Configuration["Gao.Gurps.Discord.MyWebsite"], ActivityType.Competing);

            // Wait infinitely so your bot actually stays connected.
            await Task.Delay(-1);


        }

        private async Task RegisterSlashCommands()
        {
#if DEBUG
            await _interactionService.RegisterCommandsToGuildAsync(999420421222977677);
#else
            await _interactionService.RegisterCommandsGloballyAsync();
#endif
        }

        private async Task InitCommands()
        {
            _map.AddSingleton(_commands);

            _interactionService = new InteractionService(_client.Rest);
            _map.AddSingleton(_interactionService);

            // When all your required services are in the collection, build the container.
            // Tip: There's an overload taking in a 'validateScopes' bool to make sure
            // you haven't made any mistakes in your dependency graph.
            _services = _map.BuildServiceProvider();

            //This class includes a few methods that were too difficult to port to slash commands that I keep
            //for reference.
            //await _commands.AddModuleAsync(typeof(HelpModule), _services);

            await _interactionService.AddModuleAsync(typeof(Slash.OverheadModule), _services);
            await _interactionService.AddModuleAsync(typeof(Slash.RandomModule), _services);
            await _interactionService.AddModuleAsync(typeof(Slash.CalculationModule), _services);
            await _interactionService.AddModuleAsync(typeof(Slash.DiceRollingModule), _services);

            // Subscribe a handler to see if a message invokes a command.
            _client.SlashCommandExecuted += HandleSlashCommandAsync;
        }

        private async Task HandleSlashCommandAsync(SocketSlashCommand arg)
        {
            if (GuildBlockUtility.IsBlocked(arg.Channel as SocketGuildChannel))
            {
                await arg.RespondAsync("👎");
                return;
            }
            if (UserBlockUtility.IsBlocked(arg.User.Id))
            {
                await arg.RespondAsync("👎");
                return;
            }
            var result = await _interactionService.ExecuteCommandAsync(new InteractionContext(_client, arg), _services);
            if (result.IsSuccess)
            {
                Dice.Roller.AddDailyUser(arg.User.Id);
            }

        }

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


    }
}
