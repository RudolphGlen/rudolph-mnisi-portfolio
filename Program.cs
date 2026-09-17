using Resend;

// Prepare the website and load its settings.
var builder = WebApplication.CreateBuilder(args);

// Enable MVC: controllers handle requests and views create pages.
builder.Services.AddControllersWithViews();
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

// Match incoming addresses to controller actions.
app.UseRouting();
app.UseAuthorization();

// Serve public CSS, JavaScript, and images from wwwroot.
app.MapStaticAssets();

// The home address / calls HomeController.Index. /Home/Privacy calls Privacy.
// The optional id allows future pages to request a particular item.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Keep the web server running until you stop it with Ctrl+C.
app.Run();
