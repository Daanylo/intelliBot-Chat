namespace intelliBot.Models
{
    public class Message
    {
        public string? message_id { get; set; }
        public string? conversation_id { get; set; }
        public string? type { get; set; }
        public DateTime? time { get; set; }
        public string? body { get; set; }
    }
}
