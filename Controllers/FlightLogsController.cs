using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KASCFlightLogging.Data;
using KASCFlightLogging.Models;
using System.Security.Claims;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.Packaging;
using KASCFlightLogging.Models.ViewModels;

namespace KASCFlightLogging.Controllers
{
    [Authorize(Roles = "Admin,Staff,Pilot")]
    public class FlightLogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FlightLogsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: FlightLogs/Create
        public async Task<IActionResult> Create()
        {
            // Load Aircraft Types without IsActive filter
            ViewBag.AircraftTypes = await _context.AircraftTypes
                .OrderBy(at => at.Name)
                .ToListAsync();

            // Load Pilots if user is Admin
            if (User.IsInRole("Admin"))
            {
                var pilots = await _userManager.GetUsersInRoleAsync("Pilot");
                ViewBag.PilotInCommandId = new SelectList(
                    pilots.OrderBy(p => p.LastName),
                    "Id",
                    "FullName"
                );
            }

            return View(new FlightLogCreateViewModel());
        }

        // POST: FlightLogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InitialFlightLogViewModel model)
        {

            if (ModelState.IsValid)
            {
                var requiredFields = await _context.FlightLogFields
                    .Where(f => f.AircraftTypeId == model.AircraftTypeId && f.Required)
                    .OrderBy(f => f.Order)
                    .ToListAsync();

                model.RequiredFields = requiredFields;
                return View("CreateRequiredFields", model);
            }
            return View(model);
        }

        // POST: FlightLogs/CreateRequiredFields
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequiredFields(InitialFlightLogViewModel model, List<FlightLogValue> values)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var flightLog = new FlightLog
                {
                    FlightDate = model.FlightDate,
                    PilotId = currentUser.Id,
                    Status = FlightStatus.PendingReview,
                    IsActive = false,
                    IsPublished = false,
                    Values = values
                };

                _context.FlightLogs.Add(flightLog);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // POST: FlightLogs/Review/5
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public async Task<IActionResult> Review(int id, bool approved, string comments)
        {
            var flightLog = await _context.FlightLogs.FindAsync(id);
            if (flightLog == null)
                return NotFound();

            if (approved)
            {
                flightLog.Status = FlightStatus.Approved;
                flightLog.IsActive = true;
                flightLog.IsPublished = true;
            }
            else
            {
                flightLog.Status = FlightStatus.Rejected;
            }

            // Add review comment
            var review = new FlightReview
            {
                FlightLogId = id,
                ReviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Comments = comments,
                ReviewedAt = DateTime.UtcNow,
                Status = approved ? FlightStatus.Approved : FlightStatus.Rejected
            };
            _context.FlightReviews.Add(review);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: FlightLogs/FinalFields/5
        [Authorize(Roles = "Pilot")]
        public async Task<IActionResult> FinalFields(int id)
        {
            var flightLog = await _context.FlightLogs
                .Include(f => f.Aircraft)
                .ThenInclude(a => a.AircraftType)
                .ThenInclude(at => at.FlightLogFields)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flightLog == null || flightLog.Status != FlightStatus.Approved)
                return NotFound();

            var nonRequiredFields = flightLog.Aircraft.AircraftType.FlightLogFields
                .Where(f => !f.Required)
                .OrderBy(f => f.Order);

            ViewBag.NonRequiredFields = nonRequiredFields;
            return View(flightLog);
        }

        // POST: FlightLogs/FinalFields/5
        [HttpPost]
        [Authorize(Roles = "Pilot")]
        public async Task<IActionResult> FinalFields(int id, List<FlightLogValue> finalValues)
        {
            var flightLog = await _context.FlightLogs.FindAsync(id);
            if (flightLog == null)
                return NotFound();

            flightLog.Values.AddRange(finalValues);
            flightLog.Status = FlightStatus.FinalReview;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: FlightLogs/FinalReview/5
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> FinalReview(int id, bool approved, string comments)
        {
            var flightLog = await _context.FlightLogs.FindAsync(id);
            if (flightLog == null)
                return NotFound();

            flightLog.Status = approved ? FlightStatus.Completed : FlightStatus.Approved;

            var review = new FlightReview
            {
                FlightLogId = id,
                ReviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Comments = comments,
                ReviewedAt = DateTime.UtcNow,
                Status = approved ? FlightStatus.Completed : FlightStatus.Approved
            };
            _context.FlightReviews.Add(review);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}