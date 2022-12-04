using System.Net;
using Microsoft.EntityFrameworkCore;
using QualificationSite.Models;
using QualificationSite.Models.DataBase;
using QualificationSite.Services.Interfaces;
using QualificationSite.ViewModel;

namespace QualificationSite.Services.Implementations;

public class ProfileService : IProfileService
{
    private readonly UsersDbContext context;

    public ProfileService(UsersDbContext context)
    {
        this.context = context;
    }
    public async Task<BaseResponse<Profile>> GetProfileAsync(long id)
    {
        var profile = await context.Profiles.FirstOrDefaultAsync(p => p.Id == id);
        if (profile.Age == null)
            return new BaseResponse<Profile>
            {
                Data = null,
                Description = "Профиль не найден.",
                StatusCode = HttpStatusCode.NotFound
            };
        return new BaseResponse<Profile>
        {
            Data = profile,
            Description = "Профиль найден.",
            StatusCode = HttpStatusCode.Found
        };
    }

    public async Task<BaseResponse<bool>> CreateProfile()
    {
        await context.Profiles.AddAsync(new Profile());
        await context.SaveChangesAsync();
        return new BaseResponse<bool>
        {
            Data = true,
            Description = "Профиль успешно создан",
            StatusCode = HttpStatusCode.Accepted
        };
    }

    public async Task<BaseResponse<bool>> EditProfile(ProfileViewModel profile, long id)
    {
        var check = await context.Profiles.FirstOrDefaultAsync(p => p.Id == id);
        if (check == null)
            return new BaseResponse<bool>
            {
                Data = false,
                Description = "Профиль с таким ID не существует. Неизвестная ошибка",
                StatusCode = HttpStatusCode.NotFound
            };
        check.Age = profile.Age;
        check.City = profile.City;
        check.Languages = profile.Languages;
        check.Name = profile.Name;
        check.Surname = profile.Surname;
        check.University = profile.University;
        await context.SaveChangesAsync();
        return new BaseResponse<bool>
        {
            Data = true,
            Description = "Профиль успешно изменен.",
            StatusCode = HttpStatusCode.Created
        };
    }
}