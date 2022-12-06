using QualificationSite.Models;
using QualificationSite.ViewModel;

namespace QualificationSite.Services.Interfaces;

public interface IProfileService
{
    public Task<BaseResponse<Profile>> GetProfileAsync(long id);
    public Task<BaseResponse<bool>> CreateProfile();
    public Task<BaseResponse<bool>> EditProfile(ProfileViewModel profile, long id);
    public BaseResponse<List<ProfilePin>> GetPins(long profileId);
    public Task<BaseResponse<ProfilePin>> GetPinAsync(long pinId);
    public Task<BaseResponse<bool>> DeletePinAsync(ProfilePin pin);
    public Task<BaseResponse<bool>> CreatePinAsync(ProfilePinViewModel model);
    public Task<BaseResponse<bool>> IsUserPinAsync(ProfilePin pin, long profileId);
}