namespace TransportSystem.Models
{
    public class TripAttendance
    {
        public int Id { get; set; }
        public int? TripId { get; set; }
        public int? StudentId { get; set; }
        public string? Status { get; set; }
        public DateTime NotedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Trip? Trip { get; set; }
        public Student? Student { get; set; }
    }
}