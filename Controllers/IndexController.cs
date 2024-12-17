using intelliBot.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace intelliBot.Controllers
{
    public class IndexController(ILogger<IndexController> logger, Bot bot) : Controller
    {
        private readonly ILogger<IndexController> _logger = logger;
        private readonly Bot _bot = bot;

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
            _bot.SetLanguage(selectedLanguage);
            _logger.LogInformation("Language set to: {botLanguage}", _bot.Language);
            return Ok();
        }

    }
}
