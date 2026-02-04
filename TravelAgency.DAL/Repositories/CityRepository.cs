using Microsoft.EntityFrameworkCore;
using TravelAgency.BLL.Entities;
using TravelAgency.BLL.Interfaces;
using TravelAgency.DAL.Data;

namespace TravelAgency.DAL.Repositories
{
    public class CityRepository : BaseRepository<City>, ICityRepository
    {
        public CityRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<City>> GetCitiesByCountryAsync(int countryId)
        {
            return await _context.Cities
                .Where(c => c.CountryId == countryId)
                .Include(c => c.Country)
                .ToListAsync();
        }
    }
}