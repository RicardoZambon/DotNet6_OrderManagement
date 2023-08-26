namespace Zambon.OrderManagement.WebApi.Helpers.Exceptions
{
    /// <summary>
    /// Represents error of user authentication failure during sign in or refresh token.
    /// </summary>
    public class InvalidAuthenticationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="InvalidAuthenticationException"/> class.
        /// </summary>
        public InvalidAuthenticationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="InvalidAuthenticationException"/> class.
        /// </summary>
        /// <param name="message">The custom error message.</param>
        public InvalidAuthenticationException(string? message) : base(message)
        {
        }
    }
}