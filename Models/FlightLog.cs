using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models;

public class FlightLog
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    [Required]
    public int AircraftId { get; set; }
    public Aircraft Aircraft { get; set; }

    [Required]
    [Display(Name = "Flight Date")]
    public DateTime FlightDate { get; set; }

    [Required]
    [Display(Name = "Departure Time")]
    public DateTime DepartureTime { get; set; }

    [Display(Name = "Arrival Time")]
    public DateTime? ArrivalTime { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "From")]
    public string DepartureLocation { get; set; }

    [StringLength(50)]
    [Display(Name = "To")]
    public string ArrivalLocation { get; set; }

    [Required]
    public FlightStatus Status { get; set; } = FlightStatus.Draft;

    [Display(Name = "Number of Landings")]
    public int? NumberOfLandings { get; set; }

    [Display(Name = "Total Time")]
    public TimeSpan? TotalTime { get; set; }

    [StringLength(500)]
    public string Remarks { get; set; }

    [Display(Name = "Passenger Count")]
    public int? PassengerCount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Last Modified")]
    public DateTime? LastModifiedAt { get; set; }

    public ICollection<FlightReview> Reviews { get; set; }
    public ICollection<FlightLogValue> Values { get; set; }
}