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

    // GET: Reviews
    public async Task<IActionResult> Index()
    {
        var pendingLogs = await _context.FlightLogs
            .Include(f => f.Aircraft)
            .Include(f => f.User)
            .Include(f => f.Reviews)
            .Where(f => f.Status == FlightStatus.PendingReview)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

        return View(pendingLogs);
    }

    // GET: Reviews/Review/5
    public async Task<IActionResult> Review(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var flightLog = await _context.FlightLogs
            .Include(f => f.Aircraft)
            .Include(f => f.User)
            .Include(f => f.Reviews)
                .ThenInclude(r => r.Reviewer)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (flightLog == null)
        {
            return NotFound();
        }

        if (flightLog.Status != FlightStatus.PendingReview)
        {
            return BadRequest("This flight log is not pending review.");
        }

        return View(flightLog);
    }

    // POST: Reviews/Approve/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id, string comments)
    {
        var flightLog = await _context.FlightLogs.FindAsync(id);
        if (flightLog == null)
        {
            return NotFound();
        }

        if (flightLog.Status != FlightStatus.PendingReview)
        {
            return BadRequest("This flight log is not pending review.");
        }

        var reviewer = await _userManager.GetUserAsync(User);
        var review = new FlightReview
        {
            FlightLogId = id,
            ReviewerId = reviewer.Id,
            Status = ReviewStatus.Approved,
            Comments = comments,
            ReviewedAt = DateTime.UtcNow
        };

        flightLog.Status = FlightStatus.Approved;
        flightLog.LastModifiedAt = DateTime.UtcNow;

        _context.FlightReviews.Add(review);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // POST: Reviews/Reject/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, string comments)
    {
        if (string.IsNullOrWhiteSpace(comments))
        {
            return BadRequest("Comments are required when rejecting a flight log.");
        }

        var flightLog = await _context.FlightLogs.FindAsync(id);
        if (flightLog == null)
        {
            return NotFound();
        }

        if (flightLog.Status != FlightStatus.PendingReview)
        {
            return BadRequest("This flight log is not pending review.");
        }

        var reviewer = await _userManager.GetUserAsync(User);
        var review = new FlightReview
        {
            FlightLogId = id,
            ReviewerId = reviewer.Id,
            Status = ReviewStatus.Rejected,
            Comments = comments,
            ReviewedAt = DateTime.UtcNow
        };

        flightLog.Status = FlightStatus.Draft;
        flightLog.LastModifiedAt = DateTime.UtcNow;

        _context.FlightReviews.Add(review);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}