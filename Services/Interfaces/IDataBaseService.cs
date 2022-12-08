using QualificationSite.Models;

namespace QualificationSite.Services.Interfaces;

public interface IDataBaseService
{
    public Task<string> GetNameByIdAsync(long id);
    public Task<long> GetIdByNameAsync(string name);
    public Task<BaseResponse<List<Profile>>> GetProfilesAsync();
}