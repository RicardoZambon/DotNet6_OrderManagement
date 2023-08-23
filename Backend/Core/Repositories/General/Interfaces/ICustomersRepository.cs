using Zambon.OrderManagement.Core.BusinessEntities.General;
using Zambon.OrderManagement.Core.Interfaces;

namespace Zambon.OrderManagement.Core.Repositories.General.Interfaces
{
    public interface ICustomersRepository
    {
        Task AddAsync(Customers customer);
        Task<Customers?> FindByIdAsync(long customerId);
        IQueryable<Customers> List(IListParameters parameters);
        Task RemoveAsync(long customerId);
        Task UpdateAsync(Customers customer);
        Task ValidateAsync(Customers customer);
    }
}