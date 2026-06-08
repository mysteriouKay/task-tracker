namespace TransportSystem.Models
{
    public class StudentRouteAssignment
    {
        public int Id { get; set; }
        public int? StudentId { get; set; }
        public int? RouteId { get; set; }
        public int? StopId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Student? Student { get; set; }
        public Route? Route { get; set; }
        public Stop? Stop { get; set; }
    }
}