using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAgency.DAL;
using TravelAgency.BLL.Entities;
using TravelAgency.Web.Models;
using TravelAgency.DAL.Data;

namespace TravelAgency.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var featuredTours = _context.Tours
                .Where(t => t.IsActive)
                .Include(t => t.Country)
                .Include(t => t.City)
                .Include(t => t.HotelCategory)
                .Take(6)
                .ToList();
                
            return View(featuredTours);
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