namespace TravelAgency.DAL.Entities
{
    public class TourType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
    }
}