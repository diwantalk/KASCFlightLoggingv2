namespace KASCFlightLogging.Models
{
    public enum FlightStatus
    {
        Draft,
        PendingInitialReview,
        Rejected,
        Approved,
        PendingFinalReview,
        Completed,
        Cancelled
    }



    public enum FieldType
    {
        Text,
        Number,
        Time,
        Date,
        DateTime
    }
}