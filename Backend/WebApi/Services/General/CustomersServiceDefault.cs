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
    /// <inheritdoc/>
    public class CustomersServiceDefault : ICustomersService
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ICustomersRepository customersRepository;

        /// <summary>
        /// Initializes a new instance of <see cref="CustomersServiceDefault"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="AppDbContext"/> instance.</param>
        /// <param name="mapper">The <see cref="IMapper"/> instance.</param>
        /// <param name="customersRepository">The <see cref="ICustomersRepository"/> instance.</param>
        public CustomersServiceDefault(
            AppDbContext dbContext,
            IMapper mapper,
            ICustomersRepository customersRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.customersRepository = customersRepository;
        }


        /// <inheritdoc/>
        public async Task<CustomerUpdateModel> FindCustomerByIdAsync(long customerId)
        {
            if (await customersRepository.FindByIdAsync(customerId) is not Customers customer) {
                throw new EntityNotFoundException(nameof(Users), customerId);
            }
            return mapper.Map<CustomerUpdateModel>(customer);
        }

        /// <inheritdoc/>
        public async Task<CustomerUpdateModel> InsertNewCustomerAsync(CustomerInsertModel customerModel)
        {
            var customer = mapper.Map<Customers>(customerModel);

            await customersRepository.AddAsync(customer);
            await dbContext.SaveChangesAsync();

            return mapper.Map<CustomerUpdateModel>(customer);
        }

        /// <inheritdoc/>
        public IEnumerable<CustomersListModel> ListCustomers(IListParameters parameters)
        {
            return customersRepository.List(parameters).ProjectTo<CustomersListModel>(mapper.ConfigurationProvider);
        }

        /// <inheritdoc/>
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
                throw;
            }
        }

        /// <inheritdoc/>
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