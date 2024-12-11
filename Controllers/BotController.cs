using intelliBot.Models;

namespace intelliBot.Controllers
{
    public static class BotController
    {
        public const string botId = "cbd878f7-b257-11ef-b66d-005056977546";
        public static Bot bot = DataController.GetBot();
        public static string botName = bot.Name;
        public static string botVoice = bot.Voice;
        public static int botAvatar = bot.Avatar;
        public static string botLanguage = bot.Language;
        public static int botStyle = bot.Style;
        public static int botMaxTokens = bot.MaxTokens;
        public static int botConvLength = bot.ConvLength;
        public static int botAnswerLength = bot.AnswerLength;
        public static bool botGreetUser = bot.GreetUser;
        public static bool botGenerateQr = bot.GenerateQr;
        public static bool botStoreConv = bot.StoreConv;
        public static bool botRequestReviews = bot.RequestReviews;
        public static bool botIsActive = bot.IsActive;
    }
}
