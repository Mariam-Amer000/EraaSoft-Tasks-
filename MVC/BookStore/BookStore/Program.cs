
using BookStore.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Globalization;
using System.Text;

namespace BookStore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(
                    "http://127.0.0.1:7139",
                    "http://localhost:7139")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = $"{builder.Configuration["Jwt:Issuer"]}",
                    ValidAudience = $"{builder.Configuration["Jwt:Audience"]}",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            $"{builder.Configuration["Jwt:Key"]}"))
                };
            });

        builder.Services.AddLocalization(options =>
            options.ResourcesPath = "Recourses");

        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IDbInitializer, DbInitializer>();

        builder.Services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
            options.Lockout.MaxFailedAccessAttempts = 6;
            options.SignIn.RequireConfirmedEmail = true;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddTransient<IEmailSender, EmailSender>();

        const string defaultCulture = "en";

        var supportedCultures = new[]
        {
            new CultureInfo(defaultCulture),
            new CultureInfo("ar")
        };

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(defaultCulture);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

        builder.Services.Configure<StripeSettings>(
            builder.Configuration.GetSection("Stripe"));

        var app = builder.Build();

        app.UseStaticFiles();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRequestLocalization(
            app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            dbInitializer.Initialize(); // Runs migrations and seeds data
        }

        app.Run();
    }
}
