using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using System.Net;
using intelliBot.Models;

namespace intelliBot.Controllers
{
    public class OpenAIController(HttpClient httpClient, Bot bot) : ControllerBase
    {
        private readonly ContextController contextController = new(httpClient, bot);

        public async Task<string> GetAnswer(string question)
        {
            string context = contextController.GetContext();
            question = context + question;
            //return question;9

            ChatClient client = new(model: "gpt-4o-mini", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            ChatCompletion completion = client.CompleteChat(question);

            await TextToSpeech(completion.Content[0].Text);
            return completion.Content[0].Text;
        }

        public async Task<IActionResult> TextToSpeech(string answer)
        {
            var inputBody = new
            {
                model = "tts-1",
                input = answer,
                voice = "shimmer",
                speed = 1,
            };
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.openai.com/v1/audio/speech"),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), $"Bearer {Environment.GetEnvironmentVariable("OPENAI_API_KEY")}" },
                    { HttpRequestHeader.Accept.ToString(), "application/json" }
                },
                Content = JsonContent.Create(inputBody)
            };
            var response = await httpClient.SendAsync(httpRequestMessage);
            var byteArray = await response.Content.ReadAsByteArrayAsync();
            await System.IO.File.WriteAllBytesAsync("wwwroot/resources/output.mp3", byteArray);

            return Ok();
        }
    }
}
