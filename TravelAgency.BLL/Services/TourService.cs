using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TravelAgency.BLL.DTOs;
using TravelAgency.BLL.Interfaces;
using TravelAgency.DAL.Entities;
using TravelAgency.DAL.Interfaces;

namespace TravelAgency.BLL.Services
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ILogger<TourService> _logger;
        private readonly IMapper _mapper;

        public TourService(
            ITourRepository tourRepository,
            IBookingRepository bookingRepository,
            ILogger<TourService> logger,
            IMapper mapper)
        {
            _tourRepository = tourRepository;
            _bookingRepository = bookingRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<TourDTO?> GetTourByIdAsync(int id)
        {
            try
            {
                var tour = await _tourRepository.GetByIdAsync(id);
                if (tour == null) return null;

                var dto = _mapper.Map<TourDTO>(tour);
                dto.AvailablePlaces = tour.MaxPeopleCount - await _bookingRepository.GetBookedPlacesAsync(id);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tour with ID: {TourId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<TourDTO>> GetActiveToursAsync()
        {
            var tours = await _tourRepository.GetActiveToursAsync();
            return await MapAndEnrichList(tours);
        }

        public async Task<IEnumerable<TourDTO>> GetHotDealsAsync(int count = 4)
        {
            var tours = await _tourRepository.GetHotDealsAsync(count);
            return await MapAndEnrichList(tours);
        }

        public async Task<IEnumerable<TourDTO>> GetPopularToursAsync(int count = 6)
        {
            var tours = await _tourRepository.GetPopularToursAsync(count);
            return await MapAndEnrichList(tours);
        }

        public async Task<IEnumerable<TourDTO>> SearchToursAsync(TourSearchDTO searchDto)
        {
            try
            {
                var tours = await _tourRepository.SearchToursAdvancedAsync(
                    searchTerm: searchDto.SearchTerm,
                    countryId: searchDto.CountryId,
                    tourTypeId: searchDto.TourTypeId,
                    minPrice: searchDto.MinPrice,
                    maxPrice: searchDto.MaxPrice,
                    minDuration: searchDto.MinDuration,
                    maxDuration: searchDto.MaxDuration,
                    isHotDeal: searchDto.IsHotDeal,
                    startDateFrom: searchDto.StartDateFrom,
                    startDateTo: searchDto.StartDateTo,
                    sortBy: searchDto.SortBy,
                    sortDescending: searchDto.SortDescending,
                    pageNumber: searchDto.PageNumber,
                    pageSize: searchDto.PageSize
                );

                return await MapAndEnrichList(tours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tours");
                return Enumerable.Empty<TourDTO>();
            }
        }

        public async Task<TourDTO> CreateTourAsync(TourCreateDTO createDto, string createdById)
        {
            var tour = _mapper.Map<Tour>(createDto);

            if (tour.HotelId.HasValue)
            {
                var hotel = await _tourRepository.GetHotelByIdAsync(tour.HotelId.Value);
                if (hotel != null)
                {
                    tour.CityId = hotel.CityId;
                }
            }


            tour.CreatedById = createdById;
            tour.CreatedDate = DateTime.UtcNow;
            tour.IsActive = true;

            await _tourRepository.CreateAsync(tour);
            return _mapper.Map<TourDTO>(tour);
        }

        public async Task<TourDTO?> UpdateTourAsync(TourUpdateDTO updateDto)
        {
            try
            {
                var tour = await _tourRepository.GetByIdAsync(updateDto.Id);
                if (tour == null) return null;

                _mapper.Map(updateDto, tour);
                await _tourRepository.UpdateAsync(tour);

                return _mapper.Map<TourDTO>(tour);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tour with ID: {TourId}", updateDto.Id);
                throw;
            }
        }

        public async Task<bool> DeleteTourAsync(int id)
        {
            try
            {
                var tour = await _tourRepository.GetByIdAsync(id);
                if (tour == null) return false;

                var bookedPlaces = await _bookingRepository.GetBookedPlacesAsync(id);
                if (bookedPlaces > 0)
                {
                    tour.IsActive = false;
                    await _tourRepository.UpdateAsync(tour);
                }
                else
                {
                    await _tourRepository.DeleteAsync(id);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tour with ID: {TourId}", id);
                throw;
            }
        }

        public async Task<bool> ToggleTourStatusAsync(int id, bool isActive)
        {
            var tour = await _tourRepository.GetByIdAsync(id);
            if (tour == null) return false;

            tour.IsActive = isActive;
            await _tourRepository.UpdateAsync(tour);
            return true;
        }

        public async Task IncrementTourViewsAsync(int id) =>
            await _tourRepository.IncrementViewsAsync(id);

        private async Task<IEnumerable<TourDTO>> MapAndEnrichList(IEnumerable<Tour> tours)
        {
            var dtos = _mapper.Map<IEnumerable<TourDTO>>(tours).ToList();

            foreach (var dto in dtos)
            {

            }

            return dtos;
        }

        public async Task<IEnumerable<HotelDTO>> GetAllHotelsAsync()
        {
            var hotels = await _tourRepository.GetAllHotelsAsync();
            return _mapper.Map<IEnumerable<HotelDTO>>(hotels);
        }

        public async Task<IEnumerable<HotelDTO>> GetHotelsByCountryAsync(int? countryId) 
        {
            try
            {
                
                var hotels = await _tourRepository.GetHotelsByCountryAsync(countryId);

                return _mapper.Map<IEnumerable<HotelDTO>>(hotels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении отелей для страны {CountryId}", countryId);
                return Enumerable.Empty<HotelDTO>();
            }
        }
        public async Task<IEnumerable<CityDTO>> GetCitiesByCountryAsync(int countryId)
        {
            var cities = await _tourRepository.GetCitiesByCountryAsync(countryId);
            return _mapper.Map<IEnumerable<CityDTO>>(cities);
        }
    }
}