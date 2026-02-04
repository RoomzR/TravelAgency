using Microsoft.EntityFrameworkCore;
using TravelAgency.BLL.Entities;
using TravelAgency.BLL.Interfaces;
using TravelAgency.DAL.Data;

namespace TravelAgency.DAL.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId)
        {
            return await _context.Bookings
                .Where(b => b.ClientId == userId)
                .Include(b => b.Tour)
                .ThenInclude(t => t.Country)
                .Include(b => b.Tour)
                .ThenInclude(t => t.City)
                .Include(b => b.Payment)
                .Include(b => b.PromoCode)
                .Include(b => b.ManagerConfirmed)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetPendingBookingsAsync()
        {
            return await _context.Bookings
                .Where(b => b.Status == BLL.Enums.BookingStatus.Pending)
                .Include(b => b.Tour)
                .Include(b => b.Client)
                .Include(b => b.PromoCode)
                .OrderBy(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByTourAsync(int tourId)
        {
            return await _context.Bookings
                .Where(b => b.TourId == tourId)
                .Include(b => b.Client)
                .Include(b => b.Payment)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<Booking?> GetFullBookingAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Tour)
                .ThenInclude(t => t.Country)
                .Include(b => b.Tour)
                .ThenInclude(t => t.City)
                .Include(b => b.Client)
                .Include(b => b.Payment)
                .Include(b => b.PromoCode)
                .Include(b => b.ManagerConfirmed)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> HasActiveBookingAsync(int tourId, string userId)
        {
            return await _context.Bookings
                .AnyAsync(b => b.TourId == tourId &&
                             b.ClientId == userId &&
                             b.Status != BLL.Enums.BookingStatus.Cancelled);
        }

        public async Task<int> GetBookedPlacesAsync(int tourId)
        {
            return await _context.Bookings
                .Where(b => b.TourId == tourId &&
                          b.Status != BLL.Enums.BookingStatus.Cancelled)
                .SumAsync(b => b.PeopleCount);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByStatusAsync(string status)
        {
            if (!Enum.TryParse<BLL.Enums.BookingStatus>(status, out var statusEnum))
                return Enumerable.Empty<Booking>();

            return await _context.Bookings
                .Where(b => b.Status == statusEnum)
                .Include(b => b.Tour)
                .Include(b => b.Client)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }
    }
}