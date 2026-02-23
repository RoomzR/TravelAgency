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
                return _mapper.Map<BookingDTO?>(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking with ID: {BookingId}", id);
                throw;
            }
        }
        public async Task<decimal> CalculateBookingPriceAsync(int tourId, int peopleCount, string? promoCode = null)
        {
            try
            {
                var tour = await _tourRepository.GetByIdAsync(tourId);
                if (tour == null)
                    throw new ArgumentException("Тур не найден");

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
                _logger.LogError(ex, "Ошибка при расчете стоимости для тура: {TourId}", tourId);
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
                if (tour == null) throw new KeyNotFoundException("Тур не найден.");

                var hasBooking = await _bookingRepository.HasActiveBookingAsync(createDto.TourId, clientId);
                if (hasBooking) throw new InvalidOperationException("У вас уже есть активное бронирование на этот тур.");

                // Расчет цен
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
                            discountAmount = promo.MaxdiscountAmount.Value;
                    }
                }

                var bookedPlaces = await _bookingRepository.GetBookedPlacesAsync(createDto.TourId);
                if (tour.MaxPeopleCount - bookedPlaces < createDto.PeopleCount)
                    throw new InvalidOperationException("Недостаточно свободных мест.");

                // ИСПОЛЬЗУЕМ MAPPER вместо new Booking()
                var booking = _mapper.Map<Booking>(createDto);

                // Дозаполняем системные поля, которых нет в DTO
                booking.ClientId = clientId;
                booking.TotalPrice = totalPrice - discountAmount;
                booking.DiscountAmount = discountAmount;
                booking.PromoCodeId = appliedPromoId;
                booking.Status = BookingStatus.Pending;
                booking.BookingDate = DateTime.UtcNow;

                await _bookingRepository.CreateAsync(booking);

                // Обновление статистики
                if (appliedPromoId.HasValue) await _promoCodeRepository.IncrementUsesAsync(appliedPromoId.Value);
                tour.BookingsCount++;
                await _tourRepository.UpdateAsync(tour);

                _logger.LogInformation("Бронирование {BookingId} создано", booking.Id);
                return _mapper.Map<BookingDTO>(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании бронирования: {UserId}", clientId);
                throw;
            }
        }

        public async Task<BookingDTO?> UpdateBookingStatusAsync(BookingUpdateDTO updateDto)
        {
            try
            {
                var booking = await _bookingRepository.GetFullBookingAsync(updateDto.Id);
                if (booking == null) return null;

                // Используем Mapper для обновления существующего объекта из DTO
                _mapper.Map(updateDto, booking);

                // Дополнительная логика парсинга статуса, если в DTO он приходит строкой
                if (Enum.TryParse<BookingStatus>(updateDto.Status, out var newStatus))
                    booking.Status = newStatus;

                await _bookingRepository.UpdateAsync(booking);
                return _mapper.Map<BookingDTO>(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении бронирования {BookingId}", updateDto.Id);
                throw;
            }
        }

        public async Task<DirectorStatsDTO> GetDirectorAnalyticsAsync(DateTime? dateFrom, DateTime? dateTo)
        {
            var bookings = await _bookingRepository.GetAllAsync();

            if (dateFrom.HasValue) bookings = bookings.Where(b => b.BookingDate >= dateFrom.Value);
            if (dateTo.HasValue) bookings = bookings.Where(b => b.BookingDate <= dateTo.Value.Date.AddDays(1).AddTicks(-1));

            var confirmed = bookings.Where(b => b.Status == BookingStatus.Confirmed);

            return new DirectorStatsDTO
            {
                DateFrom = dateFrom,
                DateTo = dateTo,
                TotalBookings = bookings.Count(),
                ConfirmedBookings = confirmed.Count(),
                CancelledBookings = bookings.Count(b => b.Status == BookingStatus.Cancelled),
                TotalRevenue = confirmed.Sum(b => b.TotalPrice - (b.DiscountAmount ?? 0)),
                TotalUniqueClients = bookings.Select(b => b.ClientId).Distinct().Count(),

                TopTours = bookings.Where(b => b.Tour != null)
                    .GroupBy(b => b.Tour.Title)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .ToDictionary(g => g.Key, g => g.Count()),

                ManagerRatings = confirmed.Where(b => b.ManagerConfirmed != null)
                    .GroupBy(b => b.ManagerConfirmedId)
                    .Select(g => new ManagerRatingDTO
                    {
                        Name = $"{g.First().ManagerConfirmed.FirstName} {g.First().ManagerConfirmed.LastName}",
                        ConfirmedCount = g.Count(),
                        Revenue = g.Sum(b => b.TotalPrice - (b.DiscountAmount ?? 0))
                    })
                    .OrderByDescending(r => r.Revenue).ToList()
            };
        }

        // Остальные простые методы маппятся аналогично
        public async Task<IEnumerable<BookingDTO>> GetAllBookingsAsync() =>
            _mapper.Map<IEnumerable<BookingDTO>>(await _bookingRepository.GetAllAsync());

        public async Task<IEnumerable<BookingDTO>> GetBookingsByTourAsync(int tourId) =>
            _mapper.Map<IEnumerable<BookingDTO>>(await _bookingRepository.GetBookingsByTourAsync(tourId));

        public async Task<bool> IsTourBookedByUserAsync(int tourId, string userId) =>
            await _bookingRepository.ExistsActiveBookingAsync(tourId, userId);

        public async Task<bool> CheckTourAvailabilityAsync(int tourId, int peopleCount)
        {
            var tour = await _tourRepository.GetByIdAsync(tourId);
            if (tour == null) return false;
            return (tour.MaxPeopleCount - await _bookingRepository.GetBookedPlacesAsync(tourId)) >= peopleCount;
        }

        public async Task<bool> CancelBookingAsync(int id, string userId)
        {
            var booking = await _bookingRepository.GetFullBookingAsync(id);
            if (booking == null || booking.ClientId != userId) return false;
            if (booking.Status == BookingStatus.Cancelled) return true;

            booking.Status = BookingStatus.Cancelled;
            await _bookingRepository.UpdateAsync(booking);
            return true;
        }

        public async Task UpdateClientCommentAsync(int bookingId, string comment, string userId)
        {
            var booking = await _bookingRepository.GetFullBookingAsync(bookingId);
            if (booking == null) throw new KeyNotFoundException();
            if (booking.ClientId != userId) throw new UnauthorizedAccessException();

            booking.Comments = comment;
            await _bookingRepository.UpdateAsync(booking);
        }
    }
}