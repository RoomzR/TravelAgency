using TravelAgency.DAL.Entities;

namespace TravelAgency.DAL.Interfaces
{
    public interface ITourRepository : IRepository<Tour>
    {
        Task<IEnumerable<Tour>> GetActiveToursAsync();
        Task<IEnumerable<Tour>> GetHotDealsAsync(int count);
        Task<IEnumerable<Tour>> GetPopularToursAsync(int count);
        Task IncrementViewsAsync(int tourId);
        Task<IEnumerable<Tour>> GetToursByCountryAsync(int countryId);
        Task<IEnumerable<Tour>> GetToursByTypeAsync(int typeId);

        Task<IEnumerable<Tour>> SearchToursAsync(
            string? searchTerm = null,
            int? countryId = null,
            int? tourTypeId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? minDuration = null,
            int? maxDuration = null,
            bool? isHotDeal = null,
            DateTime? startDateFrom = null,
            DateTime? startDateTo = null);

        Task<IEnumerable<Tour>> SearchToursAdvancedAsync(
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
            int pageSize = 10);

        Task<IEnumerable<Hotel>> GetAllHotelsAsync();
        Task<IEnumerable<Hotel>> GetHotelsByCountryAsync(int? countryId);
        Task<Hotel?> GetHotelByIdAsync(int id);

        Task<IEnumerable<City>> GetCitiesByCountryAsync(int countryId);
    }
}