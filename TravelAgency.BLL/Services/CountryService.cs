using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAgency.BLL.DTOs;
using TravelAgency.BLL.Entities;
using TravelAgency.BLL.Interfaces;

namespace TravelAgency.BLL.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly ILogger<CountryService> _logger;
        private readonly IMapper _mapper;

        public CountryService(
            ICountryRepository countryRepository,
            ICityRepository cityRepository,
            IHotelRepository hotelRepository,
            ILogger<CountryService> logger,
            IMapper mapper)
        {
            _countryRepository = countryRepository;
            _cityRepository = cityRepository;
            _hotelRepository = hotelRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CountryDTO>> GetAllCountriesAsync()
        {
            try
            {
                var countries = await _countryRepository.GetAllAsync();
                var countryDtos = _mapper.Map<IEnumerable<CountryDTO>>(countries);

                // Можно добавить дополнительную логику если нужно
                return countryDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all countries");
                throw;
            }
        }

        public async Task<CountryDTO?> GetCountryByIdAsync(int id)
        {
            try
            {
                var country = await _countryRepository.GetByIdAsync(id);
                if (country == null) return null;

                return _mapper.Map<CountryDTO>(country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting country with ID: {CountryId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<CountryDTO>> GetPopularCountriesAsync(int count = 6)
        {
            try
            {
                var countries = await _countryRepository.GetAllAsync();
                // Берем первые count стран (в реальности нужно логику популярности)
                var popularCountries = countries.Take(count);

                return _mapper.Map<IEnumerable<CountryDTO>>(popularCountries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular countries");
                throw;
            }
        }

        public async Task<CountryDTO> CreateCountryAsync(CountryCreateDTO createDto)
        {
            try
            {
                var country = _mapper.Map<Country>(createDto);
                await _countryRepository.CreateAsync(country);

                return _mapper.Map<CountryDTO>(country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating country");
                throw;
            }
        }

        public async Task<CountryDTO?> UpdateCountryAsync(CountryUpdateDTO updateDto)
        {
            try
            {
                var country = await _countryRepository.GetByIdAsync(updateDto.Id);
                if (country == null) return null;

                _mapper.Map(updateDto, country);
                await _countryRepository.UpdateAsync(country);

                return _mapper.Map<CountryDTO>(country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating country with ID: {CountryId}", updateDto.Id);
                throw;
            }
        }

        public async Task<bool> DeleteCountryAsync(int id)
        {
            try
            {
                await _countryRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting country with ID: {CountryId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<CityDTO>> GetCitiesByCountryAsync(int countryId)
        {
            try
            {
                var cities = await _cityRepository.GetCitiesByCountryAsync(countryId);
                return _mapper.Map<IEnumerable<CityDTO>>(cities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cities for country: {CountryId}", countryId);
                throw;
            }
        }

        public async Task<IEnumerable<HotelDTO>> GetHotelsByCityAsync(int cityId)
        {
            try
            {
                var hotels = await _hotelRepository.GetHotelsByCityAsync(cityId);
                return _mapper.Map<IEnumerable<HotelDTO>>(hotels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting hotels for city: {CityId}", cityId);
                throw;
            }
        }
    }
}