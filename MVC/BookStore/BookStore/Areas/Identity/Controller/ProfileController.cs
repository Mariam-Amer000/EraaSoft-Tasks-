using System.Security.Claims;

namespace BookStore.Areas.Identity.Controller;

[Area("Identity")]
[Route("api/[area]/[controller]")]
[ApiController]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ErrorResponse
            {
                ErrorNotification = "Invalid user token."
            });
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return NotFound(new ErrorResponse
            {
                ErrorNotification = "User not found."
            });
        }

        var profile = new ProfileResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address
        };

        return Ok(new SuccessResponse
        {
            Data = [profile],
            SuccessNotification = "Profile retrieved successfully."
        });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(
        UpdateProfileRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Invalid data.",
                Data = [request]
            });
        }

        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ErrorResponse
            {
                ErrorNotification = "Invalid user token."
            });
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return NotFound(new ErrorResponse
            {
                ErrorNotification = "User not found."
            });
        }

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Address = request.Address?.Trim() ?? string.Empty;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Could not update profile.",
                Data = result.Errors
                    .Select(error => (object)error.Description)
                    .ToList()
            });
        }

        var profile = new ProfileResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address
        };

        return Ok(new SuccessResponse
        {
            Data = [profile],
            SuccessNotification = "Profile updated successfully."
        });
    }
}
