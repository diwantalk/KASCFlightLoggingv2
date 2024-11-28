using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models.ViewModels
{
    public class InitialFlightLogViewModel : FlightLogCreateViewModel
    {
        public List<FlightLogField> RequiredFields { get; set; } = new();
    }

    public class FlightLogFieldInputViewModel
    {
        public int FlightLogFieldId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}