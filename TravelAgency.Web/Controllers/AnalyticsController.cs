using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelAgency.BLL.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TravelAgency.Web.Controllers
{
    [Authorize(Roles ="Director")]
    public class AnalyticsController : Controller
    {
        private readonly IBookingService _bookingService;

        public AnalyticsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index(DateTime? dateFrom, DateTime? dateTo)
        {
            var stats = await _bookingService.GetDirectorAnalyticsAsync(dateFrom, dateTo);
            stats.DateFrom = dateFrom;
            stats.DateTo = dateTo;
            return View(stats);
        }
    }
}
