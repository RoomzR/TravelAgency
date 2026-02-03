namespace TravelAgency.BLL.Entities
{
    public class HotelCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public int Stars { get; set; } 
        public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
        public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
    }
}