using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KASCFlightLogging.Models
{
    public class FlightReview
    {
        public FlightReview()
        {
            ReviewerId = string.Empty;
            Comments = string.Empty;
        }

        public int Id { get; set; }

        [Required]
        public int FlightLogId { get; set; }

        [Required]
        public string ReviewerId { get; set; }

        [Required]
        public FlightStatus Status { get; set; }

        [Required]
        [StringLength(500)]
        public string Comments { get; set; }

        public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public required FlightLog FlightLog { get; set; }
        public required ApplicationUser Reviewer { get; set; }
    }
}