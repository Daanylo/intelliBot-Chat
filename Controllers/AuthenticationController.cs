using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace intelliBot.Controllers 
{

    public class AuthenticationController(HttpClient httpClient, IConfiguration configuration, ILogger<AuthenticationController> logger) : Controller
    {
    private readonly HttpClient httpClient = httpClient;
    private readonly IConfiguration configuration = configuration;
    private readonly ILogger<AuthenticationController> logger = logger;
    public async Task<IActionResult> Authenticate(string authToken)
    {
        if (string.IsNullOrEmpty(authToken))
        {
            return RedirectToAction("Index", "Unauthorized");
        }

        HttpRequestMessage request = new(HttpMethod.Post, $"{configuration["intelliGuide:ApiAddress"]}/api/auth");
        request.Headers.Add("x-api-key", configuration["intelliGuide:ApiKey"]);
        request.Content = new StringContent(JsonSerializer.Serialize(new { authToken }), Encoding.UTF8, "application/json");
        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
        return RedirectToAction("Index", "Unauthorized");
        } else {
        string botId = await response.Content.ReadAsStringAsync();
        logger.LogInformation("User authenticated successfully");
        HttpContext.Session.SetString("BotId", botId);
        if (authToken == configuration["intelliGuide:ApiKey"]) 
        {
            HttpContext.Session.SetString("IsBot", "true");
        } else {
            HttpContext.Session.SetString("IsBot", "false");
        }
        return RedirectToAction("Index", "Index");
        }
    }

        public async Task<IActionResult> AuthenticateBot(string botId, string authToken)
        {
            if (string.IsNullOrEmpty(botId) || string.IsNullOrEmpty(authToken))
            {
                return BadRequest();
            }

            HttpRequestMessage request = new(HttpMethod.Post, $"{configuration["intelliGuide:ApiAddress"]}/api/auth/{botId}");
            request.Headers.Add("x-api-key", configuration["intelliGuide:ApiKey"]);
            request.Content = new StringContent(JsonSerializer.Serialize(new { token = authToken }), Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Unauthorized");
            } else {
                logger.LogInformation("Bot authenticated successfully");
                HttpContext.Session.SetString("BotId", botId);
                HttpContext.Session.SetString("IsBot", "true");
                return RedirectToAction("Index", "Index");
            }
        }
    }
}