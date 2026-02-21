using TravelAgency.DAL.Entities;

namespace TravelAgency.DAL.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId);
        Task<IEnumerable<Booking>> GetPendingBookingsAsync();
        Task<IEnumerable<Booking>> GetBookingsByTourAsync(int tourId);
        Task<Booking?> GetFullBookingAsync(int id);
        Task<bool> HasActiveBookingAsync(int tourId, string userId);
        Task<int> GetBookedPlacesAsync(int tourId);
        Task<IEnumerable<Booking>> GetBookingsByStatusAsync(string status);
        Task<bool> ExistsActiveBookingAsync(int tourId, string userId);
    }
}