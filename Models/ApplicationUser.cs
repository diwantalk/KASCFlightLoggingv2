using Microsoft.AspNetCore.Identity;

namespace KASCFlightLogging.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public ICollection<FlightLog> FlightLogs { get; set; }
    public ICollection<FlightReview> ReviewedFlights { get; set; }
}