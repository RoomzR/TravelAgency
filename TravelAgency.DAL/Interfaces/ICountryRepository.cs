using TravelAgency.DAL.Entities;

namespace TravelAgency.DAL.Interfaces
{
    public interface ICountryRepository : IRepository<Country>
    {
        Task<IEnumerable<Country>> GetPopularCountriesAsync(int count);
        Task<IEnumerable<Country>> GetAllWithCitiesAsync();
        Task<Country?> GetWithCitiesAsync(int id);
        Task<bool> HasToursAsync(int id);
    }

}