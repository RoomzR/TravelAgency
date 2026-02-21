namespace TravelAgency.Web.Models.ViewModels
{
    public class StaffUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
        public bool IsLockedOut { get; set; }

        public string FirstName { get; set; } 
        public string LastName { get; set; }
    }
}