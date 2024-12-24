namespace intelliBot.Models
{
    public class Conversation
    {
        public string? conversation_id { get; set; }
        public string? bot_id { get; set; }
        public DateTime? time { get; set; }
        public int? review { get; set; }
        public string? comment { get; set; }
    }
}
