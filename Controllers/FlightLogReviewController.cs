using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KASCFlightLogging.Data;
using KASCFlightLogging.Models;


namespace KASCFlightLogging.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class FlightLogReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FlightLogReviewController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: FlightLogReview/Review/5
        public async Task<IActionResult> Review(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightLog = await _context.FlightLogs
                .Include(f => f.Aircraft)
                    .ThenInclude(a => a.AircraftType)
                .Include(f => f.Pilot)
                .Include(f => f.Values)
                    .ThenInclude(v => v.FlightLogField)
                .Include(f => f.Reviews)
                    .ThenInclude(r => r.Reviewer)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flightLog == null)
            {
                return NotFound();
            }

            return View(flightLog);
        }

        // POST: FlightLogReview/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string comments)
        {
            var flightLog = await _context.FlightLogs.FindAsync(id);
            if (flightLog == null)
            {
                return NotFound();
            }

            var reviewer = await _userManager.GetUserAsync(User);
            if (reviewer == null)
            {
                return Problem("Unable to get current user.");
            }

            // Create review record
            var review = new FlightReview
            {
                FlightLogId = id,
                ReviewerId = reviewer.Id,
                Status = FlightStatus.Approved,
                Comments = comments,
                ReviewedAt = DateTime.UtcNow
            };

            // Update flight log status
            flightLog.IsPublished = true;
            flightLog.PublishedById = reviewer.Id;
            flightLog.Status = FlightStatus.Approved;
            flightLog.LastModifiedAt = DateTime.UtcNow;
            flightLog.ModifiedById = reviewer.Id;

            _context.FlightReviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "FlightLogs", new { id = id });
        }

        // POST: FlightLogReview/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string comments)
        {
            var flightLog = await _context.FlightLogs.FindAsync(id);
            if (flightLog == null)
            {
                return NotFound();
            }

            var reviewer = await _userManager.GetUserAsync(User);
            if (reviewer == null)
            {
                return Problem("Unable to get current user.");
            }

            // Create review record
            var review = new FlightReview
            {
                FlightLogId = id,
                ReviewerId = reviewer.Id,
                Status = FlightStatus.Rejected,
                Comments = comments,
                ReviewedAt = DateTime.UtcNow
            };

            // Update flight log status
            flightLog.Status = FlightStatus.Rejected;
            flightLog.LastModifiedAt = DateTime.UtcNow;
            flightLog.ModifiedById = reviewer.Id;

            _context.FlightReviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "FlightLogs", new { id = id });
        }

        // POST: FlightLogReview/Complete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id, string comments)
        {
            var flightLog = await _context.FlightLogs.FindAsync(id);
            if (flightLog == null)
            {
                return NotFound();
            }

            var reviewer = await _userManager.GetUserAsync(User);
            if (reviewer == null)
            {
                return Problem("Unable to get current user.");
            }

            // Create review record
            var review = new FlightReview
            {
                FlightLogId = id,
                ReviewerId = reviewer.Id,
                Status = FlightStatus.Completed,
                Comments = comments,
                ReviewedAt = DateTime.UtcNow
            };

            // Update flight log status
            flightLog.Status = FlightStatus.Completed;
            flightLog.LastModifiedAt = DateTime.UtcNow;
            flightLog.ModifiedById = reviewer.Id;

            _context.FlightReviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "FlightLogs", new { id = id });
        }
    }
}