using Microsoft.AspNetCore.Mvc;
using intelliBot.Models;

namespace intelliBot.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ILogger<ReviewController> _logger;
        private readonly HttpClient _httpClient;

        public ReviewController(ILogger<ReviewController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public IActionResult Review()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessReview([FromBody] Review userReview)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Review: {review}", userReview.review);
                _logger.LogInformation("Comment: {comment}", userReview.comment);

                var request = new HttpRequestMessage(HttpMethod.Post, $"http://{Environment.GetEnvironmentVariable("API_ADDRESS")}/api/contexts/{Environment.GetEnvironmentVariable("BOT_ID")}");
                request.Headers.Add("x-api-key", Environment.GetEnvironmentVariable("INTELLIGUIDE_API_KEY"));

                var response = await _httpClient.SendAsync(request);

                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
