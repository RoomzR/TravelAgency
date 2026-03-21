namespace TravelAgency.Web.Models.ViewModels
{
    public class DialogViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; }
        public bool IsRead { get; set; }
    }
}
