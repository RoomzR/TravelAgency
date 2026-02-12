using TravelAgency.DAL.Enums;

namespace TravelAgency.DAL.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string ClientId { get; set; } = null!;
        public int PeopleCount { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public string? Comments { get; set; }
        public string? ManagerConfirmedId { get; set; }
        public int? PromoCodeId { get; set; }
        public decimal? DiscountAmount { get; set; }

        public decimal FinalPrice => TotalPrice - (DiscountAmount ?? 0);

        public virtual Promocode? PromoCode { get; set; }
        
        public virtual Tour Tour { get; set; } = null!;
        public virtual ApplicationUser Client { get; set; } = null!;
        public virtual ApplicationUser? ManagerConfirmed { get; set; }
        public virtual Payment? Payment { get; set; }
    }
}