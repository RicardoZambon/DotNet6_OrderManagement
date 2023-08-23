using Microsoft.EntityFrameworkCore;
using Zambon.OrderManagement.Core.BusinessEntities.General;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Helpers.Validations;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.Core.Repositories.General.Interfaces;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;

namespace Zambon.OrderManagement.Core.Repositories.General
{
    public class CustomersRepository : ICustomersRepository
    {
        private readonly AppDbContext dbContext;

        public CustomersRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public async Task AddAsync(Customers customer)
        {
            await ValidateAsync(customer);

            await dbContext.Set<Customers>().AddAsync(customer);
        }

        public async Task<Customers?> FindByIdAsync(long customerId)
            => await dbContext.FindAsync<Customers>(customerId);

        public IQueryable<Customers> List(IListParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var filters = new Dictionary<string, object>(parameters.Filters, StringComparer.InvariantCultureIgnoreCase);

            var list =
                from c in dbContext.Set<Customers>()
                where
                    !parameters.Filters.Any()
                    || EF.Functions.Like(c.Name ?? string.Empty, $"%{filters[nameof(Customers.Name)]}%")
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

        public async Task RemoveAsync(long customerId)
        {
            if (await FindByIdAsync(customerId) is not Customers customer)
            {
                throw new EntityNotFoundException(nameof(Customers), customerId);
            }

            customer.IsDeleted = true;

            dbContext.Set<Customers>().Update(customer);
        }

        public async Task UpdateAsync(Customers customer)
        {
            await ValidateAsync(customer);

            dbContext.Set<Customers>().Update(customer);
        }

        public async Task ValidateAsync(Customers customer)
        {
            var result = new ValidationResult();

            // Name
            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                result.SetError(nameof(Customers.Name), "required");
            }
            else if (await dbContext.Set<Customers>().AnyAsync(x => EF.Functions.Like(x.Name ?? string.Empty, customer.Name) && x.ID != customer.ID))
            {
                result.SetError(nameof(Customers.Name), "exists");
            }

            if (result.Errors.Any())
            {
                throw new EntityValidationFailureException(nameof(Customers), customer.ID, result);
            }
        }
    }
}