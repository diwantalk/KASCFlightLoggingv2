using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models
{
    public class FlightLog
    {
        public FlightLog()
        {
            Reviews = new List<FlightReview>();
            Values = new List<FlightLogValue>();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            IsPublished = false;
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Flight Date")]
        public DateTime FlightDate { get; set; }

        [Required]
        [Display(Name = "Pilot")]
        public string PilotId { get; set; } = string.Empty;
        public virtual ApplicationUser? Pilot { get; set; }

        [Required]
        [Display(Name = "Created By")]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }

        [Required]
        [Display(Name = "Aircraft")]
        public int AircraftId { get; set; }
        public virtual Aircraft? Aircraft { get; set; }

        [Display(Name = "Total Flight Time")]
        public TimeSpan? TotalFlightTime { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime? LastModifiedAt { get; set; }

        [Required]
        public bool IsPublished { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Display(Name = "Published By")]
        public string? PublishedById { get; set; }
        public virtual ApplicationUser? PublishedBy { get; set; }

        [Display(Name = "Modified By")]
        public string? ModifiedById { get; set; }
        public virtual ApplicationUser? ModifiedBy { get; set; }

        [Required]
        public FlightStatus Status { get; set; } = FlightStatus.Draft;

        public virtual ICollection<FlightReview> Reviews { get; init; }
        public virtual ICollection<FlightLogValue> Values { get; init; }
    }
}