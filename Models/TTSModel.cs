using System.ComponentModel.DataAnnotations;

namespace intelliBot.Models;

public class TTSModel
{
    [Required (ErrorMessage = "Please enter some text.")]
    [StringLength(500, ErrorMessage = "Text must be less than 500 characters.")]
    public required string Text { get; set; }
    [Required (ErrorMessage = "Please select a voice.")]
    public required string SelectedVoice { get; set; }
    
}