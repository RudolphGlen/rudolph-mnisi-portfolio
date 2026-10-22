using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeginnerMvc.Data;
using BeginnerMvc.Models;

namespace BeginnerMvc.Controllers;

// Require membership in the Admin role for every admin action; unauthenticated requests
// are redirected by the Identity cookie handler to the configured login page.
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _dbContext;

    // ASP.NET Core's dependency injection provides the Identity services used for sign-in.
    public AdminController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        ApplicationDbContext dbContext)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    // Visitors who are not signed in must be able to reach the login page.
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // Verify credentials with Identity and redirect to the admin dashboard on success.
    // Allow credential submission without an existing authentication cookie.
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string userName, string password)
    {
        // PasswordSignInAsync expects a username. Resolve email addresses to the user's
        // Identity username so the login form can accept either identifier.
        var user = await _userManager.FindByEmailAsync(userName);
        var identityUserName = user?.UserName ?? userName;

        // SignInManager checks the password against Identity's stored password hash and
        // creates the authentication cookie when the credentials are valid.
        // Count failed passwords so Identity can apply the configured account lockout.
        var result = await _signInManager.PasswordSignInAsync(
            identityUserName,
            password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(
            string.Empty,
            result.IsLockedOut
                ? "This account is temporarily locked after too many failed attempts. Please try again in five minutes."
                : "Invalid login attempt.");
        return View();
    }

    // Clear the user's authentication cookie and return to the home page.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // The existing MVC route maps /Admin to this dashboard view.
    public IActionResult Index()
    {
        return View();
    }

    // List actions read the saved records without tracking them for changes.
    [HttpGet]
    public async Task<IActionResult> Projects(CancellationToken cancellationToken)
    {
        var projects = await _dbContext.Projects
            .AsNoTracking()
            .OrderBy(project => project.Title)
            .ToListAsync(cancellationToken);
        return View(projects);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProject(
        [Bind("Title,Description,Technologies,ProjectUrl,GithubUrl")] Project project,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _dbContext.Projects.AddAsync(project, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Projects));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProject(
        int id,
        [Bind("Title,Description,Technologies,ProjectUrl,GithubUrl")] Project submittedProject,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var project = await _dbContext.Projects
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (project is null)
            return NotFound();

        project.Title = submittedProject.Title;
        project.Description = submittedProject.Description;
        project.Technologies = submittedProject.Technologies;
        project.ProjectUrl = submittedProject.ProjectUrl;
        project.GithubUrl = submittedProject.GithubUrl;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Projects));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProject(int id, CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (project is null)
            return NotFound();

        _dbContext.Projects.Remove(project);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Projects));
    }

    [HttpGet]
    public async Task<IActionResult> Certifications(CancellationToken cancellationToken)
    {
        var certifications = await _dbContext.Certifications
            .AsNoTracking()
            .OrderBy(certification => certification.Title)
            .ToListAsync(cancellationToken);
        return View(certifications);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCertification(
        [Bind("Title,Issuer,DateEarned,CertificateUrl")] Certification certification,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _dbContext.Certifications.AddAsync(certification, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Certifications));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCertification(
        int id,
        [Bind("Title,Issuer,DateEarned,CertificateUrl")] Certification submittedCertification,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var certification = await _dbContext.Certifications
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (certification is null)
            return NotFound();

        certification.Title = submittedCertification.Title;
        certification.Issuer = submittedCertification.Issuer;
        certification.DateEarned = submittedCertification.DateEarned;
        certification.CertificateUrl = submittedCertification.CertificateUrl;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Certifications));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCertification(int id, CancellationToken cancellationToken)
    {
        var certification = await _dbContext.Certifications
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (certification is null)
            return NotFound();

        _dbContext.Certifications.Remove(certification);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Certifications));
    }

    [HttpGet]
    public async Task<IActionResult> Skills(CancellationToken cancellationToken)
    {
        var skills = await _dbContext.Skills
            .AsNoTracking()
            .OrderBy(skill => skill.Name)
            .ToListAsync(cancellationToken);
        return View(skills);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSkill(
        [Bind("Name,Category,ProficiencyLevel")] Skill skill,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _dbContext.Skills.AddAsync(skill, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Skills));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSkill(
        int id,
        [Bind("Name,Category,ProficiencyLevel")] Skill submittedSkill,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var skill = await _dbContext.Skills
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (skill is null)
            return NotFound();

        skill.Name = submittedSkill.Name;
        skill.Category = submittedSkill.Category;
        skill.ProficiencyLevel = submittedSkill.ProficiencyLevel;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Skills));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSkill(int id, CancellationToken cancellationToken)
    {
        var skill = await _dbContext.Skills
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (skill is null)
            return NotFound();

        _dbContext.Skills.Remove(skill);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Skills));
    }

    [HttpGet]
    public async Task<IActionResult> Experience(CancellationToken cancellationToken)
    {
        var experiences = await _dbContext.Experiences
            .AsNoTracking()
            .OrderByDescending(experience => experience.StartDate)
            .ToListAsync(cancellationToken);
        return View(experiences);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExperience(
        [Bind("Company,Role,StartDate,EndDate,Description,IsCurrentRole")] Experience experience,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _dbContext.Experiences.AddAsync(experience, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Experience));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditExperience(
        int id,
        [Bind("Company,Role,StartDate,EndDate,Description,IsCurrentRole")] Experience submittedExperience,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var experience = await _dbContext.Experiences
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (experience is null)
            return NotFound();

        experience.Company = submittedExperience.Company;
        experience.Role = submittedExperience.Role;
        experience.StartDate = submittedExperience.StartDate;
        experience.EndDate = submittedExperience.EndDate;
        experience.Description = submittedExperience.Description;
        experience.IsCurrentRole = submittedExperience.IsCurrentRole;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Experience));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteExperience(int id, CancellationToken cancellationToken)
    {
        var experience = await _dbContext.Experiences
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (experience is null)
            return NotFound();

        _dbContext.Experiences.Remove(experience);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Experience));
    }

    // Load the single profile record that supplies the public Home, About, and Contact sections.
    [HttpGet]
    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        var profile = await _dbContext.SiteProfiles.AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == SiteProfile.DefaultId, cancellationToken);
        return profile is null ? NotFound() : View(profile);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(
        [Bind("Name,Initials,ProfessionalTitle,Introduction,Summary,AboutMore,Email,Phone,PhoneLink,ContactLocation,Location,Availability,ProfileImageUrl,CvUrl")]
        SiteProfile submittedProfile,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View("Profile", submittedProfile);

        var profile = await _dbContext.SiteProfiles
            .SingleOrDefaultAsync(item => item.Id == SiteProfile.DefaultId, cancellationToken);
        if (profile is null)
            return NotFound();

        // Copy only editable public fields; the database key is never accepted from the form.
        profile.Name = submittedProfile.Name;
        profile.Initials = submittedProfile.Initials;
        profile.ProfessionalTitle = submittedProfile.ProfessionalTitle;
        profile.Introduction = submittedProfile.Introduction;
        profile.Summary = submittedProfile.Summary;
        profile.AboutMore = submittedProfile.AboutMore;
        profile.Email = submittedProfile.Email;
        profile.Phone = submittedProfile.Phone;
        profile.PhoneLink = submittedProfile.PhoneLink;
        profile.ContactLocation = submittedProfile.ContactLocation;
        profile.Location = submittedProfile.Location;
        profile.Availability = submittedProfile.Availability;
        profile.ProfileImageUrl = submittedProfile.ProfileImageUrl;
        profile.CvUrl = submittedProfile.CvUrl;

        await _dbContext.SaveChangesAsync(cancellationToken);
        TempData["AdminNotice"] = "Profile details saved.";
        return RedirectToAction(nameof(Profile));
    }

    // List education records in the same order they appear in the public portfolio.
    [HttpGet]
    public async Task<IActionResult> Education(CancellationToken cancellationToken)
    {
        var education = await _dbContext.Education.AsNoTracking()
            .OrderBy(item => item.DisplayOrder)
            .ThenBy(item => item.Id)
            .ToListAsync(cancellationToken);
        return View(education);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEducation(
        [Bind("Qualification,Institution,Dates,Description,DisplayOrder")] Education education,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _dbContext.Education.AddAsync(education, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Education));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEducation(
        int id,
        [Bind("Qualification,Institution,Dates,Description,DisplayOrder")] Education submittedEducation,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var education = await _dbContext.Education
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (education is null)
            return NotFound();

        education.Qualification = submittedEducation.Qualification;
        education.Institution = submittedEducation.Institution;
        education.Dates = submittedEducation.Dates;
        education.Description = submittedEducation.Description;
        education.DisplayOrder = submittedEducation.DisplayOrder;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Education));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEducation(int id, CancellationToken cancellationToken)
    {
        var education = await _dbContext.Education
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (education is null)
            return NotFound();

        _dbContext.Education.Remove(education);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Education));
    }
}
