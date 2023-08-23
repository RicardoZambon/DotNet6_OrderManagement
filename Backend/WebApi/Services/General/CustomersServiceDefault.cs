using AutoMapper;
using AutoMapper.QueryableExtensions;
using Zambon.OrderManagement.Core;
using Zambon.OrderManagement.Core.BusinessEntities.General;
using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.Core.Repositories.General.Interfaces;
using Zambon.OrderManagement.WebApi.Models.General;
using Zambon.OrderManagement.WebApi.Services.General.Interfaces;

namespace Zambon.OrderManagement.WebApi.Services.General
{
    public class CustomersServiceDefault : ICustomersService
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ICustomersRepository customersRepository;

        public CustomersServiceDefault(
            AppDbContext dbContext,
            IMapper mapper,
            ICustomersRepository customersRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.customersRepository = customersRepository;
        }


        public async Task<CustomerUpdateModel> FindCustomerByIdAsync(long customerId)
        {
            return mapper.Map<CustomerUpdateModel>(await customersRepository.FindByIdAsync(customerId));
        }

        public async Task<CustomerUpdateModel> InsertNewCustomerAsync(CustomerInsertModel customerModel)
        {
            var customer = mapper.Map<Customers>(customerModel);

            await customersRepository.AddAsync(customer);
            await dbContext.SaveChangesAsync();

            return mapper.Map<CustomerUpdateModel>(customer);
        }

        public IEnumerable<CustomersListModel> ListCustomers(IListParameters parameters)
        {
            return customersRepository.List(parameters).ProjectTo<CustomersListModel>(mapper.ConfigurationProvider);
        }

        public async Task RemoveCustomersAsync(long[] customerIds)
        {
            var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                foreach (var customer in customerIds)
                {
                    await customersRepository.RemoveAsync(customer);
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }
        }

        public async Task<CustomerUpdateModel> UpdateExistingCustomerAsync(CustomerUpdateModel customerModel)
        {
            if (await customersRepository.FindByIdAsync(customerModel.ID) is not Customers customer)
            {
                throw new EntityNotFoundException(nameof(Users), customerModel.ID);
            }

            await customersRepository.UpdateAsync(mapper.Map(customerModel, customer));
            await dbContext.SaveChangesAsync();

            return mapper.Map<CustomerUpdateModel>(customer);
        }
    }
}