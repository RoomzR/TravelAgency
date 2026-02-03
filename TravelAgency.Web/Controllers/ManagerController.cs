using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAgency.DAL;
using TravelAgency.BLL.Entities;
using TravelAgency.DAL.Data;

namespace TravelAgency.Controllers
{
    [Authorize(Roles = "Manager,Admin")]
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Tours()
        {
            var tours = await _context.Tours
                .Include(t => t.Country)
                .Include(t => t.City)
                .Include(t => t.HotelCategory)
                .Include(t => t.TourType)
                .ToListAsync();
            
            return View(tours);
        }

        public IActionResult CreateTour()
        {
            ViewBag.Countries = _context.Countries.ToList();
            ViewBag.Cities = _context.Cities.ToList();
            ViewBag.HotelCategories = _context.HotelCategories.ToList();
            ViewBag.TourTypes = _context.TourTypes.ToList();
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTour(Tour tour)
        {
            if (ModelState.IsValid)
            {
                tour.CreatedById = User.Identity.Name;
                tour.CreatedDate = DateTime.UtcNow;
                
                _context.Add(tour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Tours));
            }
            
            ViewBag.Countries = _context.Countries.ToList();
            ViewBag.Cities = _context.Cities.ToList();
            ViewBag.HotelCategories = _context.HotelCategories.ToList();
            ViewBag.TourTypes = _context.TourTypes.ToList();
            
            return View(tour);
        }
    }
}