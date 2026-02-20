using TravelAgency.BLL.DTOs;

namespace TravelAgency.BLL.Interfaces
{
    public interface IBookingService
    {
        Task<BookingDTO?> GetBookingByIdAsync(int id);
        Task<IEnumerable<BookingDTO>> GetUserBookingsAsync(string userId);
        Task<IEnumerable<BookingDTO>> GetPendingBookingsAsync();
        Task<IEnumerable<BookingDTO>> GetBookingsByTourAsync(int tourId);
        Task<BookingDTO> CreateBookingAsync(BookingCreateDTO createDto, string clientId);
        Task<BookingDTO?> UpdateBookingStatusAsync(BookingUpdateDTO updateDto);
        Task<bool> CancelBookingAsync(int id, string userId);
        Task<bool> CheckTourAvailabilityAsync(int tourId, int peopleCount);
        Task<decimal> CalculateBookingPriceAsync(int tourId, int peopleCount, string? promoCode = null);
        Task<IEnumerable<BookingDTO>> GetAllBookingsAsync();
    }
}