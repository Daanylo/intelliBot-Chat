using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using intelliBot.Models;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace intelliBot.Controllers
{
    public class OpenAIController(HttpClient httpClient, ILogger<ConversationController> logger, IConfiguration configuration) : ControllerBase
    {

        private readonly HttpClient httpClient = httpClient;
        private readonly ILogger<ConversationController> logger = logger;
        private readonly TextToSpeechController textToSpeechController = new(configuration, logger);

        public async Task<string> GetAnswer(string? context, string question, string conversationId, string messageId, ISession session)
        {
            // return "This is a test. aaaaaaaabbbbbbbbcccccccccccccc (69420) https://google.com.";
            if (string.IsNullOrEmpty(context))
            {
                logger.LogError("Context or question is null or empty.");
                return "Error: Context or question is null or empty.";
            }

            question = "User question: " + question;

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
                logger.LogError("OpenAI API call failed with status code: {responseStatusCode}", response.StatusCode);
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
                string ttsAnswer = answer;
                if (ttsAnswer.Contains("http"))
                {
                    var words = answer.Split(' ');
                    ttsAnswer = string.Join(' ', words.Where(word => !word.StartsWith("http")));
                }
                if (ttsAnswer.Contains("(69420)") || ttsAnswer.Contains("69420"))
                {
                    ttsAnswer = ttsAnswer.Replace("(69420)", "");
                    ttsAnswer = ttsAnswer.Replace("69420", "");
                }
                string? botVoice = session.GetString("BotVoice");
                string? botLanguage = session.GetString("BotLanguage");
                

                TTSModel model = new()
                {
                    Text = ttsAnswer,
                    SelectedVoice = Voice.en_GB_OllieMultilingualNeural
                };
                if (botLanguage != null && botVoice != null) {
                    model.SetVoiceByLanguage(botLanguage, botVoice);
                }
                await textToSpeechController.TextToSpeech(model, conversationId, messageId);

                return answer;
            }
            catch (JsonException ex)
            {
                logger.LogError("Failed to deserialize OpenAI response: {exMessage}", ex.Message);
                logger.LogError("Response Content: {responseContent}", responseContent);
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
        public string? FinishReason { get; set; }
    }

    public class OpenAIMessage
    {
        public required string Content { get; set; }
        public string? Role { get; set; }
    }

    }

