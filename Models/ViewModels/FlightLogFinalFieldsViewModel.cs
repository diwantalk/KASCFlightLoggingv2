using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models.ViewModels
{
    public class FlightLogFinalFieldsViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Departure Time")]
        [DataType(DataType.DateTime)]
        public DateTime DepartureTime { get; set; }

        [Required]
        [Display(Name = "Arrival Time")]
        [DataType(DataType.DateTime)]
        public DateTime ArrivalTime { get; set; }

        public List<FlightLogFieldValueViewModel> AdditionalFields { get; set; } = new();

        // Read-only information for display
        public DateTime FlightDate { get; set; }
        public string AircraftRegistration { get; set; } = string.Empty;
        public string PilotName { get; set; } = string.Empty;
    }


}