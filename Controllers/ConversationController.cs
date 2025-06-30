using intelliBot.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace intelliBot.Controllers
{
    public class ConversationController(ILogger<ConversationController> logger, HttpClient httpClient, IConfiguration configuration) : Controller
    {
        private readonly ILogger<ConversationController> logger = logger;
        private readonly HttpClient httpClient = httpClient;
        private readonly OpenAIController openAIController = new(httpClient, logger, configuration);


        public IActionResult Conversation()
        {
            string? botId = HttpContext.Session.GetString("BotId");
            if (botId == null)
            {
                return RedirectToAction("Index", "Unauthorized");
            }
            string? conversationId = HttpContext.Session.GetString("ConversationId");
            if (conversationId != null)
            {
                HttpContext.Session.Remove("ConversationId");
                HttpContext.Session.Remove("Context");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessTranscript([FromBody] string transcript)
        {
            logger.LogInformation("Received transcript: {transcript}", transcript);
            ISession session = HttpContext.Session;
            string? botId = session.GetString("BotId");
            if (botId == null)
            {
                return RedirectToAction("Index", "Unauthorized");
            }
            if (session.GetString("ConversationId") == null)
            {
                await StartConversation();
            }
            ContextController contextController = new(httpClient, configuration, new Logger<ContextController>(new LoggerFactory()));
            string? context;
            if (string.IsNullOrEmpty(session.GetString("Context")))
            {
                logger.LogInformation("No context found, initializing context...");
                await contextController.InitContext(session, botId);
                context = session.GetString("Context");
                logger.LogInformation("Context initialized successfully {context}", context);
            } else {
                context = session.GetString("Context");
            }
            
            await AddMessage(Guid.NewGuid().ToString(), transcript, "question");
            logger.LogInformation("Context sent: {context}", context);
            string messageId = Guid.NewGuid().ToString();
            string conversationId = HttpContext.Session.GetString("ConversationId") ?? "";
            string answer = await openAIController.GetAnswer(context, transcript, conversationId, messageId, HttpContext.Session);
            await AddMessage(messageId, answer, "answer");
            contextController.UpdateContext(session, transcript, answer);
            if (answer.Contains("(69420)") || answer.Contains("69420"))
            {
                answer = answer.Replace("(69420)", string.Empty);
                answer = answer.Replace("69420", string.Empty);
            }
            return Ok(new { answer, conversationId, messageId });
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
                bot_id = HttpContext.Session.GetString("BotId"),
                time = DateTime.Now,
                review = null,
                comment = ""
            };
                var request = new HttpRequestMessage(HttpMethod.Post, $"{configuration["intelliGuide:ApiAddress"]}/api/conversation") 
                {
                    Content = new StringContent(JsonSerializer.Serialize(conversation), Encoding.UTF8, "application/json")
                };
                request.Headers.Add("x-api-key", configuration["intelliGuide:ApiKey"]);

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
            HttpContext.Session.SetString("Messages", JsonSerializer.Serialize(new List<Message>()));

            logger.LogInformation("Started conversation with id: {id}", guid);
            await StoreConversation();
        }

        public async Task AddMessage(string messageId, string messageText, string messageType)
        {
            var messagesJson = HttpContext.Session.GetString("Messages");
            var messages = messagesJson != null ? JsonSerializer.Deserialize<List<Message>>(messagesJson) : [];
            Message message = new()
            {
                message_id = messageId,
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

        public async Task<IActionResult> GetGreeting()
        {
            string? botId = HttpContext.Session.GetString("BotId");
            if (botId == null)
            {
                return RedirectToAction("Index", "Unauthorized");
            }
            BotController botController = new(new Logger<BotController>(new LoggerFactory()), httpClient, configuration);
            Bot? bot = await botController.GetBot(botId);
            if (bot == null)
            {
                return NotFound();
            }
            return Ok(new { greeting = bot.Greeting });
        }
    }
}
