using Microsoft.EntityFrameworkCore;
using TravelAgency.DAL.Entities;
using TravelAgency.DAL.Interfaces;
using TravelAgency.DAL.Data;

namespace TravelAgency.DAL.Repositories
{
    public class TourRepository : BaseRepository<Tour>, ITourRepository
    {
        public TourRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Tour>> GetActiveToursAsync()
        {
            return await _context.Tours
                .Where(t => t.IsActive && t.StartDate > DateTime.UtcNow)
                .Include(t => t.Country)
                .Include(t => t.City)
                .Include(t => t.HotelCategory)
                .Include(t => t.TourType)
                .Include(t => t.Hotel)
                .Include(t => t.TourImages)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tour>> GetHotDealsAsync(int count)
        {
            return await _context.Tours
                .Where(t => t.IsActive && t.IsHotDeal && t.StartDate > DateTime.UtcNow)
                .Include(t => t.Country)
                .Include(t => t.City)
                .Include(t => t.HotelCategory)
                .Include(t => t.TourType)
                .OrderBy(t => t.Price)
                .ThenByDescending(t => t.DiscountPercent)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tour>> GetPopularToursAsync(int count)
        {
            return await _context.Tours
                .Where(t => t.IsActive && t.StartDate > DateTime.UtcNow)
                .Include(t => t.Country)
                .Include(t => t.City)
                .Include(t => t.HotelCategory)
                .Include(t => t.TourType)
                .OrderByDescending(t => t.ViewsCount)
                .ThenByDescending(t => t.BookingsCount)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tour>> SearchToursAsync(
            string? searchTerm = null,
            int? countryId = null,
            int? tourTypeId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? minDuration = null,
            int? maxDuration = null,
            bool? isHotDeal = null,
            DateTime? startDateFrom = null,
            DateTime? startDateTo = null)
        {
            var query = BuildSearchQuery(
                searchTerm, countryId, tourTypeId, minPrice, maxPrice,
                minDuration, maxDuration, isHotDeal, startDateFrom, startDateTo);

            return await query
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tour>> SearchToursAdvancedAsync(
            string? searchTerm = null,
            int? countryId = null,
            int? tourTypeId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? minDuration = null,
            int? maxDuration = null,
            bool? isHotDeal = null,
            DateTime? startDateFrom = null,
            DateTime? startDateTo = null,
            string? sortBy = null,
            bool? sortDescending = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = BuildSearchQuery(
                searchTerm, countryId, tourTypeId, minPrice, maxPrice,
                minDuration, maxDuration, isHotDeal, startDateFrom, startDateTo);

            query = ApplySorting(query, sortBy, sortDescending);

            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return await query.ToListAsync();
        }

        private IQueryable<Tour> BuildSearchQuery(
            string? searchTerm,
            int? countryId,
            int? tourTypeId,
            decimal? minPrice,
            decimal? maxPrice,
            int? minDuration,
            int? maxDuration,
            bool? isHotDeal,
            DateTime? startDateFrom,
            DateTime? startDateTo)
        {
            var query = _context.Tours
                .Where(t => t.IsActive)
                .Include(t => t.Country)
                .Include(t => t.City)
                .Include(t => t.HotelCategory)
                .Include(t => t.TourType)
                .Include(t => t.Hotel)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(t =>
                    (t.Title != null && t.Title.ToLower().Contains(term)) ||
                    (t.Description != null && t.Description.ToLower().Contains(term)) ||
                    (t.Country != null && t.Country.Name != null && t.Country.Name.ToLower().Contains(term)) ||
                    (t.City != null && t.City.Name != null && t.City.Name.ToLower().Contains(term)));
            }

            if (countryId.HasValue)
                query = query.Where(t => t.CountryId == countryId.Value);

            if (tourTypeId.HasValue)
                query = query.Where(t => t.TourTypeId == tourTypeId.Value);

            if (minPrice.HasValue)
                query = query.Where(t => t.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(t => t.Price <= maxPrice.Value);

            if (minDuration.HasValue)
                query = query.Where(t => t.DurationDays >= minDuration.Value);

            if (maxDuration.HasValue)
                query = query.Where(t => t.DurationDays <= maxDuration.Value);

            if (isHotDeal.HasValue)
                query = query.Where(t => t.IsHotDeal == isHotDeal.Value);

            if (startDateFrom.HasValue)
                query = query.Where(t => t.StartDate >= startDateFrom.Value);

            if (startDateTo.HasValue)
                query = query.Where(t => t.StartDate <= startDateTo.Value);

            return query;
        }

        private IQueryable<Tour> ApplySorting(IQueryable<Tour> query, string? sortBy, bool? descending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query.OrderByDescending(t => t.CreatedDate);

            switch (sortBy.ToLower())
            {
                case "price":
                    return descending == true
                        ? query.OrderByDescending(t => t.Price)
                        : query.OrderBy(t => t.Price);
                case "date":
                    return descending == true
                        ? query.OrderByDescending(t => t.StartDate)
                        : query.OrderBy(t => t.StartDate);
                case "name":
                    return descending == true
                        ? query.OrderByDescending(t => t.Title)
                        : query.OrderBy(t => t.Title);
                case "duration":
                    return descending == true
                        ? query.OrderByDescending(t => t.DurationDays)
                        : query.OrderBy(t => t.DurationDays);
                case "rating":
                    return query.OrderByDescending(t => t.Reviews != null
                        ? t.Reviews.Average(r => r.Rating)
                        : 0);
                default:
                    return query.OrderByDescending(t => t.CreatedDate);
            }
        }

        public async Task IncrementViewsAsync(int tourId)
        {
            var tour = await _context.Tours.FindAsync(tourId);
            if (tour != null)
            {
                tour.ViewsCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Tour>> GetToursByCountryAsync(int countryId)
        {
            return await _context.Tours
                .Where(t => t.IsActive && t.CountryId == countryId && t.StartDate > DateTime.UtcNow)
                .Include(t => t.Country)
                .Include(t => t.City)
                .Include(t => t.HotelCategory)
                .Include(t => t.TourType)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tour>> GetToursByTypeAsync(int typeId)
        {
            return await _context.Tours
                .Where(t => t.IsActive && t.TourTypeId == typeId && t.StartDate > DateTime.UtcNow)
                .Include(t => t.Country)
                .Include(t => t.City)
                .Include(t => t.HotelCategory)
                .Include(t => t.TourType)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public override async Task<Tour?> GetByIdAsync(int id)
        {
            return await _context.Tours
                .Include(t => t.Country)
                .Include(t => t.City)
                .Include(t => t.HotelCategory)
                .Include(t => t.TourType)
                .Include(t => t.Hotel)
                .Include(t => t.TourImages)
                .Include(t => t.Reviews)
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<IEnumerable<Hotel>> GetAllHotelsAsync()
        {
            return await _context.Set<Hotel>()
                .OrderBy(h => h.Name)
                .ToListAsync();
        }
        public async Task<IEnumerable<Hotel>> GetHotelsByCountryAsync(int? countryId)
        {
            if (!countryId.HasValue)
                return Enumerable.Empty<Hotel>();

            return await _context.Set<Hotel>()
                .Include(h => h.City)
                .Where(h => h.City.CountryId == countryId.Value) 
                .OrderBy(h => h.Name)
                .ToListAsync();
        }
        public async Task<Hotel?> GetHotelByIdAsync(int id)
        {
            return await _context.Set<Hotel>().FindAsync(id);
        }
        public async Task<IEnumerable<City>> GetCitiesByCountryAsync(int countryId)
        {
            return await _context.Set<City>()
                .Where(c => c.CountryId == countryId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}