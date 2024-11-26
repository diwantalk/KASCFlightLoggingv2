using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KASCFlightLogging.Models
{

    public static class StandardFields
    {
        public const string DepartureTime = "DepartureTime";
        public const string ArrivalTime = "ArrivalTime";
        public const string TotalFlightTime = "TotalFlightTime";
    }

    public class FlightLogField
    {
        public FlightLogField()
        {
            Name = string.Empty;
            DisplayText = string.Empty;
            Description = string.Empty;
            Required = false;
            Order = 0;
            FieldType = FieldType.Text;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string DisplayText { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        public bool Required { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public FieldType FieldType { get; set; }

        [Required]
        [ForeignKey(nameof(AircraftType))]
        public int AircraftTypeId { get; set; }

        public virtual AircraftType? AircraftType { get; set; }
    }
}
