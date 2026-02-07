using Microsoft.AspNetCore.Mvc;
using TravelAgency.BLL.DTOs;
using TravelAgency.BLL.Interfaces;
using TravelAgency.Web.Models.ViewModels;

namespace TravelAgency.Web.Controllers
{
    public class TourController : Controller
    {
        private readonly ITourService _tourService;
        private readonly ICountryService _countryService;
        private readonly ITourTypeService _tourTypeService;
        private readonly ILogger<TourController> _logger;

        public TourController(
            ITourService tourService,
            ICountryService countryService,
            ITourTypeService tourTypeService,
            ILogger<TourController> logger)
        {
            _tourService = tourService;
            _countryService = countryService;
            _tourTypeService = tourTypeService;
            _logger = logger;
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
    }
}