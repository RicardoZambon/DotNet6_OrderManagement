using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Helpers.Validations;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.Core.Repositories.Stock.Interfaces;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;

namespace Zambon.OrderManagement.Core.Repositories.Stock
{
    public class OrdersProductsRepository : IOrdersProductsRepository
    {
        private readonly AppDbContext dbContext;

        public OrdersProductsRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public async Task AddAsync(OrdersProducts orderProduct)
        {
            await ValidateAsync(orderProduct);

            await dbContext.Set<OrdersProducts>().AddAsync(orderProduct);
        }

        public async Task<OrdersProducts?> FindByIdAsync(long orderProductId)
            => await dbContext.FindAsync<OrdersProducts>(orderProductId);

        public IQueryable<OrdersProducts> List(long orderId, IListParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var list =
                from o in dbContext.Set<OrdersProducts>()
                where o.OrderID == orderId
                select o;

            list = list.OrderByDescending(x => x.Product != null ? x.Product.Name : string.Empty);

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

        public async Task RemoveAsync(long orderProductId)
        {
            if (await FindByIdAsync(orderProductId) is not OrdersProducts orderProduct)
            {
                throw new EntityNotFoundException(nameof(OrdersProducts), orderProductId);
            }

            orderProduct.IsDeleted = true;

            dbContext.Set<OrdersProducts>().Update(orderProduct);
        }

        public async Task UpdateAsync(OrdersProducts orderProduct)
        {
            await ValidateAsync(orderProduct);

            dbContext.Set<OrdersProducts>().Update(orderProduct);
        }

        public async Task ValidateAsync(OrdersProducts orderProduct)
        {
            var result = new ValidationResult();

            // ProductID
            if (await dbContext.Set<Products>().FindAsync(orderProduct.ProductID) is not Products product)
            {
                result.SetError(nameof(OrdersProducts.ProductID), "invalid");
            }
            else
            {
                orderProduct.UnitPrice = product.UnitPrice;
            }

            if (orderProduct.Qty <= 0)
            {
                result.SetError(nameof(OrdersProducts.Qty), "min");
            }

            if (result.Errors.Any())
            {
                throw new EntityValidationFailureException(nameof(OrdersProducts), orderProduct.ID, result);
            }
        }
    }
}