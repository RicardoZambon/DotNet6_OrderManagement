namespace Zambon.OrderManagement.WebApi.Models
{
    public class BatchUpdateModel<TEntity, TKey> where TEntity: class
    {
        public TEntity[]? EntitiesToAddUpdate { get; set; }

        public TKey[]? EntitiesToDelete { get; set; }
    }
}