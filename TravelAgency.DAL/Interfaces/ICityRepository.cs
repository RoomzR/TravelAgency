using TravelAgency.DAL.Entities;

namespace TravelAgency.DAL.Interfaces
{
    public interface ICityRepository : IRepository<City>
    {
        Task<IEnumerable<City>> GetCitiesByCountryAsync(int countryId);
    }
}