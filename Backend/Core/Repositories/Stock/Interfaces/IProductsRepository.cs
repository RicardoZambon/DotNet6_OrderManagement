using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Interfaces;

namespace Zambon.OrderManagement.Core.Repositories.Stock.Interfaces
{
    public interface IProductsRepository
    {
        Task AddAsync(Products product);
        Task<Products?> FindByIdAsync(long productId);
        IQueryable<Products> List(IListParameters parameters);
        Task RemoveAsync(long productId);
        Task UpdateAsync(Products product);
        Task ValidateAsync(Products product);
    }
}