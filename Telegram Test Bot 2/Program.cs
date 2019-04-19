using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;


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
                ($"Bot id is {me.Id}\nName: {me.FirstName} {me.LastName}\n{me.Username}.\nWe're set to go.");

            ReadXML();

            m_BotClient.OnMessage += Bot_OnMessage;
            m_BotClient.StartReceiving();



            while (true)
            {
                string cmd = Console.ReadLine();
                if (cmd.ToLower() == "serialize" || cmd.ToLower() == "save")
                    ConsoleHandler.Serialize();
                else if(cmd.ToLower() == "serialize close" || cmd.ToLower() == "save close")
                {
                    ConsoleHandler.Serialize();
                    return;
                }
            }
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
                    await tmp.SendMessageAsync(e);
                }
                catch (ApiRequestException)
                {
                    m_Users.Remove(GetOrCreateAddUser(tmp.ChatIdentifier_Reciever));
                    tmp.RemoveReciever();
                }
            }
        }

        ///<summary>Returns a User object from m_Users collection
        ///or creates one and adds to m_Users list</summary> 
        public static User GetOrCreateAddUser(ChatId chatId)
        {
            foreach (User i in m_Users)
            {
                if (i.ChatIdentifier == chatId.Identifier)
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

        public static void WriteXML()
        {
            Console.WriteLine("Starting to serialize users data.");

            FileStream file = new FileStream("DataUsers.xml", FileMode.Create);
            DataContractSerializer serializer = new DataContractSerializer(typeof(List<User>));

            serializer.WriteObject(file, m_Users);
            file.Close();

            Console.WriteLine("Finished to serialize users data.");
        }

        public static void ReadXML()
        {
            Console.WriteLine("Starting to deserialize users data.");
            FileStream file = null;
            try
            {
                file = new FileStream("DataUsers.xml", FileMode.Open);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("No users data found.");
                return;
            }

            XmlDictionaryReader reader = 
                XmlDictionaryReader.CreateTextReader(file, new XmlDictionaryReaderQuotas());
            DataContractSerializer serializer = new DataContractSerializer(typeof(List<User>));

            m_Users = (List<User>)serializer.ReadObject(reader, true);
            
            reader.Close();
            file.Close();

            Console.WriteLine("Finished to deserialize users data.");
        }
    }
}