namespace TravelAgency.BLL.DTOs
{
    public class AdminDashboardDTO
    {
        public int TotalUsersCount { get; set; }
        public int TotalToursCount { get; set; }
        public int ActiveBookingsCount { get; set; } 
        public decimal TotalRevenue { get; set; }

        public List<BookingDTO> RecentBookings { get; set; }
        public List<UserDTO> NewUsers { get; set; }
    }
}