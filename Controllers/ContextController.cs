using intelliBot.Models;
using System.Configuration;
using System.Diagnostics;

namespace intelliBot.Controllers
{
    public static class ContextController
    {
        readonly static List<Context> contexts = DataController.GetContexts();

        private static string botLanguage = BotController.botLanguage;

        public static void SetLanguage(string language)
        {
            botLanguage = language;
        }

        public static string GetContext()
        {
            string defaultContext = $"Je bent een robot op een evenement." +
                                        $"Je naam is {BotController.botName}" +
                                        $"Je gaat vragen krijgen van gasten." +
                                        $"Je mag alleen vragen beantwoorden die over het evenement gaan." +
                                        $"Als je het antwoord op een vraag niet weet moet je foutcode (69420) aan het einde van je bericht zetten" +
                                        $"Antwoord in deze taal: {botLanguage}" +
                                        $"Eerst volgt de context van het evenement en als laatste de vraag van de gast: ";

            string contextString = defaultContext;
            TrimContexts();
            foreach (var context in contexts)
            {
                contextString += context.Body;
            }
            return contextString;
        }
        public static void TrimContexts()
        {
            foreach (var context in contexts)
            {
                context.Body = context.Body.Trim();
                if (!context.IsActive)
                {
                    contexts.Remove(context);
                }
            }
        }
    }
}
