namespace TravelAgency.BLL.DTOs
{
    public class ContactRequestDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? UserId { get; set; }
    }

    public class ContactRequestCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ContactRequestUpdateDTO
    {
        public int Id { get; set; }
        public bool IsRead { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}