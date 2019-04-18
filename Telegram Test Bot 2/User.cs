using System;
using System.Threading.Tasks;


using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;



namespace ChatBot
{
    class User
    {
        public ChatId ChatId { get; private set; }
        public ChatId ChatId_Reciever { get; private set; }

        public UserStatus Status { get; set; }

        public enum UserStatus { None, InSearch, InChat };


        public User(ChatId chatId)
        {
            ChatId = chatId;
            Status = UserStatus.None;
        }

        public async Task SendMessage(MessageEventArgs e)
        {
            if (ChatId_Reciever != null)
            {
                if (e.Message.Type == MessageType.Text)
                {
                    await Program.m_BotClient.SendTextMessageAsync
                        (chatId: ChatId_Reciever, text: e.Message.Text);
                }
                else if (e.Message.Type == MessageType.Sticker)
                {
                    await Program.m_BotClient.SendStickerAsync
                        (chatId: ChatId_Reciever, sticker: e.Message.Sticker.FileId);
                }
                else if (e.Message.Type == MessageType.Voice)
                {
                    await Program.m_BotClient.SendVoiceAsync
                        (chatId: ChatId_Reciever, voice: e.Message.Voice.FileId);
                }
                else if (e.Message.Type == MessageType.Photo)
                {
                    await Program.m_BotClient.SendPhotoAsync
                        (chatId: ChatId_Reciever, photo: e.Message.Photo[0].FileId, caption: e.Message.Caption);
                }
                else if (e.Message.Type == MessageType.Audio)
                {
                    await Program.m_BotClient.SendAudioAsync
                        (chatId: ChatId_Reciever, audio: e.Message.Audio.FileId, caption: e.Message.Caption);
                }
                else if (e.Message.Type == MessageType.Video)
                {
                    await Program.m_BotClient.SendVideoAsync
                        (chatId: ChatId_Reciever, video: e.Message.Video.FileId, caption: e.Message.Caption);
                }
                else if (e.Message.Type == MessageType.VideoNote)
                {
                    await Program.m_BotClient.SendVideoNoteAsync
                        (chatId: ChatId_Reciever, videoNote: e.Message.VideoNote.FileId);
                }
                else
                {
                    //if reached here, LastMessage type not supported 
                    //respond with 'not supported' LastMessage
                    //print to console
                    await Program.m_BotClient.SendTextMessageAsync(chatId: ChatId, text: $"<code>{e.Message.Type}s not supported yet</code>", parseMode: ParseMode.Html);
                    await Program.m_BotClient.SendTextMessageAsync(chatId: ChatId_Reciever, text: $"<code>{e.Message.Type}s not supported yet</code>", parseMode: ParseMode.Html);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine
                        ("[ERROR]:" +
                        $"\n1st ChatId: {ChatId.Identifier}; 2nd: {ChatId.Identifier}." +
                        $"\nUnsupported Message Type {e.Message.Type}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else
            {
                await Program.m_BotClient.SendTextMessageAsync
                    (chatId: ChatId,
                    text: "You are not in a chat yet\n" +
                    "use /newchat to start a chat");
            }
        }

        public void AddReciever(ChatId chatId)
        {
            ChatId_Reciever = chatId;
            Status = UserStatus.InChat;
        }

        public void RemoveReciever()
        {
            ChatId_Reciever = null;
            Status = UserStatus.None;
        }
    }
}