namespace intelliBot.Models
{
    public class Message
    {
        public required string Id { get; set; }
        public required string ConversationId { get; set; }
        public required MessageType Type { get; set; }
        public required DateTime Time { get; set; }
        public required string Body { get; set; }
    }
    public enum MessageType
    {
        Question,
        Answer
    }
}
