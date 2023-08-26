using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;

namespace Zambon.OrderManagement.WebApi.Services.Security.Interfaces
{
    /// <summary>
    /// Service for authentication of the <see cref="Users"/>.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Refresh the JWT token using a valid Refresh Token.
        /// </summary>
        /// <param name="model">The <see cref="RefreshTokenModel"/> instance to refresh the token.</param>
        /// <returns>An instance of <see cref="AuthenticationResponseModel"/> representing the acess token and properties from the <see cref="Users"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If the <see cref="RefreshTokenModel" /> is null.</exception>
        /// <exception cref="RefreshTokenNotFoundException">If missing the username or refresh token, the user or refresh token are invalid, or password does not match.</exception>
        /// <exception cref="InvalidRefreshTokenException">If the refresh token is revoked or expired.</exception>
        Task<AuthenticationResponseModel> RefreshTokenAsync(RefreshTokenModel model);
        /// <summary>
        /// Validate the user credentials to grant access to the API.
        /// </summary>
        /// <param name="model">The <see cref="RefreshTokenModel"/> instance to sign in the user.</param>
        /// <returns>An instance of <see cref="AuthenticationResponseModel"/> representing the acess token and properties from the <see cref="Users"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If the <see cref="SignInModel" /> is null.</exception>
        /// <exception cref="InvalidAuthenticationException">If missing the username or password, the user is invalid, or password does not match.</exception>
        Task<AuthenticationResponseModel> SignInAsync(SignInModel model);
    }
}