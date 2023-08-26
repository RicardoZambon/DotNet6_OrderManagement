using Microsoft.AspNetCore.Mvc;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;

namespace Zambon.OrderManagement.WebApi.Helpers
{
    /// <summary>
    /// A custom entity of <see cref="ProblemDetails"/> for validation errors.
    /// </summary>
    public class ValidationProblemEntityDetails : ValidationProblemDetails
    {
        /// <summary>
        /// The ID of the entity with validation errors.
        /// </summary>
        public long EntityKey { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ValidationProblemEntityDetails"/>.
        /// </summary>
        /// <param name="exception">The <see cref="EntityValidationFailureException"/> instance.</param>
        public ValidationProblemEntityDetails(EntityValidationFailureException exception) : this(exception.EntityKey, exception.ValidationResult.Errors)
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="ValidationProblemEntityDetails"/>.
        /// </summary>
        /// <param name="entityKey">The ID of the entity with validation errors.</param>
        /// <param name="errors">The dictionary instance containing the validation errors.</param>
        public ValidationProblemEntityDetails(long entityKey, IDictionary<string, string[]> errors) : base(errors)
        {
            EntityKey = entityKey;
        }
    }
}