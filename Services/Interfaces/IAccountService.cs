using System.Security.Claims;
using QualificationSite.Models;
using QualificationSite.ViewModel;

namespace QualificationSite.Services.Interfaces;

public interface IAccountService
{
    public Task<BaseResponse<ClaimsIdentity>> Register(RegistryViewModel model);
    public Task<BaseResponse<ClaimsIdentity>> Authenticate(LoginViewModel model);
}