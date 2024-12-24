using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using System.Text.Json;
using intelliBot.Models;
using Microsoft.CognitiveServices.Speech;

namespace intelliBot.Controllers
{
    public class OpenAIController(HttpClient httpClient, Bot bot, ILogger<ConversationController> logger, IConfiguration configuration) : ControllerBase
    {

        private readonly HttpClient httpClient = httpClient;
        private readonly ILogger<ConversationController> logger = logger;
        private readonly ContextController contextController = new(httpClient, bot, configuration);
        private readonly TextToSpeechController textToSpeechController = new(configuration);

        public async Task<string> GetAnswer(string question)
        {
            // return "This is a placeholder answer.";
            string context = contextController.GetContext();
            var messages = new[]
            {
                new { role = "system", content = context },
                new { role = "user", content = question }
            };

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages
            };

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.openai.com/v1/chat/completions"),
                Headers =
                {
                    { "Authorization", $"Bearer {configuration["OpenAI:ApiKey"]}" }
                },
                Content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(httpRequestMessage);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"OpenAI API call failed with status code: {response.StatusCode}");
                return $"Error: {response.StatusCode}";
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            try
            {
                var completion = JsonSerializer.Deserialize<OpenAIResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (completion?.Choices == null || completion.Choices.Count == 0)
                {
                    logger.LogError("No choices were returned in the OpenAI API response.");
                    return "Error: No valid response from OpenAI.";
                }

                string answer = completion.Choices[0].Message.Content;

                TTSModel model = new()
                {
                    Text = answer,
                    SelectedVoice = "en-US-AriaNeural"
                };
                await textToSpeechController.TextToSpeech(model);

                return answer;
            }
            catch (JsonException ex)
            {
                logger.LogError($"Failed to deserialize OpenAI response: {ex.Message}");
                logger.LogError($"Response Content: {responseContent}");
                return "Error: Failed to process OpenAI response.";
            }

        }
    }
    public class OpenAIResponse
    {
        public required List<OpenAIChoice> Choices { get; set; }
    }

    public class OpenAIChoice
    {
        public required OpenAIMessage Message { get; set; }
        public string? FinishReason { get; set; } // Optional property from API
    }

    public class OpenAIMessage
    {
        public required string Content { get; set; }
        public string? Role { get; set; } // Optional for safety
    }

    }

