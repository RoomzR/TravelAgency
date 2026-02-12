namespace TravelAgency.DAL.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CountryId { get; set; }

        public virtual Country Country { get; set; } = null!;
        public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
        public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
    }
}