using Microsoft.EntityFrameworkCore;
using TravelAgency.BLL.DTOs;
using TravelAgency.BLL.Entities;
using TravelAgency.BLL.Interfaces;
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

        public async Task<IEnumerable<Tour>> SearchToursAsync(TourSearchDTO searchDto)
        {
            var query = _context.Tours
                .Where(t => t.IsActive)
                .Include(t => t.Country)
                .Include(t => t.City)
                .Include(t => t.HotelCategory)
                .Include(t => t.TourType)
                .Include(t => t.Hotel)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var term = searchDto.SearchTerm.ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(term) ||
                    t.Description.ToLower().Contains(term) ||
                    t.Country.Name.ToLower().Contains(term) ||
                    t.City.Name.ToLower().Contains(term));
            }

            if (searchDto.CountryId.HasValue)
                query = query.Where(t => t.CountryId == searchDto.CountryId.Value);

            if (searchDto.TourTypeId.HasValue)
                query = query.Where(t => t.TourTypeId == searchDto.TourTypeId.Value);

            if (searchDto.MinPrice.HasValue)
                query = query.Where(t => t.Price >= searchDto.MinPrice.Value);

            if (searchDto.MaxPrice.HasValue)
                query = query.Where(t => t.Price <= searchDto.MaxPrice.Value);

            return await query
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<Tour>> SearchToursAdvancedAsync(TourSearchDTO searchDto)
        {
            var query = _context.Tours
                .Where(t => t.IsActive)
                .Include(t => t.Country)
                .Include(t => t.City)
                .Include(t => t.HotelCategory)
                .Include(t => t.TourType)
                .Include(t => t.Hotel)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var term = searchDto.SearchTerm.ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(term) ||
                    t.Description.ToLower().Contains(term) ||
                    t.Country.Name.ToLower().Contains(term) ||
                    t.City.Name.ToLower().Contains(term));
            }

            if (searchDto.CountryId.HasValue)
                query = query.Where(t => t.CountryId == searchDto.CountryId.Value);

            if (searchDto.TourTypeId.HasValue)
                query = query.Where(t => t.TourTypeId == searchDto.TourTypeId.Value);

            if (searchDto.MinPrice.HasValue)
                query = query.Where(t => t.Price >= searchDto.MinPrice.Value);

            if (searchDto.MaxPrice.HasValue)
                query = query.Where(t => t.Price <= searchDto.MaxPrice.Value);

            if (searchDto.IsHotDeal.HasValue)
                query = query.Where(t => t.IsHotDeal == searchDto.IsHotDeal.Value);

            if (!string.IsNullOrEmpty(searchDto.SortBy))
            {
                query = searchDto.SortBy.ToLower() switch
                {
                    "price" => searchDto.SortDescending == true
                        ? query.OrderByDescending(t => t.Price)
                        : query.OrderBy(t => t.Price),
                    "date" => searchDto.SortDescending == true
                        ? query.OrderByDescending(t => t.StartDate)
                        : query.OrderBy(t => t.StartDate),
                    "name" => searchDto.SortDescending == true
                        ? query.OrderByDescending(t => t.Title)
                        : query.OrderBy(t => t.Title),
                    _ => query.OrderByDescending(t => t.CreatedDate)
                };
            }
            else
            {
                query = query.OrderByDescending(t => t.CreatedDate);
            }

            return await query.ToListAsync();
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
        private IQueryable<Tour> ApplySorting(IQueryable<Tour> query, string? sortBy, bool? descending)
        {
            query = (sortBy?.ToLower()) switch
            {
                "price" => descending == true ? query.OrderByDescending(t => t.Price) : query.OrderBy(t => t.Price),
                "date" => descending == true ? query.OrderByDescending(t => t.StartDate) : query.OrderBy(t => t.StartDate),
                "name" => descending == true ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                "duration" => descending == true ? query.OrderByDescending(t => t.DurationDays) : query.OrderBy(t => t.DurationDays),
                "rating" => query.OrderByDescending(t => t.TourRatings.Average(r => r.Rating)),
                _ => query.OrderByDescending(t => t.CreatedDate) 
            };

            return query;
        }
    }
}