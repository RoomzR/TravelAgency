using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TravelAgency.BLL.DTOs;
using TravelAgency.BLL.Interfaces;
using TravelAgency.DAL.Entities;
using TravelAgency.DAL.Interfaces;

namespace TravelAgency.Web.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPromoCodeRepository _promoCodeRepository;

        public BookingController(IBookingService bookingService, UserManager<ApplicationUser> userManager, IPromoCodeRepository promoCodeRepository)
        {
            _bookingService = bookingService;
            _userManager = userManager;
            _promoCodeRepository = promoCodeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();
            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return View(bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var success = await _bookingService.CancelBookingAsync(id, userId);

            if (success)
            {
                TempData["SuccessMessage"] = "Бронирование успешно отменено.";
            }
            else
            {
                TempData["ErrorMessage"] = "Не удалось отменить бронирование. Возможно, оно уже подтверждено менеджером.";
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateDTO model)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var alreadyBooked = await _bookingService.IsTourBookedByUserAsync(model.TourId, userId);
            if (alreadyBooked)
            {
                TempData["ErrorMessage"] = "Вы уже забронировали этот тур. Проверьте личный кабинет.";
                return RedirectToAction("Details", "Tour", new { id = model.TourId });
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Ошибка в данных. Проверьте заполнение полей.";
                return RedirectToAction("Details", "Tour", new { id = model.TourId });
            }

            try
            {
               
                var result = await _bookingService.CreateBookingAsync(model, userId);
                TempData["SuccessMessage"] = $"Тур успешно забронирован!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Details", "Tour", new { id = model.TourId });
            }
        }
        [Authorize(Roles = "Admin, Director, Manager")]
        public async Task<IActionResult> ManagerPanel()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return View(bookings);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Director, Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status, string? managerComments = null)
        {
            var managerId = _userManager.GetUserId(User);

            var updateDto = new BookingUpdateDTO
            {
                Id = id,
                Status = status,
                ManagerComments = managerComments,
                ManagerId = managerId
            };

            var result = await _bookingService.UpdateBookingStatusAsync(updateDto);

            if (result != null)
                TempData["SuccessMessage"] = $"Статус брони #{id} обновлен на {status}";
            else
                TempData["ErrorMessage"] = "Ошибка при обновлении статуса";

            return RedirectToAction(nameof(ManagerPanel));
        }

        [HttpGet]
        public async Task<IActionResult> CheckPromo(string code, int tourId)
        {
            var promo = await _promoCodeRepository.GetByCodeAsync(code);
            var isValid = await _promoCodeRepository.IsCodeValidAsync(code);

            if (promo != null && isValid)
            {
                return Json(new
                {
                    success = true,
                    discount = promo.DiscountPercent,
                    message = $"Активирована скидка {promo.DiscountPercent}%"
                });
            }

            return Json(new { success = false, message = "Промокод недействителен" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateComments(int bookingId, string newComment)
        {
            if (string.IsNullOrWhiteSpace(newComment))
                return RedirectToAction(nameof(Index)); 

            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                    return Challenge();

                await _bookingService.UpdateClientCommentAsync(bookingId, newComment, userId);

                TempData["SuccessMessage"] = "Сообщение успешно отправлено!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка: " + ex.Message;
            }

            return RedirectToAction(nameof(Index)); 
        }
        public async Task<IActionResult> MyBookings()
        {
            var userId = _userManager.GetUserId(User);
            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return View(bookings);
        }
    }
}
