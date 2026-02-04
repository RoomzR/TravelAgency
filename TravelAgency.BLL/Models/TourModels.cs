namespace TravelAgency.BLL.Models
{
    public class SearchTourModel
    {
        public string? SearchTerm { get; set; }
        public int? CountryId { get; set; }
        public int? TypeId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public int? Duration { get; set; }
    }

    public class CreateTourModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public int HotelCategoryId { get; set; }
        public int TourTypeId { get; set; }
        public int? HotelId { get; set; }
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsHotDeal { get; set; }
        public int MaxPeopleCount { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class UpdateTourModel : CreateTourModel
    {
    }
}