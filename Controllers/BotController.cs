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
    public class BotController(ILogger<BotController> logger, HttpClient httpClient, IConfiguration configuration) : ControllerBase
    {
        private readonly HttpClient httpClient = httpClient;
        private readonly ILogger<BotController> logger = logger;

        public async Task<Bot?> GetBot(string botId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{configuration["intelliGuide:ApiAddress"]}/api/bot/{botId}");
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
                        return null;
                    }

                    using var document = JsonDocument.Parse(jsonResponse);
                    var root = document.RootElement;
                    logger.LogInformation("Parsed JSON successfully.");

                    if (root.TryGetProperty("bot_id", out var bot_id) &&
                        root.TryGetProperty("user_id", out var user_id) &&
                        root.TryGetProperty("event_id", out var event_id) &&
                        root.TryGetProperty("name", out var name) &&
                        root.TryGetProperty("avatar", out var avatar) &&
                        root.TryGetProperty("style", out var style) &&
                        root.TryGetProperty("voice", out var voice) &&
                        root.TryGetProperty("greeting", out var greeting) &&
                        root.TryGetProperty("location", out var location) &&
                        root.TryGetProperty("status", out var status))
                    {
                        if (string.IsNullOrWhiteSpace(bot_id.GetString()))
                        {
                            logger.LogError("Required properties are missing in the JSON response.");
                            return null;
                        }
                        Bot bot = new()
                        {
                            BotId = botId,
                            UserId = user_id.GetString(),
                            EventId = event_id.GetString(),
                            Name = name.GetString(),
                            Avatar = avatar.GetString(),
                            Style = style.GetString(),
                            Voice = voice.GetString(),
                            Greeting = greeting.GetString(),
                            Location = location.GetString(),
                            Status = status.GetString()
                        };
                        logger.LogInformation("Bot properties set successfully.");
                        return bot;
                    }
                    else
                    {
                        logger.LogError("Required properties are missing in the JSON response.");
                        return null;
                    }
                }
                catch (JsonException ex)
                {
                    logger.LogError(ex, "Failed to parse JSON response: {jsonResponse}", jsonResponse);
                    return null;
                }
            }
            else
            {
                logger.LogError("Request failed with status code: {StatusCode}", response.StatusCode);
                return null;
            }
        }

        public async Task<Event?> GetEvent(string? eventId) 
        {
            if (string.IsNullOrWhiteSpace(eventId))
            {
                return null;
            }
            var apiAddress = configuration["intelliGuide:ApiAddress"];
            var apiKey = configuration["intelliGuide:ApiKey"];

            var request = new HttpRequestMessage(HttpMethod.Get, $"{apiAddress}/api/eventbyid/{eventId}");
            request.Headers.Add("x-api-key", apiKey);

            var response = await httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var _event = JsonSerializer.Deserialize<Event>(result);
                return _event;
            } else {
                return null;
            }
        }
    }
}
