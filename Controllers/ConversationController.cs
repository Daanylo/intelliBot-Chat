using intelliBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace intelliBot.Controllers
{
    public class ConversationController(ILogger<ConversationController> logger, HttpClient httpClient, Bot bot) : Controller
    {
        private readonly ILogger<ConversationController> _logger = logger;
        private readonly OpenAIController _openAIController = new(httpClient, bot);

        public IActionResult Conversation()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProcessTranscript([FromBody] string transcript)
        {
            _logger.LogInformation("Received transcript: {transcript}", transcript);
            string answer = _openAIController.GetAnswer(transcript).Result;
            return Ok(new { answer });
        }
    }
}
