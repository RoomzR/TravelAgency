using TravelAgency.BLL.DTOs;

namespace TravelAgency.BLL.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDashboardDTO> GetDashboardDataAsync();

        Task<IEnumerable<UserDTO>> GetUsersAsync();
        Task<bool> UpdateUserRoleAsync(string userId, string newRoleName);
        Task<bool> DeleteUserAsync(string userId);

        Task<IEnumerable<BookingDTO>> GetAllBookingsAsync();
        Task<bool> UpdateBookingStatusAsync(int bookingId, string status);

        Task ApproveReviewAsync(int reviewId);
        Task<IEnumerable<ContactRequestDTO>> GetContactRequestsAsync();

        Task CreateFAQAsync(FAQCreateDTO faqDto);
        Task CreatePromoCodeAsync(PromoCodeCreateDTO promoDto);
        Task<bool> BlockUserAsync(string userId);
        Task<bool> UnblockUserAsync(string userId);

        Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync();
        Task ToggleReviewApprovalAsync(int reviewId);
        Task<IEnumerable<PromoCodeDTO>> GetAllPromoCodesAsync();
        Task TogglePromoCodeStatusAsync(int promoId);
        Task DeletePromoCodeAsync(int id);
    }
}