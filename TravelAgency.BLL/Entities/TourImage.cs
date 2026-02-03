namespace TravelAgency.BLL.Entities
{
    public class TourImage
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int Order { get; set; }
        public string? AltText { get; set; }
        
        public virtual Tour Tour { get; set; } = null!;
    }
}