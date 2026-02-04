namespace TravelAgency.BLL.DTOs
{
    public class TourSearchDTO
    {
        public string? SearchTerm { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public int? TourTypeId { get; set; }
        public int? HotelCategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public int? MinDuration { get; set; }
        public int? MaxDuration { get; set; }
        public bool? IsHotDeal { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDescending { get; set; } = true;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}