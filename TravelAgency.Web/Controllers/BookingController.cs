using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TravelAgency.BLL.Interfaces;
using TravelAgency.BLL.DTOs;
using TravelAgency.DAL.Entities;

namespace TravelAgency.Web.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(IBookingService bookingService, UserManager<ApplicationUser> userManager)
        {
            _bookingService = bookingService;
            _userManager = userManager;
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

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Ошибка в данных бронирования. Проверьте количество человек.";
                return RedirectToAction("Details", "Tours", new { id = model.TourId });
            }

            try
            {
                var result = await _bookingService.CreateBookingAsync(model, userId);

                TempData["SuccessMessage"] = $"Тур '{result.TourTitle}' успешно забронирован!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Details", "Tours", new { id = model.TourId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Произошла внутренняя ошибка. Попробуйте позже.";
                return RedirectToAction("Details", "Tours", new { id = model.TourId });
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
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var updateDto = new BookingUpdateDTO
            {
                Id = id,
                Status = status,
                ManagerId = _userManager.GetUserId(User)
            };

            await _bookingService.UpdateBookingStatusAsync(updateDto);
            return RedirectToAction(nameof(ManagerPanel));
        }
        [HttpPost]
        [Authorize(Roles = "Manager, Admin, Director")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status, string managerComments)
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
    }
}
