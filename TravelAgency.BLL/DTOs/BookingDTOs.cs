namespace TravelAgency.BLL.DTOs
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public int PeopleCount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string? Comments { get; set; }
        public string? ManagerConfirmedId { get; set; }
        public int? PromoCodeId { get; set; }
        public string? TourTitle { get; set; }
        public string? ClientName { get; set; }
        public decimal FinalPrice { get; set; }

        public string? ClientEmail { get; set; }   
        public string? ClientPhoneNumber { get; set; } 
    }

    public class BookingCreateDTO
    {
        public int TourId { get; set; }
        public int PeopleCount { get; set; }
        public string? Comments { get; set; }
        public string? PromoCode { get; set; }
    }

    public class BookingUpdateDTO
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ManagerComments { get; set; }
        public string? ManagerId { get; set; }
    }
}