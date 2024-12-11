using intelliBot.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace intelliBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Conversation()
        {
            return View();
        }

        public IActionResult Review()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult ProcessTranscript([FromBody] string transcript)
        {
            _logger.LogInformation("Received transcript: " + transcript);
            string answer = OpenAIController.GetAnswer(transcript);
            //TextToSpeech.Speak(answer, "Dutch"); 
            return Ok(new { answer });
        }

        [HttpPost]
        public IActionResult SetLanguage([FromBody] JsonElement request)
        {
            string? selectedLanguage = request.GetProperty("language").GetString();
            if (selectedLanguage == null)
            {
                return BadRequest();
            }
            ContextController.SetLanguage(selectedLanguage);
            _logger.LogInformation("Set language to: " + selectedLanguage);
            _logger.LogInformation("Current context: " + ContextController.GetContext());
            return Ok();
        }

    }
}
