using Microsoft.EntityFrameworkCore;
namespace TransportSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<TransportSystem.Models.School> Schools { get; set; }
        public DbSet<TransportSystem.Models.User> Users { get; set; }
        public DbSet<TransportSystem.Models.Driver> Drivers { get; set; }
        public DbSet<TransportSystem.Models.Vehicle> Vehicles { get; set; }
        public DbSet<TransportSystem.Models.Route> Routes { get; set; }
        public DbSet<TransportSystem.Models.Stop> Stops { get; set; }
        public DbSet<TransportSystem.Models.Student> Students { get; set; }
        public DbSet<TransportSystem.Models.StudentRouteAssignment> StudentRouteAssignments { get; set; }
        public DbSet<TransportSystem.Models.Trip> Trips { get; set; }
        public DbSet<TransportSystem.Models.TripAttendance> TripAttendances { get; set; }
        public DbSet<TransportSystem.Models.Notification> Notifications { get; set; }
        public DbSet<TransportSystem.Models.Payment> Payments { get; set; }
        public DbSet<TransportSystem.Models.FeeStructure> FeeStructures { get; set; }
        public DbSet<TransportSystem.Models.TripLocation> TripLocations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TransportSystem.Models.Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.Student)
                .HasForeignKey<TransportSystem.Models.Student>(s => s.UserId);
            modelBuilder.Entity<TransportSystem.Models.Student>()
                .HasOne(s => s.Parent)
                .WithMany()
                .HasForeignKey(s => s.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}