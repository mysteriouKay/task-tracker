namespace TransportSystem.Models
{
    public class FeeStructure
    {
        public int Id { get; set; }
        public int? SchoolId { get; set; }
        public string Term { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public School? School { get; set; }
    }
}