using intelliBot.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace intelliBot.Controllers
{
    public class ContextController(HttpClient httpClient, IConfiguration configuration, ILogger<ContextController> logger) : ControllerBase
    {
        private readonly HttpClient httpClient = httpClient;
        private readonly IConfiguration configuration = configuration;
        private readonly ILogger<ContextController> logger = logger;

        private async Task<List<Context>> GetContexts(string botId)
        {
            List<Context> contexts = [];
            var request = new HttpRequestMessage(HttpMethod.Get, $"{configuration["intelliGuide:ApiAddress"]}/api/contexts/{botId}");
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
                    if (status.GetString() == "inactive")
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
        public async Task InitContext(ISession session, string botId)
        {
            BotController botController = new(new Logger<BotController>(new LoggerFactory()), httpClient, configuration);
            Bot? bot = await botController.GetBot(botId);
            if (bot == null)
            {
                logger.LogError("Bot not found.");
                return;
            }
            Event? _event = await botController.GetEvent(bot.EventId);
            session.SetString("BotVoice", bot.Voice ?? "male");
            List<Context> contexts = await GetContexts(botId);

            string contextString =  $"Je bent een robot op een evenement. " +
                        $"Je naam is {bot.Name}. " +
                        $"Je gaat vragen krijgen van gasten. " +
                        $"Ongeacht in welke taal de vraag wordt gesteld, antwoord in deze taal: {session.GetString("BotLanguage")}. " +
                        $"Je mag alleen vragen beantwoorden die over het evenement gaan. " +
                        $"Je mag vragen alleen beantwoorden als je genoeg informatie hebt op basis van deze context. " +
                        $"Als de context je niet genoeg informatie geeft om de vraag te beantwoorden moet je deze foutcode: '(69420)' aan het einde van je bericht zetten. " +
                        $"Als je een link genereert, stuur dan de gehele url inclusief http(s) en stuur de link altijd aan het einde van het bericht en geef aan dat je de qr code kan scannen. " +
                        $"De tijd is nu {DateTime.Now}. " +
                        $"Je spreekstijl is: {bot.Style}. ";

                        if (_event != null)
                        {
                        contextString += $"Evenement informatie: " +
                                $"Evenementnaam: {_event.Name}; " +
                                $"Omschrijving: {_event.Description}; " +
                                $"Locatie: {_event.Address}, {_event.Place}; " +
                                $"Datum & tijd: {_event.Time}; " +
                                $"Context van het evenement: ";
                        }

            foreach (var context in contexts)
            {
            contextString += context.Title + ": " + context.Body + "; ";
            }
            session.SetString("Context", contextString);
            logger.LogInformation("Context initialized: {contextString}", contextString);
        }

        public void UpdateContext(ISession session, string question, string answer) {
            if (session.GetString("Context") != null)
            {
                string context = session.GetString("Context") + " Vraag van gast: " + question + " Jouw antwoord: " + answer;;
                session.SetString("Context", context);
                logger.LogInformation("Context updated: {context}", context);
            } else {
                logger.LogError("Context not found in session.");
            }
        }
    }
}
