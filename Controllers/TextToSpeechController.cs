using intelliBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech;

namespace intelliBot.Controllers
{
    public class TextToSpeechController(IConfiguration configuration, ILogger<ConversationController> logger) : ControllerBase
    {
        private readonly ILogger<ConversationController> logger = logger;
        private readonly string? subscriptionKey = configuration["AzureSpeech:SubscriptionKey"];
        private readonly string? region = configuration["AzureSpeech:Region"];
        public async Task<IActionResult> TextToSpeech(TTSModel model, string conversationId, string messageId)
        {
            if (string.IsNullOrEmpty(subscriptionKey) || string.IsNullOrEmpty(region))
            {
                return BadRequest("Azure Speech configuration is missing.");
            }
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);
            config.SpeechSynthesisVoiceName = model.GetVoiceName();

            using var synthesizer = new SpeechSynthesizer(config, null);
            using var result = await synthesizer.SpeakTextAsync(model.Text);
            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                if (!Directory.Exists($"wwwroot/resources/{conversationId}")) {
                    Directory.CreateDirectory($"wwwroot/resources/{conversationId}");
                }
                await System.IO.File.WriteAllBytesAsync($"wwwroot/resources/{conversationId}/{messageId}.mp3", result.AudioData);
                return Ok();
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                logger.LogInformation("Speech synthesis cancelled: {reason}", cancellation.Reason);
                return BadRequest($"CANCELED: Reason={cancellation.Reason}");

            }
            return BadRequest("Failed to synthesize audio.");
        }
    }
}