using Microsoft.EntityFrameworkCore;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Helpers.Validations;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.Core.Repositories.Stock.Interfaces;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;

namespace Zambon.OrderManagement.Core.Repositories.Stock
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly AppDbContext dbContext;

        public ProductsRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public async Task AddAsync(Products product)
        {
            await ValidateAsync(product);

            await dbContext.Set<Products>().AddAsync(product);
        }

        public async Task<Products?> FindByIdAsync(long productId)
            => await dbContext.FindAsync<Products>(productId);

        public IQueryable<Products> List(IListParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var filters = new Dictionary<string, object>(parameters.Filters, StringComparer.InvariantCultureIgnoreCase);

            var list =
                from c in dbContext.Set<Products>()
                where
                    !parameters.Filters.Any()
                    || EF.Functions.Like(c.Name ?? string.Empty, $"*{filters[nameof(Products.Name)]}*")
                select c;

            list = list.OrderBy(x => x.Name);

            if (parameters?.StartRow > 0)
            {
                list = list.Skip(parameters.StartRow);
            }

            if (parameters?.EndRow > 0)
            {
                list = list.Take(parameters.EndRow);
            }

            return list;
        }

        public async Task RemoveAsync(long productId)
        {
            if (await FindByIdAsync(productId) is not Products product)
            {
                throw new EntityNotFoundException(nameof(Products), productId);
            }

            product.IsDeleted = true;

            dbContext.Set<Products>().Update(product);
        }

        public async Task UpdateAsync(Products product)
        {
            await ValidateAsync(product);

            dbContext.Set<Products>().Update(product);
        }

        public async Task ValidateAsync(Products product)
        {
            var result = new ValidationResult();

            // Name
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                result.SetError(nameof(Products.Name), "required");
            }
            else if (await dbContext.Set<Products>().AnyAsync(x => EF.Functions.Like(x.Name ?? string.Empty, product.Name) && x.ID != product.ID))
            {
                result.SetError(nameof(Products.Name), "exists");
            }

            // Unit price
            if (product.UnitPrice < 0)
            {
                result.SetError(nameof(Products.UnitPrice), "min");
            }

            if (result.Errors.Any())
            {
                throw new EntityValidationFailureException(nameof(Products), product.ID, result);
            }
        }
    }
}