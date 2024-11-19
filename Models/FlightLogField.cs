using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KASCFlightLogging.Models
{
    public class FlightLogField
    {
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
        public bool IsRequired { get; set; }

        [Required]
        [ForeignKey(nameof(AircraftType))]  // Added this attribute
        public int AircraftTypeId { get; set; }

        public virtual AircraftType AircraftType { get; set; }
    }
}
