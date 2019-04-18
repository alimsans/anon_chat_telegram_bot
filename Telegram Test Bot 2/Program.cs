using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;



namespace ChatBot
{
    class Program
    {
        public static ITelegramBotClient m_BotClient;
        public static List<User> m_Users = new List<User>();


        static void Main(string[] args)
        {
            m_BotClient = new TelegramBotClient("806503282:AAGIoImWwxsdWDM1_MJdVl_7GLs75vHuFkA")
            { Timeout = TimeSpan.FromSeconds(30) };

            var me = m_BotClient.GetMeAsync().Result;
            Console.WriteLine
                ($"Bot id is {me.Id}\nId: {me.FirstName}.\nWe're set to go.");


            m_BotClient.OnMessage += Bot_OnMessage;
            m_BotClient.StartReceiving();


            while (true)
                Console.ReadLine();
        }



        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text == "/newchat")
                await CommandHandler.NewChat(e.Message.Chat);
            else if (e.Message.Text == "/leavechat")
                await CommandHandler.Leave(e.Message.Chat);
            else if (e.Message.Text == "/start")
                await CommandHandler.Start(e.Message.Chat);
            ///if reached here, the message is sent to according chat
            else
            {
                User tmp = GetOrCreateAddUser(e.Message.Chat);
                try
                {
                    await tmp.SendMessage(e);
                }
                catch (ApiRequestException)
                {
                    m_Users.Remove(GetOrCreateAddUser(tmp.ChatId_Reciever));
                }
            }
        }

        //Returns a User object from m_Users collection
        public static User GetOrCreateAddUser(ChatId chatId)
        {
            foreach (User i in m_Users)
            {
                if (i.ChatId.Identifier == chatId.Identifier)
                {
                    return i;
                }
            }
            //if reached here, User does not exist
            //creating and adding User to m_Users
            User user = new User(chatId);
            m_Users.Add(user);
            return user;
        }

    }
}