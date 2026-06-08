namespace TransportSystem.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public int? SchoolId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string? Model { get; set; }
        public int? Capacity { get; set; }
        public string Status { get; set; } = "active";
        public string Condition { get; set; } = "good";
        public School? School { get; set; }
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}