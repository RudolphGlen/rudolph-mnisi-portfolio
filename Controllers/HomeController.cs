using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BeginnerMvc.Models;
using BeginnerMvc.Services;

namespace BeginnerMvc.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService _emailService;
    private readonly ILogger<HomeController> _logger;

    // ASP.NET Core supplies these dependencies; we do not create a provider here.
    public HomeController(IEmailService emailService, ILogger<HomeController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken] // Reject submissions without the form's security token.
    public async Task<IActionResult> Contact(
        [Bind(Prefix = "Contact")] ContactFormViewModel contact,
        CancellationToken cancellationToken)
    {
        // Only bind the four contact fields. Invalid input returns the page with
        // the visitor's entries and MVC's validation messages still available.
        if (!ModelState.IsValid)
            return View("Index", new HomeViewModel { Contact = contact });

        try
        {
            await _emailService.SendAsync(contact, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            // Keep visitor-facing errors generic. The email service logs sanitized
            // provider diagnostics separately, and only in Development mode.
            _logger.LogError("Contact form processing failed.");
            ModelState.AddModelError("", "We could not process your message. Please try again later.");
            return View("Index", new HomeViewModel { Contact = contact });
        }

        // TempData carries a one-time message across the redirect. Redirecting
        // prevents a normal page refresh from submitting the same form again.
        TempData["ContactSuccess"] = "Thank you for getting in touch. Your message has been sent successfully. I will respond as soon as possible.";
        return RedirectToAction(nameof(Index), "Home", null, "contact");
    }

    // Prepare the page's information, then send it to Views/Home/Index.cshtml.
    public IActionResult Index()
    {
        var model = new HomeViewModel();
        return View(model);
    }

    // MVC uses this method's name to find Views/Home/Privacy.cshtml.
    public IActionResult Privacy()
    {
        return View();
    }

    // Avoid storing error pages in the browser. A tracking ID helps diagnose
    // the request without displaying private exception details to visitors.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


