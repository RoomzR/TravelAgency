using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TravelAgency.Web.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        [Display(Name = "Email")]
        public string Email { get; set; } =string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(100, ErrorMessage ="Пароль должен быть минимум из 6 букв", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage ="Пароли не совпадают")]
        public string ConfirmPassword { get; set; } =string.Empty;

        [Required(ErrorMessage ="Имя обязательно")]
        [Display(Name ="Имя")]
        [StringLength(50, ErrorMessage ="Имя не должно превышать 50 букв")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage ="Фамилия обязательна")]
        [Display(Name ="Фамилия")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 букв")]
        public string LastName { get; set; } = string.Empty;

        [Phone(ErrorMessage ="Некорректный номер телефона")]
        [Display(Name ="Телефон")]
        public string? Phone {  get; set; }
    }
}
