using TravelAgency.BLL.Entities;

namespace TravelAgency.BLL.Interfaces
{
    public interface ICountryRepository : IRepository<Country>
    {
        Task<IEnumerable<Country>> GetPopularCountriesAsync(int count);
        Task<IEnumerable<Country>> GetAllWithCitiesAsync();
        Task<Country?> GetWithCitiesAsync(int id);
        Task<bool> HasToursAsync(int id);
    }

}