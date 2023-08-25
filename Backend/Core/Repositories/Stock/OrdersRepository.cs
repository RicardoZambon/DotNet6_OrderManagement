using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Zambon.OrderManagement.Core.BusinessEntities.General;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Helpers.Validations;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.Core.Repositories.Stock.Interfaces;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;

namespace Zambon.OrderManagement.Core.Repositories.Stock
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly AppDbContext dbContext;

        public OrdersRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public async Task AddAsync(Orders order)
        {
            await ValidateAsync(order);

            await dbContext.Set<Orders>().AddAsync(order);
        }

        public async Task<Orders?> FindByIdAsync(long orderId)
            => await dbContext.FindAsync<Orders>(orderId);

        public async Task<decimal> GetOrderTotalAsync(long orderId)
        {
            var orderIdParameter = new SqlParameter
            {
                ParameterName = "OrderID",
                DbType = DbType.Int64,
                Value = orderId
            };

            var totalParameter = new SqlParameter
            {
                ParameterName = "Total",
                DbType = DbType.Decimal,
                Direction = ParameterDirection.Output
            };

            await dbContext.Database.ExecuteSqlRawAsync("EXEC [Stock].GetOrderTotal {0}, {1} OUTPUT;", orderIdParameter, totalParameter);

            return (decimal)totalParameter.Value;
        }

        public IQueryable<Orders> List(IListParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var list =
                from c in dbContext.Set<Orders>()
                select c;

            if (parameters?.Filters?.Any() ?? false)
            {
                var filters = new Dictionary<string, object>(parameters.Filters, StringComparer.InvariantCultureIgnoreCase);

                if (filters.ContainsKey(nameof(Orders.CustomerID)) && Convert.ToInt64(filters[nameof(Orders.CustomerID)].ToString()) is long customerId)
                {
                    list = list.Where(x => x.CustomerID == customerId);
                }
            }

            list = list.OrderByDescending(x => x.ID);

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

        public async Task RemoveAsync(long orderId)
        {
            if (await FindByIdAsync(orderId) is not Orders order)
            {
                throw new EntityNotFoundException(nameof(Orders), orderId);
            }

            order.IsDeleted = true;

            dbContext.Set<Orders>().Update(order);
        }

        public async Task UpdateAsync(Orders order)
        {
            await ValidateAsync(order);

            dbContext.Set<Orders>().Update(order);
        }

        public async Task ValidateAsync(Orders order)
        {
            var result = new ValidationResult();

            // CustomerID
            if (await dbContext.Set<Customers>().FindAsync(order.CustomerID) is null)
            {
                result.SetError(nameof(Orders.CustomerID), "invalid");
            }

            if (result.Errors.Any())
            {
                throw new EntityValidationFailureException(nameof(Orders), order.ID, result);
            }
        }
    }
}