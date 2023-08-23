using AutoMapper;
using AutoMapper.QueryableExtensions;
using Zambon.OrderManagement.Core;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.Core.Repositories.Stock.Interfaces;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.Stock;
using Zambon.OrderManagement.WebApi.Services.Stock.Interfaces;

namespace Zambon.OrderManagement.WebApi.Services.Stock
{
    public class OrdersProductsServiceDefault : IOrdersProductsService
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IOrdersProductsRepository ordersProductsRepository;
        private readonly IOrdersRepository ordersRepository;

        public OrdersProductsServiceDefault(
            AppDbContext dbContext,
            IMapper mapper,
            IOrdersProductsRepository ordersProductsRepository,
            IOrdersRepository ordersRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.ordersProductsRepository = ordersProductsRepository;
            this.ordersRepository = ordersRepository;
        }


        public async Task BatchUpdateOrdersProductsAsync(long orderId, BatchUpdateModel<OrdersProductUpdateModel, long> batchUpdateModel)
        {
            if (await ordersRepository.FindByIdAsync(orderId) is not Orders order)
            {
                throw new EntityNotFoundException(nameof(Orders), orderId);
            }

            if (!(batchUpdateModel.EntitiesToAddUpdate?.Any() ?? false)
                && !(batchUpdateModel.EntitiesToDelete?.Any() ?? false))
            {
                return;
            }

            var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                if (batchUpdateModel.EntitiesToAddUpdate?.Any() ?? false)
                {
                    foreach (var orderProductModel in batchUpdateModel.EntitiesToAddUpdate)
                    {
                        if (orderProductModel.ID <= 0)
                        {
                            var orderProduct = mapper.Map<OrdersProducts>(orderProductModel);

                            orderProduct.ID = 0;
                            orderProduct.OrderID = order.ID;
                            try
                            {
                                await ordersProductsRepository.AddAsync(orderProduct);
                            }
                            catch (EntityValidationFailureException ex)
                            {
                                throw new EntityValidationFailureException(nameof(OrdersProducts), orderProductModel.ID, ex.ValidationResult);
                            }
                        }
                        else
                        {
                            if (await ordersProductsRepository.FindByIdAsync(orderProductModel.ID) is not OrdersProducts orderProduct)
                            {
                                throw new EntityNotFoundException(nameof(OrdersProducts), orderProductModel.ID);
                            }

                            await ordersProductsRepository.UpdateAsync(mapper.Map(orderProductModel, orderProduct));
                        }
                    }
                }

                if (batchUpdateModel.EntitiesToDelete?.Any() ?? false)
                {
                    foreach (var orderProductId in batchUpdateModel.EntitiesToDelete)
                    {
                        await ordersProductsRepository.RemoveAsync(orderProductId);
                    }
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<OrdersProductsListModel>> ListOrdersProductsAsync(long orderId, IListParameters parameters)
        {
            if (await ordersRepository.FindByIdAsync(orderId) is null)
            {
                throw new EntityNotFoundException(nameof(Orders), orderId);
            }

            return ordersProductsRepository.List(orderId, parameters).ProjectTo<OrdersProductsListModel>(mapper.ConfigurationProvider);
        }
    }
}