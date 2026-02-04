using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAgency.DAL.Data;
using TravelAgency.Web.Models.ViewModels;

namespace TravelAgency.Web.Controllers
{
    public class TourController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TourController> _logger;

        public TourController(
            ApplicationDbContext context,
            ILogger<TourController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            string? searchTerm = null,
            int? countryId = null,
            int? typeId = null)
        {
            try
            {
                Console.WriteLine("=== TOUR CONTROLLER START ===");

                // 1. Получаем туры ПРЯМО из базы
                var query = _context.Tours
                    .Include(t => t.Country)
                    .Include(t => t.City)
                    .Include(t => t.HotelCategory)
                    .Include(t => t.TourType)
                    .Where(t => t.IsActive) // ТОЛЬКО активные
                    .AsQueryable();

                Console.WriteLine($"Базовый запрос: IsActive фильтр применен");

                // 2. Применяем фильтры если есть
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(t =>
                        t.Title.ToLower().Contains(searchTerm) ||
                        t.Description.ToLower().Contains(searchTerm) ||
                        t.Country.Name.ToLower().Contains(searchTerm) ||
                        t.City.Name.ToLower().Contains(searchTerm));
                    Console.WriteLine($"Добавлен поиск по: {searchTerm}");
                }

                if (countryId.HasValue)
                {
                    query = query.Where(t => t.CountryId == countryId.Value);
                    Console.WriteLine($"Добавлен фильтр по стране ID: {countryId.Value}");
                }

                if (typeId.HasValue)
                {
                    query = query.Where(t => t.TourTypeId == typeId.Value);
                    Console.WriteLine($"Добавлен фильтр по типу ID: {typeId.Value}");
                }

                // 3. Выполняем запрос
                var tours = await query
                    .OrderByDescending(t => t.CreatedDate)
                    .Take(100)
                    .ToListAsync();

                Console.WriteLine($"Получено туров из базы: {tours.Count}");

                // 4. Создаем DTO вручную (без AutoMapper)
                var tourDtos = tours.Select(t => new TravelAgency.BLL.DTOs.TourDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Price = t.Price,
                    OriginalPrice = t.OriginalPrice,
                    DiscountPercent = t.DiscountPercent,
                    CountryName = t.Country?.Name ?? "Не указано",
                    CityName = t.City?.Name ?? "Не указано",
                    HotelCategoryName = t.HotelCategory?.Name ?? "Не указано",
                    TourTypeName = t.TourType?.Name ?? "Не указано",
                    DurationDays = t.DurationDays,
                    StartDate = t.StartDate,
                    IsActive = t.IsActive,
                    IsHotDeal = t.IsHotDeal,
                    MaxPeopleCount = t.MaxPeopleCount,
                    AvailablePlaces = t.MaxPeopleCount, // Временно
                    ViewsCount = t.ViewsCount,
                    BookingsCount = t.BookingsCount,
                    ImageUrlsJson = t.ImageUrlsJson
                }).ToList();

                Console.WriteLine($"Создано DTO: {tourDtos.Count}");

                // 5. Получаем страны и типы для фильтров
                var countries = await _context.Countries
                    .Select(c => new TravelAgency.BLL.DTOs.CountryDTO
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToListAsync();

                var tourTypes = await _context.TourTypes
                    .Select(t => new TravelAgency.BLL.DTOs.TourTypeDTO
                    {
                        Id = t.Id,
                        Name = t.Name
                    })
                    .ToListAsync();

                Console.WriteLine($"Стран: {countries.Count}, Типов: {tourTypes.Count}");

                // 6. Создаем ViewModel
                var viewModel = new TourIndexViewModel
                {
                    Tours = tourDtos,
                    Countries = countries,
                    TourTypes = tourTypes,
                    SearchTerm = searchTerm,
                    SelectedCountryId = countryId,
                    SelectedTypeId = typeId
                };

                Console.WriteLine("=== TOUR CONTROLLER END ===");

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА В TOUR CONTROLLER: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                // Возвращаем тестовые данные чтобы проверить View
                var testData = GetTestData();
                return View(testData);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                Console.WriteLine($"=== DETAILS для тура {id} ===");

                // Прямой запрос к базе
                var tour = await _context.Tours
                    .Include(t => t.Country)
                    .Include(t => t.City)
                    .Include(t => t.HotelCategory)
                    .Include(t => t.TourType)
                    .Include(t => t.Hotel)
                    .Include(t => t.TourImages)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tour == null)
                {
                    TempData["ErrorMessage"] = "Тур не найден";
                    return RedirectToAction("Index");
                }

