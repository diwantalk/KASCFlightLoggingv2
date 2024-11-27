using KASCFlightLogging.Models;
using System.ComponentModel.DataAnnotations;

public class InitialFlightLogViewModel
{
    [Required]
    public int AircraftTypeId { get; set; }

    [Required]
    [Display(Name = "Flight Date")]
    public DateTime FlightDate { get; set; }

    public List<FlightLogField> RequiredFields { get; set; } = new();
}