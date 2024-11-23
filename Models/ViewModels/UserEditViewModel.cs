using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models.ViewModels
{
    public class UserEditViewModel
    {
        public UserEditViewModel()
        {
            CurrentRoles = new List<string>();
            AvailableRoles = new List<RoleSelection>();
        }

        public required string Id { get; set; }
        
        [Display(Name = "Username")]
        public required string UserName { get; set; }
        
        [EmailAddress]
        public required string Email { get; set; }
        
        [Display(Name = "Current Roles")]
        public List<string> CurrentRoles { get; set; }
        
        [Display(Name = "Available Roles")]
        public List<RoleSelection> AvailableRoles { get; set; }
    }

    public class RoleSelection
    {
        public required string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}