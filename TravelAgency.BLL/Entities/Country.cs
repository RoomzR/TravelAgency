using System.Security.Authentication;

namespace TravelAgency.BLL.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public virtual ICollection<City> Cities { get; set; } = new List<City>();
        public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
    }
}