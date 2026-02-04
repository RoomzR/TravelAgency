namespace TravelAgency.BLL.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsApproved { get; set; }

        public string? ClientName { get; set; }
        public string? TourTitle { get; set; }
    }

    public class ReviewCreateDTO
    {
        public int TourId { get; set; }
        public int Rating { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}