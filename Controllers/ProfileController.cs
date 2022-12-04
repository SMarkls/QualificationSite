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
    private readonly IWebHostEnvironment appEnvironment;

    public ProfileController(IDataBaseService db, IProfileService service, IWebHostEnvironment appEnvironment)
    {
        this.db = db;
        this.service = service;
        this.appEnvironment = appEnvironment;
    }
    [HttpGet]
    public async ValueTask<IActionResult> Id(long id) // ValueTask на тот случай, если не придётся использовать асинхронный код
    {
        if (HttpContext.User.Identity is { IsAuthenticated: false })
            return RedirectToAction("Login", "Account");
        ViewBag.Path = appEnvironment.WebRootPath;
        if (HttpContext.User.IsInRole("Admin"))
        {
            var response = await service.GetProfileAsync(id);
            if (response.StatusCode == HttpStatusCode.Found)
            {
                ViewBag.EditAccess = true;
                return View(response.Data);
            }
        }
        if (await db.GetNameByIdAsync(id) == HttpContext.User.Identity.Name)
        {
            var response = await service.GetProfileAsync(id);
            if (response.StatusCode == HttpStatusCode.Found)
            {
                ViewBag.EditAccess = true;
                return View(response.Data);
            }
            return View(response.Data);
        }
        return RedirectToAction("AccessRejected");
    }
    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        if (await db.GetIdByNameAsync(HttpContext.User.Identity.Name) != id)
            return RedirectToAction("AccessRejected");
        var response = await service.GetProfileAsync(id);
        if (response.StatusCode == HttpStatusCode.Found)
            return View(response.Data);
        return RedirectToAction("AccessRejected");
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProfileViewModel model)
    {
        var id = await db.GetIdByNameAsync(HttpContext.User.Identity.Name);
        if (ModelState.IsValid)
            await service.EditProfile(model, id); // TODO: сделать обработку неверной модели.
        return Redirect("/profile/" + id);
    }
    public IActionResult AccessRejected() => View();
    [HttpGet]
    public IActionResult CreateProfile() => View();

    [HttpPost]
    public async ValueTask<IActionResult> AddPhoto(IFormFile file)
    {
        long id;
        if (file != null)
        {
            id = await db.GetIdByNameAsync(HttpContext.User.Identity.Name);
            string path = @"/Resources/ProfilePhotos/profile_photo_" + id + ".jpg";
            using var fs = new FileStream(appEnvironment.WebRootPath + path, FileMode.Create);
            await file.CopyToAsync(fs);
            return Redirect("/profile/" + id);
        }
        return Redirect(".");
    }
    [HttpPost]
    public async Task<IActionResult> CreateProfile(ProfileViewModel model)
    {
        if (ModelState.IsValid)
            await service.EditProfile(model, await db.GetIdByNameAsync(HttpContext.User.Identity.Name));
        return RedirectToAction("Index", "Home");
    }
}