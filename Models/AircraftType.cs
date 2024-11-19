using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models
{
    public class AircraftType
    {
        public AircraftType()
        {
            Aircraft = new HashSet<Aircraft>();
            FlightLogFields = new HashSet<FlightLogField>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        public virtual ICollection<Aircraft> Aircraft { get; set; }
        public virtual ICollection<FlightLogField> FlightLogFields { get; set; }
    }
}