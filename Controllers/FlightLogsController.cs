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
                .Select(f => new FlightLog
                {
                    Id = f.Id,
                    FlightDate = f.FlightDate,
                    AircraftId = f.AircraftId,
                    Aircraft = new Aircraft 
                    { 
                        Id = f.Aircraft.Id,
                        RegistrationNumber = f.Aircraft.RegistrationNumber ?? "N/A"
                    },
                    UserId = f.UserId,
                    User = new ApplicationUser 
                    { 
                        Id = f.User.Id,
                        UserName = f.User.UserName ?? "Unknown",
                        FirstName = f.User.FirstName,
                        LastName = f.User.LastName,
                        Email = f.User.Email
                    },
                    DepartureLocation = f.DepartureLocation,
                    ArrivalLocation = f.ArrivalLocation,
                    Status = f.Status,
                    DepartureTime = f.DepartureTime,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt,
                    Reviews = f.Reviews.ToList()
                })
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

            var flightLog = await (
                from f in _context.FlightLogs
                join a in _context.Aircraft on f.AircraftId equals a.Id
                join u in _context.Users on f.UserId equals u.Id
                join at in _context.AircraftTypes on a.AircraftTypeId equals at.Id
                where f.Id == id
                select new FlightLog
                {
                    Id = f.Id,
                    FlightDate = f.FlightDate,
                    AircraftId = f.AircraftId,
                    Aircraft = new Aircraft 
                    { 
                        Id = a.Id,
                        RegistrationNumber = a.RegistrationNumber ?? "N/A",
                        Model = a.Model ?? "",
                        Description = a.Description ?? "",
                        IsActive = a.IsActive,
                        CreatedAt = a.CreatedAt,
                        LastMaintenanceDate = a.LastMaintenanceDate,
                        AircraftTypeId = a.AircraftTypeId,
                        AircraftType = a.AircraftType
                    },
                    UserId = f.UserId,
                    User = new ApplicationUser 
                    { 
                        Id = u.Id,
                        UserName = u.UserName ?? "Unknown",
                        FirstName = u.FirstName ?? "Unknown",
                        LastName = u.LastName ?? "Unknown",
                        Email = u.Email ?? ""
                    },
                    DepartureLocation = f.DepartureLocation ?? "",
                    ArrivalLocation = f.ArrivalLocation ?? "",
                    Status = f.Status,
                    DepartureTime = f.DepartureTime,
                    ArrivalTime = f.ArrivalTime,
                    NumberOfLandings = f.NumberOfLandings,
                    TotalTime = f.TotalTime,
                    PassengerCount = f.PassengerCount,
                    Remarks = f.Remarks ?? "",
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt,
                    LastModifiedAt = f.LastModifiedAt,
                    Reviews = f.Reviews.Select(r => new FlightReview
                    {
                        Id = r.Id,
                        FlightLogId = r.FlightLogId,
                        ReviewerId = r.ReviewerId,
                        Status = r.Status,
                        Comments = r.Comments ?? "",
                        ReviewedAt = r.ReviewedAt,
                        Reviewer = r.Reviewer == null ? null : new ApplicationUser
                        {
                            Id = r.Reviewer.Id,
                            UserName = r.Reviewer.UserName ?? "Unknown",
                            FirstName = r.Reviewer.FirstName ?? "Unknown",
                            LastName = r.Reviewer.LastName ?? "Unknown",
                            Email = r.Reviewer.Email ?? ""
                        }
                    }).ToList(),
                    Values = f.Values.ToList()
                }).FirstOrDefaultAsync();

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
                    DepartureLocation = tempData.DepartureLocation ?? "",
                    ArrivalLocation = tempData.ArrivalLocation ?? "",
                    Status = FlightStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    Values = values ?? new List<FlightLogValue>(),
                    Remarks = "",  // Initialize with empty string instead of null
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

            // Get the flight log with all related data
            var flightLog = await _context.FlightLogs
                .Include(f => f.Aircraft)
                    .ThenInclude(a => a.AircraftType)
                .Include(f => f.User)
                .Include(f => f.Values)
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

            // Create the view model
            var viewModel = new FlightLogEditViewModel
            {
                Id = flightLog.Id,
                FlightDate = flightLog.FlightDate,
                AircraftId = flightLog.AircraftId,
                Aircraft = flightLog.Aircraft,
                DepartureLocation = flightLog.DepartureLocation ?? "",
                ArrivalLocation = flightLog.ArrivalLocation ?? "",
                Status = flightLog.Status,
                DepartureTime = flightLog.DepartureTime,
                ArrivalTime = flightLog.ArrivalTime,
                NumberOfLandings = flightLog.NumberOfLandings,
                TotalTime = flightLog.TotalTime,
                PassengerCount = flightLog.PassengerCount,
                Remarks = flightLog.Remarks ?? "",
                CustomFields = []
            };

            // Get all fields for this aircraft type
            var fields = await _context.FlightLogFields
                .Where(f => f.AircraftTypeId == flightLog.Aircraft.AircraftTypeId)
                .OrderBy(f => f.Order)
                .ToListAsync();

            // Create custom field view models
            foreach (var field in fields)
            {
                var value = flightLog.Values.FirstOrDefault(v => v.FlightLogFieldId == field.Id);
                viewModel.CustomFields.Add(new FlightLogValueViewModel
                {
                    FlightLogFieldId = field.Id,
                    DisplayText = field.DisplayText,
                    Value = value?.Value ?? "",
                    Required = field.Required,
                    FieldType = field.FieldType
                });
            }

            await PopulateAircraftDropDown(flightLog.AircraftId);
            return View(viewModel);
        }

        // POST: FlightLogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FlightLogEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var existingLog = await _context.FlightLogs
                .Include(f => f.Values)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (existingLog == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();
            var isPilot = await _userManager.IsInRoleAsync(currentUser, "Pilot");
            var isAdminOrStaff = await _userManager.IsInRoleAsync(currentUser, "Admin") || 
                                await _userManager.IsInRoleAsync(currentUser, "Staff");

            // Pilots can only edit their own logs unless they are also admin/staff
            if (isPilot && !isAdminOrStaff && existingLog.UserId != currentUser.Id)
            {
                return Forbid();
            }

            if (existingLog.Status != FlightStatus.Draft)
            {
                TempData["Error"] = "Only draft logs can be edited.";
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update primary fields
                    existingLog.FlightDate = model.FlightDate;
                    existingLog.AircraftId = model.AircraftId;
                    existingLog.DepartureLocation = model.DepartureLocation ?? "";
                    existingLog.ArrivalLocation = model.ArrivalLocation ?? "";
                    existingLog.DepartureTime = model.DepartureTime;
                    existingLog.ArrivalTime = model.ArrivalTime;
                    existingLog.NumberOfLandings = model.NumberOfLandings;
                    existingLog.TotalTime = model.TotalTime;
                    existingLog.PassengerCount = model.PassengerCount;
                    existingLog.Remarks = model.Remarks ?? "";
                    existingLog.UpdatedAt = DateTime.UtcNow;

                    // Update custom fields
                    foreach (var customField in model.CustomFields)
                    {
                        var existingValue = existingLog.Values
                            .FirstOrDefault(v => v.FlightLogFieldId == customField.FlightLogFieldId);

                        if (existingValue != null)
                        {
                            // Update existing value
                            existingValue.Value = customField.Value ?? "";
                        }
                        else
                        {
                            // Add new value
                            existingLog.Values.Add(new FlightLogValue
                            {
                                FlightLogId = existingLog.Id,
                                FlightLogFieldId = customField.FlightLogFieldId,
                                Value = customField.Value ?? ""
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightLogExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            await PopulateAircraftDropDown(model.AircraftId);

            // Reload the aircraft type fields
            var aircraft = await _context.Aircraft
                .Include(a => a.AircraftType)
                    .ThenInclude(at => at.FlightLogFields)
                .FirstOrDefaultAsync(a => a.Id == model.AircraftId);

            if (aircraft != null)
            {
                var fields = await _context.FlightLogFields
                    .Where(f => f.AircraftTypeId == aircraft.AircraftTypeId)
                    .OrderBy(f => f.Order)
                    .ToListAsync();

                model.CustomFields = fields.Select(field => new FlightLogValueViewModel
                {
                    FlightLogFieldId = field.Id,
                    DisplayText = field.DisplayText,
                    Value = model.CustomFields.FirstOrDefault(cf => cf.FlightLogFieldId == field.Id)?.Value ?? "",
                    Required = field.Required,
                    FieldType = field.FieldType
                }).ToList();
            }

            return View(model);
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
                .Select(f => new FlightLog
                {
                    Id = f.Id,
                    FlightDate = f.FlightDate,
                    AircraftId = f.AircraftId,
                    Aircraft = f.Aircraft == null ? null : new Aircraft 
                    { 
                        Id = f.Aircraft.Id,
                        RegistrationNumber = f.Aircraft.RegistrationNumber ?? "N/A",
                        Model = f.Aircraft.Model
                    },
                    UserId = f.UserId,
                    User = f.User == null ? null : new ApplicationUser 
                    { 
                        Id = f.User.Id,
                        UserName = f.User.UserName ?? "Unknown",
                        FirstName = f.User.FirstName ?? "Unknown",
                        LastName = f.User.LastName ?? "Unknown",
                        Email = f.User.Email
                    },
                    DepartureLocation = f.DepartureLocation,
                    ArrivalLocation = f.ArrivalLocation,
                    Status = f.Status,
                    DepartureTime = f.DepartureTime,
                    Remarks = f.Remarks
                })
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