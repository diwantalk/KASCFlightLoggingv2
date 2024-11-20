using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models
{
    public class AircraftType
    {
        public AircraftType()
        {
            Aircraft = [];
            FlightLogFields = [];
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        [StringLength(200)]
        public required string Description { get; set; }

        public virtual ICollection<Aircraft> Aircraft { get; set; }
        public virtual ICollection<FlightLogField> FlightLogFields { get; set; }
    }
}