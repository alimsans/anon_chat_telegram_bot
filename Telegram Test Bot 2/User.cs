using System;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;


using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;



namespace ChatBot
{
    [DataContract(Name = "User")]
    public class User : IExtensibleDataObject
    {
        [DataMember]
        public long ChatIdentifier { get; private set; }
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public long ChatIdentifier_Reciever { get; private set; }
        [DataMember]
        public UserStatus Status { get; set; }

        public enum UserStatus { None, InSearch, InChat };


        public User(ChatId chatId)
        {
            ChatIdentifier = chatId.Identifier;
            Name = chatId.Username;
            Status = UserStatus.None;
        }

        public async Task SendMessageAsync(MessageEventArgs e)
        {
            if (ChatIdentifier_Reciever != 0L)
            {
                if (e.Message.Type == MessageType.Text)
                {
                    await Program.m_BotClient.SendTextMessageAsync
                        (chatId: ChatIdentifier_Reciever, text: e.Message.Text);
                }
                else if (e.Message.Type == MessageType.Sticker)
                {
                    await Program.m_BotClient.SendStickerAsync
                        (chatId: ChatIdentifier_Reciever, sticker: e.Message.Sticker.FileId);
                }
                else if (e.Message.Type == MessageType.Voice)
                {
                    await Program.m_BotClient.SendVoiceAsync
                        (chatId: ChatIdentifier_Reciever, voice: e.Message.Voice.FileId);
                }
                else if (e.Message.Type == MessageType.Photo)
                {
                    await Program.m_BotClient.SendPhotoAsync
                        (chatId: ChatIdentifier_Reciever, photo: e.Message.Photo[0].FileId, caption: e.Message.Caption);
                }
                else if (e.Message.Type == MessageType.Audio)
                {
                    await Program.m_BotClient.SendAudioAsync
                        (chatId: ChatIdentifier_Reciever, audio: e.Message.Audio.FileId, caption: e.Message.Caption);
                }
                else if (e.Message.Type == MessageType.Video)
                {
                    await Program.m_BotClient.SendVideoAsync
                        (chatId: ChatIdentifier_Reciever, video: e.Message.Video.FileId, caption: e.Message.Caption);
                }
                else if (e.Message.Type == MessageType.VideoNote)
                {
                    await Program.m_BotClient.SendVideoNoteAsync
                        (chatId: ChatIdentifier_Reciever, videoNote: e.Message.VideoNote.FileId);
                }
                else
                {
                    //if reached here, LastMessage type not supported 
                    //respond with 'not supported' LastMessage
                    //print to console
                    await Program.m_BotClient.SendTextMessageAsync(chatId: ChatIdentifier, text: $"<code>{e.Message.Type}s not supported yet</code>", parseMode: ParseMode.Html);
                    await Program.m_BotClient.SendTextMessageAsync(chatId: ChatIdentifier_Reciever, text: $"<code>{e.Message.Type}s not supported yet</code>", parseMode: ParseMode.Html);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine
                        ("[ERROR]:" +
                        $"\n1st ChatId: {ChatIdentifier}; 2nd: {ChatIdentifier}." +
                        $"\nUnsupported Message Type {e.Message.Type}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else
            {
                await Program.m_BotClient.SendTextMessageAsync
                    (chatId: ChatIdentifier,
                    text: "You are not in a chat yet\n" +
                    "use /newchat to start a chat");
            }
        }

        public void AddReciever(long chatId)
        {
            ChatIdentifier_Reciever = chatId;
            Status = UserStatus.InChat;
        }

        public void RemoveReciever()
        {
            ChatIdentifier_Reciever = 0L;
            Status = UserStatus.None;
        }


        private ExtensionDataObject extensionData_Value;

        public ExtensionDataObject ExtensionData
        {
            get => extensionData_Value;
            set => extensionData_Value = value;
        }
    }
}