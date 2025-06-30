using Microsoft.AspNetCore.Mvc;
using intelliBot.Models;
using System.Text;
using System.Text.Json;

namespace intelliBot.Controllers
{
    public class ReviewController(ILogger<ReviewController> logger, HttpClient httpClient, IConfiguration configuration) : Controller
    {
        private readonly ILogger<ReviewController> logger = logger;
        private readonly HttpClient httpClient = httpClient;
        private readonly IConfiguration configuration = configuration;

        public IActionResult Review()
        {
            string? botId = HttpContext.Session.GetString("BotId");
            if (botId == null)
            {
                return RedirectToAction("Index", "Unauthorized");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessReview([FromBody] Review userReview)
        {
            var conversationId = HttpContext.Session.GetString("ConversationId");
            if (ModelState.IsValid && conversationId != null)
            {
                logger.LogInformation("Review: {review}", userReview.review);
                logger.LogInformation("Comment: {comment}", userReview.comment);

                var request = new HttpRequestMessage(HttpMethod.Post, $"{configuration["intelliGuide:ApiAddress"]}/api/review/{conversationId}")
                {
                    Content = new StringContent(JsonSerializer.Serialize(userReview), Encoding.UTF8, "application/json")
                };
                request.Headers.Add("x-api-key", configuration["intelliGuide:ApiKey"]);

                var response = await httpClient.SendAsync(request);

                return Ok( response.IsSuccessStatusCode ? new { message = "Review submitted successfully", botId = HttpContext.Session.GetString("BotId") } : new { message = "Review submission failed" });
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
