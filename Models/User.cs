using System.ComponentModel.DataAnnotations;

namespace QualificationSite.Models;
/// <summary>
/// Класс пользователя, для хранения в базе данных.
/// </summary>
public class User
{
    [Key]
    public long Id { get; set; }
    [MaxLength(20)]
    public string Login { get; set; }
    [MaxLength(50)]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [DataType(DataType.Password)]
    [MaxLength(120)]
    public string Password { get; set; }
    [MaxLength(20)]
    public string Role { get; set; }
}