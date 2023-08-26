using Zambon.OrderManagement.Core.BusinessEntities.General;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models.General;

namespace Zambon.OrderManagement.WebApi.Services.General.Interfaces
{
    /// <summary>
    /// Service for viewing and updating the <see cref="Customers"/>.
    /// </summary>
    public interface ICustomersService
    {
        /// <summary>
        /// Return a customer by the ID.
        /// </summary>
        /// <param name="customerId">The ID of the customer to search for.</param>
        /// <returns>An instance of <see cref="CustomerUpdateModel"/> representing the properties from the found <see cref="Customers"/> instance.</returns>
        /// <exception cref="EntityNotFoundException">If the ID for the entity is invalid.</exception>
        Task<CustomerUpdateModel> FindCustomerByIdAsync(long customerId);
        /// <summary>
        /// Validate and insert a new customer.
        /// </summary>
        /// <param name="customerModel">The <see cref="CustomerInsertModel"/> instance to insert.</param>
        /// <returns>An instance of <see cref="CustomerUpdateModel"/> representing the properties from the inserted <see cref="Customers"/> instance.</returns>
        /// <exception cref="EntityValidationFailureException">If the <see cref="CustomerInsertModel"/> has validation errors.</exception>
        Task<CustomerUpdateModel> InsertNewCustomerAsync(CustomerInsertModel customerModel);
        /// <summary>
        /// Return a list of customers.
        /// </summary>
        /// <param name="parameters">The <see cref="IListParameters"/> instance for pagination and filtering the results.</param>
        /// <returns>A list of <see cref="CustomersListModel"/> representing the properties from the <see cref="Customers"/> list.</returns>
        /// <exception cref="ArgumentNullException">If the <see cref="IListParameters" /> is null.</exception>
        IEnumerable<CustomersListModel> ListCustomers(IListParameters parameters);
        /// <summary>
        /// Remove existing customers.
        /// </summary>
        /// <param name="customerIds">The list of customer IDs to be deleted.</param>
        /// <returns>A task the represents the asynchronous delete operation.</returns>
        /// <exception cref="EntityNotFoundException">If the ID for the entity is invalid.</exception>
        Task RemoveCustomersAsync(long[] customerIds);
        /// <summary>
        /// Validate and update an existing customer.
        /// </summary>
        /// <param name="customerModel">The <see cref="CustomerUpdateModel"/> instance to insert.</param>
        /// <returns>An instance of <see cref="CustomerUpdateModel"/> representing the properties from the updated <see cref="Customers"/> instance.</returns>
        /// <exception cref="EntityNotFoundException">If the ID for the entity is invalid.</exception>
        /// <exception cref="EntityValidationFailureException">If the <see cref="CustomerUpdateModel"/> has validation errors.</exception>
        Task<CustomerUpdateModel> UpdateExistingCustomerAsync(CustomerUpdateModel customerModel);
    }
}