namespace intelliBot.Models
{
    public class Context
    {
        public required string Id { get; set; }
        public required string BotId { get; set; }
        public required string Title { get; set; }
        public required string Body { get; set; }
        public required bool IsActive { get; set; }

    }
}
