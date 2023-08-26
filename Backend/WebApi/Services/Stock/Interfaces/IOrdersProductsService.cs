using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.Stock;

namespace Zambon.OrderManagement.WebApi.Services.Stock.Interfaces
{
    /// <summary>
    /// Service for viewing and updating the <see cref="OrdersProducts"/>.
    /// </summary>
    public interface IOrdersProductsService
    {
        /// <summary>
        /// Update the products from an order.
        /// </summary>
        /// <param name="orderId">The ID of the order to search for.</param>
        /// <param name="batchUpdateModel">The model instance of <see cref="OrdersProductUpdateModel"/> and IDs to insert, update, or delete.</param>
        /// <returns>A task the represents the asynchronous batch update operation.</returns>
        Task BatchUpdateOrdersProductsAsync(long orderId, BatchUpdateModel<OrdersProductUpdateModel, long> batchUpdateModel);
        /// <summary>
        /// Return a list of products from an order.
        /// </summary>
        /// <param name="orderId">The ID of the order to search for.</param>
        /// <param name="parameters">The <see cref="IListParameters"/> instance for pagination the results.</param>
        /// <returns>A list of <see cref="OrdersProductsListModel"/> representing the properties from the <see cref="OrdersProducts"/> list.</returns>
        Task<IEnumerable<OrdersProductsListModel>> ListOrdersProductsAsync(long orderId, IListParameters parameters);
    }
}