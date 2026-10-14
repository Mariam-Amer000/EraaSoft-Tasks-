using System.ComponentModel.DataAnnotations;

namespace BookStore.DTOs.Requests;

public class ValidateOTPRequest
{
    [Required]
    public string OTP { get; set; } = string.Empty;
}
