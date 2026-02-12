using TravelAgency.DAL.Entities;

namespace TravelAgency.DAL.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetApprovedReviewsAsync(int tourId);
        Task<IEnumerable<Review>> GetPendingReviewsAsync();
        Task<double> GetAverageRatingAsync(int tourId);
    }
}