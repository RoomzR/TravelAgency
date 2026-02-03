namespace TravelAgency.BLL.Entities
{
    public class Wishlist
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int TourId { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Tour Tour { get; set; } = null!;

    }
}