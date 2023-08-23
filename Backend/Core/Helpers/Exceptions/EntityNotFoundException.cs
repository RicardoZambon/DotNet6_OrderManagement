namespace Zambon.OrderManagement.Core.Helpers.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entityName, long entityId) : base($"The Id '{entityId}' for Entity '{entityName}' is invalid.")
        {
        }
    }
}