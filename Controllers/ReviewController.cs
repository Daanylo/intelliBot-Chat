using Microsoft.AspNetCore.Mvc;

namespace intelliBot.Controllers
{
    public class ReviewController(ILogger<ReviewController> logger) : Controller
    {
        private readonly ILogger<ReviewController> _logger = logger;

        public IActionResult Review()
        {
            return View();
        }
    }
}
