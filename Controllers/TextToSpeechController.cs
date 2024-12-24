using intelliBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech;

namespace intelliBot.Controllers
{
    public class TextToSpeechController(IConfiguration configuration) : ControllerBase
    {
        private readonly IConfiguration configuration = configuration;
        private readonly string? subscriptionKey = configuration["AzureSpeech:SubscriptionKey"];
        private readonly string? region = configuration["AzureSpeech:Region"];
        public async Task<IActionResult> TextToSpeech(TTSModel model)
        {
            if (string.IsNullOrEmpty(subscriptionKey) || string.IsNullOrEmpty(region))
            {
                return BadRequest("Azure Speech configuration is missing.");
            }
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);
            config.SpeechSynthesisVoiceName = model.SelectedVoice;

            using var synthesizer = new SpeechSynthesizer(config, null);
            using var result = await synthesizer.SpeakTextAsync(model.Text);
            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                await System.IO.File.WriteAllBytesAsync("wwwroot/resources/output.mp3", result.AudioData);
                return Ok();
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                return BadRequest($"CANCELED: Reason={cancellation.Reason}");
            }
            return BadRequest("Failed to synthesize audio.");
        }
    }
}