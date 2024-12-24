using intelliBot.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace intelliBot.Controllers
{
    public class ConversationController(ILogger<ConversationController> logger, HttpClient httpClient, Bot bot, IConfiguration configuration) : Controller
    {
        private readonly ILogger<ConversationController> logger = logger;
        private readonly HttpClient httpClient = httpClient;
        private readonly OpenAIController openAIController = new(httpClient, bot, logger, configuration);
        private readonly Bot bot = bot;
        public string? conversationId;

        public IActionResult Conversation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessTranscript([FromBody] string transcript)
        {
            conversationId = HttpContext.Session.GetString("ConversationId");
            if (conversationId == null)
            {
                await StartConversation();
            }
            logger.LogInformation("Received transcript: {transcript}", transcript);
            await AddMessage(transcript, "question");
            string answer = openAIController.GetAnswer(transcript).Result;
            await AddMessage(answer, "answer");
            return Ok(new { answer });
        }

        [HttpPost]
        public IActionResult FinishConversation()
        {
            return Ok(new { message = "Conversation finished successfully" });
        }

        public async Task StoreConversation()
        {
            Conversation conversation = new()
            {
                conversation_id = HttpContext.Session.GetString("ConversationId"),
                bot_id = bot.Id,
                time = DateTime.Now,
                review = null,
                comment = null
            };
                var request = new HttpRequestMessage(HttpMethod.Post, $"{configuration["intelliGuide:ApiAddress"]}/api/conversation") 
                {
                    Content = new StringContent(JsonSerializer.Serialize(conversation), Encoding.UTF8, "application/json")
                };
                request.Headers.Add("x-api-key", configuration["intelliGuide:ApiKey"]);
                logger.LogInformation("{request}", request.ToString());

                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Conversation stored successfully");
                }
                else
                {
                    logger.LogError("Failed to store conversation");
                }
        }

        public async Task StartConversation()
        {
            var guid = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("ConversationId", guid);
            conversationId = HttpContext.Session.GetString("ConversationId");
            HttpContext.Session.SetString("Messages", JsonSerializer.Serialize(new List<Message>()));

            logger.LogInformation("Started conversation with id: {id}", guid);
            await StoreConversation();
        }

        public async Task AddMessage(string messageText, string messageType)
        {
            var messagesJson = HttpContext.Session.GetString("Messages");
            var messages = messagesJson != null ? JsonSerializer.Deserialize<List<Message>>(messagesJson) : [];
            Message message = new()
            {
                message_id = Guid.NewGuid().ToString(),
                conversation_id = HttpContext.Session.GetString("ConversationId"),
                type = messageType,
                time = DateTime.Now,
                body = messageText
            };
            messages?.Add(message);
            HttpContext.Session.SetString("Messages", JsonSerializer.Serialize(messages));
            logger.LogInformation("Added message with id {messageId} to conversation {conversationId}", message.message_id, message.conversation_id);

            await StoreMessage(message);
        }

        public async Task StoreMessage(Message message)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{configuration["intelliGuide:ApiAddress"]}/api/message")
            {
                Content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("x-api-key", configuration["intelliGuide:ApiKey"]);

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Message stored successfully");
            }
            else
            {
                logger.LogError("Failed to store message: {StatusCode}", response.StatusCode);
            }
        }
    }
}
