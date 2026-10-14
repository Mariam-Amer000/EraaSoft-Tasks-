using Mapster;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LoginRequest = BookStore.DTOs.Requests.LoginRequest;
using RegisterRequest = BookStore.DTOs.Requests.RegisterRequest;

namespace BookStore.Areas.Identity.Controller;

[Area("Identity")]
[Route("api/[area]/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly IRepository<ApplicationUserOTP> _otpRepository;
    private readonly IConfiguration _configuration;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        IRepository<ApplicationUserOTP> otpRepository,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _otpRepository = otpRepository;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Invalid data",
                Data = [registerRequest]
            });
        }

        var user = registerRequest.Adapt<ApplicationUser>();
        var result = await _userManager.CreateAsync(user, registerRequest.Password);

        if (!result.Succeeded)
        {
            ModelStateDictionary errors = new();

            foreach (var error in result.Errors)
                errors.AddModelError(string.Empty, error.Description);

            return BadRequest(new ErrorResponse { Data = [errors] });
        }

        var roleResult = await _userManager.AddToRoleAsync(user, RoleConstants.CUSTOMER);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return StatusCode(500, new ErrorResponse
            {
                ErrorNotification = "Could not assign the Customer role."
            });
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var link = Url.Action(
            nameof(Confirm),
            "Account",
            new { area = "Identity", id = user.Id, token },
            Request.Scheme);

        if (string.IsNullOrEmpty(link))
        {
            return StatusCode(500, new ErrorResponse
            {
                ErrorNotification = "Could not generate the confirmation link."
            });
        }

        var body = $"<h1>Please confirm your account by clicking " +
                   $"<a href='{link}'>here</a></h1>";

        await _emailSender.SendEmailAsync(
            user.Email!,
            "Confirm Your Account",
            body);

        return Created(
            $"{Request.Scheme}://{Request.Host}/api/Identity/Account/login",
            new SuccessResponse
            {
                SuccessNotification = "User created successfully. Please confirm your email."
            });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Invalid data",
                Data = [loginRequest]
            });
        }

        var user =
            await _userManager.FindByEmailAsync(loginRequest.EmailOrUserName)
            ?? await _userManager.FindByNameAsync(loginRequest.EmailOrUserName);

        if (user is null)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Invalid User Name or Email."
            });
        }

        var signInResult = await _signInManager.PasswordSignInAsync(
            user,
            loginRequest.Password,
            loginRequest.Remember,
            lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Too many attempts. Please try again later."
            });
        }

        if (signInResult.IsNotAllowed)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Please verify your account."
            });
        }

        if (!signInResult.Succeeded)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Invalid User Name or Password."
            });
        }

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(
                JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var jwtKey = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            return StatusCode(500, new ErrorResponse
            {
                ErrorNotification = "JWT signing key is not configured."
            });
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(10),
            signingCredentials: credentials);

        return Ok(new SuccessResponse
        {
            Data = [new JwtSecurityTokenHandler().WriteToken(jwt)],
            SuccessNotification = $"Welcome Back {user.FirstName} {user.LastName}"
        });
    }

    [HttpGet("confirm")]
    public async Task<IActionResult> Confirm(string id, string token)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
        {
            return NotFound(new ErrorResponse
            {
                ErrorNotification = "User not found."
            });
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = string.Join(
                    ", ",
                    result.Errors.Select(error => error.Description))
            });
        }

        return Ok(new SuccessResponse
        {
            SuccessNotification = "Email confirmed successfully. Please login."
        });
    }

    [HttpPost("ForgetPassword")]
    public async Task<IActionResult> ForgetPassword(
        ForgetPasswordRequest request,
        CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Invalid data.",
                Data = [request]
            });
        }

        var user =
            await _userManager.FindByEmailAsync(request.EmailOrUserName)
            ?? await _userManager.FindByNameAsync(request.EmailOrUserName);

        if (user is null)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Invalid User Name or Email."
            });
        }

        var otp = RandomNumberGenerator.GetInt32(1000, 10000).ToString();

        var created = await _otpRepository.CreateAsync(new ApplicationUserOTP
        {
            ApplicationUserId = user.Id,
            OTP = otp,
            IsUsed = false,
            ValidTo = DateTime.UtcNow.AddMinutes(10)
        }, ct);

        if (!created || await _otpRepository.CommitAsync(ct) == 0)
        {
            return StatusCode(500, new ErrorResponse
            {
                ErrorNotification = "Could not create the OTP."
            });
        }

        await _emailSender.SendEmailAsync(
            user.Email!,
            "Reset your account",
            $"<h1>Your OTP is: {otp}. Do not share it.</h1>");

        Response.Cookies.Append("userId", user.Id, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(10)
        });

        Response.Cookies.Delete("otpValidated");

        return Ok(new SuccessResponse
        {
            SuccessNotification = "OTP sent to your email."
        });
    }

    [HttpPost("ValidateOTP")]
    public async Task<IActionResult> ValidateOTP(ValidateOTPRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Invalid data.",
                Data = [request]
            });
        }

        var userId = Request.Cookies["userId"];

        if (string.IsNullOrEmpty(userId))
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Please request a new OTP."
            });

        var otpInDb = _otpRepository.Get(
                otp => otp.ApplicationUserId == userId
                    && !otp.IsUsed
                    && otp.ValidTo >= DateTime.UtcNow)
            .OrderByDescending(otp => otp.CreateAt)
            .FirstOrDefault();

        if (otpInDb is null || otpInDb.OTP != request.OTP)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Invalid or expired OTP."
            });
        }

        otpInDb.IsUsed = true;

        if (await _otpRepository.CommitAsync() == 0)
        {
            return StatusCode(500, new ErrorResponse
            {
                ErrorNotification = "Could not validate the OTP."
            });
        }

        Response.Cookies.Append("otpValidated", "true", new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(10)
        });

        return Ok(new SuccessResponse
        {
            SuccessNotification = "OTP validated. You can now change your password."
        });
    }

    [HttpPost("NewPassword")]
    public async Task<IActionResult> NewPassword(NewPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Invalid data.",
                Data = [request]
            });
        }

        var userId = Request.Cookies["userId"];
        var otpValidated = Request.Cookies["otpValidated"];

        if (string.IsNullOrEmpty(userId) || otpValidated != "true")
        {
            return BadRequest(new ErrorResponse
            {
                ErrorNotification = "Validate your OTP before changing the password."
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

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(
            user,
            resetToken,
            request.Password);

        if (!result.Succeeded)
        {
            ModelStateDictionary errors = new();

            foreach (var error in result.Errors)
                errors.AddModelError(string.Empty, error.Description);

            return BadRequest(new ErrorResponse { Data = [errors] });
        }

        Response.Cookies.Delete("userId");
        Response.Cookies.Delete("otpValidated");

        return Ok(new SuccessResponse
        {
            SuccessNotification = "Password changed successfully."
        });
    }
}
