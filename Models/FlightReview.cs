using KASCFlightLogging.Models;

//public class FlightReview
//{
//    public int Id { get; set; }
//    public int FlightLogId { get; set; }
//    public string ReviewerId { get; set; }
//    public string Comments { get; set; }
//    public DateTime ReviewedAt { get; set; } // Add this property
//    public FlightStatus Status { get; set; }

//    public virtual FlightLog FlightLog { get; set; }
//    public virtual ApplicationUser Reviewer { get; set; }
//}

public class FlightReview
{
    public int Id { get; set; }
    public int FlightLogId { get; set; }
    public string ReviewerId { get; set; }
    public FlightStatus Status { get; set; }
    public string Comments { get; set; }
    public DateTime ReviewedAt { get; set; }

    // Navigation properties
    public virtual FlightLog FlightLog { get; set; }
    public virtual ApplicationUser Reviewer { get; set; }
}