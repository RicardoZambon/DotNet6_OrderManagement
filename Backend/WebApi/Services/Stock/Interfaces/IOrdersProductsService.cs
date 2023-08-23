using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.Stock;

namespace Zambon.OrderManagement.WebApi.Services.Stock.Interfaces
{
    public interface IOrdersProductsService
    {
        Task BatchUpdateOrdersProductsAsync(long orderId, BatchUpdateModel<OrdersProductUpdateModel, long> batchUpdateModel);
        Task<IEnumerable<OrdersProductsListModel>> ListOrdersProductsAsync(long orderId, IListParameters parameters);
    }
}