namespace Zambon.OrderManagement.WebApi.Models
{
    public class RelationshipUpdateModel<TKey>
    {
        public IEnumerable<TKey> IdsToAdd { get; set; } = Enumerable.Empty<TKey>();

        public IEnumerable<TKey> IdsToRemove { get; set; } = Enumerable.Empty<TKey>();
    }
}