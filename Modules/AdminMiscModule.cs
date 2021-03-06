﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;
using static JackBotV2.Program;
using static JackBotV2.Services.GeneralService;

namespace JackBotV2.Modules
{
    public class AdminMiscModule : ModuleBase<ICommandContext>
    {
        /*
        int min = 60000; //One min is 60,000 ms
        int halfMin = 30000; //Thirty sec is 30,000 ms
        int hour = 3600000; //One hour is 3.6M ms
        int halfHour = 1800000; //30 min is 1.8M ms
        */
        DiscordSocketClient _client = new DiscordSocketClient(new DiscordSocketConfig { WebSocketProvider = WS4NetProvider.Instance });
        
        List<string> gnights = new List<string>();

        List<string> awokes = new List<string>();

        List<string> goodbyes = new List<string>();

        List<string> nos = new List<string>();

        [Command("sleep"), RequireOwner]
        public async Task SleepAsync(int duration)
        {
            duration = duration * 1000;

            await listRead(gnights, "../gnights.txt");
            await listRead(awokes, "../awokes.txt");

            Random rnd1 = new Random();
            int gnightBuffer = rnd1.Next(gnights.Count);

            Random rnd2 = new Random();
            int awokeBuffer = rnd2.Next(awokes.Count);

            await ReplyAsync(gnights[gnightBuffer]);

            Console.WriteLine($"Waiting {duration} ms");

            await _client.SetGameAsync("Nap Simulator 2018");

            await Task.Delay(duration);

            await ReplyAsync(awokes[awokeBuffer]);

            await _client.SetGameAsync("Darkest Dungeon");
        }


        [Command("logoff"), RequireOwner]
        public async Task LogoffAsync()
        {
            await listRead(goodbyes, "../goodbyes.txt");

            Random rnd1 = new Random();
            int goodbyeBuffer = rnd1.Next(goodbyes.Count);

            await ReplyAsync(goodbyes[goodbyeBuffer]);

            await _client.SetStatusAsync(UserStatus.Offline);

            await Context.Client.StopAsync();

            await Task.Delay(1000); // Wait for a second for the API to do its thing

            System.Environment.Exit(1); // Exit bot app successfully
        }

        [Command("removequote"), RequireOwner]
        public async Task addQuoteAsync([Remainder]string userMessage)
        {
            quotes = await listRead(quotes, "../quotes.txt");
            if (!quotes.Contains(userMessage))
            {
                await ReplyAsync("That quote does not exist.");
            }
            else
            {
                quotes.Remove(userMessage);
                Console.WriteLine($"\t[!] New quote: {userMessage}");
                await listUpdate(quotes, "../quotes.txt");
                await ReplyAsync($"Removed '{userMessage}'");
            }
            quotes.Clear();
        }

        [Command("purge", RunMode = RunMode.Async), RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task purgeCmd(int amount)
        {
            var items = await Context.Channel.GetMessagesAsync(amount + 1).Flatten();
            await Context.Channel.DeleteMessagesAsync(items);
            using (var sequenceEnum = items.GetEnumerator())
            {
                while (sequenceEnum.MoveNext())
                {

                    Console.WriteLine(sequenceEnum.Current);
                }
            }
            Console.WriteLine(items);
        }

    }
}