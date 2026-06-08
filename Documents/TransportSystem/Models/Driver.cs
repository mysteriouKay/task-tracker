namespace TransportSystem.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public DateOnly? LicenseExpiry { get; set; }
        public string? Phone { get; set; }
        public string Status { get; set; } = "active";

        // Navigation properties
        public User? User { get; set; }
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}