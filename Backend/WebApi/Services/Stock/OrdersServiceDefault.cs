using AutoMapper;
using AutoMapper.QueryableExtensions;
using Zambon.OrderManagement.Core;
using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.Core.Repositories.Security;
using Zambon.OrderManagement.Core.Repositories.Stock.Interfaces;
using Zambon.OrderManagement.WebApi.Models.Security;
using Zambon.OrderManagement.WebApi.Models.Stock;
using Zambon.OrderManagement.WebApi.Services.Stock.Interfaces;

namespace Zambon.OrderManagement.WebApi.Services.Stock
{
    /// <inheritdoc/>
    public class OrdersServiceDefault : IOrdersService
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IOrdersRepository ordersRepository;

        /// <summary>
        /// Initializes a new instance of <see cref="OrdersServiceDefault"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="AppDbContext"/> instance.</param>
        /// <param name="mapper">The <see cref="IMapper"/> instance.</param>
        /// <param name="ordersRepository">The <see cref="IOrdersRepository"/> instance.</param>
        public OrdersServiceDefault(
            AppDbContext dbContext,
            IMapper mapper,
            IOrdersRepository ordersRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.ordersRepository = ordersRepository;
        }


        /// <inheritdoc/>
        public async Task<OrderDisplayModel> FindOrderByIdAsync(long orderId)
        {
            if (await ordersRepository.FindByIdAsync(orderId) is not Orders order)
            {
                throw new EntityNotFoundException(nameof(Orders), orderId);
            }
            return mapper.Map<OrderDisplayModel>(order);
        }

        /// <inheritdoc/>
        public async Task<decimal> GetOrderTotalAsync(long orderId)
        {
            if (await ordersRepository.FindByIdAsync(orderId) is not Orders)
            {
                throw new EntityNotFoundException(nameof(Orders), orderId);
            }
            return await ordersRepository.GetOrderTotalAsync(orderId);
        }

        /// <inheritdoc/>
        public async Task<OrderDisplayModel> InsertNewOrderAsync(OrderInsertModel orderModel)
        {
            var order = mapper.Map<Orders>(orderModel);

            await ordersRepository.AddAsync(order);
            await dbContext.SaveChangesAsync();

            return mapper.Map<OrderDisplayModel>(order);
        }

        /// <inheritdoc/>
        public IEnumerable<OrdersListModel> ListOrders(IListParameters parameters)
        {
            return ordersRepository.List(parameters).ProjectTo<OrdersListModel>(mapper.ConfigurationProvider);
        }

        /// <inheritdoc/>
        public async Task RemoveOrdersAsync(long[] orderIds)
        {
            var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                foreach (var order in orderIds)
                {
                    await ordersRepository.RemoveAsync(order);
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }
        }

        /// <inheritdoc/>
        public async Task<OrderDisplayModel> UpdateExistingOrderAsync(OrderUpdateModel orderModel)
        {
            if (await ordersRepository.FindByIdAsync(orderModel.ID) is not Orders order)
            {
                throw new EntityNotFoundException(nameof(Users), orderModel.ID);
            }

            await ordersRepository.UpdateAsync(mapper.Map(orderModel, order));
            await dbContext.SaveChangesAsync();

            return mapper.Map<OrderDisplayModel>(order);
        }
    }
}