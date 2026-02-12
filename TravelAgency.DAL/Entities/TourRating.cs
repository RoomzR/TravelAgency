namespace TravelAgency.DAL.Entities
{
    public class TourRating
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string UserId { get; set; } = null!;
        public int Rating { get; set; }
        public DateTime RatingDate { get; set; } = DateTime.UtcNow;

        public virtual Tour Tour { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}