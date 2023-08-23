using Microsoft.AspNetCore.Mvc;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;

namespace Zambon.OrderManagement.WebApi.Helpers
{
    public class ValidationProblemEntityDetails : ValidationProblemDetails
    {
        public long EntityKey { get; }


        public ValidationProblemEntityDetails(EntityValidationFailureException exception) : this(exception.EntityKey, exception.ValidationResult.Errors)
        {
            
        }

        public ValidationProblemEntityDetails(long entityKey, IDictionary<string, string[]> errors) : base (errors)
        {
            EntityKey = entityKey;
        }
    }
}