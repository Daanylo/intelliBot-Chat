using System.ComponentModel.DataAnnotations;

namespace intelliBot.Models;

public enum Voice
{
    nl_NL_MaartenNeural,
    en_US_DavisNeural,
    de_DE_FlorianMultilingualNeural,
    fr_FR_YvesNeural,
    es_ES_NilNeural,
    en_GB_OllieMultilingualNeural,
    nl_NL_ColetteNeural,
    en_US_AnaNeural,
    de_DE_GiselaNeural,
    fr_FR_EloiseNeural,
    es_ES_IreneNeural
}

public class TTSModel
{
    public required string Text { get; set; }

    public required Voice SelectedVoice { get; set; }

    public string GetVoiceName()
    {
        return SelectedVoice.ToString().Replace('_', '-');
    }
    public void SetVoiceByLanguage(string language, string gender)
    {
        SelectedVoice = language switch
        {
            "Nederlands" => gender.Equals("female") ? Voice.nl_NL_ColetteNeural : Voice.nl_NL_MaartenNeural,
            "English" => gender.Equals("female") ? Voice.en_US_AnaNeural : Voice.en_US_DavisNeural,
            "Deutsch" => gender.Equals("female") ? Voice.de_DE_GiselaNeural : Voice.de_DE_FlorianMultilingualNeural,
            "Français" => gender.Equals("female") ? Voice.fr_FR_EloiseNeural : Voice.fr_FR_YvesNeural,
            "Español" => gender.Equals("female") ? Voice.es_ES_IreneNeural : Voice.es_ES_NilNeural,
            _ => throw new ArgumentException("Unsupported language"),
        };
    }
}
