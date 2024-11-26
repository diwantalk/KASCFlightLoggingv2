using System;

namespace KASCFlightLogging.Models.ViewModels
{
    public class FlightLogTempDTO
    {
        public DateTime FlightDate { get; set; }
        public int AircraftId { get; set; }
        public string PilotId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}