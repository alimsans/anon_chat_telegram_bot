using System.Threading.Tasks;


using Telegram.Bot.Args;
using Telegram.Bot.Types;



namespace ChatBot
{
    class CommandHandler
    {
        public static async Task NewChat(ChatId chatId)
        {
            User i = Program.GetUser(chatId);
            if (i.ChatId.Identifier == chatId.Identifier)
            {
                if (i.Status == User.UserStatus.InChat)
                {
                    await Program.m_BotClient.SendTextMessageAsync
                        (chatId: chatId,
                        text:
                        "You are already in chat!\n" +
                        "/leavechat to leave the current chat");
                    return;
                }
                else if (i.Status == User.UserStatus.InSearch)
                {
                    await Program.m_BotClient.SendTextMessageAsync
                        (chatId: chatId,
                        text: "You are already waiting for a chat :)");
                    return;
                }
                else
                {
                    i.Status = User.UserStatus.InSearch;
                    await Program.m_BotClient.SendTextMessageAsync
                        (chatId: chatId,
                        text: "You're now waiting for a chat :)");
                    foreach (User j in Program.m_Users)
                    {
                        if (j.ChatId.Identifier != i.ChatId.Identifier
                            && j.Status == User.UserStatus.InSearch)
                        {
                            Program.m_Chats.Add(new MyChat(i, j));
                            await Program.m_BotClient.SendTextMessageAsync
                                (chatId: chatId, text: "Found a chat for you!\nBe nice!");
                            await Program.m_BotClient.SendTextMessageAsync
                                (chatId: j.ChatId, text: "Found a chat for you!\nBe nice!");
                            return;
                        }
                    }
                }
            }
        }

        public static async Task Leave(ChatId chatId)
        {
            User i = Program.GetUser(chatId);
            for (int j = 0; j < Program.m_Chats.Count - 1; j++)
            {
                if (Program.m_Chats[j].HasUser(i.ChatId))
                {
                    Program.m_Chats.RemoveAt(j);
                    Program.m_Chats[j].ChatIdFirst.Status = User.UserStatus.None;
                    Program.m_Chats[j].ChatIdSecond.Status = User.UserStatus.None;
                    await Program.m_BotClient.SendTextMessageAsync
                        (chatId: chatId,
                        text:
                        "You left the chat\n" +
                        "/help for all commands");
                    await Program.m_BotClient.SendTextMessageAsync
                        (chatId: i.ChatId,
                        text:
                        "Your companion left the chat\n" +
                        "/help for all commands");
                    return;
                }
            }
        }

        public static async Task Start(ChatId chatId)
        {
            User i = Program.GetUser(chatId.Identifier);
            if (i.ChatId.Identifier == chatId.Identifier)
            {
                await Program.m_BotClient.SendTextMessageAsync
                    (chatId: chatId,
                    text:
                    "You are already registered!\n" +
                    "/newchat to start a new chat");
                return;
            }

            ///if reached here, user is unregistered
            Program.m_Users.Add(new User(chatId));
            Program.m_Users[Program.m_Users.Count - 1].Status = User.UserStatus.None;
            await Program.m_BotClient.SendTextMessageAsync
                (chatId: chatId,
                text:
                "Hello\n" +
                "/newchat to start a new chat\n" +
                "/leavechat to leave your current chat");
            return;
        }
    }
}
