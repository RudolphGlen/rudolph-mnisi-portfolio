using Microsoft.AspNetCore.Identity;
using BeginnerMvc.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Resend;

// Prepare the website and load its settings.
var builder = WebApplication.CreateBuilder(args);

// Enable MVC: controllers handle requests and views create pages.
builder.Services.AddControllersWithViews();

// Limit contact form submissions per client IP to reduce accidental or automated spam.
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("ContactForm", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(15),
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    // Return a clear response and tell the browser when the client can try again.
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.Headers["Retry-After"] = "900";
        context.HttpContext.Response.ContentType = "text/plain; charset=utf-8";
        await context.HttpContext.Response.WriteAsync(
            "Too many contact form submissions. Please try again in 15 minutes.",
            cancellationToken);
    };
});

// Read the database connection string from appsettings.json.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

// Store ASP.NET Core Identity data in SQLite through ApplicationDbContext.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Add the default Identity setup and apply the application's password requirements.
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    // Lock an account for five minutes after five failed password attempts.
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Send protected requests to the custom admin login page and limit cookie lifetime.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Admin/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

// Read the secret only from the running process's environment, never a settings file.
// Restart the website after changing its Windows environment variables.
builder.Services.AddResend(options =>
{
    options.ApiToken = Environment.GetEnvironmentVariable("RESEND_APITOKEN") ?? "";
    options.ThrowExceptions = false;
});
// Dependency injection supplies the real email service to the controller.
builder.Services.AddScoped<BeginnerMvc.Services.IEmailService, BeginnerMvc.Services.ResendEmailService>();
var app = builder.Build();

// Published sites show a friendly error page instead of private error details.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Match incoming addresses to endpoints and register the public static assets.
app.UseRouting();
app.MapStaticAssets();
app.UseRateLimiter();

// Authenticate the request before authorization checks its access to the endpoint.
app.UseAuthentication();
app.UseAuthorization();

// The home address / calls HomeController.Index. /Home/Privacy calls Privacy.
// The optional id allows future pages to request a particular item.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Create the initial administrator account before the web server starts accepting requests.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
    await DbSeeder.SeedAdminAsync(scope.ServiceProvider, builder.Configuration);
}

// Keep the web server running until you stop it with Ctrl+C.
app.Run();
