using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KASCFlightLogging.Data;
using KASCFlightLogging.Models;

namespace KASCFlightLogging.Controllers;

[Authorize(Roles = "Admin,Staff")]
public class ReviewsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Review(int id, string comments, bool approved)
    {
        var flightLog = await _context.FlightLogs
            .Include(f => f.Reviews)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (flightLog == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var review = new FlightReview
        {
            FlightLogId = id,
            ReviewerId = user.Id,
            Comments = comments,
            ReviewedAt = DateTime.UtcNow,
            Status = approved ? FlightStatus.Approved : FlightStatus.Rejected
        };

        flightLog.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}