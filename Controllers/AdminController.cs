using Microsoft.AspNetCore.Mvc;

namespace BeginnerMvc.Controllers;

public class AdminController : Controller
{
    // UI only: authentication will be added later. Existing Admin pages remain accessible.
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // The existing MVC route maps /Admin to this dashboard view.
    public IActionResult Index()
    {
        return View();
    }

    // Each action displays its matching placeholder view in Views/Admin.
    public IActionResult Projects()
    {
        return View();
    }

    public IActionResult Certifications()
    {
        return View();
    }

    public IActionResult Skills()
    {
        return View();
    }

    public IActionResult Experience()
    {
        return View();
    }
}
