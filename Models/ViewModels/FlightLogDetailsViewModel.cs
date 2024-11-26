using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KASCFlightLogging.Models.ViewModels
{
    public class FlightLogDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Flight Date")]
        [DataType(DataType.Date)]
        public DateTime FlightDate { get; set; }

        [Display(Name = "Aircraft")]
        public string AircraftRegistration { get; set; } = string.Empty;

        [Display(Name = "Aircraft Type")]
        public string AircraftType { get; set; } = string.Empty;

        [Display(Name = "Created By")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Pilot")]
        public string PilotName { get; set; } = string.Empty;

        [Display(Name = "Total Flight Time")]
        public TimeSpan TotalFlightTime { get; set; }

        [Display(Name = "Status")]
        public FlightStatus Status { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime? LastModifiedAt { get; set; }

        [Display(Name = "Published")]
        public bool IsPublished { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Published By")]
        public string? PublishedByName { get; set; }

        [Display(Name = "Modified By")]
        public string? ModifiedByName { get; set; }

        public List<FlightLogValueViewModel> Values { get; set; } = new();
        public List<FlightReviewViewModel> Reviews { get; set; } = new();
    }


}