namespace TravelAgency.BLL.DTOs
{
    public class ReviewAdminDTO : ReviewDTO
    {
        public bool IsApproved { get; set; }
        public string? ClientEmail { get; set; }
    }
}