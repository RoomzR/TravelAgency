namespace TravelAgency.BLL.Entities
{
    public class Promocode
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public decimal? MaxdiscountAmount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int? MaxUses { get; set; }
        public int CurrentUses { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}