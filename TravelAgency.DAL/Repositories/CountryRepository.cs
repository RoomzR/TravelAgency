using Microsoft.EntityFrameworkCore;
using TravelAgency.BLL.Entities;
using TravelAgency.BLL.Interfaces;
using TravelAgency.DAL.Data;

namespace TravelAgency.DAL.Repositories
{
    public class CountryRepository : BaseRepository<Country>, ICountryRepository
    {
        public CountryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Country>> GetPopularCountriesAsync(int count)
        {
            return await _context.Countries
                .OrderByDescending(c => c.Tours.Count(t => t.IsActive))
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Country>> GetAllWithCitiesAsync()
        {
            return await _context.Countries
                .Include(c => c.Cities)
                .ThenInclude(city => city.Tours.Where(t => t.IsActive))
                .ToListAsync();
        }

        public async Task<Country?> GetWithCitiesAsync(int id)
        {
            return await _context.Countries
                .Include(c => c.Cities)
                .ThenInclude(city => city.Hotels)
                .Include(c => c.Tours.Where(t => t.IsActive))
                .ThenInclude(t => t.City)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> HasToursAsync(int id)
        {
            return await _context.Tours.AnyAsync(t => t.CountryId == id && t.IsActive);
        }
    }
}