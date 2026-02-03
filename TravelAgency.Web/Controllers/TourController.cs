using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAgency.DAL;
using TravelAgency.BLL.Entities;
using TravelAgency.DAL.Data;

namespace TravelAgency.Controllers
{
    public class TourController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TourController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? country, string? type, decimal? minPrice, decimal? maxPrice)
        {
            var toursQuery = _context.Tours
                .Where(t => t.IsActive && t.StartDate > DateTime.Now)
                .Include(t => t.Country)
                .Include(t => t.City)
                .Include(t => t.HotelCategory)
                .Include(t => t.TourType)
                .AsQueryable();

            // Фильтрация
            if (!string.IsNullOrEmpty(country))
            {
                toursQuery = toursQuery.Where(t => t.Country.Name.Contains(country));
            }

            if (!string.IsNullOrEmpty(type))
            {
                toursQuery = toursQuery.Where(t => t.TourType.Name.Contains(type));
            }

            if (minPrice.HasValue)
            {
                toursQuery = toursQuery.Where(t => t.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                toursQuery = toursQuery.Where(t => t.Price <= maxPrice.Value);
            }

            var tours = await toursQuery.ToListAsync();
            
            ViewBag.Countries = await _context.Countries.ToListAsync();
            ViewBag.TourTypes = await _context.TourTypes.ToListAsync();
            
            return View(tours);
        }

        public async Task<IActionResult> Details(int id)
        {
            var tour = await _context.Tours
                .Include(t => t.Country)
                .Include(t => t.City)
                .Include(t => t.HotelCategory)
                .Include(t => t.TourType)
                .Include(t => t.Reviews.Where(r => r.IsApproved))
                .ThenInclude(r => r.Client)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }
    }
}