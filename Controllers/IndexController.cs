using intelliBot.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace intelliBot.Controllers
{
    public class IndexController(ILogger<IndexController> logger, Bot bot) : Controller
    {
        private readonly ILogger<IndexController> logger = logger;
        private readonly Bot bot = bot;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage([FromBody] JsonElement request)
        {
            string? selectedLanguage = request.GetProperty("language").GetString();
            if (selectedLanguage == null)
            {
                return BadRequest();
            } 
            bot.SetLanguage(selectedLanguage);
            logger.LogInformation("Language set to: {botLanguage}", bot.Language);
            return Ok();
        }

    }
}
