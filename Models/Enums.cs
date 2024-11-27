namespace KASCFlightLogging.Models
{
    public enum FlightStatus
    {
        Draft,
        PendingReview,
        Rejected,
        Approved,
        FinalReview,
        Completed
    }



    public enum FieldType
    {
        Text,
        Number,
        Time,
        DateTime
    }
}