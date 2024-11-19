using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models;

public class FlightLogField
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Display Text")]
    public string DisplayText { get; set; }

    [StringLength(200)]
    public string Description { get; set; }

    public bool IsRequired { get; set; }

    public int AircraftTypeId { get; set; }
    public AircraftType AircraftType { get; set; }

    public ICollection<FlightLogValue> Values { get; set; }
}