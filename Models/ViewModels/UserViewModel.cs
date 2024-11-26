using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            Id = string.Empty;
            Email = string.Empty;
            UserName = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Roles = new List<string>();
            IsLockedOut = false;
        }

        [Required]
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public List<string> Roles { get; set; }

        [Display(Name = "Locked Out")]
        public bool IsLockedOut { get; set; }
    }
}