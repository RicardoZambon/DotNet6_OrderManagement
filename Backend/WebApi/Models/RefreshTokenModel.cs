using Zambon.OrderManagement.Core.BusinessEntities.Security;

namespace Zambon.OrderManagement.WebApi.Models
{
    /// <summary>
    /// Model used to generate JWT token and refresh token for a user.
    /// </summary>
    public class RefreshTokenModel
    {
        /// <summary>
        /// The refresh token from the <see cref="RefreshTokens"/> instance to generate a new JWT token.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// The username from the <see cref="Users"/> instance owner of the refresh token.
        /// </summary>
        public string? Username { get; set; }
    }
}