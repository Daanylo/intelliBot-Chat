using System.Diagnostics;

namespace intelliBot.Controllers
{
    public static class TextToSpeech
    {
        public static void Speak(string text, string voice = "Dutch")
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "espeak-ng",
                    Arguments = $"-v {voice} \"{text}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            process.WaitForExit();
        }
    }
}
