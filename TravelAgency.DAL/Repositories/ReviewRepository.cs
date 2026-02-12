using Microsoft.EntityFrameworkCore;
using TravelAgency.DAL.Entities;
using TravelAgency.DAL.Interfaces;
using TravelAgency.DAL.Data;

namespace TravelAgency.DAL.Repositories
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Review>> GetApprovedReviewsAsync(int tourId)
        {
            return await _context.Reviews
                .Where(r => r.TourId == tourId && r.IsApproved)
                .Include(r => r.Client)
                .Include(r => r.Tour)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetPendingReviewsAsync()
        {
            return await _context.Reviews
                .Where(r => !r.IsApproved)
                .Include(r => r.Client)
                .Include(r => r.Tour)
                .OrderBy(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(int tourId)
        {
            var avgRating = await _context.Reviews
                .Where(r => r.TourId == tourId && r.IsApproved)
                .AverageAsync(r => (double?)r.Rating) ?? 0;

            return avgRating;
        }

        public override async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.Client)
                .Include(r => r.Tour)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}