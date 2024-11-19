using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KASCFlightLogging.Data;
using KASCFlightLogging.Models;

namespace KASCFlightLogging.Controllers;

[Authorize(Roles = "Admin,Staff")]
public class AircraftController : Controller
{
    private readonly ApplicationDbContext _context;

    public AircraftController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Aircraft
    public async Task<IActionResult> Index()
    {
        return View(await _context.Aircraft.OrderBy(a => a.RegistrationNumber).ToListAsync());
    }

    // GET: Aircraft/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aircraft = await _context.Aircraft
            .Include(a => a.FlightLogs)
                .ThenInclude(f => f.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (aircraft == null)
        {
            return NotFound();
        }

        return View(aircraft);
    }

    // GET: Aircraft/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Aircraft/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Aircraft aircraft)
    {
        if (ModelState.IsValid)
        {
            aircraft.CreatedAt = DateTime.UtcNow;
            aircraft.IsActive = true;
            _context.Add(aircraft);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(aircraft);
    }

    // GET: Aircraft/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aircraft = await _context.Aircraft.FindAsync(id);
        if (aircraft == null)
        {
            return NotFound();
        }
        return View(aircraft);
    }

    // POST: Aircraft/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Aircraft aircraft)
    {
        if (id != aircraft.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(aircraft);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AircraftExists(aircraft.Id))
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
        return View(aircraft);
    }

    // POST: Aircraft/ToggleStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var aircraft = await _context.Aircraft.FindAsync(id);
        if (aircraft == null)
        {
            return NotFound();
        }

        aircraft.IsActive = !aircraft.IsActive;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AircraftExists(int id)
    {
        return _context.Aircraft.Any(e => e.Id == id);
    }
}