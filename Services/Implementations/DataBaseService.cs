using System.Net;
using Microsoft.EntityFrameworkCore;
using QualificationSite.Models;
using QualificationSite.Models.DataBase;
using QualificationSite.Services.Interfaces;

namespace QualificationSite.Services.Implementations;

public class DataBaseService : IDataBaseService
{
    private readonly UsersDbContext context;

    public DataBaseService(UsersDbContext context)
    {
        this.context = context;
    }
    public async Task<string> GetNameByIdAsync(long id)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
        return user == null ? "BADREQUEST" : user.Login;
    }

    public async Task<long> GetIdByNameAsync(string name)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Login == name);
        return user?.Id ?? -1;
    }

    public async Task<BaseResponse<List<Profile>>> GetProfilesAsync()
    {
        List<Profile> list = await context.Profiles.ToListAsync();
        return new BaseResponse<List<Profile>>
        {
            Data = list,
            Description = "Список",
            StatusCode = HttpStatusCode.Accepted
        };
    }
}