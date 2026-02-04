namespace TravelAgency.BLL.DTOs
{
    public class TourDTO
    {
        public int Id { get; set; }
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
        public bool IsActive { get; set; }
        public bool IsHotDeal { get; set; }
        public int MaxPeopleCount { get; set; }
        public DateTime StartDate { get; set; }
        public string? ImageUrlsJson { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedById { get; set; }
        public int ViewsCount { get; set; }
        public int BookingsCount { get; set; }
        public string? CountryName { get; set; }
        public string? CityName { get; set; }
        public string? HotelCategoryName { get; set; }
        public string? TourTypeName { get; set; }
        public string? HotelName { get; set; }
        public int? AvailablePlaces { get; set; }
        public double? AverageRating { get; set; }
        public decimal? DiscountedPrice { get; set; }
    }

    public class TourCreateDTO
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
        public string? ImageUrlsJson { get; set; }
    }

    public class TourUpdateDTO : TourCreateDTO
    {
        public int Id { get; set; }
    }
}