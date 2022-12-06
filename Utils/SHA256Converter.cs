using System.Security.Cryptography;
using System.Text;
using BitConverter = System.BitConverter;

namespace QualificationSite.Utils;

public static class Sha256Converter
{
    /// <summary>
    /// Хэш-код для хранения пароля в базе данных
    /// </summary>
    /// <param name="password">Сам пароль в текстовом виде</param>
    /// <returns>Пароль в виде хэш-строки</returns>
    public static string ConvertToSha256(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        return hash;
    }
}