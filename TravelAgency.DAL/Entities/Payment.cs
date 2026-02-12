using TravelAgency.DAL.Enums;

namespace TravelAgency.DAL.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string Method { get; set; } = "Card"; 
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? TransactionId { get; set; }

        public virtual Booking Booking { get; set; } = null!;
    }
}