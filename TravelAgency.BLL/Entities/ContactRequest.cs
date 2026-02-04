using TravelAgency.BLL.Enums;

namespace TravelAgency.BLL.Entities
{
    public class ContactRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public ContactStatus Status { get; set; } = ContactStatus.New;

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
