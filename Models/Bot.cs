namespace intelliBot.Models
{
    public class Bot
    {
        public string? Id = Environment.GetEnvironmentVariable("BOT_ID");
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public SpeechStyle? Style { get; set; }
        public string? Location { get; set; }
        public string? Language { get; private set; }
        public void SetLanguage(string language)
        {
            string[] languages = ["Nederlands", "English", "Français", "Español", "Deutch"];
            if (languages.Contains(language))
            {
                Language = language;
            }
        }
    }

    public enum SpeechStyle
    {
        Formal,
        Informal,
        Goofy
    }
}
