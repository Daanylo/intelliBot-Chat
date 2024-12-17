using intelliBot.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;

namespace intelliBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BotController : ControllerBase
    {
        private readonly HttpClient httpClient;
        private readonly Bot _bot;
        private readonly ILogger<BotController> _logger;

        public BotController(ILogger<BotController> logger, HttpClient httpClient, Bot bot)
        {
            _logger = logger;
            this.httpClient = httpClient;
            _bot = bot;
            InitializeBot().Wait();
        }

        [HttpPost("initialize")]
        public async Task<IActionResult> InitializeBot()
        {
            _logger.LogInformation("Initializing bot...");
            await CreateBot();
            _logger.LogInformation("Bot initialized. Name:" + _bot.Name);
            return Ok(new { message = "Bot initialized successfully", data = _bot.Name });
        }

        private async Task CreateBot()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://{Environment.GetEnvironmentVariable("API_ADDRESS")}/api/bot/{Environment.GetEnvironmentVariable("BOT_ID")}");
            request.Headers.Add("x-api-key", Environment.GetEnvironmentVariable("INTELLIGUIDE_API_KEY"));

            var response = await httpClient.SendAsync(request);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Response content: {jsonResponse}", jsonResponse);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    // Check if the response content is empty
                    if (string.IsNullOrWhiteSpace(jsonResponse))
                    {
                        _logger.LogError("Response content is empty.");
                        return;
                    }

                    using var document = JsonDocument.Parse(jsonResponse);
                    var root = document.RootElement;
                    _logger.LogInformation("Parsed JSON successfully.");

                    if (root.TryGetProperty("name", out var name) &&
                        root.TryGetProperty("avatar", out var avatar) &&
                        root.TryGetProperty("style", out var style) &&
                        root.TryGetProperty("location", out var location))
                    {
                        _bot.Name = name.GetString();
                        _bot.Avatar = avatar.GetString();
                        _bot.Style = Enum.TryParse<SpeechStyle>(style.GetString(), out var parsedStyle) ? parsedStyle : (SpeechStyle?)null;
                        _bot.Location = location.GetString();
                        _bot.SetLanguage("Nederlands");
                    }
                    else
                    {
                        _logger.LogError("Required properties are missing in the JSON response.");
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to parse JSON response: {jsonResponse}", jsonResponse);
                }
            }
            else
            {
                _logger.LogError("Request failed with status code: {StatusCode}", response.StatusCode);
            }
        }


    }
}
