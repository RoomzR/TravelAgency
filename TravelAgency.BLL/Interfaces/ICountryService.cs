using TravelAgency.BLL.DTOs;

namespace TravelAgency.BLL.Interfaces
{
    public interface ICountryService
    {
        Task<IEnumerable<CountryDTO>> GetAllCountriesAsync();
        Task<CountryDTO?> GetCountryByIdAsync(int id);
        Task<IEnumerable<CountryDTO>> GetPopularCountriesAsync(int count = 6);
        Task<CountryDTO> CreateCountryAsync(CountryCreateDTO createDto);
        Task<CountryDTO?> UpdateCountryAsync(CountryUpdateDTO updateDto);
        Task<bool> DeleteCountryAsync(int id);
        Task<IEnumerable<CityDTO>> GetCitiesByCountryAsync(int countryId);
        Task<IEnumerable<HotelDTO>> GetHotelsByCityAsync(int cityId);
    }
}