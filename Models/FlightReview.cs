namespace KASCFlightLogging.Models;

public class FlightReview
{
    public int Id { get; set; }
    public int FlightLogId { get; set; }
    public FlightLog FlightLog { get; set; }
    public string ReviewerId { get; set; }
    public ApplicationUser Reviewer { get; set; }
    public ReviewStatus Status { get; set; }
    public string Comments { get; set; }
    public DateTime CreatedAt { get; set; }
}