using System.Net;
using Microsoft.AspNetCore.Mvc;
using QualificationSite.Services.Interfaces;
using QualificationSite.ViewModel;

namespace QualificationSite.Controllers;
/// <summary>
/// Контроллер для отображения профилей.
/// </summary>
public class ProfileController : Controller
{   
    // TODO: редактирование профиля, админ может редактировать всех, доступ к чужим профилям.
    private readonly IDataBaseService db;
    private readonly IProfileService service;

    public ProfileController(IDataBaseService db, IProfileService service)
    {
        this.db = db;
        this.service = service;
    }
    [HttpGet]
    public async ValueTask<IActionResult> Id(long id) // ValueTask на тот случай, если не придётся использовать асинхронный код
    {
        if (HttpContext.User.Identity is { IsAuthenticated: false })
            return RedirectToAction("Login", "Account");
        if (HttpContext.User.IsInRole("Admin"))
        {
            var response = await service.GetProfileAsync(id);
            if (response.StatusCode == HttpStatusCode.Found)
                return View(response.Data); // 
            return RedirectToAction("AccessRejected");
        }
        if (await db.GetNameById(id) == HttpContext.User.Identity.Name)
        {
            var response = await service.GetProfileAsync(id);
            if (response.StatusCode == HttpStatusCode.Found)
                return View(response.Data); //
            return RedirectToAction("CreateProfile");
        }
        return RedirectToAction("AccessRejected");
    }
    public IActionResult AccessRejected() => View();
    [HttpGet]
    public IActionResult CreateProfile() => View();

    [HttpPost]
    public async Task<IActionResult> CreateProfile(ProfileViewModel model)
    {
        await service.EditProfile(model, await db.GetIdByName(HttpContext.User.Identity.Name));
        return RedirectToAction("Index", "Home");
    }
}