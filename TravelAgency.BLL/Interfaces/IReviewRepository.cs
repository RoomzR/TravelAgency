using TravelAgency.BLL.Entities;

namespace TravelAgency.BLL.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetApprovedReviewsAsync(int tourId);
        Task<IEnumerable<Review>> GetPendingReviewsAsync();
        Task<double> GetAverageRatingAsync(int tourId);
    }
}