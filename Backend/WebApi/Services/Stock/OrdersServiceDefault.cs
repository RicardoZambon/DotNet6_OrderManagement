using AutoMapper;
using AutoMapper.QueryableExtensions;
using Zambon.OrderManagement.Core;
using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.Core.Repositories.Stock.Interfaces;
using Zambon.OrderManagement.WebApi.Models.Stock;
using Zambon.OrderManagement.WebApi.Services.Stock.Interfaces;

namespace Zambon.OrderManagement.WebApi.Services.Stock
{
    public class OrdersServiceDefault : IOrdersService
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IOrdersRepository ordersRepository;

        public OrdersServiceDefault(
            AppDbContext dbContext,
            IMapper mapper,
            IOrdersRepository ordersRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.ordersRepository = ordersRepository;
        }


        public async Task<OrderDisplayModel> FindOrderByIdAsync(long orderId)
        {
            return mapper.Map<OrderDisplayModel>(await ordersRepository.FindByIdAsync(orderId));
        }

        public async Task<OrderDisplayModel> InsertNewOrderAsync(OrderInsertModel orderModel)
        {
            var order = mapper.Map<Orders>(orderModel);

            await ordersRepository.AddAsync(order);
            await dbContext.SaveChangesAsync();

            return mapper.Map<OrderDisplayModel>(order);
        }

        public IEnumerable<OrdersListModel> ListOrders(IListParameters parameters)
        {
            return ordersRepository.List(parameters).ProjectTo<OrdersListModel>(mapper.ConfigurationProvider);
        }

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