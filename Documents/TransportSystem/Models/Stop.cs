namespace TransportSystem.Models
{
    public class Stop
    {
        public int Id { get; set; }
        public int? RouteId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? StopOrder { get; set; }

        // Navigation properties
        public Route? Route { get; set; }
        public ICollection<StudentRouteAssignment> StudentRouteAssignments { get; set; } = new List<StudentRouteAssignment>();
    }
}