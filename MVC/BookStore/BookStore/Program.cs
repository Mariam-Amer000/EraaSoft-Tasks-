using BookStore.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Globalization;
using System.Text;

namespace BookStore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        // Add services to the container.


        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddLocalization(options => options.ResourcesPath = "Recourses");
        builder.Services.AddTransient<IEmailSender, EmailSender>();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 8;
            options.Lockout.MaxFailedAccessAttempts = 6;

            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();


        const string defaultCulture = "en";

        var supportedCultures = new[]
        {
            new CultureInfo(defaultCulture),
            new CultureInfo("ar"),
        };

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(defaultCulture);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
            };
        });
        StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

        builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
        var app = builder.Build();


        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[]
            {
                RoleConstants.SUPER_ADMIN,
                RoleConstants.ADMIN,
                RoleConstants.EMPLOYEE,
                RoleConstants.CUSTOMER
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            var adminEmail = builder.Configuration["SeedAdmin:Email"];
            var adminPassword = builder.Configuration["SeedAdmin:Password"];

            if (!string.IsNullOrWhiteSpace(adminEmail) &&
                !string.IsNullOrWhiteSpace(adminPassword))
            {
                var admin = await userManager.FindByEmailAsync(adminEmail);

                if (admin is null)
                {
                    admin = new ApplicationUser
                    {
                        UserName = builder.Configuration["SeedAdmin:UserName"] ?? adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        FirstName = builder.Configuration["SeedAdmin:FirstName"] ?? "Super",
                        LastName = builder.Configuration["SeedAdmin:LastName"] ?? "Admin"
                    };

                    var createAdmin = await userManager.CreateAsync(admin, adminPassword);

                    if (!createAdmin.Succeeded)
                    {
                        throw new InvalidOperationException(
                            "Could not create the configured SuperAdmin account.");
                    }
                }

                if (!await userManager.IsInRoleAsync(admin, RoleConstants.SUPER_ADMIN))
                {
                    var addRole = await userManager.AddToRoleAsync(
                        admin,
                        RoleConstants.SUPER_ADMIN);

                    if (!addRole.Succeeded)
                    {
                        throw new InvalidOperationException(
                            "Could not assign the SuperAdmin role to the configured account.");
                    }
                }
            }
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
