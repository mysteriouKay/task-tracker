namespace TransportSystem.Models
{
    public class TripLocation
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Trip? Trip { get; set; }
    }
}