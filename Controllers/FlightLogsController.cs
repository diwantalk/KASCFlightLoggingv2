using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KASCFlightLogging.Data;
using KASCFlightLogging.Models;
using KASCFlightLogging.Models.ViewModels;
using System.Security.Claims;
using System.Text.Json;

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

        // GET: FlightLogs
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var isPilot = await _userManager.IsInRoleAsync(user, "Pilot");
            var isAdminOrStaff = await _userManager.IsInRoleAsync(user, "Admin") || 
                                await _userManager.IsInRoleAsync(user, "Staff");

            var query = _context.FlightLogs
                .Include(f => f.Aircraft)
                .Include(f => f.Pilot)
                .Include(f => f.Reviews)
                .AsQueryable();

            // Pilots can only see their own logs
            if (isPilot && !isAdminOrStaff)
            {
                query = query.Where(f => f.PilotId == user.Id);
            }

            var flightLogs = await query
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
                    .ThenInclude(a => a.AircraftType)
                .Include(f => f.Pilot)
                .Include(f => f.Reviews)
                    .ThenInclude(r => r.Reviewer)
                .Include(f => f.Values)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flightLog == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isPilot = await _userManager.IsInRoleAsync(currentUser, "Pilot");
            var isAdminOrStaff = await _userManager.IsInRoleAsync(currentUser, "Admin") || 
                                await _userManager.IsInRoleAsync(currentUser, "Staff");

            // Pilots can only view their own logs unless they are also admin/staff
            if (isPilot && !isAdminOrStaff && flightLog.PilotId != currentUser.Id)
            {
                return Forbid();
            }

            return View(flightLog);
        }

        // GET: FlightLogs/Create
        public async Task<IActionResult> Create()
        {
            await PopulateAircraftDropDown();
            if (User.IsInRole("Admin"))
            {
                await PopulatePilotDropDown();
            }
            return View();
        }

        // POST: FlightLogs/CreatePrimary
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePrimary([Bind("FlightDate,AircraftId,PilotInCommandId")] FlightLogCreateViewModel model)
        {
            if (!User.IsInRole("Admin"))
            {
                model.PilotInCommandId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            else if (string.IsNullOrEmpty(model.PilotInCommandId))
            {
                ModelState.AddModelError(nameof(model.PilotInCommandId), "Pilot Id is required!");
                ModelState.AddModelError("", "Please Choose Pilot");
            }

            if (ModelState.IsValid)
            {
                var aircraft = await _context.Aircraft
                    .Include(a => a.AircraftType)
                    .FirstOrDefaultAsync(a => a.Id == model.AircraftId);

                if (aircraft == null)
                {
                    ModelState.AddModelError("AircraftId", "Invalid aircraft selected.");
                    await PopulateAircraftDropDown();
                    if (User.IsInRole("Admin")) await PopulatePilotDropDown();
                    return View("Create", model);
                }

                // Store the essential flight log data in TempData
                var tempData = new FlightLogTempDTO
                {
                    FlightDate = model.FlightDate,
                    AircraftId = model.AircraftId,
                    PilotId = model.PilotInCommandId
                };
                TempData["PendingFlightLog"] = JsonSerializer.Serialize(tempData);

                // Get required fields for this aircraft type
                var fields = await _context.FlightLogFields
                    .Where(f => f.AircraftTypeId == aircraft.AircraftTypeId && f.Required)
                    .OrderBy(f => f.Order)
                    .ToListAsync();

                ViewBag.FlightLogFields = fields;
                return View("CreateDetails", new FlightLog 
                { 
                    FlightDate = model.FlightDate,
                    AircraftId = model.AircraftId,
                    Aircraft = aircraft,
                    PilotId = model.PilotInCommandId
                });
            }

            await PopulateAircraftDropDown(model.AircraftId);
            if (User.IsInRole("Admin")) await PopulatePilotDropDown(model.PilotInCommandId);
            return View("Create", model);
        }

        // POST: FlightLogs/CreateDetails
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDetails(List<FlightLogValue> values)
        {
            var tempDataJson = TempData["PendingFlightLog"]?.ToString();
            if (string.IsNullOrEmpty(tempDataJson))
            {
                return RedirectToAction(nameof(Create));
            }

            var tempData = JsonSerializer.Deserialize<FlightLogTempDTO>(tempDataJson);
            if (tempData == null)
            {
                return RedirectToAction(nameof(Create));
            }

            var aircraft = await _context.Aircraft
                .Include(a => a.AircraftType)
                .ThenInclude(at => at.FlightLogFields)
                .FirstOrDefaultAsync(a => a.Id == tempData.AircraftId);

            if (aircraft == null)
            {
                return RedirectToAction(nameof(Create));
            }

            // Keep the tempData in TempData for the next request if validation fails
            TempData["PendingFlightLog"] = tempDataJson;

            if (ModelState.IsValid)
            {
                // Validate required fields
                var requiredFields = aircraft.AircraftType.FlightLogFields.Where(f => f.Required).ToList();
                foreach (var field in requiredFields)
                {
                    var value = values?.FirstOrDefault(v => v.FlightLogFieldId == field.Id);
                    if (value == null || string.IsNullOrWhiteSpace(value.Value))
                    {
                        ModelState.AddModelError(string.Empty, $"The field '{field.DisplayText}' is required.");
                    }
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.FlightLogFields = requiredFields;
                    return View(new FlightLog
                    {
                        FlightDate = tempData.FlightDate,
                        AircraftId = tempData.AircraftId,
                        Aircraft = aircraft,
                        PilotId = tempData.PilotId
                    });
                }

                var newFlightLog = new FlightLog
                {
                    FlightDate = tempData.FlightDate,
                    AircraftId = tempData.AircraftId,
                    PilotId = tempData.PilotId,
                    Status = FlightStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    Values = values ?? new List<FlightLogValue>(),
                    IsActive = true,
                    IsPublished = false,
                    TotalFlightTime = TimeSpan.Zero // Will be set when departure/arrival times are entered
                };

                _context.Add(newFlightLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.FlightLogFields = aircraft.AircraftType.FlightLogFields.Where(f => f.Required).OrderBy(f => f.Order);
            return View(new FlightLog
            {
                FlightDate = tempData.FlightDate,
                AircraftId = tempData.AircraftId,
                Aircraft = aircraft,
                PilotId = tempData.PilotId
            });
        }

        // GET: FlightLogs/FinalFields/5
        public async Task<IActionResult> FinalFields(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightLog = await _context.FlightLogs
                .Include(f => f.Aircraft)
                    .ThenInclude(a => a.AircraftType)
                        .ThenInclude(at => at.FlightLogFields)
                .Include(f => f.Values)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flightLog == null)
            {
                return NotFound();
            }

            // Only allow editing final fields if the log is active and published
            if (!flightLog.IsActive || !flightLog.IsPublished || flightLog.Status != FlightStatus.Completed)
            {
                TempData["Error"] = "This flight log is not ready for final fields.";
                return RedirectToAction(nameof(Details), new { id = flightLog.Id });
            }

            // Get non-required fields that haven't been filled yet
            ViewBag.FlightLogFields = flightLog.Aircraft.AircraftType.FlightLogFields
                .Where(f => !f.Required && !flightLog.Values.Select(v => v.FlightLogFieldId).Contains(f.Id))
                .OrderBy(f => f.Order);

            return View(flightLog);
        }

        // POST: FlightLogs/FinalFields/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalFields(int id, List<FlightLogValue> newValues)
        {
            var flightLog = await _context.FlightLogs
                .Include(f => f.Aircraft)
                    .ThenInclude(a => a.AircraftType)
                        .ThenInclude(at => at.FlightLogFields)
                .Include(f => f.Values)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flightLog == null)
            {
                return NotFound();
            }

            if (!flightLog.IsActive || !flightLog.IsPublished || flightLog.Status != FlightStatus.Completed)
            {
                TempData["Error"] = "This flight log is not ready for final fields.";
                return RedirectToAction(nameof(Details), new { id = flightLog.Id });
            }

            if (ModelState.IsValid)
            {
                // Get departure and arrival times from the values
                DateTime? departureTime = null;
                DateTime? arrivalTime = null;

                if (newValues != null)
                {
                    foreach (var value in newValues)
                    {
                        // Get the field definition
                        var field = await _context.FlightLogFields.FindAsync(value.FlightLogFieldId);
                        if (field == null) continue;

                        // Handle departure time
                        if (field.Name == StandardFields.DepartureTime && DateTime.TryParse(value.Value, out var depTime))
                        {
                            departureTime = depTime;
                        }
                        // Handle arrival time
                        else if (field.Name == StandardFields.ArrivalTime && DateTime.TryParse(value.Value, out var arrTime))
                        {
                            arrivalTime = arrTime;
                        }

                        value.FlightLogId = id;
                        flightLog.Values.Add(value);
                    }

                    // Calculate total flight time if both times are available
                    if (departureTime.HasValue && arrivalTime.HasValue)
                    {
                        if (arrivalTime.Value > departureTime.Value)
                        {
                            flightLog.TotalFlightTime = arrivalTime.Value - departureTime.Value;
                        }
                        else
                        {
                            ModelState.AddModelError("", "Arrival time must be after departure time.");
                            ViewBag.FlightLogFields = flightLog.Aircraft.AircraftType.FlightLogFields
                                .Where(f => !f.Required && !flightLog.Values.Select(v => v.FlightLogFieldId).Contains(f.Id))
                                .OrderBy(f => f.Order);
                            return View(flightLog);
                        }
                    }
                }

                // Update flight log
                flightLog.LastModifiedAt = DateTime.UtcNow;
                flightLog.ModifiedById = User.FindFirstValue(ClaimTypes.NameIdentifier);
                flightLog.Status = FlightStatus.Approved;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = flightLog.Id });
            }

            // If we got this far, something failed, redisplay form
            ViewBag.FlightLogFields = flightLog.Aircraft.AircraftType.FlightLogFields
                .Where(f => !f.Required && !flightLog.Values.Select(v => v.FlightLogFieldId).Contains(f.Id))
                .OrderBy(f => f.Order);

            return View(flightLog);
        }

        private async Task PopulateAircraftDropDown(int? selectedAircraftId = null)
        {
            var aircraft = await _context.Aircraft
                .Where(a => a.IsActive)
                .OrderBy(a => a.RegistrationNumber)
                .ToListAsync();
            ViewBag.AircraftId = new SelectList(aircraft, "Id", "RegistrationNumber", selectedAircraftId);
        }

        private async Task PopulatePilotDropDown(string? selectedPilotId = null)
        {
            var pilots = await _userManager.GetUsersInRoleAsync("Pilot");
            ViewBag.PilotInCommandId = new SelectList(pilots, "Id", "FullName", selectedPilotId);
        }
    }
}