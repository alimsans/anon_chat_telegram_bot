using System;


using Telegram.Bot;
using Telegram.Bot.Types;



namespace Telegram_Test_Bot_2
{
    class User
    {
        public ChatId ChatId { get; set; }

        public UserStatus Status { get; set; }
        public enum UserStatus { None, InSearch, InChat };


        public User(ChatId chatId)
        {
            ChatId = chatId;
            Status = UserStatus.None;
        }
    }
}