using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TravelAgency.BLL.DTOs;
using TravelAgency.BLL.DTOs;
using TravelAgency.BLL.Interfaces;
using TravelAgency.DAL.Entities;
using TravelAgency.Web.Models.ViewModels;

namespace TravelAgency.Web.Controllers
{
    public class TourController : Controller
    {
        private readonly ITourService _tourService;
        private readonly ICountryService _countryService;
        private readonly ITourTypeService _tourTypeService;
        private readonly ILogger<TourController> _logger;
        private readonly UserManager<ApplicationUser> UserManager;

        public TourController(
            ITourService tourService,
            ICountryService countryService,
            ITourTypeService tourTypeService,
            ILogger<TourController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _tourService = tourService;
            _countryService = countryService;
            _tourTypeService = tourTypeService;
            _logger = logger;
            UserManager = userManager;
        }

        public async Task<IActionResult> Index(
            string? searchTerm = null,
            int? countryId = null,
            int? typeId = null)
        {
            try
            {
                var searchDto = new TourSearchDTO
                {
                    SearchTerm = searchTerm,
                    CountryId = countryId,
                    TourTypeId = typeId
                };

                var tours = await _tourService.SearchToursAsync(searchDto);

                var countries = await _countryService.GetAllCountriesAsync();
                var tourTypes = await _tourTypeService.GetAllTourTypesAsync();

                var viewModel = new TourIndexViewModel
                {
                    Tours = tours.ToList(),
                    Countries = countries.ToList(),
                    TourTypes = tourTypes.ToList(),
                    SearchTerm = searchTerm,
                    SelectedCountryId = countryId,
                    SelectedTypeId = typeId
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке туров");
                TempData["ErrorMessage"] = "Произошла ошибка при загрузке туров";
                return View(new TourIndexViewModel());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var tour = await _tourService.GetTourByIdAsync(id);

                if (tour == null)
                {
                    TempData["ErrorMessage"] = "Тур не найден";
                    return RedirectToAction("Index");
                }

                await _tourService.IncrementTourViewsAsync(id);

                return View(tour);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке тура {TourId}", id);
                TempData["ErrorMessage"] = "Произошла ошибка при загрузке тура";
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        [Authorize(Roles = "Admin, Director, Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var tour = await _tourService.GetTourByIdAsync(id);

            if (tour == null)
            {
                return NotFound(); 
            }

            var updateDto = new TourUpdateDTO
            {
                Id = tour.Id,
                Title = tour.Title,
                Description = tour.Description,
                CountryId = tour.CountryId,
                CityId = tour.CityId,
                HotelCategoryId = tour.HotelCategoryId,
                TourTypeId = tour.TourTypeId,
                HotelId = tour.HotelId,
                DurationDays = tour.DurationDays,
                Price = tour.Price,
                OriginalPrice = tour.OriginalPrice,
                DiscountPercent = tour.DiscountPercent,
                IsActive = tour.IsActive,
                IsHotDeal = tour.IsHotDeal,
                MaxPeopleCount = tour.MaxPeopleCount,
                StartDate = tour.StartDate,
                ImageUrlsJson = tour.ImageUrlsJson
            };

            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Director, Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            { 
                var deleted = await _tourService.DeleteTourAsync(id);

                if (deleted)
                {
                    TempData["SuccessMessage"] = "Тур успешно удален из системы.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Не удалось найти тур для удаления.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при удалении: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        [Authorize(Roles = "Admin, Director, Manager")]
        public async Task<IActionResult> Create()
        {
            var countries = await _countryService.GetAllCountriesAsync();
            var tourTypes = await _tourTypeService.GetAllTourTypesAsync();

            ViewBag.Countries = countries;
            ViewBag.TourTypes = tourTypes;

            return View(new TourCreateDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Director, Manager")]
        public async Task<IActionResult> Create(TourCreateDTO model, string imageUrl) 
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                model.ImageUrlsJson = $"[\"{imageUrl}\"]";
            }
            else
            {
                model.ImageUrlsJson = "[\"/img/tours/default-tour.jpg\"]";
            }

            if (model.CityId == 0) model.CityId = 1;
            if (model.HotelId == 0) model.HotelId = 1;
            model.IsActive = true;

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = UserManager.GetUserId(User);
                    await _tourService.CreateTourAsync(model, userId);

                    TempData["SuccessMessage"] = "Тур успешно создан!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    var innerMessage = ex.InnerException?.Message ?? ex.Message;
                    _logger.LogError(ex, "Ошибка при сохранении: {Msg}", innerMessage);
                    ModelState.AddModelError("", "Ошибка базы данных: " + innerMessage);
                }
            }

            ViewBag.Countries = await _countryService.GetAllCountriesAsync();
            ViewBag.TourTypes = await _tourTypeService.GetAllTourTypesAsync();

            return View(model);
        }
    }
}