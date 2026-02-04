using TravelAgency.BLL.Entities;

namespace TravelAgency.BLL.Interfaces
{
    public interface ICityRepository : IRepository<City>
    {
        Task<IEnumerable<City>> GetCitiesByCountryAsync(int countryId);
    }
}