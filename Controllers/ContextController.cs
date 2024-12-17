using intelliBot.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Configuration;
using System.Diagnostics;
using System.Text.Json;

namespace intelliBot.Controllers
{
    public class ContextController : ControllerBase
    {
        private readonly string context;
        private readonly HttpClient httpClient;
        private readonly Bot _bot;
        public ContextController(HttpClient httpClient, Bot bot)
        {
            this.httpClient = httpClient;
            _bot = bot;
            context = GetContext();
        }


        private async Task<List<Context>> GetContexts()
        {
            List<Context> contexts = [];
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://{Environment.GetEnvironmentVariable("API_ADDRESS")}/api/contexts/{Environment.GetEnvironmentVariable("BOT_ID")}");
            request.Headers.Add("x-api-key", Environment.GetEnvironmentVariable("INTELLIGUIDE_API_KEY"));

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

            string contextString = $"Je bent een robot op een evenement. " +
                                    $"Je naam is {_bot.Name}. " +
                                    $"Je gaat vragen krijgen van gasten. " +
                                    $"Je mag alleen vragen beantwoorden die over het evenement gaan. " +
                                    $"Als je het antwoord op een vraag niet weet moet je foutcode (69420) aan het einde van je bericht zetten. " +
                                    $"Antwoord in deze taal: {_bot.Language}. " +
                                    $"Eerst volgt de context van het evenement en als laatste de vraag van de gast: ";

            foreach (var context in contexts)
            {
                contextString += context.Body;
            }
            return contextString;
        }
    }
}
