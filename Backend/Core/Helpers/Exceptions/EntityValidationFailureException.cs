using Zambon.OrderManagement.Core.Helpers.Validations;

namespace Zambon.OrderManagement.WebApi.Helpers.Exceptions
{
    public class EntityValidationFailureException : Exception
    {
        public long EntityKey { get; }
        public ValidationResult ValidationResult { get; }

        public EntityValidationFailureException(string entity, long key, ValidationResult result) : base($"Entity '{entity} ({key})' has validation problems.")
        {
            EntityKey = key;
            ValidationResult = result;
        }
    }
}