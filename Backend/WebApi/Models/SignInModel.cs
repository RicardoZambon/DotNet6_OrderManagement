using Zambon.OrderManagement.Core.BusinessEntities.Security;

namespace Zambon.OrderManagement.WebApi.Models
{
    /// <summary>
    /// Model used to authenticate and generate JWT token and refresh token for a user.
    /// </summary>
    public class SignInModel
    {
        /// <summary>
        /// The password from the <see cref="Users"/> instance.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// The username from the <see cref="Users"/> instance.
        /// </summary>
        public string? Username { get; set; }
    }
}