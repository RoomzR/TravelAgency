using System.ComponentModel.DataAnnotations;

namespace TravelAgency.Web.Models.ViewModels
{
    public class ProfileViewModel
    {
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }


        [Display(Name = "Имя")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string? FirstName { get; set; }

        [Display(Name = "Фамилия")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Некорректный номер телефона")]
        [Display(Name = "Телефон")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Паспортные данные")]
        [StringLength(100, ErrorMessage = "Паспортные данные не должны превышать 100 символов")]
        public string? PassportData { get; set; }

    }
}
