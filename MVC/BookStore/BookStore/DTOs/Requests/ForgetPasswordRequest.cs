using System.ComponentModel.DataAnnotations;

namespace BookStore.DTOs.Requests;

public class ForgetPasswordRequest
{
    [Required]
    [Display(Name = "Email Or UserName")]
    public string EmailOrUserName { get; set; } = string.Empty;
}
