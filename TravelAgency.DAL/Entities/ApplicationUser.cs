using Microsoft.AspNetCore.Identity;

namespace TravelAgency.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PassportData { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
        public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
        public virtual ICollection<TourRating> TourRatings { get; set; } = new List<TourRating>();
    }
}