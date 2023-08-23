namespace Zambon.OrderManagement.WebApi.Models
{
    /// <summary>
    /// Model of values used to authenticate and generate a new user JWT token and Refresh Token.
    /// </summary>
    public class SignInModel
    {
        public string? Password { get; set; }

        public string? Username { get; set; }
    }
}