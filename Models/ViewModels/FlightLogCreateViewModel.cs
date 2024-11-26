using System;
using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models.ViewModels
{
    public class FlightLogCreateViewModel
    {
        [Required]
        [Display(Name = "Flight Date")]
        [DataType(DataType.Date)]
        public DateTime FlightDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Aircraft")]
        public int AircraftId { get; set; }

        [Display(Name = "Pilot")]
        public string? PilotInCommandId { get; set; }

        [Required]
        [Display(Name = "Created By")]
        public string UserId { get; set; } = string.Empty;
    }
}