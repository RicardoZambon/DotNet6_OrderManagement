using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Services.Security.Interfaces;

namespace Zambon.OrderManagement.WebApi.Controllers.Security
{
    /// <summary>
    /// Controller for authenticate users in the application.
    /// </summary>
    [ApiController, Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance.</param>
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }


        /// <summary>
        /// Refresh the JWT token using a valid Refresh Token.
        /// </summary>
        /// <param name="model">Username and refresh token.</param>
        /// <returns>An instance of <see cref="AuthenticationResponseModel"/> with the JWT token and refresh token.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /SignIn
        ///     {
        ///         "RefreshToken": "sample-refresh-token",
        ///         "Username": "admin"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Sucessfully refreshed the token.</response>
        /// <response code="401">Invalid refresh token, token expired, or invalid username.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpPost, Route("[action]"), AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticationResponseModel), 200)]
        public async Task<IActionResult> RefreshToken(RefreshTokenModel model)
        {
            try
            {
                return Ok(await authenticationService.RefreshTokenAsync(model));
            }
            catch (InvalidRefreshTokenException)
            {
                return Unauthorized();
            }
            catch (RefreshTokenNotFoundException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Validate the user credentials to grant access to the API.
        /// </summary>
        /// <param name="model">Username and password to authenticate.</param>
        /// <returns>An instance of <see cref="AuthenticationResponseModel"/> with the JWT token and refresh token.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /SignIn
        ///     {
        ///         "Password": "admin",
        ///         "Username": "admin"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Sucessfully authenticated.</response>
        /// <response code="401">Invalid username or password.</response>
        /// <response code="500">Internal server issue.</response>
        [HttpPost, Route("[action]"), AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticationResponseModel), 200)]
        public async Task<IActionResult> SignIn(SignInModel model)
        {
            try
            {
                return Ok(await authenticationService.SignInAsync(model));
            }
            catch (InvalidAuthenticationException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}