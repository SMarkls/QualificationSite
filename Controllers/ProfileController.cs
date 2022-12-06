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
    public async Task<IActionResult> My()
    {
        var id = await db.GetIdByNameAsync(HttpContext.User.Identity.Name);
        return Redirect($"/Profile/{id}");
    }
    [HttpGet]
    public async ValueTask<IActionResult> Id(long id) // ValueTask на тот случай, если не придётся использовать асинхронный код
    {
        if (HttpContext.User.Identity is { IsAuthenticated: false })
            return RedirectToAction("Login", "Account");
        ViewBag.Path = appEnvironment.WebRootPath;
        var pins = service.GetPins(id);
        var profile = await service.GetProfileAsync(id);
        ProfileViewModel model;
        if (HttpContext.User.IsInRole("Admin"))
        {
            if (profile.StatusCode == HttpStatusCode.Found)
            {
                ViewBag.EditAccess = true;
                model = new ProfileViewModel
                {
                    Age = profile.Data.Age.Value, City = profile.Data.City, Languages = profile.Data.Languages,
                    Name = profile.Data.Name, Surname = profile.Data.Surname, University = profile.Data.University,
                    Pins =  pins.Data, Id = profile.Data.Id
                };
                return View(model);
            }
        }
        if (await db.GetNameByIdAsync(id) == HttpContext.User.Identity.Name)
        {
            if (profile.StatusCode != HttpStatusCode.Found) return RedirectToAction("CreateProfile");
            ViewBag.EditAccess = true;
            model = new ProfileViewModel
            {
                Age = profile.Data.Age.Value, City = profile.Data.City, Languages = profile.Data.Languages,
                Name = profile.Data.Name, Surname = profile.Data.Surname, University = profile.Data.University,
                Pins = pins.Data, Id = profile.Data.Id
            };
            return View(model);
        }
        if (profile.StatusCode == HttpStatusCode.Found)
        {
            model = new ProfileViewModel
            {
                Age = profile.Data.Age.Value, City = profile.Data.City, Languages = profile.Data.Languages,
                Name = profile.Data.Name, Surname = profile.Data.Surname, University = profile.Data.University,
                Pins = pins.Data, Id = profile.Data.Id
            };
            return View(model);
        }
        else
            return RedirectToAction("AccessRejected");
    }
    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        if (await db.GetIdByNameAsync(HttpContext.User.Identity.Name) != id && !HttpContext.User.IsInRole("Admin"))
            return RedirectToAction("AccessRejected");
        var response = await service.GetProfileAsync(id);
        if (response.StatusCode == HttpStatusCode.Found)
            return View(response.Data);
        return RedirectToAction("AccessRejected");
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProfileViewModel model)
    {
        string str = HttpContext.Request.Form["profileId"].First()!;
        long profileId = long.Parse(str); // TODO: убрать Replace
        var id = await db.GetIdByNameAsync(HttpContext.User.Identity.Name);
        if (id != profileId && !HttpContext.User.IsInRole("Admin"))
            return RedirectToAction("AccessRejected");
        if (ModelState.IsValid)
            await service.EditProfile(model, profileId); // TODO: сделать обработку неверной модели.
        return Redirect("/profile/" + profileId);
    }
    public IActionResult AccessRejected() => View();
    [HttpGet]
    public IActionResult CreateProfile() => View();

    [HttpPost]
    public async ValueTask<IActionResult> AddPhoto(IFormFile file)
    {
        if (file != null)
        {
            var id = await db.GetIdByNameAsync(HttpContext.User.Identity.Name);
            string path = @"/Resources/ProfilePhotos/profile_photo_" + id + ".jpg";
            await using var fs = new FileStream(appEnvironment.WebRootPath + path, FileMode.Create);
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
        return await My();
    }

    [HttpGet]
    public async Task<IActionResult> EditPinboard(long id)
    {
        if (await db.GetIdByNameAsync(HttpContext.User.Identity.Name) != id && !HttpContext.User.IsInRole("Admin"))
            return RedirectToAction("AccessRejected");
        var response = service.GetPins(id);
        var pins = response.Data;
        return View(pins.ToList());
    }

    [HttpGet]
    public async Task<IActionResult> DeletePin(long id)
    {
        var response = await service.GetPinAsync(id);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return RedirectToAction("AccessRejected");
        var profileId = await db.GetIdByNameAsync(HttpContext.User.Identity.Name);
        if ((await service.IsUserPinAsync(response.Data, profileId)).Data || HttpContext.User.IsInRole("Admin"))
            await service.DeletePinAsync(response.Data);
        else
            return RedirectToAction("AccessRejected");
        return Redirect("/profile/editpinboard/" + profileId);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePin(ProfilePinViewModel model)
    {
        string str = HttpContext.Request.Form["profileId"].First();
        long profileId = long.Parse(str);
        var id = await db.GetIdByNameAsync(HttpContext.User.Identity.Name);
        if (id != profileId && !HttpContext.User.IsInRole("Admin"))
            return Redirect("/profile/editpinboard/" + profileId); 
        model.ProfileId = profileId;
        await service.CreatePinAsync(model);
        return Redirect("/profile/editpinboard/" + profileId);
    }
}