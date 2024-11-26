using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KASCFlightLogging.Data;
using KASCFlightLogging.Models;
using Microsoft.AspNetCore.Authorization;

namespace KASCFlightLogging.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class FlightLogFieldController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlightLogFieldController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FlightLogField
        public async Task<IActionResult> Index(int? aircraftTypeId)
        {
            if (aircraftTypeId == null)
            {
                return NotFound();
            }

            var aircraftType = await _context.AircraftTypes.FindAsync(aircraftTypeId);
            if (aircraftType == null)
            {
                return NotFound();
            }

            ViewBag.AircraftType = aircraftType;
            var fields = await _context.FlightLogFields
                .Where(f => f.AircraftTypeId == aircraftTypeId)
                .OrderBy(f => f.Order)
                .ToListAsync();
            return View(fields);
        }

        // GET: FlightLogField/Create
        public async Task<IActionResult> Create(int? aircraftTypeId)
        {
            if (aircraftTypeId == null)
            {
                return NotFound();
            }

            var aircraftType = await _context.AircraftTypes.FindAsync(aircraftTypeId);
            if (aircraftType == null)
            {
                return NotFound();
            }

            ViewBag.AircraftType = aircraftType;
            return View(new FlightLogField { AircraftTypeId = aircraftTypeId.Value, AircraftType = aircraftType });
        }

        // POST: FlightLogField/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,DisplayText,Description,Required,Order,AircraftTypeId,FieldType")] FlightLogField flightLogField)
        {
            try
            {
                // Load the AircraftType first
                var aircraftType = await _context.AircraftTypes
                    .Include(at => at.FlightLogFields)
                    .FirstOrDefaultAsync(at => at.Id == flightLogField.AircraftTypeId);

                if (aircraftType == null)
                {
                    ModelState.AddModelError("", $"Aircraft Type with ID {flightLogField.AircraftTypeId} not found.");
                    return View(flightLogField);
                }

                // Set default values if not provided
                if (flightLogField.Order == 0)
                {
                    // Set order to be after the last field
                    flightLogField.Order = aircraftType.FlightLogFields.Count + 1;
                }

                // Clear any existing model errors for AircraftType since we'll handle it manually
                ModelState.Remove("AircraftType");

                if (ModelState.IsValid)
                {
                    try
                    {
                        // Create a new context entry for the field
                        var entry = _context.Entry(flightLogField);
                        entry.State = EntityState.Added;

                        // Set the foreign key directly
                        flightLogField.AircraftTypeId = aircraftType.Id;

                        // Save changes
                        await _context.SaveChangesAsync();

                        // Redirect to the index page
                        return RedirectToAction(nameof(Index), new { aircraftTypeId = flightLogField.AircraftTypeId });
                    }
                    catch (DbUpdateException ex)
                    {
                        Console.WriteLine($"Database error: {ex.Message}");
                        Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                        ModelState.AddModelError("", "Unable to save the field. Please try again.");
                    }
                }
                else
                {
                    // Log validation errors for debugging
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            Console.WriteLine($"Validation error: {error.ErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating FlightLogField: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
            }

            // If we got this far, something failed
            ViewBag.AircraftType = await _context.AircraftTypes.FindAsync(flightLogField.AircraftTypeId);
            return View(flightLogField);
        }

        // GET: FlightLogField/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightLogField = await _context.FlightLogFields.FindAsync(id);
            if (flightLogField == null)
            {
                return NotFound();
            }

            var aircraftType = await _context.AircraftTypes.FindAsync(flightLogField.AircraftTypeId);
            ViewBag.AircraftType = aircraftType;
            return View(flightLogField);
        }

        // POST: FlightLogField/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DisplayText,Description,Required,Order,AircraftTypeId,FieldType")] FlightLogField flightLogField)
        {
            if (id != flightLogField.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flightLogField);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightLogFieldExists(flightLogField.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { aircraftTypeId = flightLogField.AircraftTypeId });
            }

            var aircraftType = await _context.AircraftTypes.FindAsync(flightLogField.AircraftTypeId);
            ViewBag.AircraftType = aircraftType;
            return View(flightLogField);
        }

        // GET: FlightLogField/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightLogField = await _context.FlightLogFields
                .Include(f => f.AircraftType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flightLogField == null)
            {
                return NotFound();
            }

            return View(flightLogField);
        }

        // POST: FlightLogField/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flightLogField = await _context.FlightLogFields.FindAsync(id);
            if (flightLogField == null)
            {
                return NotFound();
            }

            var aircraftTypeId = flightLogField.AircraftTypeId;
            _context.FlightLogFields.Remove(flightLogField);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { aircraftTypeId });
        }

        private bool FlightLogFieldExists(int id)
        {
            return _context.FlightLogFields.Any(e => e.Id == id);
        }
    }
}