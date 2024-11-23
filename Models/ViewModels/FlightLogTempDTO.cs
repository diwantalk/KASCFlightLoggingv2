using System;

namespace KASCFlightLogging.Models.ViewModels
{
    public class FlightLogTempDTO
    {
        public DateTime FlightDate { get; set; }
        public int AircraftId { get; set; }
        public string UserId { get; set; }
        public string DepartureLocation { get; set; }
        public string ArrivalLocation { get; set; }
        public string? Remarks { get; set; }
    }
}