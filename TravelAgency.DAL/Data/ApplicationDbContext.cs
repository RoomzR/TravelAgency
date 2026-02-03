using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TravelAgency.BLL.Entities;

namespace TravelAgency.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<HotelCategory> HotelCategories { get; set; } = null!;
        public DbSet<TourType> TourTypes { get; set; } = null!;
        public DbSet<Hotel> Hotels { get; set; } = null!;
        public DbSet<Tour> Tours { get; set; } = null!;
        public DbSet<TourImage> TourImages { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<NewsArticle> NewsArticles { get; set; } = null!;
        public DbSet<Promocode> PromoCodes { get; set; } = null!;
        public DbSet<Wishlist> Wishlists { get; set; } = null!;
        public DbSet<TourRating> TourRatings { get; set; } = null!;
        public DbSet<FAQ> FAQs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Tour>()
                .HasMany(t => t.Bookings)
                .WithOne(b => b.Tour)
                .HasForeignKey(b => b.TourId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.Client)
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Payment)
                .WithOne(p => p.Booking)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Tour>()
                .HasMany(t => t.Reviews)
                .WithOne(r => r.Tour)
                .HasForeignKey(r => r.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.Client)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.NewsArticles)
                .WithOne(n => n.Author)
                .HasForeignKey(n => n.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Country>()
                .HasMany(c => c.Cities)
                .WithOne(c => c.Country)
                .HasForeignKey(c => c.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<City>()
                .HasMany(c => c.Hotels)
                .WithOne(h => h.City)
                .HasForeignKey(h => h.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HotelCategory>()
                .HasMany(hc => hc.Hotels)
                .WithOne(h => h.HotelCategory)
                .HasForeignKey(h => h.HotelCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Hotel>()
                .HasMany(h => h.Tours)
                .WithOne(t => t.Hotel)
                .HasForeignKey(t => t.HotelId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Tour>()
                .HasMany(t => t.TourImages)
                .WithOne(ti => ti.Tour)
                .HasForeignKey(ti => ti.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Promocode>()
                .HasMany(pc => pc.Bookings)
                .WithOne(b => b.PromoCode)
                .HasForeignKey(b => b.PromoCodeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Wishlists)
                .WithOne(w => w.User)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Tour>()
                .HasMany(t => t.Wishlists)
                .WithOne(w => w.Tour)
                .HasForeignKey(w => w.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.TourRatings)
                .WithOne(tr => tr.User)
                .HasForeignKey(tr => tr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Tour>()
                .HasMany(t => t.TourRatings)
                .WithOne(tr => tr.Tour)
                .HasForeignKey(tr => tr.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Wishlist>()
                .HasIndex(w => new { w.UserId, w.TourId })
                .IsUnique();

            builder.Entity<TourRating>()
                .HasIndex(tr => new { tr.UserId, tr.TourId })
                .IsUnique();

            builder.Entity<Promocode>()
                .HasIndex(pc => pc.Code)
                .IsUnique();

            builder.Entity<Review>()
                .Property(r => r.Rating)
                .HasPrecision(1, 0);

            builder.Entity<TourRating>()
                .Property(tr => tr.Rating)
                .HasPrecision(2, 0);

            builder.Entity<Tour>()
                .Property(t => t.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<Booking>()
                .Property(b => b.BookingDate)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<Review>()
                .Property(r => r.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<NewsArticle>()
                .Property(n => n.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<FAQ>()
                .Property(f => f.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<Wishlist>()
                .Property(w => w.AddedDate)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<TourRating>()
                .Property(tr => tr.RatingDate)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<Tour>()
                .HasIndex(t => t.IsActive);

            builder.Entity<Tour>()
                .HasIndex(t => t.IsHotDeal);

            builder.Entity<Tour>()
                .HasIndex(t => t.StartDate);

            builder.Entity<Booking>()
                .HasIndex(b => b.Status);

            builder.Entity<Booking>()
                .HasIndex(b => b.BookingDate);

            builder.Entity<Promocode>()
                .HasIndex(pc => pc.IsActive);

            builder.Entity<Promocode>()
                .HasIndex(pc => pc.ValidTo);
        }
    }
}