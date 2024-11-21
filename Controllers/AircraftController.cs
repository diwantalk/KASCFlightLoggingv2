using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KASCFlightLogging.Data;
using KASCFlightLogging.Models;
using Microsoft.Extensions.Logging;

namespace KASCFlightLogging.Controllers;

[Authorize(Roles = "Admin,Staff")]
public class AircraftController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AircraftController> _logger;

    public AircraftController(ApplicationDbContext context, ILogger<AircraftController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Aircraft
    public async Task<IActionResult> Index()
    {
        return View(await _context.Aircraft
            .Include(a => a.AircraftType)
            .OrderBy(a => a.RegistrationNumber)
            .ToListAsync());
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
    public async Task<IActionResult> Create()
    {
        ViewBag.AircraftTypes = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            await _context.AircraftTypes.OrderBy(t => t.Name).ToListAsync(),
            "Id", "Name");
        return View();
    }

    // POST: Aircraft/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("RegistrationNumber,AircraftTypeId")] Aircraft aircraft)
    {
        try
        {
            _logger.LogInformation("Creating aircraft with Registration: {Registration}, TypeId: {TypeId}", 
                aircraft.RegistrationNumber, aircraft.AircraftTypeId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid: {Errors}", 
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                
                ViewBag.AircraftTypes = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await _context.AircraftTypes.OrderBy(t => t.Name).ToListAsync(),
                    "Id", "Name");
                return View(aircraft);
            }

            var aircraftType = await _context.AircraftTypes.FindAsync(aircraft.AircraftTypeId);
            if (aircraftType == null)
            {
                _logger.LogWarning("Aircraft type not found for Id: {TypeId}", aircraft.AircraftTypeId);
                ModelState.AddModelError("AircraftTypeId", "Selected aircraft type not found.");
                ViewBag.AircraftTypes = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    await _context.AircraftTypes.OrderBy(t => t.Name).ToListAsync(),
                    "Id", "Name");
                return View(aircraft);
            }

            // Create new Aircraft instance to ensure clean state
            var newAircraft = new Aircraft
            {
                RegistrationNumber = aircraft.RegistrationNumber,
                AircraftTypeId = aircraft.AircraftTypeId,
                Model = aircraftType.Name,
                Description = null,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                AircraftType = null // Will be loaded by EF Core when needed
            };

            _context.Aircraft.Add(newAircraft);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully created aircraft with Id: {Id}", newAircraft.Id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating aircraft: {Message}", ex.Message);
            ModelState.AddModelError("", "An error occurred while creating the aircraft. Please try again.");
            
            ViewBag.AircraftTypes = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.AircraftTypes.OrderBy(t => t.Name).ToListAsync(),
                "Id", "Name");
            return View(aircraft);
        }
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

        ViewBag.AircraftTypes = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            await _context.AircraftTypes.OrderBy(t => t.Name).ToListAsync(),
            "Id", "Name", aircraft.AircraftTypeId);
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