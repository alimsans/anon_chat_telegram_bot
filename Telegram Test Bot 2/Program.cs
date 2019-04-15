using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;



namespace ChatBot
{
    class Program
    {
        public static ITelegramBotClient m_BotClient;
        public static List<User> m_Users = new List<User>();
        public static List<MyChat> m_Chats = new List<MyChat>();


        static void Main(string[] args)
        {
            m_BotClient = new TelegramBotClient("806503282:AAGIoImWwxsdWDM1_MJdVl_7GLs75vHuFkA")
            { Timeout = TimeSpan.FromSeconds(30) };

            var me = m_BotClient.GetMeAsync().Result;
            Console.WriteLine
                ($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.\nWe're set to go.");


            m_BotClient.OnMessage += Bot_OnMessage;
            m_BotClient.StartReceiving();


            while (true)
                Console.ReadLine();
        }



        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            ///Check if the user is already in chat
            ///if not, add to Chats collection
            if (e.Message.Text == "/newchat")
            {
                await CommandHandler.NewChat(e.Message.Chat);
                return;
            }
            else if (e.Message.Text == "/leave")
            {
                await CommandHandler.Leave(e.Message.Chat);
                return;
            }
            ///Check if user is already registered
            ///if not, add to Users collection
            else if (e.Message.Text == "/start")
            {
                await CommandHandler.Start(e.Message.Chat);
                return;
            }
            ///if reached here, the message is sent to according chat
            {
                foreach (MyChat i in m_Chats)
                {
                    if (i.HasUser(e.Message.Chat.Id))
                    {
                        await i.SendMessage(e);
                        return;
                    }
                }
            }
        }

        //Returns a User object from m_Users collection
        public static User GetUser(ChatId chatId)
        {
            foreach (User i in m_Users)
            {
                if (i.ChatId.Identifier == chatId.Identifier)
                {
                    return i;
                }
            }

            User user = new User(chatId);
            m_Users.Add(user);
            return user;
        }
    }
}