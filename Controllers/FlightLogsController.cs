using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KASCFlightLogging.Data;
using KASCFlightLogging.Models;

namespace KASCFlightLogging.Controllers;

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
        var user = await _userManager.GetUserAsync(User);
        var flightLogs = await _context.FlightLogs
            .Include(f => f.Aircraft)
            .Include(f => f.User)
            .Where(f => f.UserId == user.Id || user.Role == UserRole.Admin || user.Role == UserRole.Staff)
            .OrderByDescending(f => f.CreatedAt)
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

        var currentUser = await _userManager.GetUserAsync(User);
        if (flightLog.UserId != currentUser.Id && currentUser.Role != UserRole.Admin && currentUser.Role != UserRole.Staff)
        {
            return Forbid();
        }

        return View(flightLog);
    }

    // GET: FlightLogs/Create
    public async Task<IActionResult> Create()
    {
        var aircraft = await _context.Aircraft
            .Where(a => a.IsActive)
            .OrderBy(a => a.RegistrationNumber)
            .ToListAsync();

        ViewBag.Aircraft = aircraft;
        return View();
    }

    // POST: FlightLogs/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FlightLog flightLog)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            flightLog.UserId = user.Id;
            flightLog.CreatedAt = DateTime.UtcNow;
            flightLog.Status = FlightStatus.Draft;

            _context.Add(flightLog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        var aircraft = await _context.Aircraft
            .Where(a => a.IsActive)
            .OrderBy(a => a.RegistrationNumber)
            .ToListAsync();

        ViewBag.Aircraft = aircraft;
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
        if (flightLog.UserId != currentUser.Id && currentUser.Role != UserRole.Admin && currentUser.Role != UserRole.Staff)
        {
            return Forbid();
        }

        var aircraft = await _context.Aircraft
            .Where(a => a.IsActive)
            .OrderBy(a => a.RegistrationNumber)
            .ToListAsync();

        ViewBag.Aircraft = aircraft;
        return View(flightLog);
    }

    // POST: FlightLogs/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FlightLog flightLog)
    {
        if (id != flightLog.Id)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var existingLog = await _context.FlightLogs.FindAsync(id);

        if (existingLog == null)
        {
            return NotFound();
        }

        if (existingLog.UserId != currentUser.Id && currentUser.Role != UserRole.Admin && currentUser.Role != UserRole.Staff)
        {
            return Forbid();
        }

        if (ModelState.IsValid)
        {
            try
            {
                existingLog.AircraftId = flightLog.AircraftId;
                existingLog.DepartureTime = flightLog.DepartureTime;
                existingLog.ArrivalTime = flightLog.ArrivalTime;
                existingLog.DepartureLocation = flightLog.DepartureLocation;
                existingLog.ArrivalLocation = flightLog.ArrivalLocation;
                existingLog.FlightPurpose = flightLog.FlightPurpose;
                existingLog.Notes = flightLog.Notes;
                existingLog.FlightDuration = flightLog.FlightDuration;
                existingLog.LastModifiedAt = DateTime.UtcNow;
                existingLog.FlightConfiguration = flightLog.FlightConfiguration;
                existingLog.FuelUsed = flightLog.FuelUsed;
                existingLog.WeatherConditions = flightLog.WeatherConditions;
                existingLog.PerformanceData = flightLog.PerformanceData;

                await _context.SaveChangesAsync();
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
            return RedirectToAction(nameof(Index));
        }

        var aircraft = await _context.Aircraft
            .Where(a => a.IsActive)
            .OrderBy(a => a.RegistrationNumber)
            .ToListAsync();

        ViewBag.Aircraft = aircraft;
        return View(flightLog);
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
            return BadRequest("Only draft flight logs can be submitted for review.");
        }

        flightLog.Status = FlightStatus.PendingReview;
        flightLog.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST: FlightLogs/Resubmit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Resubmit(int id)
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
            return BadRequest("Only draft flight logs can be resubmitted for review.");
        }

        flightLog.Status = FlightStatus.PendingReview;
        flightLog.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool FlightLogExists(int id)
    {
        return _context.FlightLogs.Any(e => e.Id == id);
    }
}