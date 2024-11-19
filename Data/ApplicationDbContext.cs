using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KASCFlightLogging.Models;

namespace KASCFlightLogging.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Aircraft> Aircraft { get; set; }
    public DbSet<AircraftType> AircraftTypes { get; set; }
    public DbSet<FlightLog> FlightLogs { get; set; }
    public DbSet<FlightLogField> FlightLogFields { get; set; }
    public DbSet<FlightLogValue> FlightLogValues { get; set; }
    public DbSet<FlightReview> FlightReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure relationships and constraints
        builder.Entity<Aircraft>()
            .HasOne(a => a.Type)
            .WithMany(t => t.Aircraft)
            .HasForeignKey(a => a.TypeId);

        builder.Entity<FlightLog>()
            .HasOne(fl => fl.Aircraft)
            .WithMany(a => a.FlightLogs)
            .HasForeignKey(fl => fl.AircraftId);

        builder.Entity<FlightLog>()
            .HasOne(fl => fl.Pilot)
            .WithMany(p => p.FlightLogs)
            .HasForeignKey(fl => fl.PilotId);

        builder.Entity<FlightLogValue>()
            .HasOne(flv => flv.FlightLog)
            .WithMany(fl => fl.Values)
            .HasForeignKey(flv => flv.FlightLogId);

        builder.Entity<FlightLogValue>()
            .HasOne(flv => flv.Field)
            .WithMany(flf => flf.Values)
            .HasForeignKey(flv => flv.FieldId);

        builder.Entity<FlightReview>()
            .HasOne(fr => fr.FlightLog)
            .WithMany(fl => fl.Reviews)
            .HasForeignKey(fr => fr.FlightLogId);

        builder.Entity<FlightReview>()
            .HasOne(fr => fr.Reviewer)
            .WithMany(r => r.Reviews)
            .HasForeignKey(fr => fr.ReviewerId);
    }
}
