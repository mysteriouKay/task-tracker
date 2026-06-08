namespace TransportSystem.Models
{
    public class Trip
    {
        public int Id { get; set; }
        public int? RouteId { get; set; }
        public int? VehicleId { get; set; }
        public int? DriverId { get; set; }
        public DateOnly TripDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = "scheduled";
        public TimeOnly? ScheduledTime { get; set; }

        // Navigation properties
        public Route? Route { get; set; }
        public Vehicle? Vehicle { get; set; }
        public Driver? Driver { get; set; }
        public ICollection<TripAttendance> TripAttendances { get; set; } = new List<TripAttendance>();
    }
}