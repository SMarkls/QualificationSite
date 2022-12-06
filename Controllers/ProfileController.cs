using System.Net;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
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
        var pins = await service.GetPinsAsync(id);
        var profile = await service.GetProfileAsync(id);
        ProfileViewModel model;
        if (HttpContext.User.IsInRole("Admin"))
        {
            var response = profile;
            if (response.StatusCode == HttpStatusCode.Found)
            {
                ViewBag.EditAccess = true;
                model = new ProfileViewModel
                {
                    Age = response.Data.Age.Value, City = response.Data.City, Languages = response.Data.Languages,
                    Name = response.Data.Name, Surname = response.Data.Surname, University = response.Data.University,
                    Pins =  pins.Data, Id = response.Data.Id
                };
                return View(model);
            }
        }
        if (await db.GetNameByIdAsync(id) == HttpContext.User.Identity.Name)
        {
            var response = profile;
            if (response.StatusCode == HttpStatusCode.Found)
            {
                ViewBag.EditAccess = true;
                model = new ProfileViewModel
                {
                    Age = response.Data.Age.Value, City = response.Data.City, Languages = response.Data.Languages,
                    Name = response.Data.Name, Surname = response.Data.Surname, University = response.Data.University,
                    Pins = pins.Data, Id = response.Data.Id
                };
                return View(model);
            }
            return RedirectToAction("CreateProfile");
        }
        if (profile.StatusCode == HttpStatusCode.Found)
        {
            var response = profile;
            model = new ProfileViewModel
            {
                Age = response.Data.Age.Value, City = response.Data.City, Languages = response.Data.Languages,
                Name = response.Data.Name, Surname = response.Data.Surname, University = response.Data.University,
                Pins = pins.Data, Id = response.Data.Id
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
        long profileId;
        string str = HttpContext.Request.Form["profileId"].First();
        profileId = long.Parse(str.Replace("/", ""));
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
        return await My();
    }

    [HttpGet]
    public async Task<IActionResult> EditPinboard(long id)
    {
        if (await db.GetIdByNameAsync(HttpContext.User.Identity.Name) != id && !HttpContext.User.IsInRole("Admin"))
            return RedirectToAction("AccessRejected");
        var response = await service.GetPinsAsync(id);
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
        long profileId;
        string str = HttpContext.Request.Form["profileId"].First();
        profileId = long.Parse(str.Replace("/", ""));
        var id = await db.GetIdByNameAsync(HttpContext.User.Identity.Name);
        if (id != profileId && !HttpContext.User.IsInRole("Admin"))
            return Redirect("/profile/editpinboard/" + profileId); 
        model.ProfileId = profileId;
        await service.CreatePinAsync(model);
        return Redirect("/profile/editpinboard/" + profileId);
    }
}