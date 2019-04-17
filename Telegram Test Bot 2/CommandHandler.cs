using System.Threading.Tasks;


using Telegram.Bot.Args;
using Telegram.Bot.Types;



namespace ChatBot
{
    class CommandHandler
    {
        public static async Task NewChat(ChatId chatId)
        {
            User i = Program.GetOrCreateAddUser(chatId);
            if (i.ChatId.Identifier == chatId.Identifier)
            {
                if (i.Status == User.UserStatus.InChat)
                {
                    await Program.m_BotClient.SendTextMessageAsync
                        (chatId: chatId,
                        text:
                        "You are already in chat!\n" +
                        "/leavechat to leave the current chat");
                }
                else if (i.Status == User.UserStatus.InSearch)
                {
                    await Program.m_BotClient.SendTextMessageAsync
                        (chatId: chatId,
                        text: "You are already waiting for a chat :)");
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
                            i.AddReciever(j.ChatId);
                            j.AddReciever(i.ChatId);
                            await Program.m_BotClient.SendTextMessageAsync
                                (chatId: i.ChatId, text: "Found a chat for you!\nBe nice!");
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
            User i = Program.GetOrCreateAddUser(chatId);
            foreach (User j in Program.m_Users)
            {
                if (j.ChatId == i.ChatId_Reciever)
                {
                    i.RemoveReciever();
                    j.RemoveReciever();
                    await Program.m_BotClient.SendTextMessageAsync
                        (chatId: i.ChatId,
                        text:
                        "You have left the chat\n" +
                        "/help for all commands");
                    await Program.m_BotClient.SendTextMessageAsync
                        (chatId: j.ChatId,
                        text:
                        "Your companion has left the chat\n" +
                        "/help for all commands");
                    return;
                }
            }
        }

        public static async Task Start(ChatId chatId)
        {
            User i = Program.GetOrCreateAddUser(chatId);
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
