using KASCFlightLogging.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace KASCFlightLogging.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Aircraft> Aircraft { get; set; }
        public DbSet<AircraftType> AircraftTypes { get; set; }
        public DbSet<FlightLog> FlightLogs { get; set; }
        public DbSet<FlightLogValue> FlightLogValues { get; set; }
        public DbSet<FlightLogField> FlightLogFields { get; set; }
        public DbSet<FlightReview> FlightReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Identity tables
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(m => m.Id).HasMaxLength(450);
                entity.Property(m => m.Email).HasMaxLength(256);
                entity.Property(m => m.NormalizedEmail).HasMaxLength(256);
                entity.Property(m => m.UserName).HasMaxLength(256);
                entity.Property(m => m.NormalizedUserName).HasMaxLength(256);
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.Property(m => m.Id).HasMaxLength(450);
                entity.Property(m => m.Name).HasMaxLength(256);
                entity.Property(m => m.NormalizedName).HasMaxLength(256);
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(m => m.LoginProvider).HasMaxLength(128);
                entity.Property(m => m.ProviderKey).HasMaxLength(128);
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(m => m.LoginProvider).HasMaxLength(128);
                entity.Property(m => m.Name).HasMaxLength(128);
            });

            // Configure AircraftType
            builder.Entity<AircraftType>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            // Configure Aircraft
            builder.Entity<Aircraft>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.AircraftType)
                    .WithMany(e => e.Aircraft)
                    .HasForeignKey(e => e.AircraftTypeId);

                entity.Property(e => e.RegistrationNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Model)
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(200);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure FlightLog
            builder.Entity<FlightLog>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Pilot)
                    .WithMany()
                    .HasForeignKey(e => e.PilotId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Aircraft)
                    .WithMany(e => e.FlightLogs)
                    .HasForeignKey(e => e.AircraftId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PublishedBy)
                    .WithMany()
                    .HasForeignKey(e => e.PublishedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ModifiedBy)
                    .WithMany()
                    .HasForeignKey(e => e.ModifiedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.IsPublished)
                    .IsRequired()
                    .HasDefaultValue(false);
            });

            // Configure FlightLogField
            builder.Entity<FlightLogField>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.AircraftType)
                    .WithMany(e => e.FlightLogFields)
                    .HasForeignKey(e => e.AircraftTypeId);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DisplayText)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            // Configure FlightLogValue
            builder.Entity<FlightLogValue>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.FlightLog)
                    .WithMany(e => e.Values)
                    .HasForeignKey(e => e.FlightLogId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.FlightLogField)
                    .WithMany()
                    .HasForeignKey(e => e.FlightLogFieldId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Value)
                    .IsRequired();
            });

            // Configure FlightReview
            builder.Entity<FlightReview>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.FlightLog)
                    .WithMany(e => e.Reviews)
                    .HasForeignKey(e => e.FlightLogId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Comments)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            // Seed default admin role
            var adminRoleId = "1";
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });

            // Seed default admin user
            var adminUserId = "1";
            var hasher = new PasswordHasher<ApplicationUser>();
            var adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@kasc.com",
                NormalizedUserName = "ADMIN@KASC.COM",
                Email = "admin@kasc.com",
                NormalizedEmail = "ADMIN@KASC.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                FirstName = "System",
                LastName = "Administrator"
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123456");

            builder.Entity<ApplicationUser>().HasData(adminUser);

            // Assign admin role to admin user
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            });
        }
    }
}