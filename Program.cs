using Microsoft.AspNetCore.Identity;
using BeginnerMvc.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Resend;

// Prepare the website and load its settings.
var builder = WebApplication.CreateBuilder(args);

// Enable MVC: controllers handle requests and views create pages.
builder.Services.AddControllersWithViews();

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
