using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KASCFlightLogging.Data;
using KASCFlightLogging.Models;
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

        // GET: FlightLogs
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var isPilot = await _userManager.IsInRoleAsync(user, "Pilot");
            var isAdminOrStaff = await _userManager.IsInRoleAsync(user, "Admin") || 
                                await _userManager.IsInRoleAsync(user, "Staff");

            var query = _context.FlightLogs
                .Include(f => f.Aircraft)
                .Include(f => f.User)
                .Include(f => f.Reviews)
                .AsQueryable();

            // Pilots can only see their own logs
            if (isPilot && !isAdminOrStaff)
            {
                query = query.Where(f => f.UserId == user.Id);
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
                .Include(f => f.User)
                .Include(f => f.Reviews)
                    .ThenInclude(r => r.Reviewer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (flightLog == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();
            var isPilot = await _userManager.IsInRoleAsync(currentUser, "Pilot");
            var isAdminOrStaff = await _userManager.IsInRoleAsync(currentUser, "Admin") || 
                                await _userManager.IsInRoleAsync(currentUser, "Staff");

            // Pilots can only view their own logs unless they are also admin/staff
            if (isPilot && !isAdminOrStaff && flightLog.UserId != currentUser.Id)
            {
                return Forbid();
            }

            return View(flightLog);
        }

        // GET: FlightLogs/Create
        public async Task<IActionResult> Create()
        {
            await PopulateAircraftDropDown();
            await PopulatePilotDropDown();
            return View();
        }

        // POST: FlightLogs/CreatePrimary
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePrimary([Bind("FlightDate,AircraftId,PilotInCommandId,DepartureLocation,ArrivalLocation,Remarks")] FlightLogCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var aircraft = await _context.Aircraft
                    .Include(a => a.AircraftType)
                    .FirstOrDefaultAsync(a => a.Id == model.AircraftId);

                if (aircraft == null)
                {
                    ModelState.AddModelError("AircraftId", "Invalid aircraft selected.");
                    await PopulateAircraftDropDown();
                    await PopulatePilotDropDown();
                    return View("Create", model);
                }

                var user = await _userManager.GetUserAsync(User);
                var flightLog = new FlightLog
                {
                    FlightDate = model.FlightDate,
                    AircraftId = model.AircraftId,
                    Aircraft = aircraft,
                    UserId = user.Id,
                    User = user,
                    DepartureLocation = model.DepartureLocation,
                    ArrivalLocation = model.ArrivalLocation,
                    Remarks = model.Remarks,
                    Status = FlightStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    Values = []
                };

                // Store the essential flight log data in TempData
                var tempData = new FlightLogTempDTO
                {
                    FlightDate = flightLog.FlightDate,
                    AircraftId = flightLog.AircraftId,
                    UserId = flightLog.UserId,
                    DepartureLocation = flightLog.DepartureLocation,
                    ArrivalLocation = flightLog.ArrivalLocation
                };
                TempData["PendingFlightLog"] = System.Text.Json.JsonSerializer.Serialize(tempData);

                // Get fields for this aircraft type
                var fields = await _context.FlightLogFields
                    .Where(f => f.AircraftTypeId == aircraft.AircraftTypeId)
                    .OrderBy(f => f.Order)
                    .ToListAsync();

                ViewBag.FlightLogFields = fields;
                return View("CreateDetails", flightLog);
            }

            await PopulateAircraftDropDown(model.AircraftId);
            await PopulatePilotDropDown(model.PilotInCommandId);
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

            var tempData = System.Text.Json.JsonSerializer.Deserialize<FlightLogTempDTO>(tempDataJson);
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

            // Get the user first as we'll need it in multiple places
            var user = await _userManager.FindByIdAsync(tempData.UserId);
            if (user == null)
            {
                return RedirectToAction(nameof(Create));
            }

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
                    ViewBag.FlightLogFields = aircraft.AircraftType.FlightLogFields.OrderBy(f => f.Order);
                    var flightLog = new FlightLog
                    {
                        FlightDate = tempData.FlightDate,
                        AircraftId = tempData.AircraftId,
                        Aircraft = aircraft,
                        UserId = tempData.UserId,
                        User = user,
                        DepartureLocation = tempData.DepartureLocation,
                        ArrivalLocation = tempData.ArrivalLocation
                    };
                    return View(flightLog);
                }

                var newFlightLog = new FlightLog
                {
                    FlightDate = tempData.FlightDate,
                    AircraftId = tempData.AircraftId,
                    Aircraft = aircraft,
                    UserId = tempData.UserId,
                    User = user,
                    DepartureLocation = tempData.DepartureLocation,
                    ArrivalLocation = tempData.ArrivalLocation,
                    Status = FlightStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    Values = values ?? new List<FlightLogValue>(),
                    Remarks = null,
                    DepartureTime = DateTime.Now,
                    UpdatedAt = DateTime.UtcNow,
                    LastModifiedAt = null,
                    ArrivalTime = null,
                    NumberOfLandings = null,
                    TotalTime = null,
                    PassengerCount = null
                };

                _context.Add(newFlightLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.FlightLogFields = aircraft.AircraftType.FlightLogFields.OrderBy(f => f.Order);
            var model = new FlightLog
            {
                FlightDate = tempData.FlightDate,
                AircraftId = tempData.AircraftId,
                Aircraft = aircraft,
                UserId = tempData.UserId,
                User = user,
                DepartureLocation = tempData.DepartureLocation,
                ArrivalLocation = tempData.ArrivalLocation
            };
            return View(model);
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

            var currentUser = await GetCurrentUserAsync();
            var isPilot = await _userManager.IsInRoleAsync(currentUser, "Pilot");
            var isAdminOrStaff = await _userManager.IsInRoleAsync(currentUser, "Admin") || 
                                await _userManager.IsInRoleAsync(currentUser, "Staff");

            // Pilots can only edit their own logs unless they are also admin/staff
            if (isPilot && !isAdminOrStaff && flightLog.UserId != currentUser.Id)
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

            var currentUser = await GetCurrentUserAsync();
            var isPilot = await _userManager.IsInRoleAsync(currentUser, "Pilot");
            var isAdminOrStaff = await _userManager.IsInRoleAsync(currentUser, "Admin") || 
                                await _userManager.IsInRoleAsync(currentUser, "Staff");

            // Pilots can only delete their own logs unless they are also admin/staff
            if (isPilot && !isAdminOrStaff && flightLog.UserId != currentUser.Id)
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

            var currentUser = await GetCurrentUserAsync();
            var isPilot = await _userManager.IsInRoleAsync(currentUser, "Pilot");
            var isAdminOrStaff = await _userManager.IsInRoleAsync(currentUser, "Admin") || 
                                await _userManager.IsInRoleAsync(currentUser, "Staff");

            // Pilots can only submit their own logs unless they are also admin/staff
            if (isPilot && !isAdminOrStaff && flightLog.UserId != currentUser.Id)
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

        private async Task PopulatePilotDropDown(string? selectedPilotId = null)
        {
            var pilots = await _userManager.GetUsersInRoleAsync("Pilot");
            ViewBag.PilotInCommandId = new SelectList(pilots, "Id", "UserName", selectedPilotId);
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