using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAgency.BLL.Entities;
using TravelAgency.DAL.Data;
using TravelAgency.Web.Models;
using TravelAgency.Web.Models.ViewModels;

namespace TravelAgency.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ApplicationDbContext context,
            ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var featuredTours = await _context.Tours
                .Where(t => t.IsActive && t.IsHotDeal)
                .Include(t => t.Country)
                .Include(t => t.City)
                .Include(t => t.HotelCategory)
                .Take(4)
                .ToListAsync();

            var popularTours = await _context.Tours
                .Where(t => t.IsActive)
                .OrderByDescending(t => t.ViewsCount)
                .Include(t => t.Country)
                .Include(t => t.City)
                .Take(6)
                .ToListAsync();

            var countries = await _context.Countries
                .Take(6)
                .ToListAsync();

            var news = await _context.NewsArticles
                .OrderByDescending(n => n.CreatedDate)
                .Take(3)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                FeaturedTours = featuredTours,
                PopularTours = popularTours,
                Countries = countries,
                NewsArticles = news,
                TourCount = await _context.Tours.CountAsync(t => t.IsActive),
                ClientCount = await _context.Users.CountAsync(),
                BookingCount = await _context.Bookings.CountAsync()
            };

            return View(viewModel);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}