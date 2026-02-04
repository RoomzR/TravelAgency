using TravelAgency.BLL.DTOs;

namespace TravelAgency.Web.Models.ViewModels
{
    public class TourIndexViewModel
    {
        public List<TourDTO> Tours { get; set; } = new();
        public List<CountryDTO> Countries { get; set; } = new();
        public List<TourTypeDTO> TourTypes { get; set; } = new();
        public string? SearchTerm { get; set; }
        public int? SelectedCountryId { get; set; }
        public int? SelectedTypeId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}