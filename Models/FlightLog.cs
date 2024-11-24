using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KASCFlightLogging.Models
{

    public class FlightLog
    {
        public FlightLog()
        {
            Reviews = [];
            Values = [];
        }

        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [Required]
        public int AircraftId { get; set; }
        public virtual Aircraft? Aircraft { get; set; }

        [Required]
        [Display(Name = "Flight Date")]
        public DateTime FlightDate { get; set; }

        [Required]
        [Display(Name = "Departure Time")]
        public DateTime DepartureTime { get; set; }

        [Display(Name = "Arrival Time")]
        public DateTime? ArrivalTime { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "From")]
        public required string DepartureLocation { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "To")]
        public required string ArrivalLocation { get; set; }

        [Required]
        public FlightStatus Status { get; set; } = FlightStatus.Draft;

        [Display(Name = "Number of Landings")]
        public int? NumberOfLandings { get; set; }

        [Display(Name = "Total Time")]
        public TimeSpan? TotalTime { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; } = null;

        [Display(Name = "Passenger Count")]
        public int? PassengerCount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Modified")]
        public DateTime? LastModifiedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<FlightReview> Reviews { get; init; }
        public virtual ICollection<FlightLogValue> Values { get; init; }
    }

  
}