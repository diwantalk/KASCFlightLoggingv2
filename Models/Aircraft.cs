using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KASCFlightLogging.Models
{
    public class Aircraft
    {
        public Aircraft()
        {
            FlightLogs = [];
        }

        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Registration Number")]
        public required string RegistrationNumber { get; set; }

        [StringLength(50)]
        public string? Model { get; set; }

        [Required]
        [ForeignKey(nameof(AircraftType))]
        public int AircraftTypeId { get; set; }

        public AircraftType? AircraftType { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Maintenance Date")]
        public DateTime? LastMaintenanceDate { get; set; }

        public virtual ICollection<FlightLog> FlightLogs { get; init; }
    }
}