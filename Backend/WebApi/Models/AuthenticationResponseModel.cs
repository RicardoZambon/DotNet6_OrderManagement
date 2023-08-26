using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Zambon.OrderManagement.Core.BusinessEntities.Security;

namespace Zambon.OrderManagement.WebApi.Models
{
    /// <summary>
    /// Model response when authenticating a user.
    /// </summary>
    [AutoMap(typeof(Users), ReverseMap = false)]
    public class AuthenticationResponseModel
    {
        /// <summary>
        /// The email from the <see cref="Users"/> instance.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// The refresh token generated.
        /// </summary>
        [Ignore]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// The refresh token expiration.
        /// </summary>
        [Ignore]
        public DateTime RefreshTokenExpiration { get; set; }

        /// <summary>
        /// The JWT token generated.
        /// </summary>
        [Ignore]
        public string? Token { get; set; }

        /// <summary>
        /// The username from the <see cref="Users"/> instance.
        /// </summary>
        public string? Username { get; set; }
    }
}