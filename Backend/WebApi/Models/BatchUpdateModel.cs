namespace Zambon.OrderManagement.WebApi.Models
{
    /// <summary>
    /// Model used to update entities in batch.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to add or update.</typeparam>
    /// <typeparam name="TKey">The type of the key property of the entity to delete.</typeparam>
    public class BatchUpdateModel<TEntity, TKey> where TEntity: class
    {
        /// <summary>
        /// List of entites to add or update.
        /// </summary>
        public TEntity[]? EntitiesToAddUpdate { get; set; }

        /// <summary>
        /// List of ID from the entities to delete.
        /// </summary>
        public TKey[]? EntitiesToDelete { get; set; }
    }
}