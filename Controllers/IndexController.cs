using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using intelliBot.Models;
using System.Text;

namespace intelliBot.Controllers
{
    public class IndexController(ILogger<IndexController> logger, IConfiguration configuration, HttpClient httpClient) : Controller
    {
        private readonly ILogger<IndexController> logger = logger;
        private readonly IConfiguration configuration = configuration;
        private readonly HttpClient httpClient = httpClient;


        public IActionResult Index()
        {
            string? botId = HttpContext.Session.GetString("BotId");
            string? isBot = HttpContext.Session.GetString("IsBot");
            if (botId == null)
            {
                return RedirectToAction("Index", "Unauthorized");
            }
            HttpContext.Session.Clear();
            HttpContext.Session.SetString("BotId", botId);
            if (isBot != null)
            {
                HttpContext.Session.SetString("IsBot", isBot);
            }
            logger.LogInformation("BotId set to: {botId}", botId);
            return View();
        }

        public async Task<IActionResult> ReportStatus()
        {
            if (HttpContext.Session.GetString("IsBot") != "true")
            {
                return BadRequest();
            }
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{configuration["intelliGuide:ApiAddress"]}/api/bot/{HttpContext.Session.GetString("BotId")}/status");
            request.Headers.Add("x-api-key", configuration["intelliGuide:ApiKey"]);
            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            } else
            {
                return BadRequest();
            }
        }

        public new IActionResult Unauthorized()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitHelp([FromBody] JsonElement jsonElement)
        {
            var message = jsonElement;
            var botId = HttpContext.Session.GetString("BotId");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{configuration["intelliGuide:ApiAddress"]}/api/help/{botId}")
            {
                Content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("x-api-key", configuration["intelliGuide:ApiKey"]);

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return Ok( new { message = "Help request submitted successfully" });
            } else
            {
                return BadRequest();
            }
            
        }

        [HttpPost]
        public IActionResult SetLanguage([FromBody] JsonElement request)
        {
            string? selectedLanguage = request.GetProperty("language").GetString();
            if (selectedLanguage == null)
            {
                return BadRequest();
            } 
            HttpContext.Session.SetString("BotLanguage", selectedLanguage);
            logger.LogInformation("Language set to: {selectedLanguage}", selectedLanguage);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetLanguage()
        {
            string? language = HttpContext.Session.GetString("BotLanguage");
            if (language == null)
            {
                return BadRequest();
            }
            return Ok(new { language });
        }

        public IActionResult IsBot()
        {
            string? isBot = HttpContext.Session.GetString("IsBot");
            if (isBot == null || isBot != "true")
            {
                return Ok(new { isBot = false });
            }
            return Ok(new { isBot = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthToken()
        {
            string? isBot = HttpContext.Session.GetString("IsBot");
            string? botId = HttpContext.Session.GetString("BotId");
            if (isBot == null || isBot != "true")
            {
                return Unauthorized();
            }
            var request = new HttpRequestMessage(HttpMethod.Get, $"{configuration["intelliGuide:ApiAddress"]}/api/auth/{botId}");
            request.Headers.Add("x-api-key", configuration["intelliGuide:ApiKey"]);
            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(jsonResponse);
                document.RootElement.TryGetProperty("authToken", out var authToken);
                return Ok(new { authToken = authToken.GetString() });
            }
            else
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> GetBot()
        {
            string? botId = HttpContext.Session.GetString("BotId");
            if (botId == null)
            {
                return BadRequest();
            }
            BotController botController = new(new Logger<BotController>(new LoggerFactory()), httpClient, configuration);
            Bot? bot = await botController.GetBot(botId);
            if (bot == null)
            {
                return RedirectToAction("Index", "Unauthorized");
            }
            return Ok(bot);
        }
    }
}
