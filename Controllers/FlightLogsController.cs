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

        // GET: FlightLogs
        public async Task<IActionResult> Index()
        {
            var flightLogs = await _context.FlightLogs
                .Include(f => f.Aircraft)
                .Include(f => f.Pilot)
                .OrderByDescending(f => f.FlightDate)
                .ToListAsync();

            return View(flightLogs);
        }

        // GET: FlightLogs/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var flightLog = await _context.FlightLogs
                .Include(f => f.Aircraft)
                    .ThenInclude(a => a.AircraftType)
                .Include(f => f.Pilot)
                .Include(f => f.User)
                .Include(f => f.ModifiedBy)
                .Include(f => f.PublishedBy)
                .Include(f => f.Values)
                    .ThenInclude(v => v.FlightLogField)
                .Include(f => f.Reviews)
                    .ThenInclude(r => r.Reviewer)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flightLog == null)
                return NotFound();

            return View(flightLog);
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
        public async Task<IActionResult> Create(FlightLogCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var requiredFields = await _context.FlightLogFields
                    .Where(f => f.AircraftTypeId == model.AircraftTypeId && f.Required)
                    .OrderBy(f => f.Order)
                    .ToListAsync();

                if (!requiredFields.Any())
                {
                    ModelState.AddModelError("", "No required fields found for this aircraft type.");
                    return View(model);
                }

                var initialModel = new InitialFlightLogViewModel
                {
                    FlightDate = model.FlightDate,
                    AircraftTypeId = model.AircraftTypeId,
                    AircraftId = model.AircraftId,
                    PilotInCommandId = model.PilotInCommandId,
                    Description = model.Description,
                    RequiredFields = requiredFields
                };

                return View("CreateRequiredFields", initialModel);
            }

            // If we got this far, something failed, redisplay form
            ViewBag.AircraftTypes = await _context.AircraftTypes
                .OrderBy(at => at.Name)
                .ToListAsync();

            if (User.IsInRole("Admin"))
            {
                var pilots = await _userManager.GetUsersInRoleAsync("Pilot");
                ViewBag.PilotInCommandId = new SelectList(
                    pilots.OrderBy(p => p.LastName),
                    "Id",
                    "FullName"
                );
            }

            return View(model);
        }

        // POST: FlightLogs/CreateRequiredFields
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequiredFields(InitialFlightLogViewModel model, Dictionary<string, string> formValues)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Challenge();
                }

                var aircraft = await _context.Aircraft
                    .Include(a => a.AircraftType)
                    .FirstOrDefaultAsync(a => a.Id == model.AircraftId);

                if (aircraft == null)
                {
                    ModelState.AddModelError("", "Selected aircraft not found.");
                    return View(model);
                }

                // Get required fields for this aircraft type
                var requiredFields = await _context.FlightLogFields
                    .Where(f => f.AircraftTypeId == aircraft.AircraftTypeId && f.Required)
                    .ToListAsync();

                // Create flight log values from form data
                var flightLogValues = new List<FlightLogValue>();
                foreach (var field in requiredFields)
                {
                    if (formValues.TryGetValue($"field_{field.Id}", out string value))
                    {
                        flightLogValues.Add(new FlightLogValue
                        {
                            FlightLogFieldId = field.Id,
                            Value = value
                        });
                    }
                }

                var flightLog = new FlightLog
                {
                    FlightDate = model.FlightDate,
                    PilotId = User.IsInRole("Admin") && !string.IsNullOrEmpty(model.PilotInCommandId) 
                        ? model.PilotInCommandId 
                        : currentUser.Id,
                    UserId = currentUser.Id,
                    AircraftId = model.AircraftId,
                    Status = FlightStatus.PendingInitialReview,
                    IsActive = false,
                    IsPublished = false,
                    CreatedAt = DateTime.UtcNow,
                    Values = flightLogValues
                };

                _context.FlightLogs.Add(flightLog);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Flight log created successfully and pending for initial review.";
                return RedirectToAction("Index");
            }

            // If we got this far, something failed
            model.RequiredFields = await _context.FlightLogFields
                .Where(f => f.AircraftTypeId == model.AircraftTypeId && f.Required)
                .OrderBy(f => f.Order)
                .ToListAsync();

            return View(model);
        }

        // GET: FlightLogs/Review/5
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Review(int id)
        {
            var flightLog = await _context.FlightLogs
                .Include(f => f.Pilot)
                .Include(f => f.Aircraft)
                .Include(f => f.Values)
                    .ThenInclude(v => v.FlightLogField)
                .Include(f => f.Reviews)
                    .ThenInclude(r => r.Reviewer)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flightLog == null)
                return NotFound();

            var viewModel = new FlightLogReviewViewModel
            {
                FlightLogId = flightLog.Id,
                FlightDate = flightLog.FlightDate,
                AircraftRegistration = flightLog.Aircraft?.RegistrationNumber ?? "Unknown",
                PilotName = $"{flightLog.Pilot?.FirstName} {flightLog.Pilot?.LastName}".Trim(),
                TotalFlightTime = flightLog.TotalFlightTime,
                Values = flightLog.Values
                    .Select(v => new FlightLogValueViewModel
                    {
                        FieldName = v.FlightLogField?.Name ?? "",
                        DisplayText = v.FlightLogField?.DisplayText ?? "",
                        Value = v.Value,
                        IsRequired = v.FlightLogField?.Required ?? false
                    })
                    .OrderBy(v => v.DisplayText)
                    .ToList(),
                PreviousReviews = flightLog.Reviews
                    .OrderByDescending(r => r.ReviewedAt)
                    .Select(r => new FlightReviewViewModel
                    {
                        ReviewerName = $"{r.Reviewer?.FirstName} {r.Reviewer?.LastName}".Trim(),
                        Status = r.Status,
                        Comments = r.Comments,
                        ReviewedAt = r.ReviewedAt
                    })
                    .ToList()
            };

            return View(viewModel);
        }

        // POST: FlightLogs/Review/5
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(int id, FlightLogReviewViewModel model)
        {
            var flightLog = await _context.FlightLogs.FindAsync(id);
            if (flightLog == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Update flight log status
                if (flightLog.Status == FlightStatus.PendingInitialReview)
                {
                    // Initial review
                    flightLog.Status = model.Status;
                    flightLog.IsActive = model.Status == FlightStatus.Approved;
                    flightLog.IsPublished = model.Status == FlightStatus.Approved;
                }
                else if (flightLog.Status == FlightStatus.PendingFinalReview)
                {
                    // Final review
                    flightLog.Status = model.Status == FlightStatus.Approved ? FlightStatus.Completed : FlightStatus.Approved;
                    flightLog.IsActive = true;
                    flightLog.IsPublished = true;
                }
                flightLog.LastModifiedAt = DateTime.UtcNow;
                flightLog.ModifiedById = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Add review
                var review = new FlightReview
                {
                    FlightLogId = id,
                    ReviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Comments = model.Comments,
                    ReviewedAt = DateTime.UtcNow,
                    Status = model.Status
                };
                _context.FlightReviews.Add(review);

                await _context.SaveChangesAsync();
                TempData["Message"] = $"Flight log review completed. Status: {model.Status}";
                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: FlightLogs/FinalFields/5
        [Authorize(Roles = "Pilot")]
        public async Task<IActionResult> FinalFields(int id)
        {
            var flightLog = await _context.FlightLogs
                .Include(f => f.Aircraft)
                    .ThenInclude(a => a.AircraftType)
                .Include(f => f.Values)
                    .ThenInclude(v => v.FlightLogField)
                .Include(f => f.Pilot)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flightLog == null)
                return NotFound();

            // Only allow access if the flight log is Approved and not yet Completed
            if (flightLog.Status != FlightStatus.Approved || !flightLog.IsActive || !flightLog.IsPublished)
            {
                TempData["Error"] = "This flight log cannot be edited. It must be approved and active to enter final fields.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Only allow the pilot who created the log or admin to edit
            if (!User.IsInRole("Admin") && flightLog.PilotId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            // Get all fields for this aircraft type
            var flightLogFields = await _context.FlightLogFields
                .Where(f => f.AircraftTypeId == flightLog.Aircraft.AircraftTypeId)
                .OrderBy(f => f.Order)
                .ToListAsync();

            // Make sure we have all required fields
            var requiredFields = flightLogFields.Where(f => f.Required).ToList();
            var missingRequiredFields = requiredFields
                .Where(f => !flightLog.Values.Any(v => v.FlightLogFieldId == f.Id))
                .ToList();

            if (missingRequiredFields.Any())
            {
                TempData["Error"] = "Some required fields are missing. Please contact an administrator.";
                return RedirectToAction(nameof(Details), new { id });
            }

            ViewBag.FlightLogFields = flightLogFields;
            return View(flightLog);
        }

        // POST: FlightLogs/FinalFields/5
        [HttpPost]
        [Authorize(Roles = "Pilot")]
        public async Task<IActionResult> FinalFields(int id, Dictionary<string, FlightLogValue> values)
        {
            var flightLog = await _context.FlightLogs
                .Include(f => f.Values)
                    .ThenInclude(v => v.FlightLogField)
                .Include(f => f.Aircraft)
                    .ThenInclude(a => a.AircraftType)
                        .ThenInclude(at => at.FlightLogFields)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flightLog == null)
                return NotFound();

            // Convert dictionary values to list
            var finalValues = values.Values
                .Where(v => v.FlightLogFieldId != 0 && !string.IsNullOrEmpty(v.Value))
                .ToList();

            // Calculate total flight time from departure and arrival times
            var departureValue = values["departure"].Value;
            var arrivalValue = values["arrival"].Value;

            if (DateTime.TryParse(departureValue, out DateTime departureTime) && 
                DateTime.TryParse(arrivalValue, out DateTime arrivalTime))
            {
                flightLog.TotalFlightTime = arrivalTime - departureTime;
            }
            else
            {
                return await ReturnToFinalFieldsView(flightLog, "Invalid departure or arrival time format.");
            }

            // Get valid field IDs for this aircraft type
            var validFieldIds = flightLog.Aircraft.AircraftType.FlightLogFields.Select(f => f.Id).ToHashSet();

            // Keep existing required values
            var existingRequiredValues = flightLog.Values
                .Where(v => v.FlightLogField.Required)
                .ToList();

            // Create new list for all values
            var allValues = new List<FlightLogValue>();
            allValues.AddRange(existingRequiredValues);

            // Add departure and arrival times
            if (values.ContainsKey("departure") && values.ContainsKey("arrival"))
            {
                var departureFieldId = values["departure"].FlightLogFieldId;
                var arrivalFieldId = values["arrival"].FlightLogFieldId;

                if (validFieldIds.Contains(departureFieldId) && validFieldIds.Contains(arrivalFieldId))
                {
                    allValues.Add(new FlightLogValue
                    {
                        FlightLogId = flightLog.Id,
                        FlightLogFieldId = departureFieldId,
                        Value = values["departure"].Value
                    });

                    allValues.Add(new FlightLogValue
                    {
                        FlightLogId = flightLog.Id,
                        FlightLogFieldId = arrivalFieldId,
                        Value = values["arrival"].Value
                    });
                }
                else
                {
                    return await ReturnToFinalFieldsView(flightLog, "Invalid departure or arrival field ID.");
                }
            }

            // Add all other non-required values
            foreach (var value in values.Where(v => v.Key != "departure" && v.Key != "arrival"))
            {
                if (!string.IsNullOrEmpty(value.Value.Value) && validFieldIds.Contains(value.Value.FlightLogFieldId))
                {
                    // Only add if it's not a required field (those are already included)
                    if (!existingRequiredValues.Any(v => v.FlightLogFieldId == value.Value.FlightLogFieldId))
                    {
                        allValues.Add(new FlightLogValue
                        {
                            FlightLogId = flightLog.Id,
                            FlightLogFieldId = value.Value.FlightLogFieldId,
                            Value = value.Value.Value
                        });
                    }
                }
            }

            // Update the flight log values
            _context.FlightLogValues.RemoveRange(flightLog.Values);
            await _context.SaveChangesAsync();

            // Clear and add new values
            flightLog.Values.Clear();
            foreach (var value in allValues)
            {
                flightLog.Values.Add(value);
            }
            flightLog.Status = FlightStatus.PendingFinalReview;
            flightLog.LastModifiedAt = DateTime.UtcNow;
            flightLog.ModifiedById = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _context.SaveChangesAsync();
            TempData["Message"] = "Final fields saved successfully. Flight log is pending final review.";
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

        private async Task<IActionResult> ReturnToFinalFieldsView(FlightLog flightLog, string errorMessage)
        {
            ModelState.AddModelError("", errorMessage);
            ViewBag.FlightLogFields = await _context.FlightLogFields
                .Where(f => f.AircraftTypeId == flightLog.Aircraft.AircraftTypeId)
                .OrderBy(f => f.Order)
                .ToListAsync();
            return View("FinalFields", flightLog);
        }
    }
}