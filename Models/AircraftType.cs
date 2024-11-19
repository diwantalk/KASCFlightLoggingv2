using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models;

public class AircraftType
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    
    [StringLength(200)]
    public string Description { get; set; }
    
    public ICollection<Aircraft> Aircraft { get; set; }
    public ICollection<FlightLogField> RequiredFields { get; set; }
}