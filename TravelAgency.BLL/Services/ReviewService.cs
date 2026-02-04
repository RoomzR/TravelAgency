using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAgency.BLL.DTOs;
using TravelAgency.BLL.Entities;
using TravelAgency.BLL.Interfaces;

namespace TravelAgency.BLL.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ILogger<ReviewService> _logger;
        private readonly IMapper _mapper;

        public ReviewService(
            IReviewRepository reviewRepository,
            ILogger<ReviewService> logger,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ReviewDTO?> GetReviewByIdAsync(int id)
        {
            try
            {
                var review = await _reviewRepository.GetByIdAsync(id);
                return review == null ? null : _mapper.Map<ReviewDTO>(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review with ID: {ReviewId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsByTourAsync(int tourId)
        {
            try
            {
                var reviews = await _reviewRepository.GetApprovedReviewsAsync(tourId);
                return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for tour ID: {TourId}", tourId);
                throw;
            }
        }

        public async Task<IEnumerable<ReviewDTO>> GetPendingReviewsAsync()
        {
            try
            {
                var reviews = await _reviewRepository.GetPendingReviewsAsync();
                return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending reviews");
                throw;
            }
        }

        public async Task<ReviewDTO> CreateReviewAsync(ReviewCreateDTO createDto, string userId)
        {
            try
            {
                var review = _mapper.Map<Review>(createDto);
                review.ClientId = userId;
                review.CreatedDate = DateTime.UtcNow;
                review.IsApproved = false; 

                await _reviewRepository.CreateAsync(review);
                return _mapper.Map<ReviewDTO>(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review");
                throw;
            }
        }

        public async Task<bool> ApproveReviewAsync(int id)
        {
            try
            {
                var review = await _reviewRepository.GetByIdAsync(id);
                if (review == null) return false;

                review.IsApproved = true;
                await _reviewRepository.UpdateAsync(review);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving review with ID: {ReviewId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            try
            {
                await _reviewRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review with ID: {ReviewId}", id);
                throw;
            }
        }
    }
}