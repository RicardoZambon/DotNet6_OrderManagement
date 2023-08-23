using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Interfaces;

namespace Zambon.OrderManagement.Core.Repositories.Stock.Interfaces
{
    public interface IOrdersProductsRepository
    {
        Task AddAsync(OrdersProducts orderProduct);
        Task<OrdersProducts?> FindByIdAsync(long orderProductId);
        IQueryable<OrdersProducts> List(long orderId, IListParameters parameters);
        Task RemoveAsync(long orderProductId);
        Task UpdateAsync(OrdersProducts orderProduct);
        Task ValidateAsync(OrdersProducts orderProduct);
    }
}