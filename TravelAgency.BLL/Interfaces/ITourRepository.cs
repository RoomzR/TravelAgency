using TravelAgency.BLL.DTOs;
using TravelAgency.BLL.Entities;

namespace TravelAgency.BLL.Interfaces
{
    public interface ITourRepository : IRepository<Tour>
    {
        Task<IEnumerable<Tour>> GetActiveToursAsync();
        Task<IEnumerable<Tour>> GetHotDealsAsync(int count);
        Task<IEnumerable<Tour>> GetPopularToursAsync(int count);
        Task IncrementViewsAsync(int tourId);
        Task<IEnumerable<Tour>> GetToursByCountryAsync(int countryId);
        Task<IEnumerable<Tour>> GetToursByTypeAsync(int typeId);
        Task<IEnumerable<Tour>> SearchToursAsync(TourSearchDTO searchDto);
    }
}