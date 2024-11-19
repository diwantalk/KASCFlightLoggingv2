using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using KASCFlightLogging.Models;

namespace KASCFlightLogging.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        return user.Role switch
        {
            UserRole.Admin => RedirectToAction(nameof(AdminDashboard)),
            UserRole.Staff => RedirectToAction(nameof(StaffDashboard)),
            UserRole.Pilot => RedirectToAction(nameof(PilotDashboard)),
            UserRole.Member => RedirectToAction(nameof(MemberDashboard)),
            _ => RedirectToAction(nameof(MemberDashboard))
        };
    }

    [Authorize(Roles = "Admin")]
    public IActionResult AdminDashboard()
    {
        return View();
    }

    [Authorize(Roles = "Staff")]
    public IActionResult StaffDashboard()
    {
        return View();
    }

    [Authorize(Roles = "Pilot")]
    public IActionResult PilotDashboard()
    {
        return View();
    }

    [Authorize(Roles = "Member")]
    public IActionResult MemberDashboard()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
