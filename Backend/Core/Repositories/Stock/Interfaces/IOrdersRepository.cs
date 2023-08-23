using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Interfaces;

namespace Zambon.OrderManagement.Core.Repositories.Stock.Interfaces
{
    public interface IOrdersRepository
    {
        Task AddAsync(Orders order);
        Task<Orders?> FindByIdAsync(long orderId);
        IQueryable<Orders> List(IListParameters parameters);
        Task RemoveAsync(long orderId);
        Task UpdateAsync(Orders order);
        Task ValidateAsync(Orders order);
    }
}