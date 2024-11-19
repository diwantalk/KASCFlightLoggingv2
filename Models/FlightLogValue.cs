using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KASCFlightLogging.Models
{
    public class FlightLogValue
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Value { get; set; } = string.Empty;

        [Required]
        public int FlightLogId { get; set; }

        [Required]
        [ForeignKey("FlightLogId")]
        public virtual FlightLog FlightLog { get; set; } = null!;

        [Required]
        public int FlightLogFieldId { get; set; }

        [Required]
        [ForeignKey("FlightLogFieldId")]
        public virtual FlightLogField Field { get; set; } = null!;
    }
}