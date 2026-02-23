using TravelAgency.BLL.DTOs;

namespace TravelAgency.BLL.Interfaces
{
    public interface ITourService
    {
        Task<TourDTO?> GetTourByIdAsync(int id);
        Task<IEnumerable<TourDTO>> GetActiveToursAsync();
        Task<IEnumerable<TourDTO>> GetHotDealsAsync(int count = 4);
        Task<IEnumerable<TourDTO>> GetPopularToursAsync(int count = 6);
        Task<IEnumerable<TourDTO>> SearchToursAsync(TourSearchDTO searchDto);
        Task<TourDTO> CreateTourAsync(TourCreateDTO createDto, string createdById);
        Task<TourDTO?> UpdateTourAsync(TourUpdateDTO updateDto);
        Task<bool> DeleteTourAsync(int id);
        Task<bool> ToggleTourStatusAsync(int id, bool isActive);
        Task IncrementTourViewsAsync(int id);
        Task<IEnumerable<HotelDTO>> GetAllHotelsAsync();
        Task<IEnumerable<HotelDTO>> GetHotelsByCountryAsync(int? countryId);
        Task<IEnumerable<CityDTO>> GetCitiesByCountryAsync(int countryId);
    }
}