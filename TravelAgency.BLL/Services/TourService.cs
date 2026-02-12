using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAgency.BLL.DTOs;
using TravelAgency.DAL.Entities;
using TravelAgency.BLL.Interfaces;
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
                await EnrichTourDto(tour, dto);
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
            return await MapAndEnrichTours(tours);
        }

        public async Task<IEnumerable<TourDTO>> GetHotDealsAsync(int count = 4)
        {
            var tours = await _tourRepository.GetHotDealsAsync(count);
            return await MapAndEnrichTours(tours);
        }

        public async Task<IEnumerable<TourDTO>> GetPopularToursAsync(int count = 6)
        {
            var tours = await _tourRepository.GetPopularToursAsync(count);
            return await MapAndEnrichTours(tours);
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

                return await MapAndEnrichTours(tours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tours");
                return new List<TourDTO>();
            }
        }

        public async Task<TourDTO> CreateTourAsync(TourCreateDTO createDto, string createdById)
        {
            try
            {
                var tour = _mapper.Map<Tour>(createDto);
                tour.CreatedById = createdById;
                tour.CreatedDate = DateTime.UtcNow;
                tour.ViewsCount = 0;
                tour.BookingsCount = 0;
                tour.IsActive = true;

                await _tourRepository.CreateAsync(tour);

                var dto = _mapper.Map<TourDTO>(tour);
                await EnrichTourDto(tour, dto);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tour");
                throw;
            }
        }

        public async Task<TourDTO?> UpdateTourAsync(TourUpdateDTO updateDto)
        {
            try
            {
                var tour = await _tourRepository.GetByIdAsync(updateDto.Id);
                if (tour == null) return null;

                _mapper.Map(updateDto, tour);
                await _tourRepository.UpdateAsync(tour);

                var dto = _mapper.Map<TourDTO>(tour);
                await EnrichTourDto(tour, dto);
                return dto;
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
                    return true;
                }

                await _tourRepository.DeleteAsync(id);
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
            try
            {
                var tour = await _tourRepository.GetByIdAsync(id);
                if (tour == null) return false;

                tour.IsActive = isActive;
                await _tourRepository.UpdateAsync(tour);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling tour status for ID: {TourId}", id);
                throw;
            }
        }

        public async Task IncrementTourViewsAsync(int id)
        {
            try
            {
                await _tourRepository.IncrementViewsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing views for tour ID: {TourId}", id);
            }
        }

        private async Task<IEnumerable<TourDTO>> MapAndEnrichTours(IEnumerable<Tour> tours)
        {
            var tourDtos = new List<TourDTO>();

            foreach (var tour in tours)
            {
                var dto = _mapper.Map<TourDTO>(tour);
                await EnrichTourDto(tour, dto);
                tourDtos.Add(dto);
            }

            return tourDtos;
        }

        private async Task EnrichTourDto(Tour tour, TourDTO dto)
        {
            dto.CountryName = tour.Country?.Name ?? "Не указано";
            dto.CityName = tour.City?.Name ?? "Не указано";
            dto.HotelCategoryName = tour.HotelCategory?.Name ?? "Не указано";
            dto.TourTypeName = tour.TourType?.Name ?? "Не указано";
            dto.HotelName = tour.Hotel?.Name;

            var bookedPlaces = await _bookingRepository.GetBookedPlacesAsync(tour.Id);
            dto.AvailablePlaces = tour.MaxPeopleCount - bookedPlaces;
        }
    }
}