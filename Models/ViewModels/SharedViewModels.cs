using System;
using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models.ViewModels
{
    public class FlightLogValueViewModel
    {
        public int Id { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string DisplayText { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public FieldType FieldType { get; set; }
        public bool Required { get; set; }
        public bool IsRequired { get; set; }
    }

    public class FlightReviewViewModel
    {
        public int Id { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public FlightStatus Status { get; set; }
        public string Comments { get; set; } = string.Empty;
        public DateTime ReviewedAt { get; set; }
    }

    public class FlightLogFieldValueViewModel
    {
        public int FieldId { get; set; }
        public string DisplayText { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public FieldType FieldType { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}