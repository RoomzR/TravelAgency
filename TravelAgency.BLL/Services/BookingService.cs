using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAgency.BLL.DTOs;
using TravelAgency.DAL.Entities;
using TravelAgency.BLL.Interfaces;
using TravelAgency.DAL.Enums;   
using TravelAgency.DAL.Interfaces;

namespace TravelAgency.BLL.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IPromoCodeRepository _promoCodeRepository;
        private readonly ILogger<BookingService> _logger;
        private readonly IMapper _mapper;

        public BookingService(
            IBookingRepository bookingRepository,
            ITourRepository tourRepository,
            IPromoCodeRepository promoCodeRepository,
            ILogger<BookingService> logger,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _tourRepository = tourRepository;
            _promoCodeRepository = promoCodeRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<BookingDTO?> GetBookingByIdAsync(int id)
        {
            try
            {
                var booking = await _bookingRepository.GetFullBookingAsync(id);
                return booking == null ? null : _mapper.Map<BookingDTO>(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking with ID: {BookingId}", id);
                throw;
            }
        }
        public async Task<IEnumerable<BookingDTO>> GetUserBookingsAsync(string userId)
        {
            try
            {
                var bookings = await _bookingRepository.GetUserBookingsAsync(userId);
                return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookings for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<BookingDTO>> GetPendingBookingsAsync()
        {
            try
            {
                var bookings = await _bookingRepository.GetPendingBookingsAsync();
                return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending bookings");
                throw;
            }
        }

        public async Task<BookingDTO> CreateBookingAsync(BookingCreateDTO createDto, string clientId)
        {
            try
            {
                var tour = await _tourRepository.GetByIdAsync(createDto.TourId);
                if (tour == null)
                    throw new ArgumentException("Tour not found");

                var isAvailable = await CheckTourAvailabilityAsync(createDto.TourId, createDto.PeopleCount);
                if (!isAvailable)
                    throw new InvalidOperationException("Not enough available places");

                var hasBooking = await _bookingRepository.HasActiveBookingAsync(createDto.TourId, clientId);
                if (hasBooking)
                    throw new InvalidOperationException("You already have an active booking for this tour");

                var totalPrice = await CalculateBookingPriceAsync(createDto.TourId, createDto.PeopleCount, createDto.PromoCode);

                var booking = new Booking
                {
                    TourId = createDto.TourId,
                    ClientId = clientId,
                    PeopleCount = createDto.PeopleCount,
                    TotalPrice = totalPrice,
                    Status = BookingStatus.Pending,
                    BookingDate = DateTime.UtcNow,
                    Comments = createDto.Comments
                };

                if (!string.IsNullOrEmpty(createDto.PromoCode))
                {
                    var promoCode = await _promoCodeRepository.GetByCodeAsync(createDto.PromoCode);
                    if (promoCode != null && await _promoCodeRepository.IsCodeValidAsync(createDto.PromoCode))
                    {
                        booking.PromoCodeId = promoCode.Id;
                        booking.DiscountAmount = totalPrice * (promoCode.DiscountPercent / 100);

                        if (promoCode.MaxdiscountAmount.HasValue &&
                            booking.DiscountAmount > promoCode.MaxdiscountAmount.Value)
                        {
                            booking.DiscountAmount = promoCode.MaxdiscountAmount.Value;
                        }
                    }
                }

                await _bookingRepository.CreateAsync(booking);

                if (booking.PromoCodeId.HasValue)
                {
                    await _promoCodeRepository.IncrementUsesAsync(booking.PromoCodeId.Value);
                }

                tour.BookingsCount++;
                await _tourRepository.UpdateAsync(tour);

                return _mapper.Map<BookingDTO>(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking for user: {UserId}", clientId);
                throw;
            }
        }

        public async Task<BookingDTO?> UpdateBookingStatusAsync(BookingUpdateDTO updateDto)
        {
            try
            {
                var booking = await _bookingRepository.GetFullBookingAsync(updateDto.Id);
                if (booking == null) return null;

                booking.ManagerConfirmedId = updateDto.ManagerId;

                if (Enum.TryParse<BookingStatus>(updateDto.Status, out var newStatus))
                {
                    booking.Status = newStatus;
                }

                await _bookingRepository.UpdateAsync(booking);
                return _mapper.Map<BookingDTO>(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                throw;
            }
        }

        public async Task<bool> CheckTourAvailabilityAsync(int tourId, int peopleCount)
        {
            try
            {
                var tour = await _tourRepository.GetByIdAsync(tourId);
                if (tour == null) return false;

                var bookedPlaces = await _bookingRepository.GetBookedPlacesAsync(tourId);
                var availablePlaces = tour.MaxPeopleCount - bookedPlaces;

                return availablePlaces >= peopleCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking tour availability: {TourId}", tourId);
                throw;
            }
        }

        public async Task<decimal> CalculateBookingPriceAsync(int tourId, int peopleCount, string? promoCode = null)
        {
            try
            {
                var tour = await _tourRepository.GetByIdAsync(tourId);
                if (tour == null)
                    throw new ArgumentException("Tour not found");

                var price = tour.DiscountedPrice * peopleCount;

                if (!string.IsNullOrEmpty(promoCode))
                {
                    var promo = await _promoCodeRepository.GetByCodeAsync(promoCode);
                    if (promo != null && await _promoCodeRepository.IsCodeValidAsync(promoCode))
                    {
                        var discount = price * (promo.DiscountPercent / 100);

                        if (promo.MaxdiscountAmount.HasValue && discount > promo.MaxdiscountAmount.Value)
                        {
                            discount = promo.MaxdiscountAmount.Value;
                        }

                        price -= discount;
                    }
                }

                return price;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating booking price for tour: {TourId}", tourId);
                throw;
            }
        }

        public async Task<bool> CancelBookingAsync(int id, string userId)
        {
            try
            {
                var booking = await _bookingRepository.GetFullBookingAsync(id);
                if (booking == null || booking.ClientId != userId)
                    return false;

                if (booking.Status == BookingStatus.Cancelled)
                    return true;

                if (booking.Status != BookingStatus.Pending &&
                    booking.Status != BookingStatus.Confirmed)
                    return false;

                booking.Status = BookingStatus.Cancelled;
                await _bookingRepository.UpdateAsync(booking);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking: {BookingId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByTourAsync(int tourId)
        {
            try
            {
                var bookings = await _bookingRepository.GetBookingsByTourAsync(tourId);
                return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookings for tour: {TourId}", tourId);
                throw;
            }
        }
        public async Task<IEnumerable<BookingDTO>> GetAllBookingsAsync()
        {
            try
            {
                var bookings = await _bookingRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all bookings for manager panel");
                throw;
            }
        }

    }
}