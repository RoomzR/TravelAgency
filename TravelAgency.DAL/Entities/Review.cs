namespace TravelAgency.DAL.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string ClientId { get; set; } = null!;
        public int Rating { get; set; } 
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } = false; 
        
        public virtual Tour Tour { get; set; } = null!;
        public virtual ApplicationUser Client { get; set; } = null!;
    }
}