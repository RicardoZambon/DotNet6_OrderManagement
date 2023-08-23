using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.WebApi.Models.Stock;

namespace Zambon.OrderManagement.WebApi.Services.Stock.Interfaces
{
    public interface IOrdersService
    {
        Task<OrderUpdateModel> FindOrderByIdAsync(long orderId);
        Task<OrderUpdateModel> InsertNewOrderAsync(OrderInsertModel orderModel);
        IEnumerable<OrdersListModel> ListOrders(IListParameters parameters);
        Task RemoveOrdersAsync(long[] orderIds);
        Task<OrderUpdateModel> UpdateExistingOrderAsync(OrderUpdateModel orderModel);
    }
}