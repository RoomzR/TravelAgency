namespace TravelAgency.BLL.DTOs
{
    public class DirectorStatsDTO
    {
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalUniqueClients { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public Dictionary<string, int> TopTours { get; set; } = new();

        public List<ManagerRatingDTO> ManagerRatings { get; set; } = new();
    }

    public class ManagerRatingDTO
    {
        public string Name { get; set; }
        public int ConfirmedCount { get; set; }
        public decimal Revenue { get; set; }
    }
}