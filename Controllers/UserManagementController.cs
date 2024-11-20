using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using KASCFlightLogging.Models.ViewModels; // Adjust the namespace according to your project

namespace KASCFlightLoggingv2.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagementController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: /UserManagement/
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var userViewModels = new List<UserViewModel>();
            foreach (var user in users)
            {
                var roleNames = await _userManager.GetRolesAsync(user);
                var isLockedOut = await _userManager.IsLockedOutAsync(user);

                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = string.Join(", ", roleNames),
                    IsLockedOut = isLockedOut
                });
            }

            return View(userViewModels);
        }

        // POST: /UserManagement/Activate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

        // POST: /UserManagement/Deactivate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            user.LockoutEnd = DateTimeOffset.MaxValue; // Lock the user indefinitely
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }
}