using intelliBot.Models;
using Microsoft.AspNetCore.Mvc;

namespace intelliBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BotController : ControllerBase
    {
        public const string botId = "cbd878f7-b257-11ef-b66d-005056977546";
        public static Bot? bot;
        public static string? botName;
        public static string? botVoice;
        public static int? botAvatar;
        public static string? botLanguage;
        public static int? botStyle;
        public static int? botMaxTokens;
        public static int? botConvLength;
        public static int? botAnswerLength;
        public static bool? botGreetUser;
        public static bool? botGenerateQr;
        public static bool? botStoreConv;
        public static bool? botRequestReviews;
        public static bool? botIsActive;

        [HttpGet("GetBot")]
        public IActionResult GetBot()
        {
            var response = DataController.GetBot();
            if (response.Data != null)
            {
                bot = response.Data;
                botName = bot.Name;
                botVoice = bot.Voice;
                botAvatar = bot.Avatar;
                botLanguage = bot.Language;
                botStyle = bot.Style;
                botMaxTokens = bot.MaxTokens;
                botConvLength = bot.ConvLength;
                botAnswerLength = bot.AnswerLength;
                botGreetUser = bot.GreetUser;
                botGenerateQr = bot.GenerateQr;
                botStoreConv = bot.StoreConv;
                botRequestReviews = bot.RequestReviews;
                botIsActive = bot.IsActive;
                return Ok(response.Data);
            }
            else
            {
                return StatusCode(500, response.ErrorMessage);
            }
        }
    }
}