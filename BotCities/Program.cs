using System;
using Telegram.Bot;
using System.Collections.Generic;
using System.IO;

namespace BotCities
{
    class Program
    {
        //Отче наш, Иже еси на небесе́х!
        //Да святится имя Твое, да прии́дет Царствие Твое,
        //да будет воля Твоя, яко на небеси́ и на земли́.
        //Хлеб наш насущный да́ждь нам дне́сь;
        //и оста́ви нам до́лги наша, якоже и мы оставляем должнико́м нашим;
        //и не введи нас во искушение, но изба́ви нас от лукаваго
        [Obsolete]
        static void Main(string[] args)
        {
            string token = "-------";
            TelegramBotClient client = new TelegramBotClient(token);
            Console.WriteLine(client.GetMeAsync().Result);
            string filepath = @"E:\python_нически\С#--\BotCities\BotCities\Cities.txt";
            FileStream file1 = new FileStream(filepath, FileMode.Open); //создаем файловый поток
            StreamReader reader = new StreamReader(file1); // создаем «потоковый читатель» и связываем его с файловым потоком
            Dictionary<string, List<string>> firstLett = new();
            List<string> list = new();
            string str;
            
            string key = " ";
            Console.WriteLine(list.Count);
            while (!reader.EndOfStream)
            {
                str = reader.ReadLine();
                if (str != "")
                {
                    if (str.Length > 1)
                    {
                        list.Add(str.Substring(0,str.IndexOf(" ")));
                    }
                    else
                    {
                        if (list.Count >= 1)
                        {
                            firstLett[key] = new List<string>(list);
                            key = str;
                            firstLett.Add(str, new List<string>());
                            list.Clear();
                        }
                        else
                        {
                            key = str;
                            firstLett.Add(str, new List<string>());
                        }
                    }
                }
            }
            firstLett[key] = list;
            //foreach (KeyValuePair<string, List<string>> kvp in firstLett)
            //{
            //    Console.WriteLine(kvp.Key,":\n");
            //    for (int i = 0; i < kvp.Value.Count; i++)
            //    {
            //        Console.Write(kvp.Value[i],",");
            //    }
            //    Console.WriteLine(":\n");
            //}

            bool onGame = false;
            bool firstTurn = false;
            string saveMsgLastLett = "";
            client.OnMessage += (s, args) =>
            {
                string msg = args.Message.Text;
                Console.WriteLine($"{args.Message.Chat.FirstName} : {msg}");
                string sendMessage = "Напишите /help ";
                if (msg == "/help")
                {
                    sendMessage = "/help - описание существующих команд и что они делают \n " +
                    "/start game (/start) - начать игру \n" +
                    "/end game (/end)- завершить игру";
                }
                
                if ((msg == "/end game" || msg == "/end") && onGame ) { onGame = false; sendMessage = "Игра закончена ψ(▼へ▼メ)～";}


                if (onGame)
                {   string firstLetter = Convert.ToString(msg[0]).ToUpper();
                    Console.WriteLine(saveMsgLastLett," = ", firstLetter);
                    if ((saveMsgLastLett == firstLetter || firstTurn) && firstLett[firstLetter].Contains(msg))
                    {
                        List<string> used = new();
                        string lastLetter = Convert.ToString(msg[msg.Length - 1]).ToUpper();
                        if (!firstLett.ContainsKey(lastLetter))
                        {
                            lastLetter = Convert.ToString(msg[msg.Length - 2]).ToUpper();
                        }
                        List<string> pool = firstLett[lastLetter];
                        Random rnd = new();
                        sendMessage = pool[rnd.Next(0, pool.Count)];
                        saveMsgLastLett = Convert.ToString(sendMessage[sendMessage.Length - 1]).ToUpper();
                        if (!firstLett.ContainsKey(lastLetter))
                        {
                            saveMsgLastLett = Convert.ToString(msg[msg.Length - 2]).ToUpper();
                        }
                    }
                    else
                    {
                        sendMessage = "Ощибочка. Пишите сушествующие города и с заглавной буквы!";
                    }
                }
                if (firstTurn) { firstTurn = false; }
                if (msg == "/start game" || msg == "/start") { onGame = true; sendMessage = "Игра начилась!! ψ(▼へ▼メ)～"; firstTurn = true; }
                
                client.SendTextMessageAsync(
                      chatId: args.Message.Chat.Id,
                      text: sendMessage,
                      replyToMessageId: args.Message.MessageId

                      );

            };
            client.StartReceiving();
            Console.ReadKey();

        }

    }
}
