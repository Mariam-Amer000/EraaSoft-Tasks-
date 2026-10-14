
using BookStore.Utility;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStore.Areas.Identity.Controller;

[Area("Identity")]
[Route("api/[area]/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly IStringLocalizer<Localization> _localizer;
    private readonly IConfiguration _configuration;
    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        IStringLocalizer<Localization> localizer,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _localizer = localizer;
        _configuration = configuration;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {

        if (!ModelState.IsValid)
            return BadRequest(new ErrorResponse()
            {
                ErrorNotification = "Invalid data",
                Data = [registerRequest]
            });

        var user = registerRequest.Adapt<ApplicationUser>();
        var result = await _userManager.CreateAsync(user, registerRequest.Password);

        if (!result.Succeeded)
        {
            ModelStateDictionary modelState = new();
            foreach (var error in result.Errors)
            {
                modelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(new ErrorResponse()
            {
                Data = [modelState]
            });
        }

        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("Confirm", ControllerConstants.ACCOUNT_CONTROLLER, new { area = AreaConstants.IDENTITY_AREA, user.Id, token }, Request.Scheme);
            string body = $"<h1>Please confirm your account by clicking <b><a href='{link}'>here</a></b></h1>";

            await _emailSender.SendEmailAsync(user.Email!, "Confirm Your Account", body);
        }

        return Created($"{Request.Scheme}://{Request.Host}/api/Identity/Account/Login", new SuccessResponse()
        {
            SuccessNotification = _localizer["User created successfully"]
        });
    }


    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse()
            {
                ErrorNotification = "Invalid data",
                Data = [loginRequest]
            });
        }

        var user = await _userManager.FindByEmailAsync(loginRequest.EmailOrUserName) ??
                   await _userManager.FindByNameAsync(loginRequest.EmailOrUserName);

        if (user is null)
        {
            return BadRequest(new ErrorResponse()
            {
                ErrorNotification = _localizer["Invalid Email or UserName"]
            });
        }
        var signInResult = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, loginRequest.Remember, lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Too many attempts, please try again later"
            });
        }

        if (signInResult.IsNotAllowed)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Please verify your account!"
            });
        }

        if (!signInResult.Succeeded)
        {
            ModelStateDictionary modelState = new();

            modelState.AddModelError(nameof(loginRequest.EmailOrUserName), "Invalid User Name or Email");
            modelState.AddModelError(nameof(loginRequest.Password), "Invalid Password");

            return BadRequest(new ErrorResponse()
            {
                Data = [modelState]
            });
        }
        var roles = await _userManager.GetRolesAsync(user);


        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss")),
        };

        foreach (var item in roles)
            claims.Add(new(ClaimTypes.Role, item));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{_configuration["jwt:Key"]}"));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken
            (
                issuer: $"{_configuration["Jwt:Issuer"]}",
                audience: $"{_configuration["Jwt:Audience"]}",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(10),
                signingCredentials: signingCredentials
            );
        var SUCCESS_NOTIFICATION = $"Welcome Back {user.FirstName} {user.LastName}";

        return Ok(new SuccessResponse
        {
            Data = [new JwtSecurityTokenHandler().WriteToken(token)],
            SuccessNotification = SUCCESS_NOTIFICATION
        });
        
    }

    [HttpGet]
    [Route("confirm")]
    public async Task<IActionResult> Confirm(string id,string token)
    {
        var user = await _userManager.FindByIdAsync(id);
        if(user is null)
        {
            return BadRequest(new ErrorResponse()
            {
                ErrorNotification = "Invalid User"
            });
        }
        var result = await _userManager.ConfirmEmailAsync(user, token);

        if(!result.Succeeded)
        {
            return BadRequest(new ErrorResponse()
            {
                ErrorNotification = "Invalid Token"
            });
        }
        return Ok(new SuccessResponse()
        {
            SuccessNotification = "Email Confirmed Successfully"
        });
       
    }

    //public IActionResult ResendConfirmEmail()
    //{
    //    return Ok("Account Controller");
    //}

    //public IActionResult ForgotPassword()
    //{
    //    return Ok("Account Controller");
    //}

    //public IActionResult ValidateOTP()
    //{
    //    return Ok("Account Controller");
    //}

    //public IActionResult NewPassword()
    //{
    //    return Ok("Account Controller");
    //}   
    //public IActionResult ResetPassword()
    //{
    //    return Ok("Account Controller");
    //}
}
