using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAgency.DAL;
using TravelAgency.BLL.Entities;
using TravelAgency.BLL.Enums; 
using TravelAgency.Web.Models.ViewModels;
using TravelAgency.DAL.Data;

namespace TravelAgency.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int tourId)
        {
            var tour = await _context.Tours
                .Include(t => t.Bookings)
                .FirstOrDefaultAsync(t => t.Id == tourId);

            if (tour == null)
            {
                return NotFound();
            }

            // Используйте BookingStatus напрямую, так как есть using TravelAgency.Core.Enums
            var availablePlaces = tour.MaxPeopleCount - 
                tour.Bookings.Where(b => b.Status != BookingStatus.Cancelled) // Убрали Enums.
                    .Sum(b => b.PeopleCount);

            var model = new BookingViewModel
            {
                TourId = tour.Id,
                TourTitle = tour.Title,
                TourPrice = tour.Price,
                AvailablePlaces = availablePlaces
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tour = await _context.Tours
                    .Include(t => t.Bookings)
                    .FirstOrDefaultAsync(t => t.Id == model.TourId);

                if (tour == null)
                {
                    return NotFound();
                }

                var availablePlaces = tour.MaxPeopleCount - 
                    tour.Bookings.Where(b => b.Status != BookingStatus.Cancelled) // Убрали Enums.
                        .Sum(b => b.PeopleCount);

                if (model.PeopleCount > availablePlaces)
                {
                    ModelState.AddModelError("PeopleCount", $"Доступно только {availablePlaces} мест");
                    model.AvailablePlaces = availablePlaces;
                    model.TourTitle = tour.Title;
                    model.TourPrice = tour.Price;
                    return View(model);
                }

                var booking = new Booking
                {
                    TourId = model.TourId,
                    ClientId = User.Identity.Name ?? string.Empty, // Исправляем null warning
                    PeopleCount = model.PeopleCount,
                    TotalPrice = tour.Price * model.PeopleCount,
                    Status = BookingStatus.Pending, // Убрали Enums.
                    BookingDate = DateTime.UtcNow,
                    Comments = model.Comments
                };

                _context.Add(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction("MyBookings");
            }

            return View(model);
        }

        public async Task<IActionResult> MyBookings()
        {
            var userId = User.Identity?.Name; // Проверка на null
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var bookings = await _context.Bookings
                .Where(b => b.ClientId == userId)
                .Include(b => b.Tour)
                .ThenInclude(t => t.Country)
                .Include(b => b.Payment)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }
    }
}