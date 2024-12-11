using OpenAI.Chat;

namespace intelliBot.Controllers
{
    public static class OpenAIController
    {
        public static string GetAnswer(string question)
        {
            var builder = WebApplication.CreateBuilder();
            var apiKey = builder.Configuration["ApiSettings:OPENAI_API_KEY"];

            ChatClient client = new(model: "gpt-4o-mini", apiKey: apiKey);

            if (ContextController.GetContext() != null)
            {
                question = ContextController.GetContext() + question;
            }
            ChatCompletion completion = client.CompleteChat(question);

            return completion.Content[0].Text;
        }
    }
}
