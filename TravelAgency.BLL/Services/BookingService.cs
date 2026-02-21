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
                    throw new KeyNotFoundException("Тур не найден.");

                var hasBooking = await _bookingRepository.HasActiveBookingAsync(createDto.TourId, clientId);
                if (hasBooking)
                    throw new InvalidOperationException("У вас уже есть активное бронирование на этот тур.");

                decimal totalPrice = tour.DiscountedPrice * createDto.PeopleCount;
                decimal discountAmount = 0;
                int? appliedPromoId = null;

                if (!string.IsNullOrEmpty(createDto.PromoCode))
                {
                    var promo = await _promoCodeRepository.GetByCodeAsync(createDto.PromoCode);
                    if (promo != null && await _promoCodeRepository.IsCodeValidAsync(createDto.PromoCode))
                    {
                        appliedPromoId = promo.Id;
                        discountAmount = totalPrice * (promo.DiscountPercent / 100);

                        if (promo.MaxdiscountAmount.HasValue && discountAmount > promo.MaxdiscountAmount.Value)
                        {
                            discountAmount = promo.MaxdiscountAmount.Value;
                        }
                    }
                }

                var bookedPlaces = await _bookingRepository.GetBookedPlacesAsync(createDto.TourId);
                if (tour.MaxPeopleCount - bookedPlaces < createDto.PeopleCount)
                    throw new InvalidOperationException("Недостаточно свободных мест на выбранный тур.");

                var booking = new Booking
                {
                    TourId = createDto.TourId,
                    ClientId = clientId,
                    PeopleCount = createDto.PeopleCount,
                    TotalPrice = totalPrice - discountAmount, 
                    DiscountAmount = discountAmount,
                    PromoCodeId = appliedPromoId,
                    Status = BookingStatus.Pending,
                    BookingDate = DateTime.UtcNow,
                    Comments = createDto.Comments
                };

                await _bookingRepository.CreateAsync(booking);

                if (appliedPromoId.HasValue)
                {
                    await _promoCodeRepository.IncrementUsesAsync(appliedPromoId.Value);
                }

                tour.BookingsCount++;
                await _tourRepository.UpdateAsync(tour);

                _logger.LogInformation("Бронирование {BookingId} успешно создано для пользователя {UserId}", booking.Id, clientId);

                return _mapper.Map<BookingDTO>(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании бронирования для пользователя: {UserId}", clientId);
                throw; 
            }
        }

        public async Task<BookingDTO?> UpdateBookingStatusAsync(BookingUpdateDTO updateDto)
        {
            try
            {
                var booking = await _bookingRepository.GetFullBookingAsync(updateDto.Id);
                if (booking == null)
                {
                    _logger.LogWarning("Бронирование с ID {BookingId} не найдено для обновления", updateDto.Id);
                    return null;
                }

                booking.ManagerConfirmedId = updateDto.ManagerId;


                booking.ManagerComments = updateDto.ManagerComments;

                if (Enum.TryParse<BookingStatus>(updateDto.Status, out var newStatus))
                {
                    booking.Status = newStatus;
                }
                else
                {
                    _logger.LogWarning("Некорректный статус: {Status} для бронирования {BookingId}", updateDto.Status, updateDto.Id);
                }

                await _bookingRepository.UpdateAsync(booking);

                _logger.LogInformation("Менеджер {ManagerId} обновил бронирование {BookingId}. Новый статус: {Status}",
                    updateDto.ManagerId, updateDto.Id, booking.Status);

                return _mapper.Map<BookingDTO>(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении статуса бронирования {BookingId} менеджером {ManagerId}",
                    updateDto.Id, updateDto.ManagerId);
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

        public async Task<DirectorStatsDTO> GetDirectorAnalyticsAsync(DateTime? dateFrom, DateTime? dateTo)
        {
            try
            {
                var bookings = await _bookingRepository.GetAllAsync();

                if (dateFrom.HasValue)
                {
                    bookings = bookings.Where(b => b.BookingDate >= dateFrom.Value);
                }

                if (dateTo.HasValue)
                {
                    var endOfDay = dateTo.Value.Date.AddDays(1).AddTicks(-1);
                    bookings = bookings.Where(b => b.BookingDate <= endOfDay);
                }

                var stats = new DirectorStatsDTO
                {
                    DateFrom = dateFrom,
                    DateTo = dateTo,

                    TotalBookings = bookings.Count(),
                    ConfirmedBookings = bookings.Count(b => b.Status == BookingStatus.Confirmed),
                    CancelledBookings = bookings.Count(b => b.Status == BookingStatus.Cancelled),

                    TotalRevenue = bookings
                        .Where(b => b.Status == BookingStatus.Confirmed)
                        .Sum(b => b.TotalPrice - (b.DiscountAmount ?? 0)),

                    TotalUniqueClients = bookings
                        .Select(b => b.ClientId)
                        .Distinct()
                        .Count(),

                    TopTours = bookings
                        .Where(b => b.Tour != null)
                        .GroupBy(b => b.Tour.Title)
                        .OrderByDescending(g => g.Count())
                        .Take(5)
                        .ToDictionary(g => g.Key, g => g.Count()),

                    ManagerRatings = bookings
                        .Where(b => b.Status == BookingStatus.Confirmed && b.ManagerConfirmed != null)
                        .GroupBy(b => b.ManagerConfirmedId)
                        .Select(g => new ManagerRatingDTO
                        {
                            Name = $"{g.First().ManagerConfirmed.FirstName} {g.First().ManagerConfirmed.LastName}",
                            ConfirmedCount = g.Count(),
                            Revenue = g.Sum(b => b.TotalPrice - (b.DiscountAmount ?? 0))
                        })
                        .OrderByDescending(r => r.Revenue)
                        .ToList()
                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка генерации аналитики для директора");
                throw;
            }
        }

        public async Task<bool> IsTourBookedByUserAsync(int tourId, string userId)
        {
            return await _bookingRepository.ExistsActiveBookingAsync(tourId, userId);
        }
        public async Task UpdateClientCommentAsync(int bookingId, string comment, string userId)
        {
            try
            {
                var booking = await _bookingRepository.GetFullBookingAsync(bookingId);

                if (booking == null)
                {
                    throw new KeyNotFoundException($"Бронирование с ID {bookingId} не найдено.");
                }

                if (booking.ClientId != userId)
                {
                    _logger.LogWarning("Попытка несанкционированного изменения комментария! Пользователь {UserId} пытался изменить бронь {BookingId}, которая ему не принадлежит.", userId, bookingId);
                    throw new UnauthorizedAccessException("У вас нет прав для редактирования этого бронирования.");
                }

                booking.Comments = comment;

                await _bookingRepository.UpdateAsync(booking);
                _logger.LogInformation("Пользователь {UserId} обновил комментарий к бронированию {BookingId}.", userId, bookingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении комментария для бронирования {BookingId}", bookingId);
                throw;
            }
        }
    }
}