                Console.WriteLine($"Найден тур: {tour.Title}");

                // Увеличиваем просмотры
                tour.ViewsCount++;
                await _context.SaveChangesAsync();

                // Создаем DTO вручную
                var tourDto = new TravelAgency.BLL.DTOs.TourDTO
                {
                    Id = tour.Id,
                    Title = tour.Title,
                    Description = tour.Description,
                    Price = tour.Price,
                    OriginalPrice = tour.OriginalPrice,
                    DiscountPercent = tour.DiscountPercent,
                    CountryName = tour.Country?.Name ?? "Не указано",
                    CityName = tour.City?.Name ?? "Не указано",
                    HotelCategoryName = tour.HotelCategory?.Name ?? "Не указано",
                    TourTypeName = tour.TourType?.Name ?? "Не указано",
                    HotelName = tour.Hotel?.Name,
                    DurationDays = tour.DurationDays,
                    StartDate = tour.StartDate,
                    IsActive = tour.IsActive,
                    IsHotDeal = tour.IsHotDeal,
                    MaxPeopleCount = tour.MaxPeopleCount,
                    AvailablePlaces = tour.MaxPeopleCount, // Временно
                    ViewsCount = tour.ViewsCount,
                    BookingsCount = tour.BookingsCount,
                    ImageUrlsJson = tour.ImageUrlsJson
                };

                return View(tourDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка Details: {ex.Message}");
                TempData["ErrorMessage"] = "Произошла ошибка при загрузке тура";
                return RedirectToAction("Index");
            }
        }

        // Метод для тестовых данных
        private TourIndexViewModel GetTestData()
        {
            return new TourIndexViewModel
            {
                Tours = new List<TravelAgency.BLL.DTOs.TourDTO>
                {
                    new TravelAgency.BLL.DTOs.TourDTO
                    {
                        Id = 1,
                        Title = "Тестовый тур 1 (ОШИБКА)",
                        Description = "Это тестовый тур, потому что произошла ошибка",
                        Price = 50000,
                        CountryName = "Тестовая страна",
                        CityName = "Тестовый город",
                        HotelCategoryName = "5 звезд",
                        TourTypeName = "Пляжный",
                        DurationDays = 7,
                        StartDate = DateTime.Now.AddDays(30),
                        IsActive = true,
                        IsHotDeal = true,
                        MaxPeopleCount = 20,
                        AvailablePlaces = 15,
                        DiscountPercent = 10,
                        OriginalPrice = 55000
                    },
                    new TravelAgency.BLL.DTOs.TourDTO
                    {
                        Id = 2,
                        Title = "Тестовый тур 2",
                        Description = "Еще один тестовый тур",
                        Price = 70000,
                        CountryName = "Египет",
                        CityName = "Хургада",
                        HotelCategoryName = "4 звезды",
                        TourTypeName = "Экскурсионный",
                        DurationDays = 10,
                        StartDate = DateTime.Now.AddDays(45),
                        IsActive = true,
                        IsHotDeal = false,
                        MaxPeopleCount = 15,
                        AvailablePlaces = 8
                    }
                },
                Countries = new List<TravelAgency.BLL.DTOs.CountryDTO>
                {
                    new TravelAgency.BLL.DTOs.CountryDTO { Id = 1, Name = "Турция" },
                    new TravelAgency.BLL.DTOs.CountryDTO { Id = 2, Name = "Египет" },
                    new TravelAgency.BLL.DTOs.CountryDTO { Id = 3, Name = "Испания" }
                },
                TourTypes = new List<TravelAgency.BLL.DTOs.TourTypeDTO>
                {
                    new TravelAgency.BLL.DTOs.TourTypeDTO { Id = 1, Name = "Пляжный" },
                    new TravelAgency.BLL.DTOs.TourTypeDTO { Id = 2, Name = "Экскурсионный" },
                    new TravelAgency.BLL.DTOs.TourTypeDTO { Id = 3, Name = "Горнолыжный" }
                }
            };
        }
    }
}