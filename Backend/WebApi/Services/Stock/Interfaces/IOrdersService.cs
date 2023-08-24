using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.WebApi.Models.Stock;

namespace Zambon.OrderManagement.WebApi.Services.Stock.Interfaces
{
    public interface IOrdersService
    {
        Task<OrderDisplayModel> FindOrderByIdAsync(long orderId);
        Task<OrderDisplayModel> InsertNewOrderAsync(OrderInsertModel orderModel);
        IEnumerable<OrdersListModel> ListOrders(IListParameters parameters);
        Task RemoveOrdersAsync(long[] orderIds);
        Task<OrderDisplayModel> UpdateExistingOrderAsync(OrderUpdateModel orderModel);
    }
}