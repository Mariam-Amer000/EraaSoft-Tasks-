namespace CinemaDashboard.Areas.Identity.Controllers;

[Area(nameof(Identity))]
public class ProfilleController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    public ProfilleController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return NotFound();
        return View(user);
    }
    [HttpPost]
    public async Task<IActionResult> Index(ApplicationUser applicationUser)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return NotFound();

        user.Email = applicationUser.Email;
        user.PhoneNumber = applicationUser.PhoneNumber;
        user.FirstName = applicationUser.FirstName;
        user.LastName = applicationUser.LastName;
        user.Address = applicationUser.Address;
        
       await _userManager.UpdateAsync(user);
        return View(user);
    }
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordVM changePasswordVM)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var result = await _userManager.ChangePasswordAsync(user, changePasswordVM.OldPassword, changePasswordVM.NewPassword);

        if (!result.Succeeded)
        {
            TempData[NotificationConstant.ERROR_NOTIFICATION] = String.Join(", ", result.Errors.Select(e => e.Description));
            return View(nameof(Index), user);
        }

        TempData[NotificationConstant.SUCCESS_NOTIFICATION] = "Change password successfully";
        return RedirectToAction(nameof(Index));
    }
}
