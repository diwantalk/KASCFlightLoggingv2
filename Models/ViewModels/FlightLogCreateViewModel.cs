using System;
using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models.ViewModels
{
    public class FlightLogCreateViewModel
    {
        [Required]
        public DateTime FlightDate { get; set; }

        [Required]
        public int AircraftTypeId { get; set; }

        [Required]
        public int AircraftId { get; set; }

        public string? PilotInCommandId { get; set; }

        public string? Description { get; set; }
    }
}