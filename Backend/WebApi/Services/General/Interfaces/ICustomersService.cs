using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.WebApi.Models.General;
using Zambon.OrderManagement.WebApi.Models.Security;

namespace Zambon.OrderManagement.WebApi.Services.General.Interfaces
{
    public interface ICustomersService
    {
        Task<CustomerUpdateModel> FindCustomerByIdAsync(long customerId);
        Task<CustomerUpdateModel> InsertNewCustomerAsync(CustomerInsertModel customerModel);
        IEnumerable<CustomersListModel> ListCustomers(IListParameters parameters);
        Task RemoveCustomersAsync(long[] customerIds);
        Task<CustomerUpdateModel> UpdateExistingCustomerAsync(CustomerUpdateModel customerModel);
    }
}