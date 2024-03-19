using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GomamayoNET;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace GomamayoBot{
    class Program{
        static void Main(string[] args){
            Console.Title = "GomamayoBot";
            Console.WriteLine("Run start GomamayoBot...");
            Discordbot bot = new Discordbot();
            bot.MainAsync().GetAwaiter().GetResult();
        }
    }

        class Discordbot{
            Gomamayo gomamayo = new Gomamayo();
            private DiscordSocketClient _client;
            private CommandService _commands;
            private IServiceProvider _services;

        public async Task MainAsync(){
            DotNetEnv.Env.Load("./.env");
            DiscordSocketConfig config = new DiscordSocketConfig{
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
            };
            _client = new DiscordSocketClient(config);
            _commands = new CommandService();
            _services = new ServiceCollection().BuildServiceProvider();
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: _services);
            _client.Log += LogAsync;
            _client.Ready += onReady;
            _client.MessageReceived += onMessage;
            await _client.LoginAsync(TokenType.Bot, DotNetEnv.Env.GetString("TOKEN"));
            await _client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private Task LogAsync(LogMessage log){
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task onReady(){
            

            Console.WriteLine($"{_client.CurrentUser} is Running!!");


            return Task.CompletedTask;
        }

        private async Task onMessage(SocketMessage _message){
            var message = _message as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message.Author.IsBot) return;
            if (message.Content[0] != '!'&&gomamayo.IsHigherGomamayo(message.Content)){
                await message.Channel.SendMessageAsync($"{message.Content}は高次ゴママヨです。");
            }else if(message.Content[0] != '!'&&gomamayo.IsGomamayo(message.Content)){
                await message.Channel.SendMessageAsync($"{message.Content}はゴママヨです。");
            }
            int argPos = 0;
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;
            var result = await _commands.ExecuteAsync(context: context, argPos: argPos, services: _services);
            if (!result.IsSuccess) await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        public class CommandModule : ModuleBase<SocketCommandContext>{
            Gomamayo gomamayo = new Gomamayo();

            [Command("echo")]
            public async Task EchoAsync([Remainder]string text){
                await ReplyAsync("生存確認！\n"+text);
            }

            [Command("GomamayoCheck")]
            public async Task GomamayoCheck([Remainder]string text){
                if (gomamayo.IsHigherGomamayo(text)){
                    await ReplyAsync($"{text}は高次ゴママヨです。");
                }else if(gomamayo.IsGomamayo(text)){
                    await ReplyAsync($"{text}はゴママヨです。");
                }else{
                    await ReplyAsync($"{text}はゴママヨではありません。");
                }
            }
        }
    }
}