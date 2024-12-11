namespace intelliBot.Models
{
    public class Bot
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Voice { get; set; }
        public required int Avatar { get; set; }
        public required string Language { get; set; }
        public required int Style { get; set; }
        public required int MaxTokens { get; set; }
        public required int ConvLength { get; set; }
        public required int AnswerLength { get; set; }
        public required bool GreetUser { get; set; }
        public required bool GenerateQr { get; set; }
        public required bool StoreConv { get; set; }
        public required bool RequestReviews { get; set; }
        public required bool IsActive { get; set; }
    }
}
