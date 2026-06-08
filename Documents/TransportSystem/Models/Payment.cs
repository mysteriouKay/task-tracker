namespace TransportSystem.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = "cash";
        public string? Term { get; set; }
        public string? Notes { get; set; }

        // Navigation
        public Student? Student { get; set; }
    }
}