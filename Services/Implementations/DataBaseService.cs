using Microsoft.EntityFrameworkCore;
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
    public async Task<string> GetNameById(long id)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
        return user == null ? "BADREQUEST" : user.Login;
    }

    public async Task<long> GetIdByName(string name)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Login == name);
        return user?.Id ?? -1;
    }
}