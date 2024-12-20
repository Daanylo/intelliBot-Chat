using intelliBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;

namespace intelliBot.Controllers
{
    public class ConversationController(ILogger<ConversationController> logger, HttpClient httpClient, Bot bot) : Controller
    {
        private readonly ILogger<ConversationController> _logger = logger;
        private readonly HttpClient _httpClient = httpClient;
        private readonly OpenAIController _openAIController = new(httpClient, bot);
        private readonly Bot bot = bot;
        private Conversation? conversation;
        private List<Message>? messages;

        public IActionResult Conversation()
        {
            StartConversation();
            return View();
        }

        [HttpPost]
        public IActionResult ProcessTranscript([FromBody] string transcript)
        {
            _logger.LogInformation("Received transcript: {transcript}", transcript);
            AddMessage(transcript, MessageType.Question);
            string answer = _openAIController.GetAnswer(transcript).Result;
            AddMessage(answer, MessageType.Answer);
            return Ok(new { answer });
        }

        [HttpPost]
        public async Task<IActionResult> FinishConversation()
        {
            _logger.LogInformation("FinishConversation method called.");
            if (conversation != null)
            {
                _logger.LogInformation("Finishing conversation with ID: {conversationId}", conversation.Id);
                conversation.Review = null;
                conversation.Comment = null;
                await StoreConversation();
                await StoreMessages();
                return Ok(new { message = "Conversation finished successfully" });
            }
            else
            {
                _logger.LogWarning("Attempted to finish a conversation, but no conversation was active.");
                return Ok(new { message = "Attempted to finish a conversation, but no conversation was active." });
            }
        }

        public async Task StoreConversation()
        {
                var request = new HttpRequestMessage(HttpMethod.Post, $"http://{Environment.GetEnvironmentVariable("API_ADDRESS")}/api/conversation") 
                {
                    Content = new StringContent(JsonSerializer.Serialize(conversation))
                };
                request.Headers.Add("x-api-key", Environment.GetEnvironmentVariable("INTELLIGUIDE_API_KEY"));

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Conversation stored successfully");
                }
                else
                {
                    _logger.LogError("Failed to store conversation");
                }
        }

        public async Task StoreMessages()
        {
            if (messages == null) {
                return;
            }
            foreach (Message message in messages) {
                var request = new HttpRequestMessage(HttpMethod.Post, $"http://{Environment.GetEnvironmentVariable("API_ADDRESS")}/api/message") {
                    Content = new StringContent(JsonSerializer.Serialize(message))
                };
                request.Headers.Add("x-api-key", Environment.GetEnvironmentVariable("INTELLIGUIDE_API_KEY"));

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Message stored successfully");
                }
                else
                {
                    _logger.LogError("Failed to store message");
                }
            }
        }



        public void StartConversation()
        {
            conversation = new()
            {
                Id = Guid.NewGuid().ToString(),
                BotId = bot.Id,
                Time = DateTime.Now,
                Review = null,
                Comment = null
            };
            _logger.LogInformation($"Started conversation with id {conversation.Id}");
        }

        public void AddMessage(string messageText, MessageType messageType)
        {
            messages ??= [];
            if (conversation != null) {
                Message message = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    ConversationId = conversation.Id,
                    Type = messageType,
                    Time = DateTime.Now,
                    Body = messageText
                };
                messages.Add(message);
                _logger.LogInformation($"Added message with id {message.Id} to conversation {conversation.Id}");
            } else {
                StartConversation();
            }
        }
    }
}
