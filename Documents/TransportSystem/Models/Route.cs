namespace TransportSystem.Models
{
    public class Route
    {
        public int Id { get; set; }
        public int? SchoolId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public School? School { get; set; }
        public ICollection<Stop> Stops { get; set; } = new List<Stop>();
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public ICollection<StudentRouteAssignment> StudentRouteAssignments { get; set; } = new List<StudentRouteAssignment>();
    }
}