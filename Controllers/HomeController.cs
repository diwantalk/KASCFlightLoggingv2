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
    private readonly SignInManager<ApplicationUser> _signInManager;

    public HomeController(
        ILogger<HomeController> logger,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
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
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);
        
        // Redirect based on role hierarchy
        if (roles.Contains("Admin"))
            return RedirectToAction(nameof(AdminDashboard));
        if (roles.Contains("Staff"))
            return RedirectToAction(nameof(StaffDashboard));
        if (roles.Contains("Pilot"))
            return RedirectToAction(nameof(MemberDashboard)); // Pilots use Member dashboard
        if (roles.Contains("Member"))
            return RedirectToAction(nameof(MemberDashboard));

        // Default to member dashboard if no specific role is found
        return RedirectToAction(nameof(MemberDashboard));
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
        // Redirect pilots to member dashboard
        return RedirectToAction(nameof(MemberDashboard));
    }

    [Authorize(Roles = "Member,Pilot")] // Allow both Members and Pilots
    public IActionResult MemberDashboard()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    public async Task<IActionResult> AccessDenied()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        // Get user roles and redirect to appropriate dashboard
        var roles = await _userManager.GetRolesAsync(user);
        
        if (roles.Contains("Admin"))
            return RedirectToAction(nameof(AdminDashboard));
        if (roles.Contains("Staff"))
            return RedirectToAction(nameof(StaffDashboard));
        if (roles.Contains("Pilot") || roles.Contains("Member"))
            return RedirectToAction(nameof(MemberDashboard));

        return RedirectToAction(nameof(MemberDashboard));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
