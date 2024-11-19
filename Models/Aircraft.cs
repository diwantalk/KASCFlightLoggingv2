using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models;

public class Aircraft
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Registration Number")]
    public string RegistrationNumber { get; set; }

    [Required]
    [StringLength(50)]
    public string Model { get; set; }

    [Required]
    public int AircraftTypeId { get; set; }
    public AircraftType Type { get; set; }

    [StringLength(200)]
    public string Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Last Maintenance Date")]
    public DateTime? LastMaintenanceDate { get; set; }

    public ICollection<FlightLog> FlightLogs { get; set; }
}