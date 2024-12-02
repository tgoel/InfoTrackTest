using System.ComponentModel.DataAnnotations;

namespace InfoTrack.Validation;

public class ValidSearchUrlAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var input = value?.ToString();
        if (string.IsNullOrEmpty(input))
        {
            return new ValidationResult("Invalid URL");
        }

        try
        {
            var inputUrl = input.StartsWith("www.") ? $"https://{input}" : input;
            var uri = new UriBuilder(inputUrl);
            var isValid = Uri.IsWellFormedUriString(inputUrl, UriKind.Absolute);

            return isValid ? ValidationResult.Success : new ValidationResult("Invalid URL");
        }
        catch (UriFormatException ex)
        {
            return new ValidationResult("Invalid URL");
        }
    }
}
