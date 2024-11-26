using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KASCFlightLogging.Models
{
    public class FlightLogValue
    {
        public int Id { get; set; }

        [Required]
        public int FlightLogId { get; set; }
        public virtual FlightLog? FlightLog { get; set; }

        [Required]
        public int FlightLogFieldId { get; set; }
        public virtual FlightLogField? FlightLogField { get; set; }

        [Required]
        public string Value { get; set; } = string.Empty;
    }
}