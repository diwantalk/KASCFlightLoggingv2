using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KASCFlightLogging.Models
{
    public class FlightLogField
    {
        public FlightLogField()
        {
            Name = string.Empty;
            DisplayText = string.Empty;
            Description = string.Empty;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string DisplayText { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        public bool Required { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        [ForeignKey(nameof(AircraftType))]
        public int AircraftTypeId { get; set; }

        public required AircraftType AircraftType { get; set; }
    }
}
