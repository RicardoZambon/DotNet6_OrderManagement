namespace Zambon.OrderManagement.WebApi.Models
{
    /// <summary>
    /// Model of values used to generate a new user JWT token and Refresh Token.
    /// </summary>
    public class RefreshTokenModel
    {
        /// <summary>
        /// Refresh Token to generate a new JWT Token.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Username to refresh the token.
        /// </summary>
        public string? Username { get; set; }
    }
}