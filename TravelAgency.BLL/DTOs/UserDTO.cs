namespace TravelAgency.BLL.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsBlocked { get; set; } 
        public DateTime RegistrationDate { get; set; }
    }
}