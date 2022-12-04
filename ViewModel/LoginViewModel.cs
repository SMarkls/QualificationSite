using System.ComponentModel.DataAnnotations;

namespace QualificationSite.ViewModel;
/// <summary>
/// ViewModel для авторизации.
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Введите логин.")]
    [MaxLength(20, ErrorMessage = "Логин не может превышать 20 символов.")]
    [MinLength(3, ErrorMessage = "Минимальная длина логина - 3 символа.")]
    public string Login { get; set; }
    [Required(ErrorMessage = "Введите пароль.")]
    [MinLength(3, ErrorMessage = "Минимальная длина пароля - 3 символа")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}