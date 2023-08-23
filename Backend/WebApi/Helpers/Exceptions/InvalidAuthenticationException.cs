namespace Zambon.OrderManagement.WebApi.Helpers.Exceptions
{
    public class InvalidAuthenticationException : Exception
    {
        public InvalidAuthenticationException()
        {
        }

        public InvalidAuthenticationException(string? message) : base(message)
        {
        }
    }
}