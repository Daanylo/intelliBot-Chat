namespace intelliBot.Models
{
    public class Message
    {
        public string? Id { get; set; }
        public string? ConversationId { get; set; }
        public MessageType? Type { get; set; }
        public DateTime? Time { get; set; }
        public string? Body { get; set; }
    }
    public enum MessageType
    {
        Question,
        Answer
    }
}
