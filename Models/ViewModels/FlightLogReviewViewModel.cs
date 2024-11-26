using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models.ViewModels
{
    public class FlightLogReviewViewModel
    {
        public int FlightLogId { get; set; }

        [Required]
        [Display(Name = "Review Status")]
        public FlightStatus Status { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Comments")]
        public string Comments { get; set; } = string.Empty;

        // Read-only information for display
        public DateTime FlightDate { get; set; }
        public string AircraftRegistration { get; set; } = string.Empty;
        public string PilotName { get; set; } = string.Empty;
        public TimeSpan? TotalFlightTime { get; set; }
        public List<FlightLogValueViewModel> Values { get; set; } = new();
        public List<FlightReviewViewModel> PreviousReviews { get; set; } = new();
    }
}