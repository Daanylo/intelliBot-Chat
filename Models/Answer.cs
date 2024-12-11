namespace intelliBot.Models
{
    public class Answer
    {
        public required string Id { get; set; }
        public required string ConversationId { get; set; }
        public required int Order { get; set; }
        public required string Body { get; set; }
    }
}
