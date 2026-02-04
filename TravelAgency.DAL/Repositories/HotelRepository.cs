using Microsoft.EntityFrameworkCore;
using TravelAgency.BLL.Entities;
using TravelAgency.BLL.Interfaces;
using TravelAgency.DAL.Data;

namespace TravelAgency.DAL.Repositories
{
    public class HotelRepository : BaseRepository<Hotel>, IHotelRepository
    {
        public HotelRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Hotel>> GetHotelsByCityAsync(int cityId)
        {
            return await _context.Hotels
                .Where(h => h.CityId == cityId)
                .Include(h => h.City)
                .ThenInclude(c => c.Country)
                .Include(h => h.HotelCategory)
                .ToListAsync();
        }
    }
}