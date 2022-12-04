using System.Net;

namespace QualificationSite.Models;
/// <summary>
/// Класс ответа от сервисов. Будет использоваться для авторизации и аутентификации.
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseResponse<T>
{
    public string Description { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public T Data { get; set; }
}