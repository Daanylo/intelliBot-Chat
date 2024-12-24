using intelliBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Configuration;
using System.Diagnostics;
using System.Text.Json;

namespace intelliBot.Controllers
{
    public class ContextController(HttpClient httpClient, Bot bot, IConfiguration configuration) : ControllerBase
    {
        private readonly HttpClient httpClient = httpClient;
        private readonly Bot bot = bot;

        private async Task<List<Context>> GetContexts()
        {
            List<Context> contexts = [];
            var request = new HttpRequestMessage(HttpMethod.Get, $"{configuration["intelliGuide:ApiAddress"]}/api/contexts/{configuration["intelliGuide:BotId"]}");
            request.Headers.Add("x-api-key", configuration["intelliGuide:ApiKey"]);

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(jsonResponse);
                foreach (var context in document.RootElement.EnumerateArray())
                {
                context.TryGetProperty("context_id", out var context_id);
                context.TryGetProperty("title", out var title);
                context.TryGetProperty("body", out var body);
                context.TryGetProperty("status", out var status);
                    if (status.GetString() == "Draft")
                    {
                        continue;
                    }
                    if (context_id.GetString() != null && title.GetString() != null && body.GetString() != null)
                    {
                        var newContext = new Context
                        {
                            Id = context_id.GetString(),
                            Title = title.GetString(),
                            Body = body.GetString(),
                        };
                        contexts.Add(newContext);
                    }
                }
            }
            return contexts;
        }
        public string GetContext()
        {
            List<Context> contexts = GetContexts().Result;

            string contextString =  $"Je bent een robot op een evenement. " +
                                    $"Je naam is {bot.Name}. " +
                                    $"Je gaat vragen krijgen van gasten. " +
                                    $"Je mag alleen vragen beantwoorden die over het evenement gaan. " +
                                    $"Als de context je niet genoeg informatie geeft om de vraag te beantwoorden moet je foutcode (69420) aan het einde van je bericht zetten. " +
                                    $"Antwoord in deze taal: {bot.Language}. " +
                                    $"Je spreekstijl is: {bot.Style}." +
                                    $"Context van het evenement: ";

            foreach (var context in contexts)
            {
                contextString += context.Body;
            }
            return contextString;
        }
    }
}
