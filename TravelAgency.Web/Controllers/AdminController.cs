using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelAgency.BLL.Interfaces;
using TravelAgency.BLL.DTOs;

namespace TravelAgency.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly ITourService _tourService;
        private readonly IBookingService _bookingService;

        public AdminController(
            IAdminService adminService,
            ITourService tourService,
            IBookingService bookingService)
        {
            _adminService = adminService;
            _tourService = tourService;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Users()
        {
            var users = await _adminService.GetUsersAsync(); 
            return View(users);
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _adminService.GetDashboardDataAsync();
            return View(stats);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(string userId, string role)
        {
            await _adminService.UpdateUserRoleAsync(userId, role);
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _adminService.DeleteUserAsync(userId);
            if (!result) TempData["Error"] = "Не удалось удалить пользователя";
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> Bookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBookingStatus(int id, string status)
        {
            var updateDto = new BookingUpdateDTO { Id = id, Status = status };
            await _bookingService.UpdateBookingStatusAsync(updateDto);
            return RedirectToAction(nameof(Bookings));
        }


        public async Task<IActionResult> Tours()
        {
            var tours = await _tourService.GetActiveToursAsync();
            return View(tours);
        }

        [HttpGet]
        public IActionResult CreateTour() => View();

        [HttpPost]
        public async Task<IActionResult> CreateTour(TourCreateDTO dto)
        {
            if (ModelState.IsValid)
            {
                var adminId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                await _tourService.CreateTourAsync(dto, adminId);
                return RedirectToAction(nameof(Tours));
            }
            return View(dto);
        }
        [HttpPost]
        public async Task<IActionResult> BlockUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return NotFound();

            var result = await _adminService.BlockUserAsync(userId);

            if (result)
            {
                TempData["Success"] = "Статус пользователя изменен";
            }
            else
            {
                TempData["Error"] = "Не удалось выполнить блокировку";
            }

            return RedirectToAction(nameof(Users));
        }
        [HttpPost]
        public async Task<IActionResult> UnblockUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return NotFound();

            var result = await _adminService.UnblockUserAsync(userId);

            if (result) TempData["Success"] = "Доступ пользователя восстановлен";
            else TempData["Error"] = "Ошибка при разблокировке";

            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> Reviews()
        {
            var reviews = await _adminService.GetAllReviewsAsync();
            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleApprove(int id)
        {
            await _adminService.ToggleReviewApprovalAsync(id);
            return RedirectToAction(nameof(Reviews));
        }

        public async Task<IActionResult> Promocodes()
        {
            var promos = await _adminService.GetAllPromoCodesAsync();
            return View(promos);
        }

        [HttpPost]
        public async Task<IActionResult> TogglePromoStatus(int id)
        {
            await _adminService.TogglePromoCodeStatusAsync(id);
            return RedirectToAction(nameof(Promocodes));
        }
        [HttpPost]
        public async Task<IActionResult> CreatePromoCode(PromoCodeCreateDTO dto)
        {
            if (ModelState.IsValid)
            {
                dto.ValidFrom = DateTime.Now;
                await _adminService.CreatePromoCodeAsync(dto);
                return RedirectToAction(nameof(Promocodes));
            }
            return RedirectToAction(nameof(Promocodes));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePromo(int id)
        {
            await _adminService.DeletePromoCodeAsync(id);
            return RedirectToAction(nameof(Promocodes));
        }

    }
}