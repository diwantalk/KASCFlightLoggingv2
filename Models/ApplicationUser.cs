using KASCFlightLogging.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";

    public virtual ICollection<FlightLog> FlightLogs { get; set; } = new List<FlightLog>();
    public virtual ICollection<FlightReview> Reviews { get; set; } = new List<FlightReview>();
}