using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using BeginnerMvc.Data;
using BeginnerMvc.Models;
using BeginnerMvc.Services;

namespace BeginnerMvc.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService _emailService;
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _dbContext;

    // ASP.NET Core supplies these dependencies; we do not create a provider here.
    public HomeController(IEmailService emailService, ILogger<HomeController> logger, ApplicationDbContext dbContext)
    {
        _emailService = emailService;
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpPost]
    [EnableRateLimiting("ContactForm")]
    [ValidateAntiForgeryToken] // Reject submissions without the form's security token.
    public async Task<IActionResult> Contact(
        [Bind(Prefix = "Contact")] ContactFormViewModel contact,
        CancellationToken cancellationToken)
    {
        // Silently accept the page flow but discard submissions that fill the hidden honeypot.
        if (!string.IsNullOrWhiteSpace(contact.Website))
        {
            TempData["ContactSuccess"] = "Thank you for getting in touch. Your message has been sent successfully. I will respond as soon as possible.";
            return RedirectToAction(nameof(Index), "Home", null, "contact");
        }

        // Only bind the four contact fields. Invalid input returns the page with
        // the visitor's entries and MVC's validation messages still available.
        if (!ModelState.IsValid)
            return View("Index", await BuildHomeViewModelAsync(contact, cancellationToken));

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
            return View("Index", await BuildHomeViewModelAsync(contact, cancellationToken));
        }

        // TempData carries a one-time message across the redirect. Redirecting
        // prevents a normal page refresh from submitting the same form again.
        TempData["ContactSuccess"] = "Thank you for getting in touch. Your message has been sent successfully. I will respond as soon as possible.";
        return RedirectToAction(nameof(Index), "Home", null, "contact");
    }

    // Prepare the page's information, then send it to Views/Home/Index.cshtml.
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await BuildHomeViewModelAsync(null, cancellationToken));
    }

    private async Task<HomeViewModel> BuildHomeViewModelAsync(
        ContactFormViewModel? contact,
        CancellationToken cancellationToken)
    {
        return new HomeViewModel
        {
            Contact = contact ?? new ContactFormViewModel(),
            Profile = await _dbContext.SiteProfiles.AsNoTracking()
                .SingleOrDefaultAsync(profile => profile.Id == SiteProfile.DefaultId, cancellationToken)
                ?? new SiteProfile(),
            Education = await _dbContext.Education.AsNoTracking()
                .OrderBy(item => item.DisplayOrder)
                .ThenBy(item => item.Id)
                .ToListAsync(cancellationToken),
            Projects = await _dbContext.Projects.AsNoTracking().OrderBy(item => item.Title).ToListAsync(cancellationToken),
            Skills = await _dbContext.Skills.AsNoTracking().OrderBy(item => item.Category).ThenBy(item => item.Name).ToListAsync(cancellationToken),
            Experiences = await _dbContext.Experiences.AsNoTracking().OrderByDescending(item => item.StartDate).ToListAsync(cancellationToken),
            Certifications = await _dbContext.Certifications.AsNoTracking().OrderByDescending(item => item.DateEarned).ToListAsync(cancellationToken)
        };
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


