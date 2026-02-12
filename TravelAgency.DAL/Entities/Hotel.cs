namespace TravelAgency.DAL.Entities
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int HotelCategoryId { get; set; }
        public int CityId { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public bool HasPool { get; set; }
        public bool HasSpa { get; set; }
        public bool HasKidsClub { get; set; }
        public bool AllInclusive { get; set; }
        public decimal Rating { get; set; }
        public string? ImageUrlsJson { get; set; }

        public virtual HotelCategory HotelCategory { get; set; } = null!;
        public virtual City City { get; set; } = null!;
        public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
    }
}