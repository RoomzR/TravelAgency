namespace TravelAgency.Web.Models.ViewModels
{
    public class BookingViewModel
    {
        public int TourId { get; set; }
        public int PeopleCount { get; set; }
        public string? Comments { get; set; }
        
        public string? TourTitle { get; set; }
        public decimal TourPrice { get; set; }
        public int AvailablePlaces { get; set; }
    }
}