using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using QualificationSite.Services.Interfaces;
using QualificationSite.ViewModel;

namespace QualificationSite.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService service;

    public AccountController(IAccountService service)
    {
        this.service = service;
    }
    [HttpGet]
    public IActionResult Register() => View();
    /// <summary>
    /// Метод регистрации. Сюда приходят данные с формы регистрации.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Register(RegistryViewModel model)
    {
        if (!ModelState.IsValid) return View();
        var response = await service.Register(model);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(response.Data));
            return RedirectToAction("Index", "Home");
        }

        ViewBag.ErrorDesc = response.Description;

        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity == null) return RedirectToAction("Index", "Home");
        if (!User.Identity.IsAuthenticated)
            return View();
        ViewBag.Name = User.Identity.Name;
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var response = await service.Authenticate(model);
        if (response.StatusCode == HttpStatusCode.Accepted)
        {
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(response.Data));
            return RedirectToAction("Index", "Home");
        }
        ViewBag.ErrorDesc = response.Description;

        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}