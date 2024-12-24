using intelliBot.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using System.Configuration;

namespace intelliBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BotController(ILogger<BotController> logger, HttpClient httpClient, Bot bot, IConfiguration configuration) : ControllerBase
    {
        private readonly HttpClient httpClient = httpClient;
        private readonly Bot bot = bot;
        private readonly ILogger<BotController> logger = logger;

        [HttpPost("initialize")]
        public async Task<IActionResult> InitializeBot()
        {
            logger.LogInformation("Initializing bot...");
            await CreateBot();
            logger.LogInformation("Bot initialized. Name:" + bot.Name);
            return Ok(new { message = "Bot initialized successfully", data = bot.Name });
        }

        private async Task CreateBot()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{configuration["intelliGuide:ApiAddress"]}/api/bot/{configuration["intelliGuide:BotId"]}");
            request.Headers.Add("x-api-key", configuration["intelliGuide:ApiKey"]);

            var response = await httpClient.SendAsync(request);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            logger.LogInformation("Response content: {jsonResponse}", jsonResponse);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(jsonResponse))
                    {
                        logger.LogError("Response content is empty.");
                        return;
                    }

                    using var document = JsonDocument.Parse(jsonResponse);
                    var root = document.RootElement;
                    logger.LogInformation("Parsed JSON successfully.");

                    if (root.TryGetProperty("name", out var name) &&
                        root.TryGetProperty("avatar", out var avatar) &&
                        root.TryGetProperty("style", out var style) &&
                        root.TryGetProperty("location", out var location))
                    {
                        bot.Name = name.GetString();
                        bot.Avatar = avatar.GetString();
                        bot.Style = Enum.TryParse<SpeechStyle>(style.GetString(), out var parsedStyle) ? parsedStyle : (SpeechStyle?)null;
                        bot.Location = location.GetString();
                        bot.SetLanguage("Nederlands");
                    }
                    else
                    {
                        logger.LogError("Required properties are missing in the JSON response.");
                    }
                }
                catch (JsonException ex)
                {
                    logger.LogError(ex, "Failed to parse JSON response: {jsonResponse}", jsonResponse);
                }
            }
            else
            {
                logger.LogError("Request failed with status code: {StatusCode}", response.StatusCode);
            }
        }


    }
}
