using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models.Stock;

namespace Zambon.OrderManagement.WebApi.Services.Stock.Interfaces
{
    /// <summary>
    /// Service for viewing and updating the <see cref="Orders"/>.
    /// </summary>
    public interface IOrdersService
    {
        /// <summary>
        /// Return a order by the ID.
        /// </summary>
        /// <param name="orderId">The ID of the order to search for.</param>
        /// <returns>An instance of <see cref="OrderUpdateModel"/> representing the properties from the found <see cref="Orders"/> instance.</returns>
        /// <exception cref="EntityNotFoundException">If the ID for the entity is invalid.</exception>
        Task<OrderDisplayModel> FindOrderByIdAsync(long orderId);
        /// <summary>
        /// Return the order total by the ID.
        /// </summary>
        /// <param name="orderId">The ID of the order to search for.</param>
        /// <returns>The total (sum of the products Qty * UnitPrice) of <see cref="Orders"/> instance.</returns>
        /// <exception cref="EntityNotFoundException">If the ID for the entity is invalid.</exception>
        Task<decimal> GetOrderTotalAsync(long orderId);
        /// <summary>
        /// Validate and insert a new order.
        /// </summary>
        /// <param name="orderModel">The <see cref="OrderInsertModel"/> instance to insert.</param>
        /// <returns>An instance of <see cref="OrderUpdateModel"/> representing the properties from the inserted <see cref="Orders"/> instance.</returns>
        /// <exception cref="EntityValidationFailureException">If the <see cref="OrderInsertModel"/> has validation errors.</exception>
        Task<OrderDisplayModel> InsertNewOrderAsync(OrderInsertModel orderModel);
        /// <summary>
        /// Return a list of orders.
        /// </summary>
        /// <param name="parameters">The <see cref="IListParameters"/> instance for pagination and filtering the results.</param>
        /// <returns>A list of <see cref="OrdersListModel"/> representing the properties from the <see cref="Orders"/> list.</returns>
        /// <exception cref="ArgumentNullException">If the <see cref="IListParameters" /> is null.</exception>
        IEnumerable<OrdersListModel> ListOrders(IListParameters parameters);
        /// <summary>
        /// Remove existing orders.
        /// </summary>
        /// <param name="orderIds">The list of order IDs to be deleted.</param>
        /// <returns>A task the represents the asynchronous delete operation.</returns>
        /// <exception cref="EntityNotFoundException">If the ID for the entity is invalid.</exception>
        Task RemoveOrdersAsync(long[] orderIds);
        /// <summary>
        /// Validate and update an existing order.
        /// </summary>
        /// <param name="orderModel">The <see cref="OrderUpdateModel"/> instance to insert.</param>
        /// <returns>An instance of <see cref="OrderUpdateModel"/> representing the properties from the updated <see cref="Orders"/> instance.</returns>
        /// <exception cref="EntityNotFoundException">If the ID for the entity is invalid.</exception>
        /// <exception cref="EntityValidationFailureException">If the <see cref="OrderUpdateModel"/> has validation errors.</exception>
        Task<OrderDisplayModel> UpdateExistingOrderAsync(OrderUpdateModel orderModel);
    }
}