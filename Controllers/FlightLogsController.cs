using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KASCFlightLogging.Data;
using KASCFlightLogging.Models;

namespace KASCFlightLogging.Controllers
{
    [Authorize]
    public class FlightLogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FlightLogsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: FlightLogs
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var flightLogs = await _context.FlightLogs
                .Include(f => f.Aircraft)
                .Include(f => f.User)
                .Include(f => f.Reviews)
                .Where(f => f.UserId == user.Id)
                .OrderByDescending(f => f.FlightDate)
                .ToListAsync();

            return View(flightLogs);
        }

        // GET: FlightLogs/Details/5
        public async Task<IActionResult> Details(int? id)
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

            var currentUser = await GetCurrentUserAsync();
            if (flightLog.UserId != currentUser.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(flightLog);
        }

        // GET: FlightLogs/Create
        public async Task<IActionResult> Create()
        {
            await PopulateAircraftDropDown();
            return View();
        }

        // POST: FlightLogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FlightDate,AircraftId,DepartureLocation,ArrivalLocation,Remarks")] FlightLog flightLog)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                flightLog.UserId = user.Id;
                flightLog.Status = FlightStatus.Draft;
                flightLog.CreatedAt = DateTime.UtcNow;

                _context.Add(flightLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateAircraftDropDown(flightLog.AircraftId);
            return View(flightLog);
        }

        // GET: FlightLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightLog = await _context.FlightLogs
                .Include(f => f.Aircraft)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flightLog == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (flightLog.UserId != currentUser.Id)
            {
                return Forbid();
            }

            if (flightLog.Status != FlightStatus.Draft)
            {
                TempData["Error"] = "Only draft logs can be edited.";
                return RedirectToAction(nameof(Details), new { id = flightLog.Id });
            }

            await PopulateAircraftDropDown(flightLog.AircraftId);
            return View(flightLog);
        }

        // POST: FlightLogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FlightDate,AircraftId,DepartureLocation,ArrivalLocation,Remarks,UserId,Status")] FlightLog flightLog)
        {
            if (id != flightLog.Id)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (flightLog.UserId != currentUser.Id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingLog = await _context.FlightLogs.FindAsync(id);
                    if (existingLog == null)
                    {
                        return NotFound();
                    }

                    if (existingLog.Status != FlightStatus.Draft)
                    {
                        TempData["Error"] = "Only draft logs can be edited.";
                        return RedirectToAction(nameof(Details), new { id = flightLog.Id });
                    }

                    existingLog.FlightDate = flightLog.FlightDate;
                    existingLog.AircraftId = flightLog.AircraftId;
                    existingLog.DepartureLocation = flightLog.DepartureLocation;
                    existingLog.ArrivalLocation = flightLog.ArrivalLocation;
                    existingLog.Remarks = flightLog.Remarks;
                    existingLog.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightLogExists(flightLog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            await PopulateAircraftDropDown(flightLog.AircraftId);
            return View(flightLog);
        }

        // GET: FlightLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightLog = await _context.FlightLogs
                .Include(f => f.Aircraft)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (flightLog == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (flightLog.UserId != currentUser.Id)
            {
                return Forbid();
            }

            if (flightLog.Status != FlightStatus.Draft)
            {
                TempData["Error"] = "Only draft logs can be deleted.";
                return RedirectToAction(nameof(Details), new { id = flightLog.Id });
            }

            return View(flightLog);
        }

        // POST: FlightLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flightLog = await _context.FlightLogs
                .Include(f => f.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (flightLog == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (flightLog.UserId != currentUser.Id)
            {
                return Forbid();
            }

            if (flightLog.Status != FlightStatus.Draft)
            {
                TempData["Error"] = "Only draft logs can be deleted.";
                return RedirectToAction(nameof(Details), new { id = flightLog.Id });
            }

            _context.FlightLogs.Remove(flightLog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: FlightLogs/Submit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int id)
        {
            var flightLog = await _context.FlightLogs.FindAsync(id);
            if (flightLog == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (flightLog.UserId != currentUser.Id)
            {
                return Forbid();
            }

            if (flightLog.Status != FlightStatus.Draft)
            {
                TempData["Error"] = "Only draft logs can be submitted.";
                return RedirectToAction(nameof(Details), new { id = flightLog.Id });
            }

            flightLog.Status = FlightStatus.PendingReview;
            flightLog.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = flightLog.Id });
        }

        private async Task PopulateAircraftDropDown(int? selectedAircraftId = null)
        {
            var aircraft = await _context.Aircraft
                .Where(a => a.IsActive)
                .OrderBy(a => a.RegistrationNumber)
                .ToListAsync();

            ViewBag.AircraftId = new SelectList(aircraft, "Id", "RegistrationNumber", selectedAircraftId);
        }

        private bool FlightLogExists(int id)
        {
            return _context.FlightLogs.Any(e => e.Id == id);
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User) ?? throw new InvalidOperationException("User not found");
        }
    }
}