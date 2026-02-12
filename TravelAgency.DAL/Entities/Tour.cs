using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TravelAgency.DAL.Entities
{
    public class Tour
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public int HotelCategoryId { get; set; }
        public int TourTypeId { get; set; }
        public int? HotelId { get; set; }
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsHotDeal { get; set; }
        public int MaxPeopleCount { get; set; }
        public DateTime StartDate { get; set; }
        public string? ImageUrlsJson { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string? CreatedById { get; set; }
        public int ViewsCount { get; set; }
        public int BookingsCount { get; set; }

        public virtual Country Country { get; set; } = null!;
        public virtual City City { get; set; } = null!;
        public virtual HotelCategory HotelCategory { get; set; } = null!;
        public virtual TourType TourType { get; set; } = null!;
        public virtual Hotel? Hotel { get; set; }
        public virtual ApplicationUser? CreatedBy { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<TourImage> TourImages { get; set; } = new List<TourImage>();
        public virtual ICollection<TourRating> TourRatings { get; set; } = new List<TourRating>();
        public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

        [NotMapped]
        public int AvailablePlaces => MaxPeopleCount - Bookings
            .Where(b => b.Status != Enums.BookingStatus.Cancelled)
            .Sum(b => b.PeopleCount);

        [NotMapped]
        public double AverageRating => TourRatings.Any() ?
            TourRatings.Average(r => r.Rating) : 0;

        [NotMapped]
        public decimal DiscountedPrice => OriginalPrice.HasValue ?
            OriginalPrice.Value * (1 - DiscountPercent / 100) : Price;
    }
}