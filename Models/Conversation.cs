namespace intelliBot.Models
{
    public class Conversation
    {
        public required string Id { get; set; }
        public required string BotId { get; set; }
        public required DateTime Date { get; set; }
        public int? Review { get; set; }
        public string? Comment { get; set; }
        public required int Tokens { get; set; }
    }
}
