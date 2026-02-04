namespace TravelAgency.BLL.DTOs
{
    public class TourImageDTO
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int Order { get; set; }
        public string? AltText { get; set; }
    }
}