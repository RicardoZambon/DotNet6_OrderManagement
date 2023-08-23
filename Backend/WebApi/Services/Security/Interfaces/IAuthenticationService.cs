using Zambon.OrderManagement.WebApi.Models;

namespace Zambon.OrderManagement.WebApi.Services.Security.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponseModel> RefreshTokenAsync(RefreshTokenModel model);
        Task<AuthenticationResponseModel> SignInAsync(SignInModel model);
    }
}