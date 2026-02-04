using System.ComponentModel.DataAnnotations;

namespace TravelAgency.Web.Models.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Имя обязательно")]
        [Display(Name = "Ваше имя")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Некорректный номер телефона")]
        [Display(Name = "Телефон")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Сообщение обязательно")]
        [StringLength(1000, ErrorMessage = "Сообщение не должно превышать 1000 символов")]
        [Display(Name = "Сообщение")]
        public string Message { get; set; } = string.Empty;
    }
}