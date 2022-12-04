using QualificationSite.Models;
using QualificationSite.ViewModel;

namespace QualificationSite.Services.Interfaces;

public interface IProfileService
{
    public Task<BaseResponse<Profile>> GetProfileAsync(long id);
    public Task<BaseResponse<bool>> CreateProfile();
    public Task<BaseResponse<bool>> EditProfile(ProfileViewModel profile, long id);
}