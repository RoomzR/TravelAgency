using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAgency.BLL.DTOs;
using TravelAgency.BLL.Interfaces;
using TravelAgency.DAL.Data;

namespace TravelAgency.Web.Controllers
{
    public class DebugTourController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITourService _tourService;
        private readonly ILogger<DebugTourController> _logger;

        public DebugTourController(
            ApplicationDbContext context,
            ITourService tourService,
            ILogger<DebugTourController> logger)
        {
            _context = context;
            _tourService = tourService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("=== DebugTourController.Index ===");

            try
            {
                // 1. Прямой запрос к базе
                _logger.LogInformation("1. Прямой запрос к базе:");
                var directTours = await _context.Tours
                    .Include(t => t.Country)
                    .Include(t => t.City)
                    .Include(t => t.HotelCategory)
                    .Include(t => t.TourType)
                    .Where(t => t.IsActive)
                    .Take(5)
                    .ToListAsync();

                _logger.LogInformation($"Прямой запрос: {directTours.Count} туров");
                foreach (var tour in directTours)
                {
                    _logger.LogInformation($"Тур: {tour.Id} - {tour.Title} - Active: {tour.IsActive}");
                }

                // 2. Используем TourService
                _logger.LogInformation("\n2. Используем TourService:");
                var searchDto = new TourSearchDTO();
                var serviceTours = await _tourService.SearchToursAsync(searchDto);
                _logger.LogInformation($"TourService: {serviceTours.Count()} туров");

                // 3. Проверяем маппинг
                _logger.LogInformation("\n3. Проверяем маппинг:");
                var mappedTours = new List<TourDTO>();
                foreach (var tour in directTours)
                {
                    var dto = new TourDTO
                    {
                        Id = tour.Id,
                        Title = tour.Title,
                        Description = tour.Description,
                        Price = tour.Price,
                        CountryName = tour.Country?.Name,
                        CityName = tour.City?.Name,
                        HotelCategoryName = tour.HotelCategory?.Name,
                        TourTypeName = tour.TourType?.Name,
                        DurationDays = tour.DurationDays,
                        StartDate = tour.StartDate,
                        IsActive = tour.IsActive,
                        IsHotDeal = tour.IsHotDeal,
                        DiscountPercent = tour.DiscountPercent,
                        OriginalPrice = tour.OriginalPrice,
                        MaxPeopleCount = tour.MaxPeopleCount,
                        AvailablePlaces = tour.MaxPeopleCount // временно
                    };
                    mappedTours.Add(dto);
                }

                ViewBag.DirectTours = directTours;
                ViewBag.ServiceTours = serviceTours;
                ViewBag.MappedTours = mappedTours;
                ViewBag.DirectCount = directTours.Count;
                ViewBag.ServiceCount = serviceTours.Count();
                ViewBag.MappedCount = mappedTours.Count;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в DebugTourController");
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // Простой метод для проверки
        public async Task<IActionResult> SimpleList()
        {
            var tours = await _context.Tours
                .Where(t => t.IsActive)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Price,
                    t.IsActive,
                    Country = t.Country.Name,
                    City = t.City.Name,
                    HotelCategory = t.HotelCategory.Name,
                    TourType = t.TourType.Name
                })
                .Take(20)
                .ToListAsync();

            return Json(new
            {
                Success = true,
                Count = tours.Count,
                Tours = tours
            });
        }
    }
}