using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models;

public class FlightLogValue
{
    public int Id { get; set; }

    [Required]
    public string Value { get; set; }

    public int FlightLogId { get; set; }
    public FlightLog FlightLog { get; set; }

    public int FlightLogFieldId { get; set; }
    public FlightLogField Field { get; set; }
}