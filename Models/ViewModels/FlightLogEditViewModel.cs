using System;
using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models.ViewModels
{
    public class FlightLogEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Flight Date")]
        public DateTime FlightDate { get; set; }

        [Required]
        [Display(Name = "Aircraft")]
        public int AircraftId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "From")]
        public required string DepartureLocation { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "To")]
        public required string ArrivalLocation { get; set; }

        [Required]
        [Display(Name = "Departure Time")]
        public DateTime DepartureTime { get; set; }

        [Display(Name = "Arrival Time")]
        public DateTime? ArrivalTime { get; set; }

        [Display(Name = "Number of Landings")]
        public int? NumberOfLandings { get; set; }

        [Display(Name = "Total Time")]
        public TimeSpan? TotalTime { get; set; }

        [Display(Name = "Passenger Count")]
        public int? PassengerCount { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        public FlightStatus Status { get; set; }

        // Navigation properties for display
        public Aircraft? Aircraft { get; set; }
        public ApplicationUser? User { get; set; }

        // Custom fields
        public List<FlightLogValueViewModel> CustomFields { get; set; } = new();
    }

    public class FlightLogValueViewModel
    {
        public int FlightLogFieldId { get; set; }
        public string DisplayText { get; set; } = "";
        public string Value { get; set; } = "";
        public bool Required { get; set; }
        public string FieldType { get; set; } = "text";
    }
}