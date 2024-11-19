using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KASCFlightLogging.Models
{
    public class FlightLogValue
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        [Required]
        public int FlightLogId { get; set; }

        [ForeignKey("FlightLogId")]
        public virtual FlightLog FlightLog { get; set; }

        [Required]
        public int FlightLogFieldId { get; set; }

        [ForeignKey("FlightLogFieldId")]
        public virtual FlightLogField Field { get; set; }
    }

    
}