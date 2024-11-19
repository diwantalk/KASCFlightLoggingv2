using KASCFlightLogging.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    public string FullName => $"{FirstName} {LastName}";
    
    [Required]
    public UserRole Role { get; set; }

    public virtual ICollection<FlightLog> FlightLogs { get; set; } = new List<FlightLog>();
    public virtual ICollection<FlightReview> Reviews { get; set; } = new List<FlightReview>();
}