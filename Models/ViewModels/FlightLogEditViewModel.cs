using System;
using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models.ViewModels
{
    public class FlightLogEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Flight Date")]
        [DataType(DataType.Date)]
        public DateTime FlightDate { get; set; }

        [Required]
        [Display(Name = "Aircraft")]
        public int AircraftId { get; set; }

        [Required]
        [Display(Name = "Pilot")]
        public string PilotId { get; set; } = string.Empty;

        [Display(Name = "Total Flight Time")]
        public TimeSpan TotalFlightTime { get; set; }

        public FlightStatus Status { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsPublished { get; set; }

        // Navigation properties for display
        public Aircraft? Aircraft { get; set; }
        public ApplicationUser? Pilot { get; set; }

        // Field values
        public List<FlightLogValueViewModel> Values { get; set; } = new();
    }
}