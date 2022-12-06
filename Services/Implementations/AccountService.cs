using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualificationSite.Models;
using QualificationSite.Models.DataBase;
using QualificationSite.Services.Interfaces;
using QualificationSite.Utils;
using QualificationSite.ViewModel;

namespace QualificationSite.Services.Implementations;

public class AccountService : IAccountService
{
    private readonly UsersDbContext context;
    private readonly IProfileService profileService;

    public AccountService(UsersDbContext context, IProfileService profileService)
    {
        this.context = context;
        this.profileService = profileService;
    }
    [HttpPost]
    public async Task<BaseResponse<ClaimsIdentity>> Register(RegistryViewModel model)
    {
        User? check = await context.Users.FirstOrDefaultAsync(u => u.Login == model.Login) ??
                      await context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (check == null)
        {
            if (context.Users.Select(u => u.Email).Contains(model.Email))
                return new BaseResponse<ClaimsIdentity>
                {
                    Description = "Пользователь с таким именем или Email'ом уже существует.",
                    StatusCode = HttpStatusCode.Conflict
                };
            string password = Sha256Converter.ConvertToSha256(model.Password);
            await context.Users.AddAsync(new User { Login = model.Login, Email = model.Email, Password = password, Role = "User"});
            await context.SaveChangesAsync();
            await profileService.CreateProfile();
            var result = Auth(context.Users.OrderBy(u => u.Id).Last());
            return new BaseResponse<ClaimsIdentity>
            {
                Description = "Пользователь успешно зарегистрирован.",
                StatusCode = HttpStatusCode.Created,
                Data = result
            };
        }
        return new BaseResponse<ClaimsIdentity>
        {
            Description = "Пользователь с таким именем или Email'ом уже существует.",
            StatusCode = HttpStatusCode.Conflict
        };
    }

    public async Task<BaseResponse<ClaimsIdentity>> Authenticate(LoginViewModel model)
    {
        User? check = await context.Users.FirstOrDefaultAsync(u => u.Login == model.Login) ??
                     await context.Users.FirstOrDefaultAsync(u => u.Email == model.Login);
        if (check == null)
            return new BaseResponse<ClaimsIdentity>
            {
                Description = "Пользователь с таким именем не найден.",
                StatusCode = HttpStatusCode.Conflict
            };
        string password = Sha256Converter.ConvertToSha256(model.Password);
        if (check.Password == password)
            return new BaseResponse<ClaimsIdentity>
            {
                Description = "Успешно авторизован.",
                StatusCode = HttpStatusCode.Accepted,
                Data = Auth(check)
            };
        return new BaseResponse<ClaimsIdentity>
        {
            Description = "Неверный пароль",
            StatusCode = HttpStatusCode.Conflict
        };

    }

    private ClaimsIdentity Auth(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, user.Login),
            new(ClaimsIdentity.DefaultRoleClaimType, user.Role)
        };
        return new ClaimsIdentity(claims, "ApplicationCookie",
            ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
    }
}