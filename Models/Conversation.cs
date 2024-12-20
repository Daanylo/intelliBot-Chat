namespace intelliBot.Models
{
    public class Conversation
    {
        public string? Id { get; set; }
        public string? BotId { get; set; }
        public DateTime? Time { get; set; }
        public int? Review { get; set; }
        public string? Comment { get; set; }
    }
}
