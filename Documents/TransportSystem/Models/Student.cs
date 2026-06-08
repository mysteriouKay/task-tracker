namespace TransportSystem.Models
{
    public class Student
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? SchoolId { get; set; }
        public string? Grade { get; set; }
        public int? ParentId { get; set; }
        public int? VehicleId { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public School? School { get; set; }
        public User? Parent { get; set; }
        public Vehicle? Vehicle { get; set; }
        public ICollection<StudentRouteAssignment> StudentRouteAssignments { get; set; } = new List<StudentRouteAssignment>();
        public ICollection<TripAttendance> TripAttendances { get; set; } = new List<TripAttendance>();
    }
}