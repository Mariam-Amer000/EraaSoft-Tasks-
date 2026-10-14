using System.ComponentModel.DataAnnotations;

namespace BookStore.DTOs.Requests;

public class UpdateProfileRequest
{
    [Required]
    [MinLength(2)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MinLength(2)]
    public string LastName { get; set; } = string.Empty;

    public string? Address { get; set; }
}
