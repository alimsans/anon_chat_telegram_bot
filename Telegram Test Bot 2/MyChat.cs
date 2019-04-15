using System;
using System.Threading.Tasks;


using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;



namespace Telegram_Test_Bot_2
{
    class MyChat
    {
        private static User m_FirstUser;
        private static User m_SecondUser;

        public User ChatIdFirst => m_FirstUser;
        public User ChatIdSecond => m_SecondUser;


        public MyChat(User first, User second)
        {
            m_FirstUser = first;
            m_SecondUser = second;

            m_FirstUser.Status = User.UserStatus.InChat;
            m_SecondUser.Status = User.UserStatus.InChat;
        }

        public bool HasUser(ChatId user)
        {
            if (m_FirstUser.ChatId.Identifier == user.Identifier || m_SecondUser.ChatId.Identifier == user.Identifier)
                return true;
            else
                return false;
        }

        //Send message to the other User
        public async Task SendMessage(MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                if (e.Message.Chat.Id == m_FirstUser.ChatId.Identifier)
                    await Program.m_BotClient.SendTextMessageAsync
                        (chatId: m_SecondUser.ChatId,
                        text: e.Message.Text);
                else
                    await Program.m_BotClient.SendTextMessageAsync
                        (chatId: m_FirstUser.ChatId,
                        text: e.Message.Text);
            }
            else
            {
                await Program.m_BotClient.SendTextMessageAsync(chatId: m_FirstUser.ChatId, text: $"<code>{e.Message.Type}s not supported yet</code>", parseMode: ParseMode.Html);
                await Program.m_BotClient.SendTextMessageAsync(chatId: m_SecondUser.ChatId, text: $"<code>{e.Message.Type}s not supported yet</code>", parseMode: ParseMode.Html);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine
                    ("[ERROR]:" +
                    $"\n1st ChatId: {m_FirstUser.ChatId}; 2nd: {m_SecondUser.ChatId}." +
                    $"\nUnsupported Message Type {e.Message.Type}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}