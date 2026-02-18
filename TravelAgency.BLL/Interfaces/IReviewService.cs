using TravelAgency.BLL.DTOs;

namespace TravelAgency.BLL.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDTO?> GetReviewByIdAsync(int id);
        Task<IEnumerable<ReviewDTO>> GetReviewsByTourAsync(int tourId);
        Task<IEnumerable<ReviewDTO>> GetPendingReviewsAsync();
        Task<ReviewDTO> CreateReviewAsync(ReviewCreateDTO createDto, string userId);
        Task<bool> ApproveReviewAsync(int id);
        Task<bool> DeleteReviewAsync(int id);
        Task<IEnumerable<ReviewDTO>> GetAllApprovedReviewsAsync(int count);
    }
}