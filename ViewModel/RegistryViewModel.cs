using System.ComponentModel.DataAnnotations;

namespace QualificationSite.ViewModel;
/// <summary>
/// ViewModel для регистрации.
/// </summary>
public class RegistryViewModel
{
    [Required(ErrorMessage = "Укажите логин.")]
    [MaxLength(20, ErrorMessage = "Логин не может превышать 20 символов.")]
    [MinLength(3, ErrorMessage = "Минимальная длина логина - 3 символа.")]
    public string Login { get; set; }
    [Required(ErrorMessage = "Введите пароль.")]
    [MinLength(3, ErrorMessage = "Минимальная длина пароля - 3 символа")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [MaxLength(50)]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Подтвердите пароль.")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
    public string RepeatPassword { get; set; }
}