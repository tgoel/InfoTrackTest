using InfoTrack.Validation;
using System.ComponentModel.DataAnnotations;

namespace InfoTrack.Models;

public class KeywordSearchResultModel
{
    [Required(ErrorMessage = "Keyword is required")]
    public string Keyword { get; set; }

    [Required(ErrorMessage = "URL is required")]
    [ValidSearchUrl(ErrorMessage = "Invalid URL")]
    public string Url { get; set; }

    public string? Result { get; set; }
}
