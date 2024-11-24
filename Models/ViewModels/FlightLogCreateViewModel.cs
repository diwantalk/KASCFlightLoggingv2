using System;
using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models.ViewModels
{
    public class FlightLogCreateViewModel
    {
        [Required]
        [Display(Name = "Flight Date")]
        public DateTime FlightDate { get; set; }

        [Required]
        [Display(Name = "Aircraft Registration")]
        public int AircraftId { get; set; }

        
        [Display(Name = "Pilot in Command")]
        public string? PilotInCommandId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "From")]
        public required string DepartureLocation { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "To")]
        public required string ArrivalLocation { get; set; }

        [StringLength(500)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }
    }
